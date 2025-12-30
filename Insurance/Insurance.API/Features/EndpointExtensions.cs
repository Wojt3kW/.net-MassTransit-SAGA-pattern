using System.Reflection;

namespace Insurance.API.Features;

/// <summary>
/// Extension methods for automatic endpoint registration.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Discovers and maps all IEndpoint implementations in the assembly.
    /// </summary>
    public static void MapEndpoints(this WebApplication app)
    {
        var endpointTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in endpointTypes)
        {
            var endpoint = Activator.CreateInstance(type) as IEndpoint;
            endpoint?.MapEndpoint(app);
        }
    }
}
