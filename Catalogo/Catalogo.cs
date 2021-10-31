using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;

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
                    filas.Add(string.Join(',', celdas));
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

        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            updateDataGrid();
        }
    }
}
