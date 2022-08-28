using System;
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
        /// Флаг начала дня
        /// </summary>
        private bool start = true;

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

            label3.Text = "";

            query = "SELECT buf.buf_meal FROM buf WHERE buf.buf_n=1";
            command = new OleDbCommand(query, myConnection);

            if (command.ExecuteScalar().ToString() != "")
                start = false;
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
            string query = "SELECT categoryes.c_category " +
                "FROM eating INNER JOIN(categoryes INNER JOIN catEat ON categoryes.c_n = catEat.ce_category) ON eating.e_n = catEat.ce_meal " +
                $"WHERE eating.e_meal = \"{index}\"";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            OleDbDataReader reader = command.ExecuteReader();
            int i = 0;
            while (reader.Read())
            {
                dataGridView1.Rows.Add("", "", "", "");
                dataGridView1.Rows[i].Cells[0].Value = reader[0].ToString();
                i++;
            }
            reader.Close();

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

        /// <summary>
        /// Кнопка подсчета за один прием пищи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            int sum = 0;
            string query;
            OleDbCommand command;
            try
            {
                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    query = "INSERT INTO [day] ( d_meal, d_categoryes, d_name, d_gramm, d_ccal )" +
                        $"VALUES (\"{comboBox1.SelectedItem.ToString()}\", \"{dataGridView1[0, i].Value.ToString()}\", \"{dataGridView1[1, i].Value.ToString()}\", \"{int.Parse(dataGridView1[2, i].Value.ToString())}\", \"{int.Parse(dataGridView1[3, i].Value.ToString())}\")";

                    command = new OleDbCommand(query, myConnection);
                    command.ExecuteNonQuery();
                    sum += int.Parse(dataGridView1[2, i].Value.ToString()) * int.Parse(dataGridView1[3, i].Value.ToString()) / 100;
                }
                label3.Text = sum.ToString();
                query = $"UPDATE eating SET eating.e_ccal = {sum} WHERE eating.e_meal=\"{comboBox1.SelectedItem.ToString()}\"";
                command = new OleDbCommand(query, myConnection);
                command.ExecuteNonQuery();
                query = $"UPDATE buf SET buf.buf_meal = \"{comboBox1.SelectedItem.ToString()}\" WHERE buf.buf_n=1";
                command = new OleDbCommand(query, myConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Удаление строки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
            }
            catch (Exception)
            {
                MessageBox.Show("Выбрана пустая строка", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
