using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MYMCarRental.Application.DTOs.Contact;
using MYMCarRental.Application.Interfaces;

namespace MYMCarRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ContactController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost]
    [EnableRateLimiting("ContactLimiter")]
    public async Task<IActionResult> SendMessage(
        [FromBody] SendContactMessageDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _contactService.SendMessageAsync(
                dto,
                cancellationToken);

            return Ok(new
            {
                message = "Your message has been sent successfully."
            });
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "Unable to send your message. Please try again later."
                });
        }
    }
}