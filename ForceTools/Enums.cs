using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceTools
{
    public enum UserPermissions
    {
        Admin,
        Operator
    }
    public enum OperationType 
    { 
        Purchase,
        Sale
    }
    public enum DocumentStatuses 
    { 
        AccountedDocuments = 0,
        HeldDocuments = 1,
        UnAccountedDocuments = 2,
        ReadyToBeExportedDocuments = 3,
        ExportedDocuments = 4
    }
    public enum MassEditOperationType 
    { 
        AccountingDate,
        AccountingStatus,
        Account,
        Note
    }
    public enum InvoiceLoadOperators 
    { 
        LoadNext,
        LoadPrevious
    }
}
