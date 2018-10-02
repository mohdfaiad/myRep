﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace myRep_app
{
    public partial class Form1 : Form
    {
        public static int loggedUserID;
        public static String username;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'myRep_ODS_User_DataSet.UserSet' table. You can move, or remove it, as needed.
            this.userSetTableAdapter.Fill(this.myRep_ODS_User_DataSet.UserSet);
            this.myRep_ODS_HCP_DataSet.EnforceConstraints = false;
            this.myRep_ODS_HCO_DataSet.EnforceConstraints = false;
            this.myRep_ODS_Address_DataSet.EnforceConstraints = false;
            // TODO: This line of code loads data into the 'myRep_ODS_HCO_DataSet.HCOSet' table. You can move, or remove it, as needed.
            this.hCOSetTableAdapter.Fill(this.myRep_ODS_HCO_DataSet.HCOSet);
            // TODO: This line of code loads data into the 'myRep_ODS_Address_DataSet.AddressSet' table. You can move, or remove it, as needed.
            this.addressSetTableAdapter.Fill(this.myRep_ODS_Address_DataSet.AddressSet);
            // TODO: This line of code loads data into the 'myRep_ODS_HCP_DataSet.HCPSet' table. You can move, or remove it, as needed.
            this.hCPSetTableAdapter.Fill(this.myRep_ODS_HCP_DataSet.HCPSet);
            //Ukrywa kolumny z ID w poszczególnych dataGridView
            this.hcpDataGridView.Columns[0].Visible = false;
            this.addressDataGridView.Columns[0].Visible = false;
            this.hcoDataGridView.Columns[0].Visible = false;
        }

/********
* HOME*
* *****/
        private void LoginButton_Click_1(object sender, EventArgs e)
        {

            //Nawiązanie połączenia z bazą danych
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Properties.Settings.Default.ConnectionString;
            conn.Open();
            try
            {
                //Logowanie - matchowanie passworda z wartością w bazie danych dla konkretnego usera
                String commandText = "SELECT dbo.UserCredentialsSet.PWD FROM dbo.UserCredentialsSet join dbo.UserSet on dbo.UserSet.userID = dbo.UserCredentialsSet.UserID  WHERE dbo.UserSet.Username = @param";
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@param", UsernameBox.Text.ToString());

                if (Convert.ToString(command.ExecuteScalar()) == PasswordBox.Text.ToString())
                {
                    // Pobieranie imienia i nazwiska + wyświetlanie komunikatu powitalnego + aktywacja przycisku SignOut + czyszczenie textBoxów
                    command.CommandText = "SELECT FirstName FROM dbo.UserSet WHERE Username = @param2";
                    command.Parameters.AddWithValue("@param2", UsernameBox.Text.ToString());
                    String fullname = (String)command.ExecuteScalar();
                    command.CommandText = "SELECT LastName FROM dbo.UserSet WHERE Username = @param3";
                    command.Parameters.AddWithValue("@param3", UsernameBox.Text.ToString());
                    fullname = fullname + " " + Convert.ToString(command.ExecuteScalar());
                    MessageBox.Show("Login successful. Welcome, " + fullname, "SIGN IN INFO");
                    LogoutButton.Enabled = true;
                    LoginButton.Enabled = false;
                    UsernameBox.Enabled = false;
                    PasswordBox.Enabled = false;

                    //Zapisanie w pamięci ID zalogowanego User'a
                    command.CommandText = "SELECT UserID FROM dbo.UserSet WHERE Username = @param5";
                    command.Parameters.AddWithValue("@param5", UsernameBox.Text.ToString());
                    loggedUserID = Convert.ToInt32(command.ExecuteScalar());

                    //Nadanie dostępu do odpowiednich zasobów na podstawie Job Title
                    command.CommandText = "SELECT JobTitle FROM dbo.UserSet WHERE Username = @param4";
                    command.Parameters.AddWithValue("@param4", UsernameBox.Text.ToString());
                    switch ((String)command.ExecuteScalar())
                    {
                        case "SYSADMIN": {
                                myAccountsButton.Visible = true; myAccountsButton.Enabled = true;
                                userMgmtButton.Visible = true; userMgmtButton.Enabled = true;
                                break; }
                        
                        default: { myAccountsButton.Visible = false; myAccountsButton.Enabled = false; break; }
                    }
                    UsernameBox.Text = "";
                    PasswordBox.Text = "";
                }
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server: " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
            conn.Close();
        }

        private void LogoutButton_Click_1(object sender, EventArgs e)
        {
            loggedUserID = 0;
            LogoutButton.Enabled = false;
            LoginButton.Enabled = true;
            UsernameBox.Enabled = true;
            PasswordBox.Enabled = true;
            myAccountsButton.Visible = false;
            myAccountsButton.Enabled = false;
            userMgmtButton.Visible = false;
            userMgmtButton.Enabled = false;
            MessageBox.Show("GOODBYE!");
        }

        private void myAccountsButton_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = myAccountsPage;
        }


/**************
* My Accounts*
**************/
        private void createnewhcpButon(object sender, EventArgs e)
        {
            mainController.SelectedTab = newHCPPage;
        }

/**************
* newHCP*
**************/

        //Pokazanie i schowanie panelu do wyboru znanych języków
        private void LanguageSpokenComboBox_DropDown(object sender, EventArgs e)
        {
            languagespokenPanel.Visible = true;
        }
        private void LanguageSpokenComboBox_DropDownClosed(object sender, EventArgs e)
        {
            languagespokenPanel.Visible = false;
        }

        //AKTYWACJA PRZYCISKU "CREATE!" TYLKO KIEDY POLA MANDATORY SĄ WYPEŁNIONE - trzeba dać to na każde pole przy textChanged
        private void ToogleCreateNewHCPButton()
        {
            createHCP.Enabled = (fnameBox.Text != "") && (lnameBox.Text != "") && (selectedAddressLabel.Text != "") && ((mRadio.Checked==true) || (fRadio.Checked==true)) && (academicTitleList.Text != "") && (SpecialtyList.Text != "");
        }
        private void fnameBox_TextChanged(object sender, EventArgs e)
        {
          //      createHCP.Enabled = !string.IsNullOrEmpty(fnameBox.Text);
            ToogleCreateNewHCPButton();

        }

        private void newHCPPage_Click(object sender, EventArgs e)
        {

        }

        private void lnameBox_TextChanged(object sender, EventArgs e)
        {
            ToogleCreateNewHCPButton();
        }

        private void selectedAddressLabel_TextChanged(object sender, EventArgs e)
        {
            ToogleCreateNewHCPButton();
        }

        private void mRadio_MouseClick(object sender, MouseEventArgs e)
        {
            ToogleCreateNewHCPButton();
        }

        private void fRadio_MouseClick(object sender, MouseEventArgs e)
        {
            ToogleCreateNewHCPButton();
        }

        private void academicTitleList_TextChanged(object sender, EventArgs e)
        {
            ToogleCreateNewHCPButton();
        }

        private void SpecialtyList_TextChanged(object sender, EventArgs e)
        {
            ToogleCreateNewHCPButton();
        }

        //Create HCP
        private void createHCP_Click(object sender, EventArgs e)
        {
            String commandText = "INSERT INTO HCPSet VALUES(@firstname, @middlename, @lastname, @gender, @academictitle,@specialty, @birthdate, @phonenumber, @email, @kol, @languagesspoken, @addressid)";
            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@firstname", fnameBox.Text.ToString());
                if (string.IsNullOrEmpty(mnameBox.Text.ToString())) command.Parameters.AddWithValue("@middlename", DBNull.Value); else command.Parameters.AddWithValue("@middlename", mnameBox.Text.ToString());
                command.Parameters.AddWithValue("@lastname", lnameBox.Text.ToString());
                if(mRadio.Checked == true) command.Parameters.AddWithValue("@gender", "M"); else command.Parameters.AddWithValue("@gender", "F");
                command.Parameters.AddWithValue("@academictitle", academicTitleList.Text.ToString());
                command.Parameters.Add("@birthdate", SqlDbType.Date).Value = birthdatePicker.Value.Date;
                phnumberBox.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                if (string.IsNullOrEmpty(phnumberBox.Text.ToString())) command.Parameters.AddWithValue("@phonenumber", DBNull.Value); else command.Parameters.AddWithValue("@phonenumber", Convert.ToInt32(phnumberBox.Text.ToString()));
                command.Parameters.AddWithValue("@email", emailBox.Text.ToString());
                command.Parameters.AddWithValue("@kol", Convert.ToBoolean(kolBox.Checked.ToString()));
                command.Parameters.AddWithValue("@languagesspoken", DBNull.Value); 
                command.Parameters.AddWithValue("@specialty", SpecialtyList.Text.ToString());
                command.Parameters.AddWithValue("@addressid", Convert.ToInt32(selectedAddressLabel.Text.ToString()));
                //command.Parameters.AddWithValue("@hcoid", Convert.ToInt32(label11.Text.ToString()));
                command.ExecuteNonQuery();
                conn.Close();
                mainController.SelectedTab = myAccountsPage;
                myAccounts_Controller.SelectedTab = hcpPage;
                this.myRep_ODS_HCP_DataSet.Reset();
                this.hCPSetTableAdapter.Fill(this.myRep_ODS_HCP_DataSet.HCPSet);


            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        //Create HCO
        private void CreateHCOButton_Click(object sender, EventArgs e)
        {
            String commandText = "INSERT INTO HCOSet VALUES(@Name,@PhoneNumber,@Email,@Website,@AddressID,@Range,@Level,@SpecialType,@Beds,@Employees)";
            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@Name", HCONameBox.Text.ToString());
                command.Parameters.AddWithValue("@Range", RangeHCOBox.Text.ToString());
                command.Parameters.AddWithValue("@Level", Convert.ToInt16(LevelHCOBox.Text.ToString()));
                command.Parameters.AddWithValue("@SpecialType", SpecialTypeHCOBox.Text.ToString());
                command.Parameters.AddWithValue("@Beds", BedsHCOBox.Text.ToString());
                command.Parameters.AddWithValue("@Employees", EmployeesHCOBox.Text.ToString());
                command.Parameters.AddWithValue("@PhoneNumber", Int32.Parse(PhNumberHCOBox.Text.ToString()));
                command.Parameters.AddWithValue("@Email", EmailHCOBox.Text.ToString());
                command.Parameters.AddWithValue("@Website", WebsiteHCOBox.Text.ToString());
                command.Parameters.AddWithValue("@AddressID", 1);

                //command.Parameters.AddWithValue("@AddressID", Int32.Parse(selectedAddressLabel.Text.ToString()));

                command.ExecuteNonQuery();
                conn.Close();
                mainController.SelectedTab = myAccountsPage;
                myAccounts_Controller.SelectedTab = hcoPage;
                this.myRep_ODS_HCO_DataSet.Reset();
                this.hCOSetTableAdapter.Fill(this.myRep_ODS_HCO_DataSet.HCOSet);
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        private void userMgmtButton_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = userMgmtPage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = newUserPage;
        }

        private void createUserButton_Click(object sender, EventArgs e)
        {
            String commandText = "INSERT INTO UserSet VALUES(@firstname,@middlename, @lastname,@email, @jobtitle, @phonenumber,  @hiredate, @managerid, @territory, @username)";
            String commandText2 = "select count(*) from UserSet where Username = @param2";

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Properties.Settings.Default.ConnectionString; 
            conn.Open();
            try
            {
                SqlCommand command2 = new SqlCommand(commandText2, conn);
                command2.Parameters.AddWithValue("@param2", usernameUserBox.Text.ToString());
                if (Convert.ToInt32(command2.ExecuteScalar()) > 0)
                {
                    SqlCommand command = new SqlCommand(commandText, conn);
                    command.CommandText = commandText;
                    command.Parameters.AddWithValue("@firstname", fnameUserBox.Text.ToString());
                    command.Parameters.AddWithValue("@middlename", mnameUserBox.Text.ToString());
                    command.Parameters.AddWithValue("@lastname", lnameUserBox.Text.ToString());
                    command.Parameters.AddWithValue("jobtitle", jobtitleUserBox.Text.ToString());
                    command.Parameters.AddWithValue("@email", emailUserBox.Text.ToString());
                    phnumberUserBox.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                    command.Parameters.AddWithValue("@phonenumber", Convert.ToInt32(phnumberUserBox.Text.ToString()));
                    command.Parameters.Add("@hiredate", SqlDbType.Date).Value = hireDateUserPicker.Value.Date;

                    command.Parameters.AddWithValue("@managerid", DBNull.Value);
                    //command.Parameters.AddWithValue("@managerid", selectedManagerUserLabel.Text.ToString());

                    command.Parameters.AddWithValue("@territory", territoryUserBox.Text.ToString());
                    String pomoc = fnameUserBox.Text.ToString().ToLower() + "." + Convert.ToString((Convert.ToInt32(command2.ExecuteScalar()) + 1)) + "." + lnameUserBox.Text.ToString().ToLower();
                    command.Parameters.AddWithValue("@username", pomoc);
                    username = pomoc;
                    command.ExecuteNonQuery();
                    mainController.SelectedTab = setPasswordPage;
                    conn.Close();
                    this.myRep_ODS_User_DataSet.Reset();
                    this.userSetTableAdapter.Fill(this.myRep_ODS_User_DataSet.UserSet);
                }
                else
                {
                    SqlCommand command = new SqlCommand(commandText, conn);
                    command.Parameters.AddWithValue("@firstname", fnameUserBox.Text.ToString());
                    command.Parameters.AddWithValue("@middlename", mnameUserBox.Text.ToString());
                    command.Parameters.AddWithValue("@lastname", lnameUserBox.Text.ToString());
                    command.Parameters.AddWithValue("jobtitle", jobtitleUserBox.Text.ToString());
                    command.Parameters.AddWithValue("@email", emailUserBox.Text.ToString());
                    phnumberUserBox.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                    command.Parameters.AddWithValue("@phonenumber", Convert.ToInt32(phnumberUserBox.Text.ToString()));
                    command.Parameters.Add("@hiredate", SqlDbType.Date).Value = hireDateUserPicker.Value.Date;

                    command.Parameters.AddWithValue("@managerid", DBNull.Value);
                    //command.Parameters.AddWithValue("@managerid", selectedManagerUserLabel.Text.ToString());

                    command.Parameters.AddWithValue("@territory", territoryUserBox.Text.ToString());
                    command.Parameters.AddWithValue("@username", fnameUserBox.Text.ToString().ToLower() + ".x." + lnameUserBox.Text.ToString().ToLower());
                    username = fnameUserBox.Text.ToString().ToLower() + ".x." + lnameUserBox.Text.ToString().ToLower();
                    command.ExecuteNonQuery();
                    mainController.SelectedTab = setPasswordPage;
                    conn.Close();
                    this.myRep_ODS_User_DataSet.Reset();
                    this.userSetTableAdapter.Fill(this.myRep_ODS_User_DataSet.UserSet);
                }

            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        private void SetPasswordButton_Click(object sender, EventArgs e)
        {
            String commandText = "INSERT INTO UserCredentialsSet VALUES(@uid,@pw)";
            String commandText2 = "SELECT UserID FROM UserSet WHERE Username = @param";

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Properties.Settings.Default.ConnectionString;
            conn.Open();
            try
            {
                SqlCommand command2 = new SqlCommand(commandText2, conn);
                command2.Parameters.AddWithValue("@param", username);

                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@uid", Convert.ToInt32(command2.ExecuteScalar()));
                command.Parameters.AddWithValue("@pw", newpwdBox.Text.ToString());

                command.ExecuteNonQuery();

                MessageBox.Show("all done!");
                mainController.SelectedTab = userMgmtPage;
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
            conn.Close();

        }

        private void lnameUserBox_TextChanged(object sender, EventArgs e)
        {
            usernameUserBox.Text = fnameBox.Text.ToString() + ".x." + lnameUserBox.Text.ToString();

        }

        private void lnameUserBox_KeyUp(object sender, KeyEventArgs e)
        {
            usernameUserBox.Text = fnameUserBox.Text.ToString() + ".x." + lnameUserBox.Text.ToString();

        }

        private void fnameUserBox_KeyUp(object sender, KeyEventArgs e)
        {
            usernameUserBox.Text = fnameUserBox.Text.ToString() + ".x." + lnameUserBox.Text.ToString();

        }

        private void createnewhcoButton_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = newHCOPage;
        }


    }
}
