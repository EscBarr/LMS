using System.Linq;
using LMS.EntityСontext;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Pages
{
    public class Groups : PageModel
    {
        //public SelectList Specialities { get; set; }
        private readonly ApplicationContext _db;

        public Groups(ApplicationContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            //Specialities = new SelectList(_db.Specialties.Select(spec => spec.Code));
        }
    }
}