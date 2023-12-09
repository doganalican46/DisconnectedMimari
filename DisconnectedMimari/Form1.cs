using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DisconnectedMimari
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SqlConnection baglanti = new SqlConnection("Server=.\\SQLEXPRESS;Database=KuzeyYeli;Integrated Security=True;");

            SqlDataAdapter adp = new SqlDataAdapter("select * from Urunler",baglanti);

            DataTable dt = new DataTable(); //dataadapter ın elde ettiği tabloyu tutar

            adp.Fill(dt);

            dataGridView1.DataSource=dt;

            dataGridView1.Columns["UrunID"].Visible=false;
            dataGridView1.Columns["KategoriID"].Visible = false;
            dataGridView1.Columns["TedarikciID"].Visible = false;


        }

    }
}
