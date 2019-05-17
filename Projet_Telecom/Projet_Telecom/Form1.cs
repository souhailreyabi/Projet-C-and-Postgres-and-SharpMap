using Npgsql;
using SharpMap.Layers;
using SharpMap.Styles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projet_Telecom
{
    public partial class Form1 : Form
    {
        static string strcnx = "server=localhost;port=5432;database=Telecom;user id=postgres;password=geoinfo";
        NpgsqlConnection cnx = new NpgsqlConnection(strcnx);
        NpgsqlCommand cmd, cmd1;
        NpgsqlDataAdapter ada;
        VectorLayer quartier, antenne, Selectcouche;
        VectorStyle styleall, stylequart, styleant, stylecouche;
        DataTable dt, db;


        public Form1()
        {
            InitializeComponent();
        }

        private void mapBox1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
