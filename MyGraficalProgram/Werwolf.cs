using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NAudio.Wave;

namespace MyGraficalProgram
{
    public enum WolfState {Stay,Attack,GoAway}

    public class Werwolf:Animals
    {
        Hero hero;
        public new WolfState CurrentState; //скрываем состояние, иначе наследуем состояния bee
        WaveOut waveOut;
        World_1 world;

        public Werwolf(World_1 world):base()
        {
            this.hero = world.hero;
            Location = world.verfolfBeginLocation;
            CurrentState = WolfState.Stay;
            this.world = world;
            //подписываемся на укус пчел
            foreach (Bee bee in world.bees)
                bee.BeeBeeSting += Werwolf_BeeSting; 
        }

        int countSting = 1; //счетчик укусов, т.к. две-три могут укусить, музыка не может запускаться два раза
        void Werwolf_BeeSting(bool sting) {
            CurrentState = WolfState.GoAway; //у CurrentState вольфа надо было new, иначе из bee брал
            if (countSting == 1)
                PlayMusicSting();
            countSting++;
        }

        public void Go()
        {
            if (hero.Location != world.heroBeginLocation 
                 && CurrentState!=WolfState.GoAway) { //если герой сдвинулся-атакуем тут еще условия, когда оборотня укусят и он отойдет в сторону
                currentDestination = hero.Location;
                CurrentState = WolfState.Attack;
                if (MoveTowardsLocation(hero.Location) == true) {//перемещаться к герою будем на MoveRate=3 из базового класса
                    //остановка игры, сообщение игроку
                     world.pipec("Bandit killed you!"); //обращаемся к методу симулятора
                } 
            }
            //если ужалили
            if (CurrentState==WolfState.GoAway) {
                currentDestination = new Point(500,387);
                if (MoveTowardsLocation(currentDestination) == true)
                {
                    //всплывающая подсказка I'll be back! через событие передавать на поле
                }
            }
        }

        void PlayMusicSting()
        {
            //музыку фоном запускаем mp3 ---для этого через NuGet загрузили пакет NAudio.dll(в Сервис)
            WaveStream reader = new Mp3FileReader(@"G:\C#\Repite Stilman And My Grafic\Resurce_Images\Крик разбойника.mp3");
            //WaveStream reader = new Mp3FileReader(@"C:\РУСТ\С#\Resurce_Images\Resurce_Images\Крик разбойника.mp3");
            waveOut = new WaveOut();
            waveOut.Init(reader);
            waveOut.Play();
            waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
        }

        void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            waveOut.Dispose();
        }

        void Attack() {
        }
    }
}
