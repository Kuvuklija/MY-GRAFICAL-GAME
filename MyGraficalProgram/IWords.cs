using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGraficalProgram
{
    public  interface IWords
    {
        Dictionary<Subject, FlowersUser> subjectsPanel { get; set; }
        void Go(Random random, MessageToTheForm messageToTheForm);
        Renderer renderer { get; set;}
        Point Exit { get; set;}
        Point Enter { get; set; }
    }
}
