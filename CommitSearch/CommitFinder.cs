using System;
using System.Collections.Generic;
using System.Linq;

using LibGit2Sharp;

namespace CommitSearch
{
    public static class CommitFinder
    {
        public static IEnumerable<Commit> Find(string path)
        {
            var repo = new Repository(path);
            return repo.Commits;
        }
    }
}