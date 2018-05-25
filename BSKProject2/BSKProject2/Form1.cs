using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace BSKProject2
{
    public partial class Form1 : Form
    {
        bool userLogInFlag = false;

        //pola przydatne w mainPanel.
        public string chosenProfile;    //informacja o tym jaki profil wybral uzytkownik.
        public string userLogin;        //informacja o loginie uzytkownika.
        public string userPassword;     //informacja o hasle uzytkownika.
        public string userPESEL;        //informacja o PESELu uzytkownika (do tworzenia zapytan 'tylko dla siebie').

        private SqlConnection sqlConnection = new SqlConnection(
                "Data Source=SUNNIVRANDELL;" +
                "Initial Catalog=BSKproj2;" +
                "Trusted_Connection=yes;");

        public Form1()
        {
            InitializeComponent();
        }

        static string HashSHA1(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder stringBuilder = new StringBuilder(hash.Length * 2);

                foreach (byte singleByte in hash)
                {
                    stringBuilder.Append(singleByte.ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// Metoda do zaktualizowania informacji w bazie o zalogowaniu uzytkownika.
        /// </summary>
        private void loginInDatabase()
        {
            try
            {
                this.sqlConnection.Open();

                SqlDataReader sqlReader = null;
                string command = "UPDATE Uzytkownicy"
                    + " SET czy_zalogowany='true'"
                    + " WHERE _login='" + this.loginTextBox.Text + "'";
                SqlCommand sqlCommand = new SqlCommand(command, this.sqlConnection);
                sqlReader = sqlCommand.ExecuteReader();

                sqlCommand.Dispose();
                sqlReader.Close();

                this.sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Metoda do zaktualizowania informacji w bazie o wylogowaniu uzytkownika.
        /// </summary>
        private void logoutFromDatabase()
        {
            try
            {
                this.sqlConnection.Open();

                SqlDataReader sqlReader = null;
                string command = "UPDATE Uzytkownicy"
                    + " SET czy_zalogowany='false'"
                    + " WHERE _login='" + this.loginTextBox.Text + "'";
                SqlCommand sqlCommand = new SqlCommand(command, this.sqlConnection);
                sqlReader = sqlCommand.ExecuteReader();

                sqlCommand.Dispose();
                sqlReader.Close();

                this.sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void zalogujButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.sqlConnection.Open();

                SqlDataReader sqlReader = null;
                string command = "SELECT *"
                    + " FROM Uzytkownicy"
                    + " WHERE _login='" + this.loginTextBox.Text + "'";
                SqlCommand sqlCommand = new SqlCommand(command, this.sqlConnection);
                sqlReader = sqlCommand.ExecuteReader();
                
                //sprawdzenie czy jest taki login
                if (!sqlReader.HasRows)
                {
                    this.loginTextBox.Text = "login lub haslo niepoprawne";
                    this.hasloTextBox.Text = "";

                    sqlCommand.Dispose();
                    sqlReader.Close();
                    this.sqlConnection.Close();

                    return;
                }
                //sprawdzenie czy haslo sie zgadza
                sqlReader.Read();
                if(!sqlReader[2].Equals(HashSHA1(this.hasloTextBox.Text)))
                {
                    this.loginTextBox.Text = "login lub haslo niepoprawne";
                    this.hasloTextBox.Text = "";

                    sqlCommand.Dispose();
                    sqlReader.Close();
                    this.sqlConnection.Close();

                    return;
                }

                //sprawdzanie czy uzytkownik nie jest juz zalogowany.
                if(sqlReader[6].Equals("true"))
                {
                    this.loginTextBox.Text = "uzytkownik jest juz zalogowany";
                    this.hasloTextBox.Text = "";

                    sqlCommand.Dispose();
                    sqlReader.Close();
                    this.sqlConnection.Close();

                    return;
                }

                //zapamietanie PESELu, loginu i hasla uzytkownika.
                this.userPESEL = sqlReader[7].ToString();
                this.userLogin = this.loginTextBox.Text;
                this.userPassword = this.hasloTextBox.Text;

                sqlCommand.Dispose();
                sqlReader.Close();

                command = "SELECT _Role.nazwa"
                    +" FROM Uzytkownicy, Uzytkownicy_Role, _Role"
                    +" WHERE Uzytkownicy._login='"+this.loginTextBox.Text+"'" 
                        +" AND Uzytkownicy_Role.FK_Uzytkownik=Uzytkownicy.Id_uzytkownika" 
                        +" AND Uzytkownicy_Role.FK_Rola=_Role.Id_roli";
                sqlCommand = new SqlCommand(command, this.sqlConnection);
                sqlReader = sqlCommand.ExecuteReader();

                //sprawdzenie jakie role posiada uzytkownik.
                while(sqlReader.Read())
                {
                    this.profilComboBox.Items.Add(sqlReader[0]);
                }
                sqlCommand.Dispose();
                sqlReader.Close();

                this.sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            loginInDatabase();
            userLogInFlag = true;

            this.loginTextBox.Enabled = false;
            this.hasloTextBox.Enabled = false;
            this.zalogujButton.Enabled = false;
            this.profilComboBox.Enabled = true;
            this.wybierzButton.Enabled = true;
        }

        private void wybierzButton_Click(object sender, EventArgs e)
        {
            //jesli uzytkownik nie wybral zadnej roli.
            if (this.profilComboBox.SelectedItem == null)
                return;

            //zapamietanie wybranego profilu uzytkownika.
            this.chosenProfile = this.profilComboBox.SelectedItem.ToString();

            MainPanel mainPanel = new MainPanel(this);
            mainPanel.Show();
            this.Hide();
        }

        //metoda do zresetowania okna logowania po wylogowaniu uzytkownika.
        public void resetWindow()
        {
            this.userLogInFlag = false;

            this.loginTextBox.Enabled = true;
            this.hasloTextBox.Text = "";
            this.hasloTextBox.Enabled = true;
            this.zalogujButton.Enabled = true;

            this.profilComboBox.Items.Clear();
            this.profilComboBox.Text = "";
            this.profilComboBox.Enabled = false;
            this.wybierzButton.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {   if(userLogInFlag)
                logoutFromDatabase();
        }
    }
}
