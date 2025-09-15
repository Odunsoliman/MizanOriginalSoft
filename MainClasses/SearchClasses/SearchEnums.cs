namespace MizanOriginalSoft.MainClasses.SearchClasses
{
    /// <summary>نوع الكيان الذي سيتم البحث فيه</summary>
    public enum SearchEntityType
    {
        Accounts,    // البحث في الحسابات
        Products,    // البحث في الأصناف
        Categories,  // البحث في التصنيفات
        SaleInvoices,     // البحث في الفواتير 
        ReSaleInvoices,     // البحث في الفواتير 
        PurchaseInvoices,     // البحث في الفواتير 
        RePurchaseInvoices,     // البحث في الفواتير 
        StockInvoices,     // البحث في الفواتير 

    }

    /// <summary>أنواع الحسابات</summary>
    public enum AccountKind
    {
        Customers,   // العملاء
        Suppliers,   // الموردين
        Boths,       // كلاهما
        Parteners    // الشركاء/المالكين
    }
}
