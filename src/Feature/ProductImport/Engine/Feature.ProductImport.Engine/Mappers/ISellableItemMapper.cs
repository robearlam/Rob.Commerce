using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Plugin.Catalog;

namespace Feature.ProductImport.Engine.Mappers
{
    public interface ISellableItemMapper
    {
        CreateSellableItemArgument MapToArg(CsvImportLine csvImportLine);
        SellableItem MapToEntity(SellableItem sellableItem, CsvImportLine csvImportLine);
    }
}