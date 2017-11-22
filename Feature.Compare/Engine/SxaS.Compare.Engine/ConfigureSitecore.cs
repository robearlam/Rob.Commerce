using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using SxaS.Compare.Engine.Pipelines;
using SxaS.Compare.Engine.Pipelines.Blocks;

namespace SxaS.Compare.Engine
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
                    .Add<Pipelines.Blocks.ConfigureServiceApiBlock>()
                )
            );

            services.RegisterAllCommands(assembly);
        }
    }
}
