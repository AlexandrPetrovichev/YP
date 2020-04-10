using System;

namespace YP1
{
    class ID
    {
        public ID() { t_num = -1; tt_num = -1; ttt_num = -1; }
        public int t_num;
        public int tt_num;
        public int ttt_num;
        public void init(int t_num, int tt_num, int ttt_num)
        {
            this.t_num = t_num;
            this.tt_num = tt_num;
            this.ttt_num = ttt_num;
        }
        public void init(ID id)
        {
            this.t_num = id.t_num;
            this.tt_num = id.tt_num;
            this.ttt_num = id.ttt_num;
        }
        public void Out()
        {
            Console.WriteLine($"{t_num} {tt_num} {ttt_num}");
        }
    }
}
