using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleCatalogo
{
    public partial class Form1 : Form
    {
        string pdfPath="";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pdfPath = openFileDialog1.FileName;
                textBox1.Text = pdfPath;
                File.Copy(pdfPath, @"c:\Catalogo\catalogo.pdf");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (StreamWriter w = new StreamWriter(@"C:\Catalogo\git.bat"))
            {
                string gitPath = @"C:\Catalogo\uploader\cmd\git";
                w.WriteLine(gitPath + @" add c:\Catalogo\catalogo.pdf");
                w.WriteLine(gitPath + " commit -m \"catalogo change\"");
                w.WriteLine(gitPath + " branch -M main");
                w.WriteLine(gitPath + @"  remote add origin https://matyz97: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@github.com/matyz97/catalogo.git");
                w.WriteLine(gitPath + " push -u origin main");
                w.WriteLine("del git.bat");
                w.Close();
            }
            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + @"C:\Catalogo\git.bat");
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            procStartInfo.WorkingDirectory = @"C:\Catalogo";
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            MessageBox.Show("Data Export Successfully");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (File.ReadLines(@"c:\Catalogo\README.MD").SequenceEqual(File.ReadLines(@"c:\Catalogo\version.md")))
                    File.Delete(@"c:\Catalogo\README.MD");
                else
                {
                    string[,] aux = { 
                                                { @"https://github.com/MatiasCardullo/Catalogo/raw/main/Catalogo/bin/Debug/net46/Catalogo.exe", @"C:\Catalogo2\Catalogo.exe" },
                                                { @"https://github.com/MatiasCardullo/Catalogo/raw/main/SimpleCatalogo/bin/Debug/SimpleCatalogo.exe", @"C:\Catalogo\SimpleCatalogo.exe" }
                                                };
                    for (int i = 0; i < 2; i++)
                    {
                        using (WebClient wc = new WebClient())
                        {
                            string url = aux[i, 0]; string path = aux[i, 1];
                            wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                            wc.DownloadFileAsync(new Uri(url), path);
                        }
                    }
                    File.Delete(@"c:\Catalogo\version.md");
                    File.Move(@"c:\Catalogo\README.MD", @"c:\Catalogo\version.md");
                }
            }
            catch (Exception ex) { }
        }
        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //progressBar.Value = 0;
            if (e.Cancelled)
                MessageBox.Show("The download has been cancelled");
            else if (e.Error != null)
                MessageBox.Show("An error ocurred while trying to download");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                wc.DownloadFileAsync(new Uri(@"https://raw.githubusercontent.com/MatiasCardullo/Catalogo/main/README.md"), @"c:\Catalogo\README.MD");
            }
        }
    }
}
