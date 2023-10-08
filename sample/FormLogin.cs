using Npgsql; 
using System;
using System.Drawing;
using System.Windows.Forms;

namespace sample
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }
        /// Установление соединения
        private NpgsqlConnection connection;
        string connection_string = String.Format("Server={0};Port={1};" + "User ID = {2};Password={3};Database={4};", "localhost", "5432", "postgres", "1123581321", "database");
        private NpgsqlCommand command;
        private string SQL = null;

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

        /// Кнопка авторизации
        private void button2_Click(object sender, EventArgs e)
        {
            connection = new NpgsqlConnection(connection_string);
            try
            {
                connection.Open();
                SQL = @"select * from login(:u_name, :pass)"; /// запрос на сервер
                command = new NpgsqlCommand(SQL, connection);
                command.Parameters.AddWithValue("u_name", textBoxUsername.Text);
                command.Parameters.AddWithValue("pass", textBoxPassword.Text);
                bool result = (bool)command.ExecuteScalar(); /// возвращает результат вызванной функции
                connection.Close();
                if (result) /// если вернулось true, то попадаем в соновную форму
                {
                    this.Hide();
                    new FormMain(textBoxUsername.Text).Show();
                }
                else
                {
                    MessageBox.Show("Incorrect data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unexpected error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connection.Close();
                throw;
            }
        }
    }
}
