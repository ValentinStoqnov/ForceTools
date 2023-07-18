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
    public enum DocumentSides 
    { 
        LeftSide,
        RightSide
    }
    public enum RegexExtractionMethod 
    { 
        One = 1,
        Two = 2,
        Three = 3
    }
}
