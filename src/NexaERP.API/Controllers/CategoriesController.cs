using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexaERP.BLL.DTOs.Category;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.API.Controllers;

[Authorize]
[Route("categories")]
[ApiController]
public class CategoriesController(
    ICategoryRepository categoryRepository)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
    {
        List<CategoryDto> categories = await categoryRepository
            .GetAll()
            .Select(CategoryMapping.ProjectToDto())
            .ToListAsync();

        return Ok(categories);
    }
}
