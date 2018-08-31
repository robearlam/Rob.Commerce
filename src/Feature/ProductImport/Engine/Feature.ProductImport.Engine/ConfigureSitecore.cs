using Feature.ProductImport.Engine.Pipelines.Blocks;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;

namespace Feature.ProductImport.Engine
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IRunningPluginsPipeline>(c => { c.Add<RegisteredPluginBlock>().After<RunningPluginsBlock>(); }));
        }
    }
}