using LMS.DTO;
using LMS.EntityContext;
using LMS.EntityСontext;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher, Student")]
    public class StudentCoursesModel : PageModel
    {
        private readonly ApplicationContext _db;

        private readonly CourseRepo _courseRepo;

        private readonly LabWorksRepo _labWorksRepo;

        private readonly UsersRepo _usersRepo;

        //<button asp-page-handler="delete" asp-route-id="@User.Id" class="btn custom btn-danger remove-item">Удалить</button>
        //<a asp-page="./ControlCourse" asp-route-id="@item.CourseId" >@item.Name</a>
        [Required(ErrorMessage = "Необходимо указать название курса")]
        [Display(Name = "Название курса")]
        [BindProperty]
        public CourseDTO courseDTO { get; set; }//Изменение названия курса

        public List<User>? CurUsers { get; set; }//Загружаем всех пользователей

        [BindProperty]
        public List<Course> Courses { get; set; }//Загружаем все курсы, созданные пользователем

        public StudentCoursesModel(ApplicationContext db, CourseRepo courseRepo, LabWorksRepo labWorksRepo, UsersRepo usersRepo)
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
            Courses = _courseRepo.GetAllWhereUser(UserId);
        }
    }
}