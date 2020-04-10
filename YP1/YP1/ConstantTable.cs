using System.Collections.Generic;
namespace YP1
{
    class ConstantTable 
    {
        public ConstantTable(int t_num) { size = 0; this.t_num = t_num; }
        private int size;
        private int t_num;
        private List<string> table = new List<string>();
        public void set(string word)
        {
            table.Add(word);
            this.size++;
        }
        public ID get(string w)
        {
            bool flag = false;
            ID id = new ID();
            int i = 0;
            for (int j = 0; j < size && !flag; j++)
                if (table[j] == w)
                {
                    flag = true;
                    i = j;
                }
            if (flag)
            {
                id.init(t_num, i, 0);
                return id;
            }
            else
                return id;
        }
        public string get_elem(ID id)
        {
            return table[id.tt_num];
        }
        public bool contains(string w)
        {
            foreach (string i in table)
                if (i == w)
                    return true;
            return false;

        }

    


    }
}
