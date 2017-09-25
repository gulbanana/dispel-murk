using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Dispel.Web.Pages
{
    public class IndexModel : PageModel
    {
        [Required, Display(Name = "Log File")]
        public IFormFile LogFile { get; set; }
    }
}
