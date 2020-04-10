using System;
namespace YP1
{
    class Table
    {
        public Table()
        {
            Create();               
        }
        public HashTable hashtable = new HashTable(4);
        private ConstantTable keywords = new ConstantTable(0);
        private ConstantTable operators  = new ConstantTable(1);
        private ConstantTable delimiters = new ConstantTable(2);
        private ConstantTable spaces = new ConstantTable(3);
        //private HashTable identificators = new HashTable(4);
        //private HashTable constants = new HashTable(5);

        private void Create()
        {
            keywords.set("main");
            keywords.set("return");
            keywords.set("void");
            keywords.set("int");
            keywords.set("float");

            operators.set("+");
            operators.set("-");
            operators.set("*");
            operators.set("/");
            operators.set("=");
            operators.set("+=");
            operators.set("-=");
            operators.set("*=");
            operators.set("/=");
            operators.set("==");
            operators.set("!=");
            operators.set(">");
            operators.set("<");

            delimiters.set(",");
            delimiters.set(";");
            delimiters.set("{");
            delimiters.set("}");
            delimiters.set("(");
            delimiters.set(")");

            spaces.set("\t");
            spaces.set("\n");
            spaces.set(" ");
        }

        public string search(ID id)
        {
            if (id.t_num == 0)
                return keywords.get_elem(id);
            if (id.t_num == 1)
                return operators.get_elem(id);
            if (id.t_num == 2)
                return delimiters.get_elem(id);
            if (id.t_num == 3)
                return spaces.get_elem(id);
            return "no";
        }
        public ID search(string s)
        {
            ID id = new ID();
            if (keywords.contains(s))
                id = keywords.get(s);
            else if (operators.contains(s))
                id = operators.get(s);
            else if (delimiters.contains(s))
                id = delimiters.get(s);
            else if (spaces.contains(s))
                id = spaces.get(s);
            return id;
        }
        public bool keyword(string w) { return keywords.contains(w); }
        public bool operator_(string w) { return operators.contains(w); }
        public bool delimiter(string w) { return delimiters.contains(w); }
        public bool space(string w) { return spaces.contains(w); }

        public bool contains(string w)
        {
            if(keyword(w)||operator_(w)||delimiter(w)||space(w)||hashtable.contains(w))
                return true;
            return false;
        }
        public void Out(ID id)
        {
            Console.WriteLine($"{id.t_num} {id.tt_num} {id.ttt_num} {search(id)}");
        }
        public void Out(string w)
        {
            int h = hashtable.hash(w);
            for (int i = 0; i < hashtable.Size(); i++)
            {
                Console.Write($"{h} {w} ");
                for (int j = 0; j < hashtable.table[h][w].Count; j++)
                    Console.Write(hashtable.table[h][w][j]);
                Console.Write("\n");
            }
        }
    }
}
