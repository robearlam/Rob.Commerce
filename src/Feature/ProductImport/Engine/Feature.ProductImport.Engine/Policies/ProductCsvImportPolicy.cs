using Sitecore.Commerce.Core;

namespace Feature.ProductImport.Engine.Policies
{
    public class ProductCsvImportPolicy : Policy
    {
        public char Separator { get; set; } = ',';
    }
}
