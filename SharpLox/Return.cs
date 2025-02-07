using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpLox
{
    public class Return extends RuntimeException {
        public final Object value;

        public Return(Object value) {
            super(null, null, false, false);
            this.value = value;
        }
}
