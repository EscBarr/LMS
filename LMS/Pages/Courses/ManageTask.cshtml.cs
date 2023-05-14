using LMS.DTO;
using LMS.EntityContext;
using LMS.Entity�ontext;
using LMS.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS.Pages
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
    public class LaboratoryWorksModel : PageModel
    {
        private readonly ApplicationContext _db;

        //private readonly CourseRepo _courseRepo;

        private readonly LabWorksRepo _labWorksRepo;

        [BindProperty]
        public LaboratoryWork Cur_laboratoryWork { get; set; }

        public int? CourseID { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; } //ID ������������ ������

        [BindProperty]
        public string CourseName { get; set; }//�������� �����

        [BindProperty]
        public Variant Variant { get; set; }//������� ����� ��� ����������

        public LaboratoryWorksModel(ApplicationContext db, /*CourseRepo courseRepo,*/ LabWorksRepo labWorksRepo)
        {
            _db = db;
            //_courseRepo = courseRepo;
            _labWorksRepo = labWorksRepo;
        }

        public async Task<IActionResult> OnGet()
        {
            CourseID = HttpContext.Session.GetInt32("CourseId");
            if (CourseID == null)
            {
                return BadRequest("������� �� �������� ��� �������� ��� ������ �����");
            }
            if (Id == null)
            {
                return NotFound("������������ ������ �� �������");
            }
            CourseName = await _db.Courses.Where(c => c.CourseId == CourseID).Select(c => c.Name).FirstOrDefaultAsync();
            //TODO ������ � �� � ������ ��� ���������� �������
            Cur_laboratoryWork = await _labWorksRepo.GetById(Id);
            HttpContext.Session.SetInt32("LabWorkId", (int)Id); //��������� ID ������� ��� ����������� �������(���������� ���������,������������ �����������)
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateLab()
        {
            var CourseId = (int)HttpContext.Session.GetInt32("CourseId");
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            return RedirectToPage("Manage", CourseId);
        }

        public async Task<IActionResult> OnPostAddVariant()
        {
            var CourseId = (int)HttpContext.Session.GetInt32("CourseId");
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            return RedirectToPage("Manage", CourseId);
        }

        public async Task<IActionResult> OnPostUpdateVariant()
        {
            var CourseId = (int)HttpContext.Session.GetInt32("CourseId");
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            return RedirectToPage("Manage", CourseId);
        }

        public async Task<IActionResult> OnPostDeleteVariant()
        {
            var CourseId = (int)HttpContext.Session.GetInt32("CourseId");
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            return RedirectToPage("Manage", CourseId);
        }
    }
}