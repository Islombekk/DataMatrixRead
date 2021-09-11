using System;
using System.Windows.Forms;

namespace DataMatrixRead
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }
        IniFile iniFile = new IniFile("Settings.ini");

        private void button4_Click(object sender, EventArgs e)
        {
            iniFile.Write("ScanDirectory", textBox1.Text, "SETTINGS");
            iniFile.Write("SaveDirectory", textBox2.Text, "SETTINGS");
            iniFile.Write("FailDirectory", textBox3.Text, "SETTINGS");
            iniFile.Write("ScanTime", numericUpDown1.Value.ToString(), "SETTINGS");
            iniFile.Write("ScanCount", numericUpDown2.Value.ToString(), "SETTINGS");
            iniFile.Write("DeviceName", textBox4.Text, "SETTINGS");
            iniFile.Write("Url", textBox5.Text, "SETTINGS");
            this.Hide();

        }

        private void Settings_Load(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = iniFile.ReadString("ScanDirectory", "SETTINGS");
                textBox2.Text = iniFile.ReadString("SaveDirectory", "SETTINGS");
                textBox3.Text = iniFile.ReadString("FailDirectory", "SETTINGS");
                numericUpDown1.Value = iniFile.ReadInt("ScanTime", "SETTINGS");
                numericUpDown2.Value = iniFile.ReadInt("ScanCount", "SETTINGS");
                textBox4.Text = iniFile.ReadString("DeviceName", "SETTINGS");
                textBox5.Text = iniFile.ReadString("Url", "SETTINGS");
            }
            catch (Exception ex)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = FBD.SelectedPath;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = FBD.SelectedPath;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = FBD.SelectedPath;
            }

        }
    }
}
