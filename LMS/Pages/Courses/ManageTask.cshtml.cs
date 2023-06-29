using LMS.EntityContext;
using LMS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS.Pages
{
    public class EditVarModel
    {
        public Variant Variant { get; set; }//Вариант курса для добавления
        public List<SelectListItem> _AllRepos { get; set; }

        public int SelectedRepoId { get; set; }
    }

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

        public List<SelectListItem> _AllRepos { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedRepoId { get; set; }

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
            SetRepoData();
            HttpContext.Session.SetInt32("LabWorkId", (int)Id); //Сохраняем ID задания для дальнейшего доступа(добавление вариантов,прикрепление репозитория)
            return Page();
        }

        private void SetRepoData()
        {
            var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var TeacherRepos = _db.Repos.Where(repo => repo.UserId == curUserId).Select(repo => new { repo.Id, repo.Name }).ToList();
            _AllRepos = new List<SelectListItem>();
            foreach (var item in TeacherRepos)
            {
                var Repos = new SelectListItem { Value = item.Id.ToString(), Text = item.Name };
                _AllRepos.Add(Repos);
            }
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
            Variant.AttachedRepoId = SelectedRepoId;
            await _labWorksRepo.AddVariant(LabWorkId, Variant);
            await _labWorksRepo.Save();
            return RedirectToPage("ManageTask", LabWorkId);
        }

        public async Task<PartialViewResult> OnGetVariantDetails()
        {
            var test = await _db.Variants.FirstOrDefaultAsync(L => L.VariantId == SelectedVarID);
            SetRepoData();
            EditVarModel editVarModel = new EditVarModel();
            editVarModel.Variant = await _db.Variants.FirstOrDefaultAsync(L => L.VariantId == SelectedVarID);
            if (editVarModel.Variant.AttachedRepoId != 0)
            {
                _AllRepos.Find(I => I.Value == editVarModel.Variant.AttachedRepoId.ToString()).Selected = true;
            }
            editVarModel._AllRepos = _AllRepos;

            return Partial("_EditVariant", editVarModel);
        }

        public async Task<IActionResult> OnPostUpdateVariant(int VariantId, string Description, int SelectedRepoId)
        {
            var LabWorkId = (int)HttpContext.Session.GetInt32("LabWorkId");
            var Variant = await _db.Variants.FirstOrDefaultAsync(L => L.VariantId == VariantId);
            Variant.LaboratoryWorkId = LabWorkId;
            if (SelectedRepoId != 0)
            {
                Variant.AttachedRepoId = SelectedRepoId;
            }
            Variant.Description = Description;
            await _labWorksRepo.UpdateVariant(Variant);
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