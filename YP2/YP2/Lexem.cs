using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace YP2
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
        private int dimension; //размерность: 1 - для переменных и констант
        private List<bool> isInit = new List<bool>(); //определено ли значение

        public Lexem(string name)
        {
            int k;
            float t;
            this.name = name;
            if (int.TryParse(name, out k))
            {
                isInit.Add(true);
                return;
            }
            
            if (float.TryParse(name,out t))
            {
                isInit.Add(true);
            }
            
        }

        public string Name
        {
            get => name;
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

        public ValueType Type1
        {
            get => Type;
            set => Type = value;
        }
        public ValueType Type = (ValueType)0;

        public override string ToString()
        {
            string ToReturn = $"name = {name}, type = {Type}, dimension = {Dimension}| initialized:";
            for (int i = 0; i < isInit.Count; i++)
            {
                if (isInit[i])
                {
                    ToReturn += $" {isInit[i]} ,";
                }
            }

            if (Dimension == 0)
            {
                ToReturn+= false;
            }

            return ToReturn;
        }

        public override bool Equals(object obj)
        {
            Lexem p = (Lexem)obj;
            return this.name == p.name;
        }
    };
}