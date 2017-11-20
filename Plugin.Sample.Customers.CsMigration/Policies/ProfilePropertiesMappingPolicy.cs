// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfilePropertiesMappingPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System.Collections.Generic;
    using Sitecore.Commerce.Core;

    /// <summary>
    /// Defines a Profile properties mapping policy
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Policy" />
    public class ProfilePropertiesMappingPolicy : Policy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilePropertiesMappingPolicy"/> class.
        /// </summary>
        public ProfilePropertiesMappingPolicy()
        {
            this.ProfileProperties = new Dictionary<string, string>()
            {
                { "language", "Language" },
                { "tel_number", "PhoneNumber" }
            };
        }

        /// <summary>
        /// Gets or sets the profile properties.
        /// </summary>
        /// <value>
        /// The profile properties.
        /// </value>
        public Dictionary<string, string> ProfileProperties { get; set; }
    }       
}
