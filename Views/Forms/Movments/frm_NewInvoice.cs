#region ******* Using ****************
using Microsoft.CodeAnalysis;
using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
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


#endregion 
namespace MizanOriginalSoft.Views.Forms.Movments
{

    public partial class frm_NewInvoice : Form
    {
        #region 🔹 المتغيرات العامة

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

        private InvoiceType currentInvoiceType;
        private KeyboardLanguageManager langManager;

        // أنواع الفواتير
        public enum InvoiceType
        {
            Sale = 1,            // بيع
            SaleReturn = 2,      // بيع مرتد
            Purchase = 3,        // شراء
            PurchaseReturn = 4,  // شراء مرتد
            Inventory = 5,       // إذن جرد
            DeductStock = 6,     // إذن خصم
            AddStock = 7         // إذن إضافة
        }

        #endregion

        #region 🔹 التهيئة والتحميل

        public frm_NewInvoice(int type_ID)
        {
            InitializeComponent();

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
        private void GetSalseMan() // تحميل بيانات البائعين أو منفذي الشراء
        {
            string accountIDs = "";

            if (currentInvoiceType == InvoiceType.Sale || currentInvoiceType == InvoiceType.SaleReturn)
                accountIDs = "79"; // البائعين
            else
                accountIDs = "80"; // منفذو الشراء أو التسوية

            if (string.IsNullOrEmpty(accountIDs))
                return;

            DataTable result = DBServiecs.NewInvoice_GetAcc(accountIDs);
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

        // إعدادات لكل نوع فاتورة (بدل switch متكرر)
        private readonly Dictionary<InvoiceType, InvoiceUIConfig> invoiceConfigs = new()
        {
            [InvoiceType.Sale] = new InvoiceUIConfig {
                TypeText = "فاتورة بيع رقم :",
                DirText = "البائع :",
                BackColor = Color.LightGreen,
                AllowNegative = true,
                ShowPriceMove = true,
                ProductNameText = "Product Name :",
                CodeTitle = "كود صنف",
                ShowGemDisVal = true
            },
            [InvoiceType.SaleReturn] = new InvoiceUIConfig {
                TypeText = "فاتورة بيع مرتد رقم :",
                DirText = "البائع :",
                BackColor = Color.MistyRose,
                ShowPriceMove = false,
                CodeTitle = "فاتورة بيع رقم",
                ShowGemDisVal = false
            },
            [InvoiceType.Purchase] = new InvoiceUIConfig {
                TypeText = "فاتورة شراء رقم :",
                DirText = "منفذ الشراء:",
                BackColor = Color.LightSkyBlue,
                ShowPriceMove = true,
                ProductNameText = "Product Name :",
                CodeTitle = "كود صنف",
                ShowGemDisVal = false
            },
            [InvoiceType.PurchaseReturn] = new InvoiceUIConfig {
                TypeText = "فاتورة شراء مرتد رقم :",
                DirText = "منفذ الشراء:",
                BackColor = Color.LemonChiffon,
                ShowPriceMove = false,
                CodeTitle = "فاتورة شراء رقم",
                ShowGemDisVal = false
            },
            [InvoiceType.Inventory] = new InvoiceUIConfig {
                TypeText = "اذن تسوية مخزن رقم:",
                DirText = "منفذ التسوية:",
                BackColor = Color.LightGray,
                ShowPriceMove = false,
                CodeTitle = "كود الصنف",
                ShowGemDisVal = false
            },
            [InvoiceType.DeductStock] = new InvoiceUIConfig {
                TypeText = "اذن خصم مخزن رقم:",
                DirText = "منفذ التسوية:",
                BackColor = Color.LightGray,
                ShowPriceMove = false,
                CodeTitle = "كود الصنف",
                ShowGemDisVal = false
            },
            [InvoiceType.AddStock] = new InvoiceUIConfig {
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

        #endregion

        #region 🔹 تحميل الحسابات والحساب الافتراضي

        // حسابات افتراضية لكل نوع
        private readonly Dictionary<InvoiceType, string> defaultAccounts = new()
        {
            [InvoiceType.Inventory] = "83", // جرد حـ اضافة وخصم صنف
            [InvoiceType.DeductStock] = "83", // صرف حـ اضافة وخصم صنف
            [InvoiceType.AddStock] = "83", // إضافة حـ اضافة وخصم صنف
            [InvoiceType.Sale] = "81", // مبيعات حـ عميل نقدى
            [InvoiceType.SaleReturn] = "81", // مرتجع حـ عميل نقدى
            [InvoiceType.Purchase] = "82", // مشتريات حـ مورد عام نقدى
            [InvoiceType.PurchaseReturn] = "82", // مرتجع حـ مورد عام نقدى
        };


        /// <summary>
        /// تحميل الحسابات من قاعدة البيانات
        /// </summary>
        private void LoadAcc()
        {
            string accountIDs = currentInvoiceType switch
            {
                InvoiceType.Sale or InvoiceType.SaleReturn => "13,62,29",
                InvoiceType.Purchase or InvoiceType.PurchaseReturn => "23,29",
                InvoiceType.Inventory or InvoiceType.DeductStock or InvoiceType.AddStock => "15",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(accountIDs)) return;

            DataTable result = DBServiecs.NewInvoice_GetAcc(accountIDs);

            // تصفية الحسابات
            DataRow[] filteredRows = result.Select("AccID > 200 OR AccID IN (81, 82, 83)");
            tblAcc = filteredRows.Length > 0 ? filteredRows.CopyToDataTable() : result.Clone();
        }

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

        /// <summary>
        /// التحقق من صحة الفاتورة قبل الحفظ
        /// </summary>
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

        /// <summary>
        /// حفظ مسودة الفاتورة
        /// </summary>
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

            // استدعاء الحفظ
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
                payment_BankCheck: 0,
                payment_Note: txtPayment_Note.Text,
                remainingOnAcc: ToFloat(lblRemainingOnAcc.Text),
                isReturnable: false,
                noteInvoice: txtNoteInvoice.Text,
                saved: savedText ?? string.Empty,
                Warehouse_Id: CurrentSession.WarehouseId,
                out _ // تجاهل رسالة الإخراج
            );
        }

        /// <summary>
        /// دالة مساعدة لتحويل النص إلى رقم عائم
        /// </summary>
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
                1 or 2 => "81", // بيع أو مرتجع بيع
                3 or 4 => "82", // شراء أو مرتجع شراء
                _ => "83"
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

                // 🔎 فتح شاشة البحث
                
     
                e.SuppressKeyPress = true;
                return;
            }

            // ✅ عند الضغط على Enter
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
                    // حساب موجود → تحميل بياناته
                    LoadAccountData(selectedAccount[0]);
                    SaveDraftInvoice();
                }
                else
                {
                    // حساب غير موجود → عرض خيار إضافة حساب جديد
                    DialogResult result = CustomMessageBox.ShowQuestion(
                        "الحساب غير موجود، هل تريد إضافة حساب جديد؟",
                        "حساب جديد"
                    );

                    if (result == DialogResult.OK)
                    {
                        OpenNewAccountForm();
                        LoadAcc();               // تحديث الحسابات
                        InitializeAutoComplete(); // تحديث الإكمال التلقائي

                        txtAccName.Focus();
                        txtAccName.SelectAll();
                    }
                    else
                    {
                        SetDefaultAccount();
                    }
                }

                // منع مرور الحدث إلى النظام
                e.Handled = true;
                e.SuppressKeyPress = true;

                // الانتقال إلى حقل البائع
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




                e.SuppressKeyPress = true;
                return;
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

        /// <summary>
        /// عند تغيير القطعة (Piece) يتم حفظ الـ ID في الـ Label
        /// </summary>
        private void cbxPiece_ID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxPiece_ID.SelectedValue != null)
                lblPieceID.Text = cbxPiece_ID.SelectedValue.ToString();
        }

        /// <summary>
        /// عند الضغط على Enter في ComboBox البائع → حفظ مسودة الفاتورة ثم الانتقال للبحث عن المنتج
        /// </summary>
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

        /// <summary>
        /// عند الضغط على Enter في التاريخ → حفظ مسودة الفاتورة ثم الانتقال لاسم الحساب
        /// </summary>
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

        /// <summary>
        /// عند دخول المؤشر لحقل اسم الحساب → ضبط اللغة للعربية وتحديد النص
        /// </summary>
        private void txtAccName_Enter(object sender, EventArgs e)
        {
            langManager.SetArabicLanguage();
            txtAccName.SelectAll();
        }

        /// <summary>
        /// عند دخول المؤشر لحقل ملاحظات الفاتورة → ضبط اللغة للعربية
        /// </summary>
        private void txtNoteInvoice_Enter(object sender, EventArgs e)
        {
            langManager.SetArabicLanguage();
            txtNoteInvoice.SelectAll();
        }

        /// <summary>
        /// عند دخول المؤشر لحقل ملاحظات الدفع → ضبط اللغة للعربية
        /// </summary>
        private void txtPayment_Note_Enter(object sender, EventArgs e)
        {
            langManager.SetArabicLanguage();
            txtPayment_Note.SelectAll();
        }

        /// <summary>
        /// عند دخول المؤشر لحقل البحث عن منتج → منع التعديل إذا كانت الفاتورة محفوظة
        /// </summary>
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



















        /*
        private void InputFields_Leave(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblSave.Text)) // لم يتم الحفظ نهائياً
            {
                SaveDraftInvoice();
            }
        }

        private void LoadAccountData(DataRow accountData) // وضع بيانات الحساب فى اماكنها
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


        private void ToggleControlsBasedOnSaveStatus()
        {
            bool isFinalSaved = !string.IsNullOrWhiteSpace(lblSave.Text);
            ToggleControlsRecursive(this.Controls, isFinalSaved);
            DGVStyl();
        }


        private void ToggleControlsRecursive(Control.ControlCollection controls, bool isFinalSaved)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is TextBox tb)
                {
                    // استثناء لبعض الـ TextBox التي يجب تعطيلها تمامًا (لمنع اختصارات مثل Ctrl+F)
                    if (tb.Name == "txtAccName" || tb.Name == "txtSeaarchProd")
                    {
                        tb.Enabled = !isFinalSaved;
                    }
                    else
                    {
                        tb.ReadOnly = isFinalSaved;
                    }
                }
                else if (ctrl is ComboBox || ctrl is DateTimePicker)
                {
                    ctrl.Enabled = !isFinalSaved;
                }
                else if (ctrl is DataGridView dgv)
                {
                    dgv.ReadOnly = isFinalSaved;
                }

                // التكرار داخل العناصر الفرعية
                if (ctrl.HasChildren)
                {
                    ToggleControlsRecursive(ctrl.Controls, isFinalSaved);
                }
            }
        }

        private void lblAccID_TextChanged(object sender, EventArgs e)
        {
            string accountID = lblAccID.Text.Trim();

            if (!string.IsNullOrEmpty(accountID) && tblAcc != null)
            {
                // البحث عن السجل بناءً على رقم الحساب الجديد
                DataRow[] accountData = tblAcc.Select($"AccID = '{accountID}'");

                if (accountData.Length > 0)
                {
                    // تحميل بيانات الحساب إذا تم العثور على السجل
                    LoadAccountData(accountData[0]);
                }
                else
                {
                    // إذا لم يتم العثور على السجل
                    CustomMessageBox.ShowWarning("لا يوجد حساب مرتبط برقم الحساب المحدد.", "خطأ");
                }
            }
        }

        private void btnAdditionalRate_Click(object sender, EventArgs e)
        {
            // محاولة تحويل قيمة الفاتورة الإجمالية بعد الضريبة
            if (decimal.TryParse(lblTotalValueAfterTax.Text, out decimal baseVal) && baseVal > 0)
            {
                // إظهار مربع إدخال النسبة فقط إذا كانت القيمة أكبر من صفر
                if (CustomMessageBox.ShowDecimalInputBox(out decimal rate, "أدخل نسبة الاضافة %", "نسبة الاضافة") == DialogResult.OK)
                {
                    // حساب قيمة الإضافة
                    txtValueAdded.Text = (baseVal * (rate / 100)).ToString("N2");
                    lblAdditionalRate.Text = rate.ToString("N2");
                    CalculateInvoiceFooter(); // تحديث بيانات الفاتورة
                }
            }
            else
            {
                // في حالة أن القيمة صفر أو غير صالحة
                CustomMessageBox.ShowInformation("لا يمكن حساب نسبة الاضافة قبل إدخال قيمة للفاتورة.", "تنبيه");
            }

        }

        /// <summary>زر دفع كامل القيمة نقدًا</summary>
        private void btnAllCash_Click(object sender, EventArgs e)
        {
            if (!IsValidNetTotal()) return;

            SetFullPayment(txtPayment_Cash, txtPayment_Electronic);
        }

        private void btnDiscountRate_Click(object sender, EventArgs e)
        {
            // محاولة تحويل قيمة الفاتورة الإجمالية بعد الضريبة
            if (decimal.TryParse(lblTotalValueAfterTax.Text, out decimal baseVal) && baseVal > 0)
            {
                // إظهار مربع إدخال النسبة فقط إذا كانت القيمة أكبر من صفر
                if (CustomMessageBox.ShowDecimalInputBox(out decimal rate, "أدخل نسبة الخصم %", "نسبة الخصم") == DialogResult.OK)
                {
                    // حساب قيمة الإضافة
                    txtDiscount.Text = (baseVal * (rate / 100)).ToString("N2");
                    lblDiscountRate.Text = rate.ToString("N2");
                    CalculateInvoiceFooter(); // تحديث بيانات الفاتورة
                }
            }
            else
            {
                // في حالة أن القيمة صفر أو غير صالحة
                CustomMessageBox.ShowInformation("لا يمكن حساب نسبة الخصم قبل إدخال قيمة للفاتورة.", "تنبيه");
            }
            //---------------------------------------------------------
        }

        private void btnJournal_Click(object sender, EventArgs e)
        {
            // تحقق من أن السند محفوظ (أي أن lblSave تحتوي على نص)
            if (!string.IsNullOrWhiteSpace(lblSave.Text))
            {
                int billNo;
                int invTypeId;

                // تأكد من أن رقم السند ونوع الفاتورة متوفران بشكل صحيح
                if (int.TryParse(lblInv_ID.Text, out billNo) && int.TryParse(lblTypeInvID.Text, out invTypeId))
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

        private void btnTaxRate_Click(object sender, EventArgs e)
        {
            // محاولة تحويل قيمة الفاتورة الإجمالية
            if (decimal.TryParse(lblTotalInv.Text, out decimal baseVal) && baseVal > 0)
            {
                // إظهار مربع إدخال النسبة فقط إذا كانت القيمة أكبر من صفر
                if (CustomMessageBox.ShowDecimalInputBox(out decimal rate, "أدخل نسبة الإضافة %", "نسبة الإضافة") == DialogResult.OK)
                {
                    // حساب قيمة الإضافة
                    txtTaxVal.Text = (baseVal * (rate / 100)).ToString("N2");
                    lblTaxRate.Text = rate.ToString("N2");
                    CalculateInvoiceFooter(); // تحديث بيانات الفاتورة
                }
            }
            else
            {
                // في حالة أن القيمة صفر أو غير صالحة
                CustomMessageBox.ShowInformation("لا يمكن حساب نسبة الإضافة قبل إدخال قيمة للفاتورة.", "تنبيه");
            }
        }


        private void btnAllVisa_Click(object sender, EventArgs e)
        {
            if (!IsValidNetTotal()) return;
            SetFullPayment(txtPayment_Electronic, txtPayment_Cash);
        }

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

        private void SetFullPayment(TextBox primaryMethod, TextBox secondaryMethod)
        {
            primaryMethod.Text = lblNetTotal.Text;
            secondaryMethod.Text = "0.00";
            CalculateInvoiceFooter();
        }

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
            string saveText = GetSaveTextByInvoiceType(currentInvoiceType); // ✅

            SaveDraftInvoice(saveText);

            lblSave.Text = saveText;
            MessageBox.Show("تم الحفظ النهائي للفاتورة.");
            ToggleControlsBasedOnSaveStatus();
        }

        private string GetSaveTextByInvoiceType(InvoiceType invoiceType)
        {
            switch (invoiceType)
            {
                case InvoiceType.Sale: return "تم حفظ فاتورة بيع";
                case InvoiceType.SaleReturn: return "تم حفظ فاتورة بيع مرتجع";
                case InvoiceType.Purchase: return "تم حفظ فاتورة شراء";
                case InvoiceType.PurchaseReturn: return "تم حفظ فاتورة شراء مرتجع";
                case InvoiceType.Inventory: return "تم حفظ إذن جرد";
                case InvoiceType.DeductStock: return "تم حفظ إذن خصم";
                case InvoiceType.AddStock: return "تم حفظ إذن إضافة";
                default: return "تم حفظ الفاتورة";
            }
        }
*/

        /*
        #region =========== Variabls ====================


        private List<Control> inputFieldsBeforeSearch = new();
        private List<Control> inputFieldsAfterSearch = new();
  
        private DataTable tblProd = new();
        private DataTable tblProdPieces = new();
        private string unit = string.Empty;
        public string SelectedAccID { get; set; } = string.Empty;

        private int US; //كود المستخدم
        private int Inv_ID;// رقم الفاتورة
        private int ID_Prod;
        private int Piece_id = 0;

        

        private int PieceID;
        float PriceMove;
        float Amount;
        float TotalRow;
        float GemDisVal;
        float ComitionVal = 0;
        float NetRow;
        private InvoiceType currentInvoiceType;
        private KeyboardLanguageManager langManager;
        public enum InvoiceType
        {
            Sale = 1,            // بيع
            SaleReturn = 2,      // بيع مرتد
            Purchase = 3,        // شراء
            PurchaseReturn = 4,  // شراء مرتد
            Inventory = 5,       // إذن جرد
            DeductStock = 6,     // إذن خصم رصيد
            AddStock = 7         // إذن إضافة رصيد
        }


        #endregion 
        #region ******** المنشء وتحميل الشاشة  *************
        public frm_NewInvoice(int type_ID)
        {
            InitializeComponent();
            currentInvoiceType = (InvoiceType)type_ID;

            langManager = new KeyboardLanguageManager(this);
            ConnectEventsFoter();
            US = CurrentSession.UserID;
        }

        private void ConnectEventsFoter()
        {
            langManager = new KeyboardLanguageManager(this);

            // ربط الأحداث
            txtDiscount.Leave += txtDiscount_Leave;
            txtTaxVal.Leave += txtTaxVal_Leave;
            txtValueAdded.Leave += txtValueAdded_Leave;

            txtPayment_Cash.Leave += txtPayment_Cash_Leave;
            txtPayment_Electronic.Leave += txtPayment_Electronic_Leave;

            DGV.CellEndEdit += DGV_CellEndEdit;
            DGV.EditingControlShowing += DGV_EditingControlShowing;
            DGV.KeyDown += DGV_KeyDown;
            DGV.CellFormatting += DGV_CellFormatting;

            // اللغة
            txtNoteInvoice.Enter += (s, e) => langManager.SetArabicLanguage();
            txtPayment_Note.Enter += (s, e) => langManager.SetArabicLanguage();
        }  
       
        private void frm_NewInvoice_Load(object sender, EventArgs e)
        {
            DBServiecs.A_UpdateAllDataBase();     // تحديث أرصدة الأصناف والحسابات
            LoadAcc();                          // تحميل بيانات الحسابات حسب نوع الفاتورة
            SetDefaultAccount();                // وضع الحساب الافتراضي حسب نوع الفاتورة
            InitializeAutoComplete();           // التعبئة التلقائية
            GetSalseMan();                      // إحضار البائعين أو منفذي الشراء
            InvTypeData();                      // ✅ تم التعديل
            DGVStyl();                          // تنسيق الجدول
            RegisterEvents();                   // ربط الأحداث
        }

      
       //تحديد نوع الفاتورة
        private void InvTypeData()
        {
            switch (currentInvoiceType)
            {
                case InvoiceType.Sale:
                    lblTypeInv.Text = "فاتورة بيع رقم :";
                    lblDir.Text = "البائع :";
                    SetInvoiceColors(Color.LightGreen);
                    chkAllowNegative.Visible = true;
                    lblPriceMove.Visible = true;
                    lblProductName.Text = "Product Name :";
                    lblCodeTitel.Text = "كود صنف";
                    lblGemDisVal.Visible = true;
                    break;

                case InvoiceType.SaleReturn:
                    lblTypeInv.Text = "فاتورة بيع مرتد رقم :";
                    lblDir.Text = "البائع :";
                    SetInvoiceColors(Color.MistyRose);
                    chkAllowNegative.Visible = false;
                    lblPriceMove.Visible = false;
                    lblProductName.Text = "";
                    lblCodeTitel.Text = "فاتورة بيع رقم";
                    lblGemDisVal.Visible = false;
                    break;

                case InvoiceType.Purchase:
                    lblTypeInv.Text = "فاتورة شراء رقم :";
                    lblDir.Text = "منفذ الشراء:";
                    SetInvoiceColors(Color.LightSkyBlue);
                    chkAllowNegative.Visible = false;
                    lblPriceMove.Visible = true;
                    lblProductName.Text = "Product Name :";
                    lblCodeTitel.Text = "كود صنف";
                    lblGemDisVal.Visible = false;
                    break;

                case InvoiceType.PurchaseReturn:
                    lblTypeInv.Text = "فاتورة شراء مرتد رقم :";
                    lblDir.Text = "منفذ الشراء:";
                    SetInvoiceColors(Color.LemonChiffon);
                    chkAllowNegative.Visible = false;
                    lblPriceMove.Visible = false;
                    lblProductName.Text = "";
                    lblCodeTitel.Text = "فاتورة شراء رقم";
                    lblGemDisVal.Visible = false;
                    break;

                case InvoiceType.Inventory:
                case InvoiceType.DeductStock:
                case InvoiceType.AddStock:
                    lblTypeInv.Text = "اذن تسوية مخزن رقم:";
                    lblDir.Text = "منفذ التسوية:";
                    SetInvoiceColors(Color.LightGray);
                    chkAllowNegative.Visible = false;
                    lblPriceMove.Visible = false;
                    lblProductName.Text = "";
                    lblCodeTitel.Text = "كود الصنف";
                    lblGemDisVal.Visible = false;
                    break;

                default:
                    lblTypeInv.Text = "نوع غير معروف رقم :";
                    SetInvoiceColors(SystemColors.Window);
                    break;
            }

            lblTypeInvID.Text = ((int)currentInvoiceType).ToString();
            GetInvoices();
            DGVStyl();
        }

        //تحميل الحسابات حسب نوع الفاتورة
        private void LoadAcc()
        {
            string accountIDs = "";

            switch (currentInvoiceType)
            {
                case InvoiceType.Sale:
                case InvoiceType.SaleReturn:
                    accountIDs = "7,22,39";  // بيع أو مردود مبيعات
                    break;

                case InvoiceType.Purchase:
                case InvoiceType.PurchaseReturn:
                    accountIDs = "14,39";    // شراء أو مردود مشتريات
                    break;

                case InvoiceType.Inventory:
                case InvoiceType.DeductStock:
                case InvoiceType.AddStock:
                    accountIDs = "31";       // جرد أو تسويات
                    break;
            }

            if (string.IsNullOrEmpty(accountIDs))
                return;

            DataTable result = DBServiecs.NewInvoice_GetAcc(accountIDs);

            DataRow[] filteredRows = result.Select("AccID > 200 OR AccID IN (40, 41, 50, 51, 52)");
            tblAcc = filteredRows.Length > 0 ? filteredRows.CopyToDataTable() : result.Clone();
        }

        // تعيين الحسابات الافتراضية حسب نوع الفاتورة
        private void SetDefaultAccount()
        {
            string? defaultAccID = null;

            switch (currentInvoiceType)
            {
                case InvoiceType.Inventory:
                    defaultAccID = "50";
                    break;

                case InvoiceType.DeductStock:
                    defaultAccID = "51";
                    break;

                case InvoiceType.AddStock:
                    defaultAccID = "52";
                    break;

                case InvoiceType.Sale:
                case InvoiceType.SaleReturn:
                    defaultAccID = "40";
                    break;

                case InvoiceType.Purchase:
                case InvoiceType.PurchaseReturn:
                    defaultAccID = "41";
                    break;
            }

            if (!string.IsNullOrEmpty(defaultAccID))
            {
                lblAccID.Text = defaultAccID;

                if (tblAcc != null)
                {
                    DataRow[] rows = tblAcc.Select($"AccID = {defaultAccID}");
                    if (rows.Length > 0)
                        LoadAccountData(rows[0]);
                }
            }
        }


        #endregion

        #region العمليات العامة للحفظ

        // الحفظ المؤقت للمتغيرات على الفاتورة
        public void SaveDraftInvoice(string? savedText = null)
        {
            if (!string.IsNullOrWhiteSpace(lblSave.Text))
            {
                MessageBox.Show("الفاتورة محفوظة نهائيًا، لا يمكن التعديل.");
                return;
            }

            // تحقق من الحقول الأساسية
            List<string> missingFields = new List<string>();

            if (string.IsNullOrWhiteSpace(lblInv_ID.Text))
                missingFields.Add("رقم الفاتورة");

            if (string.IsNullOrWhiteSpace(lblInv_Counter.Text))
                missingFields.Add("الرقم التسلسلي للفاتورة");

            if (cbxSellerID.SelectedValue == null)
                missingFields.Add(currentInvoiceType == InvoiceType.Sale || currentInvoiceType == InvoiceType.SaleReturn
                                  ? "البائع"
                                  : "منفذ الشراء / التسوية");

            if (string.IsNullOrWhiteSpace(lblAccID.Text))
                missingFields.Add("الحساب");

            if (CurrentSession.WarehouseId <= 0)
                missingFields.Add("المخزن");


            if (missingFields.Count > 0)
            {
                string message = "يرجى استكمال البيانات التالية قبل الحفظ:\n• " + string.Join("\n• ", missingFields);
                CustomMessageBox.ShowWarning(message, "بيانات ناقصة");
                return;
            }

            // الحفظ إذا كانت البيانات مكتملة
            DBServiecs.NewInvoice_InsertOrUpdate(
                invID: Convert.ToInt32(lblInv_ID.Text),
                invCounter: lblInv_Counter.Text,
                invType_ID: (int)currentInvoiceType,
                invDate: dtpInv_Date.Value,
                seller_ID: Convert.ToInt32(cbxSellerID.SelectedValue),
                user_ID: US,
                acc_ID: int.TryParse(lblAccID.Text, out int accId) ? accId : (int?)null,
                totalValue: TryParseFloat(lblTotalInv.Text),
                taxVal: TryParseFloat(txtTaxVal.Text),
                totalValueAfterTax: TryParseFloat(lblTotalValueAfterTax.Text),
                discount: TryParseFloat(txtDiscount.Text),
                valueAdded: TryParseFloat(txtValueAdded.Text),
                netTotal: TryParseFloat(lblNetTotal.Text),
                payment_Cash: TryParseFloat(txtPayment_Cash.Text),
                payment_Electronic: TryParseFloat(txtPayment_Electronic.Text),
                payment_BankCheck: 0,
                payment_Note: txtPayment_Note.Text,
                remainingOnAcc: TryParseFloat(lblRemainingOnAcc.Text),
                isReturnable: false,
                noteInvoice: txtNoteInvoice.Text,
                saved: savedText ?? string.Empty,
                Warehouse_Id: CurrentSession.WarehouseId,
                out _ // تجاهل رسالة الإخراج
            );
        }



        private void SaveDetailsChanges(DataGridViewRow row)
        {
            if (!int.TryParse(row.Cells["serInvDetail"]?.Value?.ToString(), out int id)) return;

            float.TryParse(row.Cells["PriceMove"]?.Value?.ToString(), out float price);
            float.TryParse(row.Cells["Amount"]?.Value?.ToString(), out float amount);
            float.TryParse(row.Cells["GemDisVal"]?.Value?.ToString(), out float disc);

            DBServiecs.NewInvoice_UpdateInvoiceDetail(id, price, amount, disc);

            row.Cells["TotalRow"].Value = price * amount;
            row.Cells["NetRow"].Value = (price * amount) - disc;
        }

   
    
        #endregion

        */

        /*
        #region التنقل بين الفواتير

        DataTable tblInv = new DataTable(); int currentInvoiceIndex = 0;
  
        // التنقل الى اول فاتورة
        private void btnFrist_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded())
                NavigateToInvoice(0);
        }
        
        //التنقل الى الفاتورة التالية
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded() && currentInvoiceIndex < tblInv.Rows.Count - 1)
                NavigateToInvoice(currentInvoiceIndex + 1);
            else
                MessageBox.Show("تم الوصول إلى آخر فاتورة.");
        }

        //التنقل الى الفاتورة السابقة
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded() && currentInvoiceIndex > 0)
                NavigateToInvoice(currentInvoiceIndex - 1);
            else
                MessageBox.Show("تم الوصول إلى أول فاتورة.");
        }

        //الذهاب الى اخر فاتورة
        private void btnLast_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded())
                NavigateToInvoice(tblInv.Rows.Count - 1);
        }

        //وظيفة التنقل بين الفواتير
        private void NavigateToInvoice(int targetIndex)
        {
            if (!EnsureInvoicesLoaded()) return;

            targetIndex = Math.Max(0, Math.Min(targetIndex, tblInv.Rows.Count - 1));
            currentInvoiceIndex = targetIndex;

            DisplayCurentRow(currentInvoiceIndex);
            ToggleControlsBasedOnSaveStatus();
            ToggleNavigationButtons();

            lblInfoInvoice.Text = $"فاتورة {targetIndex + 1} من {tblInv.Rows.Count}";
        }


        private void ToggleNavigationButtons()
        {
            btnFrist.Enabled = currentInvoiceIndex > 0;
            btnPrevious.Enabled = currentInvoiceIndex > 0;
            btnNext.Enabled = currentInvoiceIndex < tblInv.Rows.Count - 1;
            btnLast.Enabled = currentInvoiceIndex < tblInv.Rows.Count - 1;
        }

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

        public void DisplayCurentRow(int CIndex)
        {
            if (tblInv == null || tblInv.Rows.Count <= CIndex)
                return;

            DataRow row = tblInv.Rows[CIndex];

            // تحميل قيم أساسية
            lblInv_ID.Text = row["Inv_ID"].ToString();
            lblInv_Counter.Text = row["Inv_Counter"].ToString();
            lblTypeInv.Text = row["MovType"].ToString(); // نوع الحركة

            Inv_ID = Convert.ToInt32(lblInv_ID.Text);
            lblWarehouseName.Text = "الفرع الرئيسى"; // مؤقتًا

            // التاريخ
            if (row["Inv_Date"] != DBNull.Value)
                dtpInv_Date.Value = Convert.ToDateTime(row["Inv_Date"]);

            // البائع أو منفذ العملية
            if (row["Seller_ID"] != DBNull.Value)
                cbxSellerID.SelectedValue = Convert.ToInt32(row["Seller_ID"]);
            else
                cbxSellerID.SelectedIndex = -1;

            // المستخدم والحساب
            lblUserID.Text = row["User_ID"].ToString();
            lblAccID.Text = row["Acc_ID"].ToString();

            // القيم المالية
            lblTotalInv.Text = FormatNumber(row["TotalValue"]);
            txtTaxVal.Text = FormatNumber(row["TaxVal"]);
            lblTotalValueAfterTax.Text = FormatNumber(row["TotalValueAfterTax"]);
            txtDiscount.Text = FormatNumber(row["Discount"]);
            txtValueAdded.Text = FormatNumber(row["ValueAdded"]);
            lblNetTotal.Text = FormatNumber(row["NetTotal"]);

            // المدفوعات
            txtPayment_Cash.Text = FormatNumber(row["Payment_Cash"]);
            txtPayment_Electronic.Text = FormatNumber(row["Payment_Electronic"]);
            txtPayment_Note.Text = row["Payment_Note"]?.ToString();

            // الباقي على الحساب
            lblRemainingOnAcc.Text = FormatNumber(row["RemainingOnAcc"]);

            // الملاحظات وحالة الحفظ
            txtNoteInvoice.Text = row["NoteInvoice"]?.ToString();
            lblSave.Text = row["Saved"]?.ToString();
            // تحميل تفاصيل الفاتورة
            GetInvoiceDetails();


        }

        // فتح فاتورة جديدة
        private void btnNew_Click(object sender, EventArgs e)
        {
            SetDefaultAccount();

            if (tblInv == null)
                GetInvoices();

            string nextCounter = DBServiecs.NewInvoice_GetNewCounter((int)currentInvoiceType); // ✅ تحويل enum إلى int
            int nextID = DBServiecs.NewInvoice_GetNewID();

            lblInv_Counter.Text = nextCounter;
            lblInv_ID.Text = nextID.ToString();

            DisplayNewRow((int)currentInvoiceType, US); // ✅ تعديل النوع
            ToggleControlsBasedOnSaveStatus();
        }

        public void DisplayNewRow(int invType, int Us_id)
        {
            dtpInv_Date.Value = DateTime.Now;
            cbxSellerID.SelectedValue = 26;
            lblUserID.Text = Us_id.ToString();

            lblAccID.Text = invType switch
            {
                1 or 2 => "40",
                3 or 4 => "41",
                _ => "0"
            };

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
            txtPayment_Note.Text = "";
            txtNoteInvoice.Text = "";
            lblSave.Text = "";
            txtSeaarchProd.Text = "0";
            txtAmount.Text = "0";
            lblPriceMove.Text = "0";
            lblCount.Text = "0";
            lblInfoInvoice.Text = "فاتورة جديدة";

            lblProductName.Text = invType is 1 or 3 ? "Product Name :" : "Invoice No :";
            lblCodeTitel.Text = invType is 1 or 3 ? "كود صنف" : "فاتورة بيع رقم";

            DGV.DataSource = null;
            cbxPiece_ID.DataSource = null;
        }

        #endregion
        */

        /*
        #region  احداث ووظائف رأس الفاتورة

        //احداث اسم الحساب
        private void txtAccName_KeyDown(object sender, KeyEventArgs e)
        {
            // فتح شاشة البحث عند الضغط على Ctrl + F
            if (e.Control && e.KeyCode == Keys.F)
            {
                if (currentInvoiceType != InvoiceType.Sale &&
                    currentInvoiceType != InvoiceType.SaleReturn &&
                    currentInvoiceType != InvoiceType.Purchase &&
                    currentInvoiceType != InvoiceType.PurchaseReturn)
                    return; // نوع غير متوقع أو لا يدعم البحث

                // فتح شاشة البحث
                frmSearch searchForm = new frmSearch((int)currentInvoiceType, SearchEntityType.Product);

                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    lblAccID.Text = searchForm.SelectedID;

                    DataTable result = DBServiecs.MainAcc_GetAccounts(Convert.ToInt32(lblAccID.Text));
                    if (result != null && result.Rows.Count > 0)
                    {
                        DataRow row = result.Rows[0];
                        txtAccName.Text = row["FirstPhon"].ToString();
                        LoadAccountData(row); // تحميل كامل البيانات إن أردت
                    }
                }

                e.SuppressKeyPress = true;
                return;
            }

            // عند الضغط على Enter
            if (e.KeyCode == Keys.Enter)
            {
                string input = txtAccName.Text.Trim();

                if (string.IsNullOrWhiteSpace(input) || tblAcc == null)
                {
                    SetDefaultAccount();
                    return;
                }

                string filter = $"AccName = '{input.Replace("'", "''")}' OR FirstPhon = '{input.Replace("'", "''")}' OR AntherPhon = '{input.Replace("'", "''")}'";
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
                        "حساب جديد");

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

        //فتح نموذج اضافة حساب جديد
        private void OpenNewAccountForm()
        {
            string enteredName = txtAccName.Text.Trim();
            frm_AddAccount frmNew = new frm_AddAccount(enteredName, (int)currentInvoiceType); // ✅ تحويل enum إلى int

            if (frmNew.ShowDialog() == DialogResult.OK)
            {
                LoadAcc(); // تحديث القائمة
                InitializeAutoComplete(); // تحديث الإكمال التلقائي

                txtAccName.Text = frmNew.CreatedAccountName;
                lblAccID.Text = frmNew.CreatedAccountID.ToString();

                txtAccName.Focus();
                txtAccName.SelectAll();
            }
        }

        #endregion
        */

        /*
        #region  احداث ووظائف اضافة صنف

        //اضافة صنف الى الفاتورة
        public string InvoiceDetails_Insert()
        {
            GetVar();

            string message = DBServiecs.InvoiceDetails_Insert(
                (int)currentInvoiceType, Inv_ID, PieceID, PriceMove, Amount,
                TotalRow, GemDisVal, ComitionVal, NetRow, 0);

            DGVStyl();
            return message;
        }

        //تحميل بيانات قطعة من صنف
        private void LoadPieceData()
        {
            cbxPiece_ID.Visible = (currentInvoiceType == InvoiceType.Sale && isCanCut); // ✅ تعديل Type_ID

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

        //تحميل بيانات صنف
        private bool GetProd(string code)
        {
            txtAmount.Text = "0";

            string msg;
            tblProd = DBServiecs.Item_GetProductByCode(code, out msg);

            if (tblProd == null || tblProd.Rows.Count == 0)
            {
                MessageBox.Show(msg, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EmptyProdData();
                return false;
            }

            DataRow row = tblProd.Rows[0];

            // السعر حسب نوع الفاتورة
            lblPriceMove.Text = (currentInvoiceType == InvoiceType.Sale || currentInvoiceType == InvoiceType.SaleReturn)
                ? row["U_Price"].ToString()
                : row["B_Price"].ToString();

            // البيانات العامة
            ID_Prod = Convert.ToInt32(row["ID_Product"]);
            lblProductName.Text = row["ProdName"].ToString();
            unit_ID = Convert.ToInt32(row["UnitID"]);
            unit = (row["UnitProd"]?.ToString() ?? "").Trim();
            lblProductStock.Text = row["ProductStock"].ToString();
            lblMinLinth.Text = unit_ID == 1 ? row["MinLenth"].ToString() : "";
            lblLinthText.Text = unit_ID == 1 ? "اقل طول" : unit;
            isCanCut = (unit_ID == 1);
            cbxPiece_ID.Visible = (currentInvoiceType == InvoiceType.Sale && isCanCut);

            return true;
        }
        // الأحداث المرتبطة بإدخال الكمية
        private void txtAmount_KeyDown(object sender, KeyEventArgs e)
        {
            // تجاهل إذا لم يكن Enter أو إذا كانت الفاتورة محفوظة
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

            float pieceLength = 0;
            float.TryParse(cbxPiece_ID.Text, out pieceLength);
            bool isSales = currentInvoiceType == InvoiceType.Sale;

            if (isSales && unit_ID == 1 && pieceLength == 0)
            {
                CustomMessageBox.ShowWarning("يرجى اختيار طول القطعة", "خطأ");
                cbxPiece_ID.Focus();
                return;
            }

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

            DBServiecs.A_UpdateAllDataBase();
            PrepareSaleProduct(txtSeaarchProd.Text);
            GetInvoices();
            NavigateToInvoice(currentIndexBeforeInsert);
            CalculateInvoiceFooter();
        }

        // الأحداث المرتبطة بإدخال كود المنتج أو رقم الفاتورة المرتدة
        private void txtSeaarchProd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                frmSearch searchForm = new frmSearch((int)currentInvoiceType, SearchEntityType.Product);
                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    txtSeaarchProd.Text = searchForm.SelectedID;
                    txtSeaarchProd.SelectionStart = txtSeaarchProd.Text.Length;
                    SendKeys.Send("{ENTER}");
                }
                e.SuppressKeyPress = true;
                return;
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
                        CustomMessageBox.ShowWarning("نوع الفاتورة غير مدعوم في هذه العملية", "خطأ");
                        break;
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        // إعداد بيانات منتج لعملية شراء
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

        // إدخال منتج في فاتورة شراء
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

        // إدخال منتج في فاتورة بيع
        private void InsertSaleRow(float amount, float pieceLength)
        {
            if (unit_ID == 1)
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
                    DialogResult result = CustomMessageBox.ShowQuestion($"لا يجوز أن تكون القطعة المتبقية أقل من الحد الأدنى: {minLength}\nهل تريد المتابعة بالرغم من ذلك؟", "تنبيه");
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
            else
            {
                if (float.TryParse(lblProductStock.Text, out float stock))
                {
                    if (amount > stock && !chkAllowNegative.Checked)
                    {
                        CustomMessageBox.ShowWarning("الكمية المطلوبة أكبر من الرصيد ولا يسمح بالبيع على المكشوف.", "تنبيه");
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

        // إعداد منتج للعرض عند البحث في فاتورة بيع
        private void PrepareSaleProduct(string code)
        {
            if (!GetProd(code)) return;
            LoadPieceData();
        }

        // إدخال منتج في فاتورة تسوية (جرد أو إضافة)
        private void InsertInventoryRow(float amount)
        {
            InsertRow(unit_ID == 1);
            AfterInsertActions();
        }


        private void OpenReturnedInvoiceForm(string serial)
        {
            if (!int.TryParse(serial, out int serInv))
            {
                CustomMessageBox.ShowWarning("الرجاء إدخال رقم فاتورة صالح.", "تنبيه");
                return;
            }

            string? msg;
            DataTable tblInvoice = DBServiecs.NewInvoice_GetInvoiceByTypeAndCounter(1, serInv, out msg);


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

            using (frm_ReturnedInvoice returnedForm = new frm_ReturnedInvoice(1, serInv, tblInvoice, tblDetails, CurrentInvoiceID))
            {
                if (returnedForm.ShowDialog() == DialogResult.OK)
                {
                    LoadReturnedItems(returnedForm.SelectedItems);
                }
            }

            DGVStyl();
        }

        private void LoadReturnedItems(DataTable returnedItems)
        {
            foreach (DataRow row in returnedItems.Rows)
            {
                // مثال: إضافة البيانات إلى جدول الفاتورة DGV
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
        */

        /*
        #region  احداث ووظائف تذييل الفاتورة

        //حسابات تذييل الفاتورة
        private void CalculateRemainingOnAccount()
        {
            // تحويل النصوص إلى أرقام بطريقة آمنة
            decimal.TryParse(lblNetTotal.Text, out decimal netTotal);
            decimal.TryParse(txtPayment_Cash.Text, out decimal cash);
            decimal.TryParse(txtPayment_Electronic.Text, out decimal electronic);

            // جمع المدفوعات وحساب المتبقي
            decimal paid = cash + electronic;
            decimal remaining = netTotal - paid;

            // عرض الرصيد المتبقي بتنسيق 2 رقم عشري
            lblRemainingOnAcc.Text = remaining.ToString("N2");

            // تحديد الحالة اللونية والنصية
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
      
        private void CalculateInvoiceFooter()
        {
            if (DGV.DataSource is not DataTable dt) return; // حماية من null أو نوع غير مناسب

            decimal total = 0;

            foreach (DataRow row in dt.Rows)
                if (row["NetRow"] != DBNull.Value)
                    total += Convert.ToDecimal(row["NetRow"]);

            lblTotalInv.Text = total.ToString("N2");

            decimal.TryParse(txtTaxVal.Text, out var tax);
            decimal.TryParse(txtDiscount.Text, out var discount);
            decimal.TryParse(txtValueAdded.Text, out var added);

            var afterTax = total + tax;
            lblTotalValueAfterTax.Text = afterTax.ToString("N2");

            var net = total + tax - discount + added;
            lblNetTotal.Text = net.ToString("N2");

            decimal.TryParse(txtPayment_Cash.Text, out var cash);
            decimal.TryParse(txtPayment_Electronic.Text, out var visa);

            var remaining = net - (cash + visa);
            lblRemainingOnAcc.Text = remaining.ToString("N2");

            CalculateRemainingOnAccount();
        }

        #endregion

        #region  تنسيق واحداث  DataGridView
        private void DGV_SelectionChanged(object? sender, EventArgs e)
        {
            // التحقق من أن الـ DataGridView تحتوي على صفوف فعلية
            if (DGV.Rows.Count == 0 || DGV.CurrentRow == null || DGV.CurrentRow.IsNewRow)
            {
                lblMinLinth.Text = "";
                lblProductStock.Text = "";
                return;
            }

            // إذا وصلنا هنا، إذًا الصف يحتوي على بيانات فعلية
            if (DGV.Columns.Contains("MinLenth"))
                lblMinLinth.Text = DGV.CurrentRow.Cells["MinLenth"].Value?.ToString();
            else
                lblMinLinth.Text = "";

            if (DGV.Columns.Contains("ProductStock"))
                lblProductStock.Text = DGV.CurrentRow.Cells["ProductStock"].Value?.ToString();
            else
                lblProductStock.Text = "";
        }

        private void DGV_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true; // منع الصوت أو السلوك الافتراضي

                var currentCell = DGV.CurrentCell;
                if (currentCell == null) return;

                string[] editableCols = { "PriceMove", "Amount", "GemDisVal" };
                int colIndex = Array.IndexOf(editableCols, currentCell.OwningColumn.Name);

                if (colIndex >= 0)
                {
                    // انتقل إلى العمود التالي داخل نفس الصف
                    int nextColIndex = (colIndex + 1) % editableCols.Length;

                    int rowIndex = currentCell.RowIndex;
                    if (nextColIndex == 0) // يعني أكمل دورة السعر ← الكمية ← الخصم
                    {
                        // انتقل إلى السطر التالي
                        if (rowIndex + 1 < DGV.Rows.Count)
                            rowIndex++;
                        else
                            return;
                    }

                    DGV.CurrentCell = DGV.Rows[rowIndex].Cells[editableCols[nextColIndex]];
                }
            }
        }

        private void DGV_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            string columnName = DGV.Columns[e.ColumnIndex].Name;

            // الأعمدة القابلة للتعديل
            string[] editableColumns = { "PriceMove", "Amount", "GemDisVal" };

            bool isEditable = editableColumns.Contains(columnName);
            bool isSaved = !string.IsNullOrWhiteSpace(lblSave.Text);

            // خلفيات الصفوف المتعاقبة
            Color evenBackColor = Color.WhiteSmoke;
            Color oddBackColor = Color.LemonChiffon;

            // التحقق من أن CellStyle غير null أولاً
            if (e.CellStyle != null)
            {
                // حدد خلفية حسب رقم الصف
                e.CellStyle.BackColor = (e.RowIndex % 2 == 0) ? evenBackColor : oddBackColor;

                if (isEditable && !isSaved)
                {
                    // الحقول المتاحة للتعديل قبل الحفظ
                    e.CellStyle.ForeColor = Color.DarkBlue;
                    e.CellStyle.Font = new Font("Times New Roman", 14, FontStyle.Bold);
                }
                else
                {
                    // الحقول غير القابلة للتعديل أو الفاتورة محفوظة
                    e.CellStyle.ForeColor = Color.FromArgb(100, 100, 100); // رمادي واضح قليلاً
                    e.CellStyle.Font = new Font("Times New Roman", 14, FontStyle.Regular);
                }
            }

        }

        private void DGV_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox tb)
            {
                // إزالة الاشتراك السابق لتجنب التكرار
                tb.Enter -= TextBox_Enter_SelectAll;

                // التحقق من اسم العمود الحالي
                string? columnName = DGV.CurrentCell?.OwningColumn?.Name;

                if (columnName == "PriceMove" || columnName == "Amount" || columnName == "GemDisVal")
                {
                    // إذا كانت الخلية ضمن الأعمدة الثلاثة المسموح تعديلها
                    tb.Enter += TextBox_Enter_SelectAll;//نفس الخطأ
                }
            }
        }

        //المظهر العام للجدول
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
      
        #region تنقل بين الحقول
        private void InputFields_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            Control[] fields = inputFieldsBeforeSearch.Concat(inputFieldsAfterSearch).ToArray();
            var index = Array.IndexOf(fields, sender as Control);
            if (index >= 0 && index < fields.Length - 1)
            {
                e.SuppressKeyPress = true;
                fields[index + 1].Focus();
                if (fields[index + 1] is TextBox tb) tb.SelectAll();
            }
        }
        #endregion
        */

        /*
        #region 
        private void TextBox_Enter_SelectAll(object? sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
            }
        }
        
        
        public void GetInvoiceDetails()
        {

            // التحقق من وجود رقم الفاتورة وأنه رقم صحيح
            if (string.IsNullOrWhiteSpace(lblInv_ID.Text) || !int.TryParse(lblInv_ID.Text, out Inv_ID))
            {
                MessageBox.Show("رقم الفاتورة غير صالح أو غير موجود.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // جلب بيانات تفاصيل الفاتورة
            tblInvDetails = DBServiecs.NewInvoice_GetInvoiceDetails(Inv_ID);
            lblCount.Text = tblInvDetails?.Rows.Count.ToString() ?? "0";

            if (tblInvDetails == null || tblInvDetails.Rows.Count == 0)
            {
                PrepareEmptyGridStructure(); // إنشاء الأعمدة يدويًا
                DGV.DataSource = null;
            }
            else
            {
                DGV.DataSource = tblInvDetails;
            }

            // تطبيق تنسيقات الجدول
            DGVStyl();
            CalculateRemainingOnAccount();
        }

        private void PrepareEmptyGridStructure()
        {
            DGV.Columns.Clear();
            DGV.AutoGenerateColumns = false;

            // أعمدة ظاهرة
            AddTextColumn("ProductCode", "الكود");
            AddTextColumn("ProdName", "اسم الصنف", 200);
            AddTextColumn("UnitProd", "الوحدة");
            AddTextColumn("PriceMove", "السعر", format: "N2", alignRight: true);
            AddTextColumn("Amount", "الكمية", format: "N2", alignRight: true);
            AddTextColumn("TotalRow", "الإجمالي", format: "N2", alignRight: true, readOnly: true);
            AddTextColumn("GemDisVal", "الخصم", format: "N2", alignRight: true);
            AddTextColumn("NetRow", "الصافي", format: "N2", alignRight: true, readOnly: true);

            // أعمدة مخفية مهمة للحفظ
            AddHiddenColumn("serInvDetail");
            AddHiddenColumn("Inv_ID_fk");
            AddHiddenColumn("PieceID_fk");
            AddHiddenColumn("AIn");
            AddHiddenColumn("AOut");
            AddHiddenColumn("ReturnedInInvoiceNo");
            AddHiddenColumn("B_Price");
            AddHiddenColumn("U_Price");
            AddHiddenColumn("MinLenth");
            AddHiddenColumn("MinStock");
            AddHiddenColumn("ProductStock");
        }

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

        // دالة مساعدة لإضافة عمود نصي
        private void AddTextColumn(string name, string header, int width = 100, string? format = null, bool alignRight = false, bool readOnly = false)
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

            col.DefaultCellStyle.Alignment = alignRight ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleCenter;
            col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DGV.Columns.Add(col);
        }

        private string FormatNumber(object val)
        {
            if (val == null || val == DBNull.Value)
                return "0";

            if (decimal.TryParse(val.ToString(), out decimal number))
                return number.ToString("N2"); // مثال: 1234.5 → 1,234.50

            return "0";
        }

        private void GetInvoices()
        {
            // 1. تحميل الفواتير الحالية حسب النوع بترتيب تصاعدي
            tblInv = DBServiecs.NewInvoice_GetInvoicesByType((int)currentInvoiceType);

            // 2. إنشاء صف فارغ يمثل الفاتورة الجديدة
            DataRow newRow = tblInv.NewRow();

            // رقم جديد للفاتورة + الرقم التسلسلي المولد
            newRow["Inv_ID"] = DBServiecs.NewInvoice_GetNewID();
            newRow["Inv_Counter"] = DBServiecs.NewInvoice_GetNewCounter((int)currentInvoiceType);
            newRow["MovType"] = lblTypeInv.Text; // أو اجلب من جدول MovmentTypes
            newRow["Inv_Date"] = DateTime.Now;
            newRow["Seller_ID"] = cbxSellerID.Items.Count > 0 ? cbxSellerID.SelectedValue : DBNull.Value;
            newRow["User_ID"] = US;
            newRow["Acc_ID"] = lblAccID.Text;

            // القيم المالية مبدئية
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

            // 3. إضافته في نهاية الجدول
            tblInv.Rows.Add(newRow);

            // 4. الانتقال إلى آخر صف (الفاتورة الجديدة)
            currentInvoiceIndex = tblInv.Rows.Count - 1;
            lblInfoInvoice.Text = "فاتورة جديدة";

            // 5. عرضها
            DisplayCurentRow(currentInvoiceIndex);
        }

 

        private void InvTypeData(int type_ID) // تخصيص البيانات حسب نوع الفاتورة
        {
            // تعيين إعدادات العرض حسب نوع الفاتورة
            switch (type_ID)
            {
                case 1:
                    lblTypeInv.Text = "فاتورة بيع رقم :";
                    lblDir.Text = "البائع :";
                    SetInvoiceColors(Color.LightGreen);
                    chkAllowNegative.Visible = true;
                    lblPriceMove.Visible = true;
                    lblProductName.Text = "Product Name :"; lblCodeTitel.Text = "كود صنف";
                    lblGemDisVal.Visible = true;
                    break;

                case 2:
                    lblTypeInv.Text = "فاتورة بيع مرتد رقم :";
                    lblDir.Text = "البائع :";
                    SetInvoiceColors(Color.MistyRose);
                    chkAllowNegative.Visible = false;
                    lblPriceMove.Visible = false;
                    lblProductName.Text = ""; lblCodeTitel.Text = "فاتورة بيع رقم";
                    lblGemDisVal.Visible = false;
                    break;

                case 3:
                    lblTypeInv.Text = "فاتورة شراء رقم :";
                    lblDir.Text = "منفذ الشراء:";
                    SetInvoiceColors(Color.LightSkyBlue);
                    chkAllowNegative.Visible = false;
                    lblPriceMove.Visible = true;
                    lblProductName.Text = "Product Name :"; lblCodeTitel.Text = "كود صنف";
                    lblGemDisVal.Visible = false;
                    break;

                case 4:
                    lblTypeInv.Text = "فاتورة شراء مرتد رقم :";
                    lblDir.Text = "منفذ الشراء:";
                    SetInvoiceColors(Color.LemonChiffon);
                    chkAllowNegative.Visible = false;
                    lblPriceMove.Visible = false;
                    lblProductName.Text = ""; lblCodeTitel.Text = "فاتورة شراء رقم";
                    lblGemDisVal.Visible = false;
                    break;

                default:
                    if (type_ID > 4)
                    {
                        lblTypeInv.Text = "اذن تسوية مخزن رقم:";
                        lblDir.Text = "منفذ التسوية:";
                        SetInvoiceColors(Color.LightGray);
                    }
                    else
                    {
                        lblTypeInv.Text = "نوع غير معروف رقم :";
                        SetInvoiceColors(SystemColors.Window);
                    }
                    break;
            }

            lblTypeInvID.Text = type_ID.ToString();
            GetInvoices();
            DGVStyl();
        }

        private void SetInvoiceColors(Color color) // مساعدة لتوحيد تغيير ألوان الترويسات
        {
            tlpHader.BackColor = color;
            tlpNotes.BackColor = color;
        }
   
  




        public static float? TryParseFloat(string text)
        {
            if (float.TryParse(text, out float value))
                return value;
            return null;
        }


    
        // <summary>عند الخروج من قيمة الخصم → تحويلها إلى نسبة</summary>
        private void txtDiscount_Leave(object? sender, EventArgs e)
        {
            if (decimal.TryParse(txtDiscount.Text, out decimal amount))
            {
                if (amount == 0)
                {
                    lblDiscountRate.Text = "0.00";
                    return;
                }

                if (decimal.TryParse(lblTotalValueAfterTax.Text, out decimal baseVal))
                {
                    if (baseVal > 0)
                    {
                        lblDiscountRate.Text = CalculateRateFromAmount(baseVal, amount).ToString("N2");
                        CalculateInvoiceFooter();
                    }
                    else
                    {
                        txtDiscount.Text = "0.00";
                        lblDiscountRate.Text = "0.00";
                        CustomMessageBox.ShowInformation("يجب إدخال قيمة للفاتورة بعد الضريبة أولًا قبل حساب الخصم.", "تنبيه");
                        txtDiscount.Focus();
                    }
                }
            }
        }

        // <summary>عند الخروج من قيمة الضريبة → تحويلها إلى نسبة</summary>
        private void txtTaxVal_Leave(object? sender, EventArgs e)
        {
            if (decimal.TryParse(txtTaxVal.Text, out decimal amount))
            {
                // إذا كانت القيمة صفر، اسمح بالخروج ولا تقم بأي حسابات
                if (amount == 0)
                {
                    lblTaxRate.Text = "0.00";
                    return;
                }

                // إذا كانت القيمة الأساسية صالحة
                if (decimal.TryParse(lblTotalInv.Text, out decimal baseVal))
                {
                    if (baseVal > 0)
                    {
                        lblTaxRate.Text = CalculateRateFromAmount(baseVal, amount).ToString("N2");
                        CalculateInvoiceFooter();
                    }
                    else
                    {
                        // لا تسمح بالحساب عند إدخال قيمة غير صفرية والقيمة الأساسية صفر
                        txtTaxVal.Text = "0.00";
                        lblTaxRate.Text = "0.00";
                        CustomMessageBox.ShowInformation("يجب إدخال قيمة للفاتورة أولًا قبل حساب نسبة الإضافة.", "تنبيه");
                        txtTaxVal.Focus(); // يرجع التركيز فقط إذا لم يكن المستخدم كتب 0
                    }
                }
            }
        }

        // <summary>عند الخروج من قيمة الإضافة → تحويلها إلى نسبة</summary>
        private void txtValueAdded_Leave(object? sender, EventArgs e)
        {
            if (decimal.TryParse(txtValueAdded.Text, out decimal amount))
            {
                if (amount == 0)
                {
                    lblAdditionalRate.Text = "0.00";
                    return;
                }

                if (decimal.TryParse(lblTotalValueAfterTax.Text, out decimal baseVal))
                {
                    if (baseVal > 0)
                    {
                        lblAdditionalRate.Text = CalculateRateFromAmount(baseVal, amount).ToString("N2");
                        CalculateInvoiceFooter();
                    }
                    else
                    {
                        txtValueAdded.Text = "0.00";
                        lblAdditionalRate.Text = "0.00";
                        CustomMessageBox.ShowInformation("يجب إدخال قيمة للفاتورة بعد الضريبة أولًا قبل حساب الإضافة.", "تنبيه");
                        txtValueAdded.Focus();
                    }
                }
            }
        }

        // تحويل قيمة إلى نسبة بناءً على أساس معين
        private decimal CalculateRateFromAmount(decimal baseAmount, decimal amount)
        {
            if (baseAmount == 0) return 0;
            return Math.Round((amount / baseAmount) * 100, 2);
        }

    
        private void txtPayment_Cash_Leave(object? sender, EventArgs e)
        {
            if (!IsValidNetTotal()) return;
            CalculateRemainingOnAccount();
        }

        private void txtPayment_Electronic_Leave(object? sender, EventArgs e)
        {
            if (!IsValidNetTotal()) return;
            CalculateRemainingOnAccount();
        }

        private void lblAccID_TextChanged(object sender, EventArgs e)
        {
            string accountID = lblAccID.Text.Trim();

            if (!string.IsNullOrEmpty(accountID) && tblAcc != null)
            {
                // البحث عن السجل بناءً على رقم الحساب الجديد
                DataRow[] accountData = tblAcc.Select($"AccID = '{accountID}'");

                if (accountData.Length > 0)
                {
                    // تحميل بيانات الحساب إذا تم العثور على السجل
                    LoadAccountData(accountData[0]);
                }
                else
                {
                    // إذا لم يتم العثور على السجل
                    CustomMessageBox.ShowWarning("لا يوجد حساب مرتبط برقم الحساب المحدد.", "خطأ");
                }
            }
        }

        #endregion

        */

        /*
        #region 
        private void cbxPiece_ID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxPiece_ID.SelectedValue != null)
                lblPieceID.Text = cbxPiece_ID.SelectedValue.ToString();
        }

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

        private void txtAccName_Enter(object sender, EventArgs e)
        {
            langManager.SetArabicLanguage(); // تغيير اللغة للعربية
            txtAccName.SelectAll();          // تحديد النص

        }

  
        public bool isCanCut = true;
        private int unit_ID;

        private bool TryGetValidAmount(out float amount)
        {
            return float.TryParse(txtAmount.Text, out amount) && amount > 0;
        }
        private bool IsInvoiceSaved()
        {
            if (!string.IsNullOrWhiteSpace(lblSave.Text))
            {
                MessageBox.Show("الفاتورة محفوظة نهائيًا، لا يمكن التعديل.");
                return true;
            }
            return false;
        }


        private void InsertRow(bool isPiece)
        {
            if (!TryGetValidAmount(out float amount))
            {
                CustomMessageBox.ShowWarning("يرجى إدخال كمية صحيحة للمنتج", "خطأ");
                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }

            if (!TryGetValidPrice(out float priceMove))
            {
                CustomMessageBox.ShowWarning("انتبه لعدم وجود سعر للصنف", "تحذير");
            }

            if (unit_ID == 1) // يمكن قصه
            {
                int newPieceID = DBServiecs.Product_CreateNewPiece(ID_Prod);
                lblPieceID.Text = newPieceID.ToString();
            }
            else // غير قابل للقص
            {
                DataTable piece = DBServiecs.Product_GetOrCreate_DefaultPiece(ID_Prod);
                if (piece.Rows.Count > 0)
                {
                    lblPieceID.Text = piece.Rows[0]["Piece_ID"].ToString();
                }
                else
                {
                    // احتياطي: رسالة تنبيه في حال حدث خطأ غير متوقع
                    MessageBox.Show("لم يتم العثور على قطعة للمنتج المحدد.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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

            InvoiceDetails_Insert();
            Piece_id = 0;
            GetInvoiceDetails();
        }

        private void AfterInsertActions()
        {
            txtSeaarchProd.Focus();
            txtSeaarchProd.SelectAll();
            txtAmount.Text = "0";
            lblGemDisVal.Text = "0";
        }
 
        private void GetVar()
        {
            // التحويل الآمن باستخدام float.TryParse بدلًا من Convert.ToInt32 (لأن السعر والقيم قد تحتوي على كسور عشرية)
            int.TryParse(lblInv_ID.Text, out Inv_ID);
            float.TryParse(lblPriceMove.Text, out PriceMove);
            float.TryParse(txtAmount.Text, out Amount);
            float.TryParse(lblGemDisVal.Text, out GemDisVal);
            int.TryParse(lblPieceID.Text, out PieceID);
            // حساب المجموع قبل الخصم
            TotalRow = Amount * PriceMove;

            // صافي السطر بعد الخصم
            NetRow = TotalRow - GemDisVal;
        }

 
        private bool TryGetValidPrice(out float price)
        {
            return float.TryParse(lblPriceMove.Text, out price) && price > 0;
        }

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

        private void txtNoteInvoice_Enter(object sender, EventArgs e)
        {
            langManager.SetArabicLanguage();
            txtNoteInvoice.SelectAll();
        }

        private void txtPayment_Note_Enter(object sender, EventArgs e)
        {
            langManager.SetArabicLanguage();
            txtPayment_Note.SelectAll();
        }

        private void txtSeaarchProd_Enter(object sender, EventArgs e)
        {
            if (IsInvoiceSaved()) return;
            txtSeaarchProd.SelectAll();
        }

        #endregion 

        */

        /*

        #region @@@@@@@@@@@ Load Actions @@@@@@@@@@@@@@@@@@@@@@@@@

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

        private void LoadAcc() // تحميل بيانات الحسابات حسب نوع الفاتورة
        {
            string accountIDs = "";

            if (Type_ID == 1 || Type_ID == 2)
                accountIDs = "7,22,39";  // بيع أو مردود مبيعات
            else if (Type_ID == 3 || Type_ID == 4)
                accountIDs = "14,39";    // شراء أو مردود مشتريات
            else if (Type_ID >= 5)
                accountIDs = "31,33";    // جرد أو تسويات

            if (string.IsNullOrEmpty(accountIDs))
                return;

            DataTable result = DBServiecs.NewInvoice_GetAcc(accountIDs);

            // تصفية الصفوف المطلوبة
            DataRow[] filteredRows = result.Select("AccID > 200 OR AccID IN (40, 41)");
            tblAcc = filteredRows.Length > 0 ? filteredRows.CopyToDataTable() : result.Clone();
        }
        private DataTable? tblAcc = null;  //جدول الحسابات

        private void SetDefaultAccount() // تحديد رقم الحساب الافتراضي بناءً على نوع الفاتورة
        {
            string? defaultAccID = null;

            if (Type_ID >= 5)
                defaultAccID = "000";
            else if (Type_ID == 1 || Type_ID == 2)
                defaultAccID = "40";
            else if (Type_ID == 3 || Type_ID == 4)
                defaultAccID = "41";

            if (!string.IsNullOrEmpty(defaultAccID))
            {
                lblAccID.Text = defaultAccID;

                if (tblAcc != null)
                {
                    DataRow[] rows = tblAcc.Select($"AccID = {defaultAccID}");
                    if (rows.Length > 0)
                        LoadAccountData(rows[0]);
                }
            }
        }

        private void LoadAccountData(DataRow accountData) // وضع بيانات الحساب فى اماكنها
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

        private void GetSalseMan() // تحميل بيانات البائعين أو منفذي الشراء
        {
            string accountIDs = "";

            if (Type_ID == 1 || Type_ID == 2)
                accountIDs = "47"; // البائعين
            else if (Type_ID == 3 || Type_ID == 4 || Type_ID >= 5)
                accountIDs = "26"; // منفذو الشراء أو التسوية

            if (string.IsNullOrEmpty(accountIDs))
                return;

            DataTable result = DBServiecs.NewInvoice_GetAcc(accountIDs);
            tblAccSals = result;

            cbxSellerID.DataSource = tblAccSals;
            cbxSellerID.DisplayMember = "AccName";
            cbxSellerID.ValueMember = "AccID";
        }

        private void InvTypeData(int type_ID) // تخصيص البيانات حسب نوع الفاتورة
        {
            // تعيين إعدادات العرض حسب نوع الفاتورة
            switch (type_ID)
            {
                case 1:
                    lblTypeInv.Text = "فاتورة بيع رقم :";
                    lblDir.Text = "البائع :";
                    SetInvoiceColors(Color.LightGreen);
                    chkAllowNegative.Visible = true;
                    lblPriceMove.Visible = true;
                    lblProductName.Text = "Product Name :"; lblCodeTitel.Text = "كود صنف";
                    lblGemDisVal.Visible = true;
                    break;

                case 2:
                    lblTypeInv.Text = "فاتورة بيع مرتد رقم :";
                    lblDir.Text = "البائع :";
                    SetInvoiceColors(Color.MistyRose);
                    chkAllowNegative.Visible = false;
                    lblPriceMove.Visible = false;
                    lblProductName.Text = ""; lblCodeTitel.Text = "فاتورة بيع رقم";
                    lblGemDisVal.Visible = false;
                    break;

                case 3:
                    lblTypeInv.Text = "فاتورة شراء رقم :";
                    lblDir.Text = "منفذ الشراء:";
                    SetInvoiceColors(Color.LightSkyBlue);
                    chkAllowNegative.Visible = false;
                    lblPriceMove.Visible = true;
                    lblProductName.Text = "Product Name :"; lblCodeTitel.Text = "كود صنف";
                    lblGemDisVal.Visible = false;
                    break;

                case 4:
                    lblTypeInv.Text = "فاتورة شراء مرتد رقم :";
                    lblDir.Text = "منفذ الشراء:";
                    SetInvoiceColors(Color.LemonChiffon);
                    chkAllowNegative.Visible = false;
                    lblPriceMove.Visible = false;
                    lblProductName.Text = ""; lblCodeTitel.Text = "فاتورة شراء رقم";
                    lblGemDisVal.Visible = false;
                    break;

                default:
                    if (type_ID > 4)
                    {
                        lblTypeInv.Text = "اذن تسوية مخزن رقم:";
                        lblDir.Text = "منفذ التسوية:";
                        SetInvoiceColors(Color.LightGray);
                    }
                    else
                    {
                        lblTypeInv.Text = "نوع غير معروف رقم :";
                        SetInvoiceColors(SystemColors.Window);
                    }
                    break;
            }

            lblTypeInvID.Text = type_ID.ToString();
            GetInvoices();
            DGVStyl();
        }

        private void SetInvoiceColors(Color color) // مساعدة لتوحيد تغيير ألوان الترويسات
        {
            tlpHader.BackColor = color;
            tlpNotes.BackColor = color;
        }

        DataTable tblInv = new DataTable();int currentInvoiceIndex=0;
        private void GetInvoices()
        {
            // 1. تحميل الفواتير الحالية حسب النوع بترتيب تصاعدي
            tblInv = DBServiecs.NewInvoice_GetInvoicesByType(Type_ID);

            // 2. إنشاء صف فارغ يمثل الفاتورة الجديدة
            DataRow newRow = tblInv.NewRow();

            // رقم جديد للفاتورة + الرقم التسلسلي المولد
            newRow["Inv_ID"] = DBServiecs.NewInvoice_GetNewID();
            newRow["Inv_Counter"] = DBServiecs.NewInvoice_GetNewCounter(Type_ID);
            newRow["MovType"] = lblTypeInv.Text; // أو اجلب من جدول MovmentTypes
            newRow["Inv_Date"] = DateTime.Now;
            newRow["Seller_ID"] = cbxSellerID.Items.Count > 0 ? cbxSellerID.SelectedValue : DBNull.Value;
            newRow["User_ID"] = US;
            newRow["Acc_ID"] = lblAccID.Text;

            // القيم المالية مبدئية
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

            // 3. إضافته في نهاية الجدول
            tblInv.Rows.Add(newRow);

            // 4. الانتقال إلى آخر صف (الفاتورة الجديدة)
            currentInvoiceIndex = tblInv.Rows.Count - 1;
            lblInfoInvoice.Text = "فاتورة جديدة";

            // 5. عرضها
            DisplayCurentRow(currentInvoiceIndex);
        }

        public void DisplayCurentRow(int CIndex)
        {
            if (tblInv == null || tblInv.Rows.Count <= CIndex)
                return;

            DataRow row = tblInv.Rows[CIndex];

            // تحميل قيم أساسية
            lblInv_ID.Text = row["Inv_ID"].ToString();
            lblInv_Counter.Text = row["Inv_Counter"].ToString();
            lblTypeInv.Text = row["MovType"].ToString(); // نوع الحركة

            Inv_ID = Convert.ToInt32(lblInv_ID.Text);
            lblWarehouseName.Text = "الفرع الرئيسى"; // مؤقتًا

            // التاريخ
            if (row["Inv_Date"] != DBNull.Value)
                dtpInv_Date.Value = Convert.ToDateTime(row["Inv_Date"]);

            // البائع أو منفذ العملية
            if (row["Seller_ID"] != DBNull.Value)
                cbxSellerID.SelectedValue = Convert.ToInt32(row["Seller_ID"]);
            else
                cbxSellerID.SelectedIndex = -1;

            // المستخدم والحساب
            lblUserID.Text = row["User_ID"].ToString();
            lblAccID.Text = row["Acc_ID"].ToString();

            // القيم المالية
            lblTotalInv.Text = FormatNumber(row["TotalValue"]);
            txtTaxVal.Text = FormatNumber(row["TaxVal"]);
            lblTotalValueAfterTax.Text = FormatNumber(row["TotalValueAfterTax"]);
            txtDiscount.Text = FormatNumber(row["Discount"]);
            txtValueAdded.Text = FormatNumber(row["ValueAdded"]);
            lblNetTotal.Text = FormatNumber(row["NetTotal"]);

            // المدفوعات
            txtPayment_Cash.Text = FormatNumber(row["Payment_Cash"]);
            txtPayment_Electronic.Text = FormatNumber(row["Payment_Electronic"]);
            txtPayment_Note.Text = row["Payment_Note"]?.ToString();

            // الباقي على الحساب
            lblRemainingOnAcc.Text = FormatNumber(row["RemainingOnAcc"]);

            // الملاحظات وحالة الحفظ
            txtNoteInvoice.Text = row["NoteInvoice"]?.ToString();
            lblSave.Text = row["Saved"]?.ToString();
            // تحميل تفاصيل الفاتورة
            GetInvoiceDetails();


        }

        public void SaveDraftInvoice(string? savedText = null)
        {
            if (!string.IsNullOrWhiteSpace(lblSave.Text))
            {
                MessageBox.Show("الفاتورة محفوظة نهائيًا، لا يمكن التعديل.");
                return;
            }

            DBServiecs.NewInvoice_InsertOrUpdate(
                invID: Convert.ToInt32(lblInv_ID.Text),
                invCounter: lblInv_Counter.Text,
                invType_ID: Type_ID,
                invDate: dtpInv_Date.Value,
                seller_ID: cbxSellerID.SelectedValue != null
                           ? Convert.ToInt32(cbxSellerID.SelectedValue)
                           : (int?)null,
                user_ID: US,
                acc_ID: int.TryParse(lblAccID.Text, out int accId) ? accId : (int?)null,
                totalValue: TryParseFloat(lblTotalInv.Text),
                taxVal: TryParseFloat(txtTaxVal.Text),
                totalValueAfterTax: TryParseFloat(lblTotalValueAfterTax.Text),
                discount: TryParseFloat(txtDiscount.Text),
                valueAdded: TryParseFloat(txtValueAdded.Text),
                netTotal: TryParseFloat(lblNetTotal.Text),
                payment_Cash: TryParseFloat(txtPayment_Cash.Text),
                payment_Electronic: TryParseFloat(txtPayment_Electronic.Text),
                payment_BankCheck: 0,
                payment_Note: txtPayment_Note.Text,
                remainingOnAcc: TryParseFloat(lblRemainingOnAcc.Text),
                isReturnable: false,
                noteInvoice: txtNoteInvoice.Text,
                saved: savedText ?? string.Empty,
                Warehouse_Id: CurrentSession.WarehouseId, // ✅ استخدام القيمة من CurrentSession
                out _ // تجاهل رسالة الإخراج
            );
        }


        private string FormatNumber(object val)
        {
            if (val == null || val == DBNull.Value)
                return "0";

            if (decimal.TryParse(val.ToString(), out decimal number))
                return number.ToString("N2"); // مثال: 1234.5 → 1,234.50

            return "0";
        }

        DataTable tblInvDetails = new DataTable();
        public void GetInvoiceDetails()
        {

            // التحقق من وجود رقم الفاتورة وأنه رقم صحيح
            if (string.IsNullOrWhiteSpace(lblInv_ID.Text) || !int.TryParse(lblInv_ID.Text, out Inv_ID))
            {
                MessageBox.Show("رقم الفاتورة غير صالح أو غير موجود.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // جلب بيانات تفاصيل الفاتورة
            tblInvDetails = DBServiecs.NewInvoice_GetInvoiceDetails(Inv_ID);
            lblCount.Text = tblInvDetails?.Rows.Count.ToString() ?? "0";

            if (tblInvDetails == null || tblInvDetails.Rows.Count == 0)
            {
                PrepareEmptyGridStructure(); // إنشاء الأعمدة يدويًا
                DGV.DataSource = null;
            }
            else
            {
                DGV.DataSource = tblInvDetails;
            }

            // تطبيق تنسيقات الجدول
            DGVStyl();
            CalculateRemainingOnAccount();
        }

        #endregion







        // الدفعة الثانية

        #region @@@@@@@@ Header Action @@@@@@@@@@@@@@@@@
        private void SetDefaultAccount_() // تحديد رقم الحساب الافتراضي بناءً على نوع الفاتورة
        {
            string? defaultAccID = null;

            if (Type_ID >= 5)
                defaultAccID = "000";
            else if (Type_ID == 1 || Type_ID == 2)
                defaultAccID = "40";
            else if (Type_ID == 3 || Type_ID == 4)
                defaultAccID = "41";

            if (!string.IsNullOrEmpty(defaultAccID))
            {
                lblAccID.Text = defaultAccID;

                if (tblAcc != null)
                {
                    DataRow[] rows = tblAcc.Select($"AccID = {defaultAccID}");
                    if (rows.Length > 0)
                        LoadAccountData(rows[0]);
                }
            }
        }

        private void txtAccName_Enter(object sender, EventArgs e)
        {
            langManager.SetArabicLanguage(); // تغيير اللغة للعربية
            txtAccName.SelectAll();          // تحديد النص

        }

        private void txtAccName_KeyDown(object sender, KeyEventArgs e)
        {
            // فتح شاشة البحث عند الضغط على Ctrl + F
            if (e.Control && e.KeyCode == Keys.F)
            {
                // تحديد كود البحث بناءً على نوع الفاتورة
                //string? searchCode = null;//مازالت نفس المشكلة

                //if (Type_ID == 1 || Type_ID == 2)
                //    searchCode = "7"; // عملاء
                //else if (Type_ID == 3 || Type_ID == 4)
                //    searchCode = "14"; // موردين
                //else
                //    return; // نوع غير متوقع

                // فتح شاشة البحث
                frmSearch searchForm = new frmSearch(Type_ID, SearchEntityType.Product);

                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    lblAccID.Text = searchForm.SelectedID;

                    DataTable result = DBServiecs.MainAcc_GetAccounts(Convert.ToInt32(lblAccID.Text));
                    if (result != null && result.Rows.Count > 0)
                    {
                        DataRow row = result.Rows[0];
                        txtAccName.Text = row["FirstPhon"].ToString();
                        LoadAccountData(row); // تحميل كامل البيانات إن أردت
                    }
                }

                e.SuppressKeyPress = true;
                return;
            }

            // عند الضغط على Enter
            if (e.KeyCode == Keys.Enter)
            {
                string input = txtAccName.Text.Trim();

                if (string.IsNullOrWhiteSpace(input) || tblAcc == null)
                {
                    SetDefaultAccount();
                    return;
                }

                string filter = $"AccName = '{input.Replace("'", "''")}' OR FirstPhon = '{input.Replace("'", "''")}' OR AntherPhon = '{input.Replace("'", "''")}'";
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
                        "حساب جديد");

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

        private void OpenNewAccountForm()
        {
            string enteredName = txtAccName.Text.Trim();
            frm_AddAccount frmNew = new frm_AddAccount(enteredName, Type_ID);

            if (frmNew.ShowDialog() == DialogResult.OK)
            {
                // بعد حفظ الحساب الجديد، نحدث القائمة ونرجع إلى الحساب الجديد
                LoadAcc(); // تحديث القائمة
                InitializeAutoComplete(); // تحديث الإكمال التلقائي

                txtAccName.Text = frmNew.CreatedAccountName;
                lblAccID.Text = frmNew.CreatedAccountID.ToString();

                txtAccName.Focus();
                txtAccName.SelectAll();
            }
        }





        #endregion





        //الدفعة الثالثة

        #region @@@@@@@@@  Item Row Action @@@@@@@@@@@@@@@@
        public bool isCanCut = true;
        private int unit_ID;


        private bool IsInvoiceSaved()
        {
            if (!string.IsNullOrWhiteSpace(lblSave.Text))
            {
                MessageBox.Show("الفاتورة محفوظة نهائيًا، لا يمكن التعديل.");
                return true;
            }
            return false;
        }

        private bool TryGetValidAmount(out float amount)
        {
            return float.TryParse(txtAmount.Text, out amount) && amount > 0;
        }

        private bool TryGetValidPrice(out float price)
        {
            return float.TryParse(lblPriceMove.Text, out price) && price > 0;
        }

        private void txtSeaarchProd_Enter(object sender, EventArgs e)
        {
            if (IsInvoiceSaved()) return;
            txtSeaarchProd.SelectAll();
        }

        private void txtSeaarchProd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)

            {
                frmSearch searchForm = new frmSearch(Type_ID, SearchEntityType.Product );
                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    txtSeaarchProd.Text = searchForm.SelectedID;
                    txtSeaarchProd.SelectionStart = txtSeaarchProd.Text.Length;
                    SendKeys.Send("{ENTER}");
                }
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(txtSeaarchProd.Text))
            {
                if (IsInvoiceSaved()) return;

                string code = txtSeaarchProd.Text.Trim();
                switch (Type_ID)
                {
                    case 1: HandleSale(code); break;
                    case 2:
                    case 4:
                    case 5: OpenReturnedInvoiceForm(code); break;
                    case 3: HandlePurchase(code); break;
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
        private void txtSeaarchProd_KeyDown_(object sender, KeyEventArgs e)
        {
            // فتح شاشة البحث عند الضغط على Ctrl + F
            if (e.Control && e.KeyCode == Keys.F)
            {
                frmSearch searchForm = new frmSearch(Type_ID, SearchEntityType.Product);

                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    txtSeaarchProd.Text = searchForm.SelectedID;
                    txtSeaarchProd.SelectionStart = txtSeaarchProd.Text.Length;
                    SendKeys.Send("{ENTER}");
                }
                e.SuppressKeyPress = true;
                return;
            }

            // عند الضغط على Enter مع وجود قيمة في مربع البحث
            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(txtSeaarchProd.Text))
            {
                if (IsInvoiceSaved()) return;

                string code = txtSeaarchProd.Text.Trim();

                // تنفيذ الإجراء المناسب حسب نوع الفاتورة
                switch (Type_ID)
                {
                    case 1: HandleSale(code); break;
                    case 2:
                    case 4:
                    case 5: OpenReturnedInvoiceForm(code); break;
                    case 3: HandlePurchase(code); break;
                    default:
                        CustomMessageBox.ShowWarning("نوع الفاتورة غير مدعوم", "خطأ");
                        break;
                }

                // الانتقال إلى الحقل التالي بعد إدخال الصنف
                txtTaxVal.Focus();
                txtTaxVal.SelectAll();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void cbxPiece_ID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxPiece_ID.SelectedValue != null)
                lblPieceID.Text = cbxPiece_ID.SelectedValue.ToString();
        }

        private void txtAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if (IsInvoiceSaved()) return;

            int currentIndexBeforeInsert = currentInvoiceIndex;

            SaveDraftInvoice();

            if (!TryGetValidAmount(out float amount))
            {
                CustomMessageBox.ShowWarning("يرجى إدخال كمية صحيحة للمنتج", "خطأ");
                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }

            float pieceLength = 0;
            float.TryParse(cbxPiece_ID.Text, out pieceLength);
            bool isSales = Type_ID == 1;

            if (isSales && unit_ID == 1 && pieceLength == 0)
            {
                CustomMessageBox.ShowWarning("يرجى اختيار طول القطعة", "خطأ");
                cbxPiece_ID.Focus();
                return;
            }

            switch (Type_ID)
            {
                case 1: HandleSales(amount, pieceLength); break;
                case 3: HandlePurchases(amount); break;
                case 5: HandlePurchaseOrInsert(amount); break;
                default:
                    CustomMessageBox.ShowWarning("نوع الفاتورة غير مدعوم", "خطأ");
                    return;
            }

            DBServiecs.UpdateAllBalances();
            HandleSale(txtSeaarchProd.Text);

            // إعادة تحميل الفواتير فقط لو كانت تغيرت
            GetInvoices();

            // عد إلى الفاتورة التي كنت بها
            NavigateToInvoice(currentIndexBeforeInsert);

            CalculateInvoiceFooter();
        }


        private void LoadPieceData()
        {
            cbxPiece_ID.Visible = (Type_ID == 1 && isCanCut);

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

        private void InsertRow(bool isPiece)
        {
            if (!TryGetValidAmount(out float amount))
            {
                CustomMessageBox.ShowWarning("يرجى إدخال كمية صحيحة للمنتج", "خطأ");
                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }

            if (!TryGetValidPrice(out float priceMove))
            {
                CustomMessageBox.ShowWarning("انتبه لعدم وجود سعر للصنف", "تحذير");
            }

            if (unit_ID == 1) // يمكن قصه
            {
                int newPieceID = DBServiecs.Product_CreateNewPiece(ID_Prod);
                lblPieceID.Text = newPieceID.ToString();
            }
            else // غير قابل للقص
            {
                DataTable piece = DBServiecs.Product_GetOrCreate_DefaultPiece(ID_Prod);
                if (piece.Rows.Count > 0)
                {
                    lblPieceID.Text = piece.Rows[0]["Piece_ID"].ToString();
                }
                else
                {
                    // احتياطي: رسالة تنبيه في حال حدث خطأ غير متوقع
                    MessageBox.Show("لم يتم العثور على قطعة للمنتج المحدد.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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

            InvoiceDetails_Insert();
            Piece_id = 0;
            GetInvoiceDetails();
        }


        public string InvoiceDetails_Insert()
        {
            GetVar();
            string message = DBServiecs.InvoiceDetails_Insert(
                Type_ID, Inv_ID, PieceID, PriceMove, Amount,
                TotalRow, GemDisVal, ComitionVal, NetRow, 0);
            DGVStyl();
            return message;
        }

        private void GetVar()
        {
            // التحويل الآمن باستخدام float.TryParse بدلًا من Convert.ToInt32 (لأن السعر والقيم قد تحتوي على كسور عشرية)
            int.TryParse(lblInv_ID.Text, out Inv_ID);
            float.TryParse(lblPriceMove.Text, out PriceMove);
            float.TryParse(txtAmount.Text, out Amount);
            float.TryParse(lblGemDisVal.Text, out GemDisVal);
            int.TryParse(lblPieceID.Text, out PieceID);
            // حساب المجموع قبل الخصم
            TotalRow = Amount * PriceMove;

            // صافي السطر بعد الخصم
            NetRow = TotalRow - GemDisVal;
        }

        private void OpenReturnedInvoiceForm(string serial)
        {
            if (!int.TryParse(serial, out int serInv))
            {
                CustomMessageBox.ShowWarning("الرجاء إدخال رقم فاتورة صالح.", "تنبيه");
                return;
            }

            string? msg;
            DataTable tblInvoice = DBServiecs.NewInvoice_GetInvoiceByTypeAndCounter(1, serInv, out msg);


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

            using (frm_ReturnedInvoice returnedForm = new frm_ReturnedInvoice(1, serInv, tblInvoice, tblDetails, CurrentInvoiceID))
            {
                if (returnedForm.ShowDialog() == DialogResult.OK)
                {
                    LoadReturnedItems(returnedForm.SelectedItems);
                }
            }

            DGVStyl();
        }

        private bool GetProd(string code)
        {
            txtAmount.Text = "0";

            string msg;
            tblProd = DBServiecs.Item_GetProductByCode(code, out msg);

            if (tblProd == null || tblProd.Rows.Count == 0)
            {
                MessageBox.Show(msg, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EmptyProdData();
                return false;
            }

            DataRow row = tblProd.Rows[0];

            // السعر حسب نوع الفاتورة
            lblPriceMove.Text = (Type_ID == 1 || Type_ID == 2)
                ? row["U_Price"].ToString()
                : row["B_Price"].ToString();

            // البيانات العامة
            ID_Prod = Convert.ToInt32(row["ID_Product"]);
            lblProductName.Text = row["ProdName"].ToString();
            unit_ID = Convert.ToInt32(row["UnitID"]);
            unit = (row["UnitProd"]?.ToString() ?? "").Trim();
            lblProductStock.Text = row["ProductStock"].ToString();
            lblMinLinth.Text = unit_ID == 1 ? row["MinLenth"].ToString() : "";
            lblLinthText.Text = unit_ID == 1 ? "اقل طول" : unit;
            isCanCut = (unit_ID == 1);
            cbxPiece_ID.Visible = (Type_ID == 1 && isCanCut);

            return true;

        }

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

        private void LoadReturnedItems(DataTable returnedItems)
        {
            foreach (DataRow row in returnedItems.Rows)
            {
                // مثال: إضافة البيانات إلى جدول الفاتورة DGV
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


        public static float? TryParseFloat(string text)
        {
            if (float.TryParse(text, out float value))
                return value;
            return null;
        }

        #endregion






        //الدفعة الرابعة

        #region @@@@@@ Foter Calc – حسابات تذييل الفاتورة @@@@@@@@@@@@@@

        /// <summary>
        /// ربط الأحداث الخاصة بحقول التذييل مع حسابات النسبة والتفقيط.
        /// </summary>
        private void ConnectEventsFoter()
        {
            langManager = new KeyboardLanguageManager(this);


            //// ربط القيم مع النسب (النسبة تُحسب تلقائيًا وتُعرض في Label غير قابل للتعديل)
            //ValueRateBinder.Bind(txtTaxVal, lblTaxRate, lblTotalInv );         // الضريبة
            //ValueRateBinder.Bind(txtDiscount, lblDiscountRate, lblTotalValueAfterTax);  // الخصم
            //ValueRateBinder.Bind(txtValueAdded, lblAdditionalRate, lblTotalValueAfterTax); // الإضافة

            //// ربط التفقيط لصافي الفاتورة
            //ValueRateBinder.BindTafqeet(lblNetTotal, lblTafqet, "جنيهاً", "قرشاً");

            //// حساب القيم/النسب عند الخروج من الحقول اليدوية
            //txtDiscount.Leave += txtDiscount_Leave;
            //txtTaxVal.Leave += txtTaxVal_Leave;
            //txtValueAdded.Leave += txtValueAdded_Leave;

            //// أحداث DGV التكميلية
            //DGV.CellEndEdit += DGV_CellEndEdit;
            //DGV.EditingControlShowing += DGV_EditingControlShowing;
            //DGV.KeyDown += DGV_KeyDown;
            //DGV.CellFormatting += DGV_CellFormatting;

            // الحقول التي يتم تفعيلها بعد اختيار صنف
        //    inputFieldsAfterSearch = new Control[]//Cannot implicitly convert type 'System.Windows.Forms.Control[]' to 'System.Collections.Generic.List<System.Windows.Forms.Control>'
        //    {
        //txtTaxVal,
        //txtDiscount,
        //txtValueAdded,
        //txtPayment_Cash,
        //txtPayment_Electronic,
        //txtPayment_Note,
        //txtNoteInvoice
        //    };

        //    // يمكنك تحديد الحقول التي تكون نشطة قبل البحث، إن لزم الأمر
        //    inputFieldsBeforeSearch = new Control[]
        //    {
        //        // مثال: dtpInv_Date, txtAccName, cbxSellerID, txtSeaarchProd
        //    };
        }

        /// <summary>
        /// تنفيذ جميع العمليات الحسابية الخاصة بتذييل الفاتورة (صافي، ضريبة، خصم، إلخ)
        /// </summary>
        private void CalculateInvoiceFooter()
        {
            decimal totalInv = 0;
            int rowCount = 0;

            // حساب إجمالي الفاتورة من DGV
            if (DGV.DataSource is DataTable dt)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["NetRow"] != DBNull.Value)
                        totalInv += Convert.ToDecimal(row["NetRow"]);

                    rowCount++;
                }
            }

            lblTotalInv.Text = totalInv.ToString("N2");
            lblCount.Text = rowCount.ToString();

            // قراءة القيم المدخلة يدويًا
            decimal.TryParse(txtTaxVal.Text, out decimal taxVal);
            decimal.TryParse(txtDiscount.Text, out decimal discountVal);
            decimal.TryParse(txtValueAdded.Text, out decimal addedVal);

            // حساب القيمة بعد الضريبة (اختياري)
            decimal totalAfterTax = totalInv + taxVal;
            lblTotalValueAfterTax.Text = totalAfterTax.ToString("N2");

            // حساب الصافي النهائي
            decimal netTotal = totalInv + taxVal - discountVal + addedVal;
            lblNetTotal.Text = netTotal.ToString("N2");

            // حساب المتبقي
            decimal.TryParse(txtPayment_Cash.Text, out decimal paymentCash);
            decimal.TryParse(txtPayment_Electronic.Text, out decimal paymentVisa);

            decimal remaining = netTotal - (paymentCash + paymentVisa);
            lblRemainingOnAcc.Text = remaining.ToString("N2");

            // تنسيق DGV
            DGVStyl();
            CalculateRemainingOnAccount();
        }

        /// تحويل قيمة إلى نسبة بناءً على أساس معين
        private decimal CalculateRateFromAmount(decimal baseAmount, decimal amount)
        {
            if (baseAmount == 0) return 0;
            return Math.Round((amount / baseAmount) * 100, 2);
        }


        /// <summary>عند الخروج من قيمة الخصم → تحويلها إلى نسبة</summary>
        private void txtDiscount_Leave(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtDiscount.Text, out decimal amount))
            {
                if (amount == 0)
                {
                    lblDiscountRate.Text = "0.00";
                    return;
                }

                if (decimal.TryParse(lblTotalValueAfterTax.Text, out decimal baseVal))
                {
                    if (baseVal > 0)
                    {
                        lblDiscountRate.Text = CalculateRateFromAmount(baseVal, amount).ToString("N2");
                        CalculateInvoiceFooter();
                    }
                    else
                    {
                        txtDiscount.Text = "0.00";
                        lblDiscountRate.Text = "0.00";
                        CustomMessageBox.ShowInformation("يجب إدخال قيمة للفاتورة بعد الضريبة أولًا قبل حساب الخصم.", "تنبيه");
                        txtDiscount.Focus();
                    }
                }
            }
        }

        /// <summary>عند الخروج من قيمة الضريبة → تحويلها إلى نسبة</summary>
        private void txtTaxVal_Leave(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtTaxVal.Text, out decimal amount))
            {
                // إذا كانت القيمة صفر، اسمح بالخروج ولا تقم بأي حسابات
                if (amount == 0)
                {
                    lblTaxRate.Text = "0.00";
                    return;
                }

                // إذا كانت القيمة الأساسية صالحة
                if (decimal.TryParse(lblTotalInv.Text, out decimal baseVal))
                {
                    if (baseVal > 0)
                    {
                        lblTaxRate.Text = CalculateRateFromAmount(baseVal, amount).ToString("N2");
                        CalculateInvoiceFooter();
                    }
                    else
                    {
                        // لا تسمح بالحساب عند إدخال قيمة غير صفرية والقيمة الأساسية صفر
                        txtTaxVal.Text = "0.00";
                        lblTaxRate.Text = "0.00";
                        CustomMessageBox.ShowInformation("يجب إدخال قيمة للفاتورة أولًا قبل حساب نسبة الإضافة.", "تنبيه");
                        txtTaxVal.Focus(); // يرجع التركيز فقط إذا لم يكن المستخدم كتب 0
                    }
                }
            }
        }


        /// <summary>عند الخروج من قيمة الإضافة → تحويلها إلى نسبة</summary>
        private void txtValueAdded_Leave(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtValueAdded.Text, out decimal amount))
            {
                if (amount == 0)
                {
                    lblAdditionalRate.Text = "0.00";
                    return;
                }

                if (decimal.TryParse(lblTotalValueAfterTax.Text, out decimal baseVal))
                {
                    if (baseVal > 0)
                    {
                        lblAdditionalRate.Text = CalculateRateFromAmount(baseVal, amount).ToString("N2");
                        CalculateInvoiceFooter();
                    }
                    else
                    {
                        txtValueAdded.Text = "0.00";
                        lblAdditionalRate.Text = "0.00";
                        CustomMessageBox.ShowInformation("يجب إدخال قيمة للفاتورة بعد الضريبة أولًا قبل حساب الإضافة.", "تنبيه");
                        txtValueAdded.Focus();
                    }
                }
            }
        }

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

        /// <summary>زر دفع كامل القيمة نقدًا</summary>
        private void btnAllCash_Click(object sender, EventArgs e)
        {
            if (!IsValidNetTotal()) return;

            SetFullPayment(txtPayment_Cash, txtPayment_Electronic);
        }


        /// <summary>زر دفع كامل القيمة بالفيزا</summary>
        private void btnAllVisa_Click(object sender, EventArgs e)
        {
            if (!IsValidNetTotal()) return;
            SetFullPayment(txtPayment_Electronic, txtPayment_Cash);
        }

        private void txtPayment_Cash_Leave(object sender, EventArgs e)
        {
            if (!IsValidNetTotal()) return;
            CalculateRemainingOnAccount();
        }

        private void txtPayment_Electronic_Leave(object sender, EventArgs e)
        {
            if (!IsValidNetTotal()) return;
            CalculateRemainingOnAccount();
        }

        /// <summary>
        /// دالة تقوم بتعيين كامل المبلغ في طريقة دفع واحدة وتصفير الأخرى
        /// </summary>
        private void SetFullPayment(TextBox primaryMethod, TextBox secondaryMethod)
        {
            primaryMethod.Text = lblNetTotal.Text;
            secondaryMethod.Text = "0.00";
            CalculateInvoiceFooter();
        }
        private void CalculateRemainingOnAccount()
        {
            // تحويل النصوص إلى أرقام بطريقة آمنة
            decimal.TryParse(lblNetTotal.Text, out decimal netTotal);
            decimal.TryParse(txtPayment_Cash.Text, out decimal cash);
            decimal.TryParse(txtPayment_Electronic.Text, out decimal electronic);

            // الحساب
            decimal paid = cash + electronic;
            decimal remaining = netTotal - paid;

            // عرض المتبقي
            lblRemainingOnAcc.Text = remaining.ToString("N2");

            // عرض الحالة والتلوين
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
                lblStateRemaining.ForeColor = Color.Blue ;
                lblRemainingOnAcc.ForeColor = Color.Blue ;

            }


        }


        private void btnTaxRate_Click(object sender, EventArgs e)
        {
            // محاولة تحويل قيمة الفاتورة الإجمالية
            if (decimal.TryParse(lblTotalInv.Text, out decimal baseVal) && baseVal > 0)
            {
                // إظهار مربع إدخال النسبة فقط إذا كانت القيمة أكبر من صفر
                if (CustomMessageBox.ShowDecimalInputBox(out decimal rate, "أدخل نسبة الإضافة %", "نسبة الإضافة") == DialogResult.OK)
                {
                    // حساب قيمة الإضافة
                    txtTaxVal.Text = (baseVal * (rate / 100)).ToString("N2");
                    lblTaxRate.Text = rate.ToString("N2");
                    CalculateInvoiceFooter(); // تحديث بيانات الفاتورة
                }
            }
            else
            {
                // في حالة أن القيمة صفر أو غير صالحة
                CustomMessageBox.ShowInformation ("لا يمكن حساب نسبة الإضافة قبل إدخال قيمة للفاتورة.", "تنبيه");
            }
        }



        private void btnDiscountRate_Click(object sender, EventArgs e)
        {
            // محاولة تحويل قيمة الفاتورة الإجمالية بعد الضريبة
            if (decimal.TryParse(lblTotalValueAfterTax.Text, out decimal baseVal) && baseVal > 0)
            {
                // إظهار مربع إدخال النسبة فقط إذا كانت القيمة أكبر من صفر
                if (CustomMessageBox.ShowDecimalInputBox(out decimal rate, "أدخل نسبة الخصم %", "نسبة الخصم") == DialogResult.OK)
                {
                    // حساب قيمة الإضافة
                    txtDiscount.Text = (baseVal * (rate / 100)).ToString("N2");
                    lblDiscountRate.Text = rate.ToString("N2");
                    CalculateInvoiceFooter(); // تحديث بيانات الفاتورة
                }
            }
            else
            {
                // في حالة أن القيمة صفر أو غير صالحة
                CustomMessageBox.ShowInformation("لا يمكن حساب نسبة الخصم قبل إدخال قيمة للفاتورة.", "تنبيه");
            }
            //---------------------------------------------------------
        }


        private void btnAdditionalRate_Click(object sender, EventArgs e)
        {
            // محاولة تحويل قيمة الفاتورة الإجمالية بعد الضريبة
            if (decimal.TryParse(lblTotalValueAfterTax.Text, out decimal baseVal) && baseVal > 0)
            {
                // إظهار مربع إدخال النسبة فقط إذا كانت القيمة أكبر من صفر
                if (CustomMessageBox.ShowDecimalInputBox(out decimal rate, "أدخل نسبة الاضافة %", "نسبة الاضافة") == DialogResult.OK)
                {
                    // حساب قيمة الإضافة
                    txtValueAdded .Text = (baseVal * (rate / 100)).ToString("N2");
                    lblAdditionalRate .Text = rate.ToString("N2");
                    CalculateInvoiceFooter(); // تحديث بيانات الفاتورة
                }
            }
            else
            {
                // في حالة أن القيمة صفر أو غير صالحة
                CustomMessageBox.ShowInformation("لا يمكن حساب نسبة الاضافة قبل إدخال قيمة للفاتورة.", "تنبيه");
            }

        }
        private void txtNoteInvoice_Enter(object sender, EventArgs e)
        {
            langManager.SetArabicLanguage();
            txtNoteInvoice.SelectAll();
        }

        private void txtPayment_Note_Enter(object sender, EventArgs e)
        {
            langManager.SetArabicLanguage();
            txtPayment_Note.SelectAll();
        }


        #endregion







        //الدفعة  الخامسة

        #region @@@@@@@@@@@@@@ DGVStyl تنسيقات التفاصيل  @@@@@@@@@@@@@@@@

        private void DGVStyl()
        {
            if (DGV.Columns.Count == 0) return;

            // إيقاف التحديثات مؤقتًا لتجنب أي عمليات إعادة دخول
            DGV.SuspendLayout();

            try
            {
                // إلغاء التحديد بطريقة آمنة
                if (DGV.CurrentCell != null && !DGV.IsCurrentCellInEditMode)
                {
                    DGV.ClearSelection();
                    DGV.CurrentCell = null;
                }

                DGV.ColumnHeadersVisible = true;
                DGV.EnableHeadersVisualStyles = false;
                DGV.RightToLeft = RightToLeft.Yes;

                bool allowEditing = string.IsNullOrWhiteSpace(lblSave.Text);

                // إخفاء الأعمدة وجعلها للقراءة فقط
                foreach (DataGridViewColumn col in DGV.Columns)
                {
                    col.Visible = false;
                    col.ReadOnly = true;
                }

                var visibleColumns = new (string Name, string Header, bool Editable, float FillWeight)[]
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

                foreach (var (Name, Header, Editable, FillWeight) in visibleColumns)
                {
                    if (!DGV.Columns.Contains(Name)) continue;

                    var col = DGV.Columns[Name];
                    col.Visible = true;
                    col.HeaderText = Header;
                    col.ReadOnly = !(Editable && allowEditing);
                    col.FillWeight = FillWeight;

                    if (Name == "TotalRow" || Name == "NetRow" || Name == "PriceMove" || Name == "Amount" || Name == "GemDisVal")
                    {
                        col.DefaultCellStyle.Format = "N2";
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }

                    if (Editable)
                    {
                        col.DefaultCellStyle.BackColor = Color.White;
                        col.DefaultCellStyle.ForeColor = Color.Black;
                    }

                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // تنسيق عام
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

                if (DGV.Columns.Contains("serInvDetail"))
                {
                    try
                    {
                        DGV.Sort(DGV.Columns["serInvDetail"], ListSortDirection.Ascending);
                    }
                    catch { }
                }

                DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                DGV.RowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
                DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.LemonChiffon;
            }
            catch 
            {
           //     MessageBox.Show("حدث خطأ أثناء تنسيق الجدول:\n" + ex.Message);
            }
            finally
            {
                // استئناف التحديثات
                DGV.ResumeLayout();
            }
        }

        private void DGV_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true; // منع الصوت أو السلوك الافتراضي

                var currentCell = DGV.CurrentCell;
                if (currentCell == null) return;

                string[] editableCols = { "PriceMove", "Amount", "GemDisVal" };
                int colIndex = Array.IndexOf(editableCols, currentCell.OwningColumn.Name);

                if (colIndex >= 0)
                {
                    // انتقل إلى العمود التالي داخل نفس الصف
                    int nextColIndex = (colIndex + 1) % editableCols.Length;

                    int rowIndex = currentCell.RowIndex;
                    if (nextColIndex == 0) // يعني أكمل دورة السعر ← الكمية ← الخصم
                    {
                        // انتقل إلى السطر التالي
                        if (rowIndex + 1 < DGV.Rows.Count)
                            rowIndex++;
                        else
                            return;
                    }

                    DGV.CurrentCell = DGV.Rows[rowIndex].Cells[editableCols[nextColIndex]];
                }
            }
        }
        private void DGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string columnName = DGV.Columns[e.ColumnIndex].Name;

            // الأعمدة القابلة للتعديل
            string[] editableColumns = { "PriceMove", "Amount", "GemDisVal" };

            bool isEditable = editableColumns.Contains(columnName);
            bool isSaved = !string.IsNullOrWhiteSpace(lblSave.Text);

            // خلفيات الصفوف المتعاقبة
            Color evenBackColor = Color.WhiteSmoke;
            Color oddBackColor = Color.LemonChiffon;

            // التحقق من أن CellStyle غير null أولاً
            if (e.CellStyle != null)
            {
                // حدد خلفية حسب رقم الصف
                e.CellStyle.BackColor = (e.RowIndex % 2 == 0) ? evenBackColor : oddBackColor;

                if (isEditable && !isSaved)
                {
                    // الحقول المتاحة للتعديل قبل الحفظ
                    e.CellStyle.ForeColor = Color.DarkBlue;
                    e.CellStyle.Font = new Font("Times New Roman", 14, FontStyle.Bold);
                }
                else
                {
                    // الحقول غير القابلة للتعديل أو الفاتورة محفوظة
                    e.CellStyle.ForeColor = Color.FromArgb(100, 100, 100); // رمادي واضح قليلاً
                    e.CellStyle.Font = new Font("Times New Roman", 14, FontStyle.Regular);
                }
            }

        }


        private void ToggleControlsBasedOnSaveStatus()
        {
            bool isFinalSaved = !string.IsNullOrWhiteSpace(lblSave.Text);
            ToggleControlsRecursive(this.Controls, isFinalSaved);
            DGVStyl();
        }

        private void ToggleControlsRecursive_(Control.ControlCollection controls, bool isFinalSaved)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is TextBox tb)
                {
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

                if (ctrl.HasChildren)
                {
                    ToggleControlsRecursive(ctrl.Controls, isFinalSaved);
                }
            }


            ;
        }
        private void ToggleControlsRecursive(Control.ControlCollection controls, bool isFinalSaved)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is TextBox tb)
                {
                    // استثناء لبعض الـ TextBox التي يجب تعطيلها تمامًا (لمنع اختصارات مثل Ctrl+F)
                    if (tb.Name == "txtAccName" || tb.Name == "txtSeaarchProd")
                    {
                        tb.Enabled = !isFinalSaved;
                    }
                    else
                    {
                        tb.ReadOnly = isFinalSaved;
                    }
                }
                else if (ctrl is ComboBox || ctrl is DateTimePicker)
                {
                    ctrl.Enabled = !isFinalSaved;
                }
                else if (ctrl is DataGridView dgv)
                {
                    dgv.ReadOnly = isFinalSaved;
                }

                // التكرار داخل العناصر الفرعية
                if (ctrl.HasChildren)
                {
                    ToggleControlsRecursive(ctrl.Controls, isFinalSaved);
                }
            }
        }

        private void PrepareEmptyGridStructure()
        {
            DGV.Columns.Clear();
            DGV.AutoGenerateColumns = false;

            // أعمدة ظاهرة
            AddTextColumn("ProductCode", "الكود");
            AddTextColumn("ProdName", "اسم الصنف", 200);
            AddTextColumn("UnitProd", "الوحدة");
            AddTextColumn("PriceMove", "السعر", format: "N2", alignRight: true);
            AddTextColumn("Amount", "الكمية", format: "N2", alignRight: true);
            AddTextColumn("TotalRow", "الإجمالي", format: "N2", alignRight: true, readOnly: true);
            AddTextColumn("GemDisVal", "الخصم", format: "N2", alignRight: true);
            AddTextColumn("NetRow", "الصافي", format: "N2", alignRight: true, readOnly: true);

            // أعمدة مخفية مهمة للحفظ
            AddHiddenColumn("serInvDetail");
            AddHiddenColumn("Inv_ID_fk");
            AddHiddenColumn("PieceID_fk");
            AddHiddenColumn("AIn");
            AddHiddenColumn("AOut");
            AddHiddenColumn("ReturnedInInvoiceNo");
            AddHiddenColumn("B_Price");
            AddHiddenColumn("U_Price");
            AddHiddenColumn("MinLenth");
            AddHiddenColumn("MinStock");
            AddHiddenColumn("ProductStock");
        }

        // دالة مساعدة لإضافة عمود نصي
        private void AddTextColumn(string name, string header, int width = 100, string? format = null, bool alignRight = false, bool readOnly = false)
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

            col.DefaultCellStyle.Alignment = alignRight ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleCenter;
            col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DGV.Columns.Add(col);
        }

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

        private void DGV_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox tb)
            {
                // إزالة الاشتراك السابق لتجنب التكرار
                tb.Enter -= TextBox_Enter_SelectAll;//Nullability of reference types in type of parameter 'sender' of 'void frm_NewInvoice.TextBox_Enter_SelectAll(object sender, EventArgs e)' doesn't match the target delegate 'EventHandler' (possibly because of nullability attributes).

                // التحقق من اسم العمود الحالي
                string? columnName = DGV.CurrentCell?.OwningColumn?.Name;//Converting null literal or possible null value to non-nullable type.

                if (columnName == "PriceMove" || columnName == "Amount" || columnName == "GemDisVal")
                {
                    // إذا كانت الخلية ضمن الأعمدة الثلاثة المسموح تعديلها
                    tb.Enter += TextBox_Enter_SelectAll;//نفس الخطأ
                }
            }
        }

        private void TextBox_Enter_SelectAll(object? sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
            }
        }


        private void DGV_SelectionChanged(object sender, EventArgs e)
        {
            // التحقق من أن الـ DataGridView تحتوي على صفوف فعلية
            if (DGV.Rows.Count == 0 || DGV.CurrentRow == null || DGV.CurrentRow.IsNewRow)
            {
                lblMinLinth.Text = "";
                lblProductStock.Text = "";
                return;
            }

           // إذا وصلنا هنا، إذًا الصف يحتوي على بيانات فعلية
            if (DGV.Columns.Contains("MinLenth"))
                lblMinLinth.Text = DGV.CurrentRow.Cells["MinLenth"].Value?.ToString();
            else
                lblMinLinth.Text = "";

            if (DGV.Columns.Contains("ProductStock"))
                lblProductStock.Text = DGV.CurrentRow.Cells["ProductStock"].Value?.ToString();
            else
                lblProductStock.Text = "";
        }


        #endregion




        // الدفعة الاخيرة

        #region @@@@@@@@@@@@@@@@ Save item Changs @@@@@@@@@@@@@@

        //حفظ التغيرات فى DGV
        private void DGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (DGV.Columns[e.ColumnIndex] == null) return;

            string colName = DGV.Columns[e.ColumnIndex].Name;

            // احفظ فقط لو الخلية تخص أعمدة قابلة للحفظ
            string[] editableCols = { "PriceMove", "Amount", "GemDisVal" };

            if (editableCols.Contains(colName))
            {
                SaveDetailsChanges(DGV.Rows[e.RowIndex]);
            }


            // يُفضل بعد الحفظ، إعادة احتساب الإجماليات
            CalculateInvoiceFooter(); // لو موجودة عندك

        }

        private void SaveDetailsChanges(DataGridViewRow row)
        {
            // لا يتم الحفظ إذا كانت الفاتورة محفوظة بالفعل
            if (!string.IsNullOrWhiteSpace(lblSave.Text)) return;

            // تحقق من صلاحية الصف
            if (row == null || row.IsNewRow) return;

            // التحقق من وجود العمود serInvDetail في DataGridView
            if (!DGV.Columns.Contains("serInvDetail") ||
                !int.TryParse(row.Cells["serInvDetail"]?.Value?.ToString(), out int serInvDetail))
                return;

            // استخراج القيم وتأكيد صلاحيتها
            float.TryParse(row.Cells["PriceMove"]?.Value?.ToString(), out float price);
            float.TryParse(row.Cells["Amount"]?.Value?.ToString(), out float amount);
            float.TryParse(row.Cells["GemDisVal"]?.Value?.ToString(), out float gemDisc);

            // حفظ التغييرات في قاعدة البيانات
            DBServiecs.NewInvoice_UpdateInvoiceDetail(serInvDetail, price, amount, gemDisc);

            // إعادة حساب القيم الظاهرة في الجدول
            float total = price * amount;
            float net = total - gemDisc;

            // تحديث العمود TotalRow
            if (DGV.Columns.Contains("TotalRow"))
                row.Cells["TotalRow"].Value = total;
            if (DGV.Columns.Contains("NetRow"))
                row.Cells["NetRow"].Value = net;

            // تحديث تجميعة الفاتورة
            CalculateInvoiceFooter();
        }


        #endregion

        #region @@@@@ Row Navigate @@@@@@@@

        // زر أول فاتورة
        private void btnFrist_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded())
            {
                NavigateToInvoice(0);
            }
        }

        // زر التالي
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded())
            {
                if (currentInvoiceIndex < tblInv.Rows.Count - 1)
                {
                    NavigateToInvoice(currentInvoiceIndex + 1);
                }
                else
                {
                    MessageBox.Show("تم الوصول إلى آخر فاتورة.");
                }
            }
        }

        // زر السابق
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded())
            {
                if (currentInvoiceIndex > 0)
                {
                    NavigateToInvoice(currentInvoiceIndex - 1);
                }
                else
                {
                    MessageBox.Show("تم الوصول إلى أول فاتورة.");
                }
            }
        }

        // زر آخر فاتورة
        private void btnLast_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded())
            {
                NavigateToInvoice(tblInv.Rows.Count - 1);
            }
        }

        // زر إنشاء فاتورة جديدة
        private void btnNew_Click(object sender, EventArgs e)
        {
            SetDefaultAccount();

            if (tblInv == null)
                GetInvoices();

            string nextCounter = DBServiecs.NewInvoice_GetNewCounter(Type_ID);
            int nextID = DBServiecs.NewInvoice_GetNewID();

            lblInv_Counter.Text = nextCounter;
            lblInv_ID.Text = nextID.ToString();

            DisplayNewRow(Type_ID, US);
            ToggleControlsBasedOnSaveStatus();



        }

        // الدالة المركزية للتنقل
        private void NavigateToInvoice(int targetIndex)
        {
            if (!EnsureInvoicesLoaded()) return;

            // منع الخروج عن الحدود
            targetIndex = Math.Max(0, Math.Min(targetIndex, tblInv.Rows.Count - 1));

            currentInvoiceIndex = targetIndex;
            DisplayCurentRow(currentInvoiceIndex);
            ToggleControlsBasedOnSaveStatus();
            ToggleNavigationButtons();

            // ✅ تحديث معلومات الترقيم
            lblInfoInvoice.Text = $"فاتورة {targetIndex + 1} من {tblInv.Rows.Count}";
        }

        // تحديث حالة أزرار التنقل
        private void ToggleNavigationButtons()
        {
            btnFrist.Enabled = currentInvoiceIndex > 0;
            btnPrevious.Enabled = currentInvoiceIndex > 0;
            btnNext.Enabled = currentInvoiceIndex < tblInv.Rows.Count - 1;
            btnLast.Enabled = currentInvoiceIndex < tblInv.Rows.Count - 1;
        }

        // التأكد من تحميل الفواتير
        private bool EnsureInvoicesLoaded()
        {
            if (tblInv == null || tblInv.Rows.Count == 0)
            {
                GetInvoices();
            }

            if (tblInv == null || tblInv.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد فواتير.");
                return false;
            }

            return true;
        }

        public void DisplayNewRow(int invType, int Us_id)//9
        {
            dtpInv_Date.Value = DateTime.Now;
            cbxSellerID.SelectedValue = 26; // يمكن جعله ديناميكيًا لاحقًا
            lblUserID.Text = Us_id.ToString();

            // تعيين الحساب بناءً على نوع الفاتورة
            lblAccID.Text = (invType == 1 || invType == 2) ? "40" :
                            (invType == 3 || invType == 4) ? "41" : "0";

            // تفريغ الحقول الرقمية والنصية
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

            txtPayment_Note.Text = "";
            txtNoteInvoice.Text = "";
            lblSave.Text = "";
            txtSeaarchProd.Text = "0";
            txtAmount.Text = "0";
            if (Type_ID == 1 || Type_ID == 3)
            {
                lblProductName.Text = "Product Name :";lblCodeTitel .Text = "كود صنف";
            }
            else if (Type_ID == 2 || Type_ID == 4)
            {
                lblProductName.Text = "Invoice No :"; lblCodeTitel.Text = "فاتورة بيع رقم";
            }
            lblPriceMove.Text = "0";
            DGV.DataSource = null;
            cbxPiece_ID.DataSource = null;
            lblCount .Text = "0";
            lblInfoInvoice.Text = "فاتورة جديدة";
        }


        #endregion

        #region @@@@@@@@ الانتقال والحفظ المؤقت @@@@@@@@@@@@@@@
        //الاشتراك فى الاحداث
        private void RegisterEvents()
        {
            //foreach (Control ctrl in inputFieldsBeforeSearch.Concat(inputFieldsAfterSearch))
            //{
            //    ctrl.KeyDown += InputFields_KeyDown;
            //    ctrl.Leave += InputFields_Leave;
            //}
        }
        private void InputFields_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblSave.Text)) // لم يتم الحفظ نهائياً
            {
                SaveDraftInvoice();
            }
        }
        private void InputFields_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            Control? current = sender as Control;

            // قائمة الحقول
            Control[] fullList = inputFieldsBeforeSearch.Concat(inputFieldsAfterSearch).ToArray();

            int index = Array.IndexOf(fullList, current);

            // خاصية التوقف عند txtSeaarchProd
            if (current == txtSeaarchProd)
            {
                txtSeaarchProd.Focus(); // لا تنتقل، نفذ إجراءاتك داخل TextBox هذا
                return;
            }

            if (index >= 0 && index < fullList.Length - 1)
            {
                e.SuppressKeyPress = true; // منع الصوت أو التأثير الافتراضي للـ Enter
                fullList[index + 1].Focus();
                if (fullList[index + 1] is TextBox tb) tb.SelectAll();
            }
        }



        #endregion

        #region @@@@@@@@@ الحفظ النهائى @@@@@@@@@@@@@@@@@    
        // دالة الحفظ النهائى - عند الضغط على زر الحفظ
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(lblSave.Text))
            {
                CustomMessageBox .ShowInformation ("تم حفظ الفاتورة من قبل ولا يمكن تعديلها.","تنبيه");
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
            string saveText = GetSaveTextByInvoiceType(Type_ID);

            SaveDraftInvoice(saveText); // الحفظ النهائي

            lblSave.Text = saveText;
            MessageBox.Show("تم الحفظ النهائي للفاتورة.");
            ToggleControlsBasedOnSaveStatus();
        }

        //دالة كتابة النص المناسب للحفظ
        private string GetSaveTextByInvoiceType(int typeID)
        {
            switch (typeID)
            {
                case 1: return "تم حفظ البيع";
                case 2: return "تم حفظ مردودات البيع";
                case 3: return "تم حفظ الشراء";
                case 4: return "تم حفظ مردودات الشراء";
                default: return "تم حفظ الفاتورة";
            }
        }
        #endregion

        private void btnJournal_Click(object sender, EventArgs e)
        {
            // تحقق من أن السند محفوظ (أي أن lblSave تحتوي على نص)
            if (!string.IsNullOrWhiteSpace(lblSave.Text))
            {
                int billNo;
                int invTypeId;

                // تأكد من أن رقم السند ونوع الفاتورة متوفران بشكل صحيح
                if (int.TryParse(lblInv_ID .Text, out billNo) && int.TryParse(lblTypeInvID.Text, out invTypeId))
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
        */
    }
}
