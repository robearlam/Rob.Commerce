using Feature.ProductImport.Engine.Pipelines.Arguments;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines
{
    public class ImportSingleCsvRowPipeline : CommercePipeline<ImportSingleCsvLineArgument, ImportSingleCsvLineResult>, IImportSingleCsvRowPipeline
    {
        public ImportSingleCsvRowPipeline(IPipelineConfiguration<IImportSingleCsvRowPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}