using Sitecore.Commerce.Plugin.Catalog;

namespace Feature.ProductImport.Engine.Mappers
{
    public interface ISellableItemMapper
    {
        CreateSellableItemArgument MapToArg(string csvLine);
        SellableItem MapToEntity(SellableItem sellableItem, string csvLine);
    }
}