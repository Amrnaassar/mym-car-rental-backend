using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MYMCarRental.Application.DTOs.Locations;
using MYMCarRental.Application.Interfaces;

namespace MYMCarRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(
        ILocationService locationService)
    {
        _locationService = locationService;
    }

    // GET: api/Locations
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var locations =
            await _locationService.GetAllAsync();

        return Ok(locations);
    }

    // GET: api/Locations/active
    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActive()
    {
        var locations =
            await _locationService.GetActiveAsync();

        return Ok(locations);
    }

    // GET: api/Locations/1
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var location =
            await _locationService.GetByIdAsync(id);

        if (location is null)
        {
            return NotFound(new
            {
                message = "Location not found."
            });
        }

        return Ok(location);
    }

    // POST: api/Locations
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromBody] CreateLocationDto dto)
    {
        try
        {
            var location =
                await _locationService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = location.Id },
                location);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // PUT: api/Locations/1
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateLocationDto dto)
    {
        try
        {
            var updated =
                await _locationService.UpdateAsync(
                    id,
                    dto);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Location not found."
                });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // DELETE: api/Locations/1
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted =
                await _locationService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Location not found."
                });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }
}