using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Baze_LV5_predlozak
{


    public partial class Form1 : Form
    {

        
        private static string SQLSelect = "SELECT * FROM osobe ORDER BY prezime";

        private static string SQLInsert = "INSERT INTO osobe (oib, ime, prezime, dat_rod, spol, visina, broj_cipela) " +
                                          "VALUES (@oib, @ime, @prezime, @datum, @spol, @visina, @brCip)";

        private static string SQLUpdate = "UPDATE osobe SET ime=@ime, prezime=@prezime, dat_rod=@datum, spol=@spol, visina=@visina, broj_cipela=@brCip WHERE oib=@oib";

        private static string SQLDelete = "DELETE FROM osobe WHERE oib=@oib";




        public Form1()
        {
            InitializeComponent();
            btnDelete.Enabled = false;
        }
        private DBStudent Dbs;

        private void btnSve_Click(object sender, EventArgs e)
        {
            // NE MIJENJATI OVAJ KOD

            
            if (Dbs == null)
            {
                using (FormLogin wl = new FormLogin())  
                {
                    wl.ShowDialog();
                    wl.Focus();
                    Dbs = new DBStudent(wl.Pwd);        
                                                        
                    if (string.IsNullOrWhiteSpace(wl.Pwd))
                        return;
                }
            }

            using (SqlConnection conn = Dbs.GetConnection())
            {
                
                LoadOsobe(conn);

                if (dgvPodaci.Rows.Count > 0)
                    dgvPodaci.Rows[0].Selected = false;
            }

        }

        private void btnSpremi_Click(object sender, EventArgs e)
        {
            if (Dbs == null)
                return;

            using (SqlConnection conn = Dbs.GetConnection())
            {
                conn.Open();

              

                int visina = 0;
                double tempVisina;
                
                if (double.TryParse(txtVisina.Text.Replace('.', ','), out tempVisina))
                {
                    visina = (int)tempVisina;
                }

                int brCipela = 0;
                double tempBrCip;
                if (double.TryParse(txtBrCip.Text.Replace('.', ','), out tempBrCip))
                {
                    brCipela = (int)tempBrCip;
                }

                
                DateTime datum;
                if (!DateTime.TryParse(txtDatum.Text, out datum))
                {
                    
                    try { datum = DateTime.Parse(txtDatum.Text); }
                    catch { datum = DateTime.Now; } 
                }

                // --- ZADATAK 2: UNOS NOVE OSOBE ---
                if (!txtOIB.ReadOnly)
                {
                    SqlCommand cmd = new SqlCommand(SQLInsert, conn);
                    cmd.Parameters.AddWithValue("@oib", txtOIB.Text);
                    cmd.Parameters.AddWithValue("@ime", txtIme.Text);
                    cmd.Parameters.AddWithValue("@prezime", txtPrezime.Text);
                    cmd.Parameters.AddWithValue("@datum", datum);
                    cmd.Parameters.AddWithValue("@visina", visina);
                    cmd.Parameters.AddWithValue("@brCip", brCipela);

                    if (rbM.Checked) cmd.Parameters.AddWithValue("@spol", "M");
                    else cmd.Parameters.AddWithValue("@spol", "F");

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Nova osoba uspješno unesena!");
                    obrisiSve();
                }
                // --- ZADATAK 3: AŽURIRANJE POSTOJEĆE OSOBE ---
                else
                {
                    SqlCommand cmd = new SqlCommand(SQLUpdate, conn);
                    cmd.Parameters.AddWithValue("@oib", txtOIB.Text);
                    cmd.Parameters.AddWithValue("@ime", txtIme.Text);
                    cmd.Parameters.AddWithValue("@prezime", txtPrezime.Text);
                    cmd.Parameters.AddWithValue("@datum", datum);
                    cmd.Parameters.AddWithValue("@visina", visina);
                    cmd.Parameters.AddWithValue("@brCip", brCipela);

                    if (rbM.Checked) cmd.Parameters.AddWithValue("@spol", "M");
                    else cmd.Parameters.AddWithValue("@spol", "F");

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Podaci uspješno ažurirani!");
                    obrisiSve();
                }

                // NE MIJENJATI KOD ISPOD
                LoadOsobe(conn);
                SelectCurrentRow();
            }
        }

        
        private void SelectCurrentRow()
        {
            int selectedIndex = -1;

            dgvPodaci.ClearSelection();
            if (string.IsNullOrEmpty(txtOIB.Text) && dgvPodaci.Rows.Count > 0)
                selectedIndex = 0;
            else
            {
                foreach (DataGridViewRow row in dgvPodaci.Rows)
                {
                    if (row.Cells[0].Value.ToString().Trim().Equals(txtOIB.Text.Trim()))
                    {
                        selectedIndex = row.Index;
                        break;
                    }
                }
            }
            if (selectedIndex > -1)
            {
                dgvPodaci.Rows[selectedIndex].Selected = true;
                txtOIB.ReadOnly = true;
                btnDelete.Enabled = true;

            }
        }

        public void obrisiSve()
        {
            txtOIB.Text = "";
            txtIme.Text = "";
            txtPrezime.Text = "";
            txtDatum.Text = "";
            txtBrCip.Text = "";
            txtVisina.Text = "";
            dgvPodaci.ClearSelection();
            txtOIB.ReadOnly = false;
            btnDelete.Enabled = false;
        }

        private void btnObrisi_Click(object sender, EventArgs e)
        {
            obrisiSve();
        }

        private void dgvPodaci_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtIme.Text = dgvPodaci.SelectedRows[0].Cells[1].Value.ToString();
            txtPrezime.Text = dgvPodaci.SelectedRows[0].Cells[2].Value.ToString();
            txtOIB.Text = dgvPodaci.SelectedRows[0].Cells[0].Value.ToString();
            txtDatum.Text = dgvPodaci.SelectedRows[0].Cells[4].Value.ToString();

            if (dgvPodaci.SelectedRows[0].Cells[3].Value.ToString() == "M")
                rbM.Checked = true;
            else
                rbZ.Checked = true;

            string valVisina = dgvPodaci.SelectedRows[0].Cells[5].Value.ToString();
            txtVisina.Text = ((int)double.Parse(valVisina)).ToString();

            string valCipela = dgvPodaci.SelectedRows[0].Cells[6].Value.ToString();
            txtBrCip.Text = ((int)double.Parse(valCipela)).ToString();

            txtOIB.ReadOnly = true;
            btnDelete.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = Dbs.GetConnection())
            {
                conn.Open();

                
                SqlCommand cmd = new SqlCommand(SQLDelete, conn);
                cmd.Parameters.AddWithValue("@oib", txtOIB.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Osoba je obrisana.");

                // -----------------------------------

                // NE MIJENJATI KOD ISPOD
                LoadOsobe(conn);
                // Provjera da se ne sruši ako je tablica prazna nakon brisanja
                if (dgvPodaci.Rows.Count > 0)
                    dgvPodaci.Rows[0].Selected = false;
            }

            btnDelete.Enabled = false;
            obrisiSve();
        }

        private void LoadOsobe(SqlConnection conn)
        {
            // 1a - OVDJE KORISTITE DATA ADAPTER 
            
            SqlDataAdapter da = new SqlDataAdapter(SQLSelect, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvPodaci.DataSource = dt;
        }

        private void dgvPodaci_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }


}
