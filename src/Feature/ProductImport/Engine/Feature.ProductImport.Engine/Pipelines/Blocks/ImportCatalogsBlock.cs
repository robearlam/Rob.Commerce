using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines.Blocks
{
    public class ImportCatalogsBlock : PipelineBlock<ImportCsvProductsArgument, ImportCsvProductsArgument, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findEntityPipeline;
        private readonly ICreateCatalogPipeline _createCatalogPipeline;

        public ImportCatalogsBlock(IFindEntityPipeline findEntityPipeline, ICreateCatalogPipeline createCatalogPipeline)
        {
            _findEntityPipeline = findEntityPipeline;
            _createCatalogPipeline = createCatalogPipeline;
        }

        public override async Task<ImportCsvProductsArgument> Run(ImportCsvProductsArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg, nameof(arg)).IsNotNull();
            Condition.Requires(arg.FileLines, nameof(arg.FileLines)).IsNotNull();          

            var catalogList = GetDestinctCatalogData(arg);
            foreach (var (catalogName, fullCatalogName, catalogDisplayName) in catalogList)
            {
                var catalog = await _findEntityPipeline.Run(new FindEntityArgument(typeof(Catalog), fullCatalogName, 1), context);
                if (catalog != null)
                    continue;

                await _createCatalogPipeline.Run(new CreateCatalogArgument(catalogName, catalogDisplayName), context);
            }

            return arg;
        }

        private static List<(string CatalogName, string CatalogFullEntityName, string CatalogDisplayName)> GetDestinctCatalogData(ImportCsvProductsArgument arg)
        {
            var catalogList = new List<(string CatalogName, string CatalogFullEntityName, string CatalogDisplayName)>();
            foreach (var line in arg.FileLines)
            {
                if (catalogList.Exists(x => x.CatalogFullEntityName == line.FullEntityCatalogName))
                    continue;

                catalogList.Add((CatalogName:line.CatalogName, CatalogFullEntityName: line.FullEntityCatalogName, CatalogDisplayName: line.CatalogDisplayName));
            }
            return catalogList;
        }
    }
}