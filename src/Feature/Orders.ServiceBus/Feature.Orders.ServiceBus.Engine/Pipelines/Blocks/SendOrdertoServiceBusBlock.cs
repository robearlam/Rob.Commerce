using System;
using System.Text;
using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Policies;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("Feature.Order.ServiceBus.SendtoServiceBusBlock")]
    public class SendOrdertoServiceBusBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        public override async Task<Order> Run(Order order, CommercePipelineExecutionContext context)
        {
            Condition.Requires(order).IsNotNull($"{Name}: The argument can not be null");

            var serviceBusOrderPlacedPolicy = context.GetPolicy<ServiceBusOrderPlacedPolicy>();
            var serviceBusConnectionPolicy = context.GetPolicy<ServiceBusConnectionPolicy>();
            var builder = new ServiceBusConnectionStringBuilder(serviceBusConnectionPolicy.EndPoint);        
            var client = new QueueClient(builder);

            if (!serviceBusOrderPlacedPolicy.Enabled)
            {
                context.Logger.LogInformation("Feature.Order.ServiceBus: Plugin is disabled - message not sent to Service Bus");
                return order;
            }

            try
            {
                await client.SendAsync(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(order))));
                context.Logger.LogInformation($"Feature.Order.ServiceBus: Order{order.Id} send to Service Bus");
            }
            catch(Exception ex)
            {
                context.Logger.LogError($"Feature.Order.ServiceBus: {ex.Message} {ex.StackTrace}");
            }

            return order;
        }
    }
}