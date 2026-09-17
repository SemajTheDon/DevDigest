using DevDigest.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevDigest.Web.Pages;

[IgnoreAntiforgeryToken]
public class AutomationModel : PageModel
{
    private readonly DigestAutomationService _digestAutomationService;
    private readonly IConfiguration _configuration;

    public AutomationModel(
        DigestAutomationService digestAutomationService,
        IConfiguration configuration)
    {
        _digestAutomationService = digestAutomationService;
        _configuration = configuration;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Read the secret trigger key from configuration
        var expectedKey =
            _configuration["DigestAutomation:TriggerKey"];

        // Read the key GitHub/curl sent in the HTTP header
        var providedKey =
            Request.Headers["X-DevDigest-Key"].FirstOrDefault();

        // Make sure the keys match
        if (string.IsNullOrWhiteSpace(expectedKey) ||
            providedKey != expectedKey)
        {
            return Unauthorized();
        }

        // Key is valid — run the full pipeline
        await _digestAutomationService.RunDailyDigestAsync();

        return new JsonResult(new
        {
            success = true,
            message = "Daily digest completed."
        });
    }
}