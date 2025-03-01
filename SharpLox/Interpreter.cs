using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Numerics;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using static SharpLox.TokenType;

namespace SharpLox
{
    public class Interpreter : Expr.IExprVisitor<object>, Stmt.IStmtVisitor<Unit>
    {
        public static readonly Environment globals = new Environment();
        private Environment environment = globals;

        public Interpreter()
        {
            globals.Define("clock", new Clock());
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            switch (expr.Opr.Type)
            {
                case BANG_EQUAL:
                    return !IsEqual(left, right);
                case EQUAL_EQUAL:
                    return IsEqual(left, right);
                case GREATER:
                    CheckNumberOperands(expr.Opr, left, right);
                    return (double)left > (double)right;
                case GREATER_EQUAL:
                    CheckNumberOperands(expr.Opr, left, right);
                    return (double)left >= (double)right;
                case LESS:
                    CheckNumberOperands(expr.Opr, left, right);
                    return (double)left < (double)right;
                case LESS_EQUAL:
                    CheckNumberOperands(expr.Opr, left, right);
                    return (double)left <= (double)right;
                case MINUS:
                    CheckNumberOperands(expr.Opr, left, right);
                    return (double)left - (double)right;
                case PLUS:
                    if (left is double d1 && right is double d2)
                    {
                        return d1 + d2;
                    }
                    if (left is string s1 && right is string s2)
                    {
                        return s1 + s2;
                    }
                    throw new RuntimeError(expr.Opr, "Operands must be two numbers or two strings.");
                case SLASH:
                    CheckNumberOperands(expr.Opr, left, right);
                    return (double)left / (double)right;
                case STAR:
                    CheckNumberOperands(expr.Opr, left, right);
                    return (double)left * (double)right;
            }
            // Unreachable
            return null;
        }

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }

        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private string Stringify(object obj)
        {
            if (obj == null) return "nil";
            if (obj is double)
            {
                var text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }
            return obj.ToString();
        }

        private void CheckNumberOperands(Token opr, Object left, Object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(opr, "Operands must be numbers.");
        }
        private void CheckNumberOperand(Token opr, object right)
        {
            if (opr is double) return;
            throw new RuntimeError(opr, "Operand must be a number.");

        }

        private bool IsEqual(object left, object right)
        {
            if (left == null && right == null) return true;
            if (left == null) return false;

            return left.Equals(right);
        }

        public object VisitCallExpr(Expr.Call expr)
        {
            var callee = Evaluate(expr.Callee);

            var arguments = new List<object>();
            foreach (var argument in expr.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if(!(callee is ILoxCallable))
            {
                throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
            }

            var function = callee as ILoxCallable;
            if(arguments.Count != function.Arity())
            {
                throw new RuntimeError(expr.Paren, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
            }

            return function.Call(this, arguments);
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        private object Evaluate(Expr expression)
        {
            return expression.Accept(this);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            var right = Evaluate(expr.Right);
            switch (expr.Opr.Type)
            {
                case BANG:
                    return !IsTruthy(right);
                case MINUS:
                    CheckNumberOperand(expr.Opr, right);
                    return -(double)right;
            }
            // Unreachable
            return null;
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool b) return (bool)obj;
            return true;
        }

        Unit Stmt.IStmtVisitor<Unit>.VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.Xpression);
            return Unit.Void;
        }

        Unit Stmt.IStmtVisitor<Unit>.VisitPrintStmt(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.Xpression);
            Console.WriteLine(Stringify(value));
            return Unit.Void;
        }

        public object VisitVarExpr(Expr.Variable expr)
        {
            return environment.Get(expr.Name);
        }

        Unit Stmt.IStmtVisitor<Unit>.VisitVarStmt(Stmt.Var stmt)
        {
            object value = null;
            if (stmt.Initializer != null)
            {
                value = Evaluate(stmt.Initializer);
            }
            environment.Define(stmt.Name.Lexeme, value);
            return Unit.Void;
        }

        public object VisitAssignExpr(Expr.Assign expr)
        {
            var value = Evaluate(expr.Value);
            environment.Assign(expr.Name, value);
            return value;
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            return environment.Get(expr.Name);
        }

        Unit Stmt.IStmtVisitor<Unit>.VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Environment(environment));
            return Unit.Void;
        }

        private void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            var previous = this.environment;
            try
            {
                this.environment = environment;
                foreach (var stmt in statements)
                {
                    Execute(stmt);
                }
            }
            finally
            {
                this.environment = previous;
            }
        }

        Unit Stmt.IStmtVisitor<Unit>.VisitIfStmt(Stmt.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null)
            {
                Execute(stmt.ElseBranch);
            }
            return Unit.Void;
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            var left = Evaluate(expr.Left);

            if (expr.Opr.Type == TokenType.OR)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.Right);
        }

        Unit Stmt.IStmtVisitor<Unit>.VisitWhileStmt(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.Body);
            }

            return Unit.Void;
        }

        Unit Stmt.IStmtVisitor<Unit>.VisitFunctionStmt(Stmt.Function stmt)
        {
            var function = new LoxFunction(stmt);
            environment.Define(stmt.Name.Lexeme, function);
            return Unit.Void;
        }

        Unit Stmt.IStmtVisitor<Unit>.VisitReturnStmt(Stmt.Return stmt)
        {
            object value = null;
            if (stmt.Value != null) value = Evaluate(stmt.Value);

            throw new Return(value);
        }
    }
}
