using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.BusinessUsers;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using System.Reflection;
using Feature.Orders.ServiceBus.Engine.EntityViews;
using Feature.Orders.ServiceBus.Engine.Pipelines;
using Feature.Orders.ServiceBus.Engine.Pipelines.Blocks;

namespace Feature.Orders.ServiceBus.Engine
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config

                .ConfigurePipeline<IBizFxNavigationPipeline>(configuration => configuration
                    .Add<EnsureNavigationView>()
                )

                .ConfigurePipeline<ICreateOrderPipeline>(configuration => configuration
                    .Add<PutOrderonServiceBusListBlock>().Before<IPersistOrderPipeline>()
                )

                .AddPipeline<ISendtoServiceBusPipeline, SendtoServiceBusPipeline>(configuration => configuration
                    .Add<GetOrderBlock>()
                    .Add<PostmanSendOrdertoServiceBusBlock>()
                )

                .AddPipeline<ISendOrdertoServiceBusMinionPipeline, SendOrdertoServiceBusMinionPipeline>(configuration => configuration
                    .Add<GetOrderBlock>()
                    .Add<SendOrdertoServiceBusBlock>()
                    .Add<PutOrderonSenttoServiceBusListBlock>()
                    .Add<RemoveOrderfromServiceBusListBlock>()
                    .Add<PersistOrderBlock>()
                )

                .ConfigurePipeline<IGetEntityViewPipeline>(configuration => configuration
                    .Add<Dashboard>().Before<IFormatEntityViewPipeline>()
                    .Add<EntityViewEnsureServiceBus>().Before<IFormatEntityViewPipeline>()
                )

                .ConfigurePipeline<IFormatEntityViewPipeline>(configuration => configuration
                    .Add<EnsureActions>().After<PopulateEntityViewActionsBlock>()
                    .Add<EnsurePluginActions>().After<PopulateEntityViewActionsBlock>()
                )

                .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<Pipelines.Blocks.ConfigureServiceApiBlock>())
                
            );

            services.RegisterAllCommands(assembly);
        }
    }
}