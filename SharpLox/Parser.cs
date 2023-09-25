using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static SharpLox.TokenType;

namespace SharpLox
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }
            return statements;
        }

        private Stmt Declaration()
        {
            try
            {
                if (Match(VAR)) return VarDeclaration();
                return Statement();
            }
            catch (ParseError error)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt VarDeclaration()
        {
            var name = Consume(IDENTIFIER, "Expect variable name.");

            Expr initializer = null;
            if (Match(EQUAL))
            {
                initializer = Expression();
            }

            Consume(SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }

        private Stmt Statement()
        {
            if (Match(IF)) return ifStatement();
            if (Match(PRINT)) return PrintStatement();
            if (Match(LEFT_BRACE)) return new Stmt.Block(Block());
            return ExpressionStatement();
        }

        private Stmt ifStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'if'.");
            var condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after if condition.");

            var thenBranch = Statement();
            Stmt elseBranch = null;
            if (Match(ELSE))
            {
                elseBranch = Statement();
            }

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        private List<Stmt> Block()
        {
            List<Stmt> statements = new();

            while (!Check(RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        private Stmt ExpressionStatement()
        {
            var value = Expression();
            Consume(SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
        }

        private Stmt PrintStatement()
        {
            throw new NotImplementedException();
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            var expr = Or();

            if (Match(EQUAL))
            {
                var equals = Previous();
                Expr value = Assignment();

                if (expr is Expr.Variable)
                {
                    var name = ((Expr.Variable)expr).Name;
                    return new Expr.Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr Or()
        {
            var expr = And();
            while (Match(OR))
            {
                var opr = Previous();
                var right = And();
                expr = new Expr.Logical(expr, opr, right);
            }

            return expr;
        }

        private Expr And()
        {
            var expr = Equality();

            while (Match(AND))
            {
                var opr = Previous();
                var right = Equality();
                expr = new Expr.Logical(expr, opr, right);
            }

            return expr;
        }

        private Expr Equality()
        {
            var expr = Comparison();
            while (Match(BANG_EQUAL, EQUAL_EQUAL))
            {
                var op = Previous();
                var right = Comparison();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private Token Advance()
        {
            if (!IsAtEnd())
            {
                current++;
            }
            return Previous();
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd())
            {
                return false;
            }
            return Peek().Type == type;
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private bool IsAtEnd()
        {
            return Peek().Type == EOF;
        }

        private Expr Comparison()
        {
            var expr = Term();
            while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
            {
                var op = Previous();
                var right = Term();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        private Expr Term()
        {
            var expr = Factor();
            while (Match(MINUS, PLUS))
            {
                var op = Previous();
                var right = Factor();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        private Expr Factor()
        {
            var expr = Unary();
            while (Match(SLASH, STAR))
            {
                var op = Previous();
                var right = Unary();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;

        }

        private Expr Unary()
        {
            if (Match(BANG, MINUS))
            {
                var op = Previous();
                var right = Unary();
                return new Expr.Unary(op, right);
            }
            return Primary();
        }

        private Expr Primary()
        {
            if (Match(FALSE))
            {
                return new Expr.Literal(false);
            }
            if (Match(TRUE))
            {
                return new Expr.Literal(true);
            }
            if (Match(NIL))
            {
                return new Expr.Literal(null);
            }

            if (Match(NUMBER, STRING))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(IDENTIFIER))
            {
                return new Expr.Variable(Previous());
            }

            if (Match(LEFT_PAREN))
            {
                var expr = Expression();
                Consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        private void Synchronize()
        {
            Advance();
            while (!IsAtEnd())
            {
                if (Previous().Type == SEMICOLON) return;
                switch (Peek().Type)
                {
                    case CLASS:
                    case FUN:
                    case VAR:
                    case FOR:
                    case IF:
                    case WHILE:
                    case PRINT:
                    case RETURN:
                        return;
                }
                Advance();
            }
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw Error(Peek(), message);
        }

        private ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }
    }
}