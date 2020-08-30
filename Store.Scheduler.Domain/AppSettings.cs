using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Scheduler.Domain
{
    public class AppSettings
    {
        public IConfigurationRoot Configuration { get; set; }

        public string CartServiceConnectionString { get; set; }
        public string HangfireConnectionString { get; set; }
    }
}
