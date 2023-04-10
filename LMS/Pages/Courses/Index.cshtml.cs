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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher,Student")]
    public class CoursesModel : PageModel
    {
        private readonly ApplicationContext _db;

        private readonly CourseRepo _courseRepo;

        [Required(ErrorMessage = "Необходимо указать название курса")]
        [Display(Name = "Название курса")]
        [BindProperty]
        public CourseDTO courseDTO { get; set; }//Изменение названия курса

        public List<User>? CurUsers { get; set; }//Загружаем всех пользователей

        [BindProperty]
        public List<Course> Courses { get; set; }//Загружаем все курсы, созданные пользователем

        public CoursesModel(ApplicationContext db, CourseRepo courseRepo, LabWorksRepo labWorksRepo, UsersRepo usersRepo)
        {
            _db = db;
            _courseRepo = courseRepo;

            Courses = new List<Course>();
        }

        public async Task OnGet()
        {
            //var UserId = Int32.Parse(HttpContext.Session.GetString("userId"));
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            //TODO Пример записи гит имени
            //var GitNme = User.Claims.First(c => c.Type == ClaimTypes.Surname).Value;
            var Role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            if (Role == "Teacher")
            {
                Courses = await _courseRepo.GetAll(UserId);
                //return Partial("_CoursesTeacher", Courses);
            }
            else if (Role == "Student")
            {
                Courses = await _courseRepo.GetAllWhereUser(UserId);
                //return Partial("_CoursesStudent", Courses);
            }
        }

        public async Task<IActionResult> OnPostAdd(string name)
        {
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var Course = new Course { Name = name, UserId = UserId };
            _courseRepo.Create(Course);
            await _courseRepo.Save();
            return RedirectToPage("Courses");
        }
    }
}