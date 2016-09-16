using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommitSearch
{
    public static class RepoFinder
    {
        public static IEnumerable<Repo> FindRepos(string rootPath)
        {
            var directories = Directory.GetDirectories(rootPath);
            return directories.Where(IsGitRepo).Select(d => new Repo() { Name = Path.GetFileName(d), Path = d });
        }

        private static bool IsGitRepo(string path)
        {
            return Directory.Exists(Path.Combine(path, ".git"));
        }
    }
}