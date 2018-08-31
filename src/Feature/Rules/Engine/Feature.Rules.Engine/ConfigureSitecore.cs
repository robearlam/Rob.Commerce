using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Rules;

namespace Feature.Rules.Engine
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.Sitecore().Rules(config => config.Registry(registry => registry.RegisterAssembly(assembly)));
        }
    }
}