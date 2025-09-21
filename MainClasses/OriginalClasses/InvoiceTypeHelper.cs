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
                    InvoiceType.Sale or InvoiceType.SaleReturn => "SalesMen", // بائعين
                    _ => "PurchaseMen"  // أي نوع آخر يرجع موردين
                };
            }

            return type switch
            {
                InvoiceType.Sale or InvoiceType.SaleReturn => "Sale",
                InvoiceType.Purchase or InvoiceType.PurchaseReturn => "Purchase",
                InvoiceType.Inventory => "Inventory",
                InvoiceType.DeductStock => "Deduct",
                InvoiceType.AddStock => "Add",
                _ => string.Empty
            };
        }


    }
}
