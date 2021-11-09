using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
    }
}
