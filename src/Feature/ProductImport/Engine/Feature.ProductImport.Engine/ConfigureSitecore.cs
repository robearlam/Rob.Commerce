using System.Reflection;
using Feature.ProductImport.Engine.EntityViews;
using Feature.ProductImport.Engine.Pipelines.Blocks;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.BusinessUsers;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using RegisteredPluginBlock = Feature.ProductImport.Engine.Pipelines.Blocks.RegisteredPluginBlock;

namespace Feature.ProductImport.Engine
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllCommands(assembly);

            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IRunningPluginsPipeline>(c => { c.Add<RegisteredPluginBlock>().After<RunningPluginsBlock>(); }));
                //.ConfigurePipeline<IBizFxNavigationPipeline>(d => { d.Add<EnsureNavigationView>(); })
                //.ConfigurePipeline<IGetEntityViewPipeline>(d => { d.Add<Dashboard>().Before<IFormatEntityViewPipeline>(); }));            
        }
    }
}