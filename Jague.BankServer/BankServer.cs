using System;
using System.IO;
using System.Reflection;

using NLog;

using Jague.Common.Database;
using Jague.Common.Configuration;
using Jague.Common.Bank;
using Jague.Common.Network.Message;
using Jague.Common.Network;
using Jague.BankServer.Network;
using Jague.Common;

namespace Jague.BankServer
{
    internal static class BankServer
    {
#if DEBUG
        private const string Title = "Jague BankServer (DEBUG)";
#else
        private const string Title = "Jague BankServer (RELEASE)";
#endif
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            Console.Title = Title;
            log.Info("Initialising...");

            ConfigurationManager<BankServerConfiguration>.Instance.Initialise("AuthServer.json");

            DatabaseManager.Initialise(ConfigurationManager<BankServerConfiguration>.Instance.Config.Database);

            MessageManager.Instance.Initialise();
            NetworkManager<BankSession>.Instance.Initialise(ConfigurationManager<BankServerConfiguration>.Instance.Config.Network);

            BankManager.Instance.Initialise(lastTick =>
            {
                NetworkManager<BankSession>.Instance.Update(lastTick);
            });

            log.Info("Ready!");
        }
    }
}
