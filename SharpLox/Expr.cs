using System.Collections.Generic;

namespace SharpLox
{
    public abstract class Expr
    {
        public interface IExprVisitor<T>
        {
            T VisitBinaryExpr(Binary expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitUnaryExpr(Unary expr);
        }

        public abstract T Accept<T>(IExprVisitor<T> visitor);

        public class Binary : Expr
        {
            public Expr Left { get; }
            public Token Opr { get; }
            public Expr Right { get; }
            public Binary(Expr left, Token opr, Expr right)
            {
                this.Left = left;
                this.Opr = opr;
                this.Right = right;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }

        public class Grouping : Expr
        {
            public Expr Expression { get; }
            public Grouping(Expr expression)
            {
                this.Expression = expression;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }

        public class Literal : Expr
        {
            public Object Value { get; }
            public Literal(Object value)
            {
                this.Value = value;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }

        public class Unary : Expr
        {
            public Token Opr { get; }
            public Expr Right { get; }
            public Unary(Token opr, Expr right)
            {
                this.Opr = opr;
                this.Right = right;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }
    }
}
