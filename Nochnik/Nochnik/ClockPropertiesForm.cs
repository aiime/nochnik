using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nochnik
{
    public partial class ClockPropertiesForm : Form
    {
        MainForm mainForm;
        public ClockPropertiesForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int x = Int32.Parse(textBox1.Text);
            int y = Int32.Parse(textBox2.Text);

            mainForm.wallpaperClock.ChangeClockPosition(x, y);
        }
    }
}
