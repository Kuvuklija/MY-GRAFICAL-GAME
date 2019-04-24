using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGraficalProgram
{
    public enum SubjectState {OnTheField,OnSubjectForm,Apply,OutSide}

    public class Subject
    {
        public Point Location { get; set; }   
        public SubjectState subState { get; set; }
        public  Bitmap pikcher {get; private set; }

        public Subject(Point loc, Bitmap refPikcher, SubjectState state)
        {
            Location = loc;
            subState = state;
            pikcher = refPikcher;
        }

        public void Go() //если предмет не подойдет, он должен вернуться на поле
        {
        }
    }
}
