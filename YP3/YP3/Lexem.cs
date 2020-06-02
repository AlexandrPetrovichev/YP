using System.Collections.Generic;

namespace YP3
{
    enum ValueType
    {
        notype,
        integer,
        flt
    }
    class Lexem
    {
        private string name;
        public ValueType type = ValueType.notype;
        private List<bool> isInit = new List<bool>(); //определено ли значение
        public Lexem(string name)
        {
            this.name = name;
        }
            public Lexem(string name, int type)
        {
            this.name = name;
            this.type = (ValueType) this.type;
        }

        public string Name
        {
            get => name;
        }
        public string Type
        {
            get => type.ToString("G");
        }
        public void setType(int type)
        {
            this.type = (ValueType)type;
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