using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LMS.DTO;
using LMS.EntityСontext;
using LMS.Git;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LMS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher, Student")]
    public class RepositoryTeacherController : Controller
    {
        private readonly ApplicationContext _db;

        public RepositoryTeacherController(ApplicationContext db)
        {
            _db = db;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllUsersByGroup([FromHeader] int groupId)
        //    => Ok(await Task.Run(() => _db.Users.Where(user => user.GroupId == groupId).Select(user => new { user.Id, user.Name, user.Surname }).ToList()));

        [HttpGet]//Получить все репозитории пользователя в список их название описание и дату создания, для вывода на страницу ПРИДУМАТЬ КАК ПРЕВРАТИТЬ НАЗВАНИЕ В ГИПЕРССЫЛКУ
        public async Task<IActionResult> GetRepos([FromHeader] int curUserId)
        {
            //var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return Ok(await Task.Run(() => _db.Repos.Where(repo => repo.UserId == curUserId).Select(repo => new { repo.Id, repo.Name, repo.Description, repo.UpdateTime }).ToList()));
        }
    }
}