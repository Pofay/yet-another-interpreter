using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using static SharpLox.TokenType;

namespace SharpLox
{
    public class Interpreter : Expr.IExprVisitor<object>
    {
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

        public void Interpret(Expr expression)
        {
            try
            {
                var value = Evaluate(expression);
                Console.Out.WriteLine(Stringify(value));
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }

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
    }

}