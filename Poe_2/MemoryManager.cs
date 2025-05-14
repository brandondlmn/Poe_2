using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poe_2
{
    public class MemoryManager
    {
        private string path_return()
        {
            string fullpath = AppDomain.CurrentDomain.BaseDirectory;
            string new_path = fullpath.Replace("bin\\Debug\\", "");
            string path = Path.Combine(new_path, "memory.txt");
            return path;
        }

        public void check_file()
        {
            string path = path_return();
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }

        public List<string> return_memory()
        {
            string path = path_return();
            check_file();
            return new List<string>(File.ReadAllLines(path));
        }

        public void save_memory(List<string> save_new)
        {
            string path = path_return();
            check_file();
            File.AppendAllLines(path, save_new);
        }
    }
}
