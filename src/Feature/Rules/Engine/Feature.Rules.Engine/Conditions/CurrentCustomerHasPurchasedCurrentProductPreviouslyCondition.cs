using System;
using System.Linq;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Rules;

namespace Feature.Rules.Engine.Conditions
{
    [EntityIdentifier("CurrentCustomerHasPurchasedCurrentProductPreviouslyCondition")]
    public class CurrentCustomerHasPurchasedCurrentProductPreviouslyCondition : ICondition
    {
        private readonly FindEntitiesInListCommand _findEntitiesInListCommand;
        public IRuleValue<int> Days { get; set; }

        public CurrentCustomerHasPurchasedCurrentProductPreviouslyCondition(FindEntitiesInListCommand findEntitiesInListCommand)
        {
            _findEntitiesInListCommand = findEntitiesInListCommand;
        }

        public bool Evaluate(IRuleExecutionContext context)
        {
            var commerceContext = context.Fact<CommerceContext>();
            var cart = commerceContext?.GetObject<Cart>();
            var contextContactComponent = cart?.GetComponent<ContactComponent>();
            if (cart == null || string.IsNullOrWhiteSpace(contextContactComponent?.ShopperId))
                return false;

            var orderList = _findEntitiesInListCommand.Process<Order>(commerceContext, CommerceEntity.ListName<Order>(), 0, int.MaxValue).Result.Items.ToList();
            var customersOrders = orderList.Where(x => x.GetComponent<ContactComponent>().CustomerId == contextContactComponent.ShopperId).ToList();

            return cart.Lines.Any(cartLine => customersOrders.Any(order => order.Lines.Any(ProductExistsInOrderAndCart(cartLine))));
        }

        private static Func<CartLineComponent, bool> ProductExistsInOrderAndCart(CartLineComponent cartLine)
        {
            return orderLine => cartLine.GetComponent<CartProductComponent>().Id == orderLine.GetComponent<CartProductComponent>().Id;
        }
    }
}
