namespace Ocelot.Package.Archetype
{
    using DependencyInjection;

    public static class OcelotBuilderExtensions
    {
        public static IOcelotBuilder AddSomething(this IOcelotBuilder builder)
        {
            return builder;
        }
    }
}
