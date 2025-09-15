using System.Data;
using System.Windows.Forms;

namespace MizanOriginalSoft.MainClasses.SearchClasses
{
    public interface ISearchProvider
    {
        /// <summary>عنوان شاشة البحث</summary>
        string Title { get; }

        /// <summary>إرجاع البيانات بناءً على الفلتر</summary>
        DataTable GetData(string filter);

        /// <summary>إرجاع الكود والاسم من الصف المحدد</summary>
        (string Code, string Name) GetSelectedItem(DataGridViewRow row);

        /// <summary>تطبيق تنسيقات الأعمدة على DataGridView</summary>
        void ApplyGridFormatting(DataGridView dgv);
    }
}
