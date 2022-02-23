using Microsoft.AspNetCore.Mvc;
using WhiteRabbit.Messaging.Abstractions;
using WhiteRabbit.Shared;

namespace WhiteRabbit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IMessageSender messageSender;

    public InvoicesController(IMessageSender messageSender)
    {
        this.messageSender = messageSender;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Post(Invoice invoice)
    {
        await messageSender.PublishAsync(invoice);
        return Accepted();
    }
}
