using LMS.DTO;
using LMS.EntityContext;
using LMS.Entity—ontext;
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
                return NotFound("Õ‡ÁÌ‡˜ÂÌÌÓÂ Á‡‰‡ÌËÂ ÌÂ Ì‡È‰ÂÌÓ");
            }
            if (AssignedVariantDetails.RepoID != 0)
            {
                AssignedRepo = await _db.Repos.FirstOrDefaultAsync(r => r.Id == AssignedVariantDetails.RepoID);
            }
            HttpContext.Session.SetInt32("TaskId", Id);
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
            return RedirectToPage("TaskDetails", Id);
        }

        public async Task<IActionResult> OnPostSendToVerify()
        {
            Id = (int)HttpContext.Session.GetInt32("TaskId");

            return RedirectToPage("TaskDetails", Id);
        }
    }
}