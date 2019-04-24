using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGraficalProgram
{
    public abstract class Animals
    {
        protected Point location;
        public Point Location { get { return location; } set { location = value; } }
        public BeeState CurrentState { get; set; }
        int MoveRate = 3;
        public Point currentDestination { get; set; }

        //смещаем существо по направлению к цели
        public bool MoveTowardsLocation(Point destination)
        {
            if (Math.Abs(location.X - destination.X) <= MoveRate && Math.Abs(location.Y - destination.Y) <= MoveRate)
                return true;

            if (destination.X > location.X)
                location.X += MoveRate;
            else
                location.X -= MoveRate;

            if (destination.Y > location.Y)
                location.Y += MoveRate;
            else location.Y -= MoveRate;

            return false;
        }
    }
}
