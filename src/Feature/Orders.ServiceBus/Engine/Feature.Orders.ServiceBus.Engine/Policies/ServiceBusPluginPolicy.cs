using Sitecore.Commerce.Core;

namespace Feature.Orders.ServiceBus.Engine.Policies
{
    public class ServiceBusPluginPolicy : Policy
    {
        public string Icon { get; set; }
        public string ApplicationId { get; set; }
        public string Mcskey { get; set; }
        public string TenantId { get; set; }
        public string QueueName { get; set; }
        public string ListToWatch { get; set; }
        public string SentOrdersList { get; set; }
        public bool IsDisabled { get; set; }
        public string EndPoint { get; set; }
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
    }
}