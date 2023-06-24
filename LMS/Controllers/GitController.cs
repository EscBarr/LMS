using LMS.Git;
using LMS.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using LMS.Services.BasicAuth;
using System.Security.Claims;
using LMS.Entity—ontext;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    [Authorize(AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme)]
    public class GitController : GitControllerBase
    {
        private readonly ApplicationContext _db;

        public GitController(
            GitService repoService,
            IOptions<GitSettings> gitOptions, ApplicationContext db
        )
            : base(gitOptions, repoService)
        {
            _db = db;
        }

        private bool HasPermission(int UserId, string repoName)
        {
            var dbUser = _db.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).Include(U => U.Repositories).FirstOrDefault(u => u.Id == UserId);
            if (dbUser != null)
            {
                //TODO ¬ÓÁÏÓÊÌÓ ÒÚÓËÚ ËÁÏÂÌËÚ¸ ‚ ·Û‰Û˘ÂÏ
                var Role = dbUser.UserRoles.Select(role => role.Role.RoleName.ToString()).First();
                if (Role == "Teacher" || Role == "Admin")
                {
                    return true;
                }
                else
                {
                    if (dbUser.Repositories.FirstOrDefault(r => r.Name == repoName) != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //TODO œ–Œ¬≈–»“‹ œ–¿¬»À‹ÕŒ—“‹ —Œ«ƒ¿Õ»ﬂ œ”“»

        [Route("{userName}/{repoName}.git/git-upload-pack")]
        public IActionResult ExecuteUploadPack(string userName, string repoName)
        {
            var Id = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (HasPermission(Id, repoName))
            {
                return TryGetResult(repoName, () => GitUploadPack(Path.Combine(userName, repoName)));
            }
            return Unauthorized();
        }

        [Route("{userName}/{repoName}.git/git-receive-pack")]
        public IActionResult ExecuteReceivePack(string userName, string repoName)
        {
            var Id = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (HasPermission(Id, repoName))
            {
                return TryGetResult(repoName, () => GitUploadPack(Path.Combine(userName, repoName)));
            }
            return Unauthorized();
        }

        [Route("{userName}/{repoName}.git/info/refs")]
        public IActionResult GetInfoRefs(string userName, string repoName, string service)
        {
            var Id = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (HasPermission(Id, repoName))
            {
                return TryGetResult(repoName, () => GitUploadPack(Path.Combine(userName, repoName)));
            }
            return Unauthorized();
        }
    }
}