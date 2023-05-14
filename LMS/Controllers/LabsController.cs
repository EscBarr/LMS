using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LMS.DTO;
using LMS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LMS.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher, Student")]
    public class LabsController : Controller
    {
        private readonly ApplicationContext _db;

        private IWebHostEnvironment _environment;

        public LabsController(ApplicationContext db, IWebHostEnvironment env)
        {
            _db = db;
            _environment = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllByLaboratoryId([FromHeader] int laboratoryWorkId)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (await _db.LaboratoryWorks.Where(w => w.UserId == userId && w.Id == laboratoryWorkId).FirstOrDefaultAsync() != null)
            {
                return Ok(await Task.Run(() => _db.Variants
                    .Include(variant => variant.LaboratoryWork)
                    .Where(v => v.LaboratoryWork.Id == laboratoryWorkId)
                    .Select(v => new { v.VariantId, v.LaboratoryWorkId, v.LaboratoryWork.Name, v.VariantNumber, v.Description })
                    .OrderBy(x => x.VariantNumber)
                    .ToList())
                );
            }

            return BadRequest("Нет прав для просмотра");
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader] int laboratoryWorkId)
        {
            var laboratoryWorks = await _db.LaboratoryWorks
                .Where(laboratoryWork => laboratoryWork.Id == laboratoryWorkId)
                .Select(laboratoryWork => new { laboratoryWork.Name, laboratoryWork.Description, laboratoryWork.UserId })
                .FirstOrDefaultAsync();
            if (laboratoryWorks != null)
            {
                return Ok(laboratoryWorks);
            }
            return NotFound();
        }

        // проверка аута
        [HttpGet]
        public IActionResult CheckAuth()
        {
            return Ok(new string("Auth is done!"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return Ok(await Task.Run(() => _db.LaboratoryWorks.Where(lw => lw.UserId == curUserId)
                .Select(laboratoryWork => new { laboratoryWork.Id, laboratoryWork.Name, laboratoryWork.Description, laboratoryWork.CourseId, laboratoryWork.UserId }).ToList()));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllByCourse([FromHeader] int CourseId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role).Value;
            if (role == RoleEnum.Teacher.ToString())
            {
                return Ok(await Task.Run(() =>
                    _db.LaboratoryWorks.Where(lw => lw.UserId == userId && lw.Course.CourseId == CourseId)
                        .ToList()));
            }
            return Ok(await Task.Run(() =>
                _db.LaboratoryWorks.Where(lw => lw.Course.CourseId == CourseId)
                    .ToList()));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LaboratoryWorkDTO model)
        {
            var laboratoryWork =
                new LaboratoryWork { Name = model.Name, Description = model.Description, CourseId = model.CourseId, UserId = model.UserId };

            try
            {
                await _db.LaboratoryWorks.AddAsync(laboratoryWork);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await Response.WriteAsync(e.InnerException.Message);
                return BadRequest(e.InnerException.Message);
            }
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] LaboratoryWorkDTO model, [FromHeader] int laboratoryWorkId)
        {
            var laboratoryWorkUpdate = await _db.LaboratoryWorks.FirstOrDefaultAsync(laboratoryWork => laboratoryWork.Id == laboratoryWorkId);
            var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (laboratoryWorkUpdate != null)
            {
                try
                {
                    if (curUserId != laboratoryWorkUpdate.UserId)
                    {
                        throw new Exception("No access to edit this lab");
                    }

                    laboratoryWorkUpdate.Name = model.Name;
                    laboratoryWorkUpdate.CourseId = model.CourseId;
                    laboratoryWorkUpdate.Description = model.Description;
                    laboratoryWorkUpdate.UserId = model.UserId;
                    _db.LaboratoryWorks.Update(laboratoryWorkUpdate);
                    await _db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok("successfully");
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromHeader] int laboratoryWorkId)
        {
            var laboratoryWorkDelete = await _db.LaboratoryWorks.FirstOrDefaultAsync(laboratoryWork => laboratoryWork.Id == laboratoryWorkId);
            var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (laboratoryWorkDelete != null)
            {
                try
                {
                    if (curUserId != laboratoryWorkDelete.UserId)
                    {
                        throw new Exception("No access to delete this lab");
                    }
                    _db.LaboratoryWorks.Remove(laboratoryWorkDelete);
                    await _db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok("successfully");
            }
            return NotFound();
        }
    }
}