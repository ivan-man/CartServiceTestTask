using Common.Interfaces;
using System;

namespace Common
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now() => DateTime.Now;
    }
}
