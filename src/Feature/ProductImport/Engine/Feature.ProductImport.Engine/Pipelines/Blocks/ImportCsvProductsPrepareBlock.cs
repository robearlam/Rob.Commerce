using System;
using System.Threading.Tasks;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines.Blocks
{
    public class ImportCsvProductsPrepareBlock : PipelineBlock<ImportCsvProductsArgument, ImportCsvProductsArgument, CommercePipelineExecutionContext>
    {
        private readonly IRemoveAllCatalogItemsPipeline _removeAllCatalogItemsPipeline;

        public ImportCsvProductsPrepareBlock(IRemoveAllCatalogItemsPipeline removeAllCatalogItemsPipeline)
        {
            _removeAllCatalogItemsPipeline = removeAllCatalogItemsPipeline;
        }

        public override async Task<ImportCsvProductsArgument> Run(ImportCsvProductsArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg, nameof(arg)).IsNotNull();
            Condition.Requires(arg.Mode, $"{nameof(arg)}.Mode").IsNotNullOrWhiteSpace();

            var lowerInvariant = arg.Mode.ToLowerInvariant();
            if (!IsImportModeValid(lowerInvariant))
                throw new NotImplementedException($"The import mode '{arg.Mode}' is not implemented.");

            if (arg.Mode.ToLowerInvariant() != "replace")
                return arg;

            await _removeAllCatalogItemsPipeline.Run(new RemoveAllCatalogItemsArgument(), context);
            return arg;
        }

        private static bool IsImportModeValid(string lowerInvariant)
        {
            return lowerInvariant == "replace" || lowerInvariant == "add";
        }
    }
}