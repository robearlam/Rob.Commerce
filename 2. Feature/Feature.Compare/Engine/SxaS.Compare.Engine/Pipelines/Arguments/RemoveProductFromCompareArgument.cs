using SxaS.Compare.Engine.Entities;

namespace SxaS.Compare.Engine.Pipelines.Arguments
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
