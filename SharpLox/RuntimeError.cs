using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpLox
{
    public class RuntimeError : Exception
    {
        public Token Token { get; }

        public RuntimeError(Token token, string message) : base(message)
        {
            Token = token;
        }
    }
}