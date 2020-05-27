using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace YP3
{

    class VariableTable : Dictionary<string, Lexem>
    {
        private int num;
        public VariableTable(int num) { this.num = num; }

        public bool TryConst(string word)
        {
            int res;
            float r;
            return Int32.TryParse(word, out res) || float.TryParse(word, out r);
        }
    } 
}