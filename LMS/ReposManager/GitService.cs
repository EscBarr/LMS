﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime;
using LibGit2Sharp;
using LMS.Extensions;
using Microsoft.Extensions.Options;

namespace LMS.Git
{
    public class GitService
    {
        private static List<string>? _repos = null;

        private IOptions<GitSettings> _settings;
        protected GitSettings Settings => _settings.Value;

        public GitService(IOptions<GitSettings> gitOptions)
        {
            _settings = gitOptions;
        }

        public string CreatePath(string name)
        {
            return Path.Combine(Settings.BasePath, name);
        }

        public Repository GetRepository(string name)
            => new Repository(Path.Combine(Settings.BasePath, name));

        public IEnumerable<DirectoryInfo> RepositoryDirectories(int userId)//Получение списка всех репозиториев созданных пользователем
        {
            if (_repos == null)
            {
                _repos = new List<string>();
                DirectoryInfo basePath = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Repositories", userId.ToString()));
                foreach (DirectoryInfo path in basePath.EnumerateDirectories())
                {
                    string repPath = Repository.Discover(path.FullName);
                    if (repPath != null)
                        _repos.Add(repPath);
                }
            }

            return _repos.Select(d => new DirectoryInfo(d));
        }

        //private static void CreateDirectoriesSources(string RepoName, int userId)
        //{
        //    var userDirectory = Path.Combine(Environment.CurrentDirectory, "Repositories", userId.ToString());
        //    if (!Directory.Exists(userDirectory))
        //    {
        //        Directory.CreateDirectory(userDirectory);
        //    }
        //    //Получим директорию репозитория пользователя
        //    var taskDirectory = Path.Combine(userDirectory, RepoName);
        //    if (!Directory.Exists(taskDirectory))
        //    {
        //        Directory.CreateDirectory(taskDirectory);
        //    }
        //    else
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(RepoName), RepoName, "Репозиторий с таким именем уже создан");
        //    }
        //}

        public static void CreateDirectoriesSources(string RepoName, string userName)
        {
            var userDirectory = Path.Combine(Environment.CurrentDirectory, "Repositories", userName);
            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }
            //Получим директорию репозитория пользователя
            var taskDirectory = Path.Combine(userDirectory, RepoName);
            if (!Directory.Exists(taskDirectory))
            {
                Directory.CreateDirectory(taskDirectory);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(RepoName), RepoName, "Репозиторий с таким именем уже создан");
            }
        }

        //public Repository GetRepository(string RepoName, int userId)
        //{
        //    var userDirectory = Path.Combine(Environment.CurrentDirectory, "Repositories", userId.ToString());
        //    var taskDirectory = Path.Combine(userDirectory, RepoName);
        //    return new Repository(taskDirectory);
        //}

        //public Repository GetRepository(string RepoName, int userId)
        //{
        //    var userDirectory = Path.Combine(Environment.CurrentDirectory, "Repositories", userId.ToString());
        //    var taskDirectory = Path.Combine(userDirectory, RepoName);
        //    return new Repository(taskDirectory);
        //}

        public (double, string) AdjustFileSize(long fileSizeInBytes)
        {
            string[] names = { "BYTES", "KB", "MB", "GB" };

            double sizeResult = fileSizeInBytes * 1.0;
            int nameIndex = 0;
            while (sizeResult > 1024 && nameIndex < names.Length)
            {
                sizeResult /= 1024;
                nameIndex++;
            }

            return (sizeResult, names[nameIndex]);
        }

        public long GetCatalogsSize(DirectoryInfo Directory)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = Directory.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = Directory.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += GetCatalogsSize(di);
            }
            return size;
        }

        //public long GetRepositorySize(string RepoName, int userId)
        //{
        //    var userDirectory = Path.Combine(Environment.CurrentDirectory, "Repositories", userId.ToString());
        //    var taskDirectory = Path.Combine(userDirectory, RepoName);
        //    DirectoryInfo basePath = new DirectoryInfo(taskDirectory);
        //    return GetCatalogsSize(basePath);
        //}

        public long GetRepositorySize(string RepoName, string userName)
        {
            var userDirectory = Path.Combine(Environment.CurrentDirectory, "Repositories", userName);
            var taskDirectory = Path.Combine(userDirectory, RepoName);
            DirectoryInfo basePath = new DirectoryInfo(taskDirectory);
            return GetCatalogsSize(basePath);
        }

        //public Repository CreateRepository(string RepoName, int userId)
        //{
        //    CreateDirectoriesSources(RepoName, userId);
        //    var userDirectory = Path.Combine(Environment.CurrentDirectory, "Repositories", userId.ToString());
        //    var taskDirectory = Path.Combine(userDirectory, RepoName);
        //    Repository repo = new Repository(Repository.Init(taskDirectory, true));
        //    //_repos.Add(path);
        //    return repo;
        //}

        public Repository CreateRepository(string RepoName, string userName)
        {
            CreateDirectoriesSources(RepoName, userName);
            var userDirectory = Path.Combine(Environment.CurrentDirectory, "Repositories", userName);
            var taskDirectory = Path.Combine(userDirectory, RepoName);
            Repository repo = new Repository(Repository.Init(taskDirectory, true));
            //_repos.Add(path);
            return repo;
        }

        public Repository CreateRepository(string RepoName, string userName, string remoteUrl)
        {
            CreateDirectoriesSources(RepoName, userName);
            var userDirectory = Path.Combine(Environment.CurrentDirectory, "Repositories", userName);
            var taskDirectory = Path.Combine(userDirectory, RepoName);
            try
            {
                using (var repo = new Repository(Repository.Init(taskDirectory, true)))
                {
                    repo.Config.Set("core.logallrefupdates", true);
                    repo.Network.Remotes.Add("origin", remoteUrl, "+refs/*:refs/*");
                    var logMessage = "";
                    foreach (var remote in repo.Network.Remotes)
                    {
                        IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                        Commands.Fetch(repo, remote.Name, refSpecs, null, logMessage);
                    }
                    //_repos.Add(path);
                    return repo;
                }
            }
            catch
            {
                try
                {
                    Directory.Delete(taskDirectory, true);
                }
                catch { }
                return null;
            }
        }

        //public Repository CreateRepository(string RepoName, int userId, string remoteUrl)
        //{
        //    CreateDirectoriesSources(RepoName, userId);
        //    var userDirectory = Path.Combine(Environment.CurrentDirectory, "Repositories", userId.ToString());
        //    var taskDirectory = Path.Combine(userDirectory, RepoName);
        //    try
        //    {
        //        using (var repo = new Repository(Repository.Init(taskDirectory, true)))
        //        {
        //            repo.Config.Set("core.logallrefupdates", true);
        //            repo.Network.Remotes.Add("origin", remoteUrl, "+refs/*:refs/*");
        //            var logMessage = "";
        //            foreach (var remote in repo.Network.Remotes)
        //            {
        //                IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
        //                Commands.Fetch(repo, remote.Name, refSpecs, null, logMessage);
        //            }
        //            //_repos.Add(path);
        //            return repo;
        //        }
        //    }
        //    catch
        //    {
        //        try
        //        {
        //            Directory.Delete(taskDirectory, true);
        //        }
        //        catch { }
        //        return null;
        //    }
        //}

        //public void DeleteRepository(string RepoName, int userId)
        //{
        //    Exception e = null;
        //    try
        //    {
        //        var userDirectory = Path.Combine(Environment.CurrentDirectory, "Repositories", userId.ToString());
        //        var taskDirectory = Path.Combine(userDirectory, RepoName);
        //        Directory.Delete(taskDirectory, true);

        //        //_repos.Remove(path);
        //    }
        //    catch (Exception ex) { e = ex; }

        //    if (e != null)
        //        throw new Exception("Ну удалось удалить репозиторий", e);
        //}

        public void DeleteRepository(string RepoName, string userName)
        {
            Exception e = null;
            try
            {
                var userDirectory = Path.Combine(Environment.CurrentDirectory, "Repositories", userName);
                var taskDirectory = Path.Combine(userDirectory, RepoName);
                var directory = new DirectoryInfo(taskDirectory) { Attributes = FileAttributes.Normal };

                foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    info.Attributes = FileAttributes.Normal;
                }

                directory.Delete(true);

                //_repos.Remove(path);
            }
            catch (Exception ex) { e = ex; }

            if (e != null)
                throw new Exception("Не удалось удалить репозиторий", e);
        }

        public ReferenceCollection GetReferences(string repoName, int userId) => GetRepository(repoName).Refs;
    }
}