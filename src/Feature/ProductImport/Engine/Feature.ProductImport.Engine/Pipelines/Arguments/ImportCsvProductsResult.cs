using System.Collections.Generic;
using Sitecore.Commerce.Core;

namespace Feature.ProductImport.Engine.Pipelines.Arguments
{
    public class ImportCsvProductsResult : PipelineArgument
    {
        public bool Succeeded { get; set; }

        public string ResultCode { get; set; }

        public IList<CommandMessage> Errors { get; set; } = new List<CommandMessage>();
    }
}