using System.Collections.Generic;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Foundation.PluginEnhancements.Engine.EntityViews
{
    public class Dashboard : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null");

            if (entityView.Name != "Plugins")
            {
                return Task.FromResult(entityView);
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();
            entityView.UiHint = "Table";
            entityView.Icon = pluginPolicy.Icon;
            entityView.DisplayName = "Registered Plugins";

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

            return Task.FromResult(entityView);
        }
    }
}