using System.Threading.Tasks;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines.Blocks
{
    public class EnsureInventoryIsPopulatedBlock : PipelineBlock<ImportSingleCsvLineArgument, ImportSingleCsvLineArgument, CommercePipelineExecutionContext>
    {
        public override Task<ImportSingleCsvLineArgument> Run(ImportSingleCsvLineArgument arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(arg);
        }
    }
}
