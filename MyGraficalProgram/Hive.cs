using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGraficalProgram
{
    [Serializable]
    public class Hive:IWords
    {
        public double Honey{ get; private set;}
        int BeeID = 0;
        Dictionary<string, Point> locations;
        World_1 World;

        const int InitialBees= 8;
        const double InitialHoney = 3.2;
        const double MaximumHoney = 15;
        const double NectarHoneyRatio = 0.25; //коэффициент переработки нектара в мед
        const int MaximumBees = 10;
        const double MinimumHoneyForCreatingBees = 4; //мин кол-во меда для появления новых пчел

        public Renderer renderer { get; set;}
        public Dictionary<Subject, FlowersUser> subjectsPanel { get; set; }
        public Point Enter { get; set; }
        public Point Exit { get; set; }

        public Hive(IWords world) {//вторым параметром могли передать делегат sendMessage
            World = world as World_1; ;
            Honey = InitialHoney;
            InitializeLocation();
            Random random = new Random();
            for (int i = 0; i < InitialBees; i++) {
                AddBee(random);
            }
        }

        void InitializeLocation()
        {
            locations = new Dictionary<string, Point>();
            locations.Add("EnterLocation", new Point(345, 357));
            locations.Add("NuseruLocation", new Point(364, 288));
            locations.Add("FactoryLocation", new Point(420, 322));
            locations.Add("ExitLocation", new Point(129, 417));
        }

        public bool AddHoney(double Nectar) {
            double addHoney = Nectar * NectarHoneyRatio;
            if (Honey + addHoney > MaximumHoney)
                return false;
            else{
                Honey += addHoney;
                return true;
            }
        }

        public bool ConsumeHoney(double amount) {
            if (amount > Honey)
                return false;
            else Honey -= amount;
                return true; }

        void AddBee(Random random)
        {
            if (World.bees.Count >= MaximumBees)
                return;
            int rate = random.Next(50);
            Point NuseruLocation = GetLocation("NuseruLocation");
            Point BeeLocation = new Point(NuseruLocation.X + rate, NuseruLocation.Y + rate);
            Bee bee = new Bee(BeeID, BeeLocation,World,this);
            //bee.sendMessage += this.sendMessage; так могли бы подписать каждую пчелу на метод формы
            BeeID++;
            World.bees.Add(bee); //добавляем пчелу в список World                           
        }

        public void Go(Random random, MessageToTheForm message) {
            if (World.bees.Count<=MaximumBees 
                && Honey > MinimumHoneyForCreatingBees 
                && random.Next(10)==1) //пчела рождается в 1 из 10 случаев
                AddBee(random);
        }

        public Point GetLocation(string location) {
            if (locations.Keys.Contains(location))
                return locations[location];
                else
                    throw new ArgumentException("Not found bee location in hive: "+location);
            }
        }
    }

