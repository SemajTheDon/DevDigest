using DevDigest.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevDigest.Web.Controllers;

[ApiController]
[Route("api/articles")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService) =>
        _articleService = articleService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticle(int id)
    {
        var article = await _articleService.GetArticleAsync(id);

        return article is null ? NotFound() : Ok(article);
    }
}
