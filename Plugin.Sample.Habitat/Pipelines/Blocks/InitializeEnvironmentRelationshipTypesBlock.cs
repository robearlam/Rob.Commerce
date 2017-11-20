// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeEnvironmentRelationshipTypesBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Habitat
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which bootstraps relationship types in the Habitat sample environment.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{System.String, System.String,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(HabitatConstants.Pipelines.Blocks.InitializeEnvironmentRelationshipTypesBlock)]
    public class InitializeEnvironmentRelationshipTypesBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;
        private readonly IFindEntityPipeline _findEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeEnvironmentRelationshipTypesBlock"/> class.
        /// </summary>
        /// <param name="persistEntityPipeline">
        /// The persist entity pipeline.
        /// </param>
        /// <param name="findEntityPipeline">
        /// The find entity pipeline.
        /// </param>
        public InitializeEnvironmentRelationshipTypesBlock(IPersistEntityPipeline persistEntityPipeline, IFindEntityPipeline findEntityPipeline)
        {
            this._persistEntityPipeline = persistEntityPipeline;
            this._findEntityPipeline = findEntityPipeline;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="arg">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var artifactSet = "Environment.Habitat.RelationshipTypes-1.0";

            // Check if this environment has subscribed to this Artifact Set
            if (!context.GetPolicy<EnvironmentInitializationPolicy>()
                .InitialArtifactSets.Contains(artifactSet))
            {
                return arg;
            }

            context.Logger.LogInformation($"{this.Name}.InitializingArtifactSet: ArtifactSet={artifactSet}");

            await CreateRelationship("CatalogToCategory", "Represents a relationship between a catalog and a category", "Catalog", "Category", context);
            await CreateRelationship("CatalogToSellableItem", "Represents a relationship between a catalog and a sellable item", "Catalog", "SellableItem", context);
            await CreateRelationship("CategoryToCategory", "Represents a relationship between a category and another category", "Category", "Category", context);
            await CreateRelationship("CategoryToSellableItem", "Represents a relationship between a category and a sellable item", "Category", "SellableItem", context);

            await CreateRelationship("InventorySetToSellableItem", "Represents a relationship between an inventory set and a sellable item", "InventorySet", "SellableItem", context);
            await CreateRelationship("PromotionBookToCatalog", "Represents a relationship between a promotion book and a catalog", "PromotionBook", "Catalog", context);
            await CreateRelationship("PriceBookToCatalog", "Represents a relationship between a price book and a catalog", "PriceBook", "Catalog", context);

            return arg;
        }

        private async Task<string> CreateRelationship(string relationShipName, string description, string sourceType, string targetType, CommercePipelineExecutionContext context)
        {
            var itemId = $"{CommerceEntity.IdPrefix<RelationshipDefinition>()}{relationShipName}";
            var findResult = await this._findEntityPipeline.Run(new FindEntityArgument(typeof(RelationshipDefinition), itemId), context.CommerceContext.GetPipelineContextOptions());

            if (findResult == null)
            {
                var relationship = new RelationshipDefinition
                {
                    Id = itemId,
                    FriendlyId = relationShipName,
                    Name = relationShipName,
                    DisplayName = relationShipName,
                    RelationshipDescription = description,
                    SourceType = sourceType,
                    TargetType = targetType,
                    Components = new List<Component>
                    {
                        new ListMembershipsComponent
                        {
                            Memberships = new List<string>
                            {
                                context.GetPolicy<KnownRelationshipListsPolicy>().DefaultRelationshipDefinitions
                            }
                        }
                    }
                };

                await this._persistEntityPipeline.Run(new PersistEntityArgument(relationship), context);
            }

            return relationShipName;
        }
    }
}
