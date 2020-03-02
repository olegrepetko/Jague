using Jague.Common.Configuration;

namespace Jague.BankServer
{
    class BankServerConfiguration
    {
        public NetworkConfig Network { get; set; }
        public DatabaseConfig Database { get; set; }
    }
}
