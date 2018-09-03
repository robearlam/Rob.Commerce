using Feature.ProductImport.Engine.Pipelines.Arguments;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines
{
    public class ImportCsvProductsPipeline : CommercePipeline<ImportCsvProductsArgument, ImportCsvProductsResult>, IImportCsvProductsPipeline
    {
        public ImportCsvProductsPipeline(IPipelineConfiguration<IImportCsvProductsPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}