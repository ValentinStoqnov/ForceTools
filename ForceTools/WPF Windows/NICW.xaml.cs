using ForceTools.ViewModels;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ForceTools
{
    /// <summary>
    /// Interaction logic for NICW.xaml
    /// </summary>
    public partial class NICW : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private OperationType OperationType;

        //Search Strings
        private string _SearchText;
        public string SearchText { get { return _SearchText; } set { _SearchText = value; OnPropertyChanged(); } }
        //Data Tables
        private DataTable _dataTableFakturi = new DataTable();
        public DataTable dataTableFakturi { get { return _dataTableFakturi; } set { dataTableFakturi = value; OnPropertyChanged(); } }
        private DataTable _DTSearch = new DataTable();
        public DataTable DTSearch { get { return _DTSearch; } set { _DTSearch = value; OnPropertyChanged(); } }

        //Popup Propertys
        private bool _CBIsOpen;
        public bool CBIsOpen { get { return _CBIsOpen; } set { _CBIsOpen = value; OnPropertyChanged(); } }

        public NICW()
        {
            InitializeComponent();
            FillDataGrid();
            FillKontragenti();
        }

        public NICW(OperationType operationType) : this()
        {
            OperationType = operationType;
        }

        #region Helpers
        public static void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
        {
            try
            {
                if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.FullRow))
                    throw new ArgumentException("The SelectionUnit of the DataGrid must be set to FullRow.");

                if (rowIndex < 0 || rowIndex > (dataGrid.Items.Count - 1))
                    throw new ArgumentException(string.Format("{0} is an invalid row index.", rowIndex));

                /* set the SelectedItem property */
                object item = dataGrid.Items[rowIndex]; // = Product X
                dataGrid.SelectedItem = item;

                DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
                if (row == null)
                {
                    /* bring the data item (Product object) into view
                     * in case it has been virtualized away */
                    dataGrid.ScrollIntoView(item);
                    row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
                }

                if (row != null)
                {
                    DataGridCell cell = GetCell(dataGrid, row, 0);
                    if (cell != null)
                        cell.Focus();
                }
            }
            catch { }
        }

        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    /* if the row has been virtualized away, call its ApplyTemplate() method
                     * to build its visual tree in order for the DataGridCellsPresenter
                     * and the DataGridCells to be created */
                    rowContainer.ApplyTemplate();
                    presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        /* bring the column into view
                         * in case it has been virtualized away */
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        #endregion

        public void FillDataGrid()
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                string CmdStringFakturi = "SELECT ImportList.Id, ImportList.KontragentiId, ImportList.Date, " +
                        "ImportList.Number, ImportList.DO, " +
                        "ImportList.DDS," +
                        "ImportList.FullValue, ImportList.AccountingStatusId, ImportList.NameAndEik FROM ImportList";
                SqlCommand SqCmd = new SqlCommand(CmdStringFakturi, sqlConnection);
                using (SqlDataAdapter sqlDataAdapterFakturi = new SqlDataAdapter(SqCmd))
                {
                    dataTableFakturi.Clear();
                    sqlDataAdapterFakturi.Fill(dataTableFakturi);
                }
            }

        }
        public void FillKontragenti()
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                string CmdStringKontragenti = "Select * from Kontragenti";
                SqlCommand SqCmdKont = new SqlCommand(CmdStringKontragenti, sqlConnection);
                SqlDataAdapter sqlDataAdapterKontragenti = new SqlDataAdapter(SqCmdKont);
                DTSearch.Clear();
                sqlDataAdapterKontragenti.Fill(DTSearch);

            }
        }

        //Invoice Delete and Tab Logic
        private void NicwDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (NicwDataGrid.SelectedItems.Count > 0)
                {
                    var Res = MessageBox.Show("Сигурени ли сте че искате да изтриете " + NicwDataGrid.SelectedItems.Count + " документа?", "Изтриване",
                        MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    if (Res == MessageBoxResult.Yes)
                    {

                        MessageBox.Show(NicwDataGrid.SelectedItems.Count + " документа бяха изтрити!");
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }

            if (e.Key == Key.Tab)
            {
                if (NicwDataGrid.CurrentColumn.DisplayIndex == 8) 
                {
                    
                    Console.WriteLine("Items " + NicwDataGrid.Items.Count);
                    int selectedIndex = NicwDataGrid.SelectedIndex + 2;
                    Console.WriteLine("Index " + selectedIndex);
                    if (selectedIndex == NicwDataGrid.Items.Count) 
                    {
                        
                        NicwDataGrid.CurrentCell = new DataGridCellInfo(
                        NicwDataGrid.Items[NicwDataGrid.Items.Count -1], NicwDataGrid.Columns[1]);
                        NicwDataGrid.BeginEdit();
                        e.Handled = true;

                    } 
                }
            }
        }

        //Kontragenti Search and Select Logic 
        private void InnerDg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid InnerDg = (DataGrid)sender;
            Popup popup = InnerDg.Parent as Popup;
            Grid grid = popup.Parent as Grid;
            TextBox Textbox = grid.Children[0] as TextBox;

            if (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Enter)
            {
                Textbox.Focus();
            }
            if (e.Key == Key.Enter)
            {



                DataGrid Innerdg = (DataGrid)sender;
                DataRowView dataRow = (DataRowView)Innerdg.SelectedItem;
                if (dataRow != null)
                {
                    string KontID = dataRow.Row.ItemArray[0].ToString();
                    string KontName = dataRow.Row.ItemArray[1].ToString();
                    string KontEIK = dataRow.Row.ItemArray[2].ToString();

                    int row = NicwDataGrid.SelectedIndex;
                    DataRowView rowView = (NicwDataGrid.Items[row] as DataRowView); //Get RowView    
                    rowView[1] = KontID;
                    rowView[8] = KontName + " " + KontEIK;

                }


                CBIsOpen = false;
                NicwDataGrid.CurrentCell = new DataGridCellInfo(
                NicwDataGrid.Items[NicwDataGrid.SelectedIndex], NicwDataGrid.Columns[5]);

            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CBIsOpen = true;
            TextBox tb = (TextBox)sender;
            SearchText = tb.Text;

            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                string cmdSearchString = "SELECT * FROM Kontragenti WHERE EIK like '%" + _SearchText + "%'";
                SqlCommand SCmdSearch = new SqlCommand(cmdSearchString, sqlConnection);
                SqlDataAdapter SDASearch = new SqlDataAdapter(SCmdSearch);
                DTSearch.Clear();
                SDASearch.Fill(DTSearch);
                Popup PopUp = VisualTreeHelper.GetChild(tb.Parent, 1) as Popup;
                DataGrid InnerDg = PopUp.Child as DataGrid;
                SelectRowByIndex(InnerDg, 0);
            }
        }

        private void ImportInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                string DocumentItemCount = Convert.ToString(NicwDataGrid.Items.Count - 1);
                MessageBox.Show("Добавени са " + (DocumentItemCount) + " документа.");
                SqlCommand SqCmd = new SqlCommand("INSERT into Fakturi (KontragentiId, Date, Number, DO,DDS,FullValue,AccountingStatusId) SELECT KontragentiId, Date, Number, DO, DDS, FullValue, AccountingStatusId From ImportList", sqlConnection);
                SqCmd.ExecuteNonQuery();
                sqlConnection.Close();
                InvoiceGridPage igp = new InvoiceGridPage(DocumentStatuses.UnAccountedDocuments, OperationType);
                MainWindow mw = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive) as MainWindow;
                mw.ContentFrame.Content = igp.Content;
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString)) 
            {
                SqlCommand SqCmd = new SqlCommand("select * from ImportList", connection);
                SqlDataAdapter sda = new SqlDataAdapter(SqCmd);
                SqlCommandBuilder builder = new SqlCommandBuilder(sda);
                sda.UpdateCommand = builder.GetUpdateCommand();
                sda.Update(dataTableFakturi);
            }
        }

        private void NicwDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {

        }

        private void NicwDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          
        }

        private void NicwDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
        }

        private void NicwDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
           
        }
    }
}
