using DevDigest.Web.Controllers;
using DevDigest.Web.Dtos;
using DevDigest.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DevDigest.Web.Tests.Controllers;

public class ArticlesControllerTests
{
    [Fact]
    public async Task GetArticle_WhenArticleDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IArticleService>();
        mockService
            .Setup(service => service.GetArticleAsync(999))
            .ReturnsAsync((ArticleResponse?)null);

        var controller = new ArticlesController(mockService.Object);

        // Act
        var result = await controller.GetArticle(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
