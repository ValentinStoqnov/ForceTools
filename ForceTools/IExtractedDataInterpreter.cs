using System;

namespace ForceTools
{
    public interface IExtractedDataInterpreter
    {
        Kontragent Kontragent { get; set; }
        Invoice Invoice { get; set; }
        InvoiceDefaultValues DefaultValues { get; set; }
    }
}