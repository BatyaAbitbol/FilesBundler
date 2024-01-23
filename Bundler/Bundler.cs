using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bundler
{
    public enum SortType
    {
        Name,
        Type
    }
    public static class Bundler
    {
        private static string path = ".\\";
        private static string[] invalidDirs = new String[] { "Debug", "bin", "node_modules", ".git", ".vs", "publish" };
        private static string bundle = string.Empty;
        private static IEnumerable<string>? files;
        public static void Bundle(List<string> languages, FileInfo output, bool note, SortType sort, bool remove, string? author)
        {
            // files of current directory to bundle
            files = Directory.EnumerateFiles(path);
            CollectFilesRec(path, ref files);

            // filter files of required languages only
            if (languages.Any())
                files = files.Where(file => languages.Contains(Path.GetExtension(file).TrimStart('.')));
           
            // sort the files
            if (sort == SortType.Type)
            {
                files = files.OrderBy(file => Path.GetExtension(file));
            }
            else
            {
                files = files.OrderBy(file => file.ToLower());
            }

            // add the author in comment
            if (author != null && !String.IsNullOrEmpty(author))
            {
                bundle += "// Author: " + author + Environment.NewLine;
            }

            // copy the files content into "bundle" variable
            foreach (var file in files)
            {
                // note source code if required
                if (note)
                {
                    bundle += "// Source Code: " + file + Environment.NewLine;
                }

                // remove empty lines if required
                if (remove)
                {
                    var lines = File.ReadAllLines(file).Where(line => !String.IsNullOrEmpty(line));
                    foreach (var line in lines)
                    {
                        bundle += line + Environment.NewLine;
                    }
                }
                else bundle += File.ReadAllText(file);
            }

            // create the bundled file
            try
            {
                File.WriteAllText(output.FullName, bundle);
                Console.WriteLine("BUNDLE command: File was created successfully!");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("ERROR: Path is invalid.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void CollectFilesRec(string path, ref IEnumerable<string> files)
        {
            var _files = Directory.EnumerateFiles(path);
            var _dirs = Directory.EnumerateDirectories(path);

            foreach (var file in _files)
            {
                files = files.Append(file);
            }

            if (!_dirs.Any())
                return;
            _dirs = _dirs.Where(_dir => !invalidDirs.Any(d => _dir.ToLower().EndsWith(d.ToLower())));

            foreach (var dir in _dirs)
            {
                CollectFilesRec(dir, ref files);
            }
        }
    }
}