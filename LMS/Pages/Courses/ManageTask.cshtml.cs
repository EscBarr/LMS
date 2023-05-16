using LMS.EntityContext;
using LMS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS.Pages
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
    public class LaboratoryWorksModel : PageModel
    {
        private readonly ApplicationContext _db;

        //private readonly CourseRepo _courseRepo;

        private readonly LabWorksRepo _labWorksRepo;

        [BindProperty]
        public LaboratoryWork Cur_laboratoryWork { get; set; }

        public int? CourseID { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; } //ID лабораторной работы

        [BindProperty(SupportsGet = true)]
        public int SelectedVarID { get; set; }

        [BindProperty]
        public string CourseName { get; set; }//Название курса

        [BindProperty]
        public Variant Variant { get; set; }//Вариант курса для добавления

        public LaboratoryWorksModel(ApplicationContext db, /*CourseRepo courseRepo,*/ LabWorksRepo labWorksRepo)
        {
            _db = db;
            //_courseRepo = courseRepo;
            _labWorksRepo = labWorksRepo;
        }

        public async Task<IActionResult> OnGet()
        {
            CourseID = HttpContext.Session.GetInt32("CourseId");
            if (CourseID == null)
            {
                return BadRequest("Переход на страницу был совершен без выбора курса");
            }
            if (Id == null)
            {
                return NotFound("Лабораторная работа не найдена");
            }
            CourseName = await _db.Courses.Where(c => c.CourseId == CourseID).Select(c => c.Name).FirstOrDefaultAsync();
            //TODO ЗАПРОС К БД С ПОЛЯМИ ДЛЯ ЗАПОЛНЕНИЯ ЗАДАНИЯ
            Cur_laboratoryWork = await _labWorksRepo.GetById(Id);
            HttpContext.Session.SetInt32("LabWorkId", (int)Id); //Сохраняем ID задания для дальнейшего доступа(добавление вариантов,прикрепление репозитория)
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateLab()
        {
            var LabWorkId = (int)HttpContext.Session.GetInt32("LabWorkId");
            CourseID = HttpContext.Session.GetInt32("CourseId");
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            //var LabUpdate = await _labWorksRepo.GetById(LabWorkId);
            Cur_laboratoryWork.Id = LabWorkId;
            Cur_laboratoryWork.CourseId = (int)CourseID;
            Cur_laboratoryWork.UserId = UserId;
            //LabUpdate.Name = Cur_laboratoryWork.Name;
            //LabUpdate.Description = Cur_laboratoryWork.Description;
            //LabUpdate.MaxMark = Cur_laboratoryWork.MaxMark;
            _labWorksRepo.Update(Cur_laboratoryWork);
            await _labWorksRepo.Save();
            return RedirectToPage("ManageTask", LabWorkId);
        }

        public async Task<IActionResult> OnPostAddVariant()
        {
            var LabWorkId = (int)HttpContext.Session.GetInt32("LabWorkId");
            Variant.LaboratoryWorkId = LabWorkId;
            await _labWorksRepo.AddVariant(LabWorkId, Variant);
            await _labWorksRepo.Save();
            return RedirectToPage("ManageTask", LabWorkId);
        }

        public async Task<PartialViewResult> OnGetVariantDetails()
        {
            var test = await _db.Variants.FirstOrDefaultAsync(L => L.VariantId == SelectedVarID);
            return Partial("_EditVariant", await _db.Variants.FirstOrDefaultAsync(L => L.VariantId == SelectedVarID));
        }

        public async Task<IActionResult> OnPostUpdateVariant(Variant variant)
        {
            var LabWorkId = (int)HttpContext.Session.GetInt32("LabWorkId");
            variant.LaboratoryWorkId = LabWorkId;
            await _labWorksRepo.UpdateVariant(variant);
            await _labWorksRepo.Save();
            return RedirectToPage("ManageTask", LabWorkId);
        }

        public async Task<IActionResult> OnPostDeleteVariant(int VarID)
        {
            var LabWorkId = (int)HttpContext.Session.GetInt32("LabWorkId");
            await _labWorksRepo.DeleteVariant(LabWorkId, VarID);
            await _labWorksRepo.Save();
            return RedirectToPage("ManageTask", LabWorkId);
        }
    }
}