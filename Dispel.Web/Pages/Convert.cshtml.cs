using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Dispel.Web.Pages
{
    public class DownloadModel : PageModel
    {
        [BindProperty, Required]
        public IFormFile LogFile { get; set; }

        [BindProperty, BindRequired]
        public string Output { get; set; }

        public ActionResult OnGet() => RedirectToPage("Error");

        public async Task<ActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("Error");
            }

            await Task.Yield();

            using (var file = LogFile.OpenReadStream())
            {
                using (var stream = new MemoryStream())
                {
                    await Engine.ConvertAsync(file, stream, OutputFormat.HTML);
                    Output = Encoding.UTF8.GetString(stream.ToArray());
                }
            }

            return Page();
        }
    }
}
