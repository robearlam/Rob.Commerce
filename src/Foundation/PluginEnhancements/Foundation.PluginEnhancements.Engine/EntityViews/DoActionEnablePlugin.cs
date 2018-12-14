using System;
using System.Threading.Tasks;
using Foundation.PluginEnhancements.Engine.Commands;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Pipelines;

namespace Foundation.PluginEnhancements.Engine.EntityViews
{
    [PipelineDisplayName("DoActionEnablePlugin")]
    public class DoActionEnablePlugin : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionEnablePlugin(CommerceCommander commerceCommander)
        {
            _commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {

            if (entityView == null || !entityView.Action.Contains("Roles.EnablePlugin"))
            {
                return entityView;
            }

            try
            {
                var pluginName = entityView.Action.Replace("Roles.EnablePlugin.", "");

                var userPluginOptions = await _commerceCommander.Command<PluginCommander>().CurrentUserSettings(context.CommerceContext, _commerceCommander);

                userPluginOptions.EnabledPlugins.Add(pluginName);

                await _commerceCommander.PersistEntity(context.CommerceContext, userPluginOptions);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddDashboardEntity.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
