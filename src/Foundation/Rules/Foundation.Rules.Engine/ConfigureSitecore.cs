using System.Reflection;
using Foundation.Rules.Engine.Pipelines.Blocks;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using Sitecore.Framework.Rules;

namespace Foundation.Rules.Engine
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.Sitecore().Rules(config => config.Registry(registry => registry.RegisterAssembly(assembly)));

            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IRunningPluginsPipeline>(c => { c.Add<RegisteredPluginBlock>().After<RunningPluginsBlock>(); }));
        }
    }
}