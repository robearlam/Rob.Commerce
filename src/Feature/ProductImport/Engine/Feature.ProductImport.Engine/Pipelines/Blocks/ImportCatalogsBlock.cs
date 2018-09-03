using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Condition.Requires(arg.ImportFile, nameof(arg.ImportFile)).IsNotNull();

            var lines = new List<string>();
            using (var reader = new StreamReader(arg.ImportFile.OpenReadStream()))
            {
                var counter = 0;
                while (!reader.EndOfStream)
                {
                    if (counter == 0) //skip header
                        reader.ReadLine();

                    lines.Add(reader.ReadLine());
                }
            }

            var catalogNames = new List<string>();
            var catalogDisplayNames = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var catalogData = line.Split(',');
                var catalogName = catalogData[10];
                var catalogDisplayName = catalogData[11];
                if (catalogNames.Contains(catalogName))
                    continue;

                catalogNames.Add(catalogName);
                catalogDisplayNames.Add(catalogName, catalogDisplayName);
            }

            foreach (var catalogName in catalogNames)
            {
                var catalogDisplayName = catalogDisplayNames[catalogName];
                var catalog = await _findEntityPipeline.Run(new FindEntityArgument(typeof(Catalog), catalogName, 1), context);
                if (catalog != null)
                    continue;

                await _createCatalogPipeline.Run(new CreateCatalogArgument(catalogName, catalogDisplayName), context);
            }

            return arg;
        }
    }
}