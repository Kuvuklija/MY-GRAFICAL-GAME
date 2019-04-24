using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGraficalProgram
{
    public enum stateHero { Control, Move, Stay, Dead, ChangeWorld } //1- я щелкнул по нему, затем могу двигать

    public class Hero : Animals
    {
        Werwolf verwolf { get; set;}
        public new stateHero CurrentState { get; set; }
        IWords iword;
        public int rangeHero { get; set; }
        public event ExchangeWorld eExchangeWorld;

        public void GetCurrentWorld(IWords iword) {
            
            //РАЗДЕЛ НАСТРОЕК ГЕРОЯ ДЛЯ КАЖДОГО МИРА
            if (iword is World_1)
            {
                this.iword = iword;
                World_1 world = iword as World_1;
                Location = world.heroBeginLocation;
                verwolf = world.verwolf;
                CurrentState = stateHero.Stay;
                rangeHero = 50;
            }
        }

        public void Go()
        {
            if (CurrentState == stateHero.Move)
            {//встретился с вервольфом
                if (MoveTowardsLocation(currentDestination) == true)
                {

                    if (iword is World_1)
                    {
                        if (verwolf.CurrentState == WolfState.Attack)
                            CurrentState = stateHero.Dead;
                        else
                            CurrentState = stateHero.Control;
                    }
                }
            }
            else if (CurrentState == stateHero.Control)
            {
                //СМЕНА МИРА
                if (Math.Abs(Location.X - iword.Exit.X) < rangeHero && Math.Abs(Location.Y - iword.Exit.Y + 110) < rangeHero)
                {
                     eExchangeWorld(iword.Exit);
                }
            }
        }        
    }    
}    
