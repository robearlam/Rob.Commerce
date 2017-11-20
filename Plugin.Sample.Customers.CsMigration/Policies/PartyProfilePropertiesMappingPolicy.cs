// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartyProfilePropertiesMappingPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   Defines a Party-Profile properties mapping policy
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System.Collections.Generic;   
    using Sitecore.Commerce.Core;

    /// <summary>
    /// Defines a Party-Profile properties mapping policy
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Policy" />
    public class PartyProfilePropertiesMappingPolicy : Policy
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="PartyProfilePropertiesMappingPolicy"/> class from being created.
        /// </summary>
        public PartyProfilePropertiesMappingPolicy()
        {
            this.PartyProfileProperties = new Dictionary<string, string>()
            {
                { "address_name", "AddressName" },
                { "first_name", "FirstName" },
                { "last_name", "LastName" },
                { "country_name", "Country" },
                { "country_code", "CountryCode" },
                { "region_name", "State" },
                { "region_code", "StateCode" },
                { "city", "City" },
                { "address_line1", "Address1" },
                { "address_line2", "Address2" },
                { "postal_code", "ZipPostalCode" },
                { "tel_number", "PhoneNumber" }              
            };
        }

        /// <summary>
        /// Gets or sets the party profile properties.
        /// </summary>
        /// <value>
        /// The party profile properties.
        /// </value>
        public Dictionary<string, string> PartyProfileProperties { get; set; }
    }       
}
