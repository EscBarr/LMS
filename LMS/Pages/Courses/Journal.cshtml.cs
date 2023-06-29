using LMS.EntityContext;
using LMS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LMS.Pages.Courses
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher,Student")]
    public class JournalModel : PageModel
    {
        private readonly ApplicationContext _db;
        private readonly CourseRepo _courseRepo;
        private readonly AssignedVariantsRepo _assignedVariantsRepo;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        //public List<LaboratoryWork> LabAssignedVariants { get; set; }

        public List<AssignedVariant> LabAssignedVariants { get; set; }

        public List<AssignedVariant> LabSendedVariants { get; set; }

        public List<AssignedVariant> LabVerifiedVariants { get; set; }

        public List<SelectListItem> _SelectedUsers { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedUserID { get; set; }

        public JournalModel(ApplicationContext db, CourseRepo courseRepo, AssignedVariantsRepo assignedVariantsRepo)
        {
            _db = db;
            _courseRepo = courseRepo;
            _assignedVariantsRepo = assignedVariantsRepo;
        }

        public string CourseName { get; set; }//Название курса

        public async Task<IActionResult> OnGet(int? StudentId = 0)
        {
            //TODO FIX MEEE
            HttpContext.Session.SetInt32("CourseId", Id);
            if (StudentId == 0)
            {
                LabAssignedVariants = await _assignedVariantsRepo.GetAllWhereByCourse(Id);
            }
            else
            {
                LabAssignedVariants = await _assignedVariantsRepo.GetAllWhereByCourseAndUser(Id, (int)StudentId);
            }
            LabAssignedVariants = LabAssignedVariants.OrderBy(x => x.AssignDateTime).ToList();
            LabSendedVariants = new List<AssignedVariant>();
            LabVerifiedVariants = new List<AssignedVariant>();

            var TempList = new List<AssignedVariant>();
            foreach (var item in LabAssignedVariants)
            {
                if (item.CompletionDateTime != DateTime.MinValue & item.Mark == 0)
                {
                    LabSendedVariants.Add(item);
                    TempList.Add(item);
                    //LabAssignedVariants.Remove(item);
                }
                else if (item.CompletionDateTime != DateTime.MinValue & item.Mark != 0)
                {
                    LabVerifiedVariants.Add(item);
                    TempList.Add(item);
                    //LabAssignedVariants.Remove(item);
                }
            }
            LabAssignedVariants.RemoveAll(item => TempList.Contains(item));
            CourseName = await _db.Courses.Where(c => c.CourseId == Id).Select(c => c.Name).FirstOrDefaultAsync();
            _SelectedUsers = new List<SelectListItem>();
            //Боже простите меня ибо не ведаю я что творю...
            var Temp =  await _assignedVariantsRepo.GetAllWhereByCourse(Id);
            foreach (var item in Temp)
            {
                var SelUser = new SelectListItem { Value = item.User.Id.ToString(), Text = item.User.Name + " " + item.User.Surname + " " + item.User.Patronymic };
                _SelectedUsers.Add(SelUser);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostFilterStudent(int StudentId)
        {
            Id = (int)HttpContext.Session.GetInt32("CourseId");

            return RedirectToPage("Journal", new { Id = Id, StudentId = StudentId });
        }
    }
}