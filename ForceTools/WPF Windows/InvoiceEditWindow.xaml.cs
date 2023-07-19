using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ForceTools.WPF_Windows
{
    /// <summary>
    /// Interaction logic for InvoiceEditWindow.xaml
    /// </summary>
    public partial class InvoiceEditWindow : Window, INotifyPropertyChanged
    {
        //InvoiceImage Properties
        public BitmapImage BitImage { get { return _BitImage; } set { _BitImage = value; OnPropertyChanged(); } }
        private BitmapImage _BitImage;
        //Kontragent Properties
        public DataTable SearchDtEik { get { return _SearchDtEik; } set { _SearchDtEik = value; OnPropertyChanged(); } }
        private DataTable _SearchDtEik;
        public bool isEikPopupOpen { get { return _isEikPopupOpen; } set { _isEikPopupOpen = value; OnPropertyChanged(); } }
        private bool _isEikPopupOpen = false;
        //DocType Properties
        public bool isDocTypePopupOpen { get { return _isDocTypePopupOpen; } set { _isDocTypePopupOpen = value; OnPropertyChanged(); } }
        private bool _isDocTypePopupOpen = false;
        public DataTable SearchDtDocType { get { return _SearchDtDocType; } set { _SearchDtDocType = value; OnPropertyChanged(); } }
        private DataTable _SearchDtDocType;
        //DealKind Properties
        public bool isDealKindPopupOpen { get { return _isDealKindPopupOpen; } set { _isDealKindPopupOpen = value; OnPropertyChanged(); } }
        private bool _isDealKindPopupOpen = false;
        public DataTable SearchDtDealKind { get { return _SearchDtDealKind; } set { _SearchDtDealKind = value; OnPropertyChanged(); } }
        private DataTable _SearchDtDealKind;
        //Accounts Properties
        public bool isAccountsPopupOpen { get { return _isAccountsPopupOpen; } set { _isAccountsPopupOpen = value; OnPropertyChanged(); } }
        private bool _isAccountsPopupOpen = false;
        public DataTable SearchDtAccounts { get { return _SearchDtAccounts; } set { _SearchDtAccounts = value; OnPropertyChanged(); } }
        private DataTable _SearchDtAccounts = new DataTable();

        private int? DealKindId;
        private int? DocTypeId;

        private int CurrentOpenInvoice;
        private DocumentStatuses AccStatusChosen;
        private OperationType OperationType;

        public event PropertyChangedEventHandler PropertyChanged;

        public InvoiceEditWindow()
        {
            InitializeComponent();
        }
        public InvoiceEditWindow(int invoiceId, DocumentStatuses accStatusChosen, OperationType operationType) : this()
        {
            FillInvoiceDataFields(invoiceId);
            OperationType = operationType;
            AccStatusChosen = accStatusChosen;
            CurrentOpenInvoice = invoiceId;
            isEikPopupOpen = false;
            isDocTypePopupOpen = false;
            isDealKindPopupOpen = false;
        }

        private void FillInvoiceDataFields(int invoiceId)
        {
            InvoiceDataReader invoiceDataReader = new InvoiceDataReader(invoiceId);
            KontTextBox.Text = invoiceDataReader.GetKontragentNameField();
            EikTextBox.Text = invoiceDataReader.GetEikField();
            DDSNumberTextBox.Text = invoiceDataReader.GetDdsNumberField();
            DocDateTextBox.Text = invoiceDataReader.GetDocDateField();
            DocNumTextBox.Text = invoiceDataReader.GetDocNumberField();
            DOTextBox.Text = invoiceDataReader.GetDoField();
            DDSTextBox.Text = invoiceDataReader.GetDdsField();
            FullValueTextBox.Text = invoiceDataReader.GetFullValueField();
            DocAccDateTextBox.Text = invoiceDataReader.GetAccDateField();
            DealKindTb.Text = invoiceDataReader.GetDealKindField();
            DocTypeTextBox.Text = invoiceDataReader.GetDocTypeField();
            AccNumTextBox.Text = invoiceDataReader.GetAccountNumberField();
            InCashAccountTextBox.Text = invoiceDataReader.GetInCashAccountField();
            NoteTextBox.Text = invoiceDataReader.GetNoteField();
            DealKindId = invoiceDataReader.GetDealKindId();
            DocTypeId = invoiceDataReader.GetDocTypeId();
            BitImage = invoiceDataReader.GetInvoiceImage();
        }
        private void LoadNextOrPrevious(InvoiceLoadOperators invoiceLoadOperators)
        {
            int nextInvoiceId = InvoiceDataFilters.FindNextOrPreviousInvoiceIdByCurrentIdAndStatusAndOperationType(CurrentOpenInvoice, invoiceLoadOperators, OperationType, AccStatusChosen);
            if (nextInvoiceId == -1)
            {
                switch (invoiceLoadOperators)
                {
                    case InvoiceLoadOperators.LoadNext:
                        MessageBox.Show("Няма други следващи фактури", "Force Tools", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case InvoiceLoadOperators.LoadPrevious:
                        MessageBox.Show("Няма други предишни фактури", "Force Tools", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }
                return;
            }
            else
            {
                FillInvoiceDataFields(nextInvoiceId);
                CurrentOpenInvoice = nextInvoiceId;
                isEikPopupOpen = false;
                isDocTypePopupOpen = false;
                isDealKindPopupOpen = false;
            }
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (EikTextBox.Text != "")
            {
                InvoiceSingleEditor.UpdateKontragentAndInvoiceDataFields(CurrentOpenInvoice, KontTextBox.Text, EikTextBox.Text, DDSNumberTextBox.Text, DocDateTextBox.Text, DocNumTextBox.Text, DOTextBox.Text, DDSTextBox.Text, FullValueTextBox.Text, DealKindId, DocTypeId, AccNumTextBox.Text, InCashAccountTextBox.Text, NoteTextBox.Text, OperationType);
                LoadNextOrPrevious(InvoiceLoadOperators.LoadNext);
            }
            else
            {
                MessageBox.Show("Полето ЕИК не може да бъде празно.");
            }
        }
        private void RightBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadNextOrPrevious(InvoiceLoadOperators.LoadNext);
        }
        private void LeftBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadNextOrPrevious(InvoiceLoadOperators.LoadPrevious);
        }

        #region Datagrid Helper Methods
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
        #region Kontragent Events
        private void EikTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            isEikPopupOpen = true;
            SearchDtEik = InvoiceDataFilters.GetKontragentiFilteredDataTableBySearchText(EikTextBox.Text);
            if (SearchDtEik.Rows.Count == 0) isEikPopupOpen = false;
            SelectRowByIndex(PopUpDgEik, 0);
        }
        private void PopUpDgEik_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Enter)
            {
                EikTextBox.Focus();
            }
            if (e.Key == Key.Enter)
            {
                DataGrid dg = (DataGrid)sender;
                DataRowView dataRow = (DataRowView)dg.SelectedItem;
                if (dataRow != null)
                {
                    string KontName = dataRow.Row.ItemArray[1].ToString();
                    string KontEIK = dataRow.Row.ItemArray[2].ToString();
                    string KontDDSNumber = dataRow.Row.ItemArray[3].ToString();

                    KontTextBox.Text = KontName;
                    EikTextBox.Text = KontEIK;
                    DDSNumberTextBox.Text = KontDDSNumber;

                    isEikPopupOpen = false;
                    EikTextBox.Focus();
                }
            }
        }
        private void DDSNumberTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            isEikPopupOpen = false;
        }
        #endregion
        #region DocType Events
        private void DocTypeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            isDocTypePopupOpen = true;
            SearchDtDocType = InvoiceDataFilters.GetDocumentTypeFilteredDataTableBySearchText(DocTypeTextBox.Text);
            if (SearchDtDocType.Rows.Count == 0) isDocTypePopupOpen = false;
            SelectRowByIndex(PopUpDgDocType, 0);
        }
        private void PopUpDgDocType_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Enter)
            {
                DocTypeTextBox.Focus();
            }
            if (e.Key == Key.Enter)
            {
                DataGrid dg = (DataGrid)sender;
                DataRowView dataRow = (DataRowView)dg.SelectedItem;
                if (dataRow != null)
                {
                    int? DocId = Convert.ToInt32(dataRow.Row.ItemArray[0]);
                    string DocTypeName = dataRow.Row.ItemArray[1].ToString();

                    DocTypeTextBox.Text = DocTypeName;
                    DocTypeId = DocId;

                    isDocTypePopupOpen = false;
                    DocTypeTextBox.Focus();
                }
            }
        }
        #endregion
        #region DealKind Events
        private void PopUpDgDealKind_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Enter)
            {
                DealKindTb.Focus();
            }
            if (e.Key == Key.Enter)
            {
                DataGrid dg = (DataGrid)sender;
                DataRowView dataRow = (DataRowView)dg.SelectedItem;
                if (dataRow != null)
                {
                    int? DealId = Convert.ToInt32(dataRow.Row.ItemArray[0]);
                    string DealName = dataRow.Row.ItemArray[2].ToString();

                    DealKindTb.Text = DealName;
                    DealKindId = DealId;

                    isDealKindPopupOpen = false;
                    DealKindTb.Focus();
                }
            }
        }
        private void DealKindTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            isDealKindPopupOpen = true;
            SearchDtDealKind = InvoiceDataFilters.GetDealKindFilteredDataTableBySearchText(DealKindTb.Text);
            if (SearchDtDealKind.Rows.Count == 0) isDealKindPopupOpen = false;
            SelectRowByIndex(PopUpDgDealKind, 0);
        }
        #endregion
        #region Account Events
        private void AccNumTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            isAccountsPopupOpen = true;
            SearchDtAccounts = InvoiceDataFilters.GetAccountNumberFilteredDataTableBySearchText(AccNumTextBox.Text);
            if (SearchDtAccounts.Rows.Count == 0) isAccountsPopupOpen = false;
            SelectRowByIndex(PopUpDgAccNum, 0);
        }
        private void PopUpDgAccNum_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Enter)
            {
                AccNumTextBox.Focus();
            }
            if (e.Key == Key.Enter)
            {
                DataGrid dg = (DataGrid)sender;
                DataRowView dataRow = (DataRowView)dg.SelectedItem;
                if (dataRow != null)
                {
                    AccNumTextBox.Text = dataRow.Row.ItemArray[0].ToString();

                    isAccountsPopupOpen = false;
                    AccNumTextBox.Focus();
                }
            }
        }
        #endregion
        #region Payed in cash Events
        private void isPayedCash_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (isPayedCash.IsChecked == false)
                {
                    isPayedCash.IsChecked = true;
                }
                else
                {
                    isPayedCash.IsChecked = false;
                }
            }
        }
        #endregion
        private void Window_Closing(object sender, CancelEventArgs e)
        {
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

