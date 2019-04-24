using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MyGraficalProgram
{
    [Serializable]
    public class World_1:BaseWorld,IWords //это описание карты field
    {
        public Hive hive;
        public List<Bee> bees { get; set;}           //*** тут надо поработать над доступом
        public List<Flower> flowers { get; set; }    //*** тут надо поработать над доступом
        double totalNectarHarvested = 0;
        public Werwolf verwolf;
        public Hero hero { get; set;}
        public Dictionary<Point, bool> fillPanel; //ветка должна знать, куда ей лететь
        Subject subject1;
        public Point Exit { get; set;} //координаты перехода на другую карту
        public Point Enter { get; set; }//координаты возврата на эту карту

        //координаты поляны с цветами
        const int MinX = 489;
        const int MaxX = 559;
        const int MinY = 500;
        const int MaxY = 550;

        public Point fireLocation = new Point(418,260);//!---> координаты входа в первый дом в классе Hive
        public Point verfolfBeginLocation = new Point(400,387);
        public int verwolfWight = 100; 
        public int verwolfHeight = 74;
        public Point heroBeginLocation = new Point(297,500);
        public GameOver pipec { get; set;}
        public Dictionary<Subject, FlowersUser> subjectsPanel { get; set;} //предметы в ячейках (они общие для всех карт)интерфейс нужен
        public List<Subject> subjectsWorld { get; set; }//предметы на поле текущей карты
        Bitmap picher1; //ссылка на картинку---> здесь лучше массив
        BaseWorld nextWorld { get; set;}
        public Renderer renderer { get; set;} 

        //РАЗДЕЛ ТАЙМЕРОВ
        Timer timer1 = new Timer();
        Timer timer2 = new Timer();
        Timer timer3 = new Timer();

        public World_1(Dictionary<Point,bool> fillPanel, GameOver pipec, Dictionary<Subject, FlowersUser> subjectsPanelMain, Hero hero)
        {
            bees = new List<Bee>();
            hive = new Hive(this); //тут создали 6 первых пчелок и они попали в List в Hive, который знает о World
            flowers = new List<Flower>();
            Random random = new Random();
            for (int i = 0; i < 10; i++)
                AddFlowers(random);
            this.hero = hero;
            verwolf = new Werwolf(this);
            this.fillPanel = fillPanel;
            this.pipec = pipec;
            subjectsWorld = new List<Subject>();
            picher1 = Properties.Resources.ветка;
            subject1 = new Subject(new Point(262, 491),picher1, SubjectState.OnTheField); //создаем объект - ветка
            subjectsWorld.Add(subject1);
            subjectsPanel = subjectsPanelMain;
            Exit = new Point(308,441);
            Enter = new Point(Exit.X+hero.rangeHero+10,Exit.Y+hero.rangeHero+10);

            //РАЗДЕЛ ТАЙМЕРОВ ЭТОГО МИРА

            //анимация крыльев
            timer1.Interval = 1;
            timer1.Tick += Timer1_Tick;
            timer1.Enabled = true;
            //анимация вервольфа
            timer2.Interval = 70;
            timer2.Tick += Timer2_Tick;
            timer2.Enabled = true;
            //анимация огня
            timer3.Interval = 200;
            timer3.Tick += Timer3_Tick;
            timer3.Enabled = true;
        }

        void AddFlowers(Random random)
        {
            Point locationFlower = new Point(random.Next(MinX, MaxX + 1), random.Next(MinY, MaxY + 1));
            Flower flower = new Flower(locationFlower, random);
            flowers.Add(flower);
        }

        public void Go(Random random, MessageToTheForm messageToTheForm) {

            Bee bee;
            for (int i = bees.Count - 1; i >= 0; i--)
            {
                bee = bees[i];
                bee.Go(random, messageToTheForm);
                if (bee.CurrentState == BeeState.Retired)
                    bees.Remove(bee);
            }

            Flower flower;
            for (int i = flowers.Count - 1; i >= 0; i--) {
                flower = flowers[i];
                flower.Go();
                totalNectarHarvested += flower.NectarHarvested;
                if (!flower.Alive)
                    flowers.Remove(flower);
            }
           // hero.Go(); //будем анимировать в других кадрах, которые формируются в главном классе
            verwolf.Go();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            renderer.AnimateBees();
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            renderer.AnimateWolf();
            if (verwolf.CurrentState == WolfState.GoAway)
            {
                timer2.Stop();
                timer2.Dispose();
            }
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            renderer.AnimateFire();
        }
    }
}
