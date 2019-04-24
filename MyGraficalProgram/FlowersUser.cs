using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGraficalProgram
{
    public partial class FlowersUser : UserControl //от Flowers будут наследовать другие предметы ИЛИ ЧЕРЕЗ ИНТЕРФЕЙС???
    {
        public FlowersUser(Bitmap pikch)
        {
            InitializeComponent();

            BackColor = System.Drawing.Color.Transparent;
            BackgroundImage = Renderer.ResizeImage(pikch, 50, 25);
            BackgroundImageLayout = ImageLayout.Stretch;
        }
    }
}
