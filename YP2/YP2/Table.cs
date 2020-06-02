using System.Collections.Generic;
using System.IO;
using System;
namespace YP2
{
    class Table
    {
        public Table()
        {
            Create();
        }
        public VariableTable ind = new VariableTable(10);
        public VariableTable constant = new VariableTable(11);
        public ConstTable keywords = new ConstTable(0);
        public ConstTable operators = new ConstTable(1);
        public ConstTable delimiters = new ConstTable(2);
        public ConstTable latters = new ConstTable(4);
        public ConstTable nummber = new ConstTable(5);
        public ConstTable oper = new ConstTable(3);
        private void Create()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            projectDirectory = projectDirectory.Substring(0, projectDirectory.Length - 4);
            keywords.ReadFrom(projectDirectory+"\\keywords.txt");
            operators.ReadFrom(projectDirectory+"\\operators.txt");
            oper.ReadFrom(projectDirectory+"\\oper.txt");
            delimiters.ReadFrom(projectDirectory+"\\delimiters.txt");
            latters.ReadFrom(projectDirectory+"\\letters.txt");
            nummber.ReadFrom(projectDirectory+"\\num.txt");
        }
        public int GetNum(string word)
        {
            if (keywords.Contains(word))
                return 0;
            else if (operators.Contains(word))
                return 1;
            else if (delimiters.Contains(word))
                return 2;
            else if (oper.Contains(word))
                return 3;
            else if (constant.ContainsKey(word))
                return 11;
            else
                return 10;
        }
        public List<string> get(string s)
        {
            int num = GetNum(s);
            List<string> l = new List<string>();
            l.Add(num.ToString());
            l.Add(s);
            return l;
        }

        public void set(string word)
        {
                Lexem first = new Lexem(word);
                ind.Add(word, first);
        }

        public void PrinVTables()
        {
            Console.WriteLine("Const table:");
            foreach (var var in constant)
            {
                Console.WriteLine(var.ToString());
            }
            
            Console.WriteLine("Identifiers table:");
            foreach (var var in ind)
            {
                Console.WriteLine(var.ToString());
            }
            
            
        }

    }
}