using System;
using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("Feature.Order.ServiceBus.PutOrderonServiceBusListBlock")]
    public class PutOrderonServiceBusListBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        public string Status { get; set; }

        public override Task<Order> Run (Order order, CommercePipelineExecutionContext context)
        {
            Condition.Requires(order).IsNotNull($"{Name}: The argument can not be null");
            var pluginPolicy = context.GetPolicy<ServiceBusPluginPolicy>();
            var transientList = order.GetComponent<TransientListMembershipsComponent>();

            try
            {
                 transientList.Memberships.Add(pluginPolicy.ListToWatch);
            }
            catch (Exception ex)
            {
                Status = "Failure Reason: ";
                Status = Status += ex.Message;
                context.Abort(Status, context);
            }

            return Task.FromResult(order);
        }
    }
}