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
                    .Add<AddOrderToServiceBusSendListBlock>().Before<IPersistOrderPipeline>()
                )

                .AddPipeline<ISendOrderToServiceBusPipeline, SendOrderToServiceBusPipeline>(configuration => configuration
                    .Add<GetOrderBlock>()
                    .Add<SendOrdertoServiceBusBlock>()
                    .Add<AddOrderToServiceBusCompleteListBlock>()
                    .Add<RemoveOrderfromServiceBusSendListBlock>()
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
            );

            services.RegisterAllCommands(assembly);
        }
    }
}