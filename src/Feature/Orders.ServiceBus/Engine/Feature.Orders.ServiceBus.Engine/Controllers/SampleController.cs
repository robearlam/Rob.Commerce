using System;
using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Commands;
using Microsoft.AspNetCore.Mvc;
using Sitecore.Commerce.Core;

namespace Feature.Orders.ServiceBus.Engine.Controllers
{
    [Microsoft.AspNetCore.OData.EnableQuery]
    [Route("api/Sample")]
    public class SampleController : CommerceController
    {
        public SampleController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment) : base(serviceProvider, globalEnvironment)
        {
        }

        [HttpGet]
        [Route("(Id={id})")]
        [Microsoft.AspNetCore.OData.EnableQuery]
        public async Task<IActionResult> Get(string id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var process = Command<SendtoServiceBusCommand>()?.Process(CurrentContext, id);
            if (process == null)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var result = await process;
            if (result == null)
            {
                return NotFound();
            }

            return new ObjectResult(result);
        }
    }
}