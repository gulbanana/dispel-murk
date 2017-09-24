using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dispel.Web.Pages
{
    public class DownloadModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }

        public void OnGet()
        {

        }

        public void OnPost()
        {

        }
    }
}
