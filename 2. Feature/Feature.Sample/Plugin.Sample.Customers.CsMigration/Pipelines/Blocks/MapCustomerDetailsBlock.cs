// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapCustomerDetailsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which maps customer details
    /// </summary>
    /// <seealso cref="Sitecore.Framework.Pipelines.PipelineBlock{DataRow, Customer, CommercePipelineExecutionContext}" />
    [PipelineDisplayName(CustomersCsConstants.Pipelines.Blocks.MapCustomerDetailsBlock)]
    public class MapCustomerDetailsBlock : PipelineBlock<DataRow, Customer, CommercePipelineExecutionContext>
    {
        private readonly IGetProfileDefinitionPipeline _getProfileDefinitionPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapCustomerDetailsBlock"/> class.
        /// </summary>
        /// <param name="getProfileDefinitionPipeline">The get profile definition pipeline.</param>
        public MapCustomerDetailsBlock(IGetProfileDefinitionPipeline getProfileDefinitionPipeline)
        {
            this._getProfileDefinitionPipeline = getProfileDefinitionPipeline;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The <see cref="Customer" />.
        /// </returns>
        public override async Task<Customer> Run(DataRow arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return null;
            }

            var customer = new Customer();

            try
            {
                var profileProperties = context.GetPolicy<ProfilePropertiesPolicy>();

                // get profile definition           
                var definitionResult = (await this._getProfileDefinitionPipeline.Run(string.Empty, context)).ToList();
                if (!definitionResult.Any())
                {
                    return customer;
                }

                context.CommerceContext.AddUniqueObjectByType(definitionResult);
                var profileDefinition = definitionResult.FirstOrDefault(d => d.Name.Equals(profileProperties?.UserObjectType));

                // map base properties

                var definitionProperty = profileDefinition.Properties.FirstOrDefault(p => p.Name.Equals(profileProperties?.UserIdProperty, StringComparison.OrdinalIgnoreCase));
                Guid id;
                string friendlyId = string.Empty;
                if (Guid.TryParse(arg[definitionProperty.ColumnName] as string, out id))
                {
                    friendlyId = id.ToString("D");
                }
                else
                {
                    // Azure search naming restriction
                    byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(arg[definitionProperty.ColumnName] as string);
                    friendlyId = System.Convert.ToBase64String(toEncodeAsBytes).TrimEnd('=');
                }
                 
                customer.AccountNumber = friendlyId;           
                customer.FriendlyId = friendlyId;
                customer.Id = $"{CommerceEntity.IdPrefix<Customer>()}{friendlyId}";

                definitionProperty = profileDefinition.Properties.FirstOrDefault(p => p.Name.Equals(profileProperties?.EmailAddressProperty, StringComparison.OrdinalIgnoreCase));
                customer.Email = arg[definitionProperty.ColumnName] as string;

                definitionProperty = profileDefinition.Properties.FirstOrDefault(p => p.Name.Equals(profileProperties?.AccountStatusProperty, StringComparison.OrdinalIgnoreCase));
                var csStatus = arg[definitionProperty.ColumnName] as string;
                customer.AccountStatus = csStatus != null && csStatus.Equals("1", StringComparison.InvariantCulture) ? context.GetPolicy<KnownCustomersStatusesPolicy>()?.ActiveAccount : context.GetPolicy<KnownCustomersStatusesPolicy>()?.InactiveAccount;

                definitionProperty = profileDefinition.Properties.FirstOrDefault(p => p.Name.Equals(profileProperties?.FirstNameProperty, StringComparison.OrdinalIgnoreCase));
                customer.FirstName = arg[definitionProperty.ColumnName] as string;

                definitionProperty = profileDefinition.Properties.FirstOrDefault(p => p.Name.Equals(profileProperties?.LastNameProperty, StringComparison.OrdinalIgnoreCase));
                customer.LastName = arg[definitionProperty.ColumnName] as string;

                var profileMappingProperties = context.GetPolicy<ProfilePropertiesMappingPolicy>().ProfileProperties;

                // map custom properties to CustomerDetailsComponent          
                var details = new EntityView { Name = "Details" };
                foreach (var property in profileDefinition?.Properties)
                {
                    if (property.Name.Equals(profileProperties?.EmailAddressProperty, StringComparison.OrdinalIgnoreCase)
                        || property.Name.Equals(profileProperties?.AccountNumber, StringComparison.OrdinalIgnoreCase)
                        || property.Name.Equals(profileProperties?.AccountStatusProperty, StringComparison.OrdinalIgnoreCase)
                        || property.Name.Equals(profileProperties?.FirstNameProperty, StringComparison.OrdinalIgnoreCase)
                        || property.Name.Equals(profileProperties?.LastNameProperty, StringComparison.OrdinalIgnoreCase)
                        || property.Name.Equals(profileProperties?.UserIdProperty, StringComparison.OrdinalIgnoreCase)
                        || property.Name.Equals(profileProperties?.PasswordProperty, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }               

                    TypeConverter typeConverter = TypeDescriptor.GetConverter(property.OriginalType);
                    var profileValue = typeConverter.ConvertFromString(arg[property.ColumnName] as string);

                    if (profileValue == null || string.IsNullOrEmpty(profileValue.ToString()))
                    {
                        continue;
                    }

                    if (property.TypeName.Equals("PROFILE", StringComparison.OrdinalIgnoreCase))
                    {
                        string[] array = profileValue.ToString().Split(';').Skip(1).ToArray();
                        details.Properties.Add(new ViewProperty { Name = $"{property.GroupName}-{property.Name}", RawValue = array });
                    }
                    else
                    {
                        details.Properties.Add(new ViewProperty { Name = $"{property.GroupName}-{property.Name}", RawValue = profileValue });
                        if (profileMappingProperties.ContainsKey(property.Name))
                        {
                            var profileProperty = profileMappingProperties[property.Name];
                            details.Properties.Add(new ViewProperty { Name = profileProperty, RawValue = profileValue });
                        }
                        else
                        {
                            details.Properties.Add(new ViewProperty { Name = $"{property.GroupName}-{property.Name}", RawValue = profileValue });
                        }
                    }
                }

                customer.GetComponent<CustomerDetailsComponent>()?.View.ChildViews.Add(details);

                customer.SetComponent(new ListMembershipsComponent
                {
                    Memberships = new List<string>
                    {
                        CommerceEntity.ListName<Customer>()
                    }
                });

                customer.SetComponent(new TransientListMembershipsComponent { Memberships = new List<string> { context.GetPolicy<KnownCustomersListsPolicy>().CustomersIndex } });
                return customer;
            }
            catch (Exception ex)
            {
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().Warning,
                    "EntityNotFound",
                    new object[] { arg["u_user_id"] as string, arg["u_email_address"] as string, ex },
                    $"Profile { arg["u_email_address"] as string } was not found.");
                return customer;
            }          
        }
    }
}
