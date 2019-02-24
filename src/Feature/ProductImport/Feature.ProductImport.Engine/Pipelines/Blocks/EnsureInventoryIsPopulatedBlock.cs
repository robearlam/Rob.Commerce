using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Inventory;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines.Blocks
{
    public class EnsureInventoryIsPopulatedBlock : PipelineBlock<ImportSingleCsvLineArgument, ImportSingleCsvLineArgument, CommercePipelineExecutionContext>
    {
        private readonly AssociateSellableItemToInventorySetCommand _associateSellableItemToInventorySetCommand;

        public EnsureInventoryIsPopulatedBlock(AssociateSellableItemToInventorySetCommand associateSellableItemToInventorySetCommand)
        {
            _associateSellableItemToInventorySetCommand = associateSellableItemToInventorySetCommand;
        }

        public override async Task<ImportSingleCsvLineArgument> Run(ImportSingleCsvLineArgument arg, CommercePipelineExecutionContext context)
        {
            foreach (var inventorySetName in arg.Line.InventorySets.Keys)
            {
                var inventoryAmount = arg.Line.InventorySets[inventorySetName];
                await _associateSellableItemToInventorySetCommand.Process(context.CommerceContext,
                            arg.Line.FullEntitySellableItemName,
                            string.Empty,
                            inventorySetName.ToEntityId<InventorySet>(),
                            GenerateEntityView(inventoryAmount, arg.Line.FullEntitySellableItemName));
            }
            return arg;
        }

        private static EntityView GenerateEntityView(int inventoryAmount, string sellableItemId)
        {
            return new EntityView
            {
                Properties = new List<ViewProperty>
                {
                    new ViewProperty
                    {
                        Name = "SellableItem",
                        Value = sellableItemId,
                    },
                    new ViewProperty
                    {
                        Name = "Quantity",
                        Value = inventoryAmount.ToString()
                    }
                }
            };
        }
    }
}
