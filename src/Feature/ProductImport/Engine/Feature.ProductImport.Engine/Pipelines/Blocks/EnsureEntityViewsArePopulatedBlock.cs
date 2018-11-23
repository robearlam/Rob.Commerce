using System.Linq;
using System.Threading.Tasks;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Composer;
using Sitecore.Commerce.Plugin.Views;

namespace Feature.ProductImport.Engine.Pipelines.Blocks
{
    public class EnsureEntityViewsArePopulatedBlock : PipelineBlock<ImportSingleCsvLineArgument, ImportSingleCsvLineArgument, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findEntityPipeline;
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        public EnsureEntityViewsArePopulatedBlock(IFindEntityPipeline findEntityPipeline, IPersistEntityPipeline persistEntityPipeline)
        {
            _findEntityPipeline = findEntityPipeline;
            _persistEntityPipeline = persistEntityPipeline;
        }

        public override async Task<ImportSingleCsvLineArgument> Run(ImportSingleCsvLineArgument arg, CommercePipelineExecutionContext context)
        {
            const string viewName = "Sizing";
            var composerTemplate = await GetComposerTemplate(viewName, context);
            if (composerTemplate == null)
            {
                context.Abort("Sizing Composer Template not found", this);
                return arg;
            }

            var sellableItem = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), $"{CommerceEntity.IdPrefix<SellableItem>()}{arg.Line.ProductId}"), context);
            if (sellableItem == null)
            {
                context.Abort("Unable to populate EntityView data, SellableItem not found", this);
                return arg;
            }

            var entityViewComponent = GetPopulatedEntityViewComponent(arg, composerTemplate, viewName);
            if (entityViewComponent == null)
                return arg;

            sellableItem.Components.Add(entityViewComponent);
            await _persistEntityPipeline.Run(new PersistEntityArgument(sellableItem), context);
            return arg;
        }

        private static EntityViewComponent GetPopulatedEntityViewComponent(ImportSingleCsvLineArgument arg, CommerceEntity composerTemplate, string viewName)
        {
            var entityViewComponent = composerTemplate.GetComponent<EntityViewComponent>();
            if (!(entityViewComponent.View.ChildViews.FirstOrDefault(x => x.Name == viewName) is EntityView entityView))
                return null;

            entityView.EntityId = $"{CommerceEntity.IdPrefix<SellableItem>()}{arg.Line.ProductId}";
            PopulatePropertyValue(entityView, "Waist", arg.Line.Waist);
            PopulatePropertyValue(entityView, "OutsideLeg", arg.Line.Leg);
            PopulatePropertyValue(entityView, "InsideLeg", arg.Line.InsideLeg);
            return entityViewComponent;
        }

        private static void PopulatePropertyValue(EntityView entityView, string propertyName, string propertyValue)
        {
            var property = entityView.Properties.FirstOrDefault(x => x.Name == propertyName);
            if (property == null)
                return;

            property.RawValue = propertyValue;
        }

        private async Task<CommerceEntity> GetComposerTemplate(string viewName, CommercePipelineExecutionContext context)
        {
            return await _findEntityPipeline.Run(new FindEntityArgument(typeof(ComposerTemplate), $"{CommerceEntity.IdPrefix<ComposerTemplate>()}{viewName}"), context);
        }
    }
}