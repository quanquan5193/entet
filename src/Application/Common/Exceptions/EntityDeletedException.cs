using System;

namespace mrs.Application.Common.Exceptions
{
    public class EntityDeletedException : Exception
    {
        public EntityDeletedException()
            : base()
        {
        }

        public EntityDeletedException(string message)
            : base(message)
        {
        }

        public EntityDeletedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public EntityDeletedException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
}
