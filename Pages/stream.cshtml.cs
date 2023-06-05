using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TheCircle.Pages;

public class StreamModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public StreamModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}