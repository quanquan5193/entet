using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.Common.Exceptions
{
    public class DataExistedException : Exception
    {
        public DataExistedException(string message)
            : base(message)
        {
        }
    }
}
