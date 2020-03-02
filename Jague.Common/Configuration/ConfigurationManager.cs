using Microsoft.Extensions.Configuration;

namespace Jague.Common.Configuration
{
    public sealed class ConfigurationManager<T> : Singleton<ConfigurationManager<T>>
    {
        public T Config { get; private set; }

        private ConfigurationManager()
        {
        }

        public void Initialise(string file)
        {
            CommonConfiguration.Initialise(file);
            Config = CommonConfiguration.Configuration.Get<T>();
        }
    }
}
