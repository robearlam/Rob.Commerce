using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.EntityViews
{
    [PipelineDisplayName("EnsureActions")]
    public class EnsureActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null");

            if (entityView.Name != "ServiceBusDashboard" && entityView.Name != "ServiceBusScopes")
            {
                return Task.FromResult(entityView);
            }

            entityView.Icon = "find_text";

            var serviceBusScopesView = entityView.ChildViews.FirstOrDefault(p => p.Name == "ServiceBusScopes");
            if (serviceBusScopesView != null)
            {
                var actionsPolicy = serviceBusScopesView.GetPolicy<ActionsPolicy>();

                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "Search-RebuildScope",
                    DisplayName = "Rebuild Scope",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "",
                    UiHint = "",
                    Icon = "chart_funnel"
                });
                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "Search-DeleteSearchIndex",
                    DisplayName = "Delete Search Index",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "",
                    UiHint = "",
                    Icon = "delete"
                });
                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "Search-CreateSearchIndex",
                    DisplayName = "Create Search Index",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "Search-CreateSearchIndex",
                    UiHint = "",
                    Icon = "add"
                });
            }

            return Task.FromResult(entityView);
        }
    }
}