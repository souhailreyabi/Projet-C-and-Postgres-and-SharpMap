using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Styles;
using np = Npgsql;

namespace Mini_Projet2
{
    public partial class Form1 : Form
    {
        String srx = "server=localhost;port=5432;database=sharpmapM;user=postgres;pwd=geoinfo";
        VectorLayer Cquartier, Cantenne, CoucheSelected,bufferselect;
        np.NpgsqlConnection cnx;

        private void mp_MouseDown(GeoAPI.Geometries.Coordinate WorldPos, MouseEventArgs ImagePos)
        {
            FeatureDataSet selected = new FeatureDataSet();
            Envelope boundingBox = new Envelope(WorldPos);
            boundingBox.ExpandBy(0.01);
            FeatureDataSet sel = new FeatureDataSet();
            Envelope b = new Envelope(WorldPos);
            b.ExpandBy(1);
            Cquartier.DataSource.ExecuteIntersectionQuery(boundingBox, selected);
            Cantenne.DataSource.ExecuteIntersectionQuery(b, sel);

            

            if (sel.Tables[0].Count == 0) return;
            Text = sel.Tables[0].Rows[0]["num"].ToString();
            this.CoucheSelected.DataSource = new GeometryProvider(sel.Tables[0]);
            mapImage1.Map.Layers.Add(this.CoucheSelected);
            buffer_select(Int32.Parse(Text));
            mapImage1.Refresh();
        }

      

        public Form1()
        {
            InitializeComponent();
            cnx = new np.NpgsqlConnection(srx);
            np.NpgsqlCommand cmd = new np.NpgsqlCommand("SELECT code, nom FROM public.quartier;", cnx);
            np.NpgsqlDataAdapter adapter = new np.NpgsqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            grd.DataSource = dt;
            np.NpgsqlCommand cm = new np.NpgsqlCommand("SELECT num,nom,puissance FROM antenne;", cnx);
            np.NpgsqlDataAdapter adap = new np.NpgsqlDataAdapter(cm);
            DataTable dta = new DataTable();
            adap.Fill(dta);
            grd1.DataSource = dta;

            Cquartier = new VectorLayer("Quartier")
            {
                DataSource = new PostGIS(srx, "quartier", "zone", "code")

            };

            Cantenne = new VectorLayer("antenne")
            {
                DataSource = new PostGIS(srx,"antenne", "emp", "num")

            };
         
            CoucheSelected = new VectorLayer("s");
            VectorStyle styleAll = new VectorStyle
            {
                Fill = Brushes.GreenYellow,
                Outline = Pens.Black,
                EnableOutline = true
            };
            VectorStyle pointStyle = new VectorStyle
            {
                Fill = Brushes.White,
                Outline = Pens.White,
                Symbol = new System.Drawing.Bitmap("C:/Users/RET/source/repos/Mini_Projet2/src/wifi-signal.png"),
                EnableOutline = true
            };
            VectorStyle styleSelected = new VectorStyle
            {
                Fill = Brushes.Yellow,
                Outline = Pens.Blue,
                EnableOutline = true
            };
            VectorStyle bufferstyle = new VectorStyle
            {
                Fill = Brushes.Yellow,
                Outline = Pens.Blue,
                EnableOutline = true
            };
            CoucheSelected.Style = styleSelected;
            Cquartier.Style = styleAll;
            Cantenne.Style = pointStyle;
            mapImage1.Map.Layers.Add(Cquartier);
            mapImage1.Map.Layers.Add(Cantenne);
            mapImage1.Map.ZoomToExtents();
            mapImage1.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public void buffer_select(int num)
        {
            cnx.Close();
            cnx.Open();
            np.NpgsqlCommand cm = new np.NpgsqlCommand("create or replace view bf_t as Select num,st_buffer(emp,puissance) as bf from antenne where num="+num+"",cnx);
            cm.ExecuteNonQuery();
            VectorLayer vc = new VectorLayer("buffer")
            {
                DataSource = new PostGIS(srx,"bf_t", "bf", "num")
             };
            VectorStyle styleSelected = new VectorStyle
            {
                Fill = Brushes.Transparent,

                Outline = Pens.Blue,
                EnableOutline = true
            };
            vc.Style = styleSelected;
            mapImage1.Map.Layers.Add(vc);
            cnx.Close();
        }
    }
}
