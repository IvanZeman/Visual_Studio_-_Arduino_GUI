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
using System.Xml.Linq;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace program_2
{
    public partial class Form1 : Form
    {
        public delegate void d1(string indata);
        private Series series1; // Prvá série pre prvé zobrazovanie
        private Series series2; // Druhá série pre druhé zobrazovanie
        int XLabelCount;
        int rowIndex;

        public Form1()
        {
           InitializeComponent();
           serialPort1.DataReceived += SerialPort1_DataReceived; // Subscribe to the DataReceived event

            // Inicializácia sady dát pre Chart
            series1 = new Series();
            series2 = new Series();
            chart1.Series.Add(series1);
            chart1.Series.Add(series2);
            series1.ChartType = SeriesChartType.Line; // Určuje typ grafu (v tomto prípade čiara)
            series2.ChartType = SeriesChartType.Line;
            chart1.ChartAreas[0].AxisY.Minimum = 0; // Nastavenie minimálnej zobrazenej hodnoty osi Y
            chart1.ChartAreas[0].AxisY.Maximum = 5; // Nastavenie maximálnej zobrazenej hodnoty osi Y
            connection_test();
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
            string timeString = label_Real_Time.Text; // Formát času, ktorý chcete zobraziť
            char firstchar;
            Single numdata;
            Single volts;
            Single volts2;
            firstchar = indata[0];
            checkBox2.CheckState = CheckState.Checked;
            checkBox3.CheckState = CheckState.Checked;
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
                    //Hodnota č.1
                    if (checkBox2.Checked == true)
                    {
                        textBox2.Visible = true;
                        //získaná hodnota č.1
                        numdata = Convert.ToSingle(indata.Substring(1));
                        volts = numdata * 5 / 1024;
                        TextBox2Value(volts);   //zobrazí aktuálnu hodnotu v okne TextBox2
                        AddDataPointToSeries(timeString, volts, series1); // Pridanie hodnôt do prvej série

                        // Zmena farby čiary na Y-ovej osi pre hodnotu č.1
                        DataPoint lastPoint = series1.Points.Last();
                        lastPoint.Color = (volts >= 4) ? Color.Red : (volts >= 3) ? Color.Orange : Color.Green;
                        lastPoint.BorderWidth = 4;
                        progressBar1.Value = Convert.ToInt16(indata.Substring(1));  //progressBar1

                        //TABULKA
                        XLabelCount++;
                        int rowIndex = tableLayoutPanel1.ColumnCount++;

                        if (rowIndex < 5)
                        {
                            tableLayoutPanel1.Controls.Add(new Label() { Text = String.Format("{00:00:00}", timeString) }, rowIndex, 0);       //zapísanie času do tabuľky
                            tableLayoutPanel1.Controls.Add(new Label() { Text = String.Format("{0:0.00}", volts) }, rowIndex, 1);             //zapísanie hodnoty zo snímača do tabuľky
                        }
                        else
                        {
                            tableLayoutPanel1.Controls.Add(new Label() { Text = String.Format("{00:00:00}", timeString) }, rowIndex - 1, 0);       //zapísanie času do tabuľky
                            tableLayoutPanel1.Controls.Add(new Label() { Text = String.Format("{0:0.00}", volts) }, rowIndex - 1, 1);
                        }
                    }
                    else
                    {
                        textBox2.Visible = false;
                    }

                    //hodnota č.2
                    if (checkBox3.Checked == true)
                    {
                        textBox3.Visible = true;
                        //získaná hodnota č.2
                        volts2 = Convert.ToSingle(indata.Substring(1)) * 3 / 1024;
                        TextBox3Value(volts2); // Zobrazenie hodnoty v TextBox3
                        AddDataPointToSeries(timeString, volts2, series2); // Pridanie hodnôt do druhej série

                        // Zmena farby čiary na Y-ovej osi pre hodnotu č.1 pre hodnotu č.2
                        DataPoint lastPoint2 = series2.Points.Last();
                        lastPoint2.Color = (volts2 >= 2) ? Color.Purple : (volts2 >= 1) ? Color.Violet : Color.Blue;
                        lastPoint2.BorderWidth = 4;
                        progressBar1.Value = Convert.ToInt16(indata.Substring(1));  //progressBar1
                    }
                    else {
                        textBox3.Visible = false;
                    }





























                    /*  XLabelCount++;
                    if (XLabelCount < 14)
                    {
                        int rowIndex = tableLayoutPanel1.RowCount++;
                        tableLayoutPanel1.Controls.Add(new Label() { Text = XLabelCount.ToString() }, 0, 0);
                        tableLayoutPanel1.Controls.Add(new Label() { Text = volts.ToString() }, 0, 1);
                    }
                    else
                    {
                        tableLayoutPanel1.GetControlFromPosition(0, 0).Text = XLabelCount.ToString();
                        tableLayoutPanel1.GetControlFromPosition(0, 1).Text = volts.ToString();
                        rowIndex = 0;
                    }
                    */
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
            timer1.Start();
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


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }
        public void connection_test()
        {
            if(!serialPort1.IsOpen) {
                button1_Connect.Visible = true;
                button2_Disconnect.Visible = false;
                label1_Status.Text = "Serial Port : Disconnected";
                label1_Status.ForeColor = Color.Red;
            }
        }

        private void button2_Disconnect_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            button1_Connect.Visible = true;
            button2_Disconnect.Visible = false;
            label1_Status.Text = "Serial Port : Disconnected";
            label1_Status.ForeColor = Color.Red;
            panel1.BackColor = Color.White;
        }

        private void button1_Connect_Click(object sender, EventArgs e)
        {
            serialPort1.Open();
            button1_Connect.Visible = false;
            button2_Disconnect.Visible = true;
            label1_Status.Text = "Serial Port : Connected";
            label1_Status.ForeColor = Color.Green;
            panel1.BackColor = Color.LightGreen;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label_Real_Time.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void TextBox2Value(float volts)
        {
            textBox2.Text = String.Format("{0:0.00}", volts);
            textBox2.BackColor = volts >= 4 ? Color.Red : volts >= 3 ? Color.Orange : Color.Green;
        }

        private void TextBox3Value(float volts)
        {
            textBox3.Text = String.Format("{0:0.00}", volts);
            textBox3.BackColor = volts >= 2 ? Color.Purple : volts >= 1 ? Color.Violet : Color.Blue;
        }

        //OBMEDZENIE ZOBRAZENIA GRAFU NA MAX. 50 BODOV
        private void AddDataPointToSeries(string timeString, float volts, Series series)
        {
            if ((series1.Points.Count < 50 && series2.Points.Count < 50))
            {
                series.Points.AddXY(timeString, volts); // Pridá nový bod do grafu
            }
            else
            {
                series.Points.RemoveAt(0); // Odstráni prvý (najstarší) bod z grafu
                series.Points.AddXY(timeString, volts); // Pridá nový bod do grafu
            }
        }
    }
}
