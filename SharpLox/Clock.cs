using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpLox
{
    public class Clock : ILoxCallable
    {
        public int Arity()
        {
            return 0;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return DateTime.Now.Second + DateTime.Now.Millisecond / 1000.0;
        }

        public override string ToString()
        {
            return "<native fn>";
        }
    }
}
