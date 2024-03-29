using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LMS.DTO;
using LMS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
    public class GroupsController : Controller
    {
        private readonly ApplicationContext _db;

        public GroupsController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Task.Run(() => _db.Groups.Select(group => new
            {
                group.GroupId,
                group.Name,
                group.Year, /*group.SpecialtyId*/
            }).ToList()));
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllByDisciplineId([FromHeader] string disciplineCipher)
        //{
        //    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //    var role = User.FindFirst(ClaimTypes.Role).Value;
        //    if (role == RoleEnum.Teacher.ToString())
        //    {
        //        return Ok(await Task.Run(() =>
        //            _db.Plans.Where(d => d.UserId == userId && d.DisciplineCipher == disciplineCipher).Select(d => new { d.Group.Name, d.Group.GroupId }).ToList()));
        //    }
        //    return Ok(await Task.Run(() => _db.Plans.Where(plan => plan.DisciplineCipher == disciplineCipher).Select(plan => plan.Group)));
        //}

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader] int groupId)
        {
            var groups = await _db.Groups
                .Where(group => group.GroupId == groupId)
                .Select(group => new { group.GroupId, group.Name, group.Year/*, group.SpecialtyId*/ })
                .FirstOrDefaultAsync();
            if (groups != null)
            {
                return Ok(groups);
            }
            return NotFound("Group not found");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GroupDTO model)
        {
            var group = new Group { Name = model.Name, Year = model.Year/*, SpecialtyId = model.SpecialityId*/ };
            try
            {
                await _db.Groups.AddAsync(group);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await Response.WriteAsync(e.Message);
            }
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] GroupDTO model, [FromHeader] int groupId)
        {
            var groupUpdate = await _db.Groups.FirstOrDefaultAsync(group => group.GroupId == groupId);

            if (groupUpdate != null)
            {
                try
                {
                    groupUpdate.Name = model.Name;
                    groupUpdate.Year = model.Year;
                    //groupUpdate.SpecialtyId = model.SpecialityId;
                    _db.Groups.Update(groupUpdate);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok(groupId);
            }
            return NotFound("Group not found");
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromHeader] int groupId)
        {
            var group = await _db.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group != null)
            {
                try
                {
                    _db.Groups.Remove(group);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok();
            }
            return NotFound("Group not found");
        }
    }
}