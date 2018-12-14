using System.Threading.Tasks;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines.Blocks
{
    public class EnsureSellableItemIsInCategory : PipelineBlock<ImportSingleCsvLineArgument, ImportSingleCsvLineArgument, CommercePipelineExecutionContext>
    {
        private readonly IAssociateSellableItemToParentPipeline _associateSellableItemToParentPipeline;

        public EnsureSellableItemIsInCategory(IAssociateSellableItemToParentPipeline associateSellableItemToParentPipeline)
        {
            _associateSellableItemToParentPipeline = associateSellableItemToParentPipeline;
        }

        public override async Task<ImportSingleCsvLineArgument> Run(ImportSingleCsvLineArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg, nameof(arg)).IsNotNull();
            Condition.Requires(arg.Line, nameof(arg.Line)).IsNotNull();

            var catalogReferenceArgument = new CatalogReferenceArgument(arg.Line.FullEntityCatalogName, arg.Line.FullEntityCategoryName, arg.Line.FullEntitySellableItemName);
            await _associateSellableItemToParentPipeline.Run(catalogReferenceArgument, context);
            return arg;
        }
    }
}