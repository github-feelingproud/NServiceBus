// Disable obsolete warning until MessageEndpointMappings has been removed from config
#pragma warning disable CS0612, CS0619, CS0618
namespace NServiceBus.AcceptanceTesting.Support
{
    using System.Configuration;
    using Config;
    using Config.ConfigurationSource;
    using Customization;

    public class ScenarioConfigSource : IConfigurationSource
    {
        EndpointCustomizationConfiguration configuration;

        public ScenarioConfigSource(EndpointCustomizationConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public T GetConfiguration<T>() where T : class, new()
        {
            var type = typeof(T);

            if (type == typeof(UnicastBusConfig))
            {

                return new UnicastBusConfig
                {
                    MessageEndpointMappings = GenerateMappings()
                } as T;
            }

            return ConfigurationManager.GetSection(type.Name) as T;
        }

        MessageEndpointMappingCollection GenerateMappings()
        {
            var mappings = new MessageEndpointMappingCollection();

            foreach (var templateMapping in configuration.EndpointMappings)
            {
                var messageType = templateMapping.Key;
                var endpoint = templateMapping.Value;

                mappings.Add(new MessageEndpointMapping
                     {
                         AssemblyName = messageType.Assembly.FullName,
                         TypeFullName = messageType.FullName,
                         Endpoint = Conventions.EndpointNamingConvention(endpoint)
                     });
            }

            return mappings;
        }
    }
}
#pragma warning restore CS0612, CS0619, CS0618