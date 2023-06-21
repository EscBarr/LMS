using LibGit2Sharp;
using System.ComponentModel.DataAnnotations;
using System.Resources;
using System.Security.AccessControl;
using System.Text;
using System.Xml.Linq;

namespace LMS.Extensions
{
    public class RepositoryTreeDetailModel
    {
        public string Name { get; set; }

        public string CommitMessage { get; set; }

        public DateTime? CommitDate { get; set; }

        public string CommitDateString
        { get { return CommitDate.HasValue ? CommitDate.Value.ToString() : CommitDate.ToString(); } }

        public string Author { get; set; }
        public bool IsTree { get; set; }
        public bool IsLink { get; set; }
        public string TreeName { get; set; }
        public bool IsImage { get; set; }
        public bool IsText { get; set; }
        public bool IsMarkdown { get; set; }
        public string Path { get; set; }
        public byte[] Data { get; set; }
        public string Text { get; set; }
        public string TextBrush { get; set; }
        public Encoding Encoding { get; set; }
    }

    public class RepositoryBrowser : IDisposable
    {
        private readonly Repository _repository;

        public RepositoryBrowser(string repositoryPath)
        {
            if (!Repository.IsValid(repositoryPath))
            {
                throw new ArgumentException("Repository is not valid.", "repositoryPath");
            }

            _repository = new Repository(repositoryPath);
        }

        public IEnumerable<RepositoryTreeDetailModel> BrowseTree(string name, string path, out string referenceName, bool includeDetails = false)
        {
            var commit = GetCommitByName(name, out referenceName);
            if (commit == null)
            {
                return Enumerable.Empty<RepositoryTreeDetailModel>();
            }

            string branchName = referenceName ?? name;

            Tree tree;
            if (String.IsNullOrEmpty(path))
            {
                tree = commit.Tree;
            }
            else
            {
                var treeEntry = commit[path];
                if (treeEntry.TargetType == TreeEntryTargetType.Blob)
                {
                    return new[] { CreateRepositoryDetailModel(treeEntry, null, referenceName) };
                }

                if (treeEntry.TargetType == TreeEntryTargetType.GitLink)
                {
                    return new RepositoryTreeDetailModel[0];
                }

                tree = (Tree)treeEntry.Target;
            }

            return includeDetails ? GetTreeModelsWithDetails(commit, tree, branchName) : GetTreeModels(tree, branchName);
        }

        public RepositoryTreeDetailModel BrowseBlob(string name, string path, out string referenceName)
        {
            if (path == null)
            {
                path = String.Empty;
            }

            var commit = GetCommitByName(name, out referenceName);
            if (commit == null)
            {
                return null;
            }

            var entry = commit[path];
            if (entry == null)
            {
                return null;
            }

            var model = new RepositoryTreeDetailModel
            {
                Name = entry.Name,
                IsTree = false,
                IsLink = false,
                CommitDate = commit.Author.When.LocalDateTime,
                CommitMessage = commit.Message,
                Author = commit.Author.Name,
                TreeName = referenceName ?? name,
                Path = path,
            };

            if (Path.GetExtension(path).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                //var encoding = Encoding.UTF8;
                //using (var memoryStream = new FileStream())
                //{
                //    ((Blob)entry.Target).GetContentStream().CopyTo(memoryStream);
                //    StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8, true);
                //    reader.BaseStream.CopyTo(memoryStream);
                //    model.Data = memoryStream.ToArray();
                //}
                return null;
            }
            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    ((Blob)entry.Target).GetContentStream().CopyTo(memoryStream);
                    model.Data = memoryStream.ToArray();
                }
            }

            return model;
        }

        public void Dispose()
        {
            if (_repository != null)
            {
                _repository.Dispose();
            }
        }

        private IEnumerable<RepositoryTreeDetailModel> GetTreeModelsWithDetails(Commit commit, IEnumerable<TreeEntry> tree, string referenceName)
        {
            var ancestors = _repository.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = commit, SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Reverse }).ToList();
            var entries = tree.ToList();
            var result = new List<RepositoryTreeDetailModel>();

            for (int i = 0; i < ancestors.Count && entries.Any(); i++)
            {
                var ancestor = ancestors[i];

                for (int j = 0; j < entries.Count; j++)
                {
                    var entry = entries[j];
                    var ancestorEntry = ancestor[entry.Path];
                    if (ancestorEntry != null && entry.Target.Sha == ancestorEntry.Target.Sha)
                    {
                        result.Add(CreateRepositoryDetailModel(entry, ancestor, referenceName));
                        entries[j] = null;
                    }
                }

                entries.RemoveAll(entry => entry == null);
            }

            return result;
        }

        private IEnumerable<RepositoryTreeDetailModel> GetTreeModels(IEnumerable<TreeEntry> tree, string referenceName)
        {
            return tree.Select(i => CreateRepositoryDetailModel(i, null, referenceName)).ToList();
        }

        private RepositoryTreeDetailModel CreateRepositoryDetailModel(TreeEntry entry, Commit ancestor, string treeName)
        {
            var originMessage = ancestor != null ? ancestor.Message : String.Empty;
            var commitMessage = String.Empty;

            return new RepositoryTreeDetailModel
            {
                Name = entry.Name,
                CommitDate = ancestor != null ? ancestor.Author.When.LocalDateTime : default(DateTime?),
                CommitMessage = commitMessage,
                Author = ancestor != null ? ancestor.Author.Name : String.Empty,
                IsTree = entry.TargetType == TreeEntryTargetType.Tree,
                IsLink = entry.TargetType == TreeEntryTargetType.GitLink,
                TreeName = treeName,
                Path = entry.Path.Replace('\\', '/'),
            };
        }

        private Commit GetCommitByName(string name, out string referenceName)
        {
            referenceName = null;

            if (string.IsNullOrEmpty(name))
            {
                referenceName = _repository.Head.FriendlyName;
                return _repository.Head.Tip;
            }

            var branch = _repository.Branches[name];
            if (branch != null && branch.Tip != null)
            {
                referenceName = branch.FriendlyName;
                return branch.Tip;
            }

            var tag = _repository.Tags[name];
            if (tag == null)
            {
                return _repository.Lookup(name) as Commit;
            }

            referenceName = tag.FriendlyName;
            return tag.Target as Commit;
        }
    }
}