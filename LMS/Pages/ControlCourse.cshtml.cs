using LMS.DTO;
using LMS.EntityContext;
using LMS.EntityСontext;
using LMS.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Security.Claims;

namespace LMS.Pages
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
    public class ControlCourseModel : PageModel
    {
        private readonly ApplicationContext _db;

        private readonly CourseRepo _courseRepo;

        private readonly LabWorksRepo _labWorksRepo;

        private readonly UsersRepo _usersRepo;

        public List<SelectListItem>? _AddedUsers;

        public List<SelectListItem>? _AddedLabs;

        //<button asp-page-handler="delete" asp-route-id="@User.Id" class="btn custom btn-danger remove-item">Удалить</button>

        [BindProperty]
        public CourseDTO courseDTO { get; set; }//Изменение названия курса

        public List<User>? CurUsers { get; set; }//Загружаем всех пользователей

        [BindProperty]
        public List<Course> Courses { get; set; }//Загружаем все курсы, созданные пользователем

        public ControlCourseModel(ApplicationContext db, CourseRepo courseRepo, LabWorksRepo labWorksRepo, UsersRepo usersRepo)
        {
            _db = db;
            _courseRepo = courseRepo;
            _labWorksRepo = labWorksRepo;
            _usersRepo = usersRepo;
            Courses = new List<Course>();
        }

        public async void OnGet()
        {
            ////var UserId = Int32.Parse(HttpContext.Session.GetString("userId"));
            //var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            //Courses = (List<Course>)await _courseRepo.GetAll(UserId);
        }

        public async void OnPostEdit(int CourseId)
        {
            var Course = await _courseRepo.GetById(CourseId);
            Course.Name = courseDTO.Name;
            _courseRepo.Update(Course);
            await _courseRepo.Save();

            //Course
        }

        public async void OnPostAdd()
        {
            var Course = new Course { Name = courseDTO.Name };
            _courseRepo.Create(Course);
            await _courseRepo.Save();
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