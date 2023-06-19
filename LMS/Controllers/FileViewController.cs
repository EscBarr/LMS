using LMS.Models;
using LMS.Git;
using LMS.Extensions;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Ionic.Zip;
using System.Text;
using System.Web;

namespace LMS.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher, Student")]
    public class FileViewController : GitControllerBase
    {
        public FileViewController(
            IOptions<GitSettings> gitOptions,
            GitService repoService
        )
            : base(gitOptions, repoService)
        { }

        public IActionResult RedirectGitLink(string userName, string repoName)
        {
            return TryGetResult(repoName, () => RedirectToRoutePermanent("GetRepositoryHomeView", new { userName = userName, repoName = repoName }));
        }

        public IActionResult GetTreeView(string userName, string repoName, string id, string path)
        {
            return TryGetResult(repoName, () =>
            {
                repoName = Path.Combine(userName, repoName);
                Repository repo = RepositoryService.GetRepository(repoName);
                Commit commit = repo.Branches[id]?.Tip ?? repo.Lookup<Commit>(id);

                if (commit == null)
                    return View("Init");

                if (path == null)
                {
                    return View("Tree", new TreeModel(repo, "/", repoName, commit.Tree));
                }

                TreeEntry entry = commit[path];
                if (entry == null)
                    return NotFound();

                string parent = Path.GetDirectoryName(entry.Path).Replace(Path.DirectorySeparatorChar, '/');

                switch (entry.TargetType)
                {
                    case TreeEntryTargetType.Tree:
                        return View("Tree", new TreeModel(repo, entry.Path, entry.Name, (Tree)entry.Target, parent));

                    case TreeEntryTargetType.Blob:
                        return Redirect(Url.UnencodedRouteLink("GetBlobView", new { repoName = repoName, id = id, path = path }));

                    default:
                        return BadRequest();
                }
            });
        }

        public IActionResult GetBlobView(string userName, string repoName, string id, string path)
        {
            return TryGetResult(repoName, () =>
            {
                if (path == null)
                    return Redirect(Url.UnencodedRouteLink("GetTreeView", new { repoName = repoName, id = id, path = path }));

                repoName = Path.Combine(userName, repoName);
                Repository repo = RepositoryService.GetRepository(repoName);
                Commit commit = repo.Branches[id]?.Tip ?? repo.Lookup<Commit>(id);

                if (commit == null)
                    return NotFound();

                TreeEntry entry = commit[path];
                if (entry == null)
                    return NotFound();

                switch (entry.TargetType)
                {
                    case TreeEntryTargetType.Blob:
                        return View("Blob", new BlobModel(repo, entry.Path, entry.Name, (Blob)entry.Target));

                    case TreeEntryTargetType.Tree:
                        return Redirect(Url.UnencodedRouteLink("GetTreeView", new { repoName = repoName, id = id, path = path }));

                    default:
                        return BadRequest();
                }
            });
        }

        public IActionResult GetRawBlob(string userName, string repoName, string id, string path)
        {
            return TryGetResult(repoName, () =>
            {
                if (path == null)
                    return Redirect(Url.UnencodedRouteLink("GetTreeView", new { repoName = repoName, id = id, path = path }));

                repoName = Path.Combine(userName, repoName);
                Repository repo = RepositoryService.GetRepository(repoName);
                Commit commit = repo.Branches[id]?.Tip ?? repo.Lookup<Commit>(id);

                if (commit == null)
                    return NotFound();

                TreeEntry entry = commit[path.Replace('/', Path.DirectorySeparatorChar)];
                if (entry == null)
                    return NotFound();

                switch (entry.TargetType)
                {
                    case TreeEntryTargetType.Blob:
                        {
                            Blob blob = (Blob)entry.Target;

                            if (blob.IsBinary)
                                return File(blob.GetContentStream(), "application/octet-stream", entry.Name);
                            else
                                return File(blob.GetContentStream(), "text/plain");
                        }

                    case TreeEntryTargetType.Tree:
                        return Redirect(Url.UnencodedRouteLink("GetTreeView", new { repoName = repoName, id = id, path = path }));

                    default:
                        return BadRequest();
                }
            });
        }

        //TODO Реализовать историю коммитов
        public IActionResult GetCommitsHistory(string userName, string repoName, string id, string path)
        {
            //return TryGetResult(repoName, () =>
            //{
            //    if (path == null)
            //        return Redirect(Url.UnencodedRouteLink("GetTreeView", new { repoName = repoName, id = id, path = path }));

            //    repoName = Path.Combine(userName, repoName);
            //    Repository repo = RepositoryService.GetRepository(repoName);
            //    Commit commit = repo.Branches[id]?.Tip ?? repo.Lookup<Commit>(id);

            //    if (commit == null)
            //        return NotFound();

            //    TreeEntry entry = commit[path];
            //    if (entry == null)
            //        return NotFound();

            //    switch (entry.TargetType)
            //    {
            //        case TreeEntryTargetType.Blob:
            //            return View("Blob", new BlobModel(repo, entry.Path, entry.Name, (Blob)entry.Target));

            //        case TreeEntryTargetType.Tree:
            //            return Redirect(Url.UnencodedRouteLink("GetTreeView", new { repoName = repoName, id = id, path = path }));

            //        default:
            // return BadRequest();

            //    }
            //});
            return BadRequest();
        }

        //TODO Реализовать механизм загрузки архива, а после на его основе тестирование с Drone
        public ActionResult Download(string userName, string repoName, string id, string path)
        {
            var name = id;

            //Response.BufferOutput = false;
            //Response.Charset = "";
            Response.ContentType = "application/zip";

            var repo = RepositoryService.GetRepository(repoName);
            string headerValue = repoName + "-" + id + ".zip";
            Response.Headers.Add("Content-Disposition", headerValue);
            Commit commit = repo.Branches[id]?.Tip ?? repo.Lookup<Commit>(id);
            var Test = new TreeModel(repo, "/", repoName, commit.Tree);
            //var test = RepositoryService.CreatePath(repoName);
            //var Uri = new System.Uri(test).AbsoluteUri;

            //var test2 = Path.Combine(Environment.CurrentDirectory, "Testing", userName);
            //Directory.CreateDirectory(test2);
            //Repository.Clone(Uri, Path.Combine(Environment.CurrentDirectory, "Testing", userName), new CloneOptions
            //{
            //    IsBare = true
            //});

            //using (var outputZip = new ZipFile())
            //{
            //    outputZip.UseZip64WhenSaving = Zip64Option.Always;
            //    outputZip.AlternateEncodingUsage = ZipOption.AsNecessary;
            //    outputZip.AlternateEncoding = Encoding.Unicode;

            //    using (var browser = new RepositoryBrowser(Path.Combine(UserConfiguration.Current.Repositories, repo.Name)))
            //    {
            //        AddTreeToZip(browser, name, path, outputZip);
            //    }

            //    outputZip.Save(Response.Body);

            //    return new EmptyResult();
            //}
            return new EmptyResult();
        }

        //private static void AddTreeToZip(RepositoryBrowser browser, string name, string path, ZipFile outputZip)
        //{
        //    string referenceName;
        //    var treeNode = browser.BrowseTree(name, path, out referenceName);

        //    foreach (var item in treeNode)
        //    {
        //        if (item.IsLink)
        //        {
        //            outputZip.AddDirectoryByName(Path.Combine(item.TreeName, item.Path));
        //        }
        //        else if (!item.IsTree)
        //        {
        //            string blobReferenceName;
        //            var model = browser.BrowseBlob(item.TreeName, item.Path, out blobReferenceName);
        //            outputZip.AddEntry(Path.Combine(item.TreeName, item.Path), model.Data);
        //        }
        //        else
        //        {
        //            // recursive call
        //            AddTreeToZip(browser, item.TreeName, item.Path, outputZip);
        //        }
        //    }
        //}
    }
}