using System;
using MizanOriginalSoft.MainClasses.Enums; // 👈 أضف هذا السطر

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    public static class InvoiceTypeHelper
    {
        public static string ToAccountTypeString(InvoiceType type)
        {
            return type switch
            {
                InvoiceType.Sale or InvoiceType.SaleReturn => "SalesMen",
                InvoiceType.Purchase or InvoiceType.PurchaseReturn => "PurchaseMen",
                InvoiceType.Inventory or InvoiceType.DeductStock or InvoiceType.AddStock => "Inventory",
                _ => string.Empty
            };
        }
    }
}
