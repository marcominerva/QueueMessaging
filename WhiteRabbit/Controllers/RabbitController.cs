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
        private readonly IMessageSender messageSender;

        public RabbitController(IMessageSender messageSender)
        {
            this.messageSender = messageSender;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var test = new Test { Name = "Pippo" };
            await messageSender.PublishAsync(test);

            return NoContent();
        }
    }
}
