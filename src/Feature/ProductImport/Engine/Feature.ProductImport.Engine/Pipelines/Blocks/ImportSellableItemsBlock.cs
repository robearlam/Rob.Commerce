using System.Threading.Tasks;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines.Blocks
{
    public class ImportSellableItemsBlock : PipelineBlock<ImportCsvProductsArgument, ImportCsvProductsArgument, CommercePipelineExecutionContext>
    {
        private readonly IImportSingleCsvRowPipeline _importSingleCsvRowPipeline;

        public ImportSellableItemsBlock(IImportSingleCsvRowPipeline importSingleCsvRowPipeline)
        {
            _importSingleCsvRowPipeline = importSingleCsvRowPipeline;
        }

        public override Task<ImportCsvProductsArgument> Run(ImportCsvProductsArgument arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(arg);
        }
    }
}