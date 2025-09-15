namespace MizanOriginalSoft.MainClasses.SearchClasses
{
    /// <summary>نوع الكيان الذي سيتم البحث فيه</summary>
    public enum SearchEntityType
    {
        Accounts,    // البحث في الحسابات
        Products,    // البحث في الأصناف
        Categories,  // البحث في التصنيفات
        Invoices     // البحث في الفواتير
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
