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

        //global connection string her yerde geçerli
        SqlConnection baglanti = new SqlConnection("Server=.\\SQLEXPRESS;Database=KuzeyYeli;Integrated Security=True;");


        private void Form1_Load(object sender, EventArgs e)
        {
            UrunListesi();
        }

        private void UrunListesi()
        {
            SqlDataAdapter adp = new SqlDataAdapter("select * from Urunler where Sonlandi=0", baglanti);

            DataTable dt = new DataTable(); //dataadapter ın elde ettiği tabloyu tutar

            adp.Fill(dt);

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["UrunID"].Visible = false;
            dataGridView1.Columns["KategoriID"].Visible = false;
            dataGridView1.Columns["TedarikciID"].Visible = false;
        }

        private bool IsDuplicateUrunAdi(string urunAdi)
        {
            using (SqlCommand komut = new SqlCommand())
            {
                komut.CommandText = "SELECT COUNT(*) FROM Urunler WHERE UrunAdi = @UrunAdi";
                komut.Connection = baglanti;
                komut.Parameters.AddWithValue("@UrunAdi", urunAdi);

                baglanti.Open();
                int count = (int)komut.ExecuteScalar();
                baglanti.Close();

                return count > 0;
            }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string adi = txtUrunAdi.Text;
            decimal fiyat = txtFiyat.Value;
            decimal stok = txtStok.Value;

            if (string.IsNullOrEmpty(adi) || fiyat == 0 || stok == 0)
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!");
                return;
            }

            if (IsDuplicateUrunAdi(adi))
            {
                MessageBox.Show("Bu isimde bir ürün zaten var. Lütfen farklı bir isim seçiniz.");
                return;
            }

            using (SqlCommand komut = new SqlCommand())
            {
                komut.CommandText = "INSERT INTO Urunler (UrunAdi, Fiyat, Stok) VALUES (@UrunAdi, @Fiyat, @Stok)";
                komut.Connection = baglanti;

                // Set parameters
                komut.Parameters.AddWithValue("@UrunAdi", adi);
                komut.Parameters.AddWithValue("@Fiyat", fiyat);
                komut.Parameters.AddWithValue("@Stok", stok);

                baglanti.Open();
                int islem = komut.ExecuteNonQuery();

                if (islem > 0)
                {
                    MessageBox.Show("Kayıt başarılı bir şekilde eklenmiştir...");
                    UrunListesi();
                }
                else
                {
                    MessageBox.Show("Kayıt ekleme sırasında hata oluştu...");
                }
            }

            baglanti.Close();
        }

        private void btnKategoriler_Click(object sender, EventArgs e)
        {
            KategoriForm kf = new KategoriForm();
            kf.ShowDialog();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //datagridview dan seçili satırı alma işlemi
            txtUrunAdi.Text = dataGridView1.CurrentRow.Cells["UrunAdi"].Value.ToString();

            // Check and set the Fiyat value
            decimal fiyatValue = (decimal)dataGridView1.CurrentRow.Cells["Fiyat"].Value;
            txtFiyat.Minimum = decimal.MinValue; // Set the minimum value to the minimum possible decimal value
            txtFiyat.Maximum = decimal.MaxValue; // Set the maximum value to the maximum possible decimal value
            txtFiyat.Value = fiyatValue;

            // Check and set the Stok value
            decimal stokValue = Convert.ToDecimal(dataGridView1.CurrentRow.Cells["Stok"].Value);
            txtStok.Minimum = decimal.MinValue; // Set the minimum value to the minimum possible decimal value
            txtStok.Maximum = decimal.MaxValue; // Set the maximum value to the maximum possible decimal value
            txtStok.Value = stokValue;

            txtUrunAdi.Tag = dataGridView1.CurrentRow.Cells["UrunId"].Value; //hangi ürünün güncelleneceğini bulabilmek için
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            using (SqlCommand komut = new SqlCommand())
            {
                komut.CommandText = "update Urunler set UrunAdi=@UrunAdi, Fiyat=@Fiyat, Stok=@Stok where UrunID=@UrunID";
                komut.Connection = baglanti;

                // Set parameters
                komut.Parameters.AddWithValue("@UrunAdi", txtUrunAdi.Text);
                komut.Parameters.AddWithValue("@Fiyat", txtFiyat.Value);
                komut.Parameters.AddWithValue("@Stok", txtStok.Value);
                komut.Parameters.AddWithValue("@UrunID", txtUrunAdi.Tag);

                baglanti.Open();

                //try catch sql tarafında bi hata oluştuğu zaman onu yakalaması içindir.
                try
                {
                    int islem = komut.ExecuteNonQuery();
                    baglanti.Close();
                    if (islem > 0)
                    {
                        MessageBox.Show("Kayıt Güncellendi...");
                        UrunListesi();
                    }
                    else
                    {
                        MessageBox.Show("Kayıt Güncellenirken bir hata oluştu...");
                    }
                }
                catch (Exception ex)
                {
                    baglanti.Close();
                    MessageBox.Show("Kayıt Güncellenirken bir hata oluştu..." + ex.Message);
                }
            }
        }

        private void silToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null) //herhangi bir seçili satır varsa
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["UrunID"].Value);

                using (SqlCommand cmd = new SqlCommand("DELETE FROM Urunler WHERE UrunID = @UrunID", baglanti))
                {
                    cmd.Parameters.AddWithValue("@UrunID", id);
                    baglanti.Open();
                    int islem = cmd.ExecuteNonQuery();
                    baglanti.Close();

                    if (islem > 0)
                    {
                        MessageBox.Show("Kayıt silinmiştir!");
                        UrunListesi();
                    }
                    else
                    {
                        MessageBox.Show("Kayıt silinirken bir hata oluştu...");
                    }
                }
            }
        }

    }
}
