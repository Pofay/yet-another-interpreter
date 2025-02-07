using System.Collections.Generic;

namespace SharpLox {

    public abstract class Stmt {

        public interface IStmtVisitor<T> {
            T VisitBlockStmt(Block stmt);
            T VisitExpressionStmt(Expression stmt);
            T VisitFunctionStmt(Function stmt);
            T VisitIfStmt(If stmt);
            T VisitPrintStmt(Print stmt);
            T VisitReturnStmt(Return stmt);
            T VisitVarStmt(Var stmt);
            T VisitWhileStmt(While stmt);
        }

      public abstract T Accept<T>(IStmtVisitor<T> visitor);

        public class Block : Stmt {
            public List<Stmt> Statements { get; }
           public Block(List<Stmt> statements) {
                this.Statements = statements;
            }

         public override T Accept<T>(IStmtVisitor<T> visitor) {
            return visitor.VisitBlockStmt(this);
         }
      }

        public class Expression : Stmt {
            public Expr Xpression { get; }
           public Expression(Expr xpression) {
                this.Xpression = xpression;
            }

         public override T Accept<T>(IStmtVisitor<T> visitor) {
            return visitor.VisitExpressionStmt(this);
         }
      }

        public class Function : Stmt {
            public Token Name { get; }
            public List<Token> Parameters { get; }
            public List<Stmt> Body { get; }
           public Function(Token name, List<Token> parameters, List<Stmt> body) {
                this.Name = name;
                this.Parameters = parameters;
                this.Body = body;
            }

         public override T Accept<T>(IStmtVisitor<T> visitor) {
            return visitor.VisitFunctionStmt(this);
         }
      }

        public class If : Stmt {
            public Expr Condition { get; }
            public Stmt ThenBranch { get; }
            public Stmt ElseBranch { get; }
           public If(Expr condition, Stmt thenBranch, Stmt elseBranch) {
                this.Condition = condition;
                this.ThenBranch = thenBranch;
                this.ElseBranch = elseBranch;
            }

         public override T Accept<T>(IStmtVisitor<T> visitor) {
            return visitor.VisitIfStmt(this);
         }
      }

        public class Print : Stmt {
            public Expr Xpression { get; }
           public Print(Expr xpression) {
                this.Xpression = xpression;
            }

         public override T Accept<T>(IStmtVisitor<T> visitor) {
            return visitor.VisitPrintStmt(this);
         }
      }

        public class Var : Stmt {
            public Token Name { get; }
            public Expr Initializer { get; }
           public Var(Token name, Expr initializer) {
                this.Name = name;
                this.Initializer = initializer;
            }

         public override T Accept<T>(IStmtVisitor<T> visitor) {
            return visitor.VisitVarStmt(this);
         }
      }

        public class While : Stmt {
            public Expr Condition { get; }
            public Stmt Body { get; }
           public While(Expr condition, Stmt body) {
                this.Condition = condition;
                this.Body = body;
            }

         public override T Accept<T>(IStmtVisitor<T> visitor) {
            return visitor.VisitWhileStmt(this);
         }
      }

        public class Return : Stmt
        {
            public Token Keyword { get; }
            public Expr? Value { get; }

            public Return(Token keyword, Expr? value)
            {
                this.Keyword = keyword;
                this.Value = value;
            }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitReturnStmt(this);
            }
        }
    }
}
