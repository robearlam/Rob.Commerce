using System;
using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;
using SxaS.Compare.Website.Repositories;

namespace SxaS.Compare.Website.Controllers
{
    public class ProductCompareController : BaseCommerceStandardController
    {
        private readonly IProductCompareRepository _productCompareRepository;
        private readonly IVisitorContext _visitorContext;
        private readonly IModelProvider _modelProvider;

        public ProductCompareController(IProductCompareRepository productCompareRepository, IVisitorContext visitorContext, IModelProvider modelProvider, IStorefrontContext storefrontContext) : base(storefrontContext)
        {
            _productCompareRepository = productCompareRepository;
            _visitorContext = visitorContext;
            _modelProvider = modelProvider;
        }

        [HttpGet]
        public ActionResult ProductCompare()
        {
            return View(GetRenderingView(nameof(ProductCompare)), _productCompareRepository.GetProductCompareModel(StorefrontContext, _visitorContext));
        }

        [HttpGet]
        public ActionResult AddVewCompareButton()
        {
            return View(GetRenderingView(nameof(AddVewCompareButton)), _productCompareRepository.GetAddViewCompareButtonModel(StorefrontContext, _visitorContext));
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult AddProductToCompareList(string addToCompareCatalogName, string addToCompareProductId, string addToCompareVarientId)
        {
            BaseJsonResult baseJsonResult;
            try
            {
                baseJsonResult = _productCompareRepository.AddProductToCompareCollection(StorefrontContext, _visitorContext, addToCompareCatalogName, addToCompareProductId, addToCompareVarientId);
            }
            catch (Exception ex)
            {
                baseJsonResult = _modelProvider.GetModel<BaseJsonResult>();
                baseJsonResult.SetErrors(nameof(AddProductToCompareList), ex);
            }
            return Json(baseJsonResult);
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult RemoveProductFromCompareList(string removeFromCompareProductId)
        {
            BaseJsonResult baseJsonResult;
            try
            {
                baseJsonResult = _productCompareRepository.RemoveProductFromCompareCollection(StorefrontContext, _visitorContext, removeFromCompareProductId);
            }
            catch (Exception ex)
            {
                baseJsonResult = _modelProvider.GetModel<BaseJsonResult>();
                baseJsonResult.SetErrors(nameof(AddProductToCompareList), ex);
            }
            return Json(baseJsonResult);
        }
    }
}