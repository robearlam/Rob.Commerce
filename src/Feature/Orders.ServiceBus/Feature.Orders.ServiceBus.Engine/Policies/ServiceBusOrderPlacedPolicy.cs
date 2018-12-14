using Sitecore.Commerce.Core;

namespace Feature.Orders.ServiceBus.Engine.Policies
{
    public class ServiceBusOrderPlacedPolicy : Policy
    {
        public string OrderPlacedListName { get; set; }
        public bool Enabled { get; set; }
        public string Icon { get; set; }
        public string OrderSentListName { get; set; }
    }
}