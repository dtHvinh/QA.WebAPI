namespace WebAPI.DocumentDb;

public class ConnectionSettingsBase
{
    public ConnectionSettingsBase(string connectionString, string databaseName)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        ArgumentNullException.ThrowIfNull(databaseName);

        ConnectionString = connectionString;
        DatabaseName = databaseName;
    }

    public string ConnectionString { get; init; }
    public string DatabaseName { get; init; }
}
