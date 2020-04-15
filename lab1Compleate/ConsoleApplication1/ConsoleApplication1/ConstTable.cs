using System.Collections.Generic;
using System.IO;

namespace ConsoleApplication1
{
    class ConstTable:SortedSet<string>
    {
        public bool ReadFrom(string fileName)
        {
            string[] literals = File.ReadAllLines(fileName);
            if (literals.Length == 0)
            {
                return false;
            }
            foreach (var item in literals)
            {
                Add(item);
            }
            return true;
        }
    }
}