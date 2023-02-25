using LMS.DTO;
using LMS.EntityContext;
using LMS.Entity�ontext;
using LMS.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;
using System.Xml.Linq;

namespace LMS.Pages
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
    public class CoursesModel : PageModel
    {
        private readonly ApplicationContext _db;

        private readonly CourseRepo _courseRepo;

        private readonly LabWorksRepo _labWorksRepo;

        private readonly UsersRepo _usersRepo;

        public List<SelectListItem>? _AddedUsers;

        public List<SelectListItem>? _AddedLabs;

        //<button asp-page-handler="delete" asp-route-id="@User.Id" class="btn custom btn-danger remove-item">�������</button>
        //<a asp-page="./ControlCourse" asp-route-id="@item.CourseId" >@item.Name</a>
        [Required(ErrorMessage = "���������� ������� �������� �����")]
        [Display(Name = "�������� �����")]
        [BindProperty]
        public CourseDTO courseDTO { get; set; }//��������� �������� �����

        public List<User>? CurUsers { get; set; }//��������� ���� �������������

        [BindProperty]
        public List<Course> Courses { get; set; }//��������� ��� �����, ��������� �������������

        public CoursesModel(ApplicationContext db, CourseRepo courseRepo, LabWorksRepo labWorksRepo, UsersRepo usersRepo)
        {
            _db = db;
            _courseRepo = courseRepo;
            _labWorksRepo = labWorksRepo;
            _usersRepo = usersRepo;
            Courses = new List<Course>();
        }

        public async void OnGet()
        {
            //var UserId = Int32.Parse(HttpContext.Session.GetString("userId"));
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            //var GitNme = User.Claims.First(c => c.Type == ClaimTypes.Surname).Value;
            Courses = _courseRepo.GetAll(UserId);
        }

        public async void OnPostEdit(int CourseId)
        {
            var Course = await _courseRepo.GetById(CourseId);
            Course.Name = courseDTO.Name;
            _courseRepo.Update(Course);
            await _courseRepo.Save();

            //Course
        }

        public async Task<IActionResult> OnPostAdd()
        {
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var Course = new Course { Name = courseDTO.Name, UserId = UserId };
            _courseRepo.Create(Course);
            await _courseRepo.Save();
            return RedirectToPage("Courses");
        }

        public async void OnPostAddUsers()
        {
            var Course = new Course { Name = courseDTO.Name };
            _courseRepo.Create(Course);
            await _courseRepo.Save();
        }

        public async void OnPostAddLabs()
        {
            var Course = new Course { Name = courseDTO.Name };
            _courseRepo.Create(Course);
            await _courseRepo.Save();
        }
    }
}