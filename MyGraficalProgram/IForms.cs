using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGraficalProgram
{
    public interface IForms
    {
        event FieldMouseClick eFieldMouseClick;
        event FieldMouseClick eFieldMouseUp;
        event FieldMouseClick eFieldMouseMove;
    }
}
