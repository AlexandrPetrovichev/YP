using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
namespace YP1
{

    class Program
    {
        static void Main(string[] args)
        {
            Table t = new Table();
            List<string> word = new List<string> { "" };
            string path = @"C:\Users\User\source\repos\YP1\YP1\input.txt";
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        while (!t.delimiter(line[i].ToString()) && !t.space(line[i].ToString()))
                        {
                            word[word.Count - 1] += line[i];
                            i++;
                        }
                        if (t.delimiter(line[i].ToString()))
                            word.Add(line[i].ToString());
                        else
                            word.Add("");
                    }
                }
            }
            for(int i = 0; i < word.Count; i++)
            {
                if (t.contains(word[i]))
                    t.Out(t.search(word[i]));
                else
                {
                    try
                    {

                        Int32.Parse(word[i + 2]);
                        t.hashtable.set(word[i]);
                        t.hashtable.set_val(word[i], Int32.Parse(word[i + 2]));
                        t.Out(word[i]);
                    }
                    catch (FormatException e)
                    {
                    }
                    
                    
                }
            }
            
            string s = "return";
            ID id = t.search(s);
            int f;
            //t.Out(id);
            HashTable h = new HashTable(1);
            
            h.set("flag");
            h.set("flag]");
            h.set_val("flag", 1);
            h.set_val("flag]", 1345);
            h.set_val("flag]", 1343565);
            //Console.WriteLine(h.get("flag]")[0]);
            //Console.WriteLine(h.get("flag]")[1]);


            //id.Out();      
        }
    }
}
