using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.Pipelines
{
    public class SendtoServiceBusPipeline : CommercePipeline<string, string>, ISendtoServiceBusPipeline
    {
        public SendtoServiceBusPipeline(IPipelineConfiguration<ISendtoServiceBusPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}