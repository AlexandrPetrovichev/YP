using System.Collections.Generic;

namespace YP3
{
    enum ValueType
    {
        undef,
        integer,
        flt
    }
    class Lexem
    {
        private string name;
        private string type = "notype";
        private List<bool> isInit = new List<bool>(); //определено ли значение
        public Lexem(string name)
        {
            this.name = name;
        }
            public Lexem(string name, int type)
        {
            this.name = name;
            if (type == 1)
                this.type = "int";
            else
                this.type = "float";
        }

        public string Name
        {
            get => name;
        }
        public string Type
        {
            get => type;
        }
        public void setType(int type)
        {
            if (type == 1)
                this.type = "int";
            else
                this.type = "float";
        }
        public int Dimension
        {
            get => isInit.Count;
        }

        public List<bool> IsInit
        {
            get => isInit;
            set => isInit = value;
        }
        public override string ToString()
        {
            string ToReturn = $"name = {name}, type = {Type}, dimension = {Dimension}| initialized:";
            for (int i = 0; i < isInit.Count; i++)
            {
                if (isInit[i])
                {
                    ToReturn += $" {i} ,";
                }
            }

            return ToReturn;
        }
    };
}