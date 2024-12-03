using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atualizate.Class
{
    internal class Models
    {
        public class Version
        {
            public string RepoOwner { get; set; }
            public string RepoName { get; set; }
            public string BranchName { get; set; }
            public string TokenAPI { get; set; }
            public string Number { get; set; }
            public string NameExecutable { get; set; }
            public string PathExecutable { get; set; }
            public string NameFileZip { get; set; }
            public string Outdate { get; set; }
        }

        public class Content
        {
            public string name { get; set; }
            public string path { get; set; }
            public string sha { get; set; }
            public long size { get; set; }
            public string url { get; set; }
            public string content { get; set; }
            public string html_url { get; set; }
            public string git_url { get; set; }
            public string download_url { get; set; }
            public string type { get; set; }

        }
    }
}
