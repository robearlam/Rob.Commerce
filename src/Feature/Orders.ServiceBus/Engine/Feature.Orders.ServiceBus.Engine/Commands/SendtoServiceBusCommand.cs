using System;
using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Pipelines;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;

namespace Feature.Orders.ServiceBus.Engine.Commands
{
    public class SendtoServiceBusCommand : CommerceCommand
    {
        private readonly ISendtoServiceBusPipeline _pipeline;
        public string Orderid;

        public SendtoServiceBusCommand(ISendtoServiceBusPipeline pipeline, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _pipeline = pipeline;
        }

        public async Task<string> Process(CommerceContext commerceContext, string orderId)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                return await _pipeline.Run(orderId, new CommercePipelineExecutionContextOptions(commerceContext));
            }
        }
    }
}