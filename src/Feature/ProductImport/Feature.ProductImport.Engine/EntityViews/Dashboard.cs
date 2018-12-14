using System;
using System.Threading.Tasks;
using Feature.ProductImport.Engine.Commands;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.EntityViews
{
    public class Dashboard : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        public Dashboard(CommerceCommander commerceCommander)
        {
            _commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null");
            if (entityView.Name != "ProductImport-Dashboard")
            {
                return entityView;
            }

            entityView.EntityId = string.Empty;
            entityView.Name = "Product Import";
            entityView.UiHint = "Table";

            try
            {
                _commerceCommander.Command<ChildViewProductImport>().Process(context.CommerceContext, entityView);
            }
            catch (Exception ex)
            {
                context.Logger.LogError(ex, "ProductImport.DashBoard.Exception");
            }
            return entityView;
        }
    }
}
