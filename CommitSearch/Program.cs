using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CommitSearch
{
    public class Program
    {
        private readonly AzureSearch _client;

        public Program()
        {
            _client = new AzureSearch(
                ConfigurationManager.AppSettings["AzureSearch.Name"],
                ConfigurationManager.AppSettings["AzureSearch.AdminKey"],
                ConfigurationManager.AppSettings["AzureSearch.QueryKey"]);
        }

        static void Main(string[] args)
        {
            var program = new Program();

            switch (args[0])
            {
                case "create":
                    program.CreateIndex();
                    break;
                case "index":
                    program.Index();
                    break;
                case "delete":
                    program.DeleteIndex();
                    break;
                case "search":
                    program.Search(string.Join(" ", args.Skip(1)));
                    break;

            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Finished. Press any key to exit.");
                Console.ReadKey();
            }
        }

        private void DeleteIndex()
        {
            _client.DeleteIndex();
        }

        private void Index()
        {
            var repositories = RepoFinder.FindRepos(Directory.GetCurrentDirectory());

            foreach (var repo in repositories)
            {
                Console.Write($"Indexing commits in {repo.Path}...");
                var commits = CommitFinder.Find(repo.Path);
                var batches = commits.Select(c => new CommitSearchDocument()
                {
                    Author = c.Author.Name,
                    Email = c.Author.Email,
                    Sha = c.Sha,
                    Message = c.Message,
                    Path = repo.Path,
                    Repo = repo.Name,
                    Date = c.Author.When
                }).Batch(1000);

                foreach(var documents in batches)
                    _client.Index(documents);
                Console.WriteLine("Done!");
            }
        }

        private void Search(string searchTerm)
        {
            var results = _client.Search(searchTerm);
            foreach (var result in results.Take(10))
            {
                Console.WriteLine($"{result.Repo} {result.Sha} {result.Author} {result.Message}");
            }
        }

        private void CreateIndex()
        {
            _client.CreateIndex();
        }
    }
}
