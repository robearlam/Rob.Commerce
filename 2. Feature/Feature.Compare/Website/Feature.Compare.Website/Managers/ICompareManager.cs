using Feature.Compare.Engine.Entities;
using Feature.Compare.Website.Managers.Messages;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;

namespace Feature.Compare.Website.Managers
{
    public interface ICompareManager
    {
        ManagerResponse<ProductCompareResult, ProductCompare> GetCurrentProductCompare(IVisitorContext visitorContext, IStorefrontContext storefrontContext);
        ManagerResponse<ProductCompareResult, ProductCompare> AddProductToCompareCollection(IVisitorContext visitorContext, IStorefrontContext storefrontContext, string catalogName, string productId, string varientId);
        ManagerResponse<RemoveFromCompareResult, string> RemoveProductFromCompareCollection(IVisitorContext visitorContext, IStorefrontContext storefrontContext, string sellableItemId);
    }
}