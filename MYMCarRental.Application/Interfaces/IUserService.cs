using MYMCarRental.Application.DTOs.Users;

namespace MYMCarRental.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);

    Task<UserDto?> GetByEmailAsync(string email);

    Task<IEnumerable<UserDto>> GetAllAsync();

    Task<bool> UpdateRoleAsync(Guid id, UpdateUserRoleDto dto);

    Task<bool> DeleteAsync(Guid id);
}