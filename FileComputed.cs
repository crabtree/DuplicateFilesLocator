using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFilesLocator
{
    public class FileComputed
    {
        public int Amount { get; set; }

        public string Hash { get; set; }

        public List<string> Paths { get; set; }

        public FileComputed(string hash, string path)
        {
            this.Amount = 1;
            this.Hash = hash;
            this.Paths = new List<string>();
            this.Paths.Add(path);
        }

        public void AddItem(string path)
        {
            this.Amount += 1;
            this.Paths.Add(path);
        }
    }
}
