using LMS.EntityContext;
using LMS.Entity�ontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace LMS.Pages.Courses
{
    public class AssignTasksModel : PageModel
    {
        private readonly ApplicationContext _db;

        private readonly LabWorksRepo _labWorksRepo;
        private readonly CourseRepo _courseRepo;

        private readonly AssignedVariantsRepo _assignedVariantsRepo;

        [BindProperty]
        public LaboratoryWork Cur_laboratoryWork { get; set; }

        [BindProperty]
        public List<User> Cur_Users { get; set; }//���������� � ������������� � ����������� ���������

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; } //ID ������������ ������

        public int? CourseID { get; set; }

        public List<SelectListItem> _CurrentVariants { get; set; }

        [BindProperty]
        public string CourseName { get; set; }//�������� �����

        [BindProperty]
        [Required(ErrorMessage = "���������� ������� ������� ����")]
        [Display(Name = "������� ���� �����")]
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
                return BadRequest("������� �� �������� ��� �������� �� �� �������� �����");
            }

            Cur_laboratoryWork = await _labWorksRepo.GetById(Id);
            _CurrentVariants = Cur_laboratoryWork.Variants.Select(a => new SelectListItem { Value = a.VariantId.ToString(), Text = a.VariantNumber.ToString() }).ToList();
            HttpContext.Session.SetInt32("LabWorkId", (int)Id); //��������� ID ������� ��� ����������� �������
            DueDate = DateTime.Now;
            Cur_Users = await _courseRepo.GetAllUsersFromCoursePerLab((int)CourseID, (int)Id);
            Cur_Users = Cur_Users.OrderBy(x => x.Surname).ToList();
            CourseName = await _db.Courses.Where(c => c.CourseId == CourseID).Select(c => c.Name).FirstOrDefaultAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAssignVar(int userId, int VarId, DateTime Due)
        {
            var LabWorkId = HttpContext.Session.GetInt32("LabWorkId");
            AssignedVariant assigned = new AssignedVariant { UserId = userId, VariantId = VarId, AssignDateTime = DateTime.Now, DueDateTime = Due };
            await _assignedVariantsRepo.Create(assigned);
            await _assignedVariantsRepo.Save();
            return RedirectToPage("AssignTasks", LabWorkId);
        }
    }
}