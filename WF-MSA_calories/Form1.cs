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
        public OleDbConnection myConnection;

        /// <summary>
        /// Инициализация формы
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            bool flag = true;
            try
            {
                myConnection = new OleDbConnection(connectString);
                myConnection.Open();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                flag = false;
            }

            if (flag)
            {
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
                label5.Text = "";

                query = "SELECT buf.buf_meal FROM buf WHERE buf.buf_n=1";
                command = new OleDbCommand(query, myConnection);

                if (command.ExecuteScalar().ToString() != "")
                {
                    comboBox1.Text = command.ExecuteScalar().ToString();
                }
            }
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
        /// Смена приема пищи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            string index = comboBox1.SelectedItem.ToString();

            string query = $"SELECT day.d_categoryes, day.d_name, day.d_gramm, day.d_ccal, day.d_belk, day.d_giri, day.d_ugl FROM [day] WHERE day.d_meal=\"{index}\"";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            OleDbDataReader reader = command.ExecuteReader();
            //Если дань продолжается, то заполнить имеющимися значениями
            if (reader.HasRows)
            {
                int i = 0;
                while (reader.Read())
                {
                    dataGridView1.Rows.Add("", "", "", "", "", "", "");
                    dataGridView1.Rows[i].Cells[0].Value = reader[0].ToString();
                    string cb = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    string query1 = $"SELECT diet.d_gramm, diet.d_ccal FROM diet WHERE diet.d_name=\"{reader[1].ToString()}\"";
                    OleDbCommand command1 = new OleDbCommand(query1, myConnection);
                    if (command1.ExecuteScalar() != null)
                    {
                        dataGridView1.Rows[i].Cells[1].Value = reader[1].ToString();
                        dataGridView1.Rows[i].Cells[2].Value = reader[2].ToString();
                        if (reader[3].ToString() != "")
                            dataGridView1.Rows[i].Cells[3].Value = reader[3].ToString();
                        else
                            dataGridView1.Rows[i].Cells[3].Value = "0";
                        if (reader[4].ToString() != "")
                            dataGridView1.Rows[i].Cells[4].Value = Math.Round(float.Parse(reader[4].ToString()), 2).ToString();
                        else
                            dataGridView1.Rows[i].Cells[4].Value = "0";
                        if (reader[5].ToString() != "")
                            dataGridView1.Rows[i].Cells[5].Value = Math.Round(float.Parse(reader[5].ToString()), 2).ToString();
                        else
                            dataGridView1.Rows[i].Cells[5].Value = "0";
                        if (reader[6].ToString() != "")
                            dataGridView1.Rows[i].Cells[6].Value = Math.Round(float.Parse(reader[6].ToString()), 2).ToString();
                        else
                            dataGridView1.Rows[i].Cells[6].Value = "0";
                    }
                    i++;
                }
                reader.Close();
                Sum();
            }
            else
            {
                //Если начало программы, то стандартное заполнение
                query = "SELECT categoryes.c_category " +
                   "FROM eating INNER JOIN(categoryes INNER JOIN catEat ON categoryes.c_n = catEat.ce_category) ON eating.e_n = catEat.ce_meal " +
                   $"WHERE eating.e_meal = \"{index}\"";
                command = new OleDbCommand(query, myConnection);
                reader = command.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    dataGridView1.Rows.Add("", "", "0", "0", "0", "0", "0");
                    dataGridView1.Rows[i].Cells[0].Value = reader[0].ToString();
                    i++;
                }
                reader.Close();
                Sum();
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
                // Заполнение при изменении продукта
                string cb = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                if (cb != "")
                {
                    string query = $"SELECT diet.d_gramm, diet.d_ccal, diet.d_belk, diet.d_giri, diet.d_ugl FROM diet WHERE diet.d_name=\"{cb}\"";
                    OleDbCommand command = new OleDbCommand(query, myConnection);
                    OleDbDataReader reader = command.ExecuteReader();
                    reader.Read();
                    if (reader.HasRows)
                    {
                        //Записать граммы
                        dataGridView1.Rows[e.RowIndex].Cells[2].Value = reader[0].ToString();
                        //Записать калории
                        float ccalBuf = float.Parse(reader[0].ToString()) * float.Parse(reader[1].ToString()) / (float)100;
                        dataGridView1.Rows[e.RowIndex].Cells[3].Value = Math.Round(ccalBuf).ToString();
                        //Записать белки
                        if (reader[2].ToString() != "")
                        {
                            float ccalBelk = float.Parse(reader[0].ToString()) * float.Parse(reader[2].ToString()) / (float)100;
                            dataGridView1.Rows[e.RowIndex].Cells[4].Value = Math.Round(ccalBelk, 2).ToString();
                        }
                        else
                            dataGridView1.Rows[e.RowIndex].Cells[4].Value = "0";
                        //Записать жиры
                        if (reader[3].ToString() != "")
                        {
                            float ccalGiri = float.Parse(reader[0].ToString()) * float.Parse(reader[3].ToString()) / (float)100;
                            dataGridView1.Rows[e.RowIndex].Cells[5].Value = Math.Round(ccalGiri, 2).ToString();
                        }
                        else
                            dataGridView1.Rows[e.RowIndex].Cells[5].Value = "0";
                        //Записать углеводы
                        if (reader[4].ToString() != "")
                        {
                            float ccalUgl = float.Parse(reader[0].ToString()) * float.Parse(reader[4].ToString()) / (float)100;
                            dataGridView1.Rows[e.RowIndex].Cells[6].Value = Math.Round(ccalUgl, 2).ToString();
                        }
                        else
                            dataGridView1.Rows[e.RowIndex].Cells[6].Value = "0";
                    }
                    reader.Close();
                }
                else
                {
                    dataGridView1.Rows[e.RowIndex].Cells[2].Value = "0";
                    dataGridView1.Rows[e.RowIndex].Cells[3].Value = "0";
                    dataGridView1.Rows[e.RowIndex].Cells[4].Value = "0";
                    dataGridView1.Rows[e.RowIndex].Cells[5].Value = "0";
                    dataGridView1.Rows[e.RowIndex].Cells[6].Value = "0";
                }

                Sum();
            }
            else if (e.ColumnIndex == 2)
            {
                //Заполнение при изменении граммов
                string cb = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string query = $"SELECT diet.d_ccal, diet.d_belk, diet.d_giri, diet.d_ugl FROM diet WHERE diet.d_name=\"{cb}\"";
                OleDbCommand command = new OleDbCommand(query, myConnection);
                OleDbDataReader reader = command.ExecuteReader();
                reader.Read();
                try
                {
                    if (cb != "" && reader.HasRows)
                    {
                        float ccalBuf = float.Parse(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString()) *
                           float.Parse(reader[0].ToString()) / 100;
                        dataGridView1.Rows[e.RowIndex].Cells[3].Value = Math.Round(ccalBuf).ToString();
                    }
                    //Записать белки
                    if (reader[1].ToString() != "")
                    {
                        float ccalBelk = float.Parse(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString()) * 
                            float.Parse(reader[1].ToString()) / (float)100;
                        dataGridView1.Rows[e.RowIndex].Cells[4].Value = Math.Round(ccalBelk, 2).ToString();
                    }
                    else
                        dataGridView1.Rows[e.RowIndex].Cells[4].Value = "0";
                    //Записать жиры
                    if (reader[2].ToString() != "")
                    {
                        float ccalGiri = float.Parse(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString()) * 
                            float.Parse(reader[2].ToString()) / (float)100;
                        dataGridView1.Rows[e.RowIndex].Cells[5].Value = Math.Round(ccalGiri, 2).ToString();
                    }
                    else
                        dataGridView1.Rows[e.RowIndex].Cells[5].Value = "0";
                    //Записать углеводы
                    if (reader[3].ToString() != "")
                    {
                        float ccalUgl = float.Parse(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString()) * 
                            float.Parse(reader[3].ToString()) / (float)100;
                        dataGridView1.Rows[e.RowIndex].Cells[6].Value = Math.Round(ccalUgl, 2).ToString();
                    }
                    else
                        dataGridView1.Rows[e.RowIndex].Cells[6].Value = "0";
                    Sum();
                    reader.Close();
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        /// <summary>
        /// Кнопка подсчета за один прием пищи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            float sum = 0;
            float sumb = 0;
            float sumg = 0;
            float sumu = 0;
            string query;
            OleDbCommand command;
            try
            {
                query = $"DELETE day.d_meal FROM [day] WHERE day.d_meal=\"{comboBox1.SelectedItem.ToString()}\"";
                command = new OleDbCommand(query, myConnection);
                command.ExecuteNonQuery();
                if (dataGridView1.RowCount == 0)
                {
                    query = $"UPDATE eating SET eating.e_ccal = 0 WHERE eating.e_meal=\"{comboBox1.SelectedItem.ToString()}\"";
                    command = new OleDbCommand(query, myConnection);
                    command.ExecuteNonQuery();
                }
                else
                {
                    for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                    {
                        if (dataGridView1[1, i].Value == null)
                        {
                            query = "INSERT INTO [day] ( d_meal, d_categoryes, d_name, d_gramm, d_ccal, d_belk, d_giri, d_ugl )" +
                                $"VALUES (\"{comboBox1.SelectedItem.ToString()}\", \"{dataGridView1[0, i].Value.ToString()}\", \"\", \"0\", \"0\", \"0\", \"0\", \"0\")";
                        }
                        else if (dataGridView1[2, i].Value.ToString() != "" && dataGridView1[3, i].Value.ToString() != "" 
                            && dataGridView1[4, i].Value.ToString() != ""
                            && dataGridView1[5, i].Value.ToString() != ""
                            && dataGridView1[6, i].Value.ToString() != "")
                        {
                            query = "INSERT INTO [day] ( d_meal, d_categoryes, d_name, d_gramm, d_ccal, d_belk, d_giri, d_ugl )" +
                                $"VALUES (\"{comboBox1.SelectedItem.ToString()}\", " +
                                $"\"{dataGridView1[0, i].Value.ToString()}\", " +
                                $"\"{dataGridView1[1, i].Value.ToString()}\", " +
                                $"\"{int.Parse(dataGridView1[2, i].Value.ToString())}\", " +
                                $"\"{int.Parse(dataGridView1[3, i].Value.ToString())}\", " +
                                $"\"{float.Parse(dataGridView1[4, i].Value.ToString())}\", " +
                                $"\"{float.Parse(dataGridView1[5, i].Value.ToString())}\", " +
                                $"\"{float.Parse(dataGridView1[6, i].Value.ToString())}\"" +
                                $")";
                            sum += int.Parse(dataGridView1[3, i].Value.ToString());
                            sumb += float.Parse(dataGridView1[4, i].Value.ToString());
                            sumg += float.Parse(dataGridView1[5, i].Value.ToString());
                            sumu += float.Parse(dataGridView1[6, i].Value.ToString());
                        }
                        else
                            throw new Exception("Строка не заполнена");
                        command = new OleDbCommand(query, myConnection);
                        command.ExecuteNonQuery();
                    }

                    command.CommandText = ($"UPDATE eating SET eating.e_ccal = {sum}, eating.e_belk = @sumb, eating.e_giri = @sumg, eating.e_ugl = @sumu " +
                        $"WHERE eating.e_meal=\"{comboBox1.SelectedItem.ToString()}\"");
                    command.Parameters.AddWithValue("@sumb", float.Parse(sumb.ToString()));
                    command.Parameters.AddWithValue("@sumg", float.Parse(sumg.ToString()));
                    command.Parameters.AddWithValue("@sumu", float.Parse(sumu.ToString()));

                    command.ExecuteNonQuery();
                    Sum();
                }
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
                Sum();
            }
            catch (Exception)
            {
                MessageBox.Show("Выбрана пустая строка", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Подсчет суммы по дню и по приему пищи при изменении
        /// </summary>
        private void Sum()
        {
            try
            {
                float sum = 0;
                float sumb = 0;
                float sumg = 0;
                float sumu = 0;
                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    if (dataGridView1[3, i].Value.ToString() != "")
                        sum += float.Parse(dataGridView1[3, i].Value.ToString());
                    if (dataGridView1[4, i].Value.ToString() != "")
                        sumb += float.Parse(dataGridView1[4, i].Value.ToString());
                    if (dataGridView1[5, i].Value.ToString() != "")
                        sumg += float.Parse(dataGridView1[5, i].Value.ToString());
                    if (dataGridView1[6, i].Value.ToString() != "")
                        sumu += float.Parse(dataGridView1[6, i].Value.ToString());
                }
                label3.Text = Math.Round(sum).ToString();
                label9.Text = Math.Round(sumb, 2).ToString();
                label10.Text = Math.Round(sumg, 2).ToString(); ;
                label11.Text = Math.Round(sumu, 2).ToString(); ;

                //ККал за день
                string query = $"SELECT Sum([e_ccal]) FROM eating WHERE e_meal<>\"{comboBox1.Text.ToString()}\"";
                OleDbCommand command = new OleDbCommand(query, myConnection);
                int buf = int.Parse(command.ExecuteScalar().ToString()) + (int)Math.Round(sum);
                label5.Text = buf.ToString();
                //Белки за день
                query = $"SELECT Sum([e_belk]) FROM eating WHERE e_meal<>\"{comboBox1.Text.ToString()}\"";
                command = new OleDbCommand(query, myConnection);
                float buf1 = float.Parse(command.ExecuteScalar().ToString()) + sumb;
                label14.Text = Math.Round(buf1, 2).ToString();
                //Жиры за день
                query = $"SELECT Sum([e_giri]) FROM eating WHERE e_meal<>\"{comboBox1.Text.ToString()}\"";
                command = new OleDbCommand(query, myConnection);
                buf1 = float.Parse(command.ExecuteScalar().ToString()) + sumg;
                label13.Text = Math.Round(buf1, 2).ToString();
                //Углкводы за день
                query = $"SELECT Sum([e_ugl]) FROM eating WHERE e_meal<>\"{comboBox1.Text.ToString()}\"";
                command = new OleDbCommand(query, myConnection);
                buf1 = float.Parse(command.ExecuteScalar().ToString()) + sumu;
                label12.Text = Math.Round(buf1, 2).ToString();

                //Нормы БЖУ
                //Белки
                label22.Text = Math.Round((float)buf * 0.3 / (float)4, 2).ToString();
                //Жиры
                label20.Text = Math.Round((float)buf * 0.3 / (float)9, 2).ToString();
                //Углеводы
                label18.Text = Math.Round((float)buf * 0.4 / (float)4, 2).ToString();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Очистка БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            string query = $"UPDATE buf SET buf.buf_meal = \"\" WHERE buf.buf_n=1";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            command.ExecuteNonQuery();

            query = "UPDATE eating SET eating.e_ccal = 0, eating.e_belk = 0, eating.e_giri = 0, eating.e_ugl = 0";
            command = new OleDbCommand(query, myConnection);
            command.ExecuteNonQuery();

            query = "DELETE day.d_name FROM[day]";
            command = new OleDbCommand(query, myConnection);
            command.ExecuteNonQuery();

            comboBox1.Text = "";
            dataGridView1.Rows.Clear();
            label3.Text = "0";
            label5.Text = "0";
            label9.Text = "0";
            label10.Text = "0";
            label11.Text = "0";
            label12.Text = "0";
            label13.Text = "0";
            label14.Text = "0";
            label22.Text = "0";
            label20.Text = "0";
            label18.Text = "0";

        }

        /// <summary>
        /// Добавление, удаление, изменение данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            addForm form2 = new addForm(myConnection);
            form2.Show();
        }

        /// <summary>
        /// Форма отчета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            reportForm form3 = new reportForm(myConnection);
            form3.Show();
        }
    }
}
