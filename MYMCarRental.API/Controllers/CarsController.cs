using Microsoft.AspNetCore.Mvc;
using MYMCarRental.Application.DTOs.Cars;
using MYMCarRental.Application.Interfaces;

namespace MYMCarRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    // =========================
    // GET: api/cars
    // =========================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cars = await _carService.GetAllAsync();

        return Ok(cars);
    }

    // =========================
    // GET: api/cars/featured
    // =========================

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured()
    {
        var cars = await _carService.GetFeaturedAsync();

        return Ok(cars);
    }

    // =========================
    // GET: api/cars/{id}
    // =========================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var car = await _carService.GetByIdAsync(id);

        if (car is null)
        {
            return NotFound(new
            {
                message = "Car not found."
            });
        }

        return Ok(car);
    }

    // =========================
    // POST: api/cars
    // =========================

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(
        [FromForm] CreateCarDto dto,
        [FromForm] List<IFormFile>? images)
    {
        try
        {
            var imageStreams =
                new List<(Stream Stream, string FileName)>();

            try
            {
                if (images is not null)
                {
                    foreach (var image in images)
                    {
                        if (image.Length == 0)
                        {
                            return BadRequest(new
                            {
                                message = "One of the uploaded images is empty."
                            });
                        }

                        imageStreams.Add(
                            (image.OpenReadStream(), image.FileName));
                    }
                }

                var car = await _carService.CreateAsync(
                    dto,
                    imageStreams);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = car.Id },
                    car);
            }
            finally
            {
                foreach (var image in imageStreams)
                {
                    await image.Stream.DisposeAsync();
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // =========================
    // PUT: api/cars/{id}
    // =========================

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(
        int id,
        [FromForm] UpdateCarDto dto,
        [FromForm] List<IFormFile>? images)
    {
        try
        {
            var imageStreams =
                new List<(Stream Stream, string FileName)>();

            try
            {
                if (images is not null)
                {
                    foreach (var image in images)
                    {
                        if (image.Length == 0)
                        {
                            return BadRequest(new
                            {
                                message = "One of the uploaded images is empty."
                            });
                        }

                        imageStreams.Add(
                            (image.OpenReadStream(), image.FileName));
                    }
                }

                var car = await _carService.UpdateAsync(
                    id,
                    dto,
                    imageStreams);

                if (car is null)
                {
                    return NotFound(new
                    {
                        message = "Car not found."
                    });
                }

                return Ok(car);
            }
            finally
            {
                foreach (var image in imageStreams)
                {
                    await image.Stream.DisposeAsync();
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // =========================
    // DELETE: api/cars/{id}
    // =========================

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _carService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Car not found."
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

    // =========================
    // DELETE: api/cars/{carId}/images/{imageId}
    // =========================

    [HttpDelete("{carId:int}/images/{imageId:int}")]
    public async Task<IActionResult> DeleteImage(
        int carId,
        int imageId)
    {
        try
        {
            var deleted = await _carService.DeleteImageAsync(
                carId,
                imageId);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Image not found."
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

    // =========================
    // PUT: api/cars/{carId}/images/{imageId}/primary
    // =========================

    [HttpPut("{carId:int}/images/{imageId:int}/primary")]
    public async Task<IActionResult> SetPrimaryImage(
        int carId,
        int imageId)
    {
        var updated = await _carService.SetPrimaryImageAsync(
            carId,
            imageId);

        if (!updated)
        {
            return NotFound(new
            {
                message = "Image not found."
            });
        }

        return NoContent();
    }
}