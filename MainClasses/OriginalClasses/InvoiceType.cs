using System;

namespace MizanOriginalSoft.MainClasses.Enums
{
    /// <summary>
    /// أنواع الفواتير المستخدمة في النظام
    /// </summary>
    public enum InvoiceType
    {
        Sale = 1,            // فاتورة بيع
        SaleReturn = 2,      // فاتورة بيع مرتد
        Purchase = 3,        // فاتورة شراء
        PurchaseReturn = 4,  // فاتورة شراء مرتد
        Inventory = 5,       // إذن تسوية مخزن
        DeductStock = 6,     // إذن خصم مخزن
        AddStock = 7         // إذن إضافة مخزن
    }
}
