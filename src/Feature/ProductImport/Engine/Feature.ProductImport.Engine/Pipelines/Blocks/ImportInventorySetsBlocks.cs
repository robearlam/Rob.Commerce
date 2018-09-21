using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Inventory;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines.Blocks
{
    public class ImportInventorySetsBlocks : PipelineBlock<ImportCsvProductsArgument, ImportCsvProductsArgument, CommercePipelineExecutionContext>
    {
        private readonly CreateInventorySetCommand _createInventorySetCommand;
        private readonly IFindEntityPipeline _findEntityPipeline;

        public ImportInventorySetsBlocks(CreateInventorySetCommand createInventorySetCommand, IFindEntityPipeline findEntityPipeline)
        {
            _createInventorySetCommand = createInventorySetCommand;
            _findEntityPipeline = findEntityPipeline;
        }

        public override async Task<ImportCsvProductsArgument> Run(ImportCsvProductsArgument arg, CommercePipelineExecutionContext context)
        {
            var inventorySets = GetDistinctInventorySetNames(arg);
            foreach (var inventorySetName in inventorySets)
            {
                if (await InventorySetExists(inventorySetName, context))
                    continue;

                var result = await _createInventorySetCommand.Process(context.CommerceContext, inventorySetName, inventorySetName, inventorySetName);
            }

            return arg;
        }

        private async Task<bool>  InventorySetExists(string inventorySetName, CommercePipelineExecutionContext context)
        {
            var inventorySet = await _findEntityPipeline.Run(new FindEntityArgument(typeof(InventorySet), $"{CommerceEntity.IdPrefix<InventorySet>()}{inventorySetName}", 1), context);
            return inventorySet != null;
        }

        private static IEnumerable<string> GetDistinctInventorySetNames(ImportCsvProductsArgument arg)
        {
            var inventorySetNames = new List<string>();
            foreach (var line in arg.FileLines)
            {
                foreach (var inventorySetName in line.InventorySets.Keys)
                {
                    if(!inventorySetNames.Contains(inventorySetName))
                        inventorySetNames.Add(inventorySetName);
                }
            }
            return inventorySetNames;
        }
    }
}
