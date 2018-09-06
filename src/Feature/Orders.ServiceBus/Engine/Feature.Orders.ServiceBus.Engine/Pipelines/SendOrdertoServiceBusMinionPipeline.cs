using System;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.Pipelines
{
    public class SendOrdertoServiceBusMinionPipeline : CommercePipeline<String, Order>, ISendOrdertoServiceBusMinionPipeline
    {
        public SendOrdertoServiceBusMinionPipeline(IPipelineConfiguration<ISendOrdertoServiceBusMinionPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}