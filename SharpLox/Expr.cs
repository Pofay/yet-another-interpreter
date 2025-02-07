using System.Collections.Generic;

namespace SharpLox {

    public abstract class Expr {

        public interface IExprVisitor<T> {
            T VisitAssignExpr(Assign expr);
            T VisitBinaryExpr(Binary expr);
            T VisitCallExpr(Call expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitLogicalExpr(Logical expr);
            T VisitUnaryExpr(Unary expr);
            T VisitVariableExpr(Variable expr);
        }

      public abstract T Accept<T>(IExprVisitor<T> visitor);

        public class Assign : Expr {
            public Token Name { get; }
            public Expr Value { get; }
           public Assign(Token name, Expr value) {
                this.Name = name;
                this.Value = value;
            }

         public override T Accept<T>(IExprVisitor<T> visitor) {
            return visitor.VisitAssignExpr(this);
         }
      }

        public class Binary : Expr {
            public Expr Left { get; }
            public Token Opr { get; }
            public Expr Right { get; }
           public Binary(Expr left, Token opr, Expr right) {
                this.Left = left;
                this.Opr = opr;
                this.Right = right;
            }

         public override T Accept<T>(IExprVisitor<T> visitor) {
            return visitor.VisitBinaryExpr(this);
         }
      }

        public class Call : Expr {
            public Expr Callee { get; }
            public Token Paren { get; }
            public List<Expr> Arguments { get; }
           public Call(Expr callee, Token paren, List<Expr> arguments) {
                this.Callee = callee;
                this.Paren = paren;
                this.Arguments = arguments;
            }

         public override T Accept<T>(IExprVisitor<T> visitor) {
            return visitor.VisitCallExpr(this);
         }
      }

        public class Grouping : Expr {
            public Expr Expression { get; }
           public Grouping(Expr expression) {
                this.Expression = expression;
            }

         public override T Accept<T>(IExprVisitor<T> visitor) {
            return visitor.VisitGroupingExpr(this);
         }
      }

        public class Literal : Expr {
            public Object Value { get; }
           public Literal(Object value) {
                this.Value = value;
            }

         public override T Accept<T>(IExprVisitor<T> visitor) {
            return visitor.VisitLiteralExpr(this);
         }
      }

        public class Logical : Expr {
            public Expr Left { get; }
            public Token Opr { get; }
            public Expr Right { get; }
           public Logical(Expr left, Token opr, Expr right) {
                this.Left = left;
                this.Opr = opr;
                this.Right = right;
            }

         public override T Accept<T>(IExprVisitor<T> visitor) {
            return visitor.VisitLogicalExpr(this);
         }
      }

        public class Unary : Expr {
            public Token Opr { get; }
            public Expr Right { get; }
           public Unary(Token opr, Expr right) {
                this.Opr = opr;
                this.Right = right;
            }

         public override T Accept<T>(IExprVisitor<T> visitor) {
            return visitor.VisitUnaryExpr(this);
         }
      }

        public class Variable : Expr {
            public Token Name { get; }
           public Variable(Token name) {
                this.Name = name;
            }

         public override T Accept<T>(IExprVisitor<T> visitor) {
            return visitor.VisitVariableExpr(this);
         }
      }
}
}
