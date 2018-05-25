using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BSKProject2
{
    public partial class MainPanel : Form
    {
        //// Pola stale
        #region stale

        //tymczasowa zawartosc pustych komorek.
        private const string tempCellValue = "\0";
        
        //zdefiniowane kolory edytowanych i dodanych komórek.
        private Color rowsEditedColor = Color.Yellow;
        private Color rowsAddedColor = Color.Green;

        private SqlConnection sqlConnection = new SqlConnection(
                "Data Source=SUNNIVRANDELL;" +
                "Initial Catalog=BSKproj2;" +
                "Trusted_Connection=yes;");

        #endregion
        //// Pola zmienne
        #region zmienne

        private Form1 mainForm;         //ekran logowania. Zawiera wazne informacje.

        private bool isThisMainPanel;   //informacja czy to okno jest glownym mainPanel (stworzonym podczas logowania).

        private List<MainPanel> supportWindows; //lista okien pomocniczych.

        private string beforeEditCellValue;     //zawiera tresc komorki na czas jej edycji.

        private List<RowEditedList> rowsEdited; //lista indeksow edytowanych wierszy.
        private List<int> rowsAdded;                //lista indeksow dodanych wierszy.
        private List<int> rowsSelected;             //lista indeksow wybranych wierszy (np. do usuniecia).
        
        #endregion

        public MainPanel(Form1 form)
        {
            InitializeComponent();
            this.mainForm = form;
            this.isThisMainPanel = true;

            rowsEdited = new List<RowEditedList>();
            rowsAdded = new List<int>();
            rowsSelected = new List<int>();

            supportWindows = new List<MainPanel>(); 
            
            //wczytaj odpowiednie tabele dla uzytkownika o danej roli.
            try
            {
                this.sqlConnection.Open();
                
                SqlDataReader sqlReader = null;
                string command = "SELECT Tabele.nazwa"
                    + " FROM _Role, Role_Tabele, Tabele"
                    + " WHERE _Role.nazwa='" + this.mainForm.chosenProfile + "'"
                        + " AND Role_Tabele.selects='true'"
                        + " AND Role_Tabele.FK_Rola=_Role.Id_roli"
                        + " AND Role_Tabele.FK_Tabela=Tabele.Id_tabeli;";
                SqlCommand sqlCommand = new SqlCommand(command, this.sqlConnection);
                sqlReader = sqlCommand.ExecuteReader();

                while(sqlReader.Read())
                {
                    this.tableComboBox.Items.Add(sqlReader[0]);
                }
                sqlCommand.Dispose();
                sqlReader.Close();

                this.sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                this.consoleLabel.Text = "Error, can't load your tables";
            }

            this.consoleLabel.Text = "Success, loaded your tables";
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
                    + " WHERE _login='" + this.mainForm.userLogin + "'";
                SqlCommand sqlCommand = new SqlCommand(command, this.sqlConnection);
                sqlReader = sqlCommand.ExecuteReader();

                sqlCommand.Dispose();
                sqlReader.Close();

                this.sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                this.consoleLabel.Text = "Error, can't properly logout";
            }

            this.consoleLabel.Text = "Success, you have logout properly";
        }

        private void MainPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            //jesli to jest glowny mainPanel (stworzony przy logowaniu) 
            //  to wyloguj sie z bazy i pozamykaj wszystko.
            if (this.isThisMainPanel)
            {
                foreach (MainPanel panel in supportWindows)
                    panel.Close();
                logoutFromDatabase();
                this.mainForm.Close();
            }
        }

        private void wylogujButton_Click(object sender, EventArgs e)
        {
            foreach (MainPanel panel in supportWindows)
                panel.Close();
            logoutFromDatabase();
            this.mainForm.resetWindow();
            this.mainForm.Show();
            this.Hide();
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 0) //edycja checkboxa.
                return;

            //komorki moga byc puste (np. w nowopowstalym wierszu) i posiadaja wartosc null!
            try
            {
                this.beforeEditCellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            catch (NullReferenceException ex)
            {
                this.beforeEditCellValue = tempCellValue; //nadajemy im sztucznie wartosc.
            }
        }

        /// <summary>
        /// Metoda do resetowania dataGridView
        /// </summary>
        private void resetDataGridView()
        {
            this.rowsAdded.Clear();
            this.rowsEdited.Clear();
            this.rowsSelected.Clear();
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn());
            this.dataGridView1.Columns[0].Width = 20;
        }

        /// <summary>
        /// Metoda kontrolujaca uprawnienia uzytkownika
        ///     'wyszarza' przyciski INSERT, UPDATE, DELETE
        ///     sprawdza czy uzytkownik ma dostep jedynie do swoich wierszy
        ///         jesli tak to zwraca specjalne zapytanie SELECT dla tego uzytkownika
        ///         jesli nie to zwraca pusty ciag znakow
        /// </summary>
        private string checkUserVisibility()
        {
            string result = "";

            try
            {
                this.sqlConnection.Open();

                SqlDataReader sqlReader = null;
                string command = "SELECT Role_Tabele.inserts, Role_Tabele.updates, Role_Tabele.deletes, Role_Tabele.tylkoWlasny, Role_Tabele.tylkoWlasny_zapytanie"
                    + " FROM _Role, Tabele, Role_Tabele "
                    + " WHERE _Role.nazwa='"+this.mainForm.chosenProfile+"' AND Tabele.nazwa='"+this.tableComboBox.SelectedItem.ToString()+"'" 
                        +" AND _Role.Id_roli=Role_Tabele.FK_Rola AND Tabele.Id_tabeli=Role_Tabele.FK_Tabela";
                SqlCommand sqlCommand = new SqlCommand(command, this.sqlConnection);
                sqlReader = sqlCommand.ExecuteReader();

                sqlReader.Read();

                //czy wolno uzytkownikowi na INSERTy?
                if (sqlReader[0].Equals("true"))
                    this.insertButton.Enabled = true;
                else
                    this.insertButton.Enabled = false;

                //czy wolno uzytkownikowi na UPDATE'y?
                if (sqlReader[1].Equals("true"))
                    this.updateButton.Enabled = true;
                else
                    this.updateButton.Enabled = false;

                //czy wolno uzytkownikowi na DELETE'y?
                if (sqlReader[2].Equals("true"))
                    this.deleteButton.Enabled = true;
                else
                    this.deleteButton.Enabled = false;

                //czy uzytkownik ma dostep tylko swoich wierszy?
                if (sqlReader[3].Equals("true"))
                    result = sqlReader[4].ToString();

                sqlCommand.Dispose();
                sqlReader.Close();

                this.sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                this.consoleLabel.Text = "Error, can't check your visibility range in this table";
            }

            return result;
        }

        private void tableComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetDataGridView();

            string specialCommand = checkUserVisibility();

            try
            {
                this.sqlConnection.Open();

                SqlDataReader sqlReader = null;
                string command;
                if (specialCommand.Equals(""))
                {
                    //stworz zapytanie dla wszytskich wierszy.
                    command = "SELECT *"
                        + " FROM " + this.tableComboBox.SelectedItem.ToString();
                }
                else
                {
                    //skorzystaj ze specjalnego zapytania. Nalezy przedtem zastapic <tuPESEL> numerem PESEL uzytkownika otoczonego w ''.
                    command = specialCommand.Replace("<tuPESEL>", "'" + this.mainForm.userPESEL + "'");
                }
                SqlCommand sqlCommand = new SqlCommand(command, this.sqlConnection);
                sqlReader = sqlCommand.ExecuteReader();

                int numberOfColumns = sqlReader.FieldCount;
                //stworz kolumny.
                for (int i = 0; i < numberOfColumns; i++)
                {
                    this.dataGridView1.Columns.Add(sqlReader.GetName(i), sqlReader.GetName(i));
                }
                //wypelnij tabele danymi.
                while (sqlReader.Read())
                {
                    object[] data = new object[numberOfColumns + 1];
                    data[0] = false;
                    for (int i = 0; i < numberOfColumns; i++)
                    {
                        data[i + 1] = sqlReader[i];
                    }
                    dataGridView1.Rows.Add(data);
                }
                sqlCommand.Dispose();
                sqlReader.Close();

                this.sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                this.consoleLabel.Text = "Error, can't load selected table";
            }

            this.consoleLabel.Text = "Success, loaded selected table";
        }

        /// <summary>
        /// Metoda sprawdza czy edytowano checkbox dataGridView
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool cellEndEdit_isCheckBoxEdited(DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0) //edycja checkboxa. Zanaczonono lub odznaczono wiersz.
            {
                if ((bool)dataGridView1.Rows[e.RowIndex].Cells[0].Value == true)
                    this.rowsSelected.Add(e.RowIndex);
                else
                    this.rowsSelected.Remove(e.RowIndex);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Metoda sprawdza czy edytowano swiezo dodany wiersz
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool cellEndEdit_isAddedRowEdited(DataGridViewCellEventArgs e)
        {
            foreach (int rowIndex in this.rowsAdded)
            {
                if (rowIndex == e.RowIndex)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[0].Value = false;
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = this.rowsAddedColor;
                    return true;  //edytowano swiezo dodany wiersz.
                }
            }
            return false;
        }

        /// <summary>
        /// Metoda sprawdza czy edytowano juz wczesniej edytowana komorke
        ///     jesli tak to przy okazji dodaje odpowiednia informacje do listy indeksow edytowanych wierszy
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool cellEndEdit_isEditedRowEdited(DataGridViewCellEventArgs e)
        {
            foreach (RowEditedList row in this.rowsEdited)
            {
                if (row.rowIndex == e.RowIndex) //edytowano juz ten wiersz.
                {
                    foreach (int columnIndex in row.columnIndexes)
                    {
                        if (columnIndex == e.RowIndex)
                            return true;    //edytowano juz ta komorke.
                    }
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = this.rowsEditedColor;
                    row.columnIndexes.Add(e.ColumnIndex);   //dodajemy nowy indeks kolumny.     
                    return true;    //edytowano nowa komorke w wierszu.
                }
            }
            return false;
        }

        /// <summary>
        /// Metoda do sprawdzania nowych wierszy czy sa puste
        ///     jesli tak to usuwa wiersz z listy indeksow dodanych wierszy
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        private bool deleteRowIfEmpty(int rowIndex)
        {
            bool rowIsEmpty = true;
            bool checkBoxColumn = true;
            foreach (DataGridViewCell cell in dataGridView1.Rows[rowIndex].Cells)
            {
                if (checkBoxColumn)
                {
                    checkBoxColumn = false;
                    continue;
                }

                if (cell.Value != null)
                {
                    rowIsEmpty = false;
                    break;
                }
            }

            if (rowIsEmpty)
            {
                rowsAdded.Remove(rowIndex);
                //dataGridView1.Rows.Remove(dataGridView1.Rows[rowIndex]);
            }

            return rowIsEmpty;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (cellEndEdit_isCheckBoxEdited(e))
                return;

            string endEditCellValue;
            //komorki po edycji moga byc puste.
            try
            {
                endEditCellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            catch (NullReferenceException ex)
            {
                endEditCellValue = tempCellValue;   //nadajemy im sztucznie wartosc.
            }
            if (this.beforeEditCellValue.Equals(endEditCellValue))
            {
                rowsAdded.Remove(e.RowIndex);
                return; //nie dokonano zmian w komorce.
            }

            if (deleteRowIfEmpty(e.RowIndex))
                return;

            if (cellEndEdit_isAddedRowEdited(e))
                return;

            if (cellEndEdit_isEditedRowEdited(e))
                return;
            
            //obsluga przypadku gdy edytowano komorke w kolumnie klucza tabeli.
            RowEditedList newRowEdited = new RowEditedList();
            if (e.ColumnIndex == 1)
                newRowEdited.primaryKey = beforeEditCellValue;
            else
                newRowEdited.primaryKey = dataGridView1.Rows[e.RowIndex].Cells[1].Value;
            newRowEdited.primaryKeyColumnName = dataGridView1.Columns[1].Name;
            newRowEdited.rowIndex = e.RowIndex;
            newRowEdited.columnIndexes.Add(e.ColumnIndex);

            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = this.rowsEditedColor;
            rowsEdited.Add(newRowEdited);   //dodajemy nowy wiersz do listy indeksow wierszy edytowanych.
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowIndex == dataGridView1.NewRowIndex && e.RowIndex != 0)
                this.rowsAdded.Add(e.RowIndex - 1); //wykryto nowy wiersz, ALE nie ten co modyfikujemy, a nastepny zaraz za nim!
        }

        private void noweOknoButton_Click(object sender, EventArgs e)
        {
            MainPanel mainPanel = new MainPanel(this.mainForm);
            mainPanel.Show();
            mainPanel.isThisMainPanel = false;
            mainPanel.wylogujButton.Hide();
            mainPanel.noweOknoButton.Hide();
            supportWindows.Add(mainPanel);
        }

        private void odswiezButton_Click(object sender, EventArgs e)
        {
            this.tableComboBox_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// Metoda sprawdza typy kolumn obecnie wybranej tabeli
        /// </summary>
        /// <returns></returns>
        private List<String> listOfColumnTypes()
        {
            List<String> listOfTypes = new List<String>();

            try
            {
                this.sqlConnection.Open();

                SqlDataReader sqlReader = null;
                string command = "SELECT *"
                    + " FROM " + this.tableComboBox.SelectedItem.ToString();
                SqlCommand sqlCommand = new SqlCommand(command, this.sqlConnection);
                sqlReader = sqlCommand.ExecuteReader();

                int numberOfColumns = sqlReader.FieldCount;
                for (int i = 0; i < numberOfColumns; i++)
                {
                    listOfTypes.Add(sqlReader.GetDataTypeName(i));
                }
                sqlCommand.Dispose();
                sqlReader.Close();

                this.sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                this.consoleLabel.Text = "Error, can't check columns type";
            }

            return listOfTypes;
        }

        #region INSERT

        private void insertButton_Click(object sender, EventArgs e)
        {
            //zaladowanie do bazy nowych wierszy.
            //  Nalezy wykorzystac rowsAdded.

            //pierwszy trywialny problem:
            //  musimy znac typy danych kolumn, by wiedziec kiedy stosowac '' w zapytaniach
            //      int => bez ''
            //      reszta => z ''
            List<String> listOfTypes = listOfColumnTypes();

            foreach (int rowIndex in rowsAdded)
            {
                string insertCommand = "INSERT INTO " + this.tableComboBox.SelectedItem.ToString() + " VALUES(";
                int numberOfCells = dataGridView1.Rows[rowIndex].Cells.Count;
                for (int j = 1; j < numberOfCells; j++)
                {
                    dataGridView1.Rows[rowIndex].Cells[j].Style.BackColor = DefaultBackColor;

                    string value;
                    //obsluga przypadkow gdy komorki sa puste.
                    try
                    {
                        value = dataGridView1.Rows[rowIndex].Cells[j].Value.ToString();
                    }
                    catch (NullReferenceException ex)
                    {
                        value = "";   //nadajemy im sztucznie wartosc.
                    }
                    /*
                    if (dataGridView1.Rows[rowIndex].Cells[j].Value != null)
                        value = dataGridView1.Rows[rowIndex].Cells[j].Value.ToString();
                    */

                    //drugi trywialny problem: 
                    //  przecinki...
                    //      1 element z ciagu bez przecinka
                    //      nastepne już z przecinkami
                    if (j == 1)
                        if (listOfTypes[j - 1] == "int")
                            insertCommand += value;
                        else
                            insertCommand += "'" + value + "'";
                    else
                        if (listOfTypes[j - 1] == "int")
                            insertCommand += "," + value;
                        else
                            insertCommand += ",'" + value + "'";   
                }
                insertCommand += ");";

                try
                {
                    this.sqlConnection.Open();

                    SqlDataReader sqlReader = null;
                    string command = insertCommand;
                    SqlCommand sqlCommand = new SqlCommand(command, this.sqlConnection);
                    sqlReader = sqlCommand.ExecuteReader();

                    sqlCommand.Dispose();
                    sqlReader.Close();

                    this.sqlConnection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    this.consoleLabel.Text = "Error, can't insert new rows";
                }
            }

            this.consoleLabel.Text = "Success, new rows has been inserted";
            rowsAdded.Clear();
        }

        #endregion

        #region UPDATE

        private void updateButton_Click(object sender, EventArgs e)
        {
            //zaladowanie do bazy zuaktualizowane wiersze.
            //  Nalezy wykorzystac rowsEdited.

            //pierwszy trywialny problem:
            //  musimy znac typy danych kolumn, by wiedziec kiedy stosowac '' w zapytaniach
            //      int => bez ''
            //      reszta => z ''
            List<String> listOfTypes = listOfColumnTypes();

            foreach (RowEditedList row in rowsEdited)
            {
                string updateCommand = "UPDATE " + this.tableComboBox.SelectedItem.ToString() + " SET";
                bool firstParam = true;
                foreach (int columnIndex in row.columnIndexes)
                {
                    dataGridView1.Rows[row.rowIndex].Cells[columnIndex].Style.BackColor = DefaultBackColor;

                    string value;
                    //obsluga przypadkow gdy komorki sa puste.
                    try
                    {
                        value = dataGridView1.Rows[row.rowIndex].Cells[columnIndex].Value.ToString();
                    }
                    catch (NullReferenceException ex)
                    {
                        value = "";   //nadajemy im sztucznie wartosc.
                    }
                    /*
                    if (dataGridView1.Rows[row.rowIndex].Cells[columnIndex].Value != null)
                        value = dataGridView1.Rows[row.rowIndex].Cells[columnIndex].Value.ToString();
                    */

                    //drugi trywialny problem: 
                    //  przecinki...
                    //      1 element z ciagu bez przecinka
                    //      nastepne już z przecinkami
                    if(firstParam==true)
                    {
                        if(listOfTypes[columnIndex-1]=="int")
                            updateCommand += " " + dataGridView1.Columns[columnIndex].Name + "=" + value;   
                        else
                            updateCommand += " " + dataGridView1.Columns[columnIndex].Name + "=" + "'" + value + "'";
                        
                        firstParam=false;
                    }
                    else
                        if (listOfTypes[columnIndex - 1] == "int")
                            updateCommand += ", " + dataGridView1.Columns[columnIndex].Name + "=" + value;      
                        else
                            updateCommand += ", " + dataGridView1.Columns[columnIndex].Name + "=" + "'" + value + "'";
                                
                }
                updateCommand += " WHERE " + row.primaryKeyColumnName+"=";
                //musimy tez wiedziec czy stosowac '' w stosunku do klucza glownego.
                if (listOfTypes[0] == "int")
                    updateCommand += row.primaryKey;
                else
                    updateCommand += "'"+row.primaryKey+"'";

                try
                {
                    this.sqlConnection.Open();

                    SqlDataReader sqlReader = null;
                    string command = updateCommand;
                    SqlCommand sqlCommand = new SqlCommand(command, this.sqlConnection);
                    sqlReader = sqlCommand.ExecuteReader();

                    sqlCommand.Dispose();
                    sqlReader.Close();

                    this.sqlConnection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    this.consoleLabel.Text = "Error, can't update changed rows";
                }
            }

            this.consoleLabel.Text = "Success, changed rows has been updated";
            rowsEdited.Clear();
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Metoda usuwa swiezo dodane wiersze
        /// </summary>
        /// <param name="rowsToRemoveList"></param>
        private void deleteButton_handleAddedRowRemove(List<DataGridViewRow> rowsToRemoveList)
        {
            foreach (DataGridViewRow rowToRemove in rowsToRemoveList.ToList())
            {
                foreach (int addedRowIndex in rowsAdded.ToList())
                {
                    if (addedRowIndex == rowToRemove.Index) //Usuniecie dodanego wiersza
                    {
                        rowsAdded.Remove(addedRowIndex);
                        for (int i = 0; i < rowsAdded.Count; i++)
                        {
                            if (rowsAdded[i] > addedRowIndex)
                                rowsAdded[i]--;
                        }
                        dataGridView1.Rows.Remove(rowToRemove);
                        rowsToRemoveList.Remove(rowToRemove);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Metoda usuwa wiersze sprawdzajac przy okazji czy dany wiersz nie byl wczesniej edytowany
        ///     jesli tak to usuwa ten wiersz z listy indeksow wierszy edytowanych
        /// </summary>
        /// <param name="rowsToRemoveList"></param>
        private void deleteButton_handleEditedRowRemove(List<DataGridViewRow> rowsToRemoveList)
        {
            foreach (DataGridViewRow rowToRemove in rowsToRemoveList.ToList())
            {
                string deleteCommandCondition = dataGridView1.Columns[1].Name+"="+rowToRemove.Cells[1].Value.ToString();
                foreach (RowEditedList editedRow in rowsEdited.ToList())
                {
                    if (editedRow.rowIndex == rowToRemove.Index) //Usuniecie edytowanego wiersza
                    {
                        deleteCommandCondition = dataGridView1.Columns[1].Name + "=" + editedRow.primaryKey;
                        rowsEdited.Remove(editedRow);
                        for (int i = 0; i < rowsEdited.Count; i++)
                        {
                            if (rowsEdited[i].rowIndex > editedRow.rowIndex)
                                rowsEdited[i].decrementIndex();
                        }
                        break;
                    }
                }

                try
                {
                    this.sqlConnection.Open();

                    SqlDataReader sqlReader = null;
                    string command = "DELETE FROM "+this.tableComboBox.SelectedItem.ToString()
                        +" WHERE "+deleteCommandCondition;
                    SqlCommand sqlCommand = new SqlCommand(command, this.sqlConnection);
                    sqlReader = sqlCommand.ExecuteReader();

                    sqlCommand.Dispose();
                    sqlReader.Close();

                    this.sqlConnection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    this.consoleLabel.Text = "Error, can't delete selected rows" ;
                }

                dataGridView1.Rows.Remove(rowToRemove);
                rowsToRemoveList.Remove(rowToRemove);
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            //Usuniecie z bazy wybrane wiersze.
            //  Nalezy wykorzystac rowsSelected.

            List<DataGridViewRow> rowsToRemoveList = new List<DataGridViewRow>();
            foreach (int selectedRowIndex in rowsSelected)
                rowsToRemoveList.Add(dataGridView1.Rows[selectedRowIndex]);

            deleteButton_handleAddedRowRemove(rowsToRemoveList);
            deleteButton_handleEditedRowRemove(rowsToRemoveList);

            this.consoleLabel.Text = "Success, selected rows has been deleted";
            rowsSelected.Clear();
        }

        #endregion
    }
}