using MizanOriginalSoft.MainClasses.SearchClasses;
using System;
using System.Data;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frmGeneralSearch : Form
    {
        private readonly ISearchProvider _provider;

        public frmGeneralSearch(ISearchProvider provider)
        {
            InitializeComponent();
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));

            // ربط الحدث يدويًا
            this.Load += frmGeneralSearch_Load;
        }

        private void frmGeneralSearch_Load(object? sender, EventArgs e)
        {
            lblTitel.Text = _provider.Title;
            LoadData();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var data = _provider.GetData(txtSearch.Text.Trim());
            DGV.DataSource = data;
            lblcountResulte.Text = $"عدد النتائج: {data.Rows.Count}";
            _provider.ApplyGridFormatting(DGV);
        }

        private void DGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string code = _provider.GetSelectedCode(DGV.Rows[e.RowIndex]);
                this.Tag = code;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}



/*
using MizanOriginalSoft.MainClasses.SearchClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frmGeneralSearch : Form
    {
        private ISearchProvider _provider;

        public frmGeneralSearch(ISearchProvider provider)
        {
            InitializeComponent();
            _provider = provider;
        }

        private void frmGeneralSearch_Load(object sender, EventArgs e)
        {
            lblTitel.Text = _provider.Title;
            LoadData();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var data = _provider.GetData(txtSearch.Text.Trim());
            DGV.DataSource = data;
            lblcountResulte.Text = $"عدد النتائج: {data.Rows.Count}";
            _provider.ApplyGridFormatting(DGV);
        }

        private void DGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string code = _provider.GetSelectedCode(DGV.Rows[e.RowIndex]);
                this.Tag = code;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }

}
*/

/*

frmGeneralSearch
- يتم استدعائها من اى مكان بالبرنامج بغرض البحث فى اى مجموعات
- فى الحسابات او فى الاصناف بانواعها وتصنيفاتها 
- مطلوب استدعائها باضغط على ctrl+F فى المكان المراد بحث فيه
- بها الكائنات الاتية txtSearch , lblcountResulte ,lblTitel  , DGV 
- عند  الفتح يتم تحديد ما الذى ابحث عنه SearchInWate
- بناء على ذلك يتم تعبئة العنوان بما سيتم البحث فيه فى الليبل lblTitel
- فتتم تعبئة الجريد به DGV
- ثم يتم البحث فيه حسب طبيعته من خلال التكست txtSearch
- يتم تحديث عدد النتائج الظاهرة فى الجريد فى الليبل lblcountResulte
- بالنقر المزدوج على الصف المختار فى البحث يتم نقل كوده حسب نوعه الى الشاشة التى فتحت منها واغلاق شاشة البحث
- فى الجريد يكون هناك تنسيقات خاصة لكل نوع بحث  ويكون هناك تنسيقات عامة كالخطوط وترادف الوان الاسطر فى الجريد

  فما السيناريو الذى تقترحه لكي يكون كود محترف ببنية سهلة التطوير والصيانة







 الان عندى نوعين من البحث ويمكن الزيادة فى المستقبل
-البحث فى الحسابات حسب الاب المراد :عملاء -موردين -كلاهما معا-الملاك او الشركاء -العاملون الموظفون وهكذا
-والنوع الثانى للبحث داخل جدول الاصناف 

🔹 الخطة المقترحة
- AccountsSearchProvider →   كلاس  الحسابات
- ProductsSearchProvider → واخر للتعامل مع الأصناف والقطع
- ISearchProvider interface للتعامل معهما
- frmGeneralSearch  مما يجعل شاشة البحث عامة تمامًا.
- انشاء كلاس جديد لاى نوع بحث مختلف فى المستقبل ويتم التعامل معه بهذا السيناريو





// للبحث في الحسابات (عملاء)
private void btnSearchAccounts_Click(object sender, EventArgs e)
{
    var provider = new AccountsSearchProvider(parentAccountType: 1); // نوع الحساب
    using (var frm = new frmGeneralSearch(provider))
    {
        if (frm.ShowDialog() == DialogResult.OK)
            txtAccountID.Text = frm.Tag.ToString();
    }
}

// للبحث في الأصناف
private void btnSearchProducts_Click(object sender, EventArgs e)
{
    var provider = new ProductsSearchProvider();
    using (var frm = new frmGeneralSearch(provider))
    {
        if (frm.ShowDialog() == DialogResult.OK)
            txtProductID.Text = frm.Tag.ToString();
    }
}


 */