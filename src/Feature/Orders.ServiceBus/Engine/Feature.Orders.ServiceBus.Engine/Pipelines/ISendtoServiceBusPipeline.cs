using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.Pipelines
{
    [PipelineDisplayName("SendtoServiceBusPipeline")]
    public interface ISendtoServiceBusPipeline : IPipeline<string, string, CommercePipelineExecutionContext>
    {
    }
}