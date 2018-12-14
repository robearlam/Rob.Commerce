using System.Reflection;
using Feature.Initialization.Engine.Pipelines.Blocks;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;

namespace Feature.Initialization.Engine
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IInitializeEnvironmentPipeline>(d =>
                {
                    d.Add<InitializeCatalogBlock>()
                    .Add<InitializeInventoryBlock>()
                    .Add<InitializeEnvironmentPricingBlock>()
                    .Add<InitializeEnvironmentPromotionsBlock>();
                })
                .ConfigurePipeline<IRunningPluginsPipeline>(c =>
                {
                    c.Add<RegisteredPluginBlock>().After<RunningPluginsBlock>();
                })
            );
        }
    }
}