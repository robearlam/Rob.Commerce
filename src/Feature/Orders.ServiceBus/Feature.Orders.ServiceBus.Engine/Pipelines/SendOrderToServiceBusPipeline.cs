using System;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.Pipelines
{
    public class SendOrderToServiceBusPipeline : CommercePipeline<String, Order>, ISendOrderToServiceBusPipeline
    {
        public SendOrderToServiceBusPipeline(IPipelineConfiguration<ISendOrderToServiceBusPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}