using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dispel.Web.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty, Required, Display(Name = "Log File")]
        public IFormFile LogFile { get; set; }

        [BindProperty, Required]
        public string Format { get; set; }

        public async Task<ActionResult> OnPostAsync()
        {
            var f = Formats.Parse(Format);

            using (var file = LogFile.OpenReadStream())
            {
                await Engine.ConvertSingleAsync(file, Response.Body, f);
            }

            return new EmptyResult();
        }
    }
}
