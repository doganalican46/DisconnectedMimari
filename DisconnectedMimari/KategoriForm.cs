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
            SqlDataAdapter adp = new SqlDataAdapter("select * from Kategoriler", baglanti);

            DataTable dt = new DataTable(); //dataadapter ın elde ettiği tabloyu tutar

            adp.Fill(dt); //bağlantıyı otomatik açar ve kapatır (sadece select işleminde geçerli update ve insert için elle açıp-kapatmamız lazım)

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["KategoriID"].Visible = false;
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string kategoriAdi = txtKategoriAdi.Text;
            string tanimi = txtTanimi.Text;


            if (kategoriAdi == "" || tanimi == "")
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!");
                return;
            }

            SqlCommand komut = new SqlCommand();
            komut.CommandText = "INSERT INTO Kategoriler (KategoriAdi, Tanimi) VALUES (@kategoriAdi, @tanimi)";
            komut.Parameters.AddWithValue("@kategoriAdi", kategoriAdi);
            komut.Parameters.AddWithValue("@tanimi", tanimi);
            komut.Connection = baglanti;
            baglanti.Open();
            int islem = komut.ExecuteNonQuery();//insert update delete işlemlerinde kullanılan execute methodu
            if (islem > 0)
            {
                MessageBox.Show("Kayıt başarılı bir şekilde eklenmiştir...");
                KategoriListesi();
            }
            else
            {
                MessageBox.Show("Kayıt ekleme sırasında hata oluştu...");
            }

            baglanti.Close();
        }
    }
}
