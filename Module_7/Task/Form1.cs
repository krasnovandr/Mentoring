using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace KeyGen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();
            var addressBytes = networkInterface.GetPhysicalAddress().GetAddressBytes();

            var currentDateBinary = BitConverter.GetBytes(DateTime.Now.Date.ToBinary());

            var addressBytesXorWithDate = addressBytes.Select((addr, i) => addr ^ currentDateBinary[i]);

            var multipliedBytes = addressBytesXorWithDate.Select(b => b <= 999 ? b * 10 : b).ToList();

            textBox1.Text = string.Join("-", multipliedBytes); 
        }
    }
}