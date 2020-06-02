using System.CodeDom.Compiler;

namespace YP3
{
    class Generator
    {
        private string note;
        public Table t;
        private string data;

        int ReadConstants()
        {
            foreach (var elem in t.constant)
            {
                Lexem lex = elem.Value;
                string constname = "const"+lex.Name;
                constname = constname.Replace(".", "dot");
                t.ind.Add(lex.Name, new Lexem(constname, (int)lex.type));
                data +=$"";
            }

            return 1;
        }
        
        public Generator(string note, Table t)
        {
            data += ".Data\n";
            this.note = note;
            this.t = t;
        }

        public int Generate()
        {
            return 1;
        }
    }
}