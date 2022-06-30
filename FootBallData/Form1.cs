using FootBallDataHelper;
using Newtonsoft.Json;
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
            //fBDHelper.OpenTimeFinishPage(textBox4.Text);
            DateTime dateTime = DateTime.ParseExact("20220627", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            int i = 0;
            do
            {
                dateTime = dateTime.AddDays(-1);
                var date = dateTime.ToString("yyyyMMdd");
                MessageBox.Show(date);
                fBDHelper.OpenTimeFinishPage(date);
                Thread.Sleep(5000);
            }
            while (i < 5);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            fBDHelper.HoverRows(int.Parse(textBox1.Text));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            fBDHelper.HoverRows(2);
            var right=fBDHelper.isOpenFloatingWindowRow(2);
            var left=fBDHelper.isOpenFloatingWindowRow(3);
            Debug.Assert(right);
            Debug.Assert(!left);
        }

        private void button4_Click(object sender, EventArgs e)
        {            
            var floating=fBDHelper.GetFloatingData();
            var json=JsonConvert.SerializeObject(floating);
            Console.WriteLine(json);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox2.Text = fBDHelper.GetDetailID(2).ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox3.Text=fBDHelper.GeteventName(2).ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                fBDHelper.StoreData();
            });
            thread.Start();
                /*
                string date = textBox4.Text;})


                int row=int.Parse(textBox1.Text);            
                var detailId=fBDHelper.GetDetailID(row);
                var eventname = fBDHelper.GeteventName(row).Item2;
                var eventid=fBDHelper.GeteventName(row).Item3;
                fBDHelper.HoverRows(row);
                Thread.Sleep(500);
                var floating = fBDHelper.GetFloatingData();
                fBDHelper.StoreData(date,eventname, eventid, detailId, floating.Item3);
                */
            }
    }
}
