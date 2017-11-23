using System.Reflection;
using Feature.Compare.Engine.Pipelines;
using Feature.Compare.Engine.Pipelines.Blocks;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using ConfigureServiceApiBlock = Feature.Compare.Engine.Pipelines.Blocks.ConfigureServiceApiBlock;

namespace Feature.Compare.Engine
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config

                .AddPipeline<IAddToProductComparePipeline, AddToProductComparePipeline>(configure => configure
                    .Add<AddToProductCompareBlock>()
                )

                .AddPipeline<IRemoveFromProductComparePipeline, RemoveFromProductComparePipeline>(configure => configure
                    .Add<RemoveFromProductCompareBlock>()
                )

                .AddPipeline<IGetProductComparePipeline, GetProductComparePipeline>(configure => configure
                    .Add<GetProductCompareBlock>()
                )

                .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure
                    .Add<ConfigureServiceApiBlock>()
                )
            );

            services.RegisterAllCommands(assembly);
        }
    }
}
