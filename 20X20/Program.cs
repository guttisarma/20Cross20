using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _20X20
{
   public class Number
    {
      
        long Num { get; set; }
       public Number(long _Num)
        {
            Num = _Num;
        }

       public long SumNumber()
        {
            long _sNum = 0;
            while (Num % 10 != 0)
            {
                _sNum += Num % 10;
                Num = Num / 10;
            }
            return _sNum;
        }

      public  bool isEven()
        {
            return Num % 2 == 0 ? true : false;
        }

       public bool isPrime()
        {
            int count = 0;
            for (int i = 3; i <= Num / 2; ++i)
            {
                if (Num % i == 0)
                    count++;
            }
            if (count > 1)
                return false;
            else
                return true;
        }
    }

    
    class Program
    {
        static long SumNumber(long Num)
        {
            long _num=0;
            while (Num%10!=0)
	        {
	                _num+= Num%10;
                    Num=Num/10;
	        }
            return _num;
        }


        static void Main(string[] args)
        {
            List<long> lsnum = new List<long>();
            Dictionary<long, long> poselyne = new Dictionary<long, long>();
            Number n=new Number(7);
            
            Console.WriteLine("this " + n.isPrime().ToString());
            Console.ReadLine();
            //foreach (var eachnum in lsnum)
            //{
                
            //   long key= SumNumber(eachnum);
            //   if (poselyne.ContainsKey(key))
            //   {
            //       poselyne[key] += 1;
            //   }
            //   else
            //   {
            //       poselyne.Add(key, 1);
            //   }
            //} 
            //foreach (var item in collection)
            //{
                
            //}

            
        }
    }
}
