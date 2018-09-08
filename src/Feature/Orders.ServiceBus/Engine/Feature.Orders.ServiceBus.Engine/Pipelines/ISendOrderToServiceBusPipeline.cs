using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.Pipelines
{
    [PipelineDisplayName("SendOrderToServiceBusPipeline")]
    public interface ISendOrderToServiceBusPipeline : IPipeline<string, Order, CommercePipelineExecutionContext>
    {
    }
}