using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using NAudio.Wave;

namespace MyGraficalProgram
{
    //public delegate void MessageToTheForm(int ID, string Message);
    //public delegate void GameOver(string why);
    
    public partial class MainForm : Form
    {
        SizeF sizeWord;
        SizeF sizeChar;
        char[] chars;
        ArrayList arr;
        int LenghArray;
        PictureBox Start; //кнопка старт
        PictureBox Portret; //фотка
        WaveOut waveOut1;

        public MainForm()
        {
            InitializeComponent();

            Portret = new PictureBox()
            {
                Image = Properties.Resources.Портрет_21,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = 100,
                Height = 110,
                Location = new Point(132, 178)
            };
            this.Controls.Add(Portret);

            PlayKlava();

            timer1.Interval = 100;

            //посимвольный вывод текста файла 
            arr = new ArrayList();
            //using (StreamReader sr = new StreamReader(@"C:\РУСТ\С#\Resurce_Images\Resurce_Images\TextForGame.txt", Encoding.Default))
            using (StreamReader sr = new StreamReader(@"G:\C#\Repite Stilman And My Grafic\Resurce_Images\TextForGame.txt", Encoding.Default))
            {
                string s = null;
                while ((s = sr.ReadLine()) != null)
                {
                    arr.Add(s);
                    //Console.OutputEncoding = Encoding.GetEncoding(866);
                    //Console.WriteLine(s);
                }
                LenghArray = arr.Count;
                if (LenghArray != 0)
                    timer1.Enabled = true; //запускаем таймер
            }
        }

        //НАЧАЛИ
        private void Start_MouseClick(object sender, MouseEventArgs e)
        {
            this.Hide();
            MainManager mainManager = new MainManager();
        }

        void PlayKlava()
        {
            //WaveStream reader = new Mp3FileReader(@"C:\РУСТ\С#\Resurce_Images\Resurce_Images\звук клавиш.mp3");
            WaveStream reader = new Mp3FileReader(@"G:\C#\Repite Stilman And My Grafic\Resurce_Images\звук клавиш.mp3");
            waveOut1 = new WaveOut();
            waveOut1.Init(reader);
            waveOut1.Play();
        }

        int i = 0; //итератор динамического массива
        int j = 0; //итератор внутри строки динамического массива
        int h = 0;     //высота отступа строк

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (i < LenghArray)
            {
                chars = arr[i].ToString().ToCharArray(); //объект в строку, а строку в массив
                int lenghtChars = chars.Length;  //длина строки--- когда дойдем до конца, меняем i
                if (j < lenghtChars)
                {
                    using (Graphics g = CreateGraphics())
                    {
                        using (Font font1 = new Font("Arial", 12, FontStyle.Regular))
                        {
                            sizeChar = g.MeasureString(chars[0].ToString(), font1);
                            g.DrawString(chars[j].ToString(), font1, Brushes.Red, new Point(
                                   227 + (int)(sizeChar.Width - 3) * (j + 1), 178 + (int)(sizeWord.Height + h * i)));

                            j++;
                        }
                    }
                }
                else
                {
                    i++;
                    j = 0;
                    h = 25;
                }
            }
            else
            {
                timer1.Stop();
                timer1.Dispose();
                waveOut1.Stop();
                waveOut1.Dispose();
                Start = new PictureBox()
                {
                    Image = Properties.Resources.Кнопка_СТАРТ,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 70,
                    Height = 70,
                    Location = new Point(450, 522),
                    BackColor = Color.Transparent
                };
                this.Controls.Add(Start);
                Start.MouseClick += Start_MouseClick;
            }
        }
    }
}

