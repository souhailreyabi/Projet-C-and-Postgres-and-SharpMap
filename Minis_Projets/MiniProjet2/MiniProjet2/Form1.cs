using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpMap.Styles;
using n = Npgsql;
using SharpMap.Layers;
using SharpMap.Data.Providers;
using SharpMap.Data;
using GeoAPI.Geometries;
using System.Collections;

namespace MiniProjet2
{
    public partial class Form1 : Form
    {
        // La phase de declaration Globale 
        string src = "server=localhost;port=5432;database=sharpmapM;user=postgres;pwd=geoinfo";
        n.NpgsqlConnection cnx;
        n.NpgsqlCommand cmd,cmd1;
        n.NpgsqlDataAdapter ada;
        VectorLayer quart,ant,Selectcouche;
        VectorStyle styleall,styleant,couche;
        LabelLayer lbanteene,lbquartie;
        LabelStyle alllabel;
        DataTable dt,db;
        
        public Form1()
        {
            InitializeComponent();
            cnx = new n.NpgsqlConnection(src);
            db = new DataTable();

            Selectcouche = new VectorLayer("Select");

            db.Columns.Add("nom", typeof(String));
            DataRow dx = db.NewRow();
            dx["nom"] = "Veuillez Selectionner *_* ";
            db.Rows.Add(dx);
            DataRow dr = db.NewRow();
            dr["nom"] = "Quartiers";
            db.Rows.Add(dr);
            DataRow drr = db.NewRow();
            drr["nom"] = "Antennes";
            db.Rows.Add(drr);
            comboBox1.DataSource = db;
            comboBox1.ValueMember = "nom";
            comboBox1.DisplayMember = "nom";
            quart = new VectorLayer("qrt")
            {
                DataSource = new PostGIS(src,"quartier","zone","code")
            };
            ant = new VectorLayer("ant")
            {

                DataSource = new PostGIS(src, "antenne", "emp", "num")
            };
            styleall = new VectorStyle()
            {
                Fill = Brushes.Beige,
                Outline=Pens.Black,
                EnableOutline=true
            };
            couche = new VectorStyle()
            {
                Fill = Brushes.MediumBlue,
                Outline = Pens.Red,
                EnableOutline = true
            };
            styleant = new VectorStyle()
            {
                Fill = Brushes.White,
                Outline = Pens.White,
                Symbol = new System.Drawing.Bitmap("C:/Users/RET/source/repos/MiniProjet2/src/antenna.png"),
                EnableOutline = true
            };
            ant.Style = styleant;
            quart.Style = styleall;
            map1.Map.Layers.Add(quart);
            map1.Map.Layers.Add(ant);
            map1.Map.ZoomToExtents();
            map1.Refresh();
            cnx.Open();
            n.NpgsqlCommand nca = new n.NpgsqlCommand(" select SUM(ST_Area(quartier.zone)) as valeur  from bf_t,quartier ", cnx);
            double ira = 0;
            var ra = nca.ExecuteReader();
            while (ra.Read())
            {
                ira = Double.Parse(ra["valeur"].ToString());

            }
            all.Text = ira  + "";
            n.NpgsqlCommand c = new n.NpgsqlCommand(" select ST_AREA(ST_Union(zone)) as valeur from quartier", cnx);
            double i = 0;
            var rea = c.ExecuteReader();
            while (rea.Read())
            {
                i = Double.Parse(rea["valeur"].ToString());

            }
            n.NpgsqlCommand cq = new n.NpgsqlCommand(" create or replace view pop_world as select (densite*ST_AREA(zone)) as valeur from quartier group by code;", cnx);
            cq.ExecuteNonQuery();
            n.NpgsqlCommand ca = new n.NpgsqlCommand("select SUM(valeur) as valeur from pop_world;", cnx);
            double iza = 0;
            var re = ca.ExecuteReader();
            while (re.Read())
            {
                iza = Double.Parse(re["valeur"].ToString());

            }
            Pv.Text = (iza)+ "";
            n.NpgsqlCommand caa = new n.NpgsqlCommand(" select ST_AREA(ST_INTERSECTION(ST_UNION(st_buffer(antenne.emp,puissance)),ST_UNION(quartier.zone))) as valeur from antenne,quartier;", cnx);
            double izaa = 0;
            var reaa = caa.ExecuteReader();
            while (reaa.Read())
            {
                izaa = Double.Parse(reaa["valeur"].ToString());

            }
            scg.Text = izaa + "";
            sncall.Text = (ira - izaa) + "";
            n.NpgsqlCommand sh = new n.NpgsqlCommand("create or replace view buffer_a as select num,St_Intersection(ST_buffer(emp,puissance),quartier.zone) as bf,code,densite  from antenne,quartier;", cnx);
                sh.ExecuteNonQuery();
            n.NpgsqlCommand shh =new  n.NpgsqlCommand("create or replace view mex as select SUM(ST_AREA(bf)*densite)as maxi,num from buffer_a group by num", cnx);
            shh.ExecuteNonQuery();
            n.NpgsqlCommand shhh = new n.NpgsqlCommand("select num  as valeur from mex where  maxi=(select MAX(maxi) from mex)", cnx);
            double izaaa = 0;
            var reaaa= shhh.ExecuteReader();
            while (reaaa.Read())
            {
                izaaa = Double.Parse(reaaa["valeur"].ToString());

            }

            Lc.Text = izaaa + "";
            n.NpgsqlCommand don = new n.NpgsqlCommand("create or replace view popall as select ST_INTERSECTION(ST_UNION(st_buffer(antenne.emp,puissance)),ST_UNION(quartier.zone)) as zona ,densite from antenne,quartier group by quartier.code;", cnx);
            don.ExecuteNonQuery();
            n.NpgsqlCommand shhhh = new n.NpgsqlCommand(" select sum(ST_AREA(zona)*densite) as valeur from popall", cnx);
            double izaaaa = 0;
            var reaaaa = shhhh.ExecuteReader();
            while (reaaaa.Read())
            {
                izaaaa = Double.Parse(reaaaa["valeur"].ToString());

            }

           pcam.Text = izaaaa + "";
            pncc.Text = (iza-izaaaa) + "";

            cnx.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void map1_Click(object sender, EventArgs e)
        {

        }

        private void mp_down(GeoAPI.Geometries.Coordinate WorldPos, MouseEventArgs ImagePos)
        {
            FeatureDataSet selected = new FeatureDataSet();
            Envelope boundingBox = new Envelope(WorldPos);
            boundingBox.ExpandBy(0.01);
            FeatureDataSet sel = new FeatureDataSet();
            Envelope b = new Envelope(WorldPos);
            b.ExpandBy(1);
            quart.DataSource.ExecuteIntersectionQuery(boundingBox, selected);
            ant.DataSource.ExecuteIntersectionQuery(b,sel);


            if (sel.Tables[0].Count != 0)
            {
                
                Text = sel.Tables[0].Rows[0]["num"].ToString();
                this.Selectcouche.DataSource = new GeometryProvider(sel.Tables[0]);
                map1.Map.Layers.Add(this.Selectcouche);
                buffer_select(Int32.Parse(Text));
               Zonecouverte(Int32.Parse(Text));
                
                alabel.Text = Text;
                c.Text ="";
                Nantenne.Text = selected.Tables[0].Rows[0]["nom"].ToString();
                puissance.Text= sel.Tables[0].Rows[0]["puissance"].ToString();
                tc.Text ="";
                supc.Text = "";
                supnc.Text = "";
                pc.Text = "";
                pnc.Text = "";

            }
            else
            {     
                if(selected.Tables[0].Count!=0)
            {
                    map1.Map.Layers.Clear();
                    map1.Map.Layers.Add(quart);
                    map1.Map.Layers.Add(ant);
                    map1.Map.ZoomToExtents();
                    map1.Refresh();
                    Text = selected.Tables[0].Rows[0]["nom"].ToString();
                    string commo= selected.Tables[0].Rows[0]["code"].ToString();
                    this.Selectcouche.DataSource = new GeometryProvider(selected.Tables[0]);
                    Selectcouche.Style = couche;
                   map1.Map.Layers.Add(this.Selectcouche);
                    c.Text = Text;
                    alabel.Text = "";
                    Nantenne.Text = "";
                    puissance.Text = "";
                    s.Text = "";
                    densite.Text = "";
                    nbrZone.Text = "";
                    NC.Text = "";
                    NCC.Text = "";
                    quartier_traitement(Int32.Parse(commo));




                }
                else
                {
                    return;
                }
            }
            
            
            
            
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void Nantenne_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void s_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox1.SelectedValue.ToString() == "Quartiers")
            {
                grd.ClearSelection();
                cmd = new n.NpgsqlCommand("select code,nom,densite from quartier", cnx);
                ada = new n.NpgsqlDataAdapter(cmd);
                dt = new DataTable();
                ada.Fill(dt);
                grd.DataSource = dt;
            }
            else
            {
                if (comboBox1.SelectedValue.ToString() == "Antennes")
                {
                    grd.ClearSelection();
                    cmd = new n.NpgsqlCommand("select num,nom,puissance from antenne", cnx);
                    ada = new n.NpgsqlDataAdapter(cmd);
                    dt = new DataTable();
                    ada.Fill(dt);
                    grd.DataSource = dt;
                }
                else
                {


                    dt = new DataTable();

                    grd.DataSource = dt;


                }
            }
        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void scg_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            alabel.Text = "";
            c.Text = "";
            tc.Text = "";
            supc.Text = "";
            supnc.Text = "";
            pc.Text = "";
            pnc.Text = "";
            c.Text = Text;
            alabel.Text = "";
            Nantenne.Text = "";
            puissance.Text = "";
            s.Text = "";
            densite.Text = "";
            nbrZone.Text = "";
            NC.Text = "";
            NCC.Text = "";
            map1.Map.Layers.Clear();
            map1.Map.Layers.Add(quart);
            map1.Map.Layers.Add(ant);
            map1.Map.ZoomToExtents();
            map1.Refresh();
            c.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            alabel.Text = "";
            c.Text = "";
            tc.Text = "";
            supc.Text = "";
            supnc.Text = "";
            pc.Text = "";
            pnc.Text = "";
            c.Text = Text;
            alabel.Text = "";
            Nantenne.Text = "";
            puissance.Text = "";
            s.Text = "";
            densite.Text = "";
            nbrZone.Text = "";
            NC.Text = "";
            NCC.Text = "";
            cnx.Close();
            cnx.Open();
            n.NpgsqlCommand caa = new n.NpgsqlCommand(" Create or replace view zonec as select ST_INTERSECTION(ST_UNION(st_buffer(antenne.emp,puissance)),ST_UNION(quartier.zone)) as valeur from antenne,quartier;", cnx);
            caa.ExecuteNonQuery();

            VectorLayer nc = new VectorLayer("")
            {
                DataSource=new PostGIS(src,"zonec","valeur",null)
            };
            nc.Style.Fill = Brushes.Black;
            nc.Style.Outline = Pens.White;
            nc.Style.EnableOutline = true;
            map1.Map.Layers.Clear();
            map1.Map.Layers.Add(quart);
            map1.Map.Layers.Add(ant);
            map1.Map.Layers.Add(nc);
            map1.Map.ZoomToExtents();
            map1.Refresh();
            cnx.Close();
            c.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            alabel.Text = "";
            c.Text = "";
            tc.Text = "";
            supc.Text = "";
            supnc.Text = "";
            pc.Text = "";
            pnc.Text = "";
            c.Text = Text;
            alabel.Text = "";
            Nantenne.Text = "";
            puissance.Text = "";
            s.Text = "";
            densite.Text = "";
            nbrZone.Text = "";
            NC.Text = "";
            NCC.Text = "";
            cnx.Close();
            cnx.Open();
            n.NpgsqlCommand caa = new n.NpgsqlCommand(" Create or replace view zonica as select ST_Difference(ST_UNION(quartier.zone),ST_UNION(st_buffer(antenne.emp,puissance))) as valeur from antenne,quartier;", cnx);
            caa.ExecuteNonQuery();

            VectorLayer nc = new VectorLayer("")
            {
                DataSource = new PostGIS(src, "zonica", "valeur", null)
            };
            nc.Style.Fill = Brushes.Red;
            nc.Style.Outline = Pens.White;
            nc.Style.EnableOutline = true;
            map1.Map.Layers.Clear();
            map1.Map.Layers.Add(quart);
            map1.Map.Layers.Add(ant);
            map1.Map.Layers.Add(nc);
            map1.Map.ZoomToExtents();
            map1.Refresh();
            cnx.Close();
            c.Text = "";
        }

        private void Pv_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public void labeling(int code,string type) {
            cnx.Close();
            cnx.Open();
            alllabel = new LabelStyle()
            {
                CollisionDetection = true,
                CollisionBuffer = new SizeF(10F, 10F),
                Font = new Font(FontFamily.GenericSansSerif, 15),
                Halo = new Pen(Color.White, 2)
        };
            
            if (type == "antenne")
            {
                cmd = new n.NpgsqlCommand("create or replace view labelantenne as select num,puissance,emp from antenne where num="+code+";",cnx);
                cmd.ExecuteNonQuery();
                lbanteene = new LabelLayer("lb")
                {
                    LabelColumn="puissance",
                    DataSource=new PostGIS(src, "labelantenne", "emp", "num")
                };
                lbanteene.Style = alllabel;
                map1.Map.Layers.Add(lbanteene);
                map1.Refresh();
                cmd = new n.NpgsqlCommand("drop view labelantenne;",cnx);
                cmd.ExecuteNonQuery();
            }
            else
            {
                if (type == "quartier")
                {
                    cmd = new n.NpgsqlCommand("create or replace view labelquartier as select code,nom,zone from quartier where code=" + code + ";",cnx);
                    cmd.ExecuteNonQuery();
                    lbquartie= new LabelLayer("qrtt")
                    {
                        LabelColumn = "nom",
                        DataSource = new PostGIS(src, "labelquartier", "zone", "code")
                        
                    };
                    lbquartie.Style = alllabel;
                    map1.Map.Layers.Add(lbquartie);
                    map1.Refresh();
                    


                    cmd = new n.NpgsqlCommand("drop view labelquartier;",cnx);
                    cmd.ExecuteNonQuery();
                }

            }
            cnx.Close();
               }
        public void buffer_select(int num)
        {
            cnx.Close();
            cnx.Open();
            n.NpgsqlCommand cm = new n.NpgsqlCommand("create or replace view bf_t as Select num,st_buffer(emp,puissance) as bf from antenne where num=" + num + "", cnx);
            cm.ExecuteNonQuery();
            VectorLayer vc = new VectorLayer("buffer")
            {
                DataSource = new PostGIS(src, "bf_t", "bf", "num")
            };
            VectorStyle styleSelected = new VectorStyle
            {
                Fill = Brushes.Transparent,
                Symbol = new System.Drawing.Bitmap("C:/Users/RET/source/repos/MiniProjet2/src/antenna.png"),
                PointColor=Brushes.Black,
                Outline = Pens.Blue,
                EnableOutline = true
            };
            vc.Style = styleSelected;
            map1.Map.Layers.Add(vc);
            
            cnx.Close();
        }
        public void Zonecouverte(int num)
        {
            
            cnx.Close();
            cnx.Open();
            n.NpgsqlCommand cm = new n.NpgsqlCommand("create or replace view bf_t as Select num,st_buffer(emp,puissance) as bf from antenne where num=" + num + "", cnx);
            cm.ExecuteNonQuery();
            n.NpgsqlCommand cma = new n.NpgsqlCommand("CREATE OR REPLACE VIEW dist as select ST_INTERSECTION(bf_t.bf,quartier.zone) as valeur from bf_t,quartier where ST_INTERSECTS(bf_t.bf,quartier.zone)", cnx);
            cma.ExecuteNonQuery();
            n.NpgsqlCommand sauvegarde = new n.NpgsqlCommand("CREATE OR REPLACE VIEW nix as select ST_UNION(dist.valeur) as geom, count(valeur) as nbr  from dist", cnx);
            sauvegarde.ExecuteNonQuery();
        
            VectorLayer vc = new VectorLayer("couverte")
            {
                DataSource = new PostGIS(src, "nix", "geom", null)
            };
            VectorStyle styleSelected = new VectorStyle
            {
                Fill = Brushes.Red,
                PointColor = Brushes.Black,
               
                EnableOutline = true
            };
            n.NpgsqlCommand nbro = new n.NpgsqlCommand("select  nbr  from nix", cnx);
            int id = 0;
            var reader = nbro.ExecuteReader();
            while (reader.Read())
            {
                 id = Int32.Parse(reader["nbr"].ToString());
                nbrZone.Text = id + "";
            }
            n.NpgsqlCommand nb = new n.NpgsqlCommand("select  st_Area(geom) as surface  from nix", cnx);
            double ida = 0;
            var readera = nb.ExecuteReader();
            while (readera.Read())
            {
                ida = Double.Parse(readera["surface"].ToString());
                s.Text = ida + "";
            }
            n.NpgsqlCommand ho = new n.NpgsqlCommand("create or replace view den as select zone,sum(quartier.densite) as de,code as valeur from bf_t,quartier where ST_INTERSECTS(bf_t.bf,quartier.zone) group by code",cnx);
            ho.ExecuteNonQuery();
            n.NpgsqlCommand ko = new n.NpgsqlCommand("create or replace view fi as select ST_AREA(ST_intersection(bf_t.bf,zone))*de as vl,de from den,bf_t;",cnx);
            ko.ExecuteNonQuery();
            n.NpgsqlCommand c = new n.NpgsqlCommand("select sum(vl) as valeur from fi", cnx);
            double i = 0;
            var rea = c.ExecuteReader();
            while (rea.Read())
            {
                i = Double.Parse(rea["valeur"].ToString());
                NC.Text = (i) + "";
               
            }
            densite.Text = (Int32.Parse(Pv.Text)-i) + ""; 

            n.NpgsqlCommand nca = new n.NpgsqlCommand(" select SUM(ST_Area(quartier.zone)) as valeur  from bf_t,quartier ", cnx);
            double ira = 0;
            var ra = nca.ExecuteReader();
            while (ra.Read())
            {
                ira = Double.Parse(ra["valeur"].ToString());
                
            }
            NCC.Text = (ira-ida) + "";
           
            //vc.Style = styleSelected;
            //map1.Map.Layers.Add(vc);
        }
        public void quartier_traitement(int code)
        {
         

            cnx.Close();
            cnx.Open();
            n.NpgsqlCommand cm = new n.NpgsqlCommand("create or replace view buffer_all as select num,ST_buffer(emp,puissance)as bf from antenne;", cnx);
            cm.ExecuteNonQuery();
            n.NpgsqlCommand cma = new n.NpgsqlCommand("create or replace view quart as select st_intersection(quartier.zone,bf)as inter from quartier,buffer_all where st_intersects(quartier.zone,bf) and quartier.code="+code+"", cnx);
            cma.ExecuteNonQuery();
            n.NpgsqlCommand sauvegarde = new n.NpgsqlCommand("CREATE OR REPLACE VIEW cell as select ST_UNION(inter) as geom  from quart", cnx);
            sauvegarde.ExecuteNonQuery();
            n.NpgsqlCommand c = new n.NpgsqlCommand(" select ST_AREA(geom) as valeur from cell", cnx);
            double i = 0;
            var rea = c.ExecuteReader();
            while (rea.Read())
            {
                i = Double.Parse(rea["valeur"].ToString());
                
            }

            
            n.NpgsqlCommand ca = new n.NpgsqlCommand(" select ST_AREA(zone) as valeur from quartier where code="+code+"", cnx);
            double ia = 0;
            var re = ca.ExecuteReader();
            while (re.Read())
            {
                ia = Double.Parse(re["valeur"].ToString());
                

            }
            tc.Text = ((i / ia) * 100) + "%";
            supc.Text = i + "";
            supnc.Text = (ia - i) + "";

            n.NpgsqlCommand de = new n.NpgsqlCommand(" select densite as valeur from quartier where code=" + code + "", cnx);
            double iaa = 0;
            var e = de.ExecuteReader();
            while (e.Read())
            {
                iaa = Double.Parse(e["valeur"].ToString());


            }
            pc.Text = (iaa * i) + "";
            pnc.Text = ((ia - i) * iaa) + "";
        }


    }
}

