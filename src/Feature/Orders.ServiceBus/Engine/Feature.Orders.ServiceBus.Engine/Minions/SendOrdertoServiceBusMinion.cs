using System;
using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;

namespace Feature.Orders.ServiceBus.Engine.Minions
{
    public class SendOrdertoServiceBusMinion : Minion
    {
        protected SendOrderToServiceBusPipeline Pipeline { get; set; }

        public override void Initialize(IServiceProvider serviceProvider, ILogger logger, MinionPolicy policy, CommerceEnvironment environment, CommerceContext globalContext)
        {
            base.Initialize(serviceProvider, logger, policy, environment, globalContext);

            Pipeline = serviceProvider.GetService<SendOrderToServiceBusPipeline>();
        }

        public override async Task<MinionRunResultsModel> Run()
        {
            var runResults = new MinionRunResultsModel();

            var listCount = await GetListCount(Policy.ListToWatch);       
            Logger.LogInformation($"{Name}-Review List {Policy.ListToWatch}: Count:{listCount}");
            
            if (listCount > 0)
            {
                var sendOrdertoServiceBus = await GetListIds<Order>(Policy.ListToWatch, System.Convert.ToInt32(listCount));

                foreach (var id in sendOrdertoServiceBus.IdList)
                {
                    Logger.LogDebug($"{Name}-Sending Order to ServiceBus: {id}");
                    await Pipeline.Run(id, new CommercePipelineExecutionContextOptions(new CommerceContext(Logger, MinionContext.TelemetryClient) { Environment = Environment }));
                }

                listCount = await GetListCount(Policy.ListToWatch);
            }

            Logger.LogInformation($"{Name}-Send to ServiceBus complete for : {listCount}");
            return runResults;
        }
    }
}