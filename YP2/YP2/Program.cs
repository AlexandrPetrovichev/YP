using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;

namespace YP2
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
            Table table = new Table();
            Lex l = new Lex(table);
            //foreach (string s in l.decoment(l.Read()))
            //    l.string_analize(s);
            string s = l.decoment(l.Read())[0];
            l.string_analize(s);
            l.Out_errors();
            table.PrinVTables();
        }
    }
}