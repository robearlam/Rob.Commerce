using System;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.Pipelines
{
    [PipelineDisplayName("SendOrdertoServiceBusMinionPipeline")]
    public interface ISendOrdertoServiceBusMinionPipeline : IPipeline<String, Order, CommercePipelineExecutionContext>
    {
    }
}