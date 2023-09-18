using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;

namespace SharpLox
{
    public class Environment
    {
        private readonly Dictionary<string, object> values = new();

        void Define(String name, Object value)
        {
            values[name] = value;
        }

        object Get(Token name)
        {
            if(values.ContainsKey(name.Lexeme))
            {
                return values[name.Lexeme];
            }
            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
        }
    }
}