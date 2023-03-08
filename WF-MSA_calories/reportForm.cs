using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace WF_MSA_calories
{
    public partial class reportForm : Form
    { 
        /// <summary>
        /// Поле - ссылка на экземпляр класса OleDbConnection для соединения с БД
        /// </summary>
        public OleDbConnection myConnection;

        public List<GroupBox> listGB = new List<GroupBox>();
        public List<ListView> listLV = new List<ListView>();
        public List<ListView> listLV2 = new List<ListView>();
        public List<Label> listL = new List<Label>();

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawRectangle(new Pen(Color.Black), new Rectangle(label7.Location.X - 1, label7.Location.Y - 1, label7.Width + 1, label7.Height + 1));

        }

        public reportForm(OleDbConnection Connection)
        {
            InitializeComponent();
            myConnection = Connection;

            listGB.Add(groupBox1); listLV.Add(listView1); listL.Add(label1); listLV2.Add(listView7);
            listGB.Add(groupBox2); listLV.Add(listView2); listL.Add(label2); listLV2.Add(listView8);
            listGB.Add(groupBox3); listLV.Add(listView3); listL.Add(label3); listLV2.Add(listView9);
            listGB.Add(groupBox4); listLV.Add(listView4); listL.Add(label4); listLV2.Add(listView10);
            listGB.Add(groupBox5); listLV.Add(listView5); listL.Add(label5); listLV2.Add(listView11);
            listGB.Add(groupBox6); listLV.Add(listView6); listL.Add(label6); listLV2.Add(listView12);

            for (int i = 0; i < 6; i++)
            {
                listGB[i].Visible = false;
                listLV[i].Visible = false;
                listL[i].Visible = false;
                listLV2[i].Visible = false;
            }

            string query = "SELECT eating.e_meal, eating.e_ccal, e_belk, e_giri, e_ugl FROM eating";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            OleDbDataReader reader = command.ExecuteReader();
            int sum = 0;
            float sumb = 0;
            float sumg = 0;
            float sumu = 0;
            while (reader.Read())
            {
                string query1 = $"SELECT day.d_name, day.d_gramm, day.d_ccal, d_belk, d_giri, d_ugl FROM [day] WHERE day.d_meal=\"{reader[0].ToString()}\"";
                OleDbCommand command1 = new OleDbCommand(query1, myConnection);
                OleDbDataReader reader1 = command1.ExecuteReader();

                for (int i = 0; i < 6; i++)
                    if (reader[0].ToString() == listGB[i].Text)
                    {
                        int cnt = 0;
                        while (reader1.Read())
                        {
                            if (reader1[0].ToString() != "")
                            {
                                listLV[i].Items.Add(reader1[0].ToString());

                                ListViewItem item = new ListViewItem(new[] { reader1[1].ToString(),
                                        reader1[2].ToString(),
                                        Math.Round(float.Parse(reader1[3].ToString()), 2).ToString(),
                                        Math.Round(float.Parse(reader1[4].ToString()), 2).ToString(),
                                        Math.Round(float.Parse(reader1[5].ToString()), 2).ToString() });
                                listLV2[i].Items.Add(item);

                                cnt++;
                            }
                        }
                        if (cnt != 0)
                        {

                            listL[i].Text = reader[1].ToString() + " / " + Math.Round(float.Parse(reader[2].ToString()), 2).ToString() +
                                " / " + Math.Round(float.Parse(reader[3].ToString()), 2).ToString() +
                                " / " + Math.Round(float.Parse(reader[4].ToString()), 2).ToString();
                            sum += int.Parse(reader[1].ToString());
                            sumb += float.Parse(reader[2].ToString());
                            sumg += float.Parse(reader[3].ToString());
                            sumu += float.Parse(reader[4].ToString());
                            listGB[i].Visible = true;
                            listLV[i].Visible = true;
                            listL[i].Visible = true;
                            listLV2[i].Visible = true;                                
                            
                        }
                    }
                reader1.Close();

            }
            reader.Close();
            label7.Text = sum.ToString() + " / " + Math.Round(float.Parse(sumb.ToString()), 1).ToString() +
                " / " + Math.Round(float.Parse(sumg.ToString()), 1).ToString() +
                " / " + Math.Round(float.Parse(sumu.ToString()), 1).ToString();
        }
    }
}
