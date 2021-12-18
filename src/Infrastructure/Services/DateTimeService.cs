using mrs.Application.Common.Interfaces;
using System;

namespace mrs.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
