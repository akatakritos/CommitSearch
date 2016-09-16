using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace CommitSearch
{
    public class AzureSearch
    {
        public string Name { get;  }
        public string AdminKey { get; }
        public string QueryKey { get; }

        public AzureSearch(string name, string adminKey, string queryKey)
        {
            Name = name;
            AdminKey = adminKey;
            QueryKey = queryKey;
        }

        public void CreateIndex()
        {
            var client = new SearchServiceClient(Name, new SearchCredentials(AdminKey));
            var definition = new Index()
            {
                Name = "commits",
                Fields = new List<Field>()
                {
                    new Field("sha", DataType.String) { IsKey = true, IsFilterable = true, },
                    new Field("repo", DataType.String) {IsFilterable = true, IsFacetable = true, IsSortable = true},
                    new Field("path", DataType.String) {IsSortable = true, IsFilterable = true, IsFacetable = true},
                    new Field("author", DataType.String) { IsSortable = true, IsFilterable = true, IsFacetable = true },
                    new Field("email", DataType.String) { IsSortable = true, IsFilterable = true },
                    new Field("message", DataType.String) { IsSearchable = true, IsFilterable = true, Analyzer = AnalyzerName.EnMicrosoft },
                    new Field("date", DataType.DateTimeOffset) { IsSortable = true, IsFilterable = true },
                }
            };

            client.Indexes.Create(definition);
        }

        public void DeleteIndex()
        {
            var client = new SearchServiceClient(Name, new SearchCredentials(AdminKey));
            client.Indexes.Delete("commits");
        }

        public void Index(IEnumerable<CommitSearchDocument> commits)
        {
            var client = new SearchIndexClient(Name, "commits", new SearchCredentials(AdminKey));
            var actions = commits.Select(IndexAction.MergeOrUpload);
            var batch = IndexBatch.New(actions);

            try
            {
                client.Documents.Index(batch);
            }
            catch (IndexBatchException e)
            {
                Console.WriteLine(
                    "Failed to index some of the documents: {0}",
                    string.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }
        }

        public IEnumerable<CommitSearchDocument> Search(string searchTerm)
        {
            var client = new SearchIndexClient(Name, "commits", new SearchCredentials(QueryKey));
            var results = client.Documents.Search<CommitSearchDocument>(searchTerm);
            return results.Results.Select(r => r.Document);
        }
    }
}