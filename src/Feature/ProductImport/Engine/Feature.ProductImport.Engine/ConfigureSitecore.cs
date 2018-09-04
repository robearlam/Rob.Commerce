﻿using System.Reflection;
using Feature.ProductImport.Engine.Mappers;
using Feature.ProductImport.Engine.Pipelines;
using Feature.ProductImport.Engine.Pipelines.Blocks;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
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

                .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure
                    .Add<Pipelines.Blocks.ConfigureServiceApiBlock>()
                )

                .AddPipeline<IImportCsvProductsPipeline, ImportCsvProductsPipeline>(configure => configure
                    .Add<ImportCsvProductsPrepareBlock>()
                    .Add<ImportCatalogsBlock>()
                    .Add<ImportCategoriesBlock>()
                    .Add<ImportSellableItemsBlock>()
                    .Add<ImportCsvProductsFinalizeBlock>()
                )

                .AddPipeline<IImportSingleCsvRowPipeline, ImportSingleCsvRowPipeline>(configure => configure
                    .Add<EnsureSellableItemExistsBlock>()
                )
            );

            services.AddTransient<ISellableItemMapper, SellableItemMapper>();
            services.RegisterAllCommands(assembly);
        }
    }
}