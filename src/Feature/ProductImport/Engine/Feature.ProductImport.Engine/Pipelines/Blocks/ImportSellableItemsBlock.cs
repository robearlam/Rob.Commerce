using System.IO;
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
            Condition.Requires(arg.ImportFile, nameof(arg.ImportFile)).IsNotNull();

            var shouldContinue = true;
            if (shouldContinue)
            {
                using (var reader = new StreamReader(arg.ImportFile.OpenReadStream()))
                {
                    var counter = 0; 
                    while (reader.Peek() >= 0)
                    {
                        if(counter == 0) //skip header
                            await reader.ReadLineAsync();

                        var importSingleCsvLineArgument = new ImportSingleCsvLineArgument
                        {
                            Line = await reader.ReadLineAsync()
                        };

                        await _importSingleCsvRowPipeline.Run(importSingleCsvLineArgument, context);
                        counter++;
                    }
                }
            }

            return arg;
        }
    }
}