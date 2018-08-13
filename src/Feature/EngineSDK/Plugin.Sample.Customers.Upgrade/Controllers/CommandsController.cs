﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.Upgrade
{
    using Microsoft.AspNetCore.Mvc;
    using Sitecore.Commerce.Core;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    ///  Defines the commands controller for the upgrade customers plugin.
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.CommerceController" />
    public class CommandsController : CommerceController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandsController"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="globalEnvironment">The global environment.</param>
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment) : base(serviceProvider, globalEnvironment)
        {
        }

        /// <summary>
        /// Upgrades the customers.
        /// </summary>
        /// <returns>The list of upgraded customers</returns>
        [HttpPut]
        [Route("UpgradeCustomers()")]
        public async Task<IActionResult> UpgradeCustomers()
        {
            if (!this.ModelState.IsValid)
            {
                return new BadRequestObjectResult(this.ModelState);
            }

            var command = this.Command<UpgradeCustomersCommand>();
            await command.Process(this.CurrentContext).ConfigureAwait(continueOnCapturedContext: false);

            return new ObjectResult(command);
        }
    }
}
