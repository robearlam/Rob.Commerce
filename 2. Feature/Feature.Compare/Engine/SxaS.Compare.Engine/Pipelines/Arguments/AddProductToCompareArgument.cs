using SxaS.Compare.Engine.Entities;

namespace SxaS.Compare.Engine.Pipelines.Arguments
{
    public class AddProductToCompareArgument : ProductCompareArgument
    {
        public string CatalogName { get; }
        public string ProductId { get; }
        public string VariantId { get; }

        public AddProductToCompareArgument(ProductCompare compareCollection, string catalogName, string productId, string variantId) : base(compareCollection)
        {
            CatalogName = catalogName;
            ProductId = productId;
            VariantId = variantId;
        }
    }
}
