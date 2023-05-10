using LMS.DTO;
using LMS.EntityContext;
using LMS.EntityСontext;
using LMS.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LMS.Pages
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
    public class LaboratoryWorksModel : PageModel
    {
        private readonly ApplicationContext _db;

        private readonly CourseRepo _courseRepo;

        private readonly LabWorksRepo _labWorksRepo;

        private readonly UsersRepo _usersRepo;

        [BindProperty]
        public LaboratoryWorkDTO laboratoryWorkDTO { get; set; }

        [BindProperty]
        public int? CourseID { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; } //ID лабораторной работы в случае изменения

        [BindProperty]
        public string CourseName { get; set; }//Название курса

        public LaboratoryWorksModel(ApplicationContext db, CourseRepo courseRepo, LabWorksRepo labWorksRepo, UsersRepo usersRepo)
        {
            _db = db;
            _courseRepo = courseRepo;
            _labWorksRepo = labWorksRepo;
            _usersRepo = usersRepo;
        }

        public async Task<IActionResult> OnGet()
        {
            CourseID = HttpContext.Session.GetInt32("CourseId");
            if (CourseID == null)
            {
                return NotFound("Переход на страницу был совершен без выбора курса");
            }
            if (Id != null)
            {
                HttpContext.Session.SetInt32("LabWorkId", (int)Id); //Сохраняем ID задания для дальнейшего доступа(добавление вариантов,прикрепление репозитория)
                //TODO ЗАПРОС К БД С ПОЛЯМИ ДЛЯ ЗАПОЛНЕНИЯ ЗАДАНИЯ
            }
            return Page();
        }
    }
}