using System;
using System.IO;
using System.Windows.Forms;

namespace Catalogo
{
    public partial class FormAñadir : Form
    {
        public string precio, marca, producto, image;
        public FormAñadir()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            producto = txtProducto.Text;
            marca = txtMarca.Text;
            precio = txtPrecio.Text;
            image = String.Concat(
                @"C:\Catalogo\images\", marca, "-", producto,
                new FileInfo(txtImage.Text).Extension);
            File.Copy(txtImage.Text, image);
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
