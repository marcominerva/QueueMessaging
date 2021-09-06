using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WhiteRabbit.Messaging.Abstractions;
using WhiteRabbit.Shared;

namespace WhiteRabbit.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RabbitController : ControllerBase
    {
        private readonly IMessageManager messageManager;

        public RabbitController(IMessageManager messageManager)
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
