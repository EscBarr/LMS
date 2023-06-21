using LMS.DTO;
using LMS.EntityContext;
using LMS.EntityСontext;
using LMS.Git;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace LMS.Pages.Courses
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher,Student")]
    public class TaskDetailsModel : PageModel
    {
        private readonly ApplicationContext _db;
        private readonly AssignedVariantsRepo _assignedVariantsRepo;
        private GitService RepoMager;

        [BindProperty]
        public AssignedVariant AssignedVariantDetails { get; set; }

        [BindProperty]
        public RepositoryEntity AssignedRepo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public RepositoryDTO modelRepo { get; set; }

        public string TeacherName { get; set; }

        public TaskDetailsModel(ApplicationContext db, AssignedVariantsRepo assignedVariantsRepo, GitService gitService)
        {
            _db = db;
            _assignedVariantsRepo = assignedVariantsRepo;
            RepoMager = gitService;
        }

        public async Task<IActionResult> OnGet()
        {
            AssignedVariantDetails = await _assignedVariantsRepo.GetById(Id);
            if (AssignedVariantDetails == null)
            {
                return NotFound("Назначенное задание не найдено");
            }
            if (AssignedVariantDetails.RepoID != 0)
            {
                AssignedRepo = await _db.Repos.FirstOrDefaultAsync(r => r.Id == AssignedVariantDetails.RepoID);
            }
            var CourseID = HttpContext.Session.GetInt32("CourseId");
            if (CourseID == null)
            {
                return BadRequest("Переход на страницу был совершен не со страницы курса");
            }

            TeacherName = await _db.Courses.Where(c => c.CourseId == Id).Select(c => c.User.Name + " " + c.User.Surname).FirstOrDefaultAsync();
            HttpContext.Session.SetInt32("TaskId", Id);
            AssignedVariantDetails.HistoryMessages = AssignedVariantDetails.HistoryMessages.OrderBy(m => m.SendDate).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateRepo()
        {
            Id = (int)HttpContext.Session.GetInt32("TaskId");
            var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var NickName = (User.FindFirst(ClaimTypes.Surname).Value);
            RepoMager.CreateRepository(modelRepo.Name, NickName);
            var size_repo = RepoMager.GetRepositorySize(modelRepo.Name, NickName);
            var repo = new RepositoryEntity { Name = modelRepo.Name, Description = modelRepo.Description, DefaultBranch = "main", UserName = NickName, CreationDate = System.DateTime.Now, UpdateTime = System.DateTime.Now, UserId = curUserId, Size = size_repo };
            try
            {
                await _db.Repos.AddAsync(repo);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await Response.WriteAsync(e.Message);
            }
            AssignedVariantDetails = await _assignedVariantsRepo.GetById(Id);
            AssignedVariantDetails.RepoID = repo.Id;
            await _assignedVariantsRepo.Update(AssignedVariantDetails);
            await _assignedVariantsRepo.Save();
            return RedirectToPage("TaskDetails", Id);
        }

        public async Task<IActionResult> OnPostDetachRepo()
        {
            Id = (int)HttpContext.Session.GetInt32("TaskId");
            AssignedVariantDetails = await _assignedVariantsRepo.GetById(Id);
            AssignedVariantDetails.RepoID = 0;
            return RedirectToPage("TaskDetails", Id);
        }

        public async Task<IActionResult> OnPostSendComment()
        {
            Id = (int)HttpContext.Session.GetInt32("TaskId");
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            return RedirectToPage("TaskDetails", Id);
        }

        public async Task<IActionResult> OnPostSendToVerify()
        {
            Id = (int)HttpContext.Session.GetInt32("TaskId");

            return RedirectToPage("TaskDetails", Id);
        }
    }
}