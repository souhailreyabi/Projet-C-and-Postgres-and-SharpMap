using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
namespace Exercice
{
    public partial class Form1 : Form
    {
        static string strcnx = "server=localhost;port=5432;database=Exercice;user id=postgres;password=geoinfo";
        NpgsqlConnection cnx = new NpgsqlConnection(strcnx);
        public Form1()
        {
            InitializeComponent();
            NpgsqlCommand cmd = new NpgsqlCommand("select code from etudiant", cnx);
            NpgsqlDataAdapter ada = new NpgsqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ada.Fill(dt);
            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "code";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("select nom, st_distance('point(0 0)',adresse) as distance_au_centre from etudiant", cnx);
            NpgsqlDataAdapter ada = new NpgsqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ada.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("select nom,code from etudiant", cnx);
            NpgsqlDataAdapter ada = new NpgsqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ada.Fill(dt);
            dataGridView2.DataSource = dt;


        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

           
            NpgsqlCommand c = new NpgsqlCommand("select nom, st_distance((select adresse from etudiant where code=" + textBox1.Text + "),adresse) as DISTANCE_"++"Etudiant from etudiant", cnx);
            NpgsqlDataAdapter ad = new NpgsqlDataAdapter(c);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            dataGridView3.DataSource = dt;


        }

        private void button4_Click(object sender, EventArgs e)
        {
            NpgsqlCommand c = new NpgsqlCommand("select nom, st_distance((select adresse from etudiant where code=" + comboBox1.Text + "),adresse) as DISTANCE_Etudiant from etudiant where code!='"+comboBox1.Text+"'", cnx);
            NpgsqlDataAdapter ad = new NpgsqlDataAdapter(c);
            DataTable dt = new DataTable();
            ad.Fill(dt);
           
            dataGridView4.DataSource = dt;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
