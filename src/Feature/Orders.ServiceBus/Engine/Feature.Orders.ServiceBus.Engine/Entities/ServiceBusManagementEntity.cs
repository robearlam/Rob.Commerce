using System;
using System.Collections.Generic;
using Feature.Orders.ServiceBus.Engine.Components;
using Microsoft.AspNetCore.OData.Builder;
using Sitecore.Commerce.Core;

namespace Feature.Orders.ServiceBus.Engine.Entities
{
    public class ServiceBusManagementEntity : CommerceEntity
    {
        [Contained]
        public IEnumerable<SampleComponent> ChildComponents { get; set; }

        public ServiceBusManagementEntity()
        {
            Components = new List<Component>();
            DateCreated = DateTime.UtcNow;
            DateUpdated = DateCreated;
        }

        public ServiceBusManagementEntity(string id) : this()
        {
            Id = id;
        }
    }
}