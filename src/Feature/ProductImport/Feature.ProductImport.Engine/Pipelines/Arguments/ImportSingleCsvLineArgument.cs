using Sitecore.Commerce.Core;

namespace Feature.ProductImport.Engine.Pipelines.Arguments
{
    public class ImportSingleCsvLineArgument : PipelineArgument
    {
        public CsvImportLine Line { get; set; }

        public ImportSingleCsvLineArgument(CsvImportLine line)
        {
            Line = line;
        }
    }
}