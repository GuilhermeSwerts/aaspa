using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.CustonException
{
    public class ClienteException: Exception
    {
        public ClienteException() :base() { }
        public ClienteException(string message) : base(message) { }
        public ClienteException(string message, Exception inner) : base(message, inner) { }
        protected ClienteException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
