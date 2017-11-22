using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Sitecore.Commerce.Core;
using SxaS.Compare.Engine.Commands;

namespace SxaS.Compare.Engine.Controllers
{
    [EnableQuery]
    [Route("api/Compare")]
    public class CompareController : CommerceController
    {
        public CompareController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment) : base(serviceProvider, globalEnvironment)
        {
        }

        [EnableQuery]
        [HttpGet]
        [Route("Compare")]
        public Task<IActionResult> Get()
        {
            throw new NotImplementedException();
        }

        [EnableQuery]
        [HttpGet]
        [Route("(Id={id})")]
        public async Task<IActionResult> Get(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(id))
                return NotFound();

            var compareComponent = await Command<GetProductCompareCommand>().Process(CurrentContext, id);
            return compareComponent != null ? new ObjectResult(compareComponent) : (IActionResult)NotFound();
        }
    }
}
