// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeCSCatalogBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.AdventureWorks
{
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Ensure Habitat catalog has been loaded.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{System.String, System.String,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(AwConstants.Pipelines.Blocks.InitializeCSCatalogBlock)]
    public class InitializeCSCatalogBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly ImportCSCatalogCommand _importCSCatalogCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeCSCatalogBlock"/> class.
        /// </summary>
        /// <param name="importCSCatalogCommand">The import catalog command.</param>
        public InitializeCSCatalogBlock(
            ImportCSCatalogCommand importCSCatalogCommand)
        {
            _importCSCatalogCommand = importCSCatalogCommand;
        }

        /// <summary>
        /// Executes the block.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var artifactSet = "Environment.AdventureWorks.Catalog-1.0";

            // Check if this environment has subscribed to this Artifact Set
            if (!context.GetPolicy<EnvironmentInitializationPolicy>().InitialArtifactSets.Contains(artifactSet))
            {
                return arg;
            }
            
            await _importCSCatalogCommand.Process(context.CommerceContext, "AdventureWorks_Master.xml", "AdventureWorks_Inventory.xml");

            return arg;
        }
    }
}
