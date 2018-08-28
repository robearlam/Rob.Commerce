using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.CustomModels.Goals;
using Sitecore.Commerce.CustomModels.Outcomes;
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
            var siteContext = ServiceLocator.ServiceProvider.GetService(typeof(ISiteContext)) as ISiteContext;
            if (siteContext == null || 
               !siteContext.IsProduct ||
               !Sitecore.Context.IsLoggedIn)
            {
                return false;
            }

            var productId = siteContext?.CurrentCatalogItem?.Name;
            return DidUserOrderProductInTimeframe(productId, Days);
        }

        protected bool DidUserOrderProductInTimeframe(string productId, int pastDaysAmount)
        {
            var orderOutcomes = GetSubmittedOrderOutcomes(pastDaysAmount);
            if (orderOutcomes.Count() > 0)
            {
                if (orderOutcomes.Where(o => o.Order.CartLines.Where(c => (c.Product.ProductId.Contains("|") ? c.Product.ProductId.Split('|')[1] : c.Product.ProductId) == productId).FirstOrDefault() != null).FirstOrDefault() != null)
                    return true;              
                
                return false;
            }
            else
                return false;
        }

        public IEnumerable<VisitorOrderCreatedGoal> GetSubmittedOrderOutcomes(double? pastDaysAmount)
        {
            var submittedOrderOutcomes = new List<VisitorOrderCreatedGoal>();

            using (XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    var reference = new IdentifiedContactReference("CommerceUser", Sitecore.Context.GetUserName());
                    var interactionFacets = client.Model.Facets.Where(c => c.Target == EntityType.Interaction).Select(x => x.Name);
                    Contact contact = client.Get<Contact>(reference, new ContactExpandOptions()
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