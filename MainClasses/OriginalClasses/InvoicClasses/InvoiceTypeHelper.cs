
namespace MizanOriginalSoft .MainClasses .OriginalClasses .InvoicClasses
{
    /// أداة مساعدة لتحويل نوع الفاتورة إلى مفتاح الحساب أو البائع
    public static class InvoiceTypeHelper
    {
        public static string ToAccountTypeString(InvoiceType type, bool forSeller = false)
        {
            if (forSeller)
            {
                return type switch
                {
                    InvoiceType.Sale or InvoiceType.SaleReturn => "SalesMen", // بائعين
                    _ => "PurchaseMen"  // أي نوع آخر يرجع موردين
                };
            }

            return type switch
            {
                InvoiceType.Sale or InvoiceType.SaleReturn => "Sale",
                InvoiceType.Purchase or InvoiceType.PurchaseReturn => "Purchase",
                InvoiceType.Inventory => "Inventory",
                InvoiceType.DeductStock => "Inventory",
                InvoiceType.AddStock => "Inventory",
                _ => string.Empty
            };
        }


    }
}
