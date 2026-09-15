using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MYMCarRental.Application.DTOs.Users;
using MYMCarRental.Application.Interfaces;

namespace MYMCarRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // =========================
    // GET: api/users
    // =========================

    [HttpGet]
    [Authorize(Roles = "Manager")]
    [EnableRateLimiting("GeneralLimiter")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();

        return Ok(users);
    }

    // =========================
    // GET: api/users/{id}
    // =========================

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Manager")]
    [EnableRateLimiting("GeneralLimiter")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        return Ok(user);
    }

    // =========================
    // PUT: api/users/{id}/role
    // =========================

    [HttpPut("{id:guid}/role")]
    [Authorize(Roles = "Manager")]
    [EnableRateLimiting("GeneralLimiter")]
    public async Task<IActionResult> UpdateRole(
        Guid id,
        [FromBody] UpdateUserRoleDto dto)
    {
        var updated = await _userService.UpdateRoleAsync(id, dto);

        if (!updated)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        var user = await _userService.GetByIdAsync(id);

        return Ok(user);
    }

    // =========================
    // DELETE: api/users/{id}
    // =========================

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Manager")]
    [EnableRateLimiting("GeneralLimiter")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _userService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        return NoContent();
    }
}