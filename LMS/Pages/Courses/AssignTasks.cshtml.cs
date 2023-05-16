using LMS.EntityContext;
using LMS.Entity�ontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LMS.Pages.Courses
{
    public class AssignTasksModel : PageModel
    {
        private readonly ApplicationContext _db;

        private readonly LabWorksRepo _labWorksRepo;

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; } //ID ������������ ������

        public int? CourseID { get; set; }

        public AssignTasksModel(ApplicationContext db, /*CourseRepo courseRepo,*/ LabWorksRepo labWorksRepo)
        {
            _db = db;
            //_courseRepo = courseRepo;
            _labWorksRepo = labWorksRepo;
        }

        public void OnGet()
        {
        }
    }
}