using System;
using System.IO;
using System.Web.Http.OData;
using Feature.ProductImport.Engine.Commands;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Sitecore.Commerce.Core;

namespace Feature.ProductImport.Engine.Controllers
{
    public class CommandsController : CommerceController
    {
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment) : base(serviceProvider, globalEnvironment)
        {
        }

        [HttpPut]
        [DisableRequestSizeLimit]
        [Route("ImportCsvProducts()")]
        public IActionResult ImportCsvProducts([FromBody] ODataActionParameters value)
        {
            if (!ModelState.IsValid || value == null)
                return new BadRequestObjectResult(ModelState);

            if (!value.ContainsKey("importFile"))
                return new BadRequestObjectResult(value);

            if(!(value["importFile"] is FormFile formFile))
                return new BadRequestObjectResult("Import file not correct type");

            var memoryStream = new MemoryStream();
            formFile.CopyTo(memoryStream);

            var file = new FormFile(memoryStream, 0L, formFile.Length, formFile.Name, formFile.FileName);
            var updateMode = value["mode"].ToString();

            var publishEntities = true;
            if (value.ContainsKey("publish") && bool.TryParse(value["publish"].ToString(), out var result))
                publishEntities = result;

            return new ObjectResult(ExecuteLongRunningCommand(() => Command<ImportCsvProductsCommand>().Process(CurrentContext, file, updateMode,  publishEntities)));
        }
    }
}