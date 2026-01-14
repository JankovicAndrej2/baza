 private void btnSve_Click(object sender, EventArgs e)
 {
     // OVDJE SLIJEDI PRIMJER IZ PREDLOŠKA:
     SqlConnection conn = new SqlConnection("Data Source=31.147.207.14;Initial Catalog = stuslu; User ID = student; Password = student");
     conn.Open();
     string statement = "SELECT * FROM student";
     SqlDataAdapter dataAdapter = new SqlDataAdapter(statement, conn);
     DataTable dt = new DataTable();
     dataAdapter.Fill(dt);
     dgvPodaci.DataSource = dt;
     conn.Close();

 }

 private void btnTrazi_Click(object sender, EventArgs e)
 {
     // OVDJE PISATI KOD 1. ZADATKA IZ PREDLOŠKA:
     SqlConnection conn = new SqlConnection("Data Source=31.147.207.14;Initial Catalog = stuslu; User ID = student; Password = student");
     conn.Open();

     string statement = "SELECT * FROM student WHERE 1=1";

     if (txtIme.Text != "")
     {
         statement += " AND ime LIKE '" + txtIme.Text + "%'";
     }

     if (txtPrezime.Text != "")
     {
         statement += " AND prezime LIKE '" + txtPrezime.Text + "%'";
     }

     if (rbM.Checked)
     {
         statement += " AND spol = 'M'";
     }
     else if (rbZ.Checked)
     {
         statement += " AND spol = 'F'";
     }

     SqlDataAdapter dataAdapter = new SqlDataAdapter(statement, conn);
     DataTable dt = new DataTable();
     dataAdapter.Fill(dt);
     dgvPodaci.DataSource = dt;
     conn.Close();

 }
