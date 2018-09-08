using Sitecore.Commerce.Core;

namespace Feature.Orders.ServiceBus.Engine.Policies
{
    public class ServiceBusConnectionPolicy : Policy
    {
        public string ApplicationId { get; set; }
        public string Mcskey { get; set; }
        public string TenantId { get; set; }
        public string EndPoint { get; set; }
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
    }
}