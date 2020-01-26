using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace GeneticAlgorithm
{
    class GeneticEvolution
    {
        delegate double Func(double x, double y);
        Func func;//переменная - указатель на функцию 
        List<Individual> Population; 
        double MutProb { get; set; }//вероятность мутации
        double MaxPairs { get; set; }//максимальное кол-во пар, размножающихмя в 1-м поколении
        FunctionTree Function;//
        byte[] randomNumber = new byte[1];
        int IndBest=0;
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        public GeneticEvolution(string Function, double MutProb = 0.8, double max_pairs = 500)//инициализация основным полей
        {
            this.Function = new FunctionTree(Function);//создаем дерево введенной функции
            func = (x, y) => this.Function.GetValue(x,y);
            Population = new List<Individual>();
            this.MutProb = MutProb;
            this.MaxPairs = max_pairs;
        }


        private List<Individual> GenerateRandomPopulation(int size = 1000)//рандомно генерируем популяцию
        {
            Random rand = new Random();
            for (int i = 0; i < size; i++)
            {
                Individual ind;
                ind.x = Math.Round(rand.Next(0, 200000) / 100.0 - 1000, 2);
                ind.y = Math.Round(rand.Next(0, 200000) / 100.0 - 1000, 2);
                ind.ValueFunc = func(ind.x, ind.y);
                Population.Add(ind);
            }
            return Population;
        }
        public List<Individual> Initialize()
        {
            //Population.Sort();
            return GenerateRandomPopulation();

        }


        public List<Individual> Evolute()//функция эволюции популяции
        {
            IndBest = 0;
            Random rand = new Random();
            int index = 0;
            List<Individual> Children = new List<Individual>();
            while(true)
                {
                    index++;
                    if (index > MaxPairs)
                        break;
                    Individual a = Mutate(Population[rand.Next(0, Population.Count)]);//из лучшей половины поколения выбираем родителей
                    Individual b = Mutate(Population[rand.Next(0, Population.Count)]);
                    Individual NewIndiv = Crossover(a, b);//порождаем новую особь
                    Population.Add(NewIndiv);//добовляем новую особь в популяцию
                    Children.Add(NewIndiv);
                }
            return Children;;
        }

        private Individual Crossover(Individual a, Individual b, double prob = 0.5)//фукция-скрещивания
        {
            Random rand = new Random();
            if (rand.Next(0, 1000000) / 1000000.0 > prob)
                return new Individual() { x = a.x, y = b.y, ValueFunc = func(a.x, b.y)};
            else
                return new Individual() { x = b.x, y = a.y, ValueFunc = func(b.x, a.y)};
        }

        private Individual Mutate(Individual a)//функция мутации
        {
            rngCsp.GetBytes(randomNumber);
            if (randomNumber[0]%1000000 / 1000000.0 < MutProb)
            { 
                a.x = Math.Round(a.x + (randomNumber[0]%100/100.0 - 0.5),2) ;
                a.y = Math.Round(a.y + (randomNumber[0] % 100/100.0 - 0.5),2);
                return a;
            }
            else
            {
                return a;
            }
        }
        public List<Individual> Killing()//функция отбора
        {
            List<Individual> IndividualForKilling = new List<Individual>();//создаем список особей для удаления c экрана
            Population.Sort();
            while (Population.Count != 500)
            {
                IndividualForKilling.Add(Population[Population.Count - 1]);//добавляем особь, чтобы удалить ее с экрана
                Population.RemoveAt(Population.Count - 1);//удаляем особь из популяции алгоритма
            }
            return IndividualForKilling;
        }
        public Individual Best()//функция определения лучшей особи
        {
            IndBest++;
            return Population[IndBest-1];
        }
        public double BestError()
        {
            return Population[0].ValueFunc;
        }

    }
}
    
