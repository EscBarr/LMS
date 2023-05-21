using LMS.EntityContext;
using LMS.Entity�ontext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace LMS.Pages.Courses
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Student")]
    public class AssignedTasksModel : PageModel
    {
        private readonly ApplicationContext _db;
        private readonly AssignedVariantsRepo _assignedVariantsRepo;

        [BindProperty]
        public List<AssignedVariant> AssignedVariants { get; set; }

        [BindProperty]
        public List<AssignedVariant> DueAssignedVariants { get; set; }//������������

        [BindProperty]
        public List<AssignedVariant> CompletedAssignedVariants { get; set; }//����������� �������

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public AssignedTasksModel(ApplicationContext db, /*CourseRepo courseRepo,*/ AssignedVariantsRepo assignedVariantsRepo)
        {
            _db = db;
            //_courseRepo = courseRepo;
            _assignedVariantsRepo = assignedVariantsRepo;
        }

        public async Task<IActionResult> OnGet()
        {
            HttpContext.Session.SetInt32("CourseId", Id);
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            //TODO ����� ��������� ���� ������� ������� �� �� 3 ���������
            AssignedVariants = await _assignedVariantsRepo.GetAllWhereUser(UserId, Id);

            return Page();
        }
    }
}