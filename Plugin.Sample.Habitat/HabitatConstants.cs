// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HabitatConstants.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Habitat
{
    /// <summary>
    /// The Habitat constants.
    /// </summary>
    public static class HabitatConstants
    {
        /// <summary>
        /// The name of the Habitat pipelines.
        /// </summary>
        public static class Pipelines
        {
            /// <summary>
            /// The name of the Habitat pipeline blocks.
            /// </summary>
            public static class Blocks
            {
                /// <summary>
                /// The bootstrap aw sellable items block name.
                /// </summary>
                public const string BootstrapAwSellableItemsBlock = "Habitat.block.BootstrapAwSellableItems";
                
                /// <summary>
                /// The initialize catalog block name.
                /// </summary>
                public const string InitializeCSCatalogBlock = "Habitat.block.InitializeCSCatalog";

                /// <summary>
                /// The initialize environment relationship types block name.
                /// </summary>
                public const string InitializeEnvironmentRelationshipTypesBlock = "Habitat.block.InitializeEnvironmentRelationshipTypes";
            }
        }
    }
}
