using System;
using System.Text;
using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Policies;
using Microsoft.Azure.ServiceBus;
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
        public string ConnectionString { get; private set; }
        public string Status { get; set; }

        public override async Task<Order> Run(Order order, CommercePipelineExecutionContext context)
        {
            Condition.Requires(order).IsNotNull($"{Name}: The argument can not be null");

            var serviceBusPluginPolicy = context.GetPolicy<ServiceBusPluginPolicy>();
            ConnectionString = serviceBusPluginPolicy.EndPoint;
            var builder = new ServiceBusConnectionStringBuilder(ConnectionString);        
            var client = new QueueClient(builder);
            try
            {
                await client.SendAsync(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(order))));
                Status = "Success";
            }
            catch(Exception ex)
            {
                Status = $"Failure Reason: {ex.Message}";
            }
            return order;
        }
    }
}