using LMS.EntityContext;
using LMS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;

namespace LMS.Pages.Courses
{
    public record UserData
    {
        public User User { get; set; }//Информация о пользователях и назначенных вариантах
        public List<SelectListItem> _Variants { get; set; }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
    public class AssignTasksModel : PageModel
    {
        private readonly ApplicationContext _db;

        private readonly LabWorksRepo _labWorksRepo;
        private readonly CourseRepo _courseRepo;

        private readonly AssignedVariantsRepo _assignedVariantsRepo;

        [BindProperty]
        public LaboratoryWork Cur_laboratoryWork { get; set; }

        [BindProperty]
        public List<UserData> Cur_Users { get; set; }//Информация о пользователях и назначенных вариантах

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; } //ID лабораторной работы

        public int? CourseID { get; set; }

        public List<SelectListItem> _CurrentVariants { get; set; }

        [BindProperty]
        public string CourseName { get; set; }//Название курса

        [BindProperty]
        [Required(ErrorMessage = "Необходимо указать крайний срок")]
        [Display(Name = "Крайний срок сдачи")]
        public DateTime DueDate { get; set; }

        public AssignTasksModel(ApplicationContext db, CourseRepo courseRepo, LabWorksRepo labWorksRepo, AssignedVariantsRepo assignedVariants)
        {
            _db = db;
            _courseRepo = courseRepo;
            _labWorksRepo = labWorksRepo;
            _assignedVariantsRepo = assignedVariants;
        }

        public async Task<IActionResult> OnGet()
        {
            CourseID = HttpContext.Session.GetInt32("CourseId");
            if (CourseID == null)
            {
                return BadRequest("Переход на страницу был совершен не со страницы курса");
            }

            Cur_laboratoryWork = await _labWorksRepo.GetById(Id);
            _CurrentVariants = Cur_laboratoryWork.Variants.Select(a => new SelectListItem { Value = a.VariantId.ToString(), Text = a.VariantNumber.ToString() }).ToList();

            HttpContext.Session.SetInt32("LabWorkId", (int)Id); //Сохраняем ID задания для дальнейшего доступа
            DueDate = DateTime.Now.AddDays(1);
            Cur_Users = new List<UserData>();
            var Test = await _courseRepo.GetAllUsersFromCoursePerLab((int)CourseID, (int)Id);
            foreach (var item in Test)
            {
                UserData userVars = new UserData { User = item, _Variants = new List<SelectListItem>() };

                userVars._Variants = _CurrentVariants.Select(book => new SelectListItem(book.Text, book.Value)).ToList();
                var AsVariant = userVars.User.AssignedVariants.FirstOrDefault(As => As.Variant.LaboratoryWorkId == Id);
                if (AsVariant != null)
                {
                    foreach (var itemVar in userVars._Variants)
                    {
                        if (itemVar.Value == AsVariant.VariantId.ToString())
                        {
                            itemVar.Selected = true;
                            break;
                        }
                    }
                }

                Cur_Users.Add(userVars);
            }

            Cur_Users = Cur_Users.OrderBy(x => x.User.Surname).ToList();
            CourseName = await _db.Courses.Where(c => c.CourseId == CourseID).Select(c => c.Name).FirstOrDefaultAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAssignVar(int userId, int VarId, DateTime Due)
        {
            var LabWorkId = HttpContext.Session.GetInt32("LabWorkId");

            var AssignedRepoid = await _db.Variants.Where(c => c.VariantId == VarId).Select(c => c.AttachedRepoId).FirstOrDefaultAsync();
            AssignedVariant assigned;
            if (AssignedRepoid != null)
            {
                assigned = new AssignedVariant { UserId = userId, VariantId = VarId, AssignDateTime = DateTime.Now, DueDateTime = Due, TeacherAttachedRepoId = (int)AssignedRepoid };
            }
            else
            {
                assigned = new AssignedVariant { UserId = userId, VariantId = VarId, AssignDateTime = DateTime.Now, DueDateTime = Due, TeacherAttachedRepoId = 0 };
            }

            await _assignedVariantsRepo.Create(assigned);
            await _assignedVariantsRepo.Save();
            return RedirectToPage("AssignTasks", LabWorkId);
        }

        public async Task<IActionResult> OnPostUpdateAssignVar(int userId, int VarId, DateTime Due, int CurId)
        {
            var LabWorkId = HttpContext.Session.GetInt32("LabWorkId");
            AssignedVariant assigned = await _assignedVariantsRepo.GetById(CurId);
            assigned.VariantId = VarId;
            assigned.AssignDateTime = DateTime.Now;
            assigned.DueDateTime = Due;
            var AssignedRepoId = await _db.Variants.Where(c => c.VariantId == VarId).Select(c => c.AttachedRepoId).FirstOrDefaultAsync();
            if (AssignedRepoId != null)
            {
                assigned.TeacherAttachedRepoId = (int)AssignedRepoId;
            }
            else
            {
                assigned.TeacherAttachedRepoId = 0;
            }
            await _assignedVariantsRepo.Update(assigned);
            await _assignedVariantsRepo.Save();
            return RedirectToPage("AssignTasks", LabWorkId);
        }

        public async Task<IActionResult> OnPostRandomAssign()
        {
            Id = HttpContext.Session.GetInt32("LabWorkId");
            Cur_laboratoryWork = await _labWorksRepo.GetById(Id);
            List<int> AssignmentsIds = Cur_laboratoryWork.Variants.Select(a => a.VariantId).ToList();
            var Test = await _courseRepo.GetAllUsersFromCoursePerLab((int)CourseID, (int)Id);

            foreach (var user in Test)
            {
                var AsVariant = user.AssignedVariants.FirstOrDefault(As => As.Variant.LaboratoryWorkId == Id);
                if (AsVariant != null)
                {
                }
                else
                {
                }
            }
            return RedirectToPage("AssignTasks", Id);
        }
    }
}