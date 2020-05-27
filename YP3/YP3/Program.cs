using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;

namespace YP3
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
            List<string> q = new List<string>() { "1", "2" };
            List<List<string>> qq = new List<List<string>>();
            qq.Add(q);
            Lex l = new Lex();
            l.ClieanFile();
            //foreach (string s in l.decoment(l.Read()))
            //    l.string_analize(s);
            string s = l.decoment(l.Read())[0];
            Table t = new Table();
            Syntax synt = new Syntax();

            if (l.string_analize(s, t, synt))
                if (synt.analyze_syntactical(t))
                {
                    synt.postfix_print();
                    t.Out();
                }
                else
                    synt.Out_errors();
            else
                l.Out_errors();
        }
    }
}