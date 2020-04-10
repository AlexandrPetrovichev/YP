using System.Collections.Generic;
using System.IO;
using System;
namespace YP1
{
    class Check
    {
        public Check() 
        {
            
        }
        public Table t = new Table();
        public List<string> word = new List<string> { "" };
        public void Read()
        {
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
            for (int i = 0; i < word.Count; i++)
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
        }
    }
}
