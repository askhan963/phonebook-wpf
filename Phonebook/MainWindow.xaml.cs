using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Phonebook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int PhoneBookID = 0;
        public MainWindow()
        {
            InitializeComponent();

            GridFill();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void email_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (firstName.Text.Trim() != "" && lastName.Text.Trim() != "" && phoneNumber.Text.Trim() != "")
            {
                Regex reg = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = reg.Match(email.Text.Trim());
                if (match.Success)
                {
                    using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\AS KHAN\\Documents\\phone_book.accdb"))
                    {

                        conn.Open();
                        using (OleDbCommand cmd = new OleDbCommand("INSERT INTO phonebook (id, firstname, lastname,contact,email,address) VALUES (?, ?, ?,?,?,?)", conn))
                        {
                            Random random = new Random();
                            cmd.Parameters.AddWithValue("@PhoneBookID", PhoneBookID + random.Next(1, 100000000));
                            cmd.Parameters.AddWithValue("@FirstName", firstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", lastName.Text.Trim());
                            cmd.Parameters.AddWithValue("@Contact", phoneNumber.Text.Trim());
                            cmd.Parameters.AddWithValue("@Email", email.Text.Trim());
                            cmd.Parameters.AddWithValue("@Address", address.Text.Trim());
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();

                        MessageBox.Show("Inserted  Successfully");
                    }
                    Clear();
                    GridFill();
                }


                else
                    MessageBox.Show("Email Address is not Valid");
            }
            else
                MessageBox.Show("Please Fill Mandatory Fields.");
        }
        void Clear()
        {
            firstName.Text = lastName.Text = phoneNumber.Text = email.Text = address.Text = search.Text = "";
            PhoneBookID = 0;
            save.Content = "Save";

        }
        void GridFill()
        {

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\AS KHAN\\Documents\\phone_book.accdb"))
            {

                conn.Open();
                using (OleDbCommand command = new OleDbCommand("SELECT * FROM phonebook", conn))
                {
                    OleDbDataAdapter cmd = new OleDbDataAdapter(command);
                    DataTable dtbl = new DataTable();
                    cmd.Fill(dtbl);
                    dataGrid.DataContext = dtbl;
                    dataGrid.ItemsSource = dtbl.DefaultView;
                    cmd.Update(dtbl);
                }
                conn.Close();
            }




        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = dataGrid.SelectedItem as DataRowView;
            if (selectedRow != null)
            {
                using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\AS KHAN\\Documents\\phone_book.accdb"))
                {
                    conn.Open();
                    string id = Convert.ToString(selectedRow["ID"]);
                    OleDbCommand command = new OleDbCommand("DELETE FROM phonebook WHERE ID = @id", conn);
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                    MessageBox.Show(" Successfully Deleted id = " + id + " .");

                    GridFill();
                    conn.Close();
                }
            }
            else
            {
                MessageBox.Show(" Please select item ");
            }



        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (search.Text != "")
            {
                using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\AS KHAN\\Documents\\phone_book.accdb"))
                {
                    conn.Open();
                    OleDbCommand command = new OleDbCommand("SELECT * FROM phonebook WHERE firstname = @searchValue OR lastname = @searchValue ", conn);
                    command.Parameters.AddWithValue("@searchValue", search.Text);

                    OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGrid.ItemsSource = dataTable.DefaultView;
                    conn.Close();
                }
            }
            else
            {
                GridFill();
            }
        }

        private void Ccear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }
    }
}
