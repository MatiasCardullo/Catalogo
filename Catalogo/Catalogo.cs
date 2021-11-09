using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Linq;
using System.Net;
using System.ComponentModel;

namespace Catalogo
{
    public partial class Catalogo : Form
    {
        int selectedRow=-1;
        public bool update = false;
        string pathCsv = @"c:\Catalogo\list.csv";

        public Catalogo()
        {
            InitializeComponent();
            try
            {
                string[] lineas = File.ReadAllLines(pathCsv);
                for (int i = 0; i < lineas.Length; i++)
                {
                    string[] celdas = lineas[i].Split(',');
                    dataGridView1.Rows.Add(celdas);
                }
            }
            catch (Exception ex) { }
        }

        private void Catalogo_Load(object sender, EventArgs e)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                wc.DownloadFileAsync(new Uri(@"https://github.com/MatiasCardullo/Catalogo/raw/main/Catalogo/README.MD"), @"c:\Catalogo\README.MD");
            }
            if (File.ReadLines(@"c:\Catalogo\README.MD").SequenceEqual(File.ReadLines(@"c:\Catalogo\version.md")))
                File.Delete(@"c:\Catalogo\README.MD");
            else
                update = true;
        }
        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //progressBar.Value = e.ProgressPercentage;
        }
        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //progressBar.Value = 0;
            if (e.Cancelled)
                MessageBox.Show("The download has been cancelled");
            else if (e.Error != null)
                MessageBox.Show("An error ocurred while trying to download git");
        }

        private void Catalogo_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (update)
                {
                    string[,] aux = { { @"https://github.com/MatiasCardullo/Catalogo/raw/main/Catalogo/bin/Debug/net46/itextsharp.dll", @"C:\Catalogo2\itextsharp.dll" },
                                                { @"https://github.com/MatiasCardullo/Catalogo/raw/main/Catalogo/bin/Debug/net46/BouncyCastle.Crypto.dll", @"C:\Catalogo2\BouncyCastle.Crypto.dll" },
                                                { @"https://github.com/MatiasCardullo/Catalogo/raw/main/Catalogo/bin/Debug/net46/Catalogo.exe", @"C:\Catalogo2\Catalogo.exe" }
                                                };
                    for (int i = 0; i < 3; i++)
                    {
                        using (WebClient wc = new WebClient())
                        {
                            string url = aux[i, 0]; string path = aux[i, 1];
                            wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                            wc.DownloadFileAsync(new Uri(url), path);
                        }
                    }
                    File.Delete(@"c:\Catalogo\version.md");
                    File.Move(@"c:\Catalogo\README.MD",@"c:\Catalogo\version.md");
                }
                List<string> filas = new List<string>();
                foreach (DataGridViewRow fila in dataGridView1.Rows)
                {
                    List<string> celdas = new List<string>();
                    foreach (DataGridViewCell c in fila.Cells)
                        celdas.Add(c.Value.ToString());
                    filas.Add(string.Join(",", celdas));
                }
                File.WriteAllLines(pathCsv, filas);
            }
            catch (Exception ex) { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormAñadir frm = new FormAñadir();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                int r = dataGridView1.Rows.Add();
                dataGridView1.Rows[r].Cells[0].Value = frm.producto;
                dataGridView1.Rows[r].Cells[1].Value = frm.marca;
                dataGridView1.Rows[r].Cells[2].Value = frm.precio;
                dataGridView1.Rows[r].Cells[3].Value = frm.image;
                updateDataGrid();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (selectedRow != -1)
            {
                string[] data = { dataGridView1.Rows[selectedRow].Cells[0].Value.ToString(),
                    dataGridView1.Rows[selectedRow].Cells[1].Value.ToString(),
                    dataGridView1.Rows[selectedRow].Cells[2].Value.ToString(),
                    dataGridView1.Rows[selectedRow].Cells[3].Value.ToString()};
                FormModificar frm = new FormModificar(data);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.Rows[selectedRow].Cells[0].Value = frm.producto;
                    dataGridView1.Rows[selectedRow].Cells[1].Value = frm.marca;
                    dataGridView1.Rows[selectedRow].Cells[2].Value = frm.precio;
                    dataGridView1.Rows[selectedRow].Cells[3].Value = frm.image;
                    updateDataGrid();
                }
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedRow!= -1)
            {
                dataGridView1.Rows.RemoveAt(selectedRow);
                dataGridView1.ClearSelection();
                pictureBox1.ImageLocation = "";
                selectedRow = -1;
                updateDataGrid();
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                bool ErrorMessage = false;
                if (File.Exists(@"c:\Catalogo\list.pdf"))
                {
                    try
                    {
                        File.Delete(@"c:\Catalogo\list.pdf");
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = true;
                        MessageBox.Show("Unable to wride data in disk" + ex.Message);
                    }
                }
                if (!ErrorMessage)
                {
                    try
                    {
                        PdfPTable pTable = new PdfPTable(dataGridView1.Columns.Count);
                        pTable.DefaultCell.Padding = 2;
                        pTable.WidthPercentage = 100;
                        pTable.HorizontalAlignment = Element.ALIGN_LEFT;

                        foreach (DataGridViewColumn col in dataGridView1.Columns)
                        {
                            PdfPCell pCell = new PdfPCell(new Phrase(col.HeaderText));
                            pTable.AddCell(pCell);
                        }
                        byte i = 1;
                        foreach (DataGridViewRow viewRow in dataGridView1.Rows)
                        {
                            foreach (DataGridViewCell dcell in viewRow.Cells)
                            {
                                string aux = dcell.Value.ToString();
                                if (i == 4)
                                {
                                    System.Drawing.Image image = System.Drawing.Image.FromFile(aux);
                                    Image pic = iTextSharp.text.Image.GetInstance(image, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    pTable.AddCell(pic);
                                    i = 0;
                                }
                                else
                                    pTable.AddCell(aux);
                                i++;
                            }
                        }
                        using (FileStream fileStream = new FileStream(@"c:\Catalogo\list.pdf", FileMode.Create))
                        {
                            Document document = new Document(PageSize.A4, 8f, 8f, 8f, 8f);
                            PdfWriter.GetInstance(document, fileStream);
                            document.Open();
                            document.Add(pTable);
                            document.Close();
                            fileStream.Close();
                        }
                        string a = DateTime.Today.ToString();

                        Command(@"C:\Catalogo\uploader\cmd\git add c:\Catalogo\list.pdf");
                        Command("C:\\Catalogo\\uploader\\cmd\\git commit -m \"" + DateTime.Today.ToString() + "\"");
                        Command(@"C:\Catalogo\uploader\cmd\git branch -M main");
                        Command(@"C:\Catalogo\uploader\cmd\git remote add origin https://github.com/matyz97/Catalogo.git");
                        Command(@"C:\Catalogo\uploader\cmd\git push -u origin main");
                        MessageBox.Show("Data Export Successfully");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error while exporting Data" + ex.Message);
                    }
                }
            }
            else
                MessageBox.Show("No Data Found", "Info");
        }
        private void Command(string command)
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + command);
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            procStartInfo.WorkingDirectory = @"C:\Catalogo";
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
        }
        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            updateDataGrid();
        }

        private void updateDataGrid()
        {
            if (dataGridView1.CurrentCell != null)
            {
                selectedRow = dataGridView1.CurrentCell.RowIndex;
                pictureBox1.ImageLocation = dataGridView1.Rows[selectedRow].Cells[3].Value.ToString();
            }
            else
            {
                selectedRow = -1;
                pictureBox1.ImageLocation = "";
            }
        }
    }
}
