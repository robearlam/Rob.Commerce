using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;

namespace Feature.ProductImport.Engine.Pipelines.Arguments
{
    public class ImportCsvProductsArgument: PipelineArgument
    {
        public ImportCsvProductsArgument(IFormFile importFile, string mode)
        {
            Condition.Requires(importFile, nameof(importFile)).IsNotNull();
            Condition.Requires(mode, nameof(mode)).IsNotNullOrWhiteSpace();
            ImportFile = importFile;
            Mode = mode;
        }

        public IFormFile ImportFile { get; set; }

        public string Mode { get; set; }

        public int ErrorThreshold { get; set; } = 100;

        public IList<string> FileLines { get; set; }
    }
}
