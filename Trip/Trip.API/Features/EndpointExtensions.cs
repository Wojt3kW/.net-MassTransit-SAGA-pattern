using System.Reflection;

namespace Trip.API.Features;

public static class EndpointExtensions
{
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
