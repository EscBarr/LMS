//using GitServer.Handlers;
using LMS.Git;
using LMS.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO;

namespace LMS.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher, Student")]
    public class GitController : GitControllerBase
    {
        public GitController(
            GitService repoService,
            IOptions<GitSettings> gitOptions
        )
            : base(gitOptions, repoService)
        { }

        //TODO ÏÐÎÂÅÐÈÒÜ ÏÐÀÂÈËÜÍÎÑÒÜ ÑÎÇÄÀÍÈß ÏÓÒÈ

        [Route("{userName}/{repoName}.git/git-upload-pack")]
        public IActionResult ExecuteUploadPack(string userName, string repoName) => TryGetResult(repoName, () => GitUploadPack(Path.Combine(userName, repoName)));

        [Route("{userName}/{repoName}.git/git-receive-pack")]
        public IActionResult ExecuteReceivePack(string userName, string repoName) => TryGetResult(repoName, () => GitReceivePack(Path.Combine(userName, repoName)));

        [Route("{userName}/{repoName}.git/info/refs")]
        public IActionResult GetInfoRefs(string userName, string repoName, string service) => TryGetResult(repoName, () => GitCommand(Path.Combine(userName, repoName), service, true));
    }
}