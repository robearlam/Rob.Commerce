using System.Collections.Generic;
using System.Linq;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Pricing;

namespace Feature.ProductImport.Engine.Mappers
{
    public class SellableItemMapper : ISellableItemMapper
    {
        public CreateSellableItemArgument MapToArg(CsvImportLine csvImportLine)
        {
            return new CreateSellableItemArgument(csvImportLine.ProductId, csvImportLine.ProductName, csvImportLine.DisplayName, csvImportLine.Description)
            {
                Brand = csvImportLine.Brand,
                Manufacturer = csvImportLine.Manufacturer,
                TypeOfGood = csvImportLine.TypeOfGood,
                Tags = csvImportLine.Tags.ToList()
            };
        }

        public SellableItem MapToEntity(SellableItem sellableItem, CsvImportLine csvImportLine)
        {
            MapPricingEntities(sellableItem, csvImportLine);
            MapImages(sellableItem, csvImportLine);
            return sellableItem;
        }

        private void MapImages(SellableItem sellableItem, CsvImportLine csvImportLine)
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