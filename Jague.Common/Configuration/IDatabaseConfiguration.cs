namespace Jague.Common.Configuration
{
    public interface IDatabaseConfiguration
    {
        IConnectionString GetConnectionString();
    }
}
