using LMS.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LMS.Pages
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
    public class LaboratoryWorksModel : PageModel
    {
        [BindProperty]
        public LaboratoryWorkDTO laboratoryWorkDTO { get; set; }

        [BindProperty]
        public int CourseID { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var id = HttpContext.Session.GetInt32("CourseId");
            if (id == null)
            {
                return NotFound(" урс не найден");
            }
            return Page();
        }
    }
}