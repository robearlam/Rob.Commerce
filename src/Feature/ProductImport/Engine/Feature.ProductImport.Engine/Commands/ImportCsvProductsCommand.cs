using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;

namespace Feature.ProductImport.Engine.Commands
{
    public class ImportCsvProductsCommand : CommerceCommand
    {
        public async Task<CommerceCommand> Process(CommerceContext commerceContext, IFormFile importFile, string mode, bool publishEntities = true)
        {
            throw new NotImplementedException("Not implemented yet");
        }
    }
}
