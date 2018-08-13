// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpgradeCustomersCommand.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2018
// </copyright>// 
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.Upgrade
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Framework.Conditions;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an upgrade customers command
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Commands.CommerceCommand" />
    public class UpgradeCustomersCommand : CommerceCommand
    {
        private readonly IFindEntitiesInListPipeline _findEntitiesInListPipeline;
        private readonly IUpgradeCustomerPipeline _upgradeCustomerPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeCustomersCommand" /> class.
        /// </summary>
        /// <param name="findEntitiesInListPipeline">The find entities in list pipeline.</param>
        /// <param name="upgradeCustomerPipeline">The upgrade customer pipeline.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public UpgradeCustomersCommand(
            IFindEntitiesInListPipeline findEntitiesInListPipeline,
            IUpgradeCustomerPipeline upgradeCustomerPipeline,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this._findEntitiesInListPipeline = findEntitiesInListPipeline;
            this._upgradeCustomerPipeline = upgradeCustomerPipeline;
        }

        /// <summary>
        /// Processes the specified commerce context.
        /// </summary>
        /// <param name="commerceContext">The commerce context.</param>
        /// <returns>
        /// Number of migrated customers
        /// </returns>
        public virtual async Task<int> Process(CommerceContext commerceContext)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var context = commerceContext.GetPipelineContextOptions();
                var listName = CommerceEntity.ListName<Customer>();

                var result = await this.Command<GetListMetadataCommand>().Process(commerceContext, listName);
                if (result == null)
                {
                    await commerceContext.AddMessage(
                        commerceContext.GetPolicy<KnownResultCodes>().Error,
                        "EntityNotFound",
                        new object[] { listName },
                        $"There is no customers in the list {listName}.");
                    return 0;
                }

                if (result.Count == 0)
                {
                    await context.CommerceContext.AddMessage(
                        context.CommerceContext.GetPolicy<KnownResultCodes>().Error,
                        "EntityNotFound",
                        new object[] { listName },
                        $"There is no customers in the list {listName}.");
                    return 0;
                }

                int customersCount = 0;
                int skip = 0;
                int take = 20;

                while (customersCount < result.Count)
                {
                    customersCount += await this.UpgradeCustomersInList(context, listName, skip, take);
                    skip += take;
                }
                
                return customersCount;
            }
        }

        /// <summary>
        /// Upgrades customers in list.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="take">The take.</param>
        /// <returns>Number of migrated customers</returns>
        protected virtual async Task<int> UpgradeCustomersInList(CommercePipelineExecutionContextOptions context, string listName, int skip, int take)
        {
            var findResult = await this._findEntitiesInListPipeline.Run(new FindEntitiesInListArgument(typeof(Customer), listName, skip, take) { LoadEntities = true }, context);
            if (findResult?.List?.Items == null || !findResult.List.Items.Any())
            {
                return 0;
            }

            foreach (var item in findResult.List.Items)
            {
                var cloneContext = this.CloneCommerceContext(context.CommerceContext);

                await this._upgradeCustomerPipeline.Run((Customer)item, cloneContext);
                this.MergeMessages(context.CommerceContext, cloneContext.CommerceContext);
            }

            return findResult.List.Items.Count;
        }

        /// <summary>
        /// Clones the commerce context.
        /// </summary>
        /// <param name="commerceContext">The commerce context.</param>
        /// <returns>Clone pipeline execution context</returns>
        protected virtual CommercePipelineExecutionContext CloneCommerceContext(CommerceContext commerceContext)
        {
            Condition.Requires(commerceContext, nameof(commerceContext)).IsNotNull();

            var commerceContextClone = new CommerceContext(commerceContext.Logger, commerceContext.TelemetryClient, commerceContext.LocalizableMessagePipeline)
            { 
                Environment = commerceContext.Environment,
                GlobalEnvironment = commerceContext.GlobalEnvironment,
                Headers = commerceContext.Headers
            };

            var commerceOptionsClone = commerceContextClone.GetPipelineContextOptions();
            return new CommercePipelineExecutionContext(commerceOptionsClone, commerceContext.Logger);
        }

        /// <summary>
        /// Copies messages from one context into another.
        /// </summary>
        /// <param name="targetContext">The context that will recieve the messages.</param>
        /// <param name="sourceContext">The context that is the source of the messages.</param>
        protected void MergeMessages(CommerceContext targetContext, CommerceContext sourceContext)
        {
            Condition.Requires(targetContext, nameof(targetContext)).IsNotNull();
            if (sourceContext != null)
            {
                foreach (var message in sourceContext.GetMessages())
                {
                    targetContext.AddMessage(message);
                }
            }
        }
    }
}
