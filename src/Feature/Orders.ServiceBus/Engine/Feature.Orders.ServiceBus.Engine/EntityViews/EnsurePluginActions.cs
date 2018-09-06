using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Policies;
using Foundation.PluginEnhancements.Engine.Commands;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.EntityViews
{
    [PipelineDisplayName("EnsurePluginActions")]
    public class EnsurePluginActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public EnsurePluginActions(CommerceCommander commerceCommander)
        {
            _commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (!string.IsNullOrEmpty(entityView.Action))
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<ServiceBusPluginPolicy>();
            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();

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

            if (pluginPolicy.IsDisabled)
            {
                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "Roles.EnablePlugin.Feature.Orders.ServiceBus",
                    DisplayName = "Enable ServiceBus",
                    Description = "Enable ServiceBus",
                    IsEnabled = true, ConfirmationMessage = "This is where a confirmation message goes",
                    RequiresConfirmation = true,
                    EntityView = "",
                    Icon = "box_into"
                });
            }
            else
            {
                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "Roles.DisablePlugin.Feature.Orders.ServiceBus",
                    DisplayName = "Disable Plugin (Feature.Orders.ServiceBus)",
                    Description = "Disable Plugin (Feature.Orders.ServiceBus)",
                    IsEnabled = true,
                    ConfirmationMessage = "This is where a confirmation message goes",
                    RequiresConfirmation = true,
                    EntityView = "",
                    Icon = "box_out"
                });
            }

            return entityView;
        }
    }
}