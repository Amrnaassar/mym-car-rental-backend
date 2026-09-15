using MYMCarRental.Application.DTOs.Contact;

namespace MYMCarRental.Application.Interfaces;

public interface IContactService
{
    Task SendMessageAsync(
        SendContactMessageDto dto,
        CancellationToken cancellationToken = default);
}