using Microsoft.AspNetCore.Mvc;
using MYMCarRental.Application.DTOs.Categories;
using MYMCarRental.Application.Interfaces;

namespace MYMCarRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET: api/categories
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();

        return Ok(categories);
    }

    // GET: api/categories/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category is null)
        {
            return NotFound(new
            {
                message = "Category not found."
            });
        }

        return Ok(category);
    }

    // GET: api/categories/slug/suv
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var category = await _categoryService.GetBySlugAsync(slug);

        if (category is null)
        {
            return NotFound(new
            {
                message = "Category not found."
            });
        }

        return Ok(category);
    }

    // POST: api/categories
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(
        [FromForm] CreateCategoryDto dto,
        IFormFile? image)
    {
        try
        {
            Stream? imageStream = null;

            if (image is not null)
            {
                if (image.Length == 0)
                {
                    return BadRequest(new
                    {
                        message = "The uploaded image is empty."
                    });
                }

                imageStream = image.OpenReadStream();
            }

            try
            {
                var category = await _categoryService.CreateAsync(
                    dto,
                    imageStream,
                    image?.FileName);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = category.Id },
                    category);
            }
            finally
            {
                if (imageStream is not null)
                {
                    await imageStream.DisposeAsync();
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

    // PUT: api/categories/1
    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(
        int id,
        [FromForm] UpdateCategoryDto dto,
        IFormFile? image)
    {
        try
        {
            Stream? imageStream = null;

            if (image is not null)
            {
                if (image.Length == 0)
                {
                    return BadRequest(new
                    {
                        message = "The uploaded image is empty."
                    });
                }

                imageStream = image.OpenReadStream();
            }

            try
            {
                var category = await _categoryService.UpdateAsync(
                    id,
                    dto,
                    imageStream,
                    image?.FileName);

                if (category is null)
                {
                    return NotFound(new
                    {
                        message = "Category not found."
                    });
                }

                return Ok(category);
            }
            finally
            {
                if (imageStream is not null)
                {
                    await imageStream.DisposeAsync();
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

    // DELETE: api/categories/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _categoryService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Category not found."
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