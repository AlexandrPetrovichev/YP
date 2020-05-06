using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace YP2
{
    class Lex
    {
        private string[] literals;
        public List<string> errors = new List<string>();
        public string[] Read()
        {
            literals = File.ReadAllLines("C:\\Users\\User\\source\\repos\\YP2\\YP2\\input.txt");
            return literals;
        }
        public List<string> decoment(string[] s) //удаление пробелов
        {
            string str, str_1 = "";
            int index, index_1, index_2;
            str = String.Join("\n", s);
            List<string> new_s = new List<string>();
            string[] str_old = str.Split('\n');
            foreach (string line in str_old)
            {
                index = line.IndexOf("//");
                index_1 = line.IndexOf("/*");
                if ((index < index_1 && index_1 != -1 && index != -1) || index != -1)
                    str_1 += line.Remove(index);
                else
                    str_1 += line + "\n";
            }
            str = str_1;
            index_1 = str.IndexOf("/*");
            index_2 = str.IndexOf("*/");
            while (index_1 != -1 && index_2 != -1)
            {
                if (index_2 > index_1)
                    str = str.Remove(index_1, index_2 - index_1 + 2);
                //else
                //    Console.WriteLine("error, incorrect comment");
                index_1 = str.IndexOf("/*");
                index_2 = str.IndexOf("*/");
            }
            new_s.Add(str);
            return new_s;
        } 
        public bool string_analize(string str_old)
        {
            Table t = new Table();
            string ex = @"\s+", str = str_old;
            string[] s = Regex.Split(str, ex);
            string[] q = { "" };
            s = s.Except(q).ToArray();
            if (s.Length > 0)
            {
                string word;
                word = s[0];
                int num = t.GetNum(word);
                switch (num)
                {
                    case 0:     //Ключевое слово
                        {
                            List<string> l = t.get(word);
                            Console.WriteLine(l[0] + " " + l[1]);
                            str = str.Remove(0, str.IndexOf(word) + word.Length);
                            return string_analize(str);
                        }
                        break;
                    case 1:     //Операция
                        {
                            List<string> l = t.get(word);
                            Console.WriteLine(l[0] + " " + l[1]);
                            str = str.Remove(0, str.IndexOf(word) + 1);
                            return string_analize(str);
                        }
                        break;
                    case 2:     //Разделитель
                        {
                            List<string> l = t.get(word);
                            Console.WriteLine(l[0] + " " + l[1]);
                            str = str.Remove(0, str.IndexOf(word) + word.Length);
                            return string_analize(str);
                        }
                        break;
                    case 3: //Оператор
                        {
                            {
                                List<string> l = t.get(word);
                                Console.WriteLine(l[0] + " " + l[1]);
                                str = str.Remove(0, str.IndexOf(word) + word.Length);
                                return string_analize(str);
                            }
                        }
                        break;
                    case 10:     //Индентификатор
                        {
                            for (int i = 0; i < word.Length - 1; i++)
                            {
                                string a = word[i].ToString(), b = word[i + 1].ToString();
                                if (t.oper.Contains(a + b))
                                {
                                    str = str.Insert(str.IndexOf(word[i]), " ");
                                    str = str.Insert(str.IndexOf(word[i + 2]), " ");
                                    return string_analize(str);
                                }
                            }
                            for (int i = 0; i < word.Length; i++)
                            {
                                string a = word[i].ToString();
                                if (t.operators.Contains(a) || t.delimiters.Contains(a))
                                {
                                    str = str.Insert(str.IndexOf(word[i]), " ");
                                    str = str.Insert(str.IndexOf(word[i])+1, " ");
                                    return string_analize(str);
                                }
                            }
                            int n;
                            if(Int32.TryParse(word, out n))
                            {
                                if(!t.constant.ContainsKey(word))
                                {
                                    Lexem first = new Lexem(word);
                                    t.constant.Add(word, first);
                                }
                                List<string> l = t.get(word);
                                Console.WriteLine(l[0] + " " + l[1]);
                                str = str.Remove(0, str.IndexOf(word) + word.Length);
                                return string_analize(str);
                            }
                            bool f = true;
                            foreach (char c in word)
                                f = f && (t.latters.Contains(c.ToString())) || t.nummber.Contains(c.ToString());
                            if (!f)
                            {
                                int i = 0;
                                for (i = 0; i < literals.Length; i++)
                                {
                                    if (literals[i].IndexOf(word) != -1)
                                        break;
                                }
                                errors.Add(String.Format("Error, incorrect identifier {0} in line {1}", word, i+1));
                                str = str.Remove(0, str.IndexOf(word) + word.Length);
                                return string_analize(str);
                            }
                            else
                            {
                                if (!t.ind.ContainsKey(word))
                                    t.set(word);
                                List<string> l = t.get(word);
                                Console.WriteLine(l[0] + " " + l[1]);
                                str = str.Remove(0, str.IndexOf(word) + word.Length);
                                return string_analize(str);
                            }
                        }
                        break;
                    default:        //Что-то плохое
                        {
                            int i = 0;
                            for (i = 0; i < literals.Length; i++)
                            {
                                if (literals[i].IndexOf(word) != -1)
                                    break;
                            }
                            errors.Add(String.Format("Error, incorrect identifier {0} in line {1}", word, i+1));
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        public void Out_errors()
        {
            foreach(string w in errors)
                Console.WriteLine(w);
        }
    }

}