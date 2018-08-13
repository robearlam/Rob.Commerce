// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomersUpgradeConstants.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.Upgrade
{
    /// <summary>
    /// The Upgradet constants.
    /// </summary>
    public static class CustomersUpgradeConstants
    {
        /// <summary>
        /// The name of the Upgradet pipelines.
        /// </summary>
        public static class Pipelines
        {
            /// <summary>
            /// The upgrade customer
            /// </summary>
            public const string UpgradeCustomer = "CustomersUpgrade:pipelines:UpgradeCustomer";

            /// <summary>
            /// The name of the "Customers Upgrade pipeline blocks.
            /// </summary>
            public static class Blocks
            {
                /// <summary>
                /// The configure service API block
                /// </summary>
                public const string ConfigureServiceApiBlock = "CustomersUpgrade:blocks:ConfigureServiceApi";

                /// <summary>
                /// The upgrade customer block
                /// </summary>
                public const string UpgradeCustomerBlock = "CustomersUpgrade:blocks:UpgradeCustomer";
            }
        }
    }
}
