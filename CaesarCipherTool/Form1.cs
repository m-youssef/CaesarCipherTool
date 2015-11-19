using System;
using System.IO;
using System.Windows.Forms;

namespace CaesarCipherTool
{
    /// <summary>
    /// Initialize Caesar Cipher Tool GUI 
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Initialize Component
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtCipher.Text = MyCaesarCipher.Encrypt(txtPlain.Text.ToLower(), (int)numKey.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            var file = openFileDialog1.FileName;
            if (!string.IsNullOrEmpty(file))
            {
                Clear();
                var txt = File.ReadAllText(file);
                txtPlain.Text = txt.ToLower();
                txtCipher.Enabled = false;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            var file = openFileDialog1.FileName;
            if (!string.IsNullOrEmpty(file))
            {
                Clear();
                var txt = File.ReadAllText(file);
                txtCipher.Text = txt.ToLower();
                txtPlain.Enabled = false;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            int key;
            string txt;
            MyCaesarCipher.AutoDecrypt(txtCipher.Text.ToLower(), out key, out txt);

            numKey.Value = key;
            txtPlain.Text = txt;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            txtPlain.Clear();
            txtCipher.Clear();
            txtPlain.Enabled = true;
            txtCipher.Enabled = true;
            numKey.Value = 0;
        }

    }
}
