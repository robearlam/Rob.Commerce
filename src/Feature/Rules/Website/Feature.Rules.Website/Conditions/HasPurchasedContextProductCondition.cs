using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.CustomModels.Goals;
using Sitecore.Commerce.CustomModels.Models;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;

namespace Feature.Rules.Website.Conditions
{
    public class HasPurchasedContextProductCondition<T> : StringOperatorCondition<T> where T : RuleContext
    {
        public int Days { get; set; }
        protected override bool Execute(T ruleContext)
        {
            if (!(ServiceLocator.ServiceProvider.GetService(typeof(ISiteContext)) is ISiteContext siteContext) || 
               !siteContext.IsProduct ||
               !Sitecore.Context.IsLoggedIn)
            {
                return false;
            }

            var productId = siteContext.CurrentCatalogItem?.Name;
            return DidUserOrderProductInTimeframe(productId, Days);
        }

        protected bool DidUserOrderProductInTimeframe(string productId, int pastDaysAmount)
        {
            var orderGoals = GetSubmittedOrderGoals(pastDaysAmount);
            return orderGoals.Any(orderGoal => orderGoal.Order.CartLines.Any(cartline => DoesProductIdMatch(productId, cartline)));
        }

        private static bool DoesProductIdMatch(string productId, CartLine cartline)
        {
            return (cartline.Product.ProductId.Contains("|")
                       ? cartline.Product.ProductId.Split('|')[1]
                       : cartline.Product.ProductId) == productId;
        }

        public List<VisitorOrderCreatedGoal> GetSubmittedOrderGoals(double? pastDaysAmount)
        {
            var submittedOrderOutcomes = new List<VisitorOrderCreatedGoal>();

            using (var client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    var reference = new IdentifiedContactReference("CommerceUser", Sitecore.Context.GetUserName());
                    var interactionFacets = client.Model.Facets.Where(c => c.Target == EntityType.Interaction).Select(x => x.Name);
                    var contact = client.Get(reference, new ContactExpandOptions()
                    {
                        Interactions = new RelatedInteractionsExpandOptions(interactionFacets.ToArray())
                        {
                            EndDateTime = DateTime.MaxValue,
                            StartDateTime = DateTime.MinValue
                        }
                    });

                    if (contact != null)
                    {
                        foreach (var interaction in contact.Interactions)
                        {
                            var submittedOrderEvents = interaction.Events.OfType<VisitorOrderCreatedGoal>().OrderBy(ev => ev.Timestamp).ToList();
                            if (pastDaysAmount.HasValue)
                                submittedOrderEvents = submittedOrderEvents.Where(ev => ev.Timestamp > DateTime.Now.AddDays(-(pastDaysAmount.Value))).ToList();
                            submittedOrderOutcomes.AddRange(submittedOrderEvents);
                        }
                    }
                }
                catch (XdbExecutionException ex)
                {
                    Log.Error("Failed to get contact", ex, this);
                }
            }

            return submittedOrderOutcomes;
        }
    }
}