using System.Threading.Tasks;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;
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

        public override async Task<ImportCsvProductsArgument> Run(ImportCsvProductsArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg, nameof(arg)).IsNotNull();
            Condition.Requires(arg.FileLines, nameof(arg.FileLines)).IsNotNull();

            foreach (var line in arg.FileLines)
            {
                await _importSingleCsvRowPipeline.Run(new ImportSingleCsvLineArgument(line), context);
            }

            return arg;
        }
    }
}