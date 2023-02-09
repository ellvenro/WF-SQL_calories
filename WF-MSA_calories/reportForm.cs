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
            int sumb = 0;
            int sumg = 0;
            int sumu = 0;
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
                                listLV2[i].Items.Add(reader1[1].ToString() + "/" + reader1[2].ToString() + "/" + reader1[3].ToString() 
                                    + "/" + reader1[4].ToString() + "/" + reader1[5].ToString());
                cnt++;
                            }
                        }
                        if (cnt != 0)
                        {
                            listL[i].Text = reader[1].ToString() + "/" + reader[2].ToString() + "/" + reader[3].ToString() + "/" + reader[4].ToString();
                            sum += int.Parse(reader[1].ToString());
                            sumb += int.Parse(reader[2].ToString());
                            sumg += int.Parse(reader[3].ToString());
                            sumu += int.Parse(reader[4].ToString());
                            listGB[i].Visible = true;
                            listLV[i].Visible = true;
                            listL[i].Visible = true;
                            listLV2[i].Visible = true;
                        }
                    }
                reader1.Close();

            }
            reader.Close();
            label7.Text = sum.ToString() + "/" + sumb.ToString() + "/" + sumg.ToString() + "/" + sumu.ToString();
        }
    }
}
