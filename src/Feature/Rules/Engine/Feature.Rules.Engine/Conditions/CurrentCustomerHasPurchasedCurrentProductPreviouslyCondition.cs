using Sitecore.Commerce.Core;
using Sitecore.Framework.Rules;

namespace Feature.Rules.Engine.Conditions
{
    [EntityIdentifier("CurrentCustomerHasPurchasedCurrentProductPreviouslyCondition")]
    public class CurrentCustomerHasPurchasedCurrentProductPreviouslyCondition : ICondition
    {
        public IRuleValue<int> Days { get; set; }

        public bool Evaluate(IRuleExecutionContext context)
        {
            CommerceContext commerceContext = context.Fact<CommerceContext>((string)null);
            //Cart cart = commerceContext != null ? commerceContext.GetObject<Cart>() : (Cart)null;
            //if (cart == null || string.IsNullOrEmpty(this.CustomerId.Yield(context)) || (this.Operator == null || !cart.HasComponent<ContactComponent>()))
            //    return false;
            //return this.Operator.Evaluate(cart.GetComponent<ContactComponent>().CustomerId, this.CustomerId.Yield(context));
            return true;
        }
    }
}
