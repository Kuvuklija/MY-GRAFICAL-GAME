using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGraficalProgram
{
    //состояния пчелы
    public enum BeeState { Idle, FlyingToFlower, GatheringNectar, ReturningToHive, MakingHoney, Retired, Conrol }
    public delegate void StingDelegate(bool sting);

    [Serializable]
    public class Bee:Animals
    {
        const double HoneyConsumed = 0.25; //сколько меда потребляет пчела (за какой период?) ---> УМЕНЬШИЛ С 0.5
        const int CareerSpan = 1000; //продолжительность жизни/карьеры пчелы

        //если меньше, то цветок не годен для сбора? ---> имеет смысл const в Flower сделать паблик
        //зачем тогда во Flower при сборе нектара проверяли: нектара не < 0.3 (столько может взять пчела)?
        const double MinimumFlowerNectare = 1.5;

        public int Age { get; private set; }
        public bool InsideHive { get; private set;} //пчела внутри улья флажок
        public double NectarCollected { get; private set;} //нектар, собранный конкретной пчелой
        int ID; //ГУИД пчелы
        public Flower destinationFlower;  //цель пчелы - цветок
        public Point controlDestination; //цель - точка клика мышки ---> для управления пчелой
        protected World_1 world;
        private Hive hive;
        public bool StingGoal { get; set;}
        public event StingDelegate BeeBeeSting; //событие укуса

        //2-ой вариант передачи сообщения на форму ---> не в кадрах,а сразу и навсегда
        public MessageToTheForm sendMessage; 

        public Bee(int id, Point location, World_1 world,Hive hive) {
            this.world = world;
            this.hive = hive;
            ID = id;
            this.location = location;
            Age = 0;
            InsideHive = true;
            destinationFlower = null;
            NectarCollected = 0;
            CurrentState = BeeState.Idle;
            StingGoal = false;
        }

        public void Go(Random random, MessageToTheForm messageToTheForm) {
            Age++;
            BeeState OldState = CurrentState; //переменная для хранения исходного состояния 
            switch (CurrentState){
                case BeeState.Idle:
                    if (Age >= CareerSpan)
                        CurrentState = BeeState.Retired;
                    else if (world.flowers.Count > 0 && hive.ConsumeHoney(HoneyConsumed)){
                        
                            Flower flower = world.flowers[random.Next(world.flowers.Count)];
                            if (flower.Nectar >= MinimumFlowerNectare){
                                  //пчела не полетит, если нектара <1.5
                                destinationFlower = flower;                 //1-определили, к какому цветку полетим
                                CurrentState = BeeState.FlyingToFlower;
                            }
                    };
                    break;
                case BeeState.FlyingToFlower:
                    if (!world.flowers.Contains(destinationFlower))
                    {
                        CurrentState = BeeState.ReturningToHive;
                    }
                    else if (InsideHive)
                    {
                        if (MoveTowardsLocation(hive.GetLocation("ExitLocation")))
                        {
                            InsideHive = false;
                            location = hive.GetLocation("EnterLocation"); //2-перемещаем пчелу за пределы улья, дальше она полетит к цветку
                        }
                    }
                    else if (MoveTowardsLocation(destinationFlower.Location))
                    {//3-летим к цветку
                        CurrentState = BeeState.GatheringNectar;
                    }
                    break;
                case BeeState.GatheringNectar:
                    double nectar = destinationFlower.HarvestNectar();
                    if (nectar > 0)
                        NectarCollected += nectar;
                    else
                    {
                        CurrentState = BeeState.ReturningToHive;
                    }
                    break;
                case BeeState.ReturningToHive:
                    if (!InsideHive)
                    {
                        if (MoveTowardsLocation(hive.GetLocation("EnterLocation"))){//летим обратно в улей
                        InsideHive = true;}
                    }
                    else if (MoveTowardsLocation(hive.GetLocation("FactoryLocation")))
                    {
                        CurrentState = BeeState.MakingHoney;
                    }
                    break;
                case BeeState.MakingHoney:
                    if (NectarCollected < 0.5)
                    {
                        NectarCollected = 0; //типа обнуляем
                        CurrentState = BeeState.Idle;
                    }
                    else
                        if (hive.AddHoney(0.5))
                        NectarCollected -= 0.5;
                    else
                        NectarCollected = 0; //если улей переполнен медом, пчела выбрасывает нектар
                    break;
                case BeeState.Retired:
                    //do nothing! we're retired!
                    break;
                case BeeState.Conrol:
                    if (MoveTowardsLocation(controlDestination)) { //контрол был null, поэтому пчела летела в 0,0
                        NectarCollected = 0;
                        //жалим упыря-- звук укуса
                        if (StingGoal && BeeBeeSting != null) //на событие должны быть подписаны, если нет, то будет null
                        {
                            BeeBeeSting(true);
                            CurrentState = BeeState.ReturningToHive;
                        }
                    }
                    break;
            }
            
            if (OldState != CurrentState && messageToTheForm != null)
                messageToTheForm(ID,CurrentState.ToString()); //ВЫЗОВ МЕТОДА ФОРМЫ
        }
    }
}
