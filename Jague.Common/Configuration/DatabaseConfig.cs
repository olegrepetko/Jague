using System;

namespace Jague.Common.Configuration
{
    public class DatabaseConfig : IDatabaseConfiguration
    {
        public DatabaseConnectionString ConnectionString { get; set; }

        IConnectionString IDatabaseConfiguration.GetConnectionString()
        {
            return ConnectionString;
        }
    }
}
