using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using SxaS.Compare.Engine.Entities;
using SxaS.Compare.Website.Managers.Messages;

namespace SxaS.Compare.Website.Managers
{
    public interface ICompareManager
    {
        ManagerResponse<ProductCompareResult, ProductCompare> GetCurrentProductCompare(IVisitorContext visitorContext, IStorefrontContext storefrontContext);
        ManagerResponse<ProductCompareResult, ProductCompare> AddProductToCompareCollection(IVisitorContext visitorContext, IStorefrontContext storefrontContext, string catalogName, string productId, string varientId);
        ManagerResponse<RemoveFromCompareResult, string> RemoveProductFromCompareCollection(IVisitorContext visitorContext, IStorefrontContext storefrontContext, string sellableItemId);
    }
}