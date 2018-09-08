using System;
using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Eventing;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("Feature.Order.ServiceBus.RemoveOrderfromServiceBusListBlock")]
    public class RemoveOrderfromServiceBusListBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        public string Status { get; set; }
        private readonly IRemoveListEntitiesPipeline _removeListEntitiesPipeline;
        private readonly IEventRegistry _eventRegistry;
        public RemoveOrderfromServiceBusListBlock(IEventRegistry eventRegistry, IRemoveListEntitiesPipeline removeListEntitiesPipeline)
        {
            _removeListEntitiesPipeline = removeListEntitiesPipeline;
            _eventRegistry = eventRegistry;
        }

        public override async Task<Order> Run(Order order, CommercePipelineExecutionContext context)
        {
            Condition.Requires(order).IsNotNull($"{this.Name}: The argument can not be null");
            var pluginPolicy = context.GetPolicy<ServiceBusOrderPlacedPolicy>();
            order.GetComponent<TransientListMembershipsComponent>();
  
            try
            {
                await _removeListEntitiesPipeline.Run(new ListEntitiesArgument(new[] { order.Id }, pluginPolicy.OrderPlacedListName), context);
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