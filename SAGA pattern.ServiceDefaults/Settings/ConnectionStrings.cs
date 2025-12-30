namespace SAGA_pattern.ServiceDefaults.Settings
{
    /// <summary>
    /// Holds validated connection strings for SQL Server and RabbitMq.
    /// </summary>
    public record ConnectionStrings(string SqlServer, string RabbitMq);
}
