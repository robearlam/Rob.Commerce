using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Pricing;

namespace Feature.ProductImport.Engine.Mappers
{
    public class SellableItemMapper : ISellableItemMapper
    {
        private const int ProductIdIndex = 0;
        private const int ProductNameIndex = 1;
        private const int DisplayNameIndex = 2;
        private const int DescriptionIndex = 3;
        private const int BrandIndex = 4;
        private const int ManufacturerIndex = 5;
        private const int TypeOfGoodIndex = 6;
        private const int TagsIndex = 7;
        private const int ListPriceIndex = 8;
        private const int CurrencyCodeIndex = 1;
        private const int ListPriceAmountIndex = 0;

        public CreateSellableItemArgument MapToArg(string csvLine)
        {
            var fields = csvLine.Split(',');
            return new CreateSellableItemArgument(fields[ProductIdIndex], fields[ProductNameIndex], fields[DisplayNameIndex], fields[DescriptionIndex])
            {
                Brand = fields[BrandIndex],
                Manufacturer = fields[ManufacturerIndex],
                TypeOfGood = fields[TypeOfGoodIndex],
                Tags = fields[TagsIndex].Split('|').ToList(),
            };
        }

        public SellableItem MapToEntity(SellableItem sellableItem, string csvLine)
        {
            var fields = csvLine.Split(',');
            MapPricingEntities(sellableItem, fields);
            MapImages(sellableItem, fields);
            return sellableItem;
        }

        private void MapImages(SellableItem sellableItem, string[] fields)
        {
            var imagesComponent = sellableItem.GetComponent<ImagesComponent>();
            var imageData = fields[9].Split('|');
            foreach (var image in imageData)
            {
                if (imagesComponent.Images.All(x => x != image))
                {
                    imagesComponent.Images.Add(image);
                }
            }
        }

        private static void MapPricingEntities(SellableItem sellableItem, string[] fields)
        {
            var listPrices = fields[ListPriceIndex].Split('|');

            var pricingPolicy = sellableItem.GetPolicy<ListPricingPolicy>();
            foreach (var listPrice in listPrices)
            {
                var priceData = listPrice.Split('-');
                var amount = decimal.Parse(priceData[ListPriceAmountIndex]);
                var currencyCode = priceData[CurrencyCodeIndex];
                var moneyEntity = pricingPolicy.Prices.FirstOrDefault(x => x.CurrencyCode == currencyCode);
                if (moneyEntity != null)
                {
                    moneyEntity.Amount = amount;
                }
                else
                {
                    var money = new Money(currencyCode, amount);
                    (pricingPolicy.Prices as List<Money>)?.Add(money);
                }
            }
        }
    }
}
