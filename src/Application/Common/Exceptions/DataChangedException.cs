using System;

namespace mrs.Application.Common.Exceptions
{
    public class DataChangedException : Exception
    {
        public DataChangedException()
            : base()
        {
        }

        public DataChangedException(string message)
            : base(message)
        {
        }

        public DataChangedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DataChangedException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
}
