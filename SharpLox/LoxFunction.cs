using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpLox
{
    public class LoxFunction : ILoxCallable
    {

        private readonly Stmt.Function declaration;

        public LoxFunction(Stmt.Function declaration)
        {
            this.declaration = declaration;
        }

        public int Arity()
        {
            return declaration.Parameters.Count;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new Environment(Interpreter.globals);
            for(var i = 0; i < declaration.Parameters.Count; i++)
            {
                environment.Define(declaration.Parameters[i].Lexeme, arguments[i]);
            }

            interpreter.ExecuteBlock(declaration.Body, environment);
            return null;
        }

        public override string ToString()
        {
            return $"<fn {declaration.Name.Lexeme}>";
        }
    }
}
