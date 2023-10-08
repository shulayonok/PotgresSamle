using System;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace sample
{
    public partial class FormMain : Form
    {
        public FormMain(string username)
        {
            /// передаём имя пользователя с предыдущей формы, блокируем поля ввода данных
            this.username = username;
            InitializeComponent();
            textBoxid.Enabled = false;
            textBoxDate.Enabled = false;
            textBoxName.Enabled = false;
            textBoxCategory.Enabled = false;
            buttonApply.Enabled = false;
        }

        /// Установление соединения
        private string username;
        private NpgsqlConnection connection;
        string connect = String.Format("Server={0};Port={1};" + "User ID = {2};Password={3};Database={4};", "localhost", "5432", "postgres", "1123581321", "database");
        private string SQL = null;
        int turn = 0;
        string str;

        /// Кнопка выхода из приложения
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// Методы для перетаскивания формы
        Point p;
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - p.X;
                this.Top += e.Y - p.Y;
            }
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            p = new Point(e.X, e.Y);
        }

        /// Метод обновления формы, чтобы пользователь видел результат
        private void Update()
        {
            dataGridView.Rows.Clear();
            connection = new NpgsqlConnection(connect);
            try
            {
                connection.Open();
                str = "SELECT * FROM " + "\"" + "Menu" + "\"";
                SQL = @str;
                var cmd = new NpgsqlCommand(SQL, connection);
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    dataGridView.Rows.Add(rdr.GetDate(0).ToString(), rdr.GetString(1), rdr.GetString(2), rdr.GetInt32(3));
                }
                connection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Вот такой еррор: " + ex.Message, "еррор", MessageBoxButtons.OK);
            }
        }

        /// Начальная загрузка формы, отправка запроса о предоставлении таблицы Menu
        private void FormMain_Load(object sender, EventArgs e)
        {
            label2.Text = label2.Text + username;
            connection = new NpgsqlConnection(connect);
            try
            {
                connection.Open();
                str = "SELECT * FROM " + "\"" + "Menu" + "\""; /// отправка запроса
                SQL = @str;
                var cmd = new NpgsqlCommand(SQL, connection);
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    dataGridView.Rows.Add(rdr.GetDate(0).ToString(), rdr.GetString(1), rdr.GetString(2), rdr.GetInt32(3)); /// добавление данных в DataGridView
                }
                connection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Вот такой еррор: " + ex.Message, "еррор", MessageBoxButtons.OK);
            }
        }

        /// При нажатии на кнопку добавления становятся доступны все поля ввода и в переменную turn кладётся единица
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            textBoxid.Enabled = true;
            textBoxDate.Enabled = true;
            textBoxName.Enabled = true;
            textBoxCategory.Enabled = true;
            buttonApply.Enabled = true;
            turn = 1;
        }

        /// При нажатии на кнопку удаления становится доступно только поле ввода id, так как удаление можно сделать только по нему и в переменную turn кладётся двойка
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            textBoxid.Enabled = true;
            buttonApply.Enabled = true;
            turn = 2;
        }

        /// При нажатии на кнопку применения действия срабатывает switch case, который в зависимости от значения переменной turn будет либо удалять, либо добавлять элемент
        private void buttonApply_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxid.Enabled = false;
                textBoxDate.Enabled = false;
                textBoxName.Enabled = false;
                textBoxCategory.Enabled = false;
                buttonApply.Enabled = false;
                switch (turn)
                {
                    case 1:
                        connection.Open();
                        str = "INSERT INTO " + "\"" + "Menu" + "\"" + "VALUES (:Date,:Name,:Category,:id)";
                        SQL = @str;
                        var cmd = new NpgsqlCommand(SQL, connection);
                        cmd.Parameters.AddWithValue("Date", Convert.ToDateTime(textBoxDate.Text));
                        cmd.Parameters.AddWithValue("Name", textBoxName.Text);
                        cmd.Parameters.AddWithValue("Category", textBoxCategory.Text);
                        cmd.Parameters.AddWithValue("id", Convert.ToInt32(textBoxid.Text));
                        cmd.ExecuteNonQuery();
                        connection.Close();
                        dataGridView.Rows.Add(textBoxDate.Text, textBoxName.Text, textBoxid.Text, textBoxCategory.Text);
                        MessageBox.Show("The addition was successful", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Update();
                        break;
                    case 2:
                        connection.Open();
                        str = "DELETE FROM " + "\"" + "Menu" + "\"" + " WHERE id = (:id)";
                        SQL = @str;
                        cmd = new NpgsqlCommand(SQL, connection);
                        cmd.Parameters.AddWithValue("id", Convert.ToInt32(textBoxid.Text));
                        ///int homeID = Convert.ToInt32(cmd.ExecuteScalar());
                        cmd.ExecuteNonQuery();
                        connection.Close();
                        MessageBox.Show("The removal was successful", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Update();
                        break;
                    default:
                        MessageBox.Show("Something has gone wrong...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Вот такой еррор: " + ex.Message, "еррор", MessageBoxButtons.OK);
            }

        }
    }
}
