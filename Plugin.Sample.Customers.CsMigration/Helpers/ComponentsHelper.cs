// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentsHelper.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Customers;

    /// <summary>
    ///  A Components Helper to generate Components for Commerce Server Profiles system
    /// </summary>
    public class ComponentsHelper
    {
        /// <summary>
        /// Addresses the component generator.
        /// </summary>
        /// <param name="addressId">The address identifier.</param>
        /// <param name="profileDefinition">The profile definition.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A <see cref="AddressComponent" />
        /// </returns>
        internal async static Task<AddressComponent> AddressComponentGenerator(string addressId, ProfileDefinition profileDefinition, CommercePipelineExecutionContext context)
        {
            // map properties to AddressComponent
            var addressType = context.GetPolicy<ProfilePropertiesPolicy>().AddressType;
            var addressComponent = new AddressComponent { Name = addressType };
            try
            {
                var sqlContext = ConnectionHelper.GetProfilesSqlContext(context.CommerceContext);
                var address = await sqlContext.GetAddress(addressId);
                addressComponent.Id = addressId;               

                var partyProfileProperties = context.GetPolicy<PartyProfilePropertiesMappingPolicy>().PartyProfileProperties;
                var details = new EntityView { Name = "Details" };

                foreach (var property in profileDefinition.Properties)
                {
                    var rawValue = address[property.ColumnName] as string;
                    if (string.IsNullOrEmpty(rawValue) || property.Name.Equals("address_id", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (partyProfileProperties.ContainsKey(property.Name))
                    {
                        var propertyInfo = addressComponent.Party.GetType().GetProperty(partyProfileProperties[property.Name], BindingFlags.Public | BindingFlags.Instance);
                        if (propertyInfo != null && propertyInfo.CanWrite)
                        {
                            propertyInfo.SetValue(addressComponent.Party, rawValue, null);
                        }
                    }
                    else
                    {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(property.OriginalType);
                        var profileValue = typeConverter.ConvertFromString(rawValue);
                        details.Properties.Add(new ViewProperty { Name = $"{property.GroupName}-{property.Name}", RawValue = profileValue });
                    }                    
                }

                if (details.Properties.Any())
                {
                    addressComponent.View.ChildViews.Add(details);
                }
            }
            catch (Exception ex)
            {
                await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        "EntityNotFound",
                        new object[] { addressId, ex },
                        $"Address { addressId } was not found.");
                return null;
            }

            return addressComponent;
        }        

        internal static string GetPropertyOriginalType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return string.Empty;
            }

            switch (typeName.ToLower())
            {
                case "string":
                case "password":
                case "profile":
                case "siteterm":
                    return string.Empty.GetType().FullName;
                case "bool":
                    return false.GetType().FullName;
                case "datetime":
                    return DateTime.Today.GetType().FullName;
                case "number":
                    return 0.0M.GetType().FullName;
                default:
                    return typeName;
            }
        }
    }
}
