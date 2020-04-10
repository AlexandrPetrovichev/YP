using System.Collections.Generic;
using System;
namespace YP1
{
    class HashTable
    {
        public HashTable(int t_num){ this.t_num = t_num; }
        private string val;
        public int t_num, size;
        public int Size() { return table.Count; }
        public Dictionary<int, Dictionary<string, List<int>>> table = new Dictionary<int, Dictionary<string, List<int>>>();

        public void set(string val)
        {
            int h = hash(val);
            Dictionary<string, List<int>> val_d = new Dictionary<string, List<int>>();
            List<int> l = new List<int> { 0 };
            if (!table.ContainsKey(h))
            {
                val_d.Add(val, l);
                table.Add(h, val_d);
            }
        }
        public void set_val(string ind, int val)
        {
            int h = hash(ind);
            if (table[h][ind][0] == 0)
                table[h][ind][0] = val;
            else
                table[h][ind].Add(val);
        }
        public List<int> get(string val)
        {
            int h = hash(val);
            return table[h][val];
        }
        public int hash(string val)
        {
            int hash = 1;
            for (int i = 0; i < val.Length; i++)
                hash = hash * 2 + System.Math.Abs(val[i]);
            //if (table.Count != 0)
            //    return hash / table.Count;
            return hash;
        }
        public bool contains(string val)
        {
            int h = hash(val);
            if (table[h] == null)
                return false;
            return true;
        }
        

    }
}
