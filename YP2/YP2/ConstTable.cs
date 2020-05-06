using System.Collections.Generic;
using System.IO;

namespace YP2
{
    class ConstTable : SortedSet<string>
    {
        private int num;
        public ConstTable(int num) { this.num = num; }
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