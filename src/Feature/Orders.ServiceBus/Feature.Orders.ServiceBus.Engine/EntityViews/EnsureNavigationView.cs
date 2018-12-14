using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Policies;
using Foundation.PluginEnhancements.Engine.Commands;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.EntityViews
{
    [PipelineDisplayName("EnsureNavigationView")]
    public class EnsureNavigationView : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public EnsureNavigationView(CommerceCommander commerceCommander)
        {
            _commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null");

            if (entityView.Name != "ToolsNavigation")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<ServiceBusOrderPlacedPolicy>();
            var userPluginOptions = await _commerceCommander.Command<PluginCommander>().CurrentUserSettings(context.CommerceContext, _commerceCommander);

            if (userPluginOptions.EnabledPlugins.Contains("Feature.Orders.ServiceBus"))
            {
                if (userPluginOptions.HasPolicy<ServiceBusOrderPlacedPolicy>())
                {
                    pluginPolicy = userPluginOptions.GetPolicy<ServiceBusOrderPlacedPolicy>();
                }
                else
                {
                    pluginPolicy.Enabled = false;
                }
            }
            else
            {
                pluginPolicy.Enabled = true;
            }

            if (!pluginPolicy.Enabled)
            {
                return entityView;
            }

            var newEntityView = new EntityView
            {
                Name = "ServiceBus",
                DisplayName = "Service Bus",
                Icon = pluginPolicy.Icon,
                ItemId = "ServiceBus"
            };

            entityView.ChildViews.Add(newEntityView);
            return entityView;
        }
    }
}