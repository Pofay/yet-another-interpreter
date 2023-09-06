using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static SharpLox.TokenType;

namespace SharpLox
{
    public class Interpreter : Expr.IExprVisitor<object>
    {
        public object VisitBinaryExpr(Expr.Binary expr)
        {
            // var left = 
            return null;
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
                    return -(double)right;
            }
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