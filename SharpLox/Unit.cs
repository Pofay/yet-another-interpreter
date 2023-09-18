using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpLox
{
    internal class Unit
    {
        public static Unit Void => null;
        private Unit()
        {
            throw new InvalidOperationException("Cannot instantiate Unit type");
        }
    }
}