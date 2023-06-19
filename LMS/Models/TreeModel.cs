using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;

namespace LMS.Models
{
    public class TreeModel : FileViewModel<Tree>
    {
        private string _parent;

        public string Parent => _parent;
        public IEnumerable<FileViewModel> Children => Object.Select(d => FromGitObject(Repository, d.Path, d.Name, d.Target)).OrderByDescending(i => i.Type == ObjectType.Tree).ThenBy(i => i.Name);

        public TreeModel(Repository repo, string path, string name, Tree obj, string parent = null) : base(repo, path, name, obj)
        {
            _parent = parent;
            if (path != "/")
            {
                GetCommitMessage(path, Repository);
            }
        }

        private void GetCommitMessage(string _path, Repository _repository)
        {
            var path = _path;
            var commit = _repository.Head.Tip;
            var gitObj = commit[path].Target;

            var set = new HashSet<string>();
            var queue = new Queue<Commit>();
            queue.Enqueue(commit);
            set.Add(commit.Sha);

            while (queue.Count > 0)
            {
                commit = queue.Dequeue();
                var go = false;
                foreach (var parent in commit.Parents)
                {
                    var tree = parent[path];
                    if (tree == null)
                        continue;
                    var eq = tree.Target.Sha == gitObj.Sha;
                    if (eq && set.Add(parent.Sha))
                        queue.Enqueue(parent);
                    go = go || eq;
                }
                if (!go)
                    break;
            }
            Comment = commit.MessageShort;
            DateChange = DateTime.Parse(commit.Author.When.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}