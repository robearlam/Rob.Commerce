using System;
using System.Threading.Tasks;
using System.Web.Http.OData;
using Feature.Compare.Engine.Commands;
using Microsoft.AspNetCore.Mvc;
using Sitecore.Commerce.Core;

namespace Feature.Compare.Engine.Controllers
{
    public class CommandsController : CommerceController
    {
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment) : base(serviceProvider, globalEnvironment)
        {
        }

        [HttpPost]
        [Route("AddProductToComparison()")]
        public async Task<IActionResult> AddProductToComparison([FromBody] ODataActionParameters value)
        {
            if (!ModelState.IsValid || value == null)
            {
                return new BadRequestObjectResult(ModelState);
            }

            if (!value.ContainsKey("cartId") || !value.ContainsKey("productId") || !value.ContainsKey("catalogName") || !value.ContainsKey("varientId"))
            {
                return new BadRequestObjectResult(value);
            }

            var cartId = value["cartId"].ToString();
            var catalogName = value["catalogName"].ToString();
            var productId = value["productId"].ToString();
            var varientId = value["varientId"].ToString();
            if (string.IsNullOrWhiteSpace(cartId) || string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(catalogName))
            {
                return new BadRequestObjectResult(value);
            }

            var command = Command<AddToProductCompareCommand>();
            await command.Process(CurrentContext, cartId, catalogName, productId, varientId);
            return new ObjectResult(command);
        }

        [HttpDelete]
        [Route("RemoveProductFromComparison()")]
        public async Task<IActionResult> RemoveProductFromComparison([FromBody] ODataActionParameters value)
        {
            if (!ModelState.IsValid || value == null)
            {
                return new BadRequestObjectResult(ModelState);
            }

            if (!value.ContainsKey("cartId") || !value.ContainsKey("sellableItemId"))
            {
                return new BadRequestObjectResult(value);
            }

            var cartId = value["cartId"].ToString();
            var sellableItemId = value["sellableItemId"].ToString();
            if (string.IsNullOrWhiteSpace(cartId) || string.IsNullOrWhiteSpace(sellableItemId))
            {
                return new BadRequestObjectResult(value);
            }

            var command = Command<RemoveFromProductCompareCommand>();
            await command.Process(CurrentContext, cartId, sellableItemId);
            return new ObjectResult(command);
        }
    }
}
