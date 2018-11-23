using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Rules;

namespace Foundation.Rules.Engine.Benefits
{
    [EntityIdentifier(RulesConstants.Benefits.CartItemPreviouslyPurchasedPercentOffAction)]
    public class CartItemsPreviouslyPurchasedPercentOffAction : ICartLineAction
    {
        private readonly FindEntitiesInListCommand _findEntitiesInListCommand;
        public IRuleValue<int> PercentOff { get; set; }

        public CartItemsPreviouslyPurchasedPercentOffAction(FindEntitiesInListCommand findEntitiesInListCommand)
        {
            _findEntitiesInListCommand = findEntitiesInListCommand;
        }

        public void Execute(IRuleExecutionContext context)
        {
            var commerceContext = context.Fact<CommerceContext>();
            var cart = commerceContext?.GetObjects<Cart>().FirstOrDefault();
            var totals = commerceContext?.GetObjects<CartTotals>().FirstOrDefault();

            if (cart == null || !cart.Lines.Any() || (totals == null || !totals.Lines.Any()))
                return;

            var list = MatchingLines(context).ToList();
            if (!list.Any())
                return;

            list.ForEach(line =>
            {
                if (!totals.Lines.ContainsKey(line.Id))
                    return;

                var propertiesModel = commerceContext.GetObject<PropertiesModel>();
                var discount = commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount;
                var d = Convert.ToDecimal(PercentOff.Yield(context) * 0.01) * totals.Lines[line.Id].SubTotal.Amount;
                if (commerceContext.GetPolicy<GlobalPricingPolicy>().ShouldRoundPriceCalc)
                    d = decimal.Round(d, commerceContext.GetPolicy<GlobalPricingPolicy>().RoundDigits,
                        commerceContext.GetPolicy<GlobalPricingPolicy>().MidPointRoundUp
                            ? MidpointRounding.AwayFromZero
                            : MidpointRounding.ToEven);

                var amount = d * decimal.MinusOne;
                var adjustments = line.Adjustments;
                var awardedAdjustment = new CartLineLevelAwardedAdjustment
                {
                    Name = propertiesModel?.GetPropertyValue("PromotionText") as string ?? discount,
                    DisplayName = propertiesModel?.GetPropertyValue("PromotionCartText") as string ?? discount,
                    Adjustment = new Money(commerceContext.CurrentCurrency(), amount),
                    AdjustmentType = discount,
                    IsTaxable = false,
                    AwardingBlock = nameof(CartItemsPreviouslyPurchasedPercentOffAction)
                };

                adjustments.Add(awardedAdjustment);
                totals.Lines[line.Id].SubTotal.Amount = totals.Lines[line.Id].SubTotal.Amount + amount;
                line.GetComponent<MessagesComponent>().AddMessage(commerceContext.GetPolicy<KnownMessageCodePolicy>().Promotions, $"PromotionApplied: {propertiesModel?.GetPropertyValue("PromotionId") ?? nameof(CartItemsPreviouslyPurchasedPercentOffAction)}");
            });
        }

        protected virtual IEnumerable<CartLineComponent> MatchingLines(IRuleExecutionContext context)
        {
            var commerceContext = context.Fact<CommerceContext>();
            var cart = commerceContext?.GetObject<Cart>();
            var contextContactComponent = cart?.GetComponent<ContactComponent>();
            if (cart == null || string.IsNullOrWhiteSpace(contextContactComponent?.ShopperId))
                return new List<CartLineComponent>();

            var listName = string.Format(commerceContext.GetPolicy<KnownOrderListsPolicy>().CustomerOrders, contextContactComponent.ShopperId);
            var customersOrders = _findEntitiesInListCommand.Process<Order>(commerceContext, listName, 0, int.MaxValue).Result.Items.ToList();

            return cart.Lines.Where(cartLine => customersOrders.Any(order => order.Lines.Any(ProductExistsInOrderAndCart(cartLine))));
        }

        private static Func<CartLineComponent, bool> ProductExistsInOrderAndCart(CartLineComponent cartLine)
        {
            return orderLine => cartLine.GetComponent<CartProductComponent>().Id == orderLine.GetComponent<CartProductComponent>().Id;
        }
    }
}