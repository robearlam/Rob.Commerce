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

            var pluginPolicy = context.GetPolicy<ServiceBusPluginPolicy>();

            var userPluginOptions = await _commerceCommander.Command<PluginCommander>().CurrentUserSettings(context.CommerceContext, _commerceCommander);

            if (userPluginOptions.EnabledPlugins.Contains("Feature.Orders.ServiceBus"))
            {
                if (userPluginOptions.HasPolicy<ServiceBusPluginPolicy>())
                {
                    pluginPolicy = userPluginOptions.GetPolicy<ServiceBusPluginPolicy>();
                }
                else
                {
                    pluginPolicy.IsDisabled = false;
                }
            }
            else
            {
                pluginPolicy.IsDisabled = true;
            }

            if (!pluginPolicy.IsDisabled)
            {
                var newEntityView = new EntityView
                {
                    Name = "ServiceBus",
                    DisplayName = "ServiceBus",
                    Icon = pluginPolicy.Icon,
                    ItemId = "ServiceBus"
                };

                entityView.ChildViews.Add(newEntityView);
            }

            return entityView;
        }
    }
}