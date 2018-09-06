using System.Reflection;
using Feature.ProductImport.Engine.EntityViews;
using Feature.ProductImport.Engine.Pipelines;
using Feature.ProductImport.Engine.Pipelines.Blocks;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.BusinessUsers;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using ImportCatalogsBlock = Feature.ProductImport.Engine.Pipelines.Blocks.ImportCatalogsBlock;
using ImportCategoriesBlock = Feature.ProductImport.Engine.Pipelines.Blocks.ImportCategoriesBlock;
using ImportSellableItemsBlock = Feature.ProductImport.Engine.Pipelines.Blocks.ImportSellableItemsBlock;
using RegisteredPluginBlock = Feature.ProductImport.Engine.Pipelines.Blocks.RegisteredPluginBlock;

namespace Feature.ProductImport.Engine
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IRunningPluginsPipeline>(c => { c.Add<RegisteredPluginBlock>().After<RunningPluginsBlock>(); })
                .ConfigurePipeline<IBizFxNavigationPipeline>(d => { d.Add<EnsureNavigationView>(); })
                .ConfigurePipeline<IGetEntityViewPipeline>(d => { d.Add<Dashboard>().Before<IFormatEntityViewPipeline>(); })

                .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure
                    .Add<Pipelines.Blocks.ConfigureServiceApiBlock>()
                )

                .AddPipeline<IImportCsvProductsPipeline, ImportCsvProductsPipeline>(configure => configure
                    .Add<ImportCsvProductsPrepareBlock>()
                    .Add<ImportCatalogsBlock>()
                    .Add<ImportCategoriesBlock>()
                    .Add<ImportInventorySetsBlocks>()
                    .Add<ImportSellableItemsBlock>()
                    .Add<ImportCsvProductsFinalizeBlock>()
                )

                .AddPipeline<IImportSingleCsvRowPipeline, ImportSingleCsvRowPipeline>(configure => configure
                    .Add<EnsureSellableItemExistsBlock>()
                    .Add<EnsureSellableItemIsInCategory>()
                    .Add<EnsureInventoryIsPopulatedBlock>()
                )
            );

            services.RegisterAllCommands(assembly);
        }
    }
}