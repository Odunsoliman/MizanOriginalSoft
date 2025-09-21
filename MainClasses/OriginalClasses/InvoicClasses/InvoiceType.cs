using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MizanOriginalSoft.MainClasses.OriginalClasses.InvoicClasses
{
    public enum InvoiceType
    {
        Sale = 1,            // فاتورة بيع
        SaleReturn = 2,      // فاتورة بيع مرتد
        Purchase = 3,        // فاتورة شراء
        PurchaseReturn = 4,  // فاتورة شراء مرتد
        Inventory = 5,       // إذن جرد اصناف مخزن
        DeductStock = 6,     // إذن خصم من ارصدة المخزون للاصناف
        AddStock = 7         // إذن إضافة ارصدة مخزون للاصناف
    }
}
