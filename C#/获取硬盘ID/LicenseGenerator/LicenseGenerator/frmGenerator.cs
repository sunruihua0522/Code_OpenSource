using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Com.Netframe.Computer.Handware;
namespace Com.Netframe.License.Window
{
    public partial class frmGenerator : Form
    {
        public frmGenerator()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HardwareInfo info = new HardwareInfo();
            textBox1.Text = textBox1.Text + info.GetComputerName() + "\r\n";
            textBox1.Text = textBox1.Text + info.GetCpuID() + "\r\n";
            textBox1.Text = textBox1.Text + info.GetDiskID() + "\r\n";
            textBox1.Text = textBox1.Text + info.GetDiskVolumeSerialNumber() + "\r\n";
            textBox1.Text = textBox1.Text + info.GetIPAddress() + "\r\n";
            textBox1.Text = textBox1.Text + info.GetMacAddress() + "\r\n";
            textBox1.Text = textBox1.Text + info.GetMNum() + "\r\n";
            textBox1.Text = textBox1.Text + info.GetRNum() + "\r\n";
            textBox1.Text = textBox1.Text + info.GetSystemType() + "\r\n";
            textBox1.Text = textBox1.Text + info.GetTotalPhysicalMemory() + "\r\n";
            textBox1.Text = textBox1.Text + info.GetUserName() + "\r\n";

        }

        private void frmGenerator_Load(object sender, EventArgs e)
        {
            Console.WriteLine(TimeSpan.FromTicks(2585704745-742647829).TotalSeconds.ToString());
        }
    }
}
