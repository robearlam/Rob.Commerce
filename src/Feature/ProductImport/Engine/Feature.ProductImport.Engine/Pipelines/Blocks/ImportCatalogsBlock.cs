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
        private const int CatalogNameIndex = 10;
        private const int CatalogDisplayNameIndex = 11;
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

            var catalogNames = new List<string>();
            var catalogDisplayNames = new Dictionary<string, string>();
            GetDestinctCatalogData(arg, catalogNames, catalogDisplayNames);

            foreach (var catalogName in catalogNames)
            {
                var catalogDisplayName = catalogDisplayNames[catalogName];
                var catalog = await _findEntityPipeline.Run(new FindEntityArgument(typeof(Catalog), $"{CommerceEntity.IdPrefix<Catalog>()}{catalogName}", 1), context);
                if (catalog != null)
                    continue;

                await _createCatalogPipeline.Run(new CreateCatalogArgument(catalogName, catalogDisplayName), context);
            }

            return arg;
        }

        private static void GetDestinctCatalogData(ImportCsvProductsArgument arg, ICollection<string> catalogNames, IDictionary<string, string> catalogDisplayNames)
        {
            foreach (var line in arg.FileLines)
            {
                var catalogData = line.Split(',');
                var catalogName = catalogData[CatalogNameIndex];
                var catalogDisplayName = catalogData[CatalogDisplayNameIndex];
                if (catalogNames.Contains(catalogName))
                    continue;

                catalogNames.Add(catalogName);
                catalogDisplayNames.Add(catalogName, catalogDisplayName);
            }
        }
    }
}