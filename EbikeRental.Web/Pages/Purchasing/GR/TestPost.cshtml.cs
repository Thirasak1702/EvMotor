using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Purchasing.GR;

public class TestPostModel : PageModel
{
    private readonly IGoodsReceiptService _grService;

    public TestPostModel(IGoodsReceiptService grService)
    {
        _grService = grService;
    }

    public string Message { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id == 0)
        {
            Message = "? Please provide GR ID. Example: /Purchasing/GR/TestPost?id=1";
            return Page();
        }

        Console.WriteLine($"?? TEST POST: Starting test for GR ID: {id}");

        try
        {
            var userId = 1; // Test user
            var result = await _grService.PostAsync(id, userId);

            if (result.Success)
            {
                Message = $"? SUCCESS: {result.Message}";
                Console.WriteLine($"?? TEST POST: {Message}");
            }
            else
            {
                Message = $"? FAILED: {result.Message}";
                Console.WriteLine($"?? TEST POST: {Message}");
            }
        }
        catch (Exception ex)
        {
            Message = $"?? EXCEPTION: {ex.Message}";
            Console.WriteLine($"?? TEST POST: {Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
        }

        return Page();
    }
}
