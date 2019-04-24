using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MyGraficalProgram
{
    public delegate void MessageToTheForm(int ID, string Message);
    public delegate void GameOver(string why);
    public delegate void ExchangeWorld(Point Exit);

    class MainManager
    {
        private Renderer renderer;
        int framesRun = 0;
        Random random = new Random();
        DateTime start = DateTime.Now;
        DateTime end;
        private Dictionary<Point, IWords> exchangePortal = new Dictionary<Point,IWords>(); 
        private Dictionary<Point, bool> fillPanel = new Dictionary<Point, bool>();
        private Dictionary<Subject, FlowersUser> subjectsPanel = new Dictionary<Subject, FlowersUser>();
        Timer timer1 = new Timer();
        Timer timer2 = new Timer();

        IWords Iworld1;
        FieldForm1 fieldForm;
        IForms IForm1;

        IWords currentWorld;

        public Hero hero { get; set;}

        public MainManager()
        {
            //заполняем ячейки координатами 
            fillPanel.Add(new Point(915, 20), false);
            //fillPanel.Add(new Point(), false);
            //fillPanel.Add(new Point(), false);
            //fillPanel.Add(new Point(), false);
            //fillPanel.Add(new Point(), false);
            //fillPanel.Add(new Point(), false);}

            //кадры
            timer1.Interval = 50;
            timer1.Tick += new EventHandler(RunFrame); //обработка тика не в стандрартном обработчике, а в пользовательском
            timer1.Enabled = false;

            //анимация героя
            timer2.Interval = 150;
            timer2.Tick += Timer2_Tick;
            timer2.Enabled = false;

            ResetSimulator();
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
                hero.Go(); //анимируем героя с собственной частотой
                renderer.HeroAnimate(hero.CurrentState);
        }

        ////ИСХОДНАЯ ТОЧКА, ОТКУДА ЗАПУСКАЕТСЯ МИР И ВИЗУАЛИЗАТОР
        private void ResetSimulator()
        {
            framesRun = 0;
            renderer = new Renderer();
            GameOver pipec = new GameOver(YouLost);
            hero = new Hero(); //герой общий для всех миров
            hero.eExchangeWorld += new ExchangeWorld(CurrentExit);
            timer1.Enabled = true;
            timer2.Enabled = true;

            Iworld1 = new World_1(fillPanel, pipec, subjectsPanel,hero);
            World_1 w1 = Iworld1 as World_1;
            exchangePortal.Add(Iworld1.Exit, w1.hive);//т.к. у нас пчелы в улье создаются, а сначала мы мир открываем, то вот такие пляски
            currentWorld = Iworld1;
            hero.GetCurrentWorld(Iworld1); //герой узнает о мире
            Iworld1.renderer = renderer;
            IForm1 = new FieldForm1();

            StartWorld(Iworld1);
        }

        //меняем мир 
        private void CurrentExit(Point exit)
        {
        }

        //РАЗДЕЛ ВИЗУАЛИЗАЦИИ МИРОВ
        void StartWorld(IWords Iworld)
        {
            if (Iworld is World_1)
            {
                fieldForm = IForm1 as FieldForm1;
                fieldForm.Show();
                renderer.Render(Iworld, IForm1); //запуск мира №1
                                                //else if(Iworld is World2)
                                                //......................
                                                //......................}
            }
        }

        private void RunFrame(object sender, EventArgs e)
        {
            TimeSpan frameDuration = new TimeSpan();
            framesRun++; //пошел отсчет кадров
                         //передаем ссылку на метод формы (нужно довести его до пчелы)
            MessageToTheForm messageToTheForm = new MessageToTheForm(SendMessage);
            Iworld1.Go(random, messageToTheForm);
            //renderer.Render(); //начинаем рисовать пчел
            end = DateTime.Now;
            frameDuration = end - start;
            start = end;
            //GDI+ берем открытую форму и перерисовываем ее
            fieldForm.Invalidate();
        }

        //обрабатываем окончание игры
        private void YouLost(string why)
        {
            //передаю визуализатору, чтобы он вывел кнопочки на форме поля и надпись GAME OVER
            //останавливаю таймер героя
            renderer.GameOver();

            fieldForm.Enabled = false;

            timer1.Stop();
            timer1.Dispose();
            timer2.Stop();
            timer2.Dispose();
        }

        //строка состояния пчелы - вызывается обратным вызовом из класса Bee
        private void SendMessage(int ID, string Message)
        {
        }

    }
}
