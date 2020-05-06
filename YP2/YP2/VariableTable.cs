
using System.Collections.Generic;
using System.ComponentModel;

namespace YP2
{

    class VariableTable : Dictionary<string, Lexem>
    {
        private int num;
        public VariableTable(int num) { this.num = num; }
    }
}