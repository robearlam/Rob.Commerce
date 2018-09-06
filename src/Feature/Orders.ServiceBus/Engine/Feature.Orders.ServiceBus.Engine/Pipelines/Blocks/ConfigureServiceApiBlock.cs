using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Entities;
using Microsoft.AspNetCore.OData.Builder;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("SFeature.Order.ServiceBus.ConfigureServiceApiBlock")]
    public class ConfigureServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull($"{Name}: The argument cannot be null.");

            modelBuilder.AddEntityType(typeof(ServiceBusManagementEntity));
            modelBuilder.EntitySet<ServiceBusManagementEntity>("ServiceBusManagementEntity");

            var configuration = modelBuilder.Action("SendtoServiceBusCommand");
            configuration.Parameter<string>("Id");
            configuration.ReturnsFromEntitySet<CommerceCommand>("Commands");

            return Task.FromResult(modelBuilder);
        }
    }
}