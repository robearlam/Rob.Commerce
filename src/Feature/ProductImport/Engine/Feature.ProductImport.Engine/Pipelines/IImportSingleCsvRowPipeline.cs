using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines
{
    public interface IImportSingleCsvRowPipeline : IPipeline<string, string, CommercePipelineExecutionContext>
    {
        
    }
}