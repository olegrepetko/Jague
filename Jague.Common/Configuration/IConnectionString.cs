namespace Jague.Common.Configuration
{
    public interface IConnectionString
    {
        DatabaseProvider Provider { get; }
        string ConnectionString { get; }

    }
}
