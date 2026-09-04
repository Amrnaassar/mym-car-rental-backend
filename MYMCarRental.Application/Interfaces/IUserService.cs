using MYMCarRental.Application.DTOs.Users;

namespace MYMCarRental.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);

    Task<UserDto?> GetByEmailAsync(string email);

    Task<IEnumerable<UserDto>> GetAllAsync();

    Task<bool> DeleteAsync(Guid id);
}