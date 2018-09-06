using System;
using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Eventing;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("Feature.Order.ServiceBus.PutOrderonSenttoServiceBusList")]
    public class PutOrderonSenttoServiceBusListBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        public string Status { get; set; }
        private readonly IAddListEntitiesPipeline _addListEntitiesPipeline;
        private readonly IEventRegistry _eventRegistry;

        public PutOrderonSenttoServiceBusListBlock(IEventRegistry eventRegistry,
            IAddListEntitiesPipeline addListEntitiesPipeline)
        {
            _addListEntitiesPipeline = addListEntitiesPipeline;
            _eventRegistry = eventRegistry;
        }

        public override async Task<Order> Run(Order order, CommercePipelineExecutionContext context)
        {
            Condition.Requires(order).IsNotNull($"{Name}: The argument can not be null");
            var pluginPolicy = context.GetPolicy<ServiceBusPluginPolicy>();

            try
            {
                await _addListEntitiesPipeline.Run(new ListEntitiesArgument(new[] {order.Id}, pluginPolicy.SentOrdersList), context);
            }
            catch (Exception ex)
            {
                Status = "Failure Reason: ";
                Status = Status += ex.Message;
                context.Abort(Status, context);
            }

            await _eventRegistry.ListItemUpdated().Send(order, Name);
            return order;
        }
    }
}