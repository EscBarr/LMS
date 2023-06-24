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
            if (int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) != AssignedVariantDetails.UserId && User.Claims.First(c => c.Type == ClaimTypes.Role).Value != "Teacher")
            {
                return BadRequest("Задание не было назначено вам");
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

        public async Task<IActionResult> OnPostInitiateRepo()
        {
            Id = (int)HttpContext.Session.GetInt32("TaskId");
            AssignedVariantDetails = await _assignedVariantsRepo.GetById(Id);
            RepositoryEntity entity = await _db.Repos.Where(r => r.Id == AssignedVariantDetails.TeacherAttachedRepoId).FirstOrDefaultAsync();
            if (entity != null)
            {
                var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var NickName = (User.FindFirst(ClaimTypes.Surname).Value);
                GitService.CreateDirectoriesSources(modelRepo.Name, NickName);//Создаем папки репозитория студента
                var StudentDirectory = Path.Combine(Environment.CurrentDirectory, "Repositories", NickName, modelRepo.Name);//Получаем путь для копирования
                var TeacherDirectory = Path.Combine(Environment.CurrentDirectory, "Repositories", entity.UserName, entity.Name);//Получаем откуда копировать
                DirectoryInfo diSource = new DirectoryInfo(TeacherDirectory);
                DirectoryInfo diTarget = new DirectoryInfo(StudentDirectory);
                Copy(diSource, diTarget);
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

                AssignedVariantDetails.RepoID = repo.Id;
                await _assignedVariantsRepo.Update(AssignedVariantDetails);
                await _assignedVariantsRepo.Save();
            }

            return RedirectToPage("TaskDetails", Id);
        }

        private void Copy(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                Copy(diSourceSubDir, nextTargetSubDir);
            }
        }

        public async Task<IActionResult> OnPostDetachRepo()
        {
            Id = (int)HttpContext.Session.GetInt32("TaskId");
            AssignedVariantDetails = await _assignedVariantsRepo.GetById(Id);
            AssignedVariantDetails.RepoID = 0;
            return RedirectToPage("TaskDetails", Id);
        }

        public async Task<IActionResult> OnPostSendComment(string Message)
        {
            Id = (int)HttpContext.Session.GetInt32("TaskId");
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            AssignedVariantDetails = await _assignedVariantsRepo.GetById(Id);
            ChatMessages chat = new ChatMessages();
            chat.AssignedVariantId = Id;
            chat.UserId = UserId;
            chat.Message = Message;
            chat.SendDate = DateTime.Now;
            AssignedVariantDetails.HistoryMessages.Add(chat);
            await _assignedVariantsRepo.Update(AssignedVariantDetails);
            await _assignedVariantsRepo.Save();
            return RedirectToPage("TaskDetails", Id);
        }

        public async Task<IActionResult> OnPostSendToVerify()
        {
            Id = (int)HttpContext.Session.GetInt32("TaskId");
            AssignedVariantDetails = await _assignedVariantsRepo.GetById(Id);
            AssignedVariantDetails.CompletionDateTime = DateTime.Now;
            await _assignedVariantsRepo.Update(AssignedVariantDetails);
            await _assignedVariantsRepo.Save();
            return RedirectToPage("TaskDetails", Id);
        }

        public async Task<IActionResult> OnPostRateWork(int Grade)
        {
            Id = (int)HttpContext.Session.GetInt32("TaskId");
            AssignedVariantDetails = await _assignedVariantsRepo.GetById(Id);
            AssignedVariantDetails.Mark = Grade;
            await _assignedVariantsRepo.Update(AssignedVariantDetails);
            await _assignedVariantsRepo.Save();
            return RedirectToPage("TaskDetails", Id);
        }
    }
}