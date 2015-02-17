using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace DuplicateFilesLocator
{
    class Program
    {
        private static Dictionary<string, FileComputed> FilesCounter;

        static void Main(string[] args)
        {
            string startPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            if (args.Length > 0 && !args[0].Equals("."))
            {
                startPath = args[0];
            }

            Program.FilesCounter = new Dictionary<string, FileComputed>();

            DirectoryInfo rootDirectory = new DirectoryInfo(startPath);

            PerformDirectoryAction(rootDirectory);

            if (Program.FilesCounter.Any(f => f.Value.Amount > 1))
            {
                var duplicatedFiles = Program.FilesCounter.Where(f => f.Value.Amount > 1).ToArray();
                foreach (var item in duplicatedFiles)
                {
                    Console.WriteLine(String.Format("Duplicate found: {0}", item.Value.Amount));
                    item.Value.Paths.ForEach((path) => Console.WriteLine(String.Format("File: {0}", path)));
                }
            }
            else
            {
                Console.WriteLine("No duplicates found");
            }

            Console.WriteLine("Press ENTER to end");
            Console.ReadLine();
        }

        private static void PerformDirectoryAction(IEnumerable<DirectoryInfo> directories)
        {
            foreach (var directory in directories)
            {
                PerformDirectoryAction(directory);
            }
        }

        private static void PerformDirectoryAction(DirectoryInfo directory)
        {
            if (!directory.Exists) return;

            var innerDirectories = directory.GetDirectories();
            PerformDirectoryAction(innerDirectories);

            var innerFiles = directory.GetFiles();
            PerformFileAction(innerFiles);
        }

        private static void PerformFileAction(IEnumerable<FileInfo> files)
        {
            foreach (var file in files)
            {
                PerformFileAction(file);
            }
        }

        private static void PerformFileAction(FileInfo file)
        {
            if (!file.Exists) return;

            var fileBytes = GetFileBytes(file);
            var fileHash = ComputeFileHash(fileBytes);

            if (Program.FilesCounter.ContainsKey(fileHash))
            {
                Program.FilesCounter[fileHash].AddItem(file.FullName);
            }
            else
            {
                Program.FilesCounter[fileHash] = new FileComputed(fileHash, file.FullName);
            }
        }

        private static byte[] GetFileBytes(FileInfo file)
        {
            byte[] fileBytes = new byte[file.Length];
            using (FileStream stream = file.OpenRead())
            {
                stream.Read(fileBytes, 0, fileBytes.Length);
                stream.Close();
            }
            return fileBytes;
        }

        private static string ComputeFileHash(byte[] fileBytes)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var ret = sha1.ComputeHash(fileBytes);
            return BitConverter.ToString(ret);
        }
    }
}
