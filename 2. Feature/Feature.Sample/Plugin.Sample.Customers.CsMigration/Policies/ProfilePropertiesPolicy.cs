// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfilePropertiesPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   A ProfilesCs properties policy
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using Sitecore.Commerce.Core;

    /// <summary>
    /// A ProfilesCs properties policy
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Policy" />
    public class ProfilePropertiesPolicy : Policy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilePropertiesPolicy"/> class.
        /// </summary>
        public ProfilePropertiesPolicy()
        {
            this.AddressType = "Address";
            this.UserObjectType = "UserObject";
            this.GeneralInfoPropertyGroup = "GeneralInfo";
            this.AccountInfoPropertyGroup = "AccountInfo";
            this.UserIdProperty = "user_id";
            this.AccountStatusProperty = "account_status";
            this.FirstNameProperty = "first_name";
            this.LastNameProperty = "last_name";
            this.EmailAddressProperty = "email_address";
            this.AddressListProperty = "address_list";
            this.ExternalIdProperty = "ExternalId";
            this.PasswordProperty = "user_security_password";
            this.AddressIdProperty = "address_id";           
            this.Country = "Country";
            this.CountryCode = "CountryCode";
            this.State = "State";
            this.StateCode = "StateCode";
            this.City = "City";
            this.AccountNumber = "AccountNumber";
            this.Email = "Email";
            this.Password = "Password";
            this.Value = "Value";
            this.UserTypeSiteTerm = "UserType";
            this.AccountStatusSiteTerm = "AccountStatus";
            this.Languages = "Languages";
            this.SitecoreDomainName = "CommerceUsers";
        }

        /// <summary>
        /// Gets or sets the type of the address.
        /// </summary>
        /// <value>
        /// The type of the address.
        /// </value>
        public string AddressType { get; set; }

        /// <summary>
        /// Gets or sets the type of the user object.
        /// </summary>
        /// <value>
        /// The type of the user object.
        /// </value>
        public string UserObjectType { get; set; }

        /// <summary>
        /// Gets or sets the name of the sitecore domain.
        /// </summary>
        /// <value>
        /// The name of the sitecore domain.
        /// </value>
        public string SitecoreDomainName { get; set; }

        /// <summary>
        /// Gets or sets the general information property group.
        /// </summary>
        /// <value>
        /// The general information property group.
        /// </value>
        public string GeneralInfoPropertyGroup { get; set; }

        /// <summary>
        /// Gets or sets the account information property group.
        /// </summary>
        /// <value>
        /// The account information property group.
        /// </value>
        public string AccountInfoPropertyGroup { get; set; }

        /// <summary>
        /// Gets or sets the user identifier property.
        /// </summary>
        /// <value>
        /// The user identifier property.
        /// </value>
        public string UserIdProperty { get; set; }

        /// <summary>
        /// Gets or sets the account status property.
        /// </summary>
        /// <value>
        /// The account status property.
        /// </value>
        public string AccountStatusProperty { get; set; }

        /// <summary>
        /// Gets or sets the first name property.
        /// </summary>
        /// <value>
        /// The first name property.
        /// </value>
        public string FirstNameProperty { get; set; }

        /// <summary>
        /// Gets or sets the last name property.
        /// </summary>
        /// <value>
        /// The last name property.
        /// </value>
        public string LastNameProperty { get; set; }

        /// <summary>
        /// Gets or sets the email address property.
        /// </summary>
        /// <value>
        /// The email address property.
        /// </value>
        public string EmailAddressProperty { get; set; }

        /// <summary>
        /// Gets or sets the address list property.
        /// </summary>
        /// <value>
        /// The address list property.
        /// </value>
        public string AddressListProperty { get; set; }

        /// <summary>
        /// Gets or sets the external identifier property.
        /// </summary>
        /// <value>
        /// The external identifier property.
        /// </value>
        public string ExternalIdProperty { get; set; }

        /// <summary>
        /// Gets or sets the password property.
        /// </summary>
        /// <value>
        /// The password property.
        /// </value>
        public string PasswordProperty { get; set; }

        /// <summary>
        /// Gets or sets the address identifier property.
        /// </summary>
        /// <value>
        /// The address identifier property.
        /// </value>
        public string AddressIdProperty { get; set; }       

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>
        /// The country.
        /// </value>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the state code.
        /// </summary>
        /// <value>
        /// The state code.
        /// </value>
        public string StateCode { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>
        /// The city.
        /// </value>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the account number.
        /// </summary>
        /// <value>
        /// The account number.
        /// </value>
        public string  AccountNumber { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        /// <value>
        /// The languages.
        /// </value>
        public string Languages { get; set; }

        /// <summary>
        /// Gets or sets the user type site term.
        /// </summary>
        /// <value>
        /// The user type site term.
        /// </value>
        public string UserTypeSiteTerm { get; set; }

        /// <summary>
        /// Gets or sets the account status site term.
        /// </summary>
        /// <value>
        /// The account status site term.
        /// </value>
        public string AccountStatusSiteTerm { get; set; }
    }
}
