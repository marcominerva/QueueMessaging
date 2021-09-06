using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WhiteRabbit.Messaging;
using WhiteRabbit.Shared;

namespace WhiteRabbit.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RabbitController : ControllerBase
    {
        private readonly MessageManager messageManager;

        public RabbitController(MessageManager messageManager)
        {
            this.messageManager = messageManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var test = new Test { Name = "Pippo" };
            await messageManager.PublishAsync(test);

            return NoContent();
        }
    }
}
