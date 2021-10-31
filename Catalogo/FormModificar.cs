using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Catalogo
{
    public partial class FormModificar : Form
    {
        public string precio, marca, producto, image;

        public FormModificar(string[] data)
        {
            InitializeComponent();
            txtProducto.Text = data[0];
            txtMarca.Text = data[1];
            txtPrecio.Text = data[2];
            txtImage.Text = data[3];
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            producto = txtProducto.Text;
            marca = txtMarca.Text;
            precio = txtPrecio.Text;
            image = txtImage.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                txtImage.Text = openFileDialog1.FileName;
            }
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}
