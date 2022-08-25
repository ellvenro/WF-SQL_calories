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
    public partial class Form1 : Form
    {
        /// <summary>
        /// Строка подключения к MS Access
        /// </summary>
        public static string connectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Database.mdb;";

        /// <summary>
        /// Поле - ссылка на экземпляр класса OleDbConnection для соединения с БД
        /// </summary>
        private OleDbConnection myConnection;

        /// <summary>
        /// Инициализация формы
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            myConnection = new OleDbConnection(connectString);
            myConnection.Open();

            //Заполнение списка приемов пищи
            string query = "SELECT eating.e_meal FROM eating";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader[0].ToString());
            }
            reader.Close();

            //Заполнение списка категорий в таблице
            query = "SELECT categoryes.c_category FROM categoryes";
            command = new OleDbCommand(query, myConnection);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Column1.Items.Add(reader[0].ToString());
            }
            reader.Close();
        }

        /// <summary>
        /// Закрытие формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            myConnection.Close();
        }

        /// <summary>
        /// смена приема пищи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            string index = comboBox1.SelectedItem.ToString();


            if (index == "Завтрак")
            {
                string[] mas = { "Напиток", "Каша", "Другое" };
                for (int i = 0; i < mas.Length; i++)
                {
                    dataGridView1.Rows.Add(mas[i], "", "", "");
                    string query = $"SELECT diet.d_name FROM categoryes INNER JOIN diet ON categoryes.c_n = diet.c_category WHERE categoryes.c_category=\"{mas[i]}\"";
                    OleDbCommand command = new OleDbCommand(query, myConnection);
                    OleDbDataReader reader = command.ExecuteReader();
                    DataGridViewComboBoxCell comboCell = new DataGridViewComboBoxCell();
                    while (reader.Read())
                    {
                        comboCell.Items.Add(reader[0].ToString());
                    }
                    dataGridView1.Rows[i].Cells[1] = comboCell;
                    reader.Close();
                }
            }


        }

        /// <summary>
        /// Изменение значения ячейки таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == 0)
            {
                //Заполнение списка продуктов
                string cb = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                string query = $"SELECT diet.d_name FROM categoryes INNER JOIN diet ON categoryes.c_n = diet.c_category WHERE categoryes.c_category=\"{cb}\"";
                OleDbCommand command = new OleDbCommand(query, myConnection);
                OleDbDataReader reader = command.ExecuteReader();
                DataGridViewComboBoxCell comboCell = new DataGridViewComboBoxCell();
                while (reader.Read())
                {
                    comboCell.Items.Add(reader[0].ToString());
                }
                dataGridView1.Rows[e.RowIndex].Cells[1] = comboCell;
                reader.Close();
            }
            else if (e.ColumnIndex == 1)
            {
                // Заполнение грамм и калорий
                string cb = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string query = $"SELECT diet.d_gramm, diet.d_ccal FROM diet WHERE diet.d_name=\"{cb}\"";
                OleDbCommand command = new OleDbCommand(query, myConnection);
                OleDbDataReader reader = command.ExecuteReader();
                reader.Read();
                dataGridView1.Rows[e.RowIndex].Cells[2].Value = reader[0].ToString();
                dataGridView1.Rows[e.RowIndex].Cells[3].Value = reader[1].ToString();
                reader.Close();
            }

        }
    }
}
