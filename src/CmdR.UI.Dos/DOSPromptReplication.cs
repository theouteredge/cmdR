using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdR.UI.Dos
{
    public class DOSPromptReplication
    {
        private string _directory = @"c:\";

        public void ChangeDirectory(IDictionary<string, string> param)
        {
            var path = param["path"];

            if (Directory.Exists(path))
            {
                _directory = path;
            }
            else if (Directory.Exists(_directory + path))
            {
                _directory = _directory + path;
            }
            else Console.WriteLine("{0} is not a valid directory", path);

            if (_directory.Last() != '\\')
                _directory = _directory + "\\";

            Console.WriteLine(_directory);
        }
        
        public void ListDirectory(IDictionary<string, string> param)
        {
            foreach(var file in Directory.GetFiles(_directory))
            {
                Console.WriteLine(Path.GetFileName(file));
            }

            foreach (var directory in Directory.GetDirectories(_directory))
            {
                Console.WriteLine(directory);
            }
        }
        
        public void DeleteFile(IDictionary<string, string> param)
        {
            var file = param["file"];

            if (File.Exists(file))
            {
                File.Delete(file);
            }
            else if (File.Exists(_directory + file))
            {
                File.Delete(_directory + file);
            }
            else Console.WriteLine("{0} does not exist", file);
        }
    }
}
