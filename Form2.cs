using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq.Expressions;

namespace program_2
{
    public partial class Form1 : Form
    {
        public delegate void d1(string indata);
        private Series series;

        public Form1()
        {
            InitializeComponent();
            serialPort1.Close();
            serialPort1.DataReceived += SerialPort1_DataReceived; // Subscribe to the DataReceived event

            // Inicializácia sady dát pre Chart
            series = new Series();
            chart1.Series.Add(series);
            series.ChartType = SeriesChartType.Line; // Určuje typ grafu (v tomto prípade čiara)

            button_Connect.Visible = false;
            button_Connect.BringToFront();
            button_Disconnect.Visible = false;
            button_Disconnect.SendToBack();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                serialPort1.Write("R1_ON");
            }
            if (checkBox1.Checked == false)
            {
                serialPort1.Write("R1_OFF");
            }
        }

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string indata = serialPort1.ReadLine().Trim();
            d1 writeit = new d1(Write2Form);
            Invoke(writeit, indata);
        }

        public void Write2Form(string indata)
        {
            char firstchar;
            Single numdata;
            Single volts;
            firstchar = indata[0];
            switch (firstchar)
            {
                case 'Z':
                    textBox1.Text = string.Empty;
                    textBox1.AppendText(indata);
                    textBox1.BackColor = Color.Green;
                    if (!checkBox1.Checked)
                    {
                        serialPort1.Write("R1_OFF");
                    }
                    break;
                case 'V':
                    textBox1.Text = string.Empty;
                    textBox1.AppendText(indata);
                    textBox1.BackColor = Color.Red;
                    if (checkBox1.Checked)
                    {
                        serialPort1.Write("R1_ON");
                    }
                    break;
                case 'v':
                    numdata = Convert.ToSingle(indata.Substring(1));
                    volts = numdata * 5 / 1024;
                    textBox2.Text = String.Format("{0:0.00}", volts);   //textBox2 - zobrazenie hodnoty
                    if (volts >= 4)
                    {                              //zmena farby textu
                        textBox2.BackColor = Color.Red;
                    }
                    else if (volts >= 3)
                    {
                        textBox2.BackColor = Color.Orange;
                    }
                    else
                    {
                        textBox2.BackColor = SystemColors.Window;
                    }

                    series.Points.Add(volts);                           //graf - zobrazenie hodnoty

                    // Zmena farby čiary na Y-ovej osi
                    DataPoint lastPoint = series.Points.Last();
                    lastPoint.Color = (volts >= 4) ? Color.Red : (volts >= 3) ? Color.Orange : Color.Green;

                    progressBar1.Value = Convert.ToInt16(indata.Substring(1));  //progressBar1
                    break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serialPort1.Write("R1_OFF");
            Thread.Sleep(1000);
            serialPort1.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        CenterToParent();                       //centrovanie zobrazného okna (aplikácie)
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
        PopulateSerialPorts(); // Zavolajte metódu na vyhľadanie dostupných COM portov
        }
        private void PopulateSerialPorts()
        {
            // Získajte všetky dostupné COM porty a zobrazte ich v ComboBoxe
            string[] availablePorts = SerialPort.GetPortNames();
            comboBoxPort.Items.Clear();
            comboBoxPort.Items.AddRange(availablePorts);

            if (comboBoxPort.Items.Count > 0)
            {
                comboBoxPort.SelectedIndex = 0;
                button_Connect.Visible = true;
            }
            else
            {
                MessageBox.Show("No COM ports detected", "Warning !!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button_Disconnect_Click(object sender, EventArgs e)
        {

        }

        private void button_Connect_Click(object sender, EventArgs e)
        {
           
            serialPort1.PortName = comboBoxPort.SelectedItem.ToString();
            serialPort1.Open();
            //TimerSerial.Start();

            LabelStatus.Text = "Status : Connected";
            button_Connect.SendToBack();
            button_Disconnect.BringToFront();
            //PictureBoxConnectionStatus.BackColor = Color.Green
        }
    }   
}
