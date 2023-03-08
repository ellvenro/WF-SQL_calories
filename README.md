# Программа, реализованная с помощью платформы разработки Windows Forms с использованием базы данных

## Калькулятор калорий с подсчеом белков, жиров и углеводов

_Программа создана для изучения основ программирования с использованием реляционных баз данных._

При открытии пользователем программы (*.exe файла) появляется окно с перечнем еды для конкретного приема пищи, возможно произвести выбор приема пищи, категории еды и продукта, для каждого продукта можно установить вес. Справа производится подсчет итогов за прием, за день, также подсчитываются нормы БЖУ. 
При нажатии кнопки _"Сохранить"_ прозводится сохранение текущего состояния, кнопка _"Удалить"_ удяляет выбранный пункт из таблицы, кнопка _"Отчистить"_ возвращает состояние таблицы в исходное (остаются только категории). 

<p align="center">
    <img src="https://drive.google.com/uc?export=view&id=1R9loLx2r_Idq6ZU8f33IEoq9osepEff9" width="700"/>
</p>

При нажатии кнопки _"Изменить"_ появляется окно для изменения, удаления или добавления данных.

<p align="center">
    <img src="https://drive.google.com/uc?export=view&id=15xKV5WnMttEdbw8HyctqgLf5xYI_xpY2" width="500"/>
</p>

При нажатии кнопки "Отчет" выводится вся информация обо всех приемах пищи за день.

<p align="center">
    <img src="https://drive.google.com/uc?export=view&id=1F09mdbaVSvrgz_xoInLUk8lLoVlqwbar" width="700"/>
</p>

##Основные моменты при реализации

+ Файл базы данных был создан в программе MS Access и пересохранен с расширением *.mdb.

+ Для работы с БД использовано пространство имен System.Data.OleDb.

```c#
using System.Data.OleDb;
```

+ Подключение БД:

```c#
public static string connectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Database.mdb;";
public OleDbConnection myConnection;
 
myConnection = new OleDbConnection(connectString);
myConnection.Open();
```

+ Использование SQL-запросов:

```c#
//Запрос на выборку
string query = "SELECT eating.e_meal FROM eating";
OleDbCommand command = new OleDbCommand(query, myConnection);
OleDbDataReader reader = command.ExecuteReader();
while (reader.Read())
{
  comboBox1.Items.Add(reader[0].ToString());
}
reader.Close();
...
//Запрос на удаление
query = $"DELETE diet.d_name FROM diet WHERE diet.d_name = " + 
    $"\"{dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value.ToString()}\"";
command = new OleDbCommand(query, myConnection);
command.ExecuteNonQuery();
...
//Запрос на обновление
command.CommandText = ($"UPDATE diet SET " +
    $"diet.d_ccal = {int.Parse(dataGridView1[2, i].Value.ToString())}, " +
    $"diet.d_gramm = {int.Parse(dataGridView1[1, i].Value.ToString())}, " +
    "diet.d_belk = @sumb, " +
    "diet.d_giri = @sumg, " +
    "diet.d_ugl = @sumu " +
    $" WHERE diet.d_name=\"{dataGridView1[0, i].Value.ToString()}\"");
command.Parameters.AddWithValue("@sumb", float.Parse(dataGridView1[3, i].Value.ToString()));
command.Parameters.AddWithValue("@sumg", float.Parse(dataGridView1[4, i].Value.ToString()));
command.Parameters.AddWithValue("@sumu", float.Parse(dataGridView1[5, i].Value.ToString()));
command.ExecuteNonQuery();
```
