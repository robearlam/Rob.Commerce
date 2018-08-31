using System.Linq;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Rules;

namespace Feature.Rules.Engine.Benefits
{
    [EntityIdentifier("CartItemPreviouslyPurchasedPercentOffAction")]
    public class CartItemsPreviouslyPurchasedPercentOffAction : IAction
    {
        public IRuleValue<int> PercentOff { get; set; }

        public void Execute(IRuleExecutionContext context)
        {
            var commerceContext = context.Fact<CommerceContext>();
            var commerceContext1 = commerceContext;
            var cart = commerceContext1?.GetObjects<Cart>().FirstOrDefault();
        }
    }
}