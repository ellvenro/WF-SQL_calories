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
    public partial class addForm : Form
    {
        /// <summary>
        /// Поле - ссылка на экземпляр класса OleDbConnection для соединения с БД
        /// </summary>
        public OleDbConnection myConnection;

        public addForm(OleDbConnection Connection)
        {
            InitializeComponent();

            myConnection = Connection;

            //Заполнение списка категорий
            string query = "SELECT categoryes.c_category FROM categoryes";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader[0].ToString());
            }
            reader.Close();

            button1.Text = "";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            if (radioButton1.Checked || radioButton3.Checked)
            {
                string index = comboBox1.SelectedItem.ToString();
                string query = "SELECT diet.d_name, diet.d_ccal, diet.d_gramm " +
                    "FROM categoryes INNER JOIN diet ON categoryes.c_n = diet.c_category " +
                    $"WHERE categoryes.c_category = \"{index}\"";
                OleDbCommand command = new OleDbCommand(query, myConnection);
                OleDbDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    int i = 0;
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add("", "", "", "");
                        dataGridView1.Rows[i].Cells[0].Value = reader[0].ToString();
                        dataGridView1.Rows[i].Cells[1].Value = reader[2].ToString();
                        dataGridView1.Rows[i].Cells[2].Value = reader[1].ToString();
                        i++;
                    }
                    reader.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query;
            OleDbCommand command;
            try
            {
                if (radioButton1.Checked)
                {

                    query = $"DELETE diet.d_name FROM diet WHERE diet.d_name = " +
                        $"\"{dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value.ToString()}\"";
                    command = new OleDbCommand(query, myConnection);
                    command.ExecuteNonQuery();
                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                    Column1.ReadOnly = false;
                    Column2.ReadOnly = false;
                    Column3.ReadOnly = false;
                }
                else if (radioButton2.Checked)
                {
                    for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                    {
                        if (dataGridView1[1, i].Value.ToString() != "" && dataGridView1[2, i].Value.ToString() != "")
                        {
                            query = $"SELECT categoryes.c_n FROM categoryes WHERE categoryes.c_category = \"{comboBox1.SelectedItem.ToString()}\"";
                            command = new OleDbCommand(query, myConnection);
                            int j = int.Parse(command.ExecuteScalar().ToString());

                            query = "INSERT INTO [diet] ( c_category, d_name, d_ccal, d_gramm )" +
                                $"VALUES (\"{j}\", \"{dataGridView1[0, i].Value.ToString()}\", \"{int.Parse(dataGridView1[2, i].Value.ToString())}\", \"{int.Parse(dataGridView1[1, i].Value.ToString())}\")";
                            command = new OleDbCommand(query, myConnection);
                            command.ExecuteNonQuery();
                        }
                    }
                    dataGridView1.Rows.Clear();
                }
                else if (radioButton3.Checked)
                {
                    for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                    {
                        if (dataGridView1[1, i].Value.ToString() != "" && dataGridView1[2, i].Value.ToString() != "")
                        {
                            query = $"UPDATE diet SET diet.d_ccal = {int.Parse(dataGridView1[2, i].Value.ToString())}, diet.d_gramm = {int.Parse(dataGridView1[1, i].Value.ToString())} WHERE diet.d_name=\"{dataGridView1[0, i].Value.ToString()}\"";
                            command = new OleDbCommand(query, myConnection);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    throw new Exception("Действие не выбрано");
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            Column1.ReadOnly = false;
            Column2.ReadOnly = false;
            Column3.ReadOnly = false;
            button1.Text = "Добавить";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Column1.ReadOnly = true;
            Column2.ReadOnly = true;
            Column3.ReadOnly = true;
            button1.Text = "Удалить";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            Column1.ReadOnly = true;
            Column2.ReadOnly = false;
            Column3.ReadOnly = false;
            button1.Text = "Изменить";
        }
    }
}
