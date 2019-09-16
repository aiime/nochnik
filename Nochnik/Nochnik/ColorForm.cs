using System;
using System.Windows.Forms;

namespace Nochnik
{
    public partial class ColorForm : Form
    {
        WallpaperClock wallpaperClock;
        public ColorForm(WallpaperClock wallpaperClock)
        {
            InitializeComponent();
            this.wallpaperClock = wallpaperClock;
            
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            wallpaperClock.ChangeClockColor(Int32.Parse(textBox1.Text), Int32.Parse(textBox2.Text), Int32.Parse(textBox3.Text));
        }
    }
}
