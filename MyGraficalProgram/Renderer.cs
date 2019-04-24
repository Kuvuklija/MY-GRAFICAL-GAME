using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using NAudio.Wave;


namespace MyGraficalProgram
{
    public class Renderer
    {
        private IWords Iworld;
        private World_1 world;
        private FieldForm1 fieldForm;

        //GDI+
        Bitmap[] BeeAnimationLarge=new Bitmap[7];
        Bitmap[] BeeAnimationSmallRight=new Bitmap[7];
        Bitmap[] BeeAnimationSmallLeft = new Bitmap[7];
        Bitmap[] WervolfAnimation = new Bitmap[5];
        Bitmap[] HeroAnimation = new Bitmap[3];
        Bitmap[] fireAnimation = new Bitmap[3];

        bool drawDrags = false;
        public int verwolfWight; 
        public int verwolfHeight;

        //GDI+
        Bitmap HiveOutside;
        Bitmap Flower;      //цветов тоже нет
        Hero hero;
        bool gameOver;
        Font font;
        SizeF sizeWord;
        Bitmap ring;
        bool drag;
        int offsetX = 0;
        int offsetY = 0;
        Random random=new Random();
        const int rangeCuption = 10; //область захвата пчелы
        int rate;

        //!---словари для установки соответствия: объект кода----> картинка НЕ ИСПОЛЬЗУЕМ !!!!
        //private Dictionary<Flower, PictureBox> flowerLookup = new Dictionary<Flower, PictureBox>(); //цветы мы не рисуем, но пусть будет код пока
        //private Dictionary<Bee, BeeControl> beeLookup = new Dictionary<Bee, BeeControl>();

        //private List<Bee> retiredBees=new List<Bee>(); //список уставших пчел
        //private List<Flower> deadFlowers = new List<Flower>(); //список увядших цветков
        //BeeControl beeControl;

        private Dictionary<Subject, FlowersUser> subjectLookupWorld = new Dictionary<Subject, FlowersUser>();
        private Dictionary<Subject, FlowersUser> subjectPanelGame;

        //УВЕЛИЧИВАЕМ ПРОИЗВОДИТЕЛЬНОСТЬ КАРТИНОК
        public static Bitmap ResizeImage(Bitmap picture, int wight, int height) {
            Bitmap resizedPicture = new Bitmap(wight,height);               //получаем пустой битмап (ХОЛСТ) с нужными размерами
            using (Graphics graphics = Graphics.FromImage(resizedPicture)) {//преобразуем объект битмап в объект графикс
                graphics.DrawImage(picture,0,0,wight,height);               //грузим картинку на холст, подгоняя ее размеры с помощью объекта графикс
            }
            return resizedPicture;                                          //возвращаем преобразованный битмап
        }

        public void Render(IWords Iworld, IForms IfieldForm) { //мир и формы должны через интерфейсную ссылку передаваться
            this.Iworld = Iworld;                             //или перегружать класс будем(хуже)
            if (IfieldForm is FieldForm1)
                 fieldForm = IfieldForm as FieldForm1;
            
            //РАЗВИЛКА!!!!!!
            if (Iworld is World_1) {

                world = Iworld as World_1;

                verwolfHeight = world.verwolfHeight;
                verwolfWight = world.verwolfWight;
                subjectPanelGame = world.subjectsPanel;
                hero = world.hero;
                fieldForm.renderer = this;

                //ПОДПИСКИ НА СОБЫТИЯ ФОРМЫ---> ****вервольфа нужно обобщать через интерфейс или абстрактный класс
                //this.fieldForm.eTruck += new FieldForm.Truck(DrawTrucks);
                fieldForm.eFieldMouseClick += new FieldMouseClick(DrawCaptureObjects);

                //ПОДПИСКА ЧЕРЕЗ АНОНИМНЫЙ МЕТОД--->снимаем управление с Hero
                fieldForm.eFieldMouseUp += delegate(object sender, MouseEventArgs e){
                    if (hero.CurrentState == stateHero.Control &&
                         Math.Abs(hero.Location.X - e.Location.X) < hero.rangeHero &&
                           Math.Abs(hero.Location.Y- e.Location.Y) < hero.rangeHero)
                    {
                        if (e.Button == MouseButtons.Right)
                            hero.CurrentState = stateHero.Stay;
                    }
                };

                //ПОДПИСКА ЧЕРЕЗ ЛЯМБДА
                fieldForm.eFieldMouseMove += (sender, e) =>
                {
                    Point verwolfBeginLocation = world.verfolfBeginLocation;
                    int rateX = verwolfBeginLocation.X + world.verwolfWight;
                    int rateY = verwolfBeginLocation.Y + world.verwolfHeight;
                    if (e.Location.X > verwolfBeginLocation.X && e.Location.X < rateX
                       && e.Location.Y > verwolfBeginLocation.Y && e.Location.Y < rateY
                       && world.verwolf.CurrentState == WolfState.Stay)
                        DrawTrucks(true);
                    else
                        DrawTrucks(false);
                };
            }

            InitializeImages();
            
            hero = world.hero;
        }

        private void InitializeImages() {

            //чтобы отработать превращение мыши в руку, нужен пользовательский элемент на клумбе
            Flower = ResizeImage(Properties.Resources.клумба_new, 100, 75);
            HiveOutside = ResizeImage(Properties.Resources.Моя_игра_Картинка__main_, fieldForm.ClientRectangle.Width,fieldForm.ClientRectangle.Height);
            ring = ResizeImage(Properties.Resources.кольцо,100,90);

            BeeAnimationSmallRight[0] = ResizeImage(Properties.Resources.Bee_animation_1_new, 20, 20);
            BeeAnimationSmallRight[1] = ResizeImage(Properties.Resources.Bee_animation_2_new, 20, 20);
            BeeAnimationSmallRight[2] = ResizeImage(Properties.Resources.Bee_animation_3_new, 20, 20);
            BeeAnimationSmallRight[3] = ResizeImage(Properties.Resources.Bee_animation_4_new, 20, 20);
            BeeAnimationSmallRight[4] = ResizeImage(Properties.Resources.Bee_animation_3_new, 20, 20);
            BeeAnimationSmallRight[5] = ResizeImage(Properties.Resources.Bee_animation_2_new, 20, 20);
            BeeAnimationSmallRight[6] = ResizeImage(Properties.Resources.Bee_animation_1_new, 20, 20);

            BeeAnimationSmallLeft[0] = ResizeImage(Properties.Resources.Bee_animation_5_new, 20, 20);
            BeeAnimationSmallLeft[1] = ResizeImage(Properties.Resources.Bee_animation_6_new, 20, 20);
            BeeAnimationSmallLeft[2] = ResizeImage(Properties.Resources.Bee_animation_7_new, 20, 20);
            BeeAnimationSmallLeft[3] = ResizeImage(Properties.Resources.Bee_animation_8_new, 20, 20);
            BeeAnimationSmallLeft[4] = ResizeImage(Properties.Resources.Bee_animation_7_new, 20, 20);
            BeeAnimationSmallLeft[5] = ResizeImage(Properties.Resources.Bee_animation_6_new, 20, 20);
            BeeAnimationSmallLeft[6] = ResizeImage(Properties.Resources.Bee_animation_5_new, 20, 20);

            WervolfAnimation[0]=  ResizeImage(Properties.Resources.оборотень_1, verwolfWight, verwolfHeight);
            WervolfAnimation[1] = ResizeImage(Properties.Resources.оборотень_2, verwolfWight, verwolfHeight);
            WervolfAnimation[2] = ResizeImage(Properties.Resources.оборотень_3, verwolfWight, verwolfHeight);
            WervolfAnimation[3] = ResizeImage(Properties.Resources.оборотень_2, verwolfWight, verwolfHeight);
            WervolfAnimation[4] = ResizeImage(Properties.Resources.оборотень_1, verwolfWight, verwolfHeight);

            HeroAnimation[0] = ResizeImage(Properties.Resources.Герой_1, 70, 110);
            HeroAnimation[1] = ResizeImage(Properties.Resources.Герой_2, 70, 110);
            HeroAnimation[2] = ResizeImage(Properties.Resources.Герой_3, 70, 110);

            fireAnimation[0] = ResizeImage(Properties.Resources.пламя_1, 40, 62);
            fireAnimation[1] = ResizeImage(Properties.Resources.пламя_2, 40, 62);
            fireAnimation[2] = ResizeImage(Properties.Resources.пламя_3, 40, 62);
        }
       
        //анимация пчел
        private int cell = 0;
        private int frame = 0;
        /*вызывается из таймера основной формы и меняет значение переменной cell,
        которая используется в рисовании*/
        public void AnimateBees() //тоже через интерфейс можно!!!
        {
            if (frame >= 6) frame = 0;
            switch (frame)
            {
                case 0: cell = 0; break;
                case 1: cell = 1; break;
                case 2: cell = 2; break;
                case 3: cell = 3; break;
                case 4: cell = 4; break;
                case 5: cell = 5; break;
                case 6: cell = 6; break;
                default: cell = 0; break;
            }
            frame++;
            fieldForm.Invalidate();
        }

         int frame_f=0;
         int cell_f=0;
        
        //анимация огня
        public void AnimateFire()
        {
            switch (frame_f)
            {
                case 0: cell_f = 0; break;
                case 1: cell_f = 1; break;
                case 2: cell_f = 2; break;
                case 3: cell_f = 1; break;
                default: cell_f = 0; frame_f = 0; break;
            }
            frame_f++;
            fieldForm.Invalidate();
        }
        
        //анимация разбойника
        private int cell_w = 0;
        private int frame_w = 100;
        public void AnimateWolf()
        {
            if (frame_w >= 150) frame_w = 0; //делаем паузу на 150=250-100
            switch (frame_w)
            {
                case 0: cell_w = 0; break;
                case 1: cell_w = 1; break;
                case 2: cell_w = 2; break;
                case 3: cell_w = 3; break;
                case 4: cell_w = 4; break;
                default: cell_w = 0; break;
            }
            frame_w++;
            fieldForm.Invalidate();
        }

        //анимация героя
        private int cell_h = 0;
        private int frame_h = 0;
        public void HeroAnimate(stateHero state) {
            if (state == stateHero.Dead)
            {
                cell_h = 0;
                return;
            }

            switch (frame_h)
            {
                case 0: cell_h = 0; break;
                case 1: cell_h = 1; break;
                case 2: cell_h = 2; break;
                case 3: cell_h = 1; break;
                default: cell_h = 0; frame_h = 0; break;
            }
            frame_h++;
            fieldForm.Invalidate();
        }

        //РИСУЕМ ПОЛЯ
        public void PaintField(Graphics g)
        {
            g.DrawImageUnscaled(HiveOutside, 0, 0); //рисуем фон поля
            g.DrawImageUnscaled(Flower, 490, 500);  //рисуем клумбу--->  используем юзер контрол для отработки мыши
            g.DrawImageUnscaled(WervolfAnimation[cell_w], world.verwolf.Location.X, world.verwolf.Location.Y);
            if (hero.CurrentState == stateHero.Control || hero.CurrentState == stateHero.Move || hero.CurrentState == stateHero.Dead) //кольцо у ног
                g.DrawImageUnscaled(ring, hero.Location.X - 15, hero.Location.Y + 57);
            if (hero.CurrentState == stateHero.Stay)
                g.DrawImageUnscaled(HeroAnimation[0], world.heroBeginLocation.X, world.heroBeginLocation.Y);
            else if (hero.CurrentState == stateHero.Move || hero.CurrentState == stateHero.Dead)
                g.DrawImageUnscaled(HeroAnimation[cell_h], hero.Location.X, hero.Location.Y);
            else if (hero.CurrentState == stateHero.Control)
                g.DrawImageUnscaled(HeroAnimation[0], hero.Location.X, hero.Location.Y);
            
            g.DrawImageUnscaled(fireAnimation[cell_f],world.fireLocation.X,world.fireLocation.Y);

            //рисуем предметы
            DrawSubjectsOnTheForms(g);

            //рисуем пчел
            foreach (Bee bee in world.bees) {
                if (!bee.InsideHive)
                {
                    if (bee.CurrentState == BeeState.FlyingToFlower || bee.CurrentState == BeeState.GatheringNectar
                        || bee.CurrentState == BeeState.Conrol)
                        g.DrawImageUnscaled(BeeAnimationSmallRight[cell], bee.Location.X, bee.Location.Y);
               
                else if(bee.CurrentState == BeeState.ReturningToHive)
                    g.DrawImageUnscaled(BeeAnimationSmallLeft[cell], bee.Location.X, bee.Location.Y);
                }
            }

            //рисуем колесики
            if (drawDrags)
                g.DrawImageUnscaled(Properties.Resources.шестеренки,world.verfolfBeginLocation.X,world.verfolfBeginLocation.Y);

            //если игра закончилась, рисуем кнопочки и Game Over
            if (gameOver == true)
            {
                using (font = new Font("Arial", 64, FontStyle.Italic))
                {
                    string Over = "GAME OVER";
                    sizeWord = g.MeasureString(Over, font);
                    g.DrawString(Over, font, Brushes.Red, new Point(
                            fieldForm.Width / 2 - (int)sizeWord.Width / 2, fieldForm.Height / 2 - (int)sizeWord.Height / 2));
                }
            }
        }

        public void PaintHive(Graphics g) {
        }
        
        //рисуем панель с предметами
        void DrawSubjectsOnTheForms(Graphics g) {
            
            Pen pen = new Pen(Color.Gainsboro, 5);
            g.DrawRectangle(pen, 914, 10, 50, 50);
            g.DrawRectangle(pen, 914, 60, 50, 50);
            g.DrawRectangle(pen, 914, 110, 50, 50);
            g.DrawRectangle(pen, 914, 160, 50, 50);
            g.DrawRectangle(pen, 914, 210, 50, 50);
            g.DrawRectangle(pen, 914, 260, 50, 50);

            //рисуем предметы на игровом поле
            foreach (Subject subj in world.subjectsWorld)
            {
                FlowersUser stick;
                if (!subjectLookupWorld.ContainsKey(subj) && !subjectPanelGame.ContainsKey(subj))
                {
                    //добавляем предмет 
                    stick = new FlowersUser(subj.pikcher);
                    fieldForm.Controls.Add(stick);
                    //ПОДПИСКИ НА СОБЫТИЯ ПРОГРАММНО СОЗДАННОГО ЭЛЕМЕНТА-- НА ФОРМЕ ЕГО НЕТУ!
                    stick.Click += Stick_Click; //подписываемся на клик по ветке
                    stick.MouseDown += Stick_MouseDown;
                    stick.MouseUp += Stick_MouseUp;
                    stick.MouseMove += Stick_MouseMove;
                    subjectLookupWorld.Add(subj, stick);
                    stick.Location = subj.Location;
                }
            }

            //рисуем предметы на панели предметов
            foreach (Subject subj in subjectPanelGame.Keys)
            {
                if (subj.subState == SubjectState.OnSubjectForm)
                    subjectPanelGame[subj].Location = subj.Location;
            }
        }

        //обработка событий колесики - возводим флажок
        void DrawTrucks(bool truck) {
            drawDrags = truck;
        }
        //клик на форме/перехватываем пчелу/жалим разбойника/перехватываем героя
        void DrawCaptureObjects(object sender, MouseEventArgs e) {

            //перехватываем пчелу
            foreach (Bee bee in world.bees)
            {
                if (!bee.InsideHive && bee.CurrentState == BeeState.Conrol)
                {
                    bee.controlDestination = e.Location;

                    //жалим разбойника
                    Point verwolfBeginLocation = world.verfolfBeginLocation;
                    int rateX = verwolfBeginLocation.X + world.verwolfWight;
                    int rateY = verwolfBeginLocation.Y + world.verwolfHeight;

                    if (e.Location.X > verwolfBeginLocation.X && e.Location.X < rateX
                       && e.Location.Y > verwolfBeginLocation.Y && e.Location.Y < rateY)
                    {
                        bee.StingGoal = true;//ЖАЛИМ ---> CТАВИМ ФЛАГ!!!!
                    }
                }
                if (!bee.InsideHive && Math.Abs(bee.Location.X - e.Location.X) < rangeCuption
                    && Math.Abs(bee.Location.Y - e.Location.Y) < rangeCuption
                    && world.verwolf.CurrentState == WolfState.Stay)
                {
                    //если несколько пчел захватили, раскидываем их
                    rate = random.Next(30);
                    bee.Location = new Point(e.Location.X + rate, e.Location.Y + rate);
                    bee.controlDestination = bee.Location; //тут был косяк -пчела в контроле, но назначение неопределено и она летела в угол
                    bee.CurrentState = BeeState.Conrol;
                }
            }
            //перехватываем героя
            if (hero.CurrentState == stateHero.Stay && Math.Abs(hero.Location.X - e.Location.X) < hero.rangeHero && Math.Abs(hero.Location.Y - e.Location.Y) < hero.rangeHero)
            {
                //если была захвачена пчела, то отправляем ее в улей
                foreach (Bee bee_ in world.bees)
                {
                    if (!bee_.InsideHive && bee_.CurrentState == BeeState.Conrol)
                    {
                        bee_.CurrentState = BeeState.ReturningToHive;
                    }
                }
                hero.CurrentState = stateHero.Control;
            }
            else if (hero.CurrentState == stateHero.Control && e.Button == MouseButtons.Left)
            {
                hero.CurrentState = stateHero.Move; //если перехватили, двигаем за кликом мыши
                hero.currentDestination = new Point(e.Location.X - 30, e.Location.Y - 100);//поправка на размер героя
            } 
        }

        //перехват героя, чтобы снять с него управление-- правая кнопка мыши
        void DrawFredomHero(MouseEventArgs e) {
            
            if (hero.CurrentState == stateHero.Control &&
                 Math.Abs(hero.Location.X - e.Location.X) < hero.rangeHero &&
                   Math.Abs(hero.Location.Y - e.Location.Y) < hero.rangeHero)
            {
                if (e.Button == MouseButtons.Right)
                    hero.CurrentState = stateHero.Stay;
            }
        }

        public void GameOver() {
            gameOver = true;
        }
        //отправляем предмет с поля на панель
        private void Stick_Click(object sender, EventArgs e)
        {
            foreach (Point location in world.fillPanel.Keys)
            {
                FlowersUser thing = sender as FlowersUser;//приводим к типу
                if (world.fillPanel[location] == false)
                {
                    foreach (Subject str in world.subjectsWorld)
                    {
                        if (subjectLookupWorld.ContainsKey(str))
                        {
                            Subject subj = str;
                            subj.subState = SubjectState.OnSubjectForm;
                            subj.Location = location;     //устанавливаем для предмета-объекта координаты ячейки
                            subjectPanelGame.Add(subj, thing);//добавляем в панель
                            subjectLookupWorld.Remove(subj);  
                        }
                    }
                }
            }
        }

        //зажимаем предмет на панели
        private void Stick_MouseDown(object sender, MouseEventArgs e)
        {
            FlowersUser pikch = sender as FlowersUser;
            foreach (var strSub in subjectPanelGame) {//проверяем - предмет на панели?
                if (pikch == strSub.Value)
                {
                    drag = true;
                    offsetX = e.Location.X;
                    offsetY = e.Location.Y;
                    strSub.Key.subState = SubjectState.Apply;
                }
            }
        }

        //возвращаем неиспользованный предмет на панель
        private void Stick_MouseUp(object sender, MouseEventArgs e)
        {
            FlowersUser pikch = sender as FlowersUser;
            //здесь проверяем условие -- подходит ли предмет? если да, то в subjectPanelGame его удаляем и отовсюду
            drag = false;
            Subject subj;
            foreach (var strSub in subjectPanelGame) {//на панели визуально его нет, но пока считаем, что есть
                if (pikch == strSub.Value)
                {
                    subj = strSub.Key;
                    subj.subState = SubjectState.OnSubjectForm;
                }
            }
            foreach (Point location in world.fillPanel.Keys) {
                if (world.fillPanel[location] == false){
                    pikch.Location = location;
                }
            }
        }

        //мышь превращаем в руку, перемещение предмета
        private void Stick_MouseMove(object sender, MouseEventArgs e)
        {
            FlowersUser subj = sender as FlowersUser;
            //мышь превращаем в руку
            var img = new Bitmap(Properties.Resources.рука);
            Icon icon = Icon.FromHandle(img.GetHicon());
            Cursor cur = new Cursor(icon.Handle);
            Cursor.Current = cur;

            //перемещение предмета
            if (drag)
            {
                int x = Cursor.Position.X - (fieldForm.Left + (fieldForm.Size.Width - fieldForm.ClientSize.Width) / 2) - offsetX;
                int y = Cursor.Position.Y - (fieldForm.Top + (fieldForm.Size.Height - fieldForm.ClientSize.Height - 4)) - offsetY;
                if (x > 0 && x < fieldForm.ClientSize.Width - subj.Width)
                    subj.Left = x;
                else
                    subj.Left = x > 0 ? x = fieldForm.ClientSize.Width - subj.Width : 0;
                if (y > 0 && y < fieldForm.ClientSize.Height - subj.Height)
                    subj.Top = y;
                else
                    subj.Top = y > 0 ? y = fieldForm.ClientSize.Height - subj.Height : 0;
            }
        }

        //public void Render() { //НЕ ИСПОЛЬЗУЕМ !!!!!
        //    DrawBees();
        //    //DrawFlowers(); //цветы не рисуем
        //    RemoveRetiedBeesAndDeadFlowers();
        //}

        //перезагрузка
        //public void Reset() { //пустой метод
        //    //стираем цветы
        //    foreach (PictureBox flower in flowerLookup.Values) {
        //        fieldForm.Controls.Remove(flower);
        //        flower.Dispose();
        //    }
        //    //стираем пчел
        //    foreach (BeeControl bee in beeLookup.Values) {
        //        hiveForm.Controls.Remove(bee);
        //        fieldForm.Controls.Remove(bee);
        //        bee.Dispose();
        //    }
        //    flowerLookup.Clear();
        //    beeLookup.Clear();
        //}

        //РИСУЕМ ПЧЕЛУ
        //private void DrawBees(){
        //    //если картинка пчелы есть, а в коде ее уже нет, удаляем картинку с форм
        //    foreach (Bee bee in beeLookup.Keys) {
        //        if (!world.bees.Contains(bee)) {
        //            beeControl = beeLookup[bee];
        //            if (fieldForm.Contains(beeControl))
        //                fieldForm.Controls.Remove(beeControl);
        //            else if (hiveForm.Contains(beeControl))
        //                hiveForm.Controls.Remove(beeControl);
        //            beeControl.Dispose();
        //            retiredBees.Add(bee);
        //        }
        //    }
        //    //находим или создаем картинку пчелы/перемещаем картинку в улей или в поле
        //    foreach (Bee bee in world.bees) {
        //        beeControl = GetBeeControl(bee); //нужно получить картинку пчелы, чтобы после ее перемещать. С цветами это не нужно.

        //        //ТУТ!!! КОГДА ОТКРЫТО ПОЛЕ, НАМ НЕ НУЖНО ПОКАЗЫВАТЬ ПЧЕЛ В УЛЬЕ!!!! КАК СДЕЛАТЬ????????????????????????
        //        hiveForm.Controls.Add(beeControl);
        //        beeControl.BringToFront();

        //        if (bee.InsideHive)
        //        {
        //            if (fieldForm.Controls.Contains(beeControl)){
        //                MoveBeeFromFieldToHive(beeControl);
        //            }
        //        }
        //        else if (hiveForm.Controls.Contains(beeControl))
        //            MoveBeeFromHiveToField(beeControl);
        //        beeControl.Location = bee.Location; //!-----> ПРИВЯЗЫВАЕМ КООРДИНАТЫ КАРТИНКИ К КООРДИНАТАМ ПЧЕЛЫ ИЗ КОДА
        //    }
        //}

        //РИСУЕМ ЦВЕТЫ (НЕ ИСПОЛЬЗУЕТСЯ, ТОЛЬКО ДЛЯ ПРИМЕРА)
        //private void DrawFlowers() {
        //    //создаем цветок
        //    foreach (Flower flower in world.flowers) {
        //        if (!flowerLookup.ContainsKey(flower)) {
        //            PictureBox flowerControl = new PictureBox()
        //            {
        //                Width = 45,
        //                Height = 55,
        //                Image = Properties.Resources.Flower,
        //                SizeMode = PictureBoxSizeMode.StretchImage,
        //                Location = flower.Location //КООРДИНАТЫ КАРТИНКИ ЦВЕТКА ПРИВЯЗЫВАЮТСЯ К КОДУ ЦВЕТКА
        //            };
        //            flowerLookup.Add(flower,flowerControl);
        //            fieldForm.Controls.Add(flowerControl);
        //        }
        //    }
        //    //удаляем цветок
        //    foreach (Flower flower in flowerLookup.Keys) { //по ключам идем
        //        if (!world.flowers.Contains(flower)) {
        //            PictureBox flowerControl = flowerLookup[flower];
        //            fieldForm.Controls.Remove(flowerControl);
        //            flowerControl.Dispose();
        //            deadFlowers.Add(flower);
        //        }
        //    }
        //}

        //чистим лукапы, хотя можно было в процедурах удаления контролов это сделать
        //private void RemoveRetiedBeesAndDeadFlowers() {
        //    foreach (Bee bee in retiredBees)
        //        beeLookup.Remove(bee);
        //    retiredBees.Clear();

        //    foreach (Flower flower in deadFlowers)
        //        flowerLookup.Remove(flower);
        //    deadFlowers.Clear();
        //}

        //private BeeControl GetBeeControl(Bee bee) {
        //    //создаем картинку пчелы
        //    if (!beeLookup.ContainsKey(bee))
        //    {
        //        beeControl = new BeeControl() { Width = 40, Height = 40 };
        //        beeLookup.Add(bee, beeControl);
        //       /* hiveForm.Controls.Add(beeControl);
        //        beeControl.BringToFront();*/
        //    }
        //    else  beeControl = beeLookup[bee];

        //        return beeControl;

        //}

        //private void MoveBeeFromFieldToHive(BeeControl beeControl) {
        //    fieldForm.Controls.Remove(beeControl);
        //    beeControl.Size = new System.Drawing.Size(40,40);
        //    hiveForm.Controls.Add(beeControl);
        //    beeControl.BringToFront();
        //}

        //private void MoveBeeFromHiveToField(BeeControl beeControl) {
        //    hiveForm.Controls.Remove(beeControl);
        //    beeControl.Size = new System.Drawing.Size(10,10);
        //    fieldForm.Controls.Add(beeControl);
        //    beeControl.BringToFront();
        //}

        //void GetBeeLocation(Bee bee) {
        //    beeControl = beeLookup[bee];
        //    beeControl.Location = bee.Location; //координаты пчелы из кода присваиваем координатам изображения
        //}

        //BeeControl CteateBeeControl(Bee bee) {
        //    if (!beeLookup.ContainsKey(bee))
        //    {
        //        beeControl = new BeeControl() { Width = 40, Height = 40 };
        //        beeLookup.Add(bee, beeControl);
        //        hiveForm.Controls.Add(beeControl);
        //        beeControl.BringToFront(); //помещаем картинку пчелы поверх фона
        //    }
        //    else
        //        beeControl = beeLookup[bee];

        //    return beeControl;
        //}

    }
}
