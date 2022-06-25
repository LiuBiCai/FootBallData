using FootBallDataHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FootBallData
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        FBDHelper fBDHelper = new FBDHelper();
        private void button1_Click(object sender, EventArgs e)
        {            
            fBDHelper.OpenTimeFinishPage("20220623");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fBDHelper.HoverRows(2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            fBDHelper.HoverRows(2);
            var right=fBDHelper.isOpenFloatingWindowRow(2);
            var left=fBDHelper.isOpenFloatingWindowRow(3);
            Debug.Assert(right);
            Debug.Assert(!left);
        }
    }
}
