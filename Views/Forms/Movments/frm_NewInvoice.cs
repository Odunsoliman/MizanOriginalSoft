#region ******* Using ****************
using Microsoft.CodeAnalysis;
using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.SearchClasses;
using MizanOriginalSoft.Views.Forms.Accounts;
using MizanOriginalSoft.Views.Forms.MainForms;
using MizanOriginalSoft.Views.Forms.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MizanOriginalSoft.MainClasses.OriginalClasses;

using MizanOriginalSoft.MainClasses.Enums; // هنا يوجد enum InvoiceType



#endregion 
namespace MizanOriginalSoft.Views.Forms.Movments
{

    public partial class frm_NewInvoice : Form
    {
        #region 🔹 المتغيرات العامة
        // 👇 غير النوع هنا
        private InvoiceType currentInvoiceType;

        // حقول قبل وبعد البحث
        private readonly List<Control> inputFieldsBeforeSearch = new();
        private readonly List<Control> inputFieldsAfterSearch = new();

        // جداول المنتجات
        private DataTable tblProd = new();
        private DataTable tblProdPieces = new();
        DataTable tblInvDetails = new DataTable();
        // متغيرات بسيطة
        private string unit = string.Empty;
        public string SelectedAccID { get; set; } = string.Empty;

        // معرفات
        private int US;          // كود المستخدم
        private int Inv_ID;      // رقم الفاتورة
        private int ID_Prod;
        private int Piece_id = 0;

        private DataTable? tblAcc = null;
        private DataTable? tblAccSals = null;

        // متغيرات مالية
        private int PieceID;
        private float PriceMove;
        private float Amount;
        private float TotalRow;
        private float GemDisVal;
        private float ComitionVal = 0;
        private float NetRow;

        //private InvoiceType currentInvoiceType;
        private KeyboardLanguageManager langManager;

        // أنواع الفواتير
        //public enum InvoiceType
        //{
        //    Sale = 1,            // بيع
        //    SaleReturn = 2,      // بيع مرتد
        //    Purchase = 3,        // شراء
        //    PurchaseReturn = 4,  // شراء مرتد
        //    Inventory = 5,       // إذن جرد
        //    DeductStock = 6,     // إذن خصم
        //    AddStock = 7         // إذن إضافة
        //}

        #endregion

        #region 🔹 التهيئة والتحميل

        public frm_NewInvoice(int type_ID)
        {
            InitializeComponent();

            // 👌 التحويل الصحيح
            currentInvoiceType = (InvoiceType)type_ID;

            langManager = new KeyboardLanguageManager(this);

            ConnectEventsFooter();  // ربط الأحداث
            US = CurrentSession.UserID;
        }
   
        /// <summary>
        /// ربط الأحداث الخاصة بالحقول والفوتر
        /// </summary>
        private void ConnectEventsFooter()
        {
            // أحداث الحقول الرقمية
            txtDiscount.Leave += txtDiscount_Leave;
            txtTaxVal.Leave += txtTaxVal_Leave;
            txtValueAdded.Leave += txtValueAdded_Leave;

            txtPayment_Cash.Leave += txtPayment_Cash_Leave;
            txtPayment_Electronic.Leave += txtPayment_Electronic_Leave;

            // أحداث الداتا جريد
     //       DGV.CellEndEdit += DGV_CellEndEdit;
            DGV.EditingControlShowing += DGV_EditingControlShowing;
            DGV.KeyDown += DGV_KeyDown;
            DGV.CellFormatting += DGV_CellFormatting;

            // اللغة (عربي تلقائيًا)
            txtNoteInvoice.Enter += (s, e) => langManager.SetArabicLanguage();
            txtPayment_Note.Enter += (s, e) => langManager.SetArabicLanguage();
        }

        /// <summary>
        /// عند تحميل الفورم
        /// </summary>
        private void frm_NewInvoice_Load(object sender, EventArgs e)
        {
            DBServiecs.A_UpdateAllDataBase();   // تحديث أرصدة الأصناف والحسابات
            LoadAcc();                          // تحميل الحسابات
            SetDefaultAccount();                // تعيين الحساب الافتراضي
            InitializeAutoComplete();           // إعداد الإكمال التلقائي
            GetSalseMan();                      // جلب البائعين / المنفذين
            InvTypeData();                      // تحميل بيانات النوع
            DGVStyl();                          // تنسيق الداتا جريد
            RegisterEvents();                   // ربط أحداث إضافية
        }
        private void InitializeAutoComplete()
        {
            if (tblAcc == null || tblAcc.Rows.Count == 0)
                return;

            var accNames = new AutoCompleteStringCollection();

            accNames.AddRange(
                tblAcc.AsEnumerable()
                      .Select(r => r["AccName"]?.ToString())
                      .Where(name => !string.IsNullOrEmpty(name))
                      .ToArray()!);

            txtAccName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtAccName.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtAccName.AutoCompleteCustomSource = accNames;
        }

        private void GetSalseMan()
        {
            string sellerType = InvoiceTypeHelper.ToAccountTypeString(currentInvoiceType);
            if (string.IsNullOrEmpty(sellerType))
                return;

            DataTable result = DBServiecs.NewInvoice_GetAcc(sellerType);
            tblAccSals = result;

            cbxSellerID.DataSource = tblAccSals;
            cbxSellerID.DisplayMember = "AccName";
            cbxSellerID.ValueMember = "AccID";
        }





        private void RegisterEvents()
        {
            foreach (Control ctrl in inputFieldsBeforeSearch.Concat(inputFieldsAfterSearch))
            {
                ctrl.KeyDown += InputFields_KeyDown;
                ctrl.Leave += InputFields_Leave;
            }
        }
        #endregion

        #region Footer Leave Handlers

        private void txtDiscount_Leave(object? sender, EventArgs e)
        {
            if (decimal.TryParse(txtDiscount.Text, out var amount))
            {
                if (amount == 0m)
                {
                    lblDiscountRate.Text = "0.00";
                }
                else if (decimal.TryParse(lblTotalValueAfterTax.Text, out var baseVal) && baseVal > 0m)
                {
                    lblDiscountRate.Text = Math.Round((amount / baseVal) * 100m, 2).ToString("N2");
                }
                else
                {
                    txtDiscount.Text = "0.00";
                    lblDiscountRate.Text = "0.00";
                    CustomMessageBox.ShowInformation("يجب إدخال قيمة للفاتورة بعد الضريبة أولًا قبل حساب الخصم.", "تنبيه");
                    txtDiscount.Focus();
                }
            }
            CalculateInvoiceFooter();
        }

        private void txtTaxVal_Leave(object? sender, EventArgs e)
        {
            if (decimal.TryParse(txtTaxVal.Text, out var amount))
            {
                if (amount == 0m)
                {
                    lblTaxRate.Text = "0.00";
                }
                else if (decimal.TryParse(lblTotalInv.Text, out var baseVal) && baseVal > 0m)
                {
                    lblTaxRate.Text = Math.Round((amount / baseVal) * 100m, 2).ToString("N2");
                }
                else
                {
                    txtTaxVal.Text = "0.00";
                    lblTaxRate.Text = "0.00";
                    CustomMessageBox.ShowInformation("يجب إدخال قيمة للفاتورة أولًا قبل حساب نسبة الإضافة.", "تنبيه");
                    txtTaxVal.Focus();
                }
            }
            CalculateInvoiceFooter();
        }

        private void txtValueAdded_Leave(object? sender, EventArgs e)
        {
            if (decimal.TryParse(txtValueAdded.Text, out var amount))
            {
                if (amount == 0m)
                {
                    lblAdditionalRate.Text = "0.00";
                }
                else if (decimal.TryParse(lblTotalValueAfterTax.Text, out var baseVal) && baseVal > 0m)
                {
                    lblAdditionalRate.Text = Math.Round((amount / baseVal) * 100m, 2).ToString("N2");
                }
                else
                {
                    txtValueAdded.Text = "0.00";
                    lblAdditionalRate.Text = "0.00";
                    CustomMessageBox.ShowInformation("يجب إدخال قيمة للفاتورة بعد الضريبة أولًا قبل حساب الإضافة.", "تنبيه");
                    txtValueAdded.Focus();
                }
            }
            CalculateInvoiceFooter();
        }

        private void txtPayment_Cash_Leave(object? sender, EventArgs e)
        {
            CalculateRemainingOnAccount();
        }

        private void txtPayment_Electronic_Leave(object? sender, EventArgs e)
        {
            CalculateRemainingOnAccount();
        }

        #endregion

        #region 🔹 إعدادات نوع الفاتورة

        /// <summary>
        /// كلاس يحدد إعدادات الواجهة حسب نوع الفاتورة
        /// </summary>
        private class InvoiceUIConfig
        {
            public string TypeText { get; set; } = "";
            public string DirText { get; set; } = "";
            public Color BackColor { get; set; } = SystemColors.Window;
            public bool AllowNegative { get; set; }
            public bool ShowPriceMove { get; set; }
            public string ProductNameText { get; set; } = "";
            public string CodeTitle { get; set; } = "";
            public bool ShowGemDisVal { get; set; }
        }
        private void LoadAcc()
        {
            // استخدم enum مع الدالة
            string invoiceTypeKey = InvoiceTypeHelper.ToAccountTypeString(currentInvoiceType);

            if (string.IsNullOrEmpty(invoiceTypeKey))
                return;

            // استدعاء الإجراء المخزن
            DataTable result = DBServiecs.NewInvoice_GetAcc(invoiceTypeKey);
            
            // تصفية الحسابات عميل نقدى =55 مورد عام نقدى=56 تسوية حساب المخزون=72
            DataRow[] filteredRows = result.Select("AccID > 200 OR AccID IN (55, 56, 72)"); 
            // لا افهم عما تعبر كيف اكتشف ذلك
            tblAcc = filteredRows.Length > 0 ? filteredRows.CopyToDataTable() : result.Clone();
        }




        // إعدادات لكل نوع فاتورة (بدل switch متكرر)
        private readonly Dictionary<InvoiceType, InvoiceUIConfig> invoiceConfigs = new()
        {
            [InvoiceType.Sale] = new InvoiceUIConfig
            {
                TypeText = "فاتورة بيع رقم :",
                DirText = "البائع :",
                BackColor = Color.LightGreen,
                AllowNegative = true,
                ShowPriceMove = true,
                ProductNameText = "Product Name :",
                CodeTitle = "كود صنف",
                ShowGemDisVal = true
            },
            [InvoiceType.SaleReturn] = new InvoiceUIConfig
            {
                TypeText = "فاتورة بيع مرتد رقم :",
                DirText = "البائع :",
                BackColor = Color.MistyRose,
                ShowPriceMove = false,
                CodeTitle = "فاتورة بيع رقم",
                ShowGemDisVal = false
            },
            [InvoiceType.Purchase] = new InvoiceUIConfig
            {
                TypeText = "فاتورة شراء رقم :",
                DirText = "منفذ الشراء:",
                BackColor = Color.LightSkyBlue,
                ShowPriceMove = true,
                ProductNameText = "Product Name :",
                CodeTitle = "كود صنف",
                ShowGemDisVal = false
            },
            [InvoiceType.PurchaseReturn] = new InvoiceUIConfig
            {
                TypeText = "فاتورة شراء مرتد رقم :",
                DirText = "منفذ الشراء:",
                BackColor = Color.LemonChiffon,
                ShowPriceMove = false,
                CodeTitle = "فاتورة شراء رقم",
                ShowGemDisVal = false
            },
            [InvoiceType.Inventory] = new InvoiceUIConfig
            {
                TypeText = "اذن تسوية مخزن رقم:",
                DirText = "منفذ التسوية:",
                BackColor = Color.LightGray,
                ShowPriceMove = false,
                CodeTitle = "كود الصنف",
                ShowGemDisVal = false
            },
            [InvoiceType.DeductStock] = new InvoiceUIConfig
            {
                TypeText = "اذن خصم مخزن رقم:",
                DirText = "منفذ التسوية:",
                BackColor = Color.LightGray,
                ShowPriceMove = false,
                CodeTitle = "كود الصنف",
                ShowGemDisVal = false
            },
            [InvoiceType.AddStock] = new InvoiceUIConfig
            {
                TypeText = "اذن إضافة مخزن رقم:",
                DirText = "منفذ التسوية:",
                BackColor = Color.LightGray,
                ShowPriceMove = false,
                CodeTitle = "كود الصنف",
                ShowGemDisVal = false
            }
        };

        /// <summary>
        /// تطبيق إعدادات النوع الحالي
        /// </summary>
        private void InvTypeData()
        {
            if (!invoiceConfigs.TryGetValue(currentInvoiceType, out var config))
                return;

            lblTypeInv.Text = config.TypeText;
            lblDir.Text = config.DirText;
            SetInvoiceColors(config.BackColor);

            chkAllowNegative.Visible = config.AllowNegative;
            lblPriceMove.Visible = config.ShowPriceMove;
            lblProductName.Text = config.ProductNameText;
            lblCodeTitel.Text = config.CodeTitle;
            lblGemDisVal.Visible = config.ShowGemDisVal;

            lblTypeInvID.Text = ((int)currentInvoiceType).ToString();
            GetInvoices();
            DGVStyl();
        }

    
        // حسابات افتراضية لكل نوع
        private readonly Dictionary<InvoiceType, string> defaultAccounts = new()
        {
            [InvoiceType.Inventory] = "72", // جرد حـ اضافة وخصم صنف
            [InvoiceType.DeductStock] = "72", // صرف حـ اضافة وخصم صنف
            [InvoiceType.AddStock] = "72", // إضافة حـ اضافة وخصم صنف
            [InvoiceType.Sale] = "55", // مبيعات حـ عميل نقدى
            [InvoiceType.SaleReturn] = "55", // مرتجع حـ عميل نقدى
            [InvoiceType.Purchase] = "56", // مشتريات حـ مورد عام نقدى
            [InvoiceType.PurchaseReturn] = "56", // مرتجع حـ مورد عام نقدى
        };

        // مثال عملي:


        /// <summary>
        /// تعيين الحساب الافتراضي
        /// </summary>
        private void SetDefaultAccount()
        {
            if (defaultAccounts.TryGetValue(currentInvoiceType, out string? defaultAccID))
            {
                lblAccID.Text = defaultAccID;

                if (tblAcc != null)
                {
                    var rows = tblAcc.Select($"AccID = {defaultAccID}");
                    if (rows.Length > 0)
                        LoadAccountData(rows[0]); 
                }
            }
        }

        #endregion

        #region 🔹 العمليات العامة للحفظ

        // التحقق من صحة الفاتورة قبل الحفظ
        private List<string> ValidateInvoice()
        {
            var missing = new List<string>();

            if (string.IsNullOrWhiteSpace(lblInv_ID.Text))
                missing.Add("رقم الفاتورة");

            if (string.IsNullOrWhiteSpace(lblInv_Counter.Text))
                missing.Add("الرقم التسلسلي للفاتورة");

            if (cbxSellerID.SelectedValue == null)
                missing.Add(currentInvoiceType is InvoiceType.Sale or InvoiceType.SaleReturn
                    ? "البائع"
                    : "منفذ الشراء / التسوية");

            if (string.IsNullOrWhiteSpace(lblAccID.Text))
                missing.Add("الحساب");

            if (CurrentSession.WarehouseId <= 0)
                missing.Add("المخزن");

            return missing;
        }

        // حفظ مسودة الفاتورة
        // 🔹 حفظ مسودة الفاتورة
        public void SaveDraftInvoice(string? savedText = null)
        {
            if (!string.IsNullOrWhiteSpace(lblSave.Text))
            {
                MessageBox.Show("الفاتورة محفوظة نهائيًا، لا يمكن التعديل.");
                return;
            }

            var missingFields = ValidateInvoice();
            if (missingFields.Count > 0)
            {
                string message = "يرجى استكمال البيانات التالية:\n• " + string.Join("\n• ", missingFields);
                CustomMessageBox.ShowWarning(message, "بيانات ناقصة");
                return;
            }

            // ✅ استدعاء الحفظ باستخدام Named Arguments بالكامل
            DBServiecs.NewInvoice_InsertOrUpdate(
                invID: Convert.ToInt32(lblInv_ID.Text),
                invCounter: lblInv_Counter.Text,
                invType_ID: (int)currentInvoiceType,
                invDate: dtpInv_Date.Value,
                seller_ID: Convert.ToInt32(cbxSellerID.SelectedValue),
                user_ID: US,
                acc_ID: int.TryParse(lblAccID.Text, out int accId) ? accId : (int?)null,
                totalValue: ToFloat(lblTotalInv.Text),
                taxVal: ToFloat(txtTaxVal.Text),
                totalValueAfterTax: ToFloat(lblTotalValueAfterTax.Text),
                discount: ToFloat(txtDiscount.Text),
                valueAdded: ToFloat(txtValueAdded.Text),
                netTotal: ToFloat(lblNetTotal.Text),
                payment_Cash: ToFloat(txtPayment_Cash.Text),
                payment_Electronic: ToFloat(txtPayment_Electronic.Text),
                payment_Note: txtPayment_Note.Text,
                remainingOnAcc: ToFloat(lblRemainingOnAcc.Text),
                noteInvoice: txtNoteInvoice.Text,
                saved: savedText ?? string.Empty,
                Warehouse_Id: CurrentSession.WarehouseId,
                resultMessage: out _ // ✅ استخدم اسم المعامل الصحيح   
            );
        }

        // 🔹 دالة مساعدة لتحويل النص إلى رقم عائم
        private static float ToFloat(object? value, float defaultVal = 0) =>
            float.TryParse(value?.ToString(), out float result) ? result : defaultVal;

   
        #endregion

        #region التنقل بين الفواتير

        // 📝 DataTable يحتوي كل الفواتير المحملة من قاعدة البيانات
        private DataTable tblInv = new DataTable();

        // 📝 مؤشر الفاتورة الحالية داخل DataTable
        private int currentInvoiceIndex = 0;

        /// <summary>
        /// الانتقال إلى أول فاتورة
        /// </summary>
        private void btnFrist_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded())
                NavigateToInvoice(0);
             
        }

        /// <summary>
        /// الانتقال إلى الفاتورة التالية
        /// </summary>
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded() && currentInvoiceIndex < tblInv.Rows.Count - 1)
                NavigateToInvoice(currentInvoiceIndex + 1);
            else
                MessageBox.Show("تم الوصول إلى آخر فاتورة.");
        }

        /// <summary>
        /// الانتقال إلى الفاتورة السابقة
        /// </summary>
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded() && currentInvoiceIndex > 0)
                NavigateToInvoice(currentInvoiceIndex - 1);
            else
                MessageBox.Show("تم الوصول إلى أول فاتورة.");
        }

        /// <summary>
        /// الانتقال إلى آخر فاتورة
        /// </summary>
        private void btnLast_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded())
                NavigateToInvoice(tblInv.Rows.Count - 1);
        }

        /// <summary>
        /// وظيفة التنقل بين الفواتير
        /// </summary>
        /// <param name="targetIndex">الفهرس المستهدف للفواتير</param>
        private void NavigateToInvoice(int targetIndex)
        {
            if (!EnsureInvoicesLoaded()) return;

            // 📝 ضبط المؤشر ليكون دائمًا داخل الحدود
            targetIndex = Math.Max(0, Math.Min(targetIndex, tblInv.Rows.Count - 1));
            currentInvoiceIndex = targetIndex;

            // تحميل بيانات الفاتورة
            DisplayCurentRow(currentInvoiceIndex);

            // تفعيل أو تعطيل الأزرار بناءً على حالة الحفظ والموقع الحالي
            ToggleControlsBasedOnSaveStatus();
            ToggleNavigationButtons();

            // تحديث شريط المعلومات
            lblInfoInvoice.Text = $"فاتورة {targetIndex + 1} من {tblInv.Rows.Count}";
        }

        /// <summary>
        /// تفعيل/تعطيل أزرار التنقل حسب موقع الفاتورة
        /// </summary>
        private void ToggleNavigationButtons()
        {
            btnFrist.Enabled = currentInvoiceIndex > 0;
            btnPrevious.Enabled = currentInvoiceIndex > 0;
            btnNext.Enabled = currentInvoiceIndex < tblInv.Rows.Count - 1;
            btnLast.Enabled = currentInvoiceIndex < tblInv.Rows.Count - 1;
        }

        /// <summary>
        /// التأكد من تحميل الفواتير
        /// </summary>
        /// <returns>True إذا كانت الفواتير متاحة، False خلاف ذلك</returns>
        private bool EnsureInvoicesLoaded()
        {
            if (tblInv == null || tblInv.Rows.Count == 0)
                GetInvoices();

            if (tblInv == null || tblInv.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد فواتير.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// عرض بيانات الفاتورة الحالية في الواجهة
        /// </summary>
        /// <param name="CIndex">فهرس الفاتورة داخل الجدول</param>
        public void DisplayCurentRow(int CIndex)
        {
            if (tblInv == null || tblInv.Rows.Count <= CIndex)
                return;

            DataRow row = tblInv.Rows[CIndex];

            // 🔹 تحميل قيم أساسية
            lblInv_ID.Text = row["Inv_ID"].ToString();
            lblInv_Counter.Text = row["Inv_Counter"].ToString();
            lblTypeInv.Text = row["MovType"].ToString(); // نوع الحركة
            Inv_ID = Convert.ToInt32(lblInv_ID.Text);

            // 🔹 المستودع (مؤقت)
            lblWarehouseName.Text = "الفرع الرئيسى";

            // 🔹 التاريخ
            if (row["Inv_Date"] != DBNull.Value)
                dtpInv_Date.Value = Convert.ToDateTime(row["Inv_Date"]);

            // 🔹 البائع / منفذ العملية
            cbxSellerID.SelectedValue = row["Seller_ID"] != DBNull.Value
                ? Convert.ToInt32(row["Seller_ID"])
                : -1;

            // 🔹 المستخدم والحساب
            lblUserID.Text = row["User_ID"].ToString();
            lblAccID.Text = row["Acc_ID"].ToString();

            // 🔹 القيم المالية
            lblTotalInv.Text = FormatNumber(row["TotalValue"]);
            txtTaxVal.Text = FormatNumber(row["TaxVal"]);
            lblTotalValueAfterTax.Text = FormatNumber(row["TotalValueAfterTax"]);
            txtDiscount.Text = FormatNumber(row["Discount"]);
            txtValueAdded.Text = FormatNumber(row["ValueAdded"]);
            lblNetTotal.Text = FormatNumber(row["NetTotal"]);

            // 🔹 المدفوعات
            txtPayment_Cash.Text = FormatNumber(row["Payment_Cash"]);
            txtPayment_Electronic.Text = FormatNumber(row["Payment_Electronic"]);
            txtPayment_Note.Text = row["Payment_Note"]?.ToString();

            // 🔹 الباقي على الحساب
            lblRemainingOnAcc.Text = FormatNumber(row["RemainingOnAcc"]);

            // 🔹 الملاحظات وحالة الحفظ
            txtNoteInvoice.Text = row["NoteInvoice"]?.ToString();
            lblSave.Text = row["Saved"]?.ToString();

            // 🔹 تحميل تفاصيل الفاتورة
            GetInvoiceDetails();
        }

        /// <summary>
        /// فتح فاتورة جديدة
        /// </summary>
        private void btnNew_Click(object sender, EventArgs e)
        {
            SetDefaultAccount();

            if (tblInv == null)
                GetInvoices();

            // الحصول على الأرقام الجديدة من قاعدة البيانات
            string nextCounter = DBServiecs.NewInvoice_GetNewCounter((int)currentInvoiceType);
            int nextID = DBServiecs.NewInvoice_GetNewID();

            // تعيين البيانات المبدئية
            lblInv_Counter.Text = nextCounter;
            lblInv_ID.Text = nextID.ToString();

            DisplayNewRow((int)currentInvoiceType, US);
            ToggleControlsBasedOnSaveStatus();
        }

        /// <summary>
        /// تجهيز واجهة فاتورة جديدة بقيم افتراضية
        /// </summary>
        public void DisplayNewRow(int invType, int userId)
        {
            dtpInv_Date.Value = DateTime.Now;
            cbxSellerID.SelectedValue = 26; // 🔹 قيمة افتراضية مؤقتة
            lblUserID.Text = userId.ToString();

            // 🔹 الحساب الافتراضي حسب نوع الفاتورة
            lblAccID.Text = invType switch
            {
                1 or 2 => "55", // بيع أو مرتجع بيع
                3 or 4 => "56", // شراء أو مرتجع شراء
                _ => "72"
            };

            // 🔹 إعادة ضبط القيم المالية
            string zero = "0";
            lblTotalInv.Text = zero;
            txtTaxVal.Text = zero;
            lblTotalValueAfterTax.Text = zero;
            txtDiscount.Text = zero;
            txtValueAdded.Text = zero;
            lblNetTotal.Text = zero;
            txtPayment_Cash.Text = zero;
            txtPayment_Electronic.Text = zero;
            lblRemainingOnAcc.Text = zero;

            // 🔹 باقي الحقول
            txtPayment_Note.Text = "";
            txtNoteInvoice.Text = "";
            lblSave.Text = "";
            txtSeaarchProd.Text = "0";
            txtAmount.Text = "0";
            lblPriceMove.Text = "0";
            lblCount.Text = "0";
            lblInfoInvoice.Text = "فاتورة جديدة";

            // 🔹 تغيير النصوص حسب النوع
            lblProductName.Text = invType is 1 or 3 ? "Product Name :" : "Invoice No :";
            lblCodeTitel.Text = invType is 1 or 3 ? "كود صنف" : "فاتورة بيع رقم";

            // 🔹 إعادة تهيئة الـ DataGridView و ComboBox
            DGV.DataSource = null;
            cbxPiece_ID.DataSource = null;
        }

        #endregion

        #region أحداث ووظائف رأس الفاتورة

        /// <summary>
        /// حدث الضغط على لوحة المفاتيح داخل مربع نص اسم الحساب
        /// </summary>
        private void txtAccName_KeyDown(object sender, KeyEventArgs e)
        {
            // ✅ فتح شاشة البحث عند الضغط على Ctrl + F
            if (e.Control && e.KeyCode == Keys.F)
            {
                // السماح فقط لأنواع الفواتير المدعومة
                if (currentInvoiceType != InvoiceType.Sale &&
                    currentInvoiceType != InvoiceType.SaleReturn &&
                    currentInvoiceType != InvoiceType.Purchase &&
                    currentInvoiceType != InvoiceType.PurchaseReturn)
                    return;

                // 🔎 اختيار نوع الحساب حسب نوع الفاتورة
                AccountKind accountKind = (currentInvoiceType == InvoiceType.Purchase ||
                                           currentInvoiceType == InvoiceType.PurchaseReturn)
                                           ? AccountKind.Suppliers
                                           : AccountKind.Customers;

                // 🔎 فتح شاشة البحث
                var provider = new GenericSearchProvider(SearchEntityType.Accounts, accountKind);
                var result = SearchHelper.ShowSearchDialog(provider);

                if (!string.IsNullOrEmpty(result.Code))
                {
                    lblAccID.Text = result.Code;
                    txtAccName.Text = result.Name;
                }

                e.SuppressKeyPress = true;
                return;
            }

            // ✅ باقي الكود كما هو...
            if (e.KeyCode == Keys.Enter)
            {
                string input = txtAccName.Text.Trim();

                if (string.IsNullOrWhiteSpace(input) || tblAcc == null)
                {
                    SetDefaultAccount();
                    return;
                }

                // فلترة الحسابات حسب الاسم أو الرموز الصوتية
                string safeInput = input.Replace("'", "''");
                string filter =
                    $"AccName = '{safeInput}' OR " +
                    $"FirstPhon = '{safeInput}' OR " +
                    $"AntherPhon = '{safeInput}'";

                DataRow[] selectedAccount = tblAcc.Select(filter);

                if (selectedAccount.Length > 0)
                {
                    LoadAccountData(selectedAccount[0]);
                    SaveDraftInvoice();
                }
                else
                {
                    DialogResult result = CustomMessageBox.ShowQuestion(
                        "الحساب غير موجود، هل تريد إضافة حساب جديد؟",
                        "حساب جديد"
                    );

                    if (result == DialogResult.OK)
                    {
                        OpenNewAccountForm();
                        LoadAcc();
                        InitializeAutoComplete();

                        txtAccName.Focus();
                        txtAccName.SelectAll();
                    }
                    else
                    {
                        SetDefaultAccount();
                    }
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
                cbxSellerID.Focus();
            }
        }


        /// <summary>
        /// فتح نموذج إضافة حساب جديد وربطه بالفاتورة
        /// </summary>
        private void OpenNewAccountForm()
        {
            string enteredName = txtAccName.Text.Trim();

            // فتح الفورم مع تمرير اسم الحساب المدخل والنوع الحالي للفاتورة
            frm_AddAccount frmNew = new frm_AddAccount(
                enteredName,
                (int)currentInvoiceType // ✅ تحويل enum إلى int
            );

            if (frmNew.ShowDialog() == DialogResult.OK)
            {
                // تحديث مصادر البيانات بعد إضافة الحساب
                LoadAcc();
                InitializeAutoComplete();

                // ربط الفاتورة بالحساب الجديد
                txtAccName.Text = frmNew.CreatedAccountName;
                lblAccID.Text = frmNew.CreatedAccountID.ToString();

                txtAccName.Focus();
                txtAccName.SelectAll();
            }
        }

        #endregion

        #region أحداث ووظائف إضافة صنف

        /// <summary>
        /// إدخال صنف جديد إلى تفاصيل الفاتورة وحفظه في قاعدة البيانات.
        /// </summary>
        public string InvoiceDetails_Insert()
        {
            GetVar(); // تحميل المتغيرات الأساسية من الواجهة

            string message = DBServiecs.InvoiceDetails_Insert(
                (int)currentInvoiceType, Inv_ID, PieceID, PriceMove, Amount,
                TotalRow, GemDisVal, ComitionVal, NetRow, 0
            );

            DGVStyl(); // إعادة تهيئة تصميم الجدول
            return message;
        }

        /// <summary>
        /// تحميل بيانات القطع الخاصة بالصنف (في حال كان المنتج يقبل القص).
        /// </summary>
        private void LoadPieceData()
        {
            cbxPiece_ID.Visible =
                (currentInvoiceType == InvoiceType.Sale && isCanCut);

            if (unit_ID == 1) // المنتج يقبل القص
            {
                tblProdPieces = DBServiecs.Product_GetOrCreatePieces(ID_Prod);
                DataRow[] filtered = tblProdPieces.Select("Piece_Length <> 0");

                if (filtered.Length > 0)
                {
                    cbxPiece_ID.DataSource = filtered.CopyToDataTable();
                    cbxPiece_ID.DisplayMember = "Piece_Length";
                    cbxPiece_ID.ValueMember = "Piece_ID";

                    if (cbxPiece_ID.Visible)
                    {
                        cbxPiece_ID.DroppedDown = true;
                        cbxPiece_ID.Focus();
                    }
                    else
                    {
                        txtAmount.Focus();
                    }
                }
                else
                {
                    cbxPiece_ID.DataSource = null;
                    MessageBox.Show("لا توجد أرصدة بهذا الصنف.");
                    txtAmount.Focus();
                }
            }
            else // المنتج لا يقبل القص
            {
                DataTable piece = DBServiecs.Product_GetOrCreate_DefaultPiece(ID_Prod);
                if (piece.Rows.Count > 0)
                    lblPieceID.Text = piece.Rows[0]["Piece_ID"].ToString();

                txtAmount.Focus();
            }
        }

        /// <summary>
        /// تحميل بيانات منتج حسب كوده.
        /// </summary>
        private bool GetProd(string code)
        {
            txtAmount.Text = "0";

            tblProd = DBServiecs.Item_GetProductByCode(code, out string msg);

            if (tblProd == null || tblProd.Rows.Count == 0)
            {
                MessageBox.Show(msg, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EmptyProdData();
                return false;
            }

            DataRow row = tblProd.Rows[0];

            // ✅ السعر حسب نوع الفاتورة
            lblPriceMove.Text = (currentInvoiceType == InvoiceType.Sale ||
                                 currentInvoiceType == InvoiceType.SaleReturn)
                ? row["U_Price"].ToString()
                : row["B_Price"].ToString();

            // ✅ البيانات العامة
            ID_Prod = Convert.ToInt32(row["ID_Product"]);
            lblProductName.Text = row["ProdName"].ToString();
            unit_ID = Convert.ToInt32(row["UnitID"]);
            unit = (row["UnitProd"]?.ToString() ?? "").Trim();
            lblProductStock.Text = row["ProductStock"].ToString();

            // الطول الأدنى (للمنتجات القابلة للقص)
            lblMinLinth.Text = unit_ID == 1 ? row["MinLenth"].ToString() : "";
            lblLinthText.Text = unit_ID == 1 ? "اقل طول" : unit;

            isCanCut = (unit_ID == 1);
            cbxPiece_ID.Visible = (currentInvoiceType == InvoiceType.Sale && isCanCut);

            return true;
        }

        /// <summary>
        /// حدث إدخال الكمية (Enter في txtAmount).
        /// </summary>
        private void txtAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || IsInvoiceSaved()) return;

            int currentIndexBeforeInsert = currentInvoiceIndex;
            SaveDraftInvoice(); // حفظ الفاتورة مؤقتًا

            if (!TryGetValidAmount(out float amount))
            {
                CustomMessageBox.ShowWarning("يرجى إدخال كمية صحيحة للمنتج", "خطأ");
                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }

            // التحقق من طول القطعة عند البيع
            float.TryParse(cbxPiece_ID.Text, out float pieceLength);
            if (currentInvoiceType == InvoiceType.Sale && unit_ID == 1 && pieceLength == 0)
            {
                CustomMessageBox.ShowWarning("يرجى اختيار طول القطعة", "خطأ");
                cbxPiece_ID.Focus();
                return;
            }

            // إدراج حسب نوع الفاتورة
            switch (currentInvoiceType)
            {
                case InvoiceType.Sale:
                    InsertSaleRow(amount, pieceLength);
                    break;

                case InvoiceType.Purchase:
                    InsertPurchaseRow(amount);
                    break;

                case InvoiceType.Inventory:
                    InsertInventoryRow(amount);
                    break;

                default:
                    CustomMessageBox.ShowWarning("نوع الفاتورة غير مدعوم", "خطأ");
                    return;
            }

            // ✅ تحديثات بعد الإدخال
            DBServiecs.A_UpdateAllDataBase();
            PrepareSaleProduct(txtSeaarchProd.Text);
            GetInvoices();
            NavigateToInvoice(currentIndexBeforeInsert);
            CalculateInvoiceFooter();
        }

        /// <summary>
        /// حدث إدخال كود المنتج أو رقم فاتورة مرتجعة.
        /// </summary>
        private void txtSeaarchProd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                var provider = new GenericSearchProvider(SearchEntityType.Products);

                var result = SearchHelper.ShowSearchDialog(provider);

                if (!string.IsNullOrEmpty(result.Code))
                {
                    txtSeaarchProd .Text = result.Code;
                }
            }


            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(txtSeaarchProd.Text))
            {
                if (IsInvoiceSaved()) return;

                string code = txtSeaarchProd.Text.Trim();

                switch (currentInvoiceType)
                {
                    case InvoiceType.Sale:
                        PrepareSaleProduct(code);
                        break;

                    case InvoiceType.SaleReturn:
                    case InvoiceType.PurchaseReturn:
                    case InvoiceType.Inventory:
                        OpenReturnedInvoiceForm(code);
                        break;

                    case InvoiceType.Purchase:
                        PreparePurchaseProduct(code);
                        break;

                    default:
                        CustomMessageBox.ShowWarning(
                            "نوع الفاتورة غير مدعوم في هذه العملية", "خطأ"
                        );
                        break;
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// تجهيز منتج لفاتورة شراء.
        /// </summary>
        private void PreparePurchaseProduct(string code)
        {
            if (!GetProd(code)) return;

            float price = float.TryParse(lblPriceMove.Text, out float result) ? result : 0;

            if (price <= 0)
            {
                CustomMessageBox.ShowWarning("يرجى تحديد سعر شراء صالح.", "تنبيه");
                return;
            }

            txtAmount.Focus();
            txtAmount.SelectAll();
        }

        /// <summary>
        /// إدراج منتج في فاتورة شراء.
        /// </summary>
        private void InsertPurchaseRow(float amount)
        {
            if (amount <= 0)
            {
                CustomMessageBox.ShowWarning("الرجاء إدخال كمية صحيحة أكبر من الصفر.", "تنبيه");
                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }

            InsertRow(unit_ID == 1);
            AfterInsertActions();
            DGVStyl();
        }

        /// <summary>
        /// إدراج منتج في فاتورة بيع.
        /// </summary>
        private void InsertSaleRow(float amount, float pieceLength)
        {
            if (unit_ID == 1) // منتج يقبل القص
            {
                float minLength = float.Parse(lblMinLinth.Text);
                float remaining = pieceLength - amount;

                if (remaining >= minLength || remaining == 0)
                {
                    InsertRow(true);
                    AfterInsertActions();
                }
                else
                {
                    DialogResult result = CustomMessageBox.ShowQuestion(
                        $"لا يجوز أن تكون القطعة المتبقية أقل من الحد الأدنى: {minLength}\nهل تريد المتابعة بالرغم من ذلك؟",
                        "تنبيه"
                    );

                    if (result == DialogResult.OK)
                    {
                        InsertRow(true);
                        AfterInsertActions();
                    }
                    else
                    {
                        txtAmount.Focus();
                        txtAmount.SelectAll();
                    }
                }
            }
            else // منتج لا يقبل القص
            {
                if (float.TryParse(lblProductStock.Text, out float stock))
                {
                    if (amount > stock && !chkAllowNegative.Checked)
                    {
                        CustomMessageBox.ShowWarning(
                            "الكمية المطلوبة أكبر من الرصيد ولا يسمح بالبيع على المكشوف.",
                            "تنبيه"
                        );
                        txtAmount.Focus();
                        txtAmount.SelectAll();
                        return;
                    }
                }

                if (float.TryParse(lblMinLinth.Text, out float minLength2))
                {
                    txtAmount.Text = (amount * minLength2).ToString();
                }

                InsertRow(false);
                AfterInsertActions();
            }
        }

        /// <summary>
        /// إعداد منتج لعملية بيع.
        /// </summary>
        private void PrepareSaleProduct(string code)
        {
            if (!GetProd(code)) return;
            LoadPieceData();
        }

        /// <summary>
        /// إدراج منتج في فاتورة جرد أو تسوية.
        /// </summary>
        private void InsertInventoryRow(float amount)
        {
            InsertRow(unit_ID == 1);
            AfterInsertActions();
        }

        /// <summary>
        /// فتح فاتورة مرتجعة حسب رقمها.
        /// </summary>
        private void OpenReturnedInvoiceForm(string serial)
        {
            if (!int.TryParse(serial, out int serInv))
            {
                CustomMessageBox.ShowWarning("الرجاء إدخال رقم فاتورة صالح.", "تنبيه");
                return;
            }

            DataTable tblInvoice = DBServiecs.NewInvoice_GetInvoiceByTypeAndCounter(
                1, serInv, out string? msg
            );

            if (!string.IsNullOrWhiteSpace(msg))
            {
                CustomMessageBox.ShowWarning(msg, "تنبيه");
                return;
            }

            if (tblInvoice == null || tblInvoice.Rows.Count == 0)
            {
                CustomMessageBox.ShowWarning("لم يتم العثور على الفاتورة.", "تنبيه");
                return;
            }

            if (!int.TryParse(tblInvoice.Rows[0]["Inv_ID"]?.ToString(), out int Inv_ID))
            {
                CustomMessageBox.ShowWarning("فشل في قراءة رقم الفاتورة.", "خطأ");
                return;
            }

            DataTable tblDetails = DBServiecs.NewInvoice_GetInvoiceDetails(Inv_ID);

            if (!int.TryParse(lblInv_ID.Text, out int CurrentInvoiceID))
            {
                CustomMessageBox.ShowWarning("رقم الفاتورة الحالي غير صالح.", "خطأ");
                return;
            }

            using (frm_ReturnedInvoice returnedForm = new frm_ReturnedInvoice(
                1, serInv, tblInvoice, tblDetails, CurrentInvoiceID))
            {
                if (returnedForm.ShowDialog() == DialogResult.OK)
                {
                    LoadReturnedItems(returnedForm.SelectedItems);
                }
            }

            DGVStyl();
        }

        /// <summary>
        /// تحميل الأصناف المرتجعة إلى الجدول.
        /// </summary>
        private void LoadReturnedItems(DataTable returnedItems)
        {
            foreach (DataRow row in returnedItems.Rows)
            {
                DGV.Rows.Add(
                    row["ProdID"],
                    row["ProdName"],
                    row["Piece_ID"],
                    row["Amount"],
                    row["Price"],
                    row["Total"]
                );
            }
        }

        #endregion

        // ====================================================
        // 🔹 أحداث ووظائف تذييل الفاتورة (الحسابات النهائية)
        // ====================================================
        #region  احداث ووظائف تذييل الفاتورة

        /// <summary>
        /// حساب الرصيد المتبقي على العميل (أو له) بعد إدخال المدفوعات.
        /// - يجمع المدفوعات النقدية والإلكترونية.
        /// - يخصمها من صافي الفاتورة.
        /// - يعرض النتيجة مع تنسيق عشري (2 رقم).
        /// - يغير النص واللون تبعًا للحالة:
        ///   • باقي عليه (أحمر) | باقي له (أخضر) | تم السداد (أزرق).
        /// </summary>
        private void CalculateRemainingOnAccount()
        {
            // 🟦 التحويل إلى أرقام بطريقة آمنة (في حالة الحقول فارغة)
            decimal.TryParse(lblNetTotal.Text, out decimal netTotal);
            decimal.TryParse(txtPayment_Cash.Text, out decimal cash);
            decimal.TryParse(txtPayment_Electronic.Text, out decimal electronic);

            // 🟦 حساب المتبقي
            decimal paid = cash + electronic;
            decimal remaining = netTotal - paid;

            // 🟦 عرض الرصيد المتبقي
            lblRemainingOnAcc.Text = remaining.ToString("N2");

            // 🟦 تحديد الحالة اللونية
            if (remaining > 0)
            {
                lblStateRemaining.Text = "باقي عليه";
                lblStateRemaining.ForeColor = Color.Red;
                lblRemainingOnAcc.ForeColor = Color.Red;
            }
            else if (remaining < 0)
            {
                lblStateRemaining.Text = "باقي له";
                lblStateRemaining.ForeColor = Color.Green;
                lblRemainingOnAcc.ForeColor = Color.Green;
            }
            else
            {
                lblStateRemaining.Text = "تم السداد";
                lblStateRemaining.ForeColor = Color.Blue;
                lblRemainingOnAcc.ForeColor = Color.Blue;
            }
        }

        /// <summary>
        /// تحديث كل القيم أسفل الفاتورة (الإجمالي - الضريبة - الخصم - الصافي - المتبقي).
        /// يعتمد على بيانات الجدول (NetRow) + مدخلات المستخدم للضرائب والمدفوعات.
        /// </summary>
        private void CalculateInvoiceFooter()
        {
            if (DGV.DataSource is not DataTable dt) return; // حماية من null

            // 🟦 حساب إجمالي الصفوف
            decimal total = 0;
            foreach (DataRow row in dt.Rows)
                if (row["NetRow"] != DBNull.Value)
                    total += Convert.ToDecimal(row["NetRow"]);
            lblTotalInv.Text = total.ToString("N2");

            // 🟦 قراءة الضريبة والخصومات والإضافات
            decimal.TryParse(txtTaxVal.Text, out var tax);
            decimal.TryParse(txtDiscount.Text, out var discount);
            decimal.TryParse(txtValueAdded.Text, out var added);

            // 🟦 الإجمالي بعد الضريبة
            var afterTax = total + tax;
            lblTotalValueAfterTax.Text = afterTax.ToString("N2");

            // 🟦 الصافي النهائي
            var net = total + tax - discount + added;
            lblNetTotal.Text = net.ToString("N2");

            // 🟦 المدفوعات
            decimal.TryParse(txtPayment_Cash.Text, out var cash);
            decimal.TryParse(txtPayment_Electronic.Text, out var visa);

            // 🟦 المتبقي
            var remaining = net - (cash + visa);
            lblRemainingOnAcc.Text = remaining.ToString("N2");

            // 🟦 تحديث الحالة النصية واللونية
            CalculateRemainingOnAccount();
        }
        #endregion


        // ====================================================
        // 🔹 تنسيق وأحداث DataGridView
        // ====================================================
        #region  تنسيق واحداث  DataGridView

        /// <summary>
        /// عند تغيير التحديد في الجدول:
        /// - إذا الصف فارغ → إفراغ الحقول.
        /// - إذا يحتوي بيانات → عرض الحد الأدنى للطول والمخزون.
        /// </summary>
        private void DGV_SelectionChanged(object? sender, EventArgs e)
        {
            if (DGV.Rows.Count == 0 || DGV.CurrentRow == null || DGV.CurrentRow.IsNewRow)
            {
                lblMinLinth.Text = "";
                lblProductStock.Text = "";
                return;
            }

            lblMinLinth.Text = DGV.Columns.Contains("MinLenth")
                ? DGV.CurrentRow.Cells["MinLenth"].Value?.ToString()
                : "";

            lblProductStock.Text = DGV.Columns.Contains("ProductStock")
                ? DGV.CurrentRow.Cells["ProductStock"].Value?.ToString()
                : "";
        }

        /// <summary>
        /// عند الضغط على Enter أو Tab:
        /// - التنقل بين الأعمدة القابلة للتحرير (السعر → الكمية → الخصم).
        /// - عند اكتمال الدورة، ينتقل للسطر التالي.
        /// </summary>
        private void DGV_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true;

                var currentCell = DGV.CurrentCell;
                if (currentCell == null) return;

                string[] editableCols = { "PriceMove", "Amount", "GemDisVal" };
                int colIndex = Array.IndexOf(editableCols, currentCell.OwningColumn.Name);

                if (colIndex >= 0)
                {
                    int nextColIndex = (colIndex + 1) % editableCols.Length;
                    int rowIndex = currentCell.RowIndex;

                    if (nextColIndex == 0) // دورة كاملة
                    {
                        if (rowIndex + 1 < DGV.Rows.Count)
                            rowIndex++;
                        else
                            return;
                    }

                    DGV.CurrentCell = DGV.Rows[rowIndex].Cells[editableCols[nextColIndex]];
                }
            }
        }

        /// <summary>
        /// تنسيق الأعمدة:
        /// - خلفيات الصفوف (متناوبة).
        /// - الأعمدة القابلة للتحرير باللون الأزرق الغامق قبل الحفظ.
        /// - الأعمدة المغلقة أو بعد الحفظ باللون الرمادي.
        /// </summary>
        private void DGV_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            string columnName = DGV.Columns[e.ColumnIndex].Name;
            string[] editableColumns = { "PriceMove", "Amount", "GemDisVal" };

            bool isEditable = editableColumns.Contains(columnName);
            bool isSaved = !string.IsNullOrWhiteSpace(lblSave.Text);

            Color evenBackColor = Color.WhiteSmoke;
            Color oddBackColor = Color.LemonChiffon;

            if (e.CellStyle != null)
            {
                e.CellStyle.BackColor = (e.RowIndex % 2 == 0) ? evenBackColor : oddBackColor;

                if (isEditable && !isSaved)
                {
                    e.CellStyle.ForeColor = Color.DarkBlue;
                    e.CellStyle.Font = new Font("Times New Roman", 14, FontStyle.Bold);
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(100, 100, 100);
                    e.CellStyle.Font = new Font("Times New Roman", 14, FontStyle.Regular);
                }
            }
        }

        /// <summary>
        /// عند بدء التحرير داخل خلية:
        /// - إذا كانت من الأعمدة الثلاثة المسموح بها،
        ///   يتم تحديد النص بالكامل عند الدخول (SelectAll).
        /// </summary>
        private void DGV_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox tb)
            {
                tb.Enter -= TextBox_Enter_SelectAll; // إزالة الاشتراك القديم

                string? columnName = DGV.CurrentCell?.OwningColumn?.Name;
                if (columnName is "PriceMove" or "Amount" or "GemDisVal")
                    tb.Enter += TextBox_Enter_SelectAll;
            }
        }

        /// <summary>
        /// إعداد المظهر العام للجدول:
        /// - إظهار الأعمدة المطلوبة فقط.
        /// - التحكم في الصلاحيات (Editable / ReadOnly).
        /// - تنسيق العناوين والصفوف والألوان.
        /// </summary>
        private void DGVStyl()
        {
            if (DGV.Columns.Count == 0) return;

            DGV.SuspendLayout();
            try
            {
                DGV.ClearSelection();
                DGV.CurrentCell = null;

                DGV.ColumnHeadersVisible = true;
                DGV.EnableHeadersVisualStyles = false;
                DGV.RightToLeft = RightToLeft.Yes;

                bool allowEdit = string.IsNullOrWhiteSpace(lblSave.Text);

                foreach (DataGridViewColumn col in DGV.Columns)
                {
                    col.Visible = false;
                    col.ReadOnly = true;
                }

                var columns = new (string Name, string Header, bool Editable, float Width)[]
                {
            ("ProductCode", "الكود", false, 1),
            ("ProdName", "اسم الصنف", false, 3),
            ("UnitProd", "الوحدة", false, 1),
            ("PriceMove", "السعر", true, 1),
            ("Amount", "الكمية", true, 1),
            ("TotalRow", "الإجمالي", false, 1),
            ("GemDisVal", "الخصم", true, 1),
            ("NetRow", "الصافي", false, 1)
                };

                foreach (var (name, header, editable, width) in columns)
                {
                    if (!DGV.Columns.Contains(name)) continue;

                    var col = DGV.Columns[name];
                    col.Visible = true;
                    col.HeaderText = header;
                    col.ReadOnly = !(editable && allowEdit);
                    col.FillWeight = width;

                    if (name is "TotalRow" or "NetRow" or "PriceMove" or "Amount" or "GemDisVal")
                    {
                        col.DefaultCellStyle.Format = "N2";
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }

                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // 🟦 تنسيقات عامة
                DGV.DefaultCellStyle.Font = new Font("Times New Roman", 14);
                DGV.DefaultCellStyle.ForeColor = Color.Blue;
                DGV.DefaultCellStyle.BackColor = Color.LightYellow;
                DGV.DefaultCellStyle.SelectionBackColor = Color.SteelBlue;
                DGV.DefaultCellStyle.SelectionForeColor = Color.White;

                DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
                DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
                DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                DGV.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                DGV.RowHeadersVisible = false;
                DGV.GridColor = Color.Black;
                DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                DGV.RowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
                DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.LemonChiffon;
            }
            finally
            {
                DGV.ResumeLayout();
            }
        }
        #endregion


        // ====================================================
        // 🔹 التنقل بين الحقول (بالضغط على Enter)
        // ====================================================
        #region تنقل بين الحقول

        /// <summary>
        /// عند الضغط على Enter داخل أي حقل إدخال:
        /// - ينتقل إلى الحقل التالي بالترتيب.
        /// - إذا الحقل التالي TextBox → يتم تحديد النص بالكامل.
        /// </summary>
        private void InputFields_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            Control[] fields = inputFieldsBeforeSearch.Concat(inputFieldsAfterSearch).ToArray();
            var index = Array.IndexOf(fields, sender as Control);

            if (index >= 0 && index < fields.Length - 1)
            {
                e.SuppressKeyPress = true;
                fields[index + 1].Focus();

                if (fields[index + 1] is TextBox tb)
                    tb.SelectAll();
            }
        }
        #endregion


        #region دوال مساعدة عامة
        /// <summary>
        /// تحديد النص بالكامل عند دخول المؤشر لحقل نصي
        /// </summary>
        private void TextBox_Enter_SelectAll(object? sender, EventArgs e)
        {
            if (sender is TextBox tb)
                tb.SelectAll();
        }

        /// <summary>
        /// تنسيق رقم عشري (مثال: 1234.5 → 1,234.50)
        /// </summary>
        private string FormatNumber(object val)
        {
            if (val == null || val == DBNull.Value)
                return "0";

            return decimal.TryParse(val.ToString(), out decimal number)
                ? number.ToString("N2")
                : "0";
        }

        /// <summary>
        /// محاولة التحويل إلى Float وإرجاع Null عند الفشل
        /// </summary>
        public static float? TryParseFloat(string text)
        {
            return float.TryParse(text, out float value) ? value : null;
        }
        #endregion

        #region تحميل وتجهيز بيانات الفاتورة
        /// <summary>
        /// جلب تفاصيل الفاتورة (أصنافها + تهيئة الجدول)
        /// </summary>
        public void GetInvoiceDetails()
        {
            if (string.IsNullOrWhiteSpace(lblInv_ID.Text) || !int.TryParse(lblInv_ID.Text, out Inv_ID))
            {
                MessageBox.Show("رقم الفاتورة غير صالح أو غير موجود.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            tblInvDetails = DBServiecs.NewInvoice_GetInvoiceDetails(Inv_ID);
            lblCount.Text = tblInvDetails?.Rows.Count.ToString() ?? "0";

            if (tblInvDetails == null || tblInvDetails.Rows.Count == 0)
            {
                PrepareEmptyGridStructure();
                DGV.DataSource = null;
            }
            else
            {
                DGV.DataSource = tblInvDetails;
            }

            DGVStyl();
            CalculateRemainingOnAccount();
        }

        /// <summary>
        /// جلب الفواتير حسب النوع + إضافة فاتورة جديدة فارغة
        /// </summary>
        private void GetInvoices()
        {
            tblInv = DBServiecs.NewInvoice_GetInvoicesByType((int)currentInvoiceType);

            DataRow newRow = tblInv.NewRow();
            newRow["Inv_ID"] = DBServiecs.NewInvoice_GetNewID();
            newRow["Inv_Counter"] = DBServiecs.NewInvoice_GetNewCounter((int)currentInvoiceType);
            newRow["MovType"] = lblTypeInv.Text;
            newRow["Inv_Date"] = DateTime.Now;
            newRow["Seller_ID"] = cbxSellerID.Items.Count > 0 ? cbxSellerID.SelectedValue : DBNull.Value;
            newRow["User_ID"] = US;
            newRow["Acc_ID"] = lblAccID.Text;

            // قيم مالية افتراضية
            newRow["TotalValue"] = 0;
            newRow["TaxVal"] = 0;
            newRow["TotalValueAfterTax"] = 0;
            newRow["Discount"] = 0;
            newRow["ValueAdded"] = 0;
            newRow["NetTotal"] = 0;
            newRow["Payment_Cash"] = 0;
            newRow["Payment_Electronic"] = 0;
            newRow["Payment_Note"] = "";
            newRow["RemainingOnAcc"] = 0;
            newRow["NoteInvoice"] = "";
            newRow["Saved"] = "";

            tblInv.Rows.Add(newRow);
            currentInvoiceIndex = tblInv.Rows.Count - 1;
            lblInfoInvoice.Text = "فاتورة جديدة";
            DisplayCurentRow(currentInvoiceIndex);
        }
        #endregion

        #region تهيئة وتصميم DataGridView
        /// <summary>
        /// إنشاء أعمدة الجدول يدوياً عند عدم وجود بيانات
        /// </summary>
        private void PrepareEmptyGridStructure()
        {
            DGV.Columns.Clear();
            DGV.AutoGenerateColumns = false;

            // أعمدة مرئية
            AddTextColumn("ProductCode", "الكود");
            AddTextColumn("ProdName", "اسم الصنف", 200);
            AddTextColumn("UnitProd", "الوحدة");
            AddTextColumn("PriceMove", "السعر", format: "N2", alignRight: true);
            AddTextColumn("Amount", "الكمية", format: "N2", alignRight: true);
            AddTextColumn("TotalRow", "الإجمالي", format: "N2", alignRight: true, readOnly: true);
            AddTextColumn("GemDisVal", "الخصم", format: "N2", alignRight: true);
            AddTextColumn("NetRow", "الصافي", format: "N2", alignRight: true, readOnly: true);

            // أعمدة مخفية مهمة للحفظ
            string[] hiddenCols =
            {
        "serInvDetail","Inv_ID_fk","PieceID_fk","AIn","AOut",
        "ReturnedInInvoiceNo","B_Price","U_Price","MinLenth","MinStock","ProductStock"
    };

            foreach (var name in hiddenCols)
                AddHiddenColumn(name);
        }

        /// <summary>
        /// إضافة عمود نصي مرئي
        /// </summary>
        private void AddTextColumn(string name, string header, int width = 100,
            string? format = null, bool alignRight = false, bool readOnly = false)
        {
            var col = new DataGridViewTextBoxColumn
            {
                Name = name,
                DataPropertyName = name,
                HeaderText = header,
                Width = width,
                ReadOnly = readOnly
            };

            if (!string.IsNullOrWhiteSpace(format))
                col.DefaultCellStyle.Format = format;

            col.DefaultCellStyle.Alignment =
                alignRight ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleCenter;
            col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DGV.Columns.Add(col);
        }

        /// <summary>
        /// إضافة عمود مخفي
        /// </summary>
        private void AddHiddenColumn(string name)
        {
            var col = new DataGridViewTextBoxColumn
            {
                Name = name,
                DataPropertyName = name,
                Visible = false,
                ReadOnly = true
            };
            DGV.Columns.Add(col);
        }
        #endregion

        #region تخصيص نوع الفاتورة
        /// <summary>
        /// ضبط بيانات العرض حسب نوع الفاتورة
        /// </summary>
        private void InvTypeData(int type_ID)
        {
            switch (type_ID)
            {
                case 1: // بيع
                    lblTypeInv.Text = "فاتورة بيع رقم :";
                    lblDir.Text = "البائع :";
                    SetInvoiceColors(Color.LightGreen);
                    chkAllowNegative.Visible = true;
                    lblPriceMove.Visible = true;
                    lblProductName.Text = "Product Name :";
                    lblCodeTitel.Text = "كود صنف";
                    lblGemDisVal.Visible = true;
                    break;

                case 2: // مرتجع بيع
                    lblTypeInv.Text = "فاتورة بيع مرتد رقم :";
                    lblDir.Text = "البائع :";
                    SetInvoiceColors(Color.MistyRose);
                    chkAllowNegative.Visible = false;
                    lblPriceMove.Visible = false;
                    lblProductName.Text = "";
                    lblCodeTitel.Text = "فاتورة بيع رقم";
                    lblGemDisVal.Visible = false;
                    break;

                case 3: // شراء
                    lblTypeInv.Text = "فاتورة شراء رقم :";
                    lblDir.Text = "منفذ الشراء:";
                    SetInvoiceColors(Color.LightSkyBlue);
                    chkAllowNegative.Visible = false;
                    lblPriceMove.Visible = true;
                    lblProductName.Text = "Product Name :";
                    lblCodeTitel.Text = "كود صنف";
                    lblGemDisVal.Visible = false;
                    break;

                case 4: // مرتجع شراء
                    lblTypeInv.Text = "فاتورة شراء مرتد رقم :";
                    lblDir.Text = "منفذ الشراء:";
                    SetInvoiceColors(Color.LemonChiffon);
                    chkAllowNegative.Visible = false;
                    lblPriceMove.Visible = false;
                    lblProductName.Text = "";
                    lblCodeTitel.Text = "فاتورة شراء رقم";
                    lblGemDisVal.Visible = false;
                    break;

                default: // تسويات مخزن أو غير معروف
                    lblTypeInv.Text = type_ID > 4 ? "اذن تسوية مخزن رقم:" : "نوع غير معروف رقم :";
                    lblDir.Text = "منفذ التسوية:";
                    SetInvoiceColors(type_ID > 4 ? Color.LightGray : SystemColors.Window);
                    break;
            }

            lblTypeInvID.Text = type_ID.ToString();
            GetInvoices();
            DGVStyl();
        }

        /// <summary>
        /// تغيير ألوان الترويسات الخاصة بالفاتورة
        /// </summary>
        private void SetInvoiceColors(Color color)
        {
            tlpHader.BackColor = color;
            tlpNotes.BackColor = color;
        }
        #endregion


        #region أحداث عناصر الواجهة (Inputs & Controls)

        // عند تغيير القطعة (Piece) يتم حفظ الـ ID في الـ Label
        private void cbxPiece_ID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxPiece_ID.SelectedValue != null)
                lblPieceID.Text = cbxPiece_ID.SelectedValue.ToString();
        }

        // عند الضغط على Enter في ComboBox البائع → حفظ مسودة الفاتورة ثم الانتقال للبحث عن المنتج
        private void cbxSellerID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveDraftInvoice();
                e.Handled = true;
                e.SuppressKeyPress = true;
                txtSeaarchProd.Focus();
                txtSeaarchProd.SelectAll();
            }
        }

        // عند الضغط على Enter في التاريخ → حفظ مسودة الفاتورة ثم الانتقال لاسم الحساب
        private void dtpInv_Date_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveDraftInvoice();
                e.Handled = true;
                e.SuppressKeyPress = true;
                txtAccName.Focus();
                txtAccName.SelectAll();
            }
        }

        // عند دخول المؤشر لحقل اسم الحساب → ضبط اللغة للعربية وتحديد النص
        private void txtAccName_Enter(object sender, EventArgs e)
        {
            langManager.SetArabicLanguage();
            txtAccName.SelectAll();
        }

        // عند دخول المؤشر لحقل ملاحظات الفاتورة → ضبط اللغة للعربية
        private void txtNoteInvoice_Enter(object sender, EventArgs e)
        {
            langManager.SetArabicLanguage();
            txtNoteInvoice.SelectAll();
        }

        // عند دخول المؤشر لحقل ملاحظات الدفع → ضبط اللغة للعربية
        private void txtPayment_Note_Enter(object sender, EventArgs e)
        {
            langManager.SetArabicLanguage();
            txtPayment_Note.SelectAll();
        }

        // عند دخول المؤشر لحقل البحث عن منتج → منع التعديل إذا كانت الفاتورة محفوظة
        private void txtSeaarchProd_Enter(object sender, EventArgs e)
        {
            if (IsInvoiceSaved()) return;
            txtSeaarchProd.SelectAll();
        }

        #endregion

        #region فحص صلاحيات وصحة البيانات

        /// <summary>
        /// تحديد إذا كان يمكن قص القطعة (true = يمكن قصها)
        /// </summary>
        public bool isCanCut = true;

        /// <summary>
        /// معرّف الوحدة الحالي
        /// </summary>
        private int unit_ID;

        /// <summary>
        /// محاولة قراءة كمية صحيحة > 0
        /// </summary>
        private bool TryGetValidAmount(out float amount)
        {
            return float.TryParse(txtAmount.Text, out amount) && amount > 0;
        }

        /// <summary>
        /// محاولة قراءة سعر صحيح > 0
        /// </summary>
        private bool TryGetValidPrice(out float price)
        {
            return float.TryParse(lblPriceMove.Text, out price) && price > 0;
        }

        /// <summary>
        /// التحقق هل الفاتورة محفوظة نهائيًا (لا يمكن تعديلها)
        /// </summary>
        private bool IsInvoiceSaved()
        {
            if (!string.IsNullOrWhiteSpace(lblSave.Text))
            {
                MessageBox.Show("الفاتورة محفوظة نهائيًا، لا يمكن التعديل.");
                return true;
            }
            return false;
        }
        #endregion

        #region عمليات إدخال الصفوف

        /// <summary>
        /// إدراج صف جديد في تفاصيل الفاتورة
        /// </summary>
        private void InsertRow(bool isPiece)
        {
            // التحقق من الكمية
            if (!TryGetValidAmount(out float amount))
            {
                CustomMessageBox.ShowWarning("يرجى إدخال كمية صحيحة للمنتج", "خطأ");
                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }

            // التحقق من السعر
            if (!TryGetValidPrice(out float priceMove))
            {
                CustomMessageBox.ShowWarning("انتبه لعدم وجود سعر للصنف", "تحذير");
            }

            // التعامل مع القطعة (قابلة للقص / غير قابلة للقص)
            if (unit_ID == 1) // قابل للقص
            {
                int newPieceID = DBServiecs.Product_CreateNewPiece(ID_Prod);
                lblPieceID.Text = newPieceID.ToString();
            }
            else // غير قابل للقص → جلب القطعة الافتراضية
            {
                DataTable piece = DBServiecs.Product_GetOrCreate_DefaultPiece(ID_Prod);
                if (piece.Rows.Count > 0)
                {
                    lblPieceID.Text = piece.Rows[0]["Piece_ID"].ToString();
                }
                else
                {
                    MessageBox.Show("لم يتم العثور على قطعة للمنتج المحدد.",
                        "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // التحقق من صلاحية معرف القطعة
            if (isPiece)
            {
                if (!int.TryParse(lblPieceID.Text, out Piece_id))
                {
                    CustomMessageBox.ShowWarning("معرف القطعة غير صالح", "خطأ");
                    return;
                }
            }
            else
            {
                Piece_id = 0;
            }

            // إدراج تفاصيل الصف
            InvoiceDetails_Insert();
            Piece_id = 0;
            GetInvoiceDetails();
        }

        /// <summary>
        /// الإجراءات التي تتم بعد إدراج صف جديد
        /// </summary>
        private void AfterInsertActions()
        {
            txtSeaarchProd.Focus();
            txtSeaarchProd.SelectAll();
            txtAmount.Text = "0";
            lblGemDisVal.Text = "0";
        }

        #endregion

        #region حساب المتغيرات وتفريغ البيانات

        /// <summary>
        /// تحميل القيم من الحقول وتحويلها إلى متغيرات رقمية
        /// </summary>
        private void GetVar()
        {
            int.TryParse(lblInv_ID.Text, out Inv_ID);
            float.TryParse(lblPriceMove.Text, out PriceMove);
            float.TryParse(txtAmount.Text, out Amount);
            float.TryParse(lblGemDisVal.Text, out GemDisVal);
            int.TryParse(lblPieceID.Text, out PieceID);

            // الحسابات الأساسية
            TotalRow = Amount * PriceMove;
            NetRow = TotalRow - GemDisVal;
        }

        /// <summary>
        /// تفريغ بيانات المنتج بعد الإضافة أو الإلغاء
        /// </summary>
        private void EmptyProdData()
        {
            txtSeaarchProd.Text = "0";
            txtSeaarchProd.Focus();
            txtSeaarchProd.SelectAll();

            lblPriceMove.Text = "0";
            lblProductName.Text = "Product Name";
            cbxPiece_ID.SelectedIndex = -1;
            txtAmount.Text = "0";
            lblGemDisVal.Text = "0";
            Piece_id = 0;
        }

        #endregion


        #region إدخال البيانات العامة وحفظ المسودة

        /// <summary>
        /// عند مغادرة أي حقل إدخال يتم الحفظ كمسودة إذا لم تكن الفاتورة محفوظة نهائيًا.
        /// </summary>
        private void InputFields_Leave(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblSave.Text)) // لم يتم الحفظ النهائي بعد
            {
                SaveDraftInvoice();
            }
        }

        /// <summary>
        /// تحميل بيانات الحساب المحدد في الحقول المخصصة له.
        /// </summary>
        private void LoadAccountData(DataRow accountData)
        {
            lblAccID.Text = accountData["AccID"].ToString();
            txtAccName.Text = accountData["AccName"].ToString();
            lblBalance.Text = accountData["Balance"].ToString();
            lblB_Status.Text = accountData["BalanceState"].ToString();
            lblFirstPhon.Text = accountData["FirstPhon"].ToString();
            lblAntherPhon.Text = accountData["AntherPhon"].ToString();
            lblClientAddress.Text = accountData["ClientAddress"].ToString();
            lblClientEmail.Text = accountData["ClientEmail"].ToString();
        }

        #endregion


        #region التحكم في تفعيل وتعطيل عناصر النموذج

        /// <summary>
        /// تعطيل أو تمكين عناصر النموذج بناءً على حالة الحفظ النهائي.
        /// </summary>
        private void ToggleControlsBasedOnSaveStatus()
        {
            bool isFinalSaved = !string.IsNullOrWhiteSpace(lblSave.Text);
            ToggleControlsRecursive(this.Controls, isFinalSaved);
            DGVStyl(); // إعادة تهيئة شكل الجدول
        }

        /// <summary>
        /// تطبيق التمكين/التعطيل بشكل متكرر على جميع عناصر التحكم.
        /// </summary>
        private void ToggleControlsRecursive(Control.ControlCollection controls, bool isFinalSaved)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is TextBox tb)
                {
                    // استثناء لبعض TextBox التي يجب أن تُغلق تمامًا
                    if (tb.Name == "txtAccName" || tb.Name == "txtSeaarchProd")
                        tb.Enabled = !isFinalSaved;
                    else
                        tb.ReadOnly = isFinalSaved;
                }
                else if (ctrl is ComboBox || ctrl is DateTimePicker)
                {
                    ctrl.Enabled = !isFinalSaved;
                }
                else if (ctrl is DataGridView dgv)
                {
                    dgv.ReadOnly = isFinalSaved;
                }

                // تكرار داخل العناصر الفرعية
                if (ctrl.HasChildren)
                    ToggleControlsRecursive(ctrl.Controls, isFinalSaved);
            }
        }

        #endregion


        #region تحديث بيانات الحساب عند تغيير رقم الحساب

        /// <summary>
        /// تحميل بيانات الحساب عند تغيير قيمة lblAccID.
        /// </summary>
        private void lblAccID_TextChanged(object sender, EventArgs e)
        {
            string accountID = lblAccID.Text.Trim();

            if (!string.IsNullOrEmpty(accountID) && tblAcc != null)
            {
                DataRow[] accountData = tblAcc.Select($"AccID = '{accountID}'");
                if (accountData.Length > 0)
                {
                    LoadAccountData(accountData[0]);
                }
                else
                {
                    CustomMessageBox.ShowWarning("لا يوجد حساب مرتبط برقم الحساب المحدد.", "خطأ");

                }
            }
        }

        #endregion


        #region أزرار الحسابات الإضافية (خصم - إضافة - ضريبة)

        /// <summary>إدخال نسبة إضافة وحساب قيمتها.</summary>
        private void btnAdditionalRate_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(lblTotalValueAfterTax.Text, out decimal baseVal) && baseVal > 0)
            {
                if (CustomMessageBox.ShowDecimalInputBox(out decimal rate, "أدخل نسبة الاضافة %", "نسبة الاضافة") == DialogResult.OK)
                {
                    txtValueAdded.Text = (baseVal * (rate / 100)).ToString("N2");
                    lblAdditionalRate.Text = rate.ToString("N2");
                    CalculateInvoiceFooter();
                }
            }
            else
            {
                CustomMessageBox.ShowInformation("لا يمكن حساب نسبة الإضافة قبل إدخال قيمة للفاتورة.", "تنبيه");
            }
        }

        /// <summary>إدخال نسبة خصم وحساب قيمتها.</summary>
        private void btnDiscountRate_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(lblTotalValueAfterTax.Text, out decimal baseVal) && baseVal > 0)
            {
                if (CustomMessageBox.ShowDecimalInputBox(out decimal rate, "أدخل نسبة الخصم %", "نسبة الخصم") == DialogResult.OK)
                {
                    txtDiscount.Text = (baseVal * (rate / 100)).ToString("N2");
                    lblDiscountRate.Text = rate.ToString("N2");
                    CalculateInvoiceFooter();
                }
            }
            else
            {
                CustomMessageBox.ShowInformation("لا يمكن حساب نسبة الخصم قبل إدخال قيمة للفاتورة.", "تنبيه");
            }
        }

        /// <summary>إدخال نسبة ضريبة وحساب قيمتها.</summary>
        private void btnTaxRate_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(lblTotalInv.Text, out decimal baseVal) && baseVal > 0)
            {
                if (CustomMessageBox.ShowDecimalInputBox(out decimal rate, "أدخل نسبة الإضافة %", "نسبة الإضافة") == DialogResult.OK)
                {
                    txtTaxVal.Text = (baseVal * (rate / 100)).ToString("N2");
                    lblTaxRate.Text = rate.ToString("N2");
                    CalculateInvoiceFooter();
                }
            }
            else
            {
                CustomMessageBox.ShowInformation("لا يمكن حساب نسبة الإضافة قبل إدخال قيمة للفاتورة.", "تنبيه");
            }
        }

        #endregion


        #region أزرار الدفع

        /// <summary>دفع كامل القيمة نقدًا.</summary>
        private void btnAllCash_Click(object sender, EventArgs e)
        {
            if (!IsValidNetTotal()) return;
            SetFullPayment(txtPayment_Cash, txtPayment_Electronic);
        }

        /// <summary>دفع كامل القيمة إلكترونيًا (بطاقة).</summary>
        private void btnAllVisa_Click(object sender, EventArgs e)
        {
            if (!IsValidNetTotal()) return;
            SetFullPayment(txtPayment_Electronic, txtPayment_Cash);
        }

        /// <summary>التأكد من أن صافي الفاتورة صالح.</summary>
        private bool IsValidNetTotal()
        {
            if (string.IsNullOrWhiteSpace(lblNetTotal.Text))
            {
                MessageBox.Show("قيمة الفاتورة غير صالحة.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(lblNetTotal.Text, out decimal netTotal) || netTotal <= 0)
            {
                MessageBox.Show("قيمة الفاتورة يجب أن تكون أكبر من صفر.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>تعيين كامل المبلغ لطريقة دفع واحدة وتصفير الأخرى.</summary>
        private void SetFullPayment(TextBox primaryMethod, TextBox secondaryMethod)
        {
            primaryMethod.Text = lblNetTotal.Text;
            secondaryMethod.Text = "0.00";
            CalculateInvoiceFooter();
        }

        #endregion


        #region أزرار الحفظ وفتح القيد

        /// <summary>حفظ الفاتورة نهائيًا.</summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(lblSave.Text))
            {
                CustomMessageBox.ShowInformation("تم حفظ الفاتورة من قبل ولا يمكن تعديلها.", "تنبيه");
                return;
            }

            int actualRowCount = DGV.Rows.Cast<DataGridViewRow>()
                                  .Count(r => !r.IsNewRow && r.Cells["ProductCode"].Value != null);

            if (actualRowCount == 0)
            {
                CustomMessageBox.ShowInformation("لا توجد بيانات لحفظ الفاتورة.", "تنبيه");
                return;
            }

            int invID = Convert.ToInt32(lblInv_ID.Text);
            string saveText = GetSaveTextByInvoiceType(currentInvoiceType);

            SaveDraftInvoice(saveText);

            lblSave.Text = saveText;
            MessageBox.Show("تم الحفظ النهائي للفاتورة.");
            ToggleControlsBasedOnSaveStatus();
        }

        /// <summary>الحصول على نص الحفظ المناسب حسب نوع الفاتورة.</summary>
        private string GetSaveTextByInvoiceType(InvoiceType invoiceType)
        {
            return invoiceType switch
            {
                InvoiceType.Sale => "تم حفظ فاتورة بيع",
                InvoiceType.SaleReturn => "تم حفظ فاتورة بيع مرتجع",
                InvoiceType.Purchase => "تم حفظ فاتورة شراء",
                InvoiceType.PurchaseReturn => "تم حفظ فاتورة شراء مرتجع",
                InvoiceType.Inventory => "تم حفظ إذن جرد",
                InvoiceType.DeductStock => "تم حفظ إذن خصم",
                InvoiceType.AddStock => "تم حفظ إذن إضافة",
                _ => "تم حفظ الفاتورة"
            };
        }

        /// <summary>فتح قيد اليومية المرتبط بالفاتورة.</summary>
        private void btnJournal_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(lblSave.Text))
            {
                if (int.TryParse(lblInv_ID.Text, out int billNo) &&
                    int.TryParse(lblTypeInvID.Text, out int invTypeId))
                {
                    frm_Journal journalForm = new frm_Journal(billNo, invTypeId);
                    journalForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("تأكد من رقم السند ونوع العملية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("يجب حفظ السند أولًا قبل عرض القيد المحاسبي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion


















    }
}
