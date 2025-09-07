using MizanOriginalSoft.MainClasses.Enums;

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    /// <summary>
    /// أداة مساعدة لتحويل نوع الفاتورة إلى مفتاح الحساب أو البائع
    /// </summary>
    public static class InvoiceTypeHelper
    {
        /// <summary>
        /// إرجاع مفتاح الحساب أو البائع بناءً على نوع الفاتورة
        /// </summary>
        /// <param name="type">نوع الفاتورة</param>
        /// <param name="forSeller">إذا true يرجع مفتاح البائع</param>
        /// <returns>مفتاح الحساب أو البائع</returns>
        public static string ToAccountTypeString(InvoiceType type, bool forSeller = false)
        {
            if (forSeller)
            {
                return type switch
                {
                    InvoiceType.Sale or InvoiceType.SaleReturn => "SalesMen",
                    InvoiceType.Purchase or InvoiceType.PurchaseReturn => "PurchaseMen",
                    _ => string.Empty
                };
            }

            return type switch
            {
                InvoiceType.Sale or InvoiceType.SaleReturn => "Sale",
                InvoiceType.Purchase or InvoiceType.PurchaseReturn => "Purchase",
                InvoiceType.Inventory or InvoiceType.DeductStock or InvoiceType.AddStock => "Inventory",
                _ => string.Empty
            };
        }
    }
}
