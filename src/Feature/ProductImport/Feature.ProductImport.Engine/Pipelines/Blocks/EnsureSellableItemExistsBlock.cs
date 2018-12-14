using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines.Blocks
{
    public class EnsureSellableItemExistsBlock : PipelineBlock<ImportSingleCsvLineArgument, ImportSingleCsvLineArgument, CommercePipelineExecutionContext>
    {
        private readonly ICreateSellableItemPipeline _createSellableItemPipeline;
        private readonly IEditSellableItemPipeline _editSellableItemPipeline;

        public EnsureSellableItemExistsBlock(ICreateSellableItemPipeline createSellableItemPipeline, IEditSellableItemPipeline editSellableItemPipeline)
        {
            _createSellableItemPipeline = createSellableItemPipeline;
            _editSellableItemPipeline = editSellableItemPipeline;
        }

        public override async Task<ImportSingleCsvLineArgument> Run(ImportSingleCsvLineArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg, nameof(arg)).IsNotNull();
            Condition.Requires(arg.Line, nameof(arg.Line)).IsNotNull();

            var createSellableItemArg = MapToArg(arg.Line);
            var createResult = await _createSellableItemPipeline.Run(createSellableItemArg, context);
            var sellableItem = createResult.SellableItems.FirstOrDefault();
            if (sellableItem == null)
                return arg;

            sellableItem = MapToEntity(sellableItem, arg.Line);
            await _editSellableItemPipeline.Run(sellableItem, context);
            return arg;
        }

        private static CreateSellableItemArgument MapToArg(CsvImportLine csvImportLine)
        {
            return new CreateSellableItemArgument(csvImportLine.ProductId, csvImportLine.ProductName, csvImportLine.DisplayName, csvImportLine.Description)
            {
                Brand = csvImportLine.Brand,
                Manufacturer = csvImportLine.Manufacturer,
                TypeOfGood = csvImportLine.TypeOfGood,
                Tags = csvImportLine.Tags.ToList()
            };
        }

        private static SellableItem MapToEntity(SellableItem sellableItem, CsvImportLine csvImportLine)
        {
            MapPricingEntities(sellableItem, csvImportLine);
            MapImages(sellableItem, csvImportLine);
            return sellableItem;
        }

        private static void MapImages(SellableItem sellableItem, CsvImportLine csvImportLine)
        {
            var imagesComponent = sellableItem.GetComponent<ImagesComponent>();
            foreach (var image in csvImportLine.Images)
            {
                if (imagesComponent.Images.All(x => x != image))
                {
                    imagesComponent.Images.Add(image);
                }
            }
        }

        private static void MapPricingEntities(SellableItem sellableItem, CsvImportLine csvImportLine)
        {
            var pricingPolicy = sellableItem.GetPolicy<ListPricingPolicy>();
            foreach (var listPrice in csvImportLine.ListPrices)
            {
                var moneyEntity = pricingPolicy.Prices.FirstOrDefault(x => x.CurrencyCode == listPrice.CurrencyCode);
                if (moneyEntity != null)
                {
                    moneyEntity.Amount = listPrice.Amount;
                }
                else
                {
                    var money = new Money(listPrice.CurrencyCode, listPrice.Amount);
                    (pricingPolicy.Prices as List<Money>)?.Add(money);
                }
            }
        }
    }
}