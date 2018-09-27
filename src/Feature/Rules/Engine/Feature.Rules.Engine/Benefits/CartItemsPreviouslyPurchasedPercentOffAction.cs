using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Rules;
using static System.Decimal;

namespace Feature.Rules.Engine.Benefits
{
    [EntityIdentifier("CartItemPreviouslyPurchasedPercentOffAction")]
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
                    d = Round(d, commerceContext.GetPolicy<GlobalPricingPolicy>().RoundDigits,
                        commerceContext.GetPolicy<GlobalPricingPolicy>().MidPointRoundUp
                            ? MidpointRounding.AwayFromZero
                            : MidpointRounding.ToEven);

                var amount = d * MinusOne;
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

            var orderList = _findEntitiesInListCommand.Process<Order>(commerceContext, CommerceEntity.ListName<Order>(), 0, int.MaxValue).Result.Items.ToList();
            var customersOrders = orderList.Where(x => x.GetComponent<ContactComponent>().CustomerId == contextContactComponent.ShopperId).ToList();

            return cart.Lines.Where(cartLine => customersOrders.Any(order => order.Lines.Any(ProductExistsInOrderAndCart(cartLine))));

        }

        private static Func<CartLineComponent, bool> ProductExistsInOrderAndCart(CartLineComponent cartLine)
        {
            return orderLine => cartLine.GetComponent<CartProductComponent>().Id == orderLine.GetComponent<CartProductComponent>().Id;
        }
    }
}