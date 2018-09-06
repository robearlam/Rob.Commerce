using System;
using System.Threading.Tasks;
using System.Web.Http.OData;
using Feature.Orders.ServiceBus.Engine.Commands;
using Microsoft.AspNetCore.Mvc;
using Sitecore.Commerce.Core;

namespace Feature.Orders.ServiceBus.Engine.Controllers
{
    public class CommandsController : CommerceController
    {
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment) : base(serviceProvider, globalEnvironment)
        {
        }

        [HttpPut]
        [Route("SendtoServiceBusCommand()")]
        public async Task<IActionResult> SendtoServiceBusCommand([FromBody] ODataActionParameters value)
        {
            var orderid = value["orderId"].ToString();
            var command = Command<SendtoServiceBusCommand>();
            await command.Process(CurrentContext, orderid);

            return new ObjectResult(command);
        }
    }
}