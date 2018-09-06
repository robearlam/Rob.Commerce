using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.BusinessUsers;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using System.Reflection;
using Foundation.PluginEnhancements.Engine.EntityViews;

namespace Foundation.PluginEnhancements.Engine
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config

                .ConfigurePipeline<IBizFxNavigationPipeline>(configure => configure
                    .Add<EnsureNavigationView>()
                )

                .ConfigurePipeline<IDoActionPipeline>(configure => configure
                    .Add<DoActionEnablePlugin>().After<ValidateEntityVersionBlock>()
                    .Add<DoActionDisablePlugin>().After<ValidateEntityVersionBlock>()
                )
            );

            services.RegisterAllCommands(assembly);
        }
    }
}