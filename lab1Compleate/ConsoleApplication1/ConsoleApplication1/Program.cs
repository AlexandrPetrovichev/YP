using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;

namespace ConsoleApplication1
{
    enum LexType
    {
        undef,
        integer,
        flt
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            ConstTable cTable = new ConstTable();
            
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            var path = projectDirectory+ "\\input.txt";
            cTable.ReadFrom(path);
            foreach (var item in cTable)
            {
                Console.WriteLine(item.ToString());
            }

            bool check;
            check = cTable.Contains("main");
            check = cTable.Contains("double");
            string a;
            check = cTable.TryGetValue("main", out a);
            Lexem first = new Lexem("x");
            VariableTable vTable = new VariableTable();
            vTable.Add("x", first);
            Lexem second = vTable["x"];
            second.IsInit.Add(true);
            second.Type = ValueType.integer;
            Console.WriteLine(vTable["x"].ToString());
        }
    }
}