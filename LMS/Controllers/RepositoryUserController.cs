using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LMS.DTO;
using LMS.EntityСontext;
using LMS.Extensions;
using LMS.Git;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LMS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher, Student")]
    public class RepositoryUserController : Controller
    {
        private readonly ApplicationContext _db;
        private IWebHostEnvironment _environment;
        private GitService RepoMager;

        public RepositoryUserController(ApplicationContext db, IWebHostEnvironment env, GitService gitService)
        {
            _db = db;
            _environment = env;
            RepoMager = gitService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]//Получить все репозитории пользователя в список их название описание и дату создания, для вывода на страницу
        public async Task<IActionResult> GetRepos()
        {
            var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return Ok(await Task.Run(() => _db.Repos.Where(repo => repo.UserId == curUserId).Select(repo => new { repo.Id, repo.Name, repo.Description, repo.UpdateTime }).ToList()));
        }

        [HttpPost]//Создать
        public async Task<IActionResult> create([FromBody] RepositoryDTO model)
        {
            var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var NickName = (User.FindFirst(ClaimTypes.Surname).Value);
            //RepoMager.CreateRepository(model.Name, curUserId);
            try
            {
                RepoMager.CreateRepository(model.Name, NickName);
            }
            catch (Exception e)
            {
                await Response.WriteAsync(e.Message);
                return Ok(model);
            }

            var size_repo = RepoMager.GetRepositorySize(model.Name, NickName);
            var repo = new RepositoryEntity { Name = model.Name, Description = model.Description, DefaultBranch = "main", UserName = NickName, CreationDate = System.DateTime.Now, UpdateTime = System.DateTime.Now, UserId = curUserId, Size = size_repo };
            try
            {
                await _db.Repos.AddAsync(repo);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await Response.WriteAsync(e.Message);
            }
            return Ok(model);
        }

        [HttpPost]//Удалить
        public async Task<IActionResult> delete([FromHeader] int Id)
        {
            var Repo = await _db.Repos.FirstOrDefaultAsync(g => g.Id == Id);
            if (Repo != null)
            {
                var AssignedTasks = await _db.AssignedVariants.ToListAsync();
                if (AssignedTasks.FirstOrDefault(A => A.RepoID == Repo.Id) != null)
                {
                    await Response.WriteAsync("Репозиторий прикреплен к назначенному заданию");
                    return Ok();
                }

                var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var NickName = (User.FindFirst(ClaimTypes.Surname).Value);
                try
                {
                    RepoMager.DeleteRepository(Repo.Name, NickName);
                }
                catch (Exception e)
                {
                    //Не делаем ничего если не удалось найти репозиторий на диске, удаляем запись из бд
                }

                try
                {
                    _db.Repos.Remove(Repo);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok();
            }

            return NotFound("Репозиторий не найден");
        }
    }
}