using System;
using System.IO;
using LMS.Models;

//using GitServer.Services;
using LMS.Git;
using LMS.Extensions;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LMS.Controllers
{
    public class GitControllerBase : Controller
    {
        private IOptions<GitSettings> _gitOptions;
        private GitService _repoService;

        protected GitSettings GitSettings => _gitOptions.Value;
        protected GitService RepositoryService => _repoService;

        protected GitControllerBase(
            IOptions<GitSettings> gitOptions,
            GitService repoService
        )
        {
            _gitOptions = gitOptions;
            _repoService = repoService;
        }

        protected GitCommandResult GitCommand(string repoName, string service, bool advertiseRefs, bool endStreamWithNull = true)
        {
            return new GitCommandResult(_gitOptions.Value.GitPath, new GitCommandOptions(
                RepositoryService.GetRepository(repoName, 1),
                service,
                advertiseRefs,
                endStreamWithNull
            ));
        }

        protected GitCommandResult GitUploadPack(string repoName) => GitCommand(repoName, "git-upload-pack", false, false);

        protected GitCommandResult GitReceivePack(string repoName) => GitCommand(repoName, "git-receive-pack", false);

        protected IActionResult TryGetResult(string repoName, Func<IActionResult> resFunc)
        {
            try
            {
                return resFunc();
            }
            catch (RepositoryNotFoundException)
            {
                return MakeError("Репозиторий не найден", repoName, 404);
            }
            catch (NotFoundException)
            {
                return MakeError("Запрашиваемый файл не найден", repoName, 404);
            }
            catch (Exception e)
            {
                return MakeError(e, repoName);
            }
        }

        private IActionResult MakeError(string message, string repoName, int statusCode = 500)
        {
            ErrorModel model = new ErrorModel
            {
                StatusCode = statusCode,
                Message = message,
                Description = $" Произошла ошибка при совершении операции над репозиторием \"{repoName}\": {message}"
            };

            ViewResult viewResult = View("_Error", model);
            viewResult.StatusCode = statusCode;
            return viewResult;
        }

        private IActionResult MakeError(Exception error, string repoName, int statusCode = 500)
        {
            return MakeError(error.Message, repoName, statusCode);
        }
    }
}