using LMS.EntityContext;
using LMS.Entity—ontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LMS.Pages.Courses
{
    public class JournalModel : PageModel
    {
        private readonly ApplicationContext _db;
        private readonly CourseRepo _courseRepo;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public List<LaboratoryWork> LabAssignedVariants { get; set; }

        public JournalModel(ApplicationContext db, CourseRepo courseRepo)
        {
            _db = db;
            _courseRepo = courseRepo;
        }

        public string CourseName { get; set; }//Õ‡Á‚‡ÌËÂ ÍÛÒ‡

        public async Task<IActionResult> OnGet()
        {
            LabAssignedVariants = await _courseRepo.GetAllLabsFromCourseAndAssignedVariants(Id);
            LabAssignedVariants = LabAssignedVariants.OrderBy(x => x.Id).ToList();

            CourseName = await _db.Courses.Where(c => c.CourseId == Id).Select(c => c.Name).FirstOrDefaultAsync();
            return Page();
        }
    }
}