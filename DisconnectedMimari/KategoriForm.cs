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
    public partial class KategoriForm : Form
    {
        public KategoriForm()
        {
            InitializeComponent();
        }


        SqlConnection baglanti = new SqlConnection("Server=.\\SQLEXPRESS;Database=KuzeyYeli;Integrated Security=True;");


        private void KategoriForm_Load(object sender, EventArgs e)
        {
            KategoriListesi();
        }

        private void KategoriListesi()
        {
            //SqlDataAdapter adp = new SqlDataAdapter("select * from Kategoriler", baglanti); //1.yöntem
            SqlDataAdapter adp = new SqlDataAdapter("procedure_KategoriListele", baglanti); //2.yöntem
            adp.SelectCommand.CommandType = CommandType.StoredProcedure; //2.yöntem için gerekli kısım

            DataTable dt = new DataTable(); //dataadapter ın elde ettiği tabloyu tutar

            adp.Fill(dt); //bağlantıyı otomatik açar ve kapatır (sadece select işleminde geçerli update ve insert için elle açıp-kapatmamız lazım)

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["KategoriID"].Visible = false;
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            SqlCommand komut = new SqlCommand("proc_KategoriEkle",baglanti);
            komut.CommandType = CommandType.StoredProcedure;

            komut.Parameters.AddWithValue("@adi", txtKategoriAdi.Text);
            komut.Parameters.AddWithValue("@tanim", txtTanimi.Text);
            baglanti.Open();
            int islem = komut.ExecuteNonQuery();
            baglanti.Close();
            if (islem > 0)
            {
                MessageBox.Show("Kategori başarıyla eklenmiştir...");
                KategoriListesi();
            }
            else
            {
                MessageBox.Show("Kategori eklenirken hata oluştu!");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void silToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                SqlCommand komut = new SqlCommand("proc_KategoriSil",baglanti);
                komut.CommandType = CommandType.StoredProcedure;
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["KategoriID"].Value);
                komut.Parameters.AddWithValue("@kategoriID",id);

                baglanti.Open();
                int islem = komut.ExecuteNonQuery();
                baglanti.Close();

                if (islem > 0)
                {
                    MessageBox.Show("Kategori başarıyla silinmiştir...");
                    KategoriListesi();
                }
                else
                {
                    MessageBox.Show("Kategori silinirken hata oluştu!");
                }

            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SqlCommand komut = new SqlCommand("prc_KategoriGuncelle", baglanti);
            komut.CommandType = CommandType.StoredProcedure;

            DataGridViewRow row = dataGridView1.CurrentRow;
            komut.Parameters.AddWithValue("@id", row.Cells["KategoriID"].Value);
            komut.Parameters.AddWithValue("@adi", row.Cells["KategoriAdi"].Value);
            komut.Parameters.AddWithValue("@tanim", row.Cells["Tanimi"].Value);

            baglanti.Open();
            int islem = komut.ExecuteNonQuery();
            baglanti.Close();

            if (islem > 0)
            {
                MessageBox.Show("Kategori başarıyla güncellenmiştir...");
                KategoriListesi();
            }
            else
            {
                MessageBox.Show("Kategori güncellenirken hata oluştu!");
            }
        }
    }
}
