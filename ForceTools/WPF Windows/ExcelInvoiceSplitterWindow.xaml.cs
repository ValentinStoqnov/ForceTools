using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace ForceTools.WPF_Windows
{

    public partial class ExcelInvoiceSplitterWindow : Window
    {
        decimal DoOriginalValue;
        decimal DDSOriginalValue;
        decimal FullValueOriginalValue;
        ExcelUploaderWindow ExcelUploaderWindow;
        ExcelFinalEditDataTableEditor excelFinalEditDataTableEditor;

        public ExcelInvoiceSplitterWindow(ExcelUploaderWindow parentWindow, DataTable FinalEditDataTable, int SelectedPosition, int InsertPosition)
        {
            InitializeComponent();
            this.Owner = parentWindow;
            ExcelUploaderWindow = parentWindow;
            excelFinalEditDataTableEditor = new ExcelFinalEditDataTableEditor(SelectedPosition, InsertPosition, FinalEditDataTable);
            Kontragent kontragentToBeSplit = excelFinalEditDataTableEditor.GetKontragentForSplitting();
            Invoice invoiceToBeSplit = excelFinalEditDataTableEditor.GetInvoiceForSplitting();
            GetOriginalValues(invoiceToBeSplit);
            FillToBeSplitInvoiceTextBoxes(kontragentToBeSplit,invoiceToBeSplit);
            FillNewInvoiceTextBoxes(kontragentToBeSplit,invoiceToBeSplit);
        }
        private void GetOriginalValues(Invoice invoiceToBeSplit)
        {
            DoOriginalValue = Convert.ToDecimal(invoiceToBeSplit.DO);
            DDSOriginalValue = Convert.ToDecimal(invoiceToBeSplit.DDS);
            FullValueOriginalValue = Convert.ToDecimal(invoiceToBeSplit.FullValue);
        }
        private void FillToBeSplitInvoiceTextBoxes(Kontragent kontragentToBeSplit,Invoice invoiceToBeSplit)
        {
            SplitDateTb.Text = invoiceToBeSplit.Date.ToString();
            SplitNumberTb.Text = invoiceToBeSplit.Number.ToString();
            SplitNameTb.Text = kontragentToBeSplit.Name;
            SplitEIKTb.Text = kontragentToBeSplit.EIK;
            SplitDDSNumberTb.Text = kontragentToBeSplit.DdsNumber;
            SplitDoTb.Text = invoiceToBeSplit.DO.ToString();
            SplitDDSTb.Text = invoiceToBeSplit.DDS.ToString();
            SplitFullValueTb.Text = invoiceToBeSplit.FullValue.ToString();
            SplitInCashAccTb.Text = invoiceToBeSplit.InCashAccount.ToString();
            SplitAccountTb.Text = invoiceToBeSplit.Account.ToString();
            SplitNoteTb.Text = invoiceToBeSplit.Note;
            SplitDocTypeTb.Text = invoiceToBeSplit.DocTypeId.ToString();
            SplitDealTypeTb.Text = invoiceToBeSplit.DealKindId.ToString();
        }
        private void FillNewInvoiceTextBoxes(Kontragent kontragentToBeSplit, Invoice invoiceToBeSplit)
        {
            NewDateTb.Text = invoiceToBeSplit.Date.ToString();
            NewNumberTb.Text = invoiceToBeSplit.Number.ToString();
            NewNameTb.Text = kontragentToBeSplit.Name;
            NewEIKTb.Text = kontragentToBeSplit.EIK;
            NewDDSNumberTb.Text = kontragentToBeSplit.DdsNumber;
            NewInCashAccTb.Text = invoiceToBeSplit.InCashAccount.ToString();
            NewAccountTb.Text = invoiceToBeSplit.Account.ToString();
            NewNoteTb.Text = invoiceToBeSplit.Note;
            NewDocTypeTb.Text = invoiceToBeSplit.DocTypeId.ToString();
            NewDealTypeTb.Text = invoiceToBeSplit.DealKindId.ToString();
        }
        private Invoice GetSplitInvoiceFromTextBoxes()
        {
            Invoice splitInvoice = new Invoice();
            splitInvoice.Date = Convert.ToDateTime(SplitDateTb.Text);
            splitInvoice.Number = Convert.ToInt64(SplitNumberTb.Text);
            splitInvoice.DO = Convert.ToDecimal(SplitDoTb.Text);
            splitInvoice.DDS = Convert.ToDecimal(SplitDDSTb.Text);
            splitInvoice.FullValue = Convert.ToDecimal(SplitFullValueTb.Text);
            splitInvoice.InCashAccount = Convert.ToInt32(SplitInCashAccTb.Text);
            splitInvoice.Account = Convert.ToInt32(SplitAccountTb.Text);
            splitInvoice.Note = SplitNoteTb.Text;
            splitInvoice.DocTypeId = Convert.ToInt32(SplitDocTypeTb.Text);
            splitInvoice.DealKindId = Convert.ToInt32(SplitDealTypeTb.Text);
            return splitInvoice;
        }
        private Invoice GetNewInvoiceFromTextBoxes()
        {
            Invoice newInvoice = new Invoice();
            newInvoice.Date = Convert.ToDateTime(NewDateTb.Text);
            newInvoice.Number = Convert.ToInt64(NewNumberTb.Text);
            newInvoice.DO = Convert.ToDecimal(NewDoTb.Text);
            newInvoice.DDS = Convert.ToDecimal(NewDDSTb.Text);
            newInvoice.FullValue = Convert.ToDecimal(NewFullValueTb.Text);
            newInvoice.InCashAccount = Convert.ToInt32(NewInCashAccTb.Text);
            newInvoice.Account = Convert.ToInt32(NewAccountTb.Text);
            newInvoice.Note = NewNoteTb.Text;
            newInvoice.DocTypeId = Convert.ToInt32(NewDocTypeTb.Text);
            newInvoice.DealKindId = Convert.ToInt32(NewDealTypeTb.Text);
            return newInvoice;
        }
        private Kontragent GetNewKontragentFromTextBoxes()
        {
            Kontragent newKontragent = new Kontragent();
            newKontragent.Name = NewNameTb.Text;
            newKontragent.EIK = NewEIKTb.Text;
            newKontragent.DdsNumber = NewDDSNumberTb.Text;
            return newKontragent;
        }
        private void FinishEditBtn_Click(object sender, RoutedEventArgs e)
        {
            Invoice splitInvoice = GetSplitInvoiceFromTextBoxes();
            Kontragent newKontragent = GetNewKontragentFromTextBoxes();
            Invoice newInvoice = GetNewInvoiceFromTextBoxes();
            ExcelUploaderWindow.FinalEditDataTable = excelFinalEditDataTableEditor.SplitRowsAndReturnTable(splitInvoice, newKontragent, newInvoice);
            this.Close();
        }
        private void NewDoTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal NewValue = 0;
            if (NewDoTb.Text != string.Empty) NewValue = Convert.ToDecimal(NewDoTb.Text);
            SplitDoTb.Text = Convert.ToString(DoOriginalValue - NewValue);
        }
        private void NewDDSTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal NewValue = 0;
            if (NewDDSTb.Text != string.Empty) NewValue = Convert.ToDecimal(NewDDSTb.Text);
            SplitDDSTb.Text = Convert.ToString(DDSOriginalValue - NewValue);
        }
        private void NewFullValueTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal NewValue = 0;
            if (NewFullValueTb.Text != string.Empty) NewValue = Convert.ToDecimal(NewFullValueTb.Text);
            SplitFullValueTb.Text = Convert.ToString(FullValueOriginalValue - NewValue);
        }
    }
}
