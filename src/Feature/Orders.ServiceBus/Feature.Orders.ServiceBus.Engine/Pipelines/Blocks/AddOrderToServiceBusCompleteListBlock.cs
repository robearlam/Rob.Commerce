using System;
using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Policies;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Eventing;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("Feature.Order.ServiceBus.PutOrderonSenttoServiceBusList")]
    public class AddOrderToServiceBusCompleteListBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        private readonly IAddListEntitiesPipeline _addListEntitiesPipeline;
        private readonly IEventRegistry _eventRegistry;

        public AddOrderToServiceBusCompleteListBlock(IEventRegistry eventRegistry, IAddListEntitiesPipeline addListEntitiesPipeline)
        {
            _addListEntitiesPipeline = addListEntitiesPipeline;
            _eventRegistry = eventRegistry;
        }

        public override async Task<Order> Run(Order order, CommercePipelineExecutionContext context)
        {
            Condition.Requires(order).IsNotNull($"{Name}: The argument can not be null");

            var serviceBusOrderPlacedPolicy = context.GetPolicy<ServiceBusOrderPlacedPolicy>();
            if (!serviceBusOrderPlacedPolicy.Enabled)
            {
                context.Logger.LogInformation("Feature.Order.ServiceBus: Plugin is disabled - Order not added to Complete list");
                return order;
            }

            try
            {
                await _addListEntitiesPipeline.Run(new ListEntitiesArgument(new[] {order.Id}, serviceBusOrderPlacedPolicy.OrderSentListName), context);
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