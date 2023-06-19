using LibGit2Sharp;

namespace LMS.Models
{
    public class BlobModel : FileViewModel<Blob>
    {
        public bool IsBinary => Object.IsBinary;

        public string Content => Object.GetContentText();
        public long Size => Repository.ObjectDatabase.RetrieveObjectMetadata(Object.Id).Size;

        public BlobModel(Repository repo, string path, string name, Blob obj) : base(repo, path, name, obj)
        {
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