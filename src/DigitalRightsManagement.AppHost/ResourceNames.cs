using DigitalRightsManagement.Infrastructure.Persistence;

namespace DigitalRightsManagement.AppHost;

public static class ResourceNames
{
    public static string DatabaseServer => "database-server";
    public static string Database => PersistenceDefaults.ConnectionStringName;
    public static string Api => "digitalrightsmanagement-api";
    public static string MigrationService => "digitalrightsmanagement-migrationservice";
}
