using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;


namespace ComPortUygulamasi
{
    public partial class Form1 : Form
    {

        public static Int32 AlınanData = 0;
        public static Byte[] Buffer = new byte[40];
        public static string[] ports;

        public Form1()
        {
            InitializeComponent();
            //Mevcut portları görüntüle
            getAvaiblePorts();
            //Arka plan rengini ayarla
            this.BackColor = Color.Silver;
            //this.BackColor = Color.FloralWhite;

        }


        //mevcut portları combobox a gönderdik..
        void getAvaiblePorts()
        {

             ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
        }

        //Baudrate için formda items;collection tıklanır ve değerler alt alta eklenir. 
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        //form menususunden readonly true yaptık
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        //Portu Aç
        private void OpenPort_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "" || comboBox2.Text == "")
                {
                    //textBox2.Text = "Lütfen port ayarlamalarını yapınız";                    
                    MessageBox.Show("Lütfen port ayarlamalarını yapınız!", "Bilgilendirme Penceresi");

                }
                else
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.StopBits = StopBits.One;
                    serialPort1.Parity = Parity.None;
                    serialPort1.Open();
                    progressBar1.Value = 100;
                    send.Enabled = true;
                    read.Enabled = true;
                    textBox1.Enabled = true;
                    OpenPort.Enabled = false;
                    ClosePort.Enabled = true;
                    status_label.Text = string.Format("{0} Bağlandı.", ports);
                    textBox2.Text = "";
                }
            }
            catch (UnauthorizedAccessException)
            {

                textBox2.Text = "Yetkilendirilmemiş Erişim";
            }
        }

        //Portu kapat
        private void ClosePort_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            progressBar1.Value = 0;
            send.Enabled = false;
            read.Enabled = false;
            OpenPort.Enabled = true;
            ClosePort.Enabled = false;
            textBox1.Enabled = false;
            status_label.Text = "Bağlantı yok";
            status_label.ForeColor = Color.FromArgb(50, 0, 0);

        }


        //bilgileri yolla
        private void send_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Write(textBox1.Text + Environment.NewLine);
                    textBox1.Clear();
                }

            }
            catch (Exception Ex)
            {

                MessageBox.Show(Ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //read_Click(sender, e);

            //if (Buffer[0] == 0x80)
            //{
            //    status_label.Text = "Yazma başarılı";
            //    status_label.ForeColor = Color.FromArgb(0, 50, 0);
            //}
            //else
            //{
            //    status_label.Text = "Yazma Başarısız";
            //    status_label.ForeColor = Color.FromArgb(50, 0, 0);
            //}
        }

        //gelen bilgileri oku
        private void read_Click(object sender, EventArgs e)
        {
            //delay_ms(100);

            textBox2.Text = "";

            if (!serialPort1.IsOpen) return;

            try
            {
                textBox2.Text = serialPort1.ReadExisting();
            }

            catch (TimeoutException)
            {
                textBox2.Text = "Timeout Exception";
            }

            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
                //delay_ms(100);
        }



        void delay_ms(int x)
        {
            System.Threading.Thread.Sleep(x);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();

            label4.Text = DateTime.Now.ToLongTimeString();
            label5.Text = DateTime.Now.ToLongDateString();

        }

        //Saat/Tarih için timer
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            label4.Text = DateTime.Now.ToLongTimeString();
            timer1.Start();
        }

        //Portları yenile
        private void refleshButton_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Text = "";
            getAvaiblePorts();
        }

    }
    }