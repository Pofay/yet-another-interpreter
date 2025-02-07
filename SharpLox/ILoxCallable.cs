using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpLox
{
    public interface ILoxCallable
    {
        int Arity();
        object Call(Interpreter interpreter, List<object> arguments);
    }
}
