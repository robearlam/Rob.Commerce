using Sitecore.Commerce.Core;

namespace Feature.ProductImport.Engine.Pipelines.Arguments
{
    public class ImportSingleCsvLineResult : PipelineArgument
    {
        public bool Success { get; set; }
    }
}