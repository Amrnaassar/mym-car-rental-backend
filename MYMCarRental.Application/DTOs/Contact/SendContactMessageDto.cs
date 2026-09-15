using System.ComponentModel.DataAnnotations;

namespace MYMCarRental.Application.DTOs.Contact;

public sealed class SendContactMessageDto
{
    [Required]
    [StringLength(100)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(30)]
    public string Phone { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Message { get; init; } = string.Empty;
}