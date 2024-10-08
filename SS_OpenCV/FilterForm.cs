using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SS_OpenCV
{
    public partial class FilterForm : Form
    {
        public float[,] matrix = new float[3, 3];
        public float weight, offset;
        public FilterForm()
        {
            InitializeComponent();
        }

        private void FilterForm_Load(object sender, EventArgs e)
        {

        }

        private void FilterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(FilterBox.SelectedIndex)
            {
                case 0:
                    textBox1.Text = "1"; textBox2.Text = "0";
                    textBox3.Text = "-1"; textBox4.Text = "-1";  textBox5.Text = "-1";
                    textBox6.Text = "-1"; textBox7.Text = "9";   textBox8.Text = "-1";
                    textBox9.Text = "-1"; textBox10.Text = "-1"; textBox11.Text = "-1";
                    break;
                case 1:
                    textBox1.Text = "16"; textBox2.Text = "0";
                    textBox3.Text = "1"; textBox4.Text = "2"; textBox5.Text = "1";
                    textBox6.Text = "2"; textBox7.Text = "4"; textBox8.Text = "2";
                    textBox9.Text = "1"; textBox10.Text = "2"; textBox11.Text = "1";
                    break;
                case 2:
                    textBox1.Text = "1"; textBox2.Text = "0";
                    textBox3.Text = "1"; textBox4.Text = "-2"; textBox5.Text = "1";
                    textBox6.Text = "-2"; textBox7.Text = "4"; textBox8.Text = "-2";
                    textBox9.Text = "1"; textBox10.Text = "-2"; textBox11.Text = "1";
                    break;
                case 3:
                    textBox1.Text = "1"; textBox2.Text = "128";
                    textBox3.Text = "0"; textBox4.Text = "0"; textBox5.Text = "0";
                    textBox6.Text = "-1"; textBox7.Text = "2"; textBox8.Text = "-1";
                    textBox9.Text = "0"; textBox10.Text = "0"; textBox11.Text = "0";
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!(float.TryParse(textBox3.Text, out matrix[0, 0]) && (float.TryParse(textBox4.Text, out matrix[0, 1])) && (float.TryParse(textBox5.Text, out matrix[0, 2])) &&
                 (float.TryParse(textBox6.Text, out matrix[1, 0])) && (float.TryParse(textBox7.Text, out matrix[1, 1])) && (float.TryParse(textBox8.Text, out matrix[1, 2])) &&
                 (float.TryParse(textBox9.Text, out matrix[2, 0])) && (float.TryParse(textBox10.Text, out matrix[2, 1])) && (float.TryParse(textBox11.Text, out matrix[2, 2])) &&
                 (float.TryParse(textBox1.Text, out weight)) && (float.TryParse(textBox2.Text, out offset)))) 
            {
                MessageBox.Show("Error!");
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
