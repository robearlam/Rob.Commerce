using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines
{
    public interface IImportCsvProductsPipeline : IPipeline<ImportCsvProductsArgument, ImportCsvProductsResult, CommercePipelineExecutionContext>
    {

    }
}