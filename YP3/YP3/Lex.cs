using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace YP3
{
    class Lex
    {
        private string[] literals;
        public List<string> errors = new List<string>();
        public void ClieanFile()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            projectDirectory = projectDirectory.Substring(0, projectDirectory.Length - 4);
            File.WriteAllText(projectDirectory + "\\token.txt", string.Empty);
            File.WriteAllText(projectDirectory + "\\postfix.txt", string.Empty);
            File.WriteAllText(projectDirectory + "\\error.txt", string.Empty);
        }
        public string[] Read()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            projectDirectory = projectDirectory.Substring(0, projectDirectory.Length - 4);
            literals = File.ReadAllLines(projectDirectory + "\\input.txt");
            return literals;
        }
        public void Write(List<string> l)
        {

            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            projectDirectory = projectDirectory.Substring(0, projectDirectory.Length - 4);
            string writePath = projectDirectory + "\\token.txt";

            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                {
                    sw.Write(l[0] + " " + l[1]+"\n");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public List<string> decoment(string[] s) 
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
                else
                    Console.WriteLine("error, incorrect comment");
                index_1 = str.IndexOf("/*");
                index_2 = str.IndexOf("*/");
            }
            new_s.Add(str);
            return new_s;
        }
        public bool string_analize(string str_old, Table t, Syntax synt)
        {
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
                            Write(l);
                            //Console.WriteLine(l[0] + " " + l[1]);
                            str = str.Remove(0, str.IndexOf(word) + word.Length);
                            synt.list.Add(word);
                            return string_analize(str, t, synt);
                        }
                    case 1:     //Операция
                        {
                            List<string> l = t.get(word);
                            Write(l);
                            //Console.WriteLine(l[0] + " " + l[1]);
                            str = str.Remove(0, str.IndexOf(word) + 1);
                            synt.list.Add(word);
                            return string_analize(str, t, synt);
                        }
                    case 2:     //Разделитель
                        {
                            List<string> l = t.get(word);
                            Write(l);
                            //Console.WriteLine(l[0] + " " + l[1]);
                            str = str.Remove(0, str.IndexOf(word) + word.Length);
                            synt.list.Add(word);
                            return string_analize(str, t, synt);
                        }
                    case 3: //Оператор
                        {
                            {
                                List<string> l = t.get(word);
                                Write(l);
                                //Console.WriteLine(l[0] + " " + l[1]);
                                str = str.Remove(0, str.IndexOf(word) + word.Length);
                                synt.list.Add(word);
                                return string_analize(str, t, synt);
                            }
                        }
                    case 10:     //Индентификатор
                        {
                            for (int i = 0; i < word.Length - 1; i++)
                            {
                                string a = word[i].ToString(), b = word[i + 1].ToString();
                                if (t.oper.Contains(a + b))
                                {
                                    str = str.Insert(str.IndexOf(word[i]), " ");
                                    str = str.Insert(str.IndexOf(word[i + 2]), " ");
                                    return string_analize(str, t, synt);
                                }
                            }
                            for (int i = 0; i < word.Length; i++)
                            {
                                string a = word[i].ToString();
                                if (t.operators.Contains(a) || t.delimiters.Contains(a) || t.constant.TryConst(word))
                                {
                                    str = str.Insert(str.IndexOf(word[i]), " ");
                                    str = str.Insert(str.IndexOf(word[i]) + 1, " ");
                                    return string_analize(str, t, synt);
                                }
                            }
                            int n;
                            if (Int32.TryParse(word, out n))
                            {
                                if (!t.constant.ContainsKey(word))
                                {
                                    Lexem first = new Lexem(word);
                                    t.constant.Add(word, first);
                                }
                                List<string> l = t.get(word);
                                Write(l);
                                //Console.WriteLine(l[0] + " " + l[1]);
                                synt.list.Add(word);
                                str = str.Remove(0, str.IndexOf(word) + word.Length);
                                return string_analize(str, t, synt);
                            }
                            bool f = true;
                            foreach (char c in word)
                                f = f && (t.latters.Contains(c.ToString())) || t.nummber.Contains(c.ToString()) && t.latters.Contains(word[0].ToString());
                            if (!f)
                            {
                                int i = 0;
                                for (i = 0; i < literals.Length; i++)
                                {
                                    if (literals[i].IndexOf(word) != -1)
                                        break;
                                }
                                errors.Add(String.Format("Error, incorrect identifier {0} in line {1}", word, i + 1));
                                str = str.Remove(0, str.IndexOf(word) + word.Length);
                                return string_analize(str, t, synt);
                            }
                            else
                            {
                                if (!t.ind.ContainsKey(word))
                                    t.set(word);
                                List<string> l = t.get(word);
                                Write(l);
                                //Console.WriteLine(l[0] + " " + l[1]);
                                synt.list.Add(word);
                                str = str.Remove(0, str.IndexOf(word) + word.Length);
                                return string_analize(str, t, synt);
                            }
                        }
                    case 11:
                        {
                            List<string> l = t.get(word);
                            Write(l);
                            Lexem first = new Lexem(word);
                            if(!t.constant.ContainsKey(word))
                                t.constant.Add(word, first);
                            str = str.Remove(0, str.IndexOf(word) + word.Length);
                            synt.list.Add(word);

                            return string_analize(str, t, synt);
                        }
                    default:        //Что-то плохое
                        {
                            int i = 0;
                            for (i = 0; i < literals.Length; i++)
                            {
                                if (literals[i].IndexOf(word) != -1)
                                    break;
                            }
                            errors.Add(String.Format("Error, incorrect identifier {0} in line {1}", word, i + 1));
                            return false;
                        }
                }
            }
            if (errors.Count == 0)
                return true;
            else
                return false;
        }

        public void Out_errors()
        {
            foreach(string w in errors)
                Console.WriteLine(w);
        }
    }

}