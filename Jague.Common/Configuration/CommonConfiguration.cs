using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Jague.Common.Configuration
{
    public static class CommonConfiguration
    {
        public static IConfiguration Configuration { get; private set; }

        public static void Initialise(string file)
        {
            if (Configuration != null)
                return;

            var builder = new ConfigurationBuilder()
                .AddJsonFile(file, false, true)
                .AddEnvironmentVariables()
                .AddCommandLine(Environment.GetCommandLineArgs().Skip(1).ToArray());

            Configuration = builder.Build();
        }
    }
}
