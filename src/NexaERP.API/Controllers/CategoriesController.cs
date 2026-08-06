using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using NexaERP.BLL.DTOs.Category;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Authorization;
using NexaERP.DAL.Caching;
using NexaERP.DAL.Extensions;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.API.Controllers;

[EnableRateLimiting(RateLimitingPolicies.Default)]
[Authorize]
[Route("categories")]
[ApiController]
public class CategoriesController(
    CacheService cacheService,
    ICategoryRepository categoryRepository)
    : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.CategoriesRead)]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
    {
        const string cacheKey = "categories:all";

        List<CategoryDto>? cachedCategories =
            await cacheService.GetAsync<List<CategoryDto>>(cacheKey);

        if (cachedCategories is not null)
        {
            return Ok(cachedCategories);
        }

        List<CategoryDto> categories =
            await categoryRepository
                .GetAll()
                .Select(CategoryMapping.ProjectToDto())
                .ToListAsync();

        await cacheService.SetAsync(
            cacheKey,
            categories,
            TimeSpan.FromMinutes(30));

        return Ok(categories);
    }
}
