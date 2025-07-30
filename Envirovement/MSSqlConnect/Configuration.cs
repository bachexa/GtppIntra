using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace MSSqlConnect
{
    internal class Configuration
    {
        private IConfiguration configuration;

        public Configuration()
        {
            // Create a Configuration object
            configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            // Create a Database instance using the configuration
           

            // Use the db instance
            // ...


        }
    }
}
