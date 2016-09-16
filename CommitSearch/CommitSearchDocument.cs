using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace CommitSearch
{
    public class CommitSearchDocument
    {
        [JsonProperty("repo")]
        public string Repo { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("sha")]
        public string Sha { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }
    }
}