using System.Threading.Tasks;
using Foundation.PluginEnhancements.Engine.Commands;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Foundation.PluginEnhancements.Engine.EntityViews
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

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            await _commerceCommander.Command<PluginCommander>().CurrentUserSettings(context.CommerceContext, _commerceCommander);

            var newEntityView = new EntityView
            {
                Name = "Plugins",
                DisplayName = "Plugins",
                Icon = pluginPolicy.Icon,
                ItemId = "Plugins"
            };

            entityView.ChildViews.Add(newEntityView);

            return entityView;
        }
    }
}
