using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    struct Individual : IComparable//класс особи популяции
    {
        public double x;//значение х точки
        public double y;//значение у точки
        public double ValueFunc;//значение функции в данной точке

        public int CompareTo(object obj)
        {
            Individual ind;
            try
            {
                ind = (Individual)obj;
            }
            catch
            {
                throw new ArgumentException("object is not Individual");
            }
            return this.ValueFunc.CompareTo(ind.ValueFunc);
        }
    }
}
