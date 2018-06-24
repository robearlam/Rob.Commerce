using Feature.Compare.Engine.Entities;

namespace Feature.Compare.Engine.Pipelines.Arguments
{
    public class RemoveProductFromCompareArgument : ProductCompareArgument
    {
        public string SellableItemId { get; }

        public RemoveProductFromCompareArgument(ProductCompare compareCollection, string sellableItemId) : base(compareCollection)
        {
            SellableItemId = sellableItemId;
        }
    }
}
