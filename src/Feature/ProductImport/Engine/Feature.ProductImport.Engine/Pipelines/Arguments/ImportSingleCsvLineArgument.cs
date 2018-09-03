using Sitecore.Commerce.Core;

namespace Feature.ProductImport.Engine.Pipelines.Arguments
{
    public class ImportSingleCsvLineArgument : PipelineArgument
    {
        public string Line { get; set; }
    }
}