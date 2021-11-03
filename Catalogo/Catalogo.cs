using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Net;

namespace Catalogo
{
    public partial class Catalogo : Form
    {
        int selectedRow=-1;
        string path = @"c:\Catalogo\list.csv";

        public Catalogo()
        {
            InitializeComponent();
            try
            {
                string[] lineas = File.ReadAllLines(path);
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
            
        }

        private void Catalogo_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                List<string> filas = new List<string>();
                foreach (DataGridViewRow fila in dataGridView1.Rows)
                {
                    List<string> celdas = new List<string>();
                    foreach (DataGridViewCell c in fila.Cells)
                        celdas.Add(c.Value.ToString());
                    filas.Add(string.Join(",", celdas));
                }
                File.WriteAllLines(path, filas);
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
            
            /* fuck pastebin
            string text = Clipboard.GetText();
            if (text.Length != 0 && textBox1.Text.Length != 0)
            {
            Upload:

                System.Collections.Specialized.NameValueCollection Data
                    = new System.Collections.Specialized.NameValueCollection();

                Data["api_paste_name"] = "Catalogo.txt"; //Title
                Data["api_paste_expire_date"] = "2W"; //2 Weeks
                Data["api_paste_code"] = "here is the content";
                Data["api_dev_key"] = "";
                Data["api_option"] = "paste";
                Data["$api_paste_private"] = "0"; // 0=public 1=unlisted 2=private

                WebClient wb = new WebClient();
                byte[] bytes = wb.UploadValues("http://pastebin.com/api/api_post.php", Data);

                string response;
                using (MemoryStream ms = new MemoryStream(bytes))
                using (StreamReader reader = new StreamReader(ms))
                    response = reader.ReadToEnd();

                if (response.StartsWith("Bad API request"))
                {
                    if (MessageBox.Show("Failed to upload!", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                        goto Upload;
                }
                else
                {
                    response = "http://pastebin.com/raw.php?i=" + response.Substring(20);

                    Clipboard.SetText(response);
                    MessageBox.Show("YOUR PASTE: " + response);
                    //MessageBox.Show("Succesfully uploaded! \r\nLink copied to clipboard.", "Success!");
                    System.Diagnostics.Process.Start(response);
                }

            }
            else
            {
                MessageBox.Show("NO DATA IN CLIPBOARD!");
            }*/

            /*string apiKey = "";
            var client = new PasteBinClient(apiKey);
            var entry = new PasteBinEntry
            {
                Title = "PasteBinCatalogo",
                Text = "Here it goes the content",
                Expiration = PasteBinExpiration.OneMonth,
                Private = false,
                Format = "None"
            };

            string pasteUrl = client.Paste(entry);
            MessageBox.Show("Your paste is published at this URL: " + pasteUrl);*/


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
                        foreach (DataGridViewRow viewRow in dataGridView1.Rows)
                        {
                            foreach (DataGridViewCell dcell in viewRow.Cells)
                            {
                                pTable.AddCell(dcell.Value.ToString());
                            }
                        }


                        using (FileStream fileStream = new FileStream(@"c:\Catalogo\list.pdf", FileMode.Create))
                        {
                            Document document = new Document(PageSize.A4, 8f, 16f, 16f, 8f);
                            PdfWriter.GetInstance(document, fileStream);
                            document.Open();
                            document.Add(pTable);
                            document.Close();
                            fileStream.Close();
                        }
                        MessageBox.Show("Data Export Successfully", "info");

                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show("Error while exporting Data" + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("No Data Found", "Info");

            }

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
