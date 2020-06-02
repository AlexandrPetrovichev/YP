using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
namespace YP3
{
    class Syntax
    {
        public List<string> list = new List<string>();
        public List<string> errors = new List<string>();
        // Структура элемент таблицы разбора
        public struct table_parse_elem
        {
            public List<string> terminal; // Терминалы
            public int jump; // Переход
            public bool accept; // Принимать или нет
            public bool stack_; // Класть в стек или нет
            public bool return_; // Возвращать или нет
            public bool error; // Может ли быть ошибка
        };
        // Таблица разбора
        public List<table_parse_elem> table_parse = new List<table_parse_elem>();
        // Структура элемент постфиксной записи
        public struct postfix_elem
        {
            public string id;
            public int type;
            public postfix_elem(string id_, int type_)
            {
                id = id_;
                type = type_;
            }
            public postfix_elem(string id_)
            {
                id = id_;
                type = 1;
            }
        }


        // Сравнение приоритетов операций
        public bool priority_le(string what, string with_what)
        {
            int pw = 0, pww = 0;
            if (what == "=" || what == "+=" || what == "-=" || what == "*=") pw = 10;
            else if (what == "!=" || what == ">" || what == "<" || what == "==") pw = 20;
            else if (what == "+" || what == "-") pw = 30;
            else pw = 40;
            if (with_what == "=" || with_what == "+=" || with_what == "-=" || with_what == "*=") pww = 10;
            else if (with_what == "!=" || with_what == ">" || with_what == "<" || with_what == "==") pww = 20;
            else if (with_what == "+" || with_what == "-") pww = 30;
            else if (with_what == "*") pww = 40;
            if (pw <= pww) return true;
            return false;
        }

        // Постфиксная запись
        public List<postfix_elem> postfix_record = new List<postfix_elem>();

        // Отладочный вывод таблиц
        public void debug_print(string o)
        {

        }

        public void Table_parse()
        {
            table_parse_elem t = new table_parse_elem();
            t.jump = 1;
            t.accept = false;
            t.stack_ = true;
            t.return_ = false;
            t.error = true;
            t.terminal = new List<string>();
            List<string> table = Read("table_parse.txt");

            table_parse.Add(t);
            string str = table[0];
            for (int i = 1; i < table.Count(); i++) //(!in_table_parse.eof())
            {
                table_parse_elem te = new table_parse_elem();
                te.terminal = new List<string>();
                str = "";
                while (str.Length == 0 || str.IndexOf("\t") != -1)
                {
                    str = table[i].Split('\t')[1];
                    table[i] = table[i].Remove(0, table[i].IndexOf("\t") + 1);
                }
                int n = str.Split(" ").Length;
                for (int j = 0; j < n; j++)
                {
                    te.terminal.Add(str.Split(' ')[0]);
                    str = str.Remove(0, str.IndexOf(" ") + 1);
                }

                string s;
                table[i] = table[i].Remove(0, table[i].IndexOf("\t") + 1);

                te.jump = Convert.ToInt32(table[i].Split('\t')[0]);
                table[i] = table[i].Remove(0, table[i].IndexOf("\t") + 1);

                s = table[i].Split('\t')[0];
                te.accept = Convert.ToBoolean(Convert.ToInt32(s));
                table[i] = table[i].Remove(0, table[i].IndexOf("\t") + 1);

                s = table[i].Split('\t')[0];
                te.stack_ = Convert.ToBoolean(Convert.ToInt32(s));
                table[i] = table[i].Remove(0, table[i].IndexOf("\t") + 1);

                s = table[i].Split('\t')[0];
                te.return_ = Convert.ToBoolean(Convert.ToInt32(s));
                table[i] = table[i].Remove(0, table[i].IndexOf("\t") + 1);

                s = table[i].Split('\t')[0];
                te.error = Convert.ToBoolean(Convert.ToInt32(s));

                table_parse.Add(te);
            }
            for (int i = 0; i < table_parse[1].terminal.Count(); i++)
                table_parse[0].terminal.Add(table_parse[1].terminal[i]);

        }
        public List<string> Read(string filename)
        {
            List<string> list_token = new List<string>();
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            projectDirectory = projectDirectory.Substring(0, projectDirectory.Length - 4);

            using (StreamReader sr = new StreamReader(projectDirectory + "\\" + filename))
            {
                while (sr.Peek() > -1)
                {
                    list_token.Add(sr.ReadLine());
                }

            }
            return list_token;
        }

        // Синтаксический анализ
        public bool analyze_syntactical(Table t)
        {
            List<string> token = new List<string>();
            List<string> curr_token = new List<string>();
            List<string> next_token = new List<string>();
            Stack<int> parse_stack = new Stack<int>();
            int f = 0;
            bool error_flag = false;
            int curr_row = 0;
            bool have_type = false; // Находимся ли мы в строке с объявлением типа
            int type_type = 1;          // Если находимся, то какой тип объявляем
            bool need_postfix = false;      // Нужно ли выполнять построение постфиксной записи для данной строки
            List<List<string>> code_expr_infix = new List<List<string>>();  // Если да, то сюда помещаем токены в инфиксном (обычном) порядке
            bool need_array_resize = false;         // Объявляем ли мы сейчас размер массива
            List<List<string>> array_resize_expr_infix = new List<List<string>>();  // Если да, то сюда помещаем токены в инфиксном (обычном) порядке
            Table_parse();
            token = Read("token.txt");
            curr_token = token[0].Split(' ').ToList();
            next_token = token[1].Split(' ').ToList();
            int i = 1;

            while (!error_flag && f < token.Count && i < token.Count() + 1)
            {
                string token_str = curr_token[1];
                if (curr_token[0] == "10") token_str = "var";
                if (curr_token[0] == "11") token_str = "const";
                // Ищем терминалы из списка
                bool find_terminal = false;
                for (int j = 0; j < table_parse[curr_row].terminal.Count() && !find_terminal; j++)
                {
                    if (table_parse[curr_row].terminal[j] == token_str)
                        find_terminal = true;
                }

                // Если нашли
                if (find_terminal)
                {
                    if (table_parse[curr_row].stack_)
                        parse_stack.Push(curr_row + 1);

                    if (table_parse[curr_row].accept)
                    {
                        if ((token_str == "var" || token_str == "const") &&
                                        next_token[1] == "=" ||
                                        next_token[1] == "[" && !have_type)
                            need_postfix = true;

                        if ((token_str == "var" || token_str == "const") && have_type && next_token[1] == "[")
                            need_array_resize = true;

                        // Обработка необъявленного типа
                        if (!have_type && token_str == "var")
                        {
                            if (t.ind[curr_token[1]].Type == "notype")
                            {
                                error_flag = true;
                                errors.Add("Syntax Error: Undefined identifier "+curr_token[1]);
                            }
                        }

                        //Обработка унарного минуса
                        bool flag_unary_minus = false;
                        if (curr_row == 54 && need_postfix)
                        {
                            Lexem first = new Lexem("-1");
                            if (!t.constant.ContainsKey("-1"))
                            {
                                t.constant.Add("-1", first);
                            }
                            //t.constant.Add("-1");
                            code_expr_infix.Add(new List<string>() {"11", "-1"});
                            code_expr_infix.Add(new List<string>() { "1", "*"});
                            flag_unary_minus = true;
                        }

                        if (need_postfix)// && !flag_unary_minus)
                            code_expr_infix.Add(curr_token);
                        
                        // Обработка унарного минуса
                        flag_unary_minus = false;
                        if (curr_row == 54 && need_array_resize)
                        {
                            Lexem first = new Lexem("-1");
                            t.constant.Add("-1", first);
                            array_resize_expr_infix.Add(new List<string>() { "11", "-1" });
                            array_resize_expr_infix.Add(new List<string>() { "1", "*" });
                            flag_unary_minus = true;
                        }
                        
                        if (need_array_resize && !flag_unary_minus)
                        {
                            array_resize_expr_infix.Add(curr_token);
                            if (token_str == "=" || token_str == "+=" || token_str == "-=" || token_str == "*=")
                            {
                                error_flag = true;
                                errors.Add("Syntax Error: Can`t assign to array \""+ array_resize_expr_infix[0][1]+ "\"");
                            }
                        }

                        // Если закончили разбор присваивания или части объявления
                        if (token_str == ";" || token_str == ",")
                        {
                            // Добавим все, что разобрали, в постфиксную запись
                            if (!make_postfix(code_expr_infix))
                                error_flag = true;
                            if (need_array_resize && !error_flag)
                            {
                                if (!make_postfix(array_resize_expr_infix))
                                    error_flag = true;
                            }
                            // Сбрасываем все флаги
                            code_expr_infix.Clear();
                            array_resize_expr_infix.Clear();
                            need_postfix = false;
                            need_array_resize = false;
                        }

                        // Если закончили разбор объявления, сбросим флаг объявления
                        if (token_str == ";")
                            have_type = false;

                        // Если попался тип, запоминаем его
                        if (token_str == "int" || token_str == "float")
                        {
                            have_type = true;
                            if (token_str == "int")
                                type_type = 1;
                            if (token_str == "float")
                                type_type = 2;
                        }

                        // Заносим тип в таблицу идентификаторов
                        if (token_str == "var" && have_type && curr_row == 69)
                        {
                            Lexem first = new Lexem(curr_token[1], type_type);
                            t.ind[curr_token[1]].setType(type_type);
                        }
                        curr_token = next_token;
                        if (i < token.Count - 1)
                            next_token = token[i + 1].Split(' ').ToList();
                        i++;
                    }

                    if (table_parse[curr_row].return_)
                    {
                        if (parse_stack.Count() != 0)
                        {
                            curr_row = parse_stack.Peek();
                            parse_stack.Pop();
                        }
                        else // Если внезапно стек пуст
                        {
                            error_flag = true;
                            errors.Add("Syntax Error: Parse stack is empty!\n" + "Syntax Error: Parse stack is empty!"
                                + Convert.ToString(curr_token) + " at token " + curr_token[0] +" "+ curr_token[1]);
                        }
                    }
                    else
                        curr_row = table_parse[curr_row].jump;
                }
                else
                {
                    // Если ошибка безальтернативная
                    if (table_parse[curr_row].error)
                    {
                        error_flag = true;
                        errors.Add("Syntax Error: Unexpected terminal \"" + curr_token[1] + "\"");
                        errors[errors.Count() - 1] += "\nMust be:";
                        for (int j = 0; j < table_parse[curr_row].terminal.Count(); j++)
                            errors[errors.Count() - 1] += "\"" + table_parse[curr_row].terminal[j] + "\"";
                    }
                    else
                    {
                        curr_row++;
                    }
                }
            }

            // Если внезапно стек не пуст
            if (!error_flag && parse_stack.Count() != 0)
            {
                error_flag = true;
                errors.Add("Syntax Error: Parse stack isn`t empty!\nSize = " + Convert.ToString(parse_stack.Count())
                   +" " + "Contains: ");
                while (parse_stack.Count() != 0)
                {
                    errors[errors.Count() - 1] += "\"" + Convert.ToString(parse_stack.Peek()) + "\" \n";
                    parse_stack.Pop();
                }
            }
            return !error_flag;
        }


        // Построение постфиксной записи
        public bool make_postfix(List<List<string>> t)
        {
            Stack<string> stack_temp = new Stack<string>();
            bool error_flag = false;
            int index = 0;
            while (index < t.Count() && !error_flag)
            {
                int i;
                for (i = index; i < (int)t.Count() && !error_flag && t[i][1] != ";" && t[i][1] != ","; i++)
                {
                    string token_text = t[i][1];
                    if (t[i][0] == "10" || t[i][0] == "11")
                    {
                        postfix_record.Add(new postfix_elem(token_text));
                    }
                    else if (token_text == "(" || token_text == "[")
                    {
                        stack_temp.Push(token_text);
                    }
                    else if (token_text == ")")
                    {
                        while (stack_temp.Count() != 0 && stack_temp.Peek() != "(")
                        {
                            string tmpstr = stack_temp.Peek();
                            postfix_record.Add(new postfix_elem(tmpstr));
                            stack_temp.Pop();
                        }
                        if (stack_temp.Count == 0)
                        {
                            errors.Add("Syntax Error: Unexpected \")\" !");
                            error_flag = true;
                        }
                        else
                        {
                            stack_temp.Pop();
                        }
                    }
                    else if (token_text == "]")
                    {
                        while (stack_temp.Count() != 0 && stack_temp.Peek() != "[")
                        {
                            string tmpstr = stack_temp.Peek();
                            postfix_record.Add(new postfix_elem(tmpstr));
                            stack_temp.Pop();
                        }
                        if (stack_temp.Count() == 0)
                        {
                            errors.Add("Syntax Error: Unexpected \")\" !");
                            error_flag = true;
                        }
                        else
                        {
                            postfix_record.Add(new postfix_elem("[]", 3));
                            stack_temp.Pop();
                        }
                    }
                    else if (t[i][0] == "1")
                    {
                        while (stack_temp.Count() != 0 && priority_le(token_text, stack_temp.Peek()))
                        {
                            string tmpstr = stack_temp.Peek();
                            postfix_record.Add(new postfix_elem(tmpstr));
                            stack_temp.Pop();
                        }
                        stack_temp.Push(token_text);
                    }
                }
                if (error_flag)
                {
                    postfix_record.Clear();
                    return false;
                }
                else
                {
                    while (stack_temp.Count() != 0 &&
                            stack_temp.Peek() != "(" && stack_temp.Peek() != ")" &&
                            stack_temp.Peek() != "[" && stack_temp.Peek() != "]")
                    {
                        string tmpstr = stack_temp.Peek();
                        postfix_record.Add(new postfix_elem(tmpstr, 1));
                        stack_temp.Pop();
                    }
                    if (stack_temp.Count() != 0)
                    {
                        errors.Add("Syntax Error: Brackets balance error!");
                        error_flag = true;
                    }
                }
                if (error_flag)
                {
                    postfix_record.Clear();
                    return false;
                }
                if (postfix_record[postfix_record.Count() - 1].id == "[]")
                {
                    postfix_record[postfix_record.Count() - 1] = new postfix_elem("[*]", 2);
                }
                index = i + 1;
                postfix_record.Add(new postfix_elem(";", 4));
            }
            return true;
        }

        public void Out_errors()
        {
            foreach (string w in errors)
                Console.WriteLine(w);
        }

        public void postfix_print()
        {
            Console.WriteLine("Postfix notation:");
            for (int i = 0; i < postfix_record.Count(); i++)
            {
                Console.Write(postfix_record[i].id + " ");
            }
            Console.WriteLine("");
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            projectDirectory = projectDirectory.Substring(0, projectDirectory.Length - 4);
            string writePath = projectDirectory + "\\postfix.txt";

            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                {
                    for (int i = 0; i < postfix_record.Count(); i++)
                    {
                        sw.Write(postfix_record[i].id + " ");
                    }
                    sw.Write( "\n");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}