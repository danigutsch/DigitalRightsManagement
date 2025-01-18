using DigitalRightsManagement.Infrastructure.Persistence;

namespace DigitalRightsManagement.AppHost;

public static class ResourceNames
{
    public const string DatabaseServer = "database-server";
    public const string Database = PersistenceDefaults.ConnectionStringName;
    public const string Api = "digitalrightsmanagement-api";
    public const string MigrationService = "digitalrightsmanagement-migrationservice";
}
