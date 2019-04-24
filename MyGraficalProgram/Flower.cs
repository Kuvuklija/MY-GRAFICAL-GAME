using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGraficalProgram
{
    [Serializable]
    public class Flower
    {
        public Point Location { get; private set; }
        public int Age { get; private set; }
        public bool Alive { get; private set;}
        public double Nectar { get; private set; } //нектара в цветке
        public double NectarHarvested { get; private set;} //копим собранный нектар
        int lifespan { get; set; }

        private const int LifeSpanMin=15000;
        private const int LifeSpanMax = 30000;
        private const double InitialNectar = 1.5;
        private const double MaxNectar = 5;
        private const double NectarAddedPerTurn = 0.01; //прибавление нектара в цветке за цикл
        private const double NectarGatheredPerTurn = 0.3; //может быть собрано нектара пчелой с цветка

        public Flower(Point location, Random random){
            Location = location;
            lifespan = random.Next(LifeSpanMin,LifeSpanMax+1); //рандом исключает последнюю циферку, поэтому +1
            Age = 0;
            Alive = true;
            Nectar = InitialNectar;
            NectarHarvested = 0;
        }

        //метод для Bee 
        public double HarvestNectar() {
            if (NectarGatheredPerTurn> Nectar) { /*пчелы могли выбрать весь нектар из цветка-->
                                                   это условие странное, ибо мин нектара для полета пчелы=1.5
                                                   НЕТ! пчела уже полетела и выбирает весь нектар из цветка в каждом кадре по 0.3*/
                                                 
                return 0;
            }
            else {
                 NectarHarvested += NectarGatheredPerTurn;
                 Nectar -= NectarGatheredPerTurn;
                 return NectarGatheredPerTurn;
            }
        }

        public void Go() {
            Age++;
            if (Age >= lifespan)
            {
                Alive = false;
                return;
            }

            Nectar += NectarAddedPerTurn;
            if (Nectar > MaxNectar) {
                Nectar = MaxNectar;
            }
        }
    }

}
