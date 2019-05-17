using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using Npgsql;
namespace Mini_Projet1
{
    public partial class Form1 : Form
    {
        static string con = "server=localhost;port=5432;database=Exercice;user id=postgres;password=geoinfo";
        NpgsqlConnection cnx = new NpgsqlConnection(con);
        NpgsqlCommand cmd;
        NpgsqlDataAdapter adap;
        int tmp=0;
        int count = 0;

        NpgsqlCommand suprimer;
        NpgsqlCommand min;
        NpgsqlCommand t_dis;
        NpgsqlCommand t_dist;
        NpgsqlCommand mina;
        NpgsqlCommand tt_distance;
        double distance_total=0;
        double local = 0;

        public Form1()
        {
            InitializeComponent(); cnx.Open();
            cmd = new NpgsqlCommand("Select nom,code from etudiant", cnx);
            adap = new NpgsqlDataAdapter(cmd); ;
            DataTable dt = new DataTable();
            adap.Fill(dt);
            dataGridView1.DataSource = dt;
            comboBox1.DisplayMember = "nom";
            comboBox1.DataSource = dt;
            comboBox1.ValueMember = "code";
            mina = new NpgsqlCommand("select count(*) from etudiant", cnx);
            NpgsqlDataReader pgR = mina.ExecuteReader();
            while (pgR.Read())
            {
                Int32.TryParse(pgR.GetInt32(0).ToString(), out count);
            }
            pgR.Close();
           


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            ArrayList Ma_liste = new ArrayList();
            int code;
            Int32.TryParse(comboBox1.SelectedValue.ToString(), out code);
            Min_dist(Ma_liste, code, count);


          if (distance_total != 0)
            {
                
                listView1.Items.Add(" Point  depart:");
                foreach (int c in Ma_liste)
            {
                
                listView1.Items.Add(c.ToString()); listView1.Items.Add("->");
            }


                listView1.Items.Add("\n");
                listView1.Items.Add("La distance total :");
                listView1.Items.Add(distance_total.ToString());
            }
            textBox1.Clear();
            textBox1.AppendText((distance_total.ToString()));

            listView1.Items.Add(" \n ");
            listView1.Items.Add(" \n ");
            listView1.Items.Add(" \n ");
            distance_total = 0;


        }
        public void Min_dist(ArrayList liste, int code, int count)
        {
            cnx.Close();
            cnx.Open();

            if (code != 0)
            {


                
                if (code != tmp)
                {
                    
                    try
                    {
                        t_dist = new NpgsqlCommand("create table dist as select code,st_distance((select adresse from etudiant where code=" + code + "),adresse) as dist from etudiant where code!="+tmp+";", cnx);
                        t_dist.ExecuteNonQuery();

                    }
                    catch (Exception e)
                    {

                    }
                    finally
                    {
                            liste.Add(code);
                        tmp = code;
                        foreach(int d in liste) { 
                        suprimer = new NpgsqlCommand("delete from  dist where code=" + d + "", cnx);
                        suprimer.ExecuteNonQuery();
                        }

                        min = new NpgsqlCommand("select code from dist where dist=(select MIN(dist) from dist)", cnx);
                        
                        NpgsqlDataReader pgReader = min.ExecuteReader();
                        while (pgReader.Read())
                        {
                            Int32.TryParse(pgReader.GetInt32(0).ToString(), out code);

                            

                        }
                        pgReader.Close();

                        tt_distance = new NpgsqlCommand("select dist from dist where dist=(select MIN(dist) from dist)", cnx);
                        NpgsqlDataReader pg = tt_distance.ExecuteReader();
                        while (pg.Read())
                        {
                            Double.TryParse(pg.GetDouble(0).ToString(), out local);
                            distance_total = distance_total + local;



                        }
                        pg.Close();
                        if (count > 0)
                        {
                            t_dis = new NpgsqlCommand("drop table dist ", cnx);
                            t_dis.ExecuteNonQuery();
                            Min_dist(liste, code, count - 1);
                            cnx.Close();
                            


                        }
                    }





                }
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }

}