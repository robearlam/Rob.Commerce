using System;
using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Policies;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Eventing;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("Feature.Order.ServiceBus.RemoveOrderfromServiceBusSendListBlock")]
    public class RemoveOrderfromServiceBusSendListBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        private readonly IRemoveListEntitiesPipeline _removeListEntitiesPipeline;
        private readonly IEventRegistry _eventRegistry;

        public RemoveOrderfromServiceBusSendListBlock(IEventRegistry eventRegistry, IRemoveListEntitiesPipeline removeListEntitiesPipeline)
        {
            _removeListEntitiesPipeline = removeListEntitiesPipeline;
            _eventRegistry = eventRegistry;
        }

        public override async Task<Order> Run(Order order, CommercePipelineExecutionContext context)
        {
            Condition.Requires(order).IsNotNull($"{Name}: The argument can not be null");

            var orderPlacedPolicy = context.GetPolicy<ServiceBusOrderPlacedPolicy>();
            order.GetComponent<TransientListMembershipsComponent>();
            if (!orderPlacedPolicy.Enabled)
            {
                context.Logger.LogInformation("Feature.Order.ServiceBus: Plugin is disabled - Order not added to send list.");
                return order;
            }

            try
            {
                await _removeListEntitiesPipeline.Run(new ListEntitiesArgument(new[] { order.Id }, orderPlacedPolicy.OrderPlacedListName), context);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Feature.Order.ServiceBus: {ex.Message} {ex.StackTrace}");
            }

            await _eventRegistry.ListItemUpdated().Send(order, Name);
            return order;
        }
    }
}