using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Dispel.Web.Pages
{
    public class IndexModel : PageModel
    {
        [Required]
        public string Name { get; set; }

        public void OnGet()
        {

        }
    }
}
