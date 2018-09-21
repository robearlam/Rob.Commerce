using System.Collections.Generic;
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
            if (entityView.Name != "Plugins")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<ServiceBusOrderPlacedPolicy>();
            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();
            var userPluginOptions = await _commerceCommander.Command<PluginCommander>().CurrentUserSettings(context.CommerceContext, _commerceCommander);
            if (userPluginOptions.EnabledPlugins.Contains("Feature.Orders.ServiceBus"))
            {
                if (userPluginOptions.HasPolicy<ServiceBusOrderPlacedPolicy>())
                {
                    pluginPolicy = userPluginOptions.GetPolicy<ServiceBusOrderPlacedPolicy>();
                }
            }

            entityView.ChildViews.Add(
                new EntityView
                {
                    ItemId = "localordersnotsent",
                    Icon = pluginPolicy.Icon,
                    DisplayName = "Local Orders pending send to ServiceBus",
                    Properties = new List<ViewProperty> {
                        new ViewProperty {Name = "ItemId", RawValue = "localordersnotsent", IsHidden = true },
                        new ViewProperty {Name = "QueueName", RawValue = "something1"  },
                        new ViewProperty {Name = "Orders Count", RawValue = "count1" }

                    }
                });

            entityView.ChildViews.Add(
                new EntityView
                {
                    ItemId = "Total Orders sent to ServiceBus",
                    Icon = pluginPolicy.Icon,
                    DisplayName = "Total Orders sent to ServiceBus",
                    Properties = new List<ViewProperty> {
                        new ViewProperty {Name = "ItemId", RawValue = "localordersnotsent", IsHidden = true },
                        new ViewProperty {Name = "QueueName", RawValue = "something2" },
                        new ViewProperty {Name = "Orders Count", RawValue = "count2" }

                    }
                });

            //var pluginRowEntityView = new EntityView
            //{
            //    Properties = new List<ViewProperty>
            //    {
            //        new ViewProperty { Name = "ItemId", RawValue = "plugin 1", UiType = "EntityLink", IsHidden = true },
            //        new ViewProperty { Name = "Plugin Name", RawValue = "Service Bus", IsReadOnly = true },
            //        new ViewProperty { Name = "Is Enabled?", RawValue = pluginPolicy.Enabled, IsReadOnly = true }
            //    }
            //};
            //entityView.ChildViews.Add(pluginRowEntityView);

            //var pluginRowEntityView2 = new EntityView
            //{
            //    Properties = new List<ViewProperty>
            //    {
            //        new ViewProperty { Name = "ItemId", RawValue = "plugin 2", UiType = "EntityLink", IsHidden = true },
            //        new ViewProperty { Name = "Plugin Name", RawValue = "Service Bus", IsReadOnly = true },
            //        new ViewProperty { Name = "Is Enabled?", RawValue = pluginPolicy.Enabled, IsReadOnly = true }
            //    }
            //};
            //entityView.ChildViews.Add(pluginRowEntityView2);

            //if (pluginPolicy.Enabled)
            //{
            //    actionsPolicy.Actions.Add(new EntityActionView
            //    {
            //        Name = "Roles.EnablePlugin.Feature.Orders.ServiceBus",
            //        DisplayName = "Enable ServiceBus",
            //        Description = "Enable ServiceBus",
            //        IsEnabled = true,
            //        ConfirmationMessage = "This is where a confirmation message goes",
            //        RequiresConfirmation = true,
            //        EntityView = "",
            //        Icon = "box_into"
            //    });
            //}
            //else
            //{
            //    actionsPolicy.Actions.Add(new EntityActionView
            //    {
            //        Name = "Roles.DisablePlugin.Feature.Orders.ServiceBus",
            //        DisplayName = "Disable Plugin (Feature.Orders.ServiceBus)",
            //        Description = "Disable Plugin (Feature.Orders.ServiceBus)",
            //        IsEnabled = true,
            //        ConfirmationMessage = "This is where a confirmation message goes",
            //        RequiresConfirmation = true,
            //        EntityView = "",
            //        Icon = "box_out"
            //    });
            //}

            return entityView;
        }
    }
}