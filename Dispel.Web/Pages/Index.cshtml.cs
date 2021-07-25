using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dispel.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DispelOptions options;

        [BindProperty, Required, Display(Name = "Log File")]
        public IFormFile LogFile { get; set; }

        [BindProperty, Required]
        public string Format { get; set; }

        public IndexModel(IOptions<DispelOptions> options)
        {
            this.options = options.Value;
        }

        public async Task<ActionResult> OnPostAsync()
        {
            var e = new Engine(options);
            var f = Formats.Parse(Format);

            using (var file = LogFile.OpenReadStream())
            {
                await e.ConvertSingleAsync(file, Response.Body, f);
            }

            return new EmptyResult();
        }
    }
}
