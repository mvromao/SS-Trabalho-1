using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;


namespace SS_OpenCV
{
    public partial class Histogram : Form
    {
        public Histogram(int[,] array)
        {
            InitializeComponent();

            DataPointCollection list1 = chart1.Series[1].Points;
            DataPointCollection list2 = chart1.Series[2].Points;
            DataPointCollection list3 = chart1.Series[3].Points;

            for(int j = 0; j < array.GetLength(0); j++)
                for (int i = 0; i < array.GetLength(1); i++)
                { 
                    list1.AddXY(i, array[j,i]);
                }

            chart1.Series[0].Color = Color.Gray;
            chart1.ChartAreas[0].AxisX.Maximum = 255;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Title = "Intensidade";
            chart1.ChartAreas[0].AxisY.Title = "Numero Pixeis";
            chart1.ResumeLayout();
        }
        public Histogram(int[] array)
        {
            InitializeComponent();

            DataPointCollection list1 = chart1.Series[0].Points;
            for (int i = 0; i < array.Length; i++)
            {
                list1.AddXY(i, array[i]);
            }
            chart1.Series[0].Color = Color.Gray;
            chart1.ChartAreas[0].AxisX.Maximum = 255;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Title = "Intensidade";
            chart1.ChartAreas[0].AxisY.Title = "Numero Pixeis";
            chart1.ResumeLayout();
        }
    }
}
