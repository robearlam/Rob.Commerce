using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Internal;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Pipelines;

namespace Feature.Initialization.Engine.Pipelines.Blocks
{
    [PipelineDisplayName(HabitatConstants.Pipelines.Blocks.InitializeCatalogBlock)]
    public class InitializeCatalogBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ImportCatalogsCommand _importCatalogsCommand;

        public InitializeCatalogBlock(IHostingEnvironment hostingEnvironment, ImportCatalogsCommand importCatalogsCommand)
        {
            _hostingEnvironment = hostingEnvironment;
            _importCatalogsCommand = importCatalogsCommand;
        }

        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var artifactSet = "Environment.Habitat.Catalog-1.0";
            if (!context.GetPolicy<EnvironmentInitializationPolicy>().InitialArtifactSets.Contains(artifactSet))
                return arg;

            using (var stream = new FileStream(GetPath("Habitat.zip"), FileMode.Open, FileAccess.Read))
            {
                var file = new FormFile(stream, 0, stream.Length, stream.Name, stream.Name);
                await _importCatalogsCommand.Process(context.CommerceContext, file, CatalogConstants.ImportMode.Replace, 10);
            }

            return arg;
        }

        private string GetPath(string fileName)
        {
            return Path.Combine(_hostingEnvironment.WebRootPath, "data", "Catalogs", fileName);
        }
    }
}
