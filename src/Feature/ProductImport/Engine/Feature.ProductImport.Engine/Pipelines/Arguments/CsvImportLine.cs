using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;

namespace Feature.ProductImport.Engine.Pipelines.Arguments
{
    public class CsvImportLine
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
        private const int ImagesIndex = 9;
        private const int CatalogNameIndex = 10;
        private const int CatalogDisplayNameIndex = 11;
        private const int CategoryNameIndex = 12;
        private const int CurrencyCodeIndex = 1;
        private const int ListPriceAmountIndex = 0;
        private readonly string[] _rawFields;

        public CsvImportLine(string rawData)
        {
            _rawFields = rawData.Split(',');
        }

        public string ProductId => _rawFields[ProductIdIndex];
        public string ProductName => _rawFields[ProductNameIndex];
        public string DisplayName => _rawFields[DisplayNameIndex];
        public string Description => _rawFields[DescriptionIndex];
        public string Brand => _rawFields[BrandIndex];
        public string Manufacturer => _rawFields[ManufacturerIndex];
        public string TypeOfGood => _rawFields[TypeOfGoodIndex];
        public IList<string> Tags => _rawFields[TagsIndex].Split('|');
        public IEnumerable<Money> ListPrices => GenerateListPrice();
        public IList<string> Images => _rawFields[ImagesIndex].Split('|');
        public string CatalogName => _rawFields[CatalogNameIndex];
        public string CatalogDisplayName => _rawFields[CatalogDisplayNameIndex];
        public IList<string> Categories => _rawFields[CategoryNameIndex].Split('|').ToList();

        public string FullEntityCatalogName => $"{CommerceEntity.IdPrefix<Catalog>()}{CatalogName}";
        public string FullEntityCategoryName => $"{CommerceEntity.IdPrefix<Category>()}{CatalogName}-{Categories.Last()}";
        public string FullEntitySellableItemName => $"{CommerceEntity.IdPrefix<SellableItem>()}{ProductId}";

        private IEnumerable<Money> GenerateListPrice()
        {
            var listPrices = _rawFields[ListPriceIndex].Split('|');
            var priceList = new List<Money>();
            foreach (var listPrice in listPrices)
            {
                var priceData = listPrice.Split('-');
                var amount = decimal.Parse(priceData[ListPriceAmountIndex]);
                var currencyCode = priceData[CurrencyCodeIndex];
                var money = new Money(currencyCode, amount);
                priceList.Add(money);
            }
            return priceList;
        }
    }
}