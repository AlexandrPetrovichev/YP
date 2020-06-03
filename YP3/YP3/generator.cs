using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Reflection.Metadata;
using Microsoft.VisualBasic;

namespace YP3
{
    class Generator
    {
        private List<string> errors = new List<string>();
        private string note;
        public Table t;
        private string data=".386\n" +
                            ".MODEL FLAT, STDCALL\n" +
                            "EXTRN	ExitProcess@4:NEAR\n" +
                            ".data \n" +
                            "\ttmp_var dd ?\n" +
                            "\t tmp_comp dd ?\n";
        
        public string asmCode=".code\n\tSTART:\n;Инициализпция математического со процессора\t\n" +
                              "\tfinit\n";

        public Generator(string note, Table t)
        {
            this.note = note;
            this.t = t;
        }

        int InitialVar()
        {
            foreach (var elem in t.identificators)
            {
                Lexem lex = elem.Value;
                string sType = "";
                switch (lex.type)
                {
                    case ValueType.notype:
                        errors.Add($"undef identificator {lex.Name}");
                        continue;
                        break;
                    case ValueType.flt:
                        sType = "real8";
                        break;
                    case ValueType.integer:
                        sType = "dd";
                        break;
                    default:
                        break;
                }

                string dim;
                if (lex.Dimension == 0)
                {
                    dim = "?";
                }
                else
                {
                    dim = $"\t{lex.Dimension} dup (?)";
                }

                data += $"\t{lex.Name} {sType} {dim}\n";
            }

            return 1;
        }

        int ReadConstants()
        {
            foreach (var elem in t.constant)
            {
                Lexem lex = elem.Value;
                string constname = "const" + lex.Name;
                if (constname.Contains('.'))
                {
                    lex.type = ValueType.flt;
                }
                else
                {
                    lex.type = ValueType.integer;
                    continue;
                }

                constname = constname.Replace(".", "dot");
                t.identificators.Add(lex.Name, new Lexem(constname, (int) lex.type));
                string sType = "";
                switch (lex.type)
                {
                    case ValueType.notype:
                        errors.Add($"undef identificator {lex.Name}");
                        errors.Add("undef identificator");
                        continue;
                        break;
                    case ValueType.flt:
                        sType = "real8";
                        break;
                    case ValueType.integer:
                        sType = "dd";
                        break;
                    default:
                        break;
                }

                string dim;
                if (lex.Dimension == 0)
                {
                    dim = $"{lex.Name}";
                }
                else
                {
                    dim = $"{lex.Dimension} dup (?)";
                }

                data += $"{constname} {sType} {dim}\n";
            }

            return 1;
        }

        string LoadVar(Lexem a)
        {
            string load;
            int tmp;
            if (Int32.TryParse(a.Name,out tmp))
            {
                load = $"\tmov tmp_var, {a.Name}\n" +
                       $"\tfild tmp_var\n";
                return load;
            }
            
            if (a.type == ValueType.integer)
            {
                load = $"\tfild {a.Name}\n";
            }
            else
            {
                load = $"\tfld {a.Name}\n";
            }

            return load;
        }

        string LoadConstant(Lexem c)
        {
            string load = "";
            if (c.type == ValueType.flt)
            {
                load = $"\tfld {c.Name}\n";
            }

            if (c.type == ValueType.integer)
            {
                load = $"\tmov tmp_var {c.Name}\n" +
                       $"\tfild tmp_var\n";
            }
            return load;
        }
        
        string TransformOperation(Stack<Lexem> st,string operation, bool first)
        {
            string rez = "";
            Lexem left, right;
            switch (operation)
            {
                case "+":
                    left= st.Pop();
                    right = st.Pop();
                    rez+=LoadVar(left);
                    rez+=LoadVar(right);
                    rez += "\tfadd\n";
                    first = false;
                    break;
                case "-":
                    if (first)
                    {
                        right= st.Pop();
                        left = st.Pop();
                        rez+=LoadVar(left);
                        rez+=LoadVar(right);
                        rez += "\tfsub\n";
                        first = false;
                    }
                    else
                    {
                        right = st.Pop();
                        rez+=LoadVar(right);
                        rez += "\tfsub\n";
                    }

                    break;
                case "*": 
                    right= st.Pop();
                    left = st.Pop();
                    rez+=LoadVar(left);
                    rez+=LoadVar(right);
                    rez += "\tfmul\n";
                    first = false;
                    break;
                case "=":
                    if (first)
                    {
                        right= st.Pop();
                        left = st.Pop();
                        rez+=LoadVar(right);
                        if (left.type == ValueType.integer)
                        {
                            rez += $"\tfistp {left.Name}\n";
                        }
                        else
                        {
                            rez += $"\tfstp {left.Name}\n";
                        }
                    }
                    else
                    {
                        left= st.Pop();
                        if (left.type == ValueType.integer)
                        {
                            rez += $"\tfistp {left.Name}\n";
                        }
                        else
                        {
                            rez += $"\tfstp {left.Name}\n";
                        }
                    }
                  
                    break;
                case "[]": //ToDO
                    break;
            }
            return rez;
        }

        string TranslateExpression(List<string> expr)
        {
            bool first = true;
            string rez = "";

                Stack<Lexem> st = new Stack<Lexem>();
            foreach (string s in expr)
            {
                if (s == "")
                {
                    continue;
                }
                int num = t.GetNum(s);
                if (num == 11)
                {
                    if (t.identificators.ContainsKey(s))
                    {
                        st.Push(t.identificators[s]);
                    }
                    else
                    {
                        st.Push(t.constant[s]);
                    }
                    continue;
                }

                if (num == 10)
                {
                    st.Push(t.identificators[s]);
                    continue;
                }
                rez += TransformOperation(st, s, first);
                first = false;
                }
            return rez;
        }

        public int Generate()
        {
            ReadConstants();
            InitialVar();
            string[] exprs = note.Split(";");
            foreach (string expr in exprs)
            {
                asmCode += TranslateExpression(new List<string>(expr.Split(' ')));
            }

            asmCode = string.Concat(data, asmCode);
            asmCode += "\tCALL ExitProcess@4\n" +
                       "END START\n";
            return 1;
        }
    }
}