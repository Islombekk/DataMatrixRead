using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Vintasoft.Barcode;
using Vintasoft.Barcode.BarcodeInfo;
using Vintasoft.Barcode.QualityTests;
using Image = System.Drawing.Image;

namespace DataMatrixRead
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void checkLicence()
        {
            Vintasoft.Barcode.BarcodeGlobalSettings.Register("Saken Alimkulov", "sakenalimkulov.dev@gmail.com", "2021-08-30", "oh1+VSDfAwVWQk+EYVCCQkwOXlWnOm4blCrVBGSN0lJ27BlUgkl0ePnlGgxc+VU7b90YYlLZbpAzFHKMlI3fVYInfT/14N7LsEgnfYXuRCTFy3aRMMdy44fKhqwMBNwbFoPqS0xzTawKSrju6Vp63rM5AJ6YoKqAvljEQf9Gmp0");
        }

        List<string> fileNames = new List<string>();
        static IniFile iniFile = new IniFile("Settings.ini");
        string[] fileArray;
        UInt32 success = 0;
        UInt32 fail = 0;
        UInt32 count = 0;
        List<string> dataMatrix = new List<string>();
        List<Marks> marks1 = new List<Marks>();

        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer
        {
            Interval = iniFile.ReadInt("ScanTime", "SETTINGS") * 1000
        };

        private void начатьToolStripMenuItem_Click(object sender, EventArgs e)
        {

            fileNames.Clear();
            checkLicence();



            timer1.Enabled = true;
            timer1.Tick += new System.EventHandler(ReadFiles);
        }

        public void ReadFiles(object sender, EventArgs e)
        {
            fileArray = new string[iniFile.ReadInt("ScanCount", "SETTINGS")];
            fileArray = Directory.GetFiles(iniFile.ReadString("ScanDirectory", "SETTINGS"));

            foreach (string filenames in fileArray)
            {
                //pictureBox1.Image = Image.FromFile(filenames);
                fileNames.Add(filenames);

                ReadBarcodesFromImage(filenames);
                //ReadBarcodesAndTestBarcodePrintQuality(filenames, Image.FromFile(filenames));
                count++;
                label1.Text = count.ToString();
            }
        }


        public void ReadBarcodesFromImage(string filename)
        {
            // create barcode reader
            using (Vintasoft.Barcode.BarcodeReader reader = new Vintasoft.Barcode.BarcodeReader())
            {
                // specify that reader must search for Code39, Code39Extended,
                // Code128, SSCC18 and DataMatrix barcodes
                reader.Settings.ScanBarcodeTypes =
                    Vintasoft.Barcode.BarcodeType.DataMatrix;
                reader.Settings.ScanBarcodeSubsets.Add(Vintasoft.Barcode.SymbologySubsets.BarcodeSymbologySubsets.Code39Extended);
                reader.Settings.ScanBarcodeSubsets.Add(Vintasoft.Barcode.SymbologySubsets.BarcodeSymbologySubsets.SSCC18);

                // specify that reader must search for horizontal and vertical barcodes only
                reader.Settings.ScanDirection = Vintasoft.Barcode.ScanDirection.Horizontal | Vintasoft.Barcode.ScanDirection.Vertical;

                // use Automatic Recognition
                reader.Settings.AutomaticRecognition = true;

                // read barcodes from image file
                Vintasoft.Barcode.IBarcodeInfo[] infos = reader.ReadBarcodes(filename);

                // if barcodes are not detected

                if (infos.Length == 0)
                {
                    //pictureBox1.Image.Dispose();
                    //Console.WriteLine("No barcodes found.");
                    fail++;
                    string from = filename;
                    string[] url = filename.Split('\\');
                    string to = iniFile.ReadString("FailDirectory", "SETTINGS") + "\\" + url[url.Length - 1];

                    DirectoryInfo dirInfo = new DirectoryInfo(iniFile.ReadString("FailDirectory", "SETTINGS"));
                    File.Move(from, to);
                    label6.Text = fail.ToString();
                }
                // if barcodes are detected
                else
                {
                    // get information about extracted barcodes


                    for (int i = 0; i < infos.Length; i++)
                    {
                        try
                        {
                            Vintasoft.Barcode.IBarcodeInfo info = infos[i];
                            List<Marks> marks = new List<Marks>();

                            success++;
                            //pictureBox1.Image.Dispose();

                            string from = filename;
                            string[] url = filename.Split('\\');
                            string to = iniFile.ReadString("SaveDirectory", "SETTINGS") + "\\" + url[url.Length - 1];
                            marks.Add(new Marks(info.Value.Replace("<FNC1>", " "), " "));
                            //ReadBarcodesAndTestBarcodePrintQuality(Image.FromFile(filename), info.Value);
                            DirectoryInfo dirInfo = new DirectoryInfo(iniFile.ReadString("SaveDirectory", "SETTINGS"));
                            // Image.FromFile(filename).Dispose();

                            File.Move(from, to);
                            label4.Text = success.ToString();


                            SendMarks(marks);

                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                continue;
                                fail++;
                                string from = filename;
                                string[] url = filename.Split('\\');
                                string to = iniFile.ReadString("SaveDirectory", "SETTINGS") + "\\" + url[url.Length - 1];
                                DirectoryInfo dirInfo = new DirectoryInfo(iniFile.ReadString("FailDirectory", "SETTINGS"));
                                //Image.FromFile(filename).Dispose();

                                File.Copy(from, to);
                                label6.Text = fail.ToString();
                                SendMarks(from, null);
                            }
                            catch (Exception e)
                            {
                                continue;
                                fail++;
                                string from = filename;
                                string[] url = filename.Split('\\');
                                string to = iniFile.ReadString("FailDirectory", "SETTINGS") + "\\" + url[url.Length - 1];
                                DirectoryInfo dirInfo = new DirectoryInfo(iniFile.ReadString("SaveDirectory", "SETTINGS"));
                                //Image.FromFile(filename).Dispose();

                                File.Copy(from, to);
                                label6.Text = fail.ToString();
                                SendMarks(from, null);
                            }
                        }
                    }
                }
            }
        }
        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }

        private void остановитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void SendMarks(List<Marks> marks)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(iniFile.ReadString("Url", "SETTINGS"));
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new MarkJSON(iniFile.ReadString("DeviceName", "SETTINGS"), marks));

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

            System.IO.StreamWriter writer = new System.IO.StreamWriter("result.txt", true);

            for (int i = 0; i < marks1.Count; i++)
            {
                writer.WriteLine(DateTime.Now + " " + fileNames[i] + " " + marks1[i].code + " " + marks1[i].decode);


            }
            writer.Close();

        }

        public void SendMarks(string filename, string code)
        {


            System.IO.StreamWriter writer = new System.IO.StreamWriter("result.txt", true);


            writer.WriteLine(DateTime.Now + " " + filename + " " + code);

            writer.Close();

        }

        private string GradeToString(ISO15415QualityGrade grade)
        {
            return string.Format("{0}({1})", ((int)grade).ToString(), grade);
        }

        private void ReadBarcodesAndTestBarcodePrintQuality(string filename, System.Drawing.Image imageWithBarcodes)
        {

            using (BarcodeReader reader = new BarcodeReader())
            {
                reader.Settings.CollectTestInformation = true;

                reader.Settings.ScanBarcodeTypes = BarcodeType.Aztec | BarcodeType.DataMatrix | BarcodeType.QR | BarcodeType.MicroQR | BarcodeType.HanXinCode;
                reader.Settings.AutomaticRecognition = true;
                ISO15415QualityTest test = new ISO15415QualityTest();
                IBarcodeInfo[] barcodeInfos = reader.ReadBarcodes(imageWithBarcodes);

                if (barcodeInfos.Length > 0)
                {
                    for (int i = 0; i < barcodeInfos.Length; i++)
                    {

                        // ((BarcodeInfo2D)barcodeInfos[i]).Value
                        // test.AdditionalGrades.ContainsKey()
                        try
                        {
                            test.CalculateGrades((BarcodeInfo2D)barcodeInfos[i], imageWithBarcodes);
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        var additionalGradesKeys = "";
                        var quietZone = "";
                        foreach (string name in test.AdditionalGrades.Keys)
                        {
                            additionalGradesKeys = name.PadRight(40, ' ') + " " + GradeToString(test.AdditionalGrades[name]);
                        }
                        imageWithBarcodes.Dispose();

                        //ReadBarcodesFromImage(filename, GradeToString(test.DecodeGrade));
                        //marks1.Add(new Marks(markInfo, GradeToString(test.DecodeGrade)));
                        if (test.QuietZone >= 0)
                        {
                            quietZone = string.Format("{0} ({1} %)", GradeToString(test.QuietZoneGrade), test.QuietZone);
                        }

                    }
                }
                else
                {
                    fail++;
                    string from = filename;
                    string[] url = filename.Split('\\');
                    string to = iniFile.ReadString("FailDirectory", "SETTINGS") + "\\" + url[url.Length - 1];
                    DirectoryInfo dirInfo = new DirectoryInfo(iniFile.ReadString("FailDirectory", "SETTINGS"));
                    imageWithBarcodes.Dispose();

                    File.Move(from, to);
                    label6.Text = fail.ToString();
                    //SendMarks(from, null, null);
                }
            }

            imageWithBarcodes.Dispose();

        }
    }
}