using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace MyGraficalProgram
{
    public delegate void FieldMouseClick(object sender, MouseEventArgs e);//***лучше перенести в визуализатор

    public partial class FieldForm1 : Form,IForms
    {
        public event FieldMouseClick eFieldMouseClick;
        public event FieldMouseClick eFieldMouseUp;
        public event FieldMouseClick eFieldMouseMove;
        public WaveOut waveOut;
        public Renderer renderer { get; set; }

        public FieldForm1()
        {
            InitializeComponent();
            PlayMusic();
        }

        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            PlayMusic();
        }

        private void FieldForm_MouseClick(object sender, MouseEventArgs e)
        {
            //MessageBox.Show(e.Location.ToString()); //выведем координаты улья
            eFieldMouseClick(sender,e); //в рендерер
        }

        private void FieldForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            waveOut.Stop();
            waveOut.Dispose();
        }

        //ПЕРЕРИСОВЫВАЕМ ПОЛЕ
        private void FieldForm_Paint(object sender, PaintEventArgs e)
        {
            renderer.PaintField(e.Graphics);
        }

        //колесики возле вервольфа 
        private void FieldForm_MouseMove(object sender, MouseEventArgs e)
        {
            eFieldMouseMove(sender,e);
        }

        //поют птички
        void PlayMusic()
        {
            //музыку фоном запускаем mp3 ---для этого через NuGet загрузили пакет NAudio.dll(в Сервис)
            //WaveStream reader = new Mp3FileReader(@"C:\РУСТ\С#\Resurce_Images\Resurce_Images\Пение птиц.mp3");
            WaveStream reader = new Mp3FileReader(@"G:\C#\Repite Stilman And My Grafic\Resurce_Images\Пение птиц.mp3");
            waveOut = new WaveOut();
            waveOut.Init(reader);
            waveOut.Play();
            waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
        }

         //если при отжатии клавиши это правая кнопка, то СНИМАЕМ УПРАВЛЕНИЕ С ГЕРОЯ
         //при клике не отработает????
        private void FieldForm_MouseUp(object sender, MouseEventArgs e)
        {
            eFieldMouseUp(sender,e); 
        }
    }
}
