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


using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.Views.Forms.Accounts;
using MizanOriginalSoft.MainClasses.SearchClasses;
using MizanOriginalSoft.Views.Forms.MainForms;
using MizanOriginalSoft.MainClasses.OriginalClasses.InvoicClasses; // هنا يوجد enum InvoiceType

namespace MizanOriginalSoft.Views.Forms.Movments
{


    public partial class frm_DynamicInvoice : Form
    {
        #region Fields
        private InvoiceType currentInvoiceType; // نوع الفاتورة الحالية

        // 🔹 متغير يحدد إذا كان مسموح البيع بدون رصيد (على المكشوف)
        private bool allowNegativeStock;

        // 🔹 متغير يحدد إذا كان المرتجع يشترط إدخال رقم فاتورة البيع
        private int returnSaleMode;

        // 🔹 متغير يحدد إذا كان المرتجع يشترط إدخال رقم فاتورة البيع
        private int returnPurchaseMode;

        // 🟦 متغيرات إعدادات
        private decimal MaxRateDiscount = 0m;
        private bool AllowChangeTax = true;

        #endregion
        #region 🔹 المتغيرات العامة

        // 📝 DataTable يحتوي كل الفواتير المحملة من قاعدة البيانات
        private DataTable tblInv = new DataTable();

        // 📝 مؤشر الفاتورة الحالية داخل DataTable
        private int currentInvoiceIndex = 0;
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
        // private int US;          // كود المستخدم
        private int Inv_ID;      // رقم الفاتورة
        private int ID_Prod;
        private int Piece_id = 0;

        private DataTable? tblAcc = null;

        // متغيرات مالية
        private int PieceID;
        private float PriceMove;
        private float Amount;
        private float TotalRow;
        private float GemDisVal;
        private float ComitionVal = 0;
        private float NetRow;

        // تحديد إذا كان يمكن قص القطعة (true = يمكن قصها)
        public bool isCanCut = true;

        // معرف الوحدة الحالي
        private int unit_ID;

        //// محاولة قراءة كمية صحيحة > 0
        //private bool TryGetValidAmount(out float amount)
        //{
        //    return float.TryParse(txtAmount.Text, out amount) && amount > 0;
        //}

        //// محاولة قراءة سعر صحيح > 0
        //private bool TryGetValidPrice(out float price)
        //{
        //    price = PriceMove; // وضع القيمة في out
        //    return price > 0;  // إرجاع true إذا كان السعر > 0
        //}


        // 🔹 التحقق من حفظ الفاتورة   ***
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


        private KeyboardLanguageManager langManager;
        /*هل الترتيب للاحداث والدوال هنا منطقى ام هو السبب فى ذلك*/
        public frm_DynamicInvoice()
        {
            InitializeComponent();
            langManager = new KeyboardLanguageManager(this);
        }

        public void InitializeInvoice(InvoiceType type)
        {
            // 🔹 تعيين النوع الحالي
            currentInvoiceType = type;
            ConfigureAutoCompleteForAccount();
            // 🔹 تحديد العنوان ورقم النوع
            (string arabicTitle, string typeId) = type switch
            {
                InvoiceType.Sale => ("فاتورة بيع رقم: ", "1"),
                InvoiceType.SaleReturn => ("فاتورة بيع مرتد رقم: ", "2"),
                InvoiceType.Purchase => ("فاتورة شراء رقم: ", "3"),
                InvoiceType.PurchaseReturn => ("فاتورة شراء مرتد رقم: ", "4"),
                InvoiceType.Inventory => ("إذن تسوية مخزن رقم: ", "5"),
                InvoiceType.DeductStock => ("إذن خصم مخزن رقم: ", "6"),
                InvoiceType.AddStock => ("إذن إضافة مخزن رقم: ", "7"),
                _ => ("فاتورة", "0")
            };

            // 🔹 تحديث العناوين في الفورم
            this.Text = arabicTitle;
            lblTypeInv.Text = arabicTitle;
            lblTypeInvID.Text = typeId;

            // 🔹 تجهيز الحقول الأساسية
            FillDefaultAccount();

            FillSellerComboBox();
            SetupFormByInvoiceType();

            // 🔥 تحميل كل الفواتير
            GetInvoices();

            // 🔥 الانتقال مباشرة إلى آخر فاتورة (الجديدة)
            if (tblInv != null && tblInv.Rows.Count > 0)
            {
                currentInvoiceIndex = tblInv.Rows.Count - 1; // آخر صف
                DisplayCurentRow(currentInvoiceIndex);       // يعرض الفاتورة الجديدة
            }
            else
            {

                lblInfoInvoice.Text = "لا توجد فواتير";
                PrepareEmptyGridStructure();
                DGV.DataSource = null;
            }
        }

        // جلب تفاصيل الفاتورة (أصنافها + تهيئة الجدول)
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

        // جلب الفواتير حسب النوع + إضافة فاتورة جديدة فارغة
        private void GetInvoices()
        {
            tblInv = DBServiecs.NewInvoice_GetInvoicesByType((int)currentInvoiceType);

            DataRow newRow = tblInv.NewRow();
            newRow["Inv_ID"] = DBServiecs.NewInvoice_GetNewID();
            newRow["Inv_Counter"] = DBServiecs.NewInvoice_GetNewCounter((int)currentInvoiceType);
            newRow["MovType"] = lblTypeInv.Text;
            newRow["Inv_Date"] = DateTime.Now;
            newRow["Seller_ID"] = cbxSellerID.Items.Count > 0 ? cbxSellerID.SelectedValue : DBNull.Value;
            //       newRow["User_ID"] = US;
            newRow["Acc_ID"] = lblAccID.Text;
            newRow["AccName"] = txtAccName.Text;

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
        private void frm_DynamicInvoice_Load(object sender, EventArgs e)
        {
            DBServiecs.A_UpdateAllDataBase();   // تحديث أرصدة الأصناف والحسابات
            cbxPiece_ID.DropDownStyle = ComboBoxStyle.DropDownList;

            // ✅ قراءة سياسات البيع والبيع المرتد
            LoadSalesPolicies();

            // ✅ تحويل النص لرقم أولاً
            if (int.TryParse(lblTypeInvID.Text, out int typeInvID))
            {
                // 🔹 حدد نوع الفاتورة
                switch (typeInvID)
                {
                    case 1:
                        currentInvoiceType = InvoiceType.Sale;
                        UpdateLabelsForSale();
                        break;

                    case 2:
                        currentInvoiceType = InvoiceType.SaleReturn;
                        UpdateLabelsForReturn();
                        break;

                    case 3:
                        currentInvoiceType = InvoiceType.Purchase;
                        UpdateLabelsForPurchase();
                        break;

                    case 4:
                        currentInvoiceType = InvoiceType.PurchaseReturn;
                        UpdateLabelsForReturn();
                        break;

                    case 5:
                        currentInvoiceType = InvoiceType.Inventory;
                        UpdateLabelsForInventory();
                        break;

                    case 6:
                        currentInvoiceType = InvoiceType.DeductStock;
                        UpdateLabelsForDeduct();
                        break;

                    case 7:
                        currentInvoiceType = InvoiceType.AddStock;
                        UpdateLabelsForAddStock();
                        break;

                    default:
                        currentInvoiceType = InvoiceType.Sale;
                        UpdateLabelsForSale();
                        break;
                }
            }

            LoadFooterSettings();
            CalculateInvoiceFooter();
            DGVStyl();          // تنسيق الداتا جريد
            RegisterEvents();   // ربط أحداث إضافية
                                // هنا هتحدد أي TextBox عايزه يقبل أرقام + فاصلة عشرية
            AttachDecimalValidation(txtTaxVal, txtDiscount, txtValueAdded, txtPayment_Cash, txtPayment_Electronic);

            // ولو عايز صناديق تانية للأرقام الصحيحة فقط
            AttachIntegerValidation(txtSeaarchProd);

        }





        #region تنقل بين الحقول
        private void RegisterEvents()
        {
            foreach (Control ctrl in inputFieldsBeforeSearch.Concat(inputFieldsAfterSearch))
            {
                ctrl.KeyDown += InputFields_KeyDown;
                ctrl.Leave += InputFields_Leave;
            }
        }
        // عند مغادرة أي حقل إدخال يتم الحفظ كمسودة إذا لم تكن الفاتورة محفوظة نهائيًا.
        private void InputFields_Leave(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblSave.Text)) // لم يتم الحفظ النهائي بعد
            {
                SaveDraftInvoice();
            }
        }
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




        #region ******** cbxPiece_ID events

        private void cbxPiece_ID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;   // ✅ منع الانتقال الافتراضي للسطر التالي
                txtAmount.Visible = true;
                txtAmount.Focus();
                txtAmount.SelectAll();       // ✅ تحديد كل النص داخل التكست
            }
        }

        // عند تغيير القطعة 
        private void cbxPiece_ID_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePieceLabels();// التحديث التلقائى لليبل
        }









        #endregion









        #region أحداث عناصر الواجهة (Inputs & Controls)
        //

        // عند الضغط على Enter في ComboBox البائع → حفظ مسودة الفاتورة ثم الانتقال للبحث عن المنتج
        private void cbxSellerID_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                SaveDraftInvoice();
                // Enter فقط → التالي
                txtSeaarchProd.Focus();
                txtSeaarchProd.SelectAll();
                e.Handled = true;
            }
            else if ((e.KeyCode == Keys.Enter && e.Shift) || e.KeyCode == Keys.Up)
            {
                // Shift+Enter أو سهم ↑ → السابق
                this.SelectNextControl((Control)sender, false, true, true, true);
                e.Handled = true;
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



        #region  *********  txtAmount_KeyDown احداث ******************


        // 🔹 حدث إدخال الكمية (Enter في txtAmount)
        private void txtAmount_KeyDown(object sender, KeyEventArgs e)
        {
            // الخروج إذا لم يضغط المستخدم Enter أو إذا كانت الفاتورة محفوظة مسبقًا
            if (e.KeyCode != Keys.Enter || IsInvoiceSaved())
                return;

            // حفظ مؤشر الفاتورة الحالي (لتحديد موقع العودة)لاحقا
            int currentIndexBeforeInsert = currentInvoiceIndex;

            // حفظ الفاتورة مؤقتًا لتجنب فقدان البيانات
            SaveDraftInvoice();

            // التحقق من أن القيمة رقم وصحيحة (>0)
            if (!float.TryParse(txtAmount.Text, out float amount) || amount <= 0)
            {
                // 🔹 تظليل الحقل بالأحمر
                txtAmount.BackColor = Color.LightPink;

                CustomMessageBox.ShowWarning("يرجى إدخال كمية صحيحة", "خطأ");
                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }
            else
            {
                // 🔹 إعادة اللون الطبيعي عند إدخال قيمة صحيحة
                txtAmount.BackColor = SystemColors.Window;
            }

            // قراءة طول القطعة
            float.TryParse(cbxPiece_ID.Text, out float pieceLength);

            // إدراج الصف حسب نوع الفاتورة
            switch (currentInvoiceType)
            {
                case InvoiceType.Sale:
                    InsertSaleRow(amount, pieceLength);
                    break;

                case InvoiceType.SaleReturn:
                    //                   InsertReSaleRow(amount);
                    InsertRow(unit_ID == 1);
                    break;

                case InvoiceType.Purchase:
                    InsertPurchaseRow(amount);
                    break;

                case InvoiceType.PurchaseReturn:
                    InsertRePurchaseRow(amount);
                    break;

                case InvoiceType.Inventory:
                    InsertInventoryRow(amount);
                    break;

                default:
                    CustomMessageBox.ShowWarning("نوع الفاتورة غير مدعوم", "خطأ");
                    return;
            }

            // تحديث البيانات والواجهة بعد الإدخال
            DBServiecs.A_UpdateAllDataBase();
            PrepareSaleProduct(txtSeaarchProd.Text);
            GetInvoices();
            NavigateToInvoice(currentIndexBeforeInsert);
            CalculateInvoiceFooter();
        }


        // إدراج منتج في فاتورة بيع.          ***
        private void InsertSaleRow(float amount, float pieceLength)
        {
            if (unit_ID == 1) // منتج يقبل القص
            {
                float minLength = float.Parse(lblMinLinth.Text);
                float remaining = pieceLength - amount;

                if (remaining >= minLength || remaining == 0)
                {
                    InsertRow(true);
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
                    }
                    else
                    {
                        txtAmount.Visible = true;
                        txtAmount.Focus();
                        txtAmount.SelectAll();
                    }
                }
            }
            else // منتج لا يقبل القص
            {
                if (float.TryParse(lblProductStock.Text, out float stock))
                {
                    // تحقق: هل الكمية أكبر من المخزون؟
                    if (amount > stock)
                    {
                        // إذا لا يسمح بالبيع بالسالب → منع العملية
                        if (!allowNegativeStock)
                        {
                            CustomMessageBox.ShowWarning(
                                "⚠️ الكمية المطلوبة أكبر من الرصيد ولا يسمح بالبيع على المكشوف.",
                                "تنبيه"
                            );
                            txtAmount.Focus();
                            txtAmount.SelectAll();
                            return;
                        }
                        // إذا يسمح بالسالب → يكمل عادي (لا يفعل شيء هنا)
                    }
                }

                // لو فيه حد أدنى (أحيانًا يستخدم للتحويل من وحدة لأخرى)
                if (float.TryParse(lblMinLinth.Text, out float minLength2))
                {
                    txtAmount.Text = (amount * minLength2).ToString();
                }

                // إدراج الصنف
                InsertRow(false);
            }
        }



        // إدراج منتج في فاتورة شراء.
        private void InsertPurchaseRow(float amount)
        {
            if (amount <= 0)
            {
                CustomMessageBox.ShowWarning("الرجاء إدخال كمية صحيحة أكبر من الصفر.", "تنبيه");
                txtAmount.Visible = true;
                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }

            InsertRow(unit_ID == 1);
        }

        // إدراج منتج في فاتورة شراء مرتد.
        private void InsertRePurchaseRow(float amount)
        {
            if (amount <= 0)
            {
                CustomMessageBox.ShowWarning("الرجاء إدخال كمية صحيحة أكبر من الصفر.", "تنبيه");
                txtAmount.Visible = true;
                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }

            InsertRow(unit_ID == 1);
        }

        // إدراج منتج في فاتورة جرد أو تسوية.
        private void InsertInventoryRow(float amount)
        {
            if (currentInvoiceType == InvoiceType.Inventory)
            {
                if (isGardDone) // يعني = true
                {
                    DialogResult result = CustomMessageBox.ShowQuestion(
                        "هذا الصنف تم جرده من قبل فى عملية الجرد المفتوحة.\nهل تريد إعادة جرده مرة أخرى؟",
                        "تنبيه"
                    );

                    if (result == DialogResult.OK)
                    {
                        // حذف السطر السابق من جدول الجرد
                        if (int.TryParse(lblPieceID.Text, out int pieceID))
                        {
                            DBServiecs.Product_DeleteLastRowInventry(pieceID);
                        }

                        // تنفيذ الإدراج الجديد
                        InsertRow(unit_ID == 1);
                    }

                    return; // وقف التنفيذ بعد المعالجة (سواء Yes أو No)
                }
            }

            // ✅ في كل الحالات الأخرى (فاتورة جرد جديدة أو غير جرد)
            InsertRow(unit_ID == 1);
        }



        // إدراج صف جديد في تفاصيل الفاتورة
        private void InsertRow(bool isPiece)
        {

            if (unit_ID == 1) // قابل للقص
            {
                // تحديد قيمة UpPiece_ID حسب نوع الفاتورة
                int upPieceID;

                // لو نوع الفاتورة مبيعات أو مرتجع شراء
                if (currentInvoiceType == InvoiceType.Sale || currentInvoiceType == InvoiceType.PurchaseReturn)
                {
                    // في حالة المبيعات أو المشتريات المرتدة → ناخد القيمة من الـ Label
                    if (!int.TryParse(lblPieceID.Text, out upPieceID))
                    {
                        CustomMessageBox.ShowWarning("معرف القطعة غير صالح", "خطأ");
                        return;
                    }
                }
                else
                {
                    // باقي أنواع الفواتير → نثبتها بـ -1
                    upPieceID = -1;
                }

                // إنشاء قطعة جديدة
                int newPieceID = DBServiecs.Product_CreateNewPiece(ID_Prod, upPieceID);
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
            AfterInsertActions();
            DGVStyl();
        }

        // الإجراءات التي تتم بعد إدراج صف جديد
        private void AfterInsertActions()
        {
            txtSeaarchProd.Focus();
            txtSeaarchProd.SelectAll();
            txtAmount.Text = "0";
            lblGemDisVal.Text = "0";
        }



        // 🔹  إدخال صنف جديد إلى تفاصيل الفاتورة وحفظه في قاعدة البيانات.
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


        #endregion

        #region *********  txtSeaarchProd_KeyDown   ************

        private void txtSeaarchProd_KeyDown(object sender, KeyEventArgs e)
        {
            // 🟦 ENTER → تنفيذ
            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(txtSeaarchProd.Text))
            {
                if (IsInvoiceSaved()) return;

                string code = txtSeaarchProd.Text.Trim();
                HandleSearchByInvoiceType(code);

                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            // 🟦 CTRL+F → فتح شاشة البحث
            if (e.Control && e.KeyCode == Keys.F)
            {
                var code = SearchProductOrInvoice();
                if (!string.IsNullOrEmpty(code))
                    txtSeaarchProd.Text = code;
                return;
            }
        }

        // 🔹 تحديد وضع الكود
        private void HandleSearchByInvoiceType(string code)
        {
            switch (currentInvoiceType)
            {
                case InvoiceType.Sale:
                    HandleSale(code);
                    break;

                case InvoiceType.SaleReturn:
                    HandleSaleReturn(code);
                    break;

                case InvoiceType.Purchase:
                    HandlePurchase(code);
                    break;

                case InvoiceType.PurchaseReturn:
                    HandlePurchaseReturn(code);
                    break;

                case InvoiceType.Inventory:
                    HandleInventory(code);
                    break;

                case InvoiceType.DeductStock:
                    HandleInventoryLoss(code);
                    break;

                case InvoiceType.AddStock:
                    HandleInventoryAddition(code);
                    break;

                default:
                    CustomMessageBox.ShowWarning("نوع الفاتورة غير مدعوم", "خطأ");
                    break;
            }
        }

        // 🔹 متابعة حالة البيع
        private void HandleSale(string code)
        {
            if (!GetProd(code)) return;
            LoadPieceData();
        }

        private void HandleSaleReturn(string code)
        {
            if (rdoInvoice.Checked)
            {
                OpenReturnedInvoiceForm(code); // رقم فاتورة أصلية
            }
            else // rdoFree = كود صنف مباشرة
            {
                if (!GetProd(code)) return;
                LoadPieceData();
            }
        }

        private void HandlePurchase(string code)
        {
            if (!GetProd(code)) return;
            LoadPieceData();
        }

        private void HandlePurchaseReturn(string code)
        {
            if (rdoInvoice.Checked)
            {
                OpenReturnedInvoiceForm(code);
            }
            else
            {
                if (!GetProd(code)) return;
                LoadPieceData();
            }
        }

        private void HandleInventory(string code)
        {
            if (!GetProd(code)) return;
            LoadPieceData();
        }

        private void HandleInventoryLoss(string code)
        {
            if (!GetProd(code)) return;
            if (unit_ID == 1) // يقبل القص
            {
                LoadPieceData();
            }
            else
            {
                CustomMessageBox.ShowWarning("الصنف غير قابل للقص، تعامل معه من شاشة الجرد.", "تنبيه");
                ResetSearchBox();
            }
        }

        private void HandleInventoryAddition(string code)
        {
            if (!GetProd(code)) return;
            LoadPieceData();
        }


        #region ===== دوال مساعدة =====

        private void ResetSearchBox()
        {
            txtSeaarchProd.Clear();
            txtSeaarchProd.Focus();
        }

        // 🔹 دالة البحث حسب نوع الفاتورة واختيار المستخدم
        private string SearchProductOrInvoice()
        {
            // 🟢 في حالة فاتورة مرتجع
            if (currentInvoiceType == InvoiceType.SaleReturn)
            {
                if (rdoFree.Checked)
                {
                    // 🔍 بحث عن صنف
                    var provider = new GenericSearchProvider(SearchEntityType.Products);
                    var result = SearchHelper.ShowSearchDialog(provider);
                    return result.Code;
                }
                else if (rdoInvoice.Checked)
                {
                    // 🔍 بحث عن فاتورة قديمةللمبيعات
                    var provider = new GenericSearchProvider(SearchEntityType.SaleInvoices);
                    var result = SearchHelper.ShowSearchDialog(provider);
                    return result.Code;
                }
            }
            else if (currentInvoiceType == InvoiceType.PurchaseReturn)
            {
                if (rdoFree.Checked)
                {
                    // 🔍 بحث عن صنف
                    var provider = new GenericSearchProvider(SearchEntityType.Products);
                    var result = SearchHelper.ShowSearchDialog(provider);
                    return result.Code;
                }
                else if (rdoInvoice.Checked)
                {
                    // 🔍 بحث عن فاتورة قديمة للمشتريات
                    var provider = new GenericSearchProvider(SearchEntityType.PurchaseInvoices);
                    var result = SearchHelper.ShowSearchDialog(provider);
                    return result.Code;
                }
            }
            else
            {
                // 🟢 باقي الأنواع → بحث عن صنف
                var provider = new GenericSearchProvider(SearchEntityType.Products);
                var result = SearchHelper.ShowSearchDialog(provider);
                return result.Code;
            }

            return string.Empty;
        }

        //// 🔹 تحميل بيانات منتج حسب كوده.
        private bool isGardDone = false;
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
            PriceMove = (currentInvoiceType == InvoiceType.Sale ||
                               currentInvoiceType == InvoiceType.SaleReturn)
                ? Convert.ToSingle(row["U_Price"])
                : Convert.ToSingle(row["B_Price"]);

            lblPriceMove.Text = "سعر : " + PriceMove.ToString("0.00"); // عرض السعر برقمين عشريين


            // ✅ البيانات العامة
            ID_Prod = Convert.ToInt32(row["ID_Product"]);
            lblProductName.Text = row["ProdName"].ToString();
            unit_ID = Convert.ToInt32(row["UnitID"]);
            unit = (row["UnitProd"]?.ToString() ?? "").Trim();
            lblProductStock.Text = row["ProductStock"].ToString();
            lblUnit.Text = unit;
            // الطول الأدنى (للمنتجات القابلة للقص)
            lblMinLinth.Text = unit_ID == 1 ? row["MinLenth"].ToString() : "";
            lblLinthText.Text = unit_ID == 1 ? "اقل طول" : unit;
            isGardDone = row["IsGardDone"] != DBNull.Value && Convert.ToBoolean(row["IsGardDone"]);
            isCanCut = (unit_ID == 1);
            cbxPiece_ID.Visible = (currentInvoiceType == InvoiceType.Sale && isCanCut);

            return true;
        }

        #endregion

        #endregion



        //*****************************
        #region ======== تحميل بيانات القطعة حسب نوع الفاتورة ========

        private void LoadPieceData()
        {
            // الحالات اللي يظهر فيها الكومبو ويُفلتر بالأطوال
            bool showCombo =
                (currentInvoiceType == InvoiceType.Sale && isCanCut) ||
                (currentInvoiceType == InvoiceType.PurchaseReturn && isCanCut) || // ✅ أضفنا مرتجع الشراء هنا
                (currentInvoiceType == InvoiceType.Inventory) ||
                (currentInvoiceType == InvoiceType.AddStock) ||
                (currentInvoiceType == InvoiceType.DeductStock);

            // حالات الشراء والبيع المرتد => إدراج قطعة جديدة
            bool forceNewPiece =
                (currentInvoiceType == InvoiceType.SaleReturn || currentInvoiceType == InvoiceType.Purchase);

            if (unit_ID == 1) // المنتج يقبل القص
            {
                if (forceNewPiece)
                {
                    // 📌 إدراج قطعة جديدة بغض النظر عن الموجود
                    DataTable piece = DBServiecs.Product_InsertNewPiece(ID_Prod);
                    if (piece.Rows.Count > 0)
                        lblPieceID.Text = piece.Rows[0]["Piece_ID"].ToString();

                    cbxPiece_ID.Visible = false;
                    txtAmount.Visible = true;
                    txtAmount.Focus();
                }
                else
                {
                    // 📌 جلب القطع الموجودة
                    tblProdPieces = DBServiecs.Product_GetOrCreatePieces(ID_Prod);

                    // فلترة الأطوال الأكبر من الصفر
                    DataRow[] filtered = tblProdPieces.Select("Piece_Length <> 0");

                    if (filtered.Length > 0)
                    {
                        cbxPiece_ID.DataSource = filtered.CopyToDataTable();
                        cbxPiece_ID.DisplayMember = "Piece_Length";
                        cbxPiece_ID.ValueMember = "Piece_ID";

                        cbxPiece_ID.Visible = showCombo;

                        if (cbxPiece_ID.Visible)
                        {
                            cbxPiece_ID.DroppedDown = true;
                            cbxPiece_ID.SelectedIndex = 0;   // ✅ تحديد أول عنصر
                            cbxPiece_ID.Focus();

                            UpdatePieceLabels(); // ✅ تحديث الليبلات مباشرة
                        }
                        else
                        {
                            txtAmount.Visible = true;
                            txtAmount.Focus();
                        }
                    }
                    else
                    {
                        cbxPiece_ID.DataSource = null;
                        MessageBox.Show("لا توجد أرصدة بهذا الصنف.");
                        txtSeaarchProd.Focus();
                        txtSeaarchProd.Text = "";
                        cbxPiece_ID.Visible = false;
                        EmptyProdData();
                    }
                }
            }
            else // المنتج لا يقبل القص
            {
                DataTable piece = DBServiecs.Product_GetOrCreate_DefaultPiece(ID_Prod);
                if (piece.Rows.Count > 0)
                    lblPieceID.Text = piece.Rows[0]["Piece_ID"].ToString();

                cbxPiece_ID.Visible = false;
                txtAmount.Visible = true;
                txtAmount.Focus();
            }
        }

        private void UpdatePieceLabels()
        {
            if (cbxPiece_ID.SelectedValue != null && cbxPiece_ID.SelectedValue is not DataRowView)
            {
                // الرقم المختار من ValueMember
                lblPieceID.Text = cbxPiece_ID.SelectedValue.ToString();

                // النص المعروض من DisplayMember
                lblPiece_Length.Text = cbxPiece_ID.Text;
            }
        }

        // فتح فاتورة مرتجعة حسب رقمها.
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



        // إعداد منتج لعملية بيع.
        private void PrepareSaleProduct(string code)
        {
            if (!GetProd(code)) return;
            LoadPieceData();
        }


        // تحميل الأصناف المرتجعة إلى الجدول.
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

            // 🔹 الحذف عند الضغط على Delete
            if (e.KeyCode == Keys.Delete)
            {
                e.SuppressKeyPress = true;

                if (DGV.CurrentRow == null) return;

                // تأكد من أن العمود موجود
                if (!DGV.Columns.Contains("serInvDetail"))
                {
                    MessageBox.Show("لم يتم العثور على العمود serInvDetail.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int? serInv_detail = DGV.CurrentRow.Cells["serInvDetail"].Value as int?;
                if (serInv_detail == null)
                {
                    MessageBox.Show("لا يمكن تحديد السطر المراد حذفه.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // تأكيد من المستخدم
                DialogResult confirm = MessageBox.Show(
                    "هل أنت متأكد أنك تريد حذف هذا السطر؟",
                    "تأكيد الحذف",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        string message = DBServiecs.NewInvoice_DeleteDetailsRow(serInv_detail);
                        MessageBox.Show(message, "نتيجة العملية", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // إعادة تحميل البيانات بعد الحذف
                        GetInvoiceDetails();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("حدث خطأ أثناء الحذف:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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

        #region حساب المتغيرات وتفريغ البيانات

        // تحميل القيم من الحقول وتحويلها إلى متغيرات رقمية
        private void GetVar()
        {
            int.TryParse(lblInv_ID.Text, out Inv_ID);
            float.TryParse(txtAmount.Text, out Amount);
            float.TryParse(lblGemDisVal.Text, out GemDisVal);
            int.TryParse(lblPieceID.Text, out PieceID);

            // ✅ الحسابات الأساسية باستخدام PriceMove المخزن مسبقًا
            TotalRow = Amount * PriceMove;
            NetRow = TotalRow - GemDisVal;
        }


        // تفريغ بيانات المنتج بعد الإضافة أو الإلغاء
        private void EmptyProdData()
        {
            txtSeaarchProd.Text = string.Empty;
            txtSeaarchProd.Focus();
            txtSeaarchProd.SelectAll();

            lblPriceMove.Text = string.Empty;
            lblProductName.Text = string.Empty;
            cbxPiece_ID.SelectedIndex = -1;
            txtAmount.Visible = false;
            txtAmount.Text = string.Empty;
            lblGemDisVal.Text = string.Empty;
            lblUnit.Text = string.Empty;
            Piece_id = 0;
        }

        #endregion

        #region التنقل بين الفواتير



        // الانتقال إلى أول فاتورة
        private void btnFrist_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded())
                NavigateToInvoice(0);
        }

        // الانتقال إلى الفاتورة التالية
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded() && currentInvoiceIndex < tblInv.Rows.Count - 1)
                NavigateToInvoice(currentInvoiceIndex + 1);
            else
                MessageBox.Show("تم الوصول إلى آخر فاتورة.");
        }

        // الانتقال إلى الفاتورة السابقة
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded() && currentInvoiceIndex > 0)
                NavigateToInvoice(currentInvoiceIndex - 1);
            else
                MessageBox.Show("تم الوصول إلى أول فاتورة.");
        }

        // الانتقال إلى آخر فاتورة
        private void btnLast_Click(object sender, EventArgs e)
        {
            if (EnsureInvoicesLoaded())
                NavigateToInvoice(tblInv.Rows.Count - 1);
        }

        // وظيفة التنقل بين الفواتير
        private void NavigateToInvoice(int targetIndex)
        {
            if (!EnsureInvoicesLoaded()) return;
            EmptyProdData();
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

        // تفعيل/تعطيل أزرار التنقل حسب موقع الفاتورة
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
                GetInvoices();

            if (tblInv == null || tblInv.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد فواتير.");
                return false;
            }

            return true;
        }

        // عرض بيانات الفاتورة الحالية في الواجهة
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

            // 🔹 التاريخ
            if (row["Inv_Date"] != DBNull.Value)
                dtpInv_Date.Value = Convert.ToDateTime(row["Inv_Date"]);

            // 🔹 البائع / منفذ العملية
            cbxSellerID.SelectedValue = row["Seller_ID"] != DBNull.Value
                ? Convert.ToInt32(row["Seller_ID"])
                : -1;
            /*هو يدخل الحدث عندما يصل الى هنا lblAccID وهو محمل برقم وايضا الجدول غير فارغ */
            // 🔹 المستخدم والحساب
            lblAccID.Text = row["Acc_ID"].ToString();
            txtAccName.Text = row["AccName"].ToString();

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

            // 🔥 إعادة حساب الإجماليات والتفقيط
            CalculateInvoiceFooter();
        }

        // فتح فاتورة جديدة
        private void btnNew_Click(object sender, EventArgs e)
        {
            //     SetDefaultAccount();

            if (tblInv == null)
                GetInvoices();

            // الحصول على الأرقام الجديدة من قاعدة البيانات
            string nextCounter = DBServiecs.NewInvoice_GetNewCounter((int)currentInvoiceType);
            int nextID = DBServiecs.NewInvoice_GetNewID();

            // تعيين البيانات المبدئية
            lblInv_Counter.Text = nextCounter;
            lblInv_ID.Text = nextID.ToString();

            DisplayNewRow((int)currentInvoiceType, CurrentSession.UserID);
            ToggleControlsBasedOnSaveStatus();
        }

        // تجهيز واجهة فاتورة جديدة بقيم افتراضية
        public void DisplayNewRow(int invType, int userId)
        {
            dtpInv_Date.Value = DateTime.Now;
            cbxSellerID.SelectedValue = 26; // 🔹 قيمة افتراضية مؤقتة

            // 🔹 الحساب الافتراضي حسب نوع الفاتورة
            lblAccID.Text = invType switch
            {
                1 or 2 => "55", // بيع أو مرتجع بيع
                3 or 4 => "56", // شراء أو مرتجع شراء
                5 => "72",
                6 => "73",
                _ => "74"
            };

            // 🔹 ضبط اسم الحساب بناءً على رقم الحساب
            if (lblAccID.Text == "55")
                txtAccName.Text = "عميل نقدى";
            else if (lblAccID.Text == "56")
                txtAccName.Text = "مورد عام نقدى";
            else
                txtAccName.Text = string.Empty; // أو أي نص افتراضي


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




        #region التحكم في تفعيل وتعطيل عناصر النموذج

        // تعطيل أو تمكين عناصر النموذج بناءً على حالة الحفظ النهائي.
        private void ToggleControlsBasedOnSaveStatus()
        {
            bool isFinalSaved = !string.IsNullOrWhiteSpace(lblSave.Text);
            ToggleControlsRecursive(this.Controls, isFinalSaved);
            DGVStyl(); // إعادة تهيئة شكل الجدول
        }

        // تطبيق التمكين/التعطيل بشكل متكرر على جميع عناصر التحكم.
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



        #region تهيئة وتصميم DataGridView

        // إنشاء أعمدة الجدول يدوياً عند عدم وجود بيانات
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

        // إضافة عمود نصي مرئي
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

        // إضافة عمود مخفي
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




        //*******************************************
        #region Header   وظائف الجزء الاعلى من الفاتورة

        // ✅ تحميل القيم من ملف الإعدادات
        private void LoadSalesPolicies()
        {
            allowNegativeStock = AppSettings.GetBool("IsSaleByNegativeStock");
            returnSaleMode = AppSettings.GetInt("ReturnSaleMode");
            returnPurchaseMode = AppSettings.GetInt("ReturnPurchasesMode");
        }

        // 🔹 تحديث النصوص لو اخترت "بيع"
        private void UpdateLabelsForSale()
        {
            if (allowNegativeStock)
                lblInvStat.Text = "البيع على مكشوف";
            else
                lblInvStat.Text = "البيع حسب الرصيد";

            lblCodeTitel.Text = "ادخل كود الصنف";
        }

        // 🔹 تحديث النصوص لو اخترت "مرتجع"
        private void UpdateLabelsForReturn()
        {
            int mode = currentInvoiceType == InvoiceType.SaleReturn
                ? returnSaleMode
                : returnPurchaseMode;

            switch (mode)
            {
                case 1: // InvoiceOnly
                    lblCodeTitel.Text = currentInvoiceType == InvoiceType.SaleReturn
                        ? "رقم فاتورة البيع"
                        : "رقم فاتورة الشراء";

                    lblInvStat.Text = currentInvoiceType == InvoiceType.SaleReturn
                        ? "البيع المرتد يكون عن طريق رقم فاتورة البيع الأصلية"
                        : "الشراء المرتد يكون عن طريق رقم فاتورة الشراء الأصلية";

                    tlpReturnMod.Visible = false;
                    rdoInvoice.Checked = true;
                    break;

                case 2: // FreeMode
                    lblCodeTitel.Text = "رقم كود الصنف";
                    lblInvStat.Text = "إرجاع بالكود";
                    tlpReturnMod.Visible = false;
                    rdoFree.Checked = true;
                    break;

                case 3: // MixedMode
                    lblCodeTitel.Text = "رقم كود الصنف";
                    lblInvStat.Text = "إرجاع بالكود";
                    tlpReturnMod.Visible = true;
                    rdoFree.Checked = true;
                    break;

                default: // fallback
                    lblCodeTitel.Text = "رقم كود الصنف";
                    lblInvStat.Text = "إرجاع حر";
                    tlpReturnMod.Visible = false;
                    rdoFree.Checked = true;
                    break;
            }
        }

        // 🔹 تحديث النصوص لو اخترت "مشريات"
        private void UpdateLabelsForPurchase()
        {

        }

        // 🔹 تحديث النصوص لو اخترت "جرد"
        private void UpdateLabelsForInventory()
        {

        }

        // 🔹 تحديث النصوص لو اخترت "خصم رصيد"
        private void UpdateLabelsForDeduct()
        {

        }

        // 🔹 تحديث النصوص لو اخترت "اضافة رصيد"
        private void UpdateLabelsForAddStock()
        {

        }


        // 🔹 تغيير النصوص لما أختار راديو rdoFree
        private void rdoFree_CheckedChanged(object sender, EventArgs e)
        {
            if (!rdoFree.Checked) return;

            lblCodeTitel.Text = "رقم كود الصنف";
            lblInvStat.Text = currentInvoiceType == InvoiceType.SaleReturn
                ? "البيع المرتد بالكود"
                : "الشراء المرتد بالكود";
            EmptyProdData();
        }

        // 🔹 تغيير النصوص لما أختار راديو rdoInvoice
        private void rdoInvoice_CheckedChanged(object sender, EventArgs e)
        {
            if (!rdoInvoice.Checked) return;

            lblCodeTitel.Text = currentInvoiceType == InvoiceType.SaleReturn
                ? "رقم فاتورة البيع"
                : "رقم فاتورة الشراء";

            lblInvStat.Text = currentInvoiceType == InvoiceType.SaleReturn
                ? "البيع المرتد يكون عن طريق رقم فاتورة البيع الأصلية"
                : "الشراء المرتد يكون عن طريق رقم فاتورة الشراء الأصلية";

            EmptyProdData();
        }



        #region AutoComplete Configuration
        private void ConfigureAutoCompleteForAccount()
        {
            txtAccName.AutoCompleteCustomSource.Clear();
            string invoiceTypeKey = InvoiceTypeHelper.ToAccountTypeString(currentInvoiceType);

            if (!string.IsNullOrEmpty(invoiceTypeKey))
            {
                DataTable dt = DBServiecs.NewInvoice_GetAcc(invoiceTypeKey);
                var names = dt.AsEnumerable()
                              .Select(r => r.Field<string?>("AccName") ?? string.Empty)
                              .ToArray();

                txtAccName.AutoCompleteCustomSource.AddRange(names);
            }

            txtAccName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtAccName.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }
        #endregion

        #region Seller ComboBox
        private void cbxSellerID_Enter(object sender, EventArgs e)
        {
            cbxSellerID.DroppedDown = true;
        }

        private void FillSellerComboBox()
        {
            string sellerKey = InvoiceTypeHelper.ToAccountTypeString(currentInvoiceType, forSeller: true);

            if (string.IsNullOrEmpty(sellerKey))
            {
                cbxSellerID.DataSource = null;
                return;
            }

            DataTable dt = DBServiecs.NewInvoice_GetAcc(sellerKey);

            cbxSellerID.DataSource = dt;
            cbxSellerID.DisplayMember = "AccName";
            cbxSellerID.ValueMember = "AccID";

            // 🔥 حدد الحساب الافتراضي حسب نوع الفاتورة
            int defaultAccID = currentInvoiceType switch
            {
                InvoiceType.Sale or InvoiceType.SaleReturn => 57,  // حساب إدارة البائعين
                not (InvoiceType.Sale or InvoiceType.SaleReturn) => 61, // أي نوع آخر يروح لحساب إدارة المشتريات
            };


            // 🔍 البحث عن الصف الذي يحتوي على الحساب الافتراضي
            if (defaultAccID != -1)
            {
                DataRow[] rows = dt.Select($"AccID = {defaultAccID}");
                if (rows.Length > 0)
                {
                    cbxSellerID.SelectedValue = defaultAccID;
                }
                else if (dt.Rows.Count > 0)
                {
                    cbxSellerID.SelectedIndex = 0; // fallback لو الحساب الافتراضي غير موجود
                }
            }
            else if (dt.Rows.Count > 0)
            {
                cbxSellerID.SelectedIndex = 0;

            }


            // 🔒 منع الكتابة في الكومبوبوكس
            cbxSellerID.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        #endregion


        #region Form Setup by Invoice Type
        #endregion

        #region تحديث بيانات الحساب عند تغيير رقم الحساب


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
            lblClientAddress.Text = accountData["ClientAddress"].ToString();
            lblClientEmail.Text = accountData["ClientEmail"].ToString();
        }
        #endregion

        #region Account Data Display
        
        private void txtAccName_KeyDown(object sender, KeyEventArgs e)
        {
            // 1️⃣ Ctrl + F → فتح شاشة البحث
            if (e.Control && e.KeyCode == Keys.F)
            {
                if (currentInvoiceType != InvoiceType.Sale &&
                    currentInvoiceType != InvoiceType.SaleReturn &&
                    currentInvoiceType != InvoiceType.Purchase &&
                    currentInvoiceType != InvoiceType.PurchaseReturn)
                    return;

                AccountKind accountKind = (currentInvoiceType == InvoiceType.Purchase ||
                                           currentInvoiceType == InvoiceType.PurchaseReturn)
                                           ? AccountKind.Suppliers
                                           : AccountKind.Customers;

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

            // 2️⃣ Shift + Enter أو سهم ↑ → الانتقال للحقل السابق
            if ((e.KeyCode == Keys.Enter && e.Shift) || e.KeyCode == Keys.Up)
            {
                this.SelectNextControl((Control)sender, false, true, true, true);
                e.Handled = true;
                return;
            }

            // 3️⃣ Enter فقط → تحقق من الحساب ثم انتقل للحقل التالي
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                string input = txtAccName.Text.Trim();

                if (string.IsNullOrWhiteSpace(input) || tblAcc == null)
                {
                    SetDefaultAccount();
                }
                else
                {
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
                }

                // بعد كل ذلك، انتقل للحقل التالي
                cbxSellerID.Focus();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        // فتح نموذج إضافة حساب جديد وربطه بالفاتورة
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

        // تعيين الحساب الافتراضي
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

        

        private void txtAccName_Leave(object sender, EventArgs e)
        {
            string input = txtAccName.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                ClearAccountDetails();
                return;
            }

            string invoiceTypeKey = InvoiceTypeHelper.ToAccountTypeString(currentInvoiceType);
            if (string.IsNullOrEmpty(invoiceTypeKey))
            {
                ClearAccountDetails();
                return;
            }

            DataTable dt = DBServiecs.NewInvoice_GetAcc(invoiceTypeKey);

            DataRow? selectedAccount = null;

            // 🔹 تحقق: هل المدخل رقم هاتف؟
            bool isPhoneNumber = input.All(c => char.IsDigit(c) || c == '+' || c == '-');

            if (isPhoneNumber)
            {
                // 🔹 البحث بالهاتف
                selectedAccount = dt.AsEnumerable()
                    .FirstOrDefault(row =>
                        string.Equals(row.Field<string?>("Phone"), input, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(row.Field<string?>("Mobile"), input, StringComparison.OrdinalIgnoreCase));

                if (selectedAccount == null)
                {
                    // ❗ الرقم غير مسجل
                    DialogResult result = MessageBox.Show(
                        "⚠️ هذا الرقم غير مسجل، هل تريد إضافته إلى البيانات؟",
                        "إضافة حساب جديد",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        int type = (int)currentInvoiceType;  // 🔥 يحوّل Enum لرقم حقيقي
                        frm_AddAccount frm = new frm_AddAccount(input, type);
                        frm.ShowDialog();
                    }
                    else
                    {
                        LoadDefaultAccount();
                    }

                    return;
                }

                txtAccName.Text = selectedAccount["AccName"]?.ToString() ?? string.Empty;
            }
            else
            {
                // 🔹 البحث بالاسم
                selectedAccount = dt.AsEnumerable()
                    .FirstOrDefault(row =>
                        string.Equals(row.Field<string?>("AccName"), input, StringComparison.OrdinalIgnoreCase));

                if (selectedAccount == null)
                {
                    // ❗ الاسم غير مسجل
                    DialogResult result = MessageBox.Show(
                        "⚠️ هذا الاسم غير مسجل، هل تريد إضافته إلى البيانات؟",
                        "إضافة حساب جديد",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        int type = (currentInvoiceType == InvoiceType.Sale || currentInvoiceType == InvoiceType.SaleReturn) ? 1 : 2;
                        frm_AddAccount frm = new frm_AddAccount(input, type);
                        frm.ShowDialog();
                    }
                    else
                    {
                        LoadDefaultAccount();
                    }

                    return;
                }
            }

            if (selectedAccount != null)
            {
                lblAccID.Text = selectedAccount["AccID"]?.ToString() ?? "0";
                DisplayAccountDetails(selectedAccount);
            }
            else
            {
                ClearAccountDetails();
            }
        }

        // 🔹 دالة تحميل الحساب الافتراضي
        private void LoadDefaultAccount()
        {
            string invoiceTypeKey = InvoiceTypeHelper.ToAccountTypeString(currentInvoiceType);
            if (string.IsNullOrEmpty(invoiceTypeKey)) return;

            DataTable dt = DBServiecs.NewInvoice_GetAcc(invoiceTypeKey);

            if (dt.Rows.Count > 0)
            {
                int defaultId = (currentInvoiceType == InvoiceType.Sale || currentInvoiceType == InvoiceType.SaleReturn) ? 55 : 56;
                DataRow? defaultAccount = dt.AsEnumerable()
                    .FirstOrDefault(row => row.Field<int>("AccID") == defaultId);

                if (defaultAccount != null)
                {
                    lblAccID.Text = defaultAccount["AccID"].ToString();
                    txtAccName.Text = defaultAccount["AccName"].ToString();
                    DisplayAccountDetails(defaultAccount);
                }
            }
        }

        private void DisplayAccountDetails(DataRow accountRow)
        {
            // 🔹 الهاتفين
            string? firstPhone = accountRow.Field<string?>("Phone");
            string? anotherPhone = accountRow.Field<string?>("Mobile");

            if (!string.IsNullOrWhiteSpace(firstPhone) && !string.IsNullOrWhiteSpace(anotherPhone))
            {
                lblFirstPhon.Text = $"هواتف: {firstPhone} - {anotherPhone}";
            }
            else if (!string.IsNullOrWhiteSpace(firstPhone))
            {
                lblFirstPhon.Text = $"هاتف: {firstPhone}";
            }
            else if (!string.IsNullOrWhiteSpace(anotherPhone))
            {
                lblFirstPhon.Text = $"هاتف: {anotherPhone}";
            }
            else
            {
                lblFirstPhon.Text = string.Empty;
            }

            // 🔹 البريد الإلكتروني
            string? email = accountRow.Field<string?>("Email");
            lblClientEmail.Text = !string.IsNullOrWhiteSpace(email)
                ? $"Email: {email}"
                : string.Empty;

            // 🔹 العنوان
            string? address = accountRow.Field<string?>("Address");
            lblClientAddress.Text = !string.IsNullOrWhiteSpace(address)
                ? $"العنوان: {address}"
                : string.Empty;

            // 🔹 الرصيد (حل مشكلة التحويل)
            decimal balance = 0;
            if (accountRow["Balance"] != DBNull.Value)
            {
                balance = Convert.ToDecimal(accountRow["Balance"]);
            }

            if (balance == 0)
            {
                lblB_Status.Text = "الرصيد: متوازن";
            }
            else if (balance > 0)
            {
                lblB_Status.Text = $"الرصيد: دائن بـ {balance:N2}";
            }
            else
            {
                lblB_Status.Text = $"الرصيد: مدين بـ {Math.Abs(balance):N2}";
            }
        }

        private void ClearAccountDetails()
        {
            lblAccID.Text = "0";
            lblFirstPhon.Text = string.Empty;
            lblClientEmail.Text = string.Empty;
            lblClientAddress.Text = string.Empty;
        }
        #endregion

        #endregion End Heder

        //*******************************************
        #region Foter وظائف اجماليات الفاتورة
        private decimal defaultTax = 0m; // 🟦 نخزن النسبة هنا لاستخدامها لاحقاً

        // 🔹 تحميل الإعدادات
        private void LoadFooterSettings()
        {
            try
            {
                // 🟦 قراءة القيم من ملف الإعدادات
                defaultTax = AppSettings.GetDecimal("SalesTax", 0m);
                AllowChangeTax = AppSettings.GetBool("IsEnablToChangTax", true);
                MaxRateDiscount = AppSettings.GetDecimal("MaxRateDiscount", 0.10m); // 10% افتراضياً

                // 🟦 عند التحميل نخلي الحقول فاضية
                lblTaxRate.Text = "";
                txtTaxVal.Text = "0.00";

                // 🟦 لا نغير خاصية ReadOnly
                txtTaxVal.ReadOnly = false;
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowWarning($"خطأ أثناء تحميل إعدادات الفاتورة:\n{ex.Message}", "خطأ");
            }
        }

        // 🔹 تحديث نسبة الضريبة
        private void txtTaxVal_Leave(object sender, EventArgs e)
        {
            if (!decimal.TryParse(lblTotalInv.Text, out decimal total))
                total = 0m;

            if (!AllowChangeTax)
            {
                // 🟦 إعادة حساب الضريبة من جديد من القيمة الافتراضية
                decimal taxValue = (total > 0 && defaultTax > 0) ? total * defaultTax : 0m;
                txtTaxVal.Text = taxValue.ToString("N2");
                lblTaxRate.Text = total > 0 ? (defaultTax * 100m).ToString("N0") + "%" : "0%";
            }
            else
            {
                // 🟦 حساب نسبة الضريبة بناءً على القيمة المدخلة
                if (!decimal.TryParse(txtTaxVal.Text, out decimal tax))
                    tax = 0m;

                lblTaxRate.Text = total > 0
                    ? ((tax / total) * 100m).ToString("N0") + "%"
                    : "0%";
            }

            // 🟦 في كل الحالات نحسب الفوتر من جديد
            CalculateInvoiceFooter();
        }

        // 🔹 حساب الصريبة
        private void txtTaxVal_DoubleClick(object sender, EventArgs e)
        {
            // 🟦 منع التعديل إذا كانت الفاتورة محفوظة
            if (IsInvoiceSaved())
            {
                return;
            }

            // 🟦 إذا مافيش نسبة ضريبة في الإعدادات
            if (defaultTax <= 0)
            {
                MessageBox.Show("لا توجد نسبة ضريبة محددة في الإعدادات.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 🟦 نقرأ الإجمالي
            if (!decimal.TryParse(lblTotalInv.Text, out decimal total) || total <= 0)
            {
                MessageBox.Show("يجب إدخال إجمالي الفاتورة أولاً.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 🟦 عرض النسبة
            lblTaxRate.Text = (defaultTax * 100m).ToString("N0") + "%";

            // 🟦 حساب القيمة
            decimal taxValue = total * defaultTax;
            txtTaxVal.Text = taxValue.ToString("N2");

            // 🟦 تحديث الفوتر
            CalculateInvoiceFooter();
        }

        // 🔹 الحد من الخصم
        private void txtDiscount_Leave(object? sender, EventArgs e)
        {
            if (!decimal.TryParse(txtDiscount.Text, out var discount)) discount = 0m;
            if (!decimal.TryParse(lblTotalInv.Text, out var total)) total = 0m;

            var maxDiscount = total * MaxRateDiscount;
            if (discount > maxDiscount)
            {
                discount = maxDiscount;
                txtDiscount.Text = discount.ToString("N2");
                CustomMessageBox.ShowWarning(
                    $"⚠️ الخصم لا يمكن أن يتجاوز {MaxRateDiscount:P0} من إجمالي الفاتورة.",
                    "تنبيه");
            }

            lblDiscountRate.Text = total > 0
            ? ((discount / total) * 100m).ToString("N0") + "%"
            : "0%";

            CalculateInvoiceFooter();
        }

        // 🔹 تحديث نسبة الإضافة
        private void txtValueAdded_Leave(object? sender, EventArgs e)
        {
            if (!decimal.TryParse(txtValueAdded.Text, out var added)) added = 0m;
            if (!decimal.TryParse(lblTotalInv.Text, out var total)) total = 0m;

            lblAdditionalRate.Text = total > 0
              ? ((added / total) * 100m).ToString("N0") + "%"
              : "0%";

            CalculateInvoiceFooter();
        }

        // 🔹 تحديث المدفوعات
        private void txtPayment_Cash_Leave(object? sender, EventArgs e)
        {
            CalculateRemainingOnAccount();
            UpdatePaymentNote();
        }

        private void txtPayment_Electronic_Leave(object? sender, EventArgs e)
        {
            CalculateRemainingOnAccount();
            UpdatePaymentNote();
        }
        // 🔹 دعم النقر المزدوج للمدفوعات
        private void txtPayment_Cash_DoubleClick(object? sender, EventArgs e)
        {
            // 🟦 منع التعديل إذا كانت الفاتورة محفوظة
            if (IsInvoiceSaved())
            {
                return;
            }

            decimal.TryParse(txtPayment_Electronic.Text, out var electronic);
            decimal.TryParse(lblNetTotal.Text, out var net);

            if (electronic == 0)
            {
                txtPayment_Cash.Text = net.ToString("N2");
                txtPayment_Electronic.Text = "0";
            }
            else
            {
                txtPayment_Cash.SelectAll(); // تحديد النص لكتابة يدويّة
            }

            UpdatePaymentNote();
            CalculateRemainingOnAccount();
        }

        private void txtPayment_Electronic_DoubleClick(object? sender, EventArgs e)
        {
            // 🟦 منع التعديل إذا كانت الفاتورة محفوظة
            if (IsInvoiceSaved())
            {
                return;
            }

            decimal.TryParse(txtPayment_Cash.Text, out var cash);
            decimal.TryParse(lblNetTotal.Text, out var net);

            if (cash == 0)
            {
                txtPayment_Electronic.Text = net.ToString("N2");
                txtPayment_Cash.Text = "0";
            }
            else
            {
                txtPayment_Electronic.SelectAll(); // تحديد النص لكتابة يدويّة
            }

            UpdatePaymentNote();
            CalculateRemainingOnAccount();
        }

        // 🔹 دالة لتحديث طريقة الدفع
        private void UpdatePaymentNote()
        {
            decimal.TryParse(txtPayment_Cash.Text, out var cash);
            decimal.TryParse(txtPayment_Electronic.Text, out var electronic);

            if (cash > 0 && electronic > 0)
                txtPayment_Note.Text = "مدفوع نقدي وبالفيزا معا";
            else if (cash > 0)
                txtPayment_Note.Text = "مدفوع نقدي";
            else if (electronic > 0)
                txtPayment_Note.Text = "مدفوع بالفيزا";
            else
                txtPayment_Note.Clear(); // لا يوجد مدفوع
        }


        // 🔹 حساب الرصيد المتبقي
        private void CalculateRemainingOnAccount()
        {
            decimal.TryParse(lblNetTotal.Text, out decimal net);
            decimal.TryParse(txtPayment_Cash.Text, out decimal cash);
            decimal.TryParse(txtPayment_Electronic.Text, out decimal visa);

            var remaining = net - (cash + visa);
            lblRemainingOnAcc.Text = remaining.ToString("N2");

            if (remaining > 0)
            {
                lblStateRemaining.Text = $"باقي عليه {remaining:N2}";
                lblStateRemaining.ForeColor = Color.Red;
                lblRemainingOnAcc.Visible = false;
            }
            else if (remaining < 0)
            {
                lblStateRemaining.Text = $"باقي له {Math.Abs(remaining):N2}";
                lblStateRemaining.ForeColor = Color.Green;
                lblRemainingOnAcc.Visible = false;
            }
            else
            {
                lblStateRemaining.Text = "تم السداد";
                lblStateRemaining.ForeColor = Color.Blue;
                lblRemainingOnAcc.Visible = false;
            }

        }

        // 🔹 حساب جميع القيم
        private void CalculateInvoiceFooter()
        {
            // 🔹 مجموع الفاتورة من الجريد (مؤقتاً ثابت للتجربة)
            decimal total = 0;
            if (DGV.DataSource is not DataTable dt) return;
            total = dt.AsEnumerable()
                     .Where(r => r["NetRow"] != DBNull.Value)
                     .Sum(r => Convert.ToDecimal(r["NetRow"]));
            lblTotalInv.Text = total.ToString("N2");

            // 🔹 قراءة القيم
            decimal.TryParse(txtTaxVal.Text, out var tax);
            decimal.TryParse(txtDiscount.Text, out var discount);
            decimal.TryParse(txtValueAdded.Text, out var added);

            // 🔹 حساب الإجمالي بعد الضريبة فقط
            var totalAfterTax = total + tax;
            lblTotalValueAfterTax.Text = totalAfterTax.ToString("N2");

            // 🔹 حساب الصافي النهائي
            var net = total + tax - discount + added;
            lblNetTotal.Text = net.ToString("N2");
            decimal value = net;
            string result = TafqeetHelper.Tafqeet(value);
            lblTafqet.Text = result;


            // 🔹 تحديث المتبقي
            CalculateRemainingOnAccount();
        }

        #endregion

        //*******************************************
        #region ********** الحفظ النهائى للفاتورة 🔹 العمليات العامة للحفظ
        // استعراض قيد اليومية المرتبط بالفاتورة بعد الحفظ
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
                user_ID: CurrentSession.UserID,
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

        // دالة مساعدة لتحويل النص إلى رقم عائم
        private static float ToFloat(object? value, float defaultVal = 0) =>
            float.TryParse(value?.ToString(), out float result) ? result : defaultVal;

        #endregion

        #region  تحميلات افتتاحية  لرأس الفاتورة 
        //عنوان الفاتورة - رقم النوع 1:7 - المسلسل - معرف الفاتورة - معلومات عنها - التاريخ
        //رقم واسم الحساب وبياناته المسجلة - االبائع او المشترى - سياسة البيع والمردودات
        //ادوات البحث للحصول على الحساب
        // 🔹 تعيين الحساب الافتراضى حسب الفاتورة
        private void FillDefaultAccount()
        {
            string invoiceTypeKey = InvoiceTypeHelper.ToAccountTypeString(currentInvoiceType);

            if (string.IsNullOrEmpty(invoiceTypeKey))
                return;

            DataTable dt = DBServiecs.NewInvoice_GetAcc(invoiceTypeKey);

            // 🔥 تحديد الحساب الافتراضي حسب نوع الفاتورة
            int defaultAccID = currentInvoiceType switch
            {
                InvoiceType.Sale or InvoiceType.SaleReturn => 14,   // عميل نقدي
                InvoiceType.Purchase or InvoiceType.PurchaseReturn => 28, // مورد نقدي
                InvoiceType.Inventory => 20,     // حساب جرد المخزون
                InvoiceType.DeductStock => 19,   // حساب خصم من المخزون
                InvoiceType.AddStock => 19,      // حساب إضافة إلى المخزون
                _ => -1
            };


            if (defaultAccID != -1)
            {
                // 🔍 البحث عن الحساب في الجدول
                DataRow[] rows = dt.Select($"AccID = {defaultAccID}");
                if (rows.Length > 0)
                {
                    lblAccID.Text = rows[0]["AccID"].ToString();
                    txtAccName.Text = rows[0]["AccName"].ToString();
                    return;
                }
            }

            // 📌 لو الحساب الافتراضي غير موجود نرجع لأول صف
            if (dt.Rows.Count > 0)
            {
                lblAccID.Text = dt.Rows[0]["AccID"].ToString();
                txtAccName.Text = dt.Rows[0]["AccName"].ToString();
            }
            else
            {
                lblAccID.Text = "0";
                txtAccName.Text = string.Empty;
            }
        }

        // تحميل بيانات الحساب عند تغيير قيمة lblAccID.
        private void lblAccID_TextChanged(object sender, EventArgs e)
        {
            string accountID = lblAccID.Text.Trim();
            /* اريد معرفة منطق هذا الكود وقد لاحظت انه دائما وفى كل الحالات يتجاوز الشرط ولا يدخله ابدا فما فائدته*/
            if (!string.IsNullOrEmpty(accountID) && tblAcc != null)
            {
                DataRow[] accountData = tblAcc.Select($"AccID = '{accountID}'");
                if (accountData.Length > 0)
                {
                    LoadDefaultAccount();
                    LoadAccountData(accountData[0]);
                }
                else
                {
                    CustomMessageBox.ShowWarning("لا يوجد حساب مرتبط برقم الحساب المحدد.", "خطأ");

                }
            }
        }

        private void SetupFormByInvoiceType()
        {
            switch (currentInvoiceType)
            {
                case InvoiceType.Inventory:
                case InvoiceType.DeductStock:
                case InvoiceType.AddStock:
                    // تعطيل أو تمكين بعض الحقول الخاصة بالمبيعات/المشتريات
                    txtAccName.Enabled = false;
                    break;

                default:
                    txtAccName.Enabled = true;
                    break;
            }

            // إعداد DataGridView أو أي عناصر أخرى
        }


        #endregion





        #region دوال مجمعة لضبط الادخالات 
        // 🔹 منع كتابة أي شيء غير الأرقام + التحكم في الفاصلة العشرية       ***
        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (unit_ID == 2)// فى حالة القطعة
            {
                // أرقام صحيحة فقط
                InputValidationHelper.AllowOnlyNumbers(sender, e);
            }
            else//فى حالة غير القطعة
            {
                // أرقام + فاصلة عشرية
                InputValidationHelper.AllowOnlyNumbersAndDecimal(sender, e);
            }
        }

        // دالة تربط صناديق نصوص بالأرقام + فاصلة عشرية
        private void AttachDecimalValidation(params TextBox[] textBoxes)
        {
            foreach (var txt in textBoxes)
            {
                txt.KeyPress += (s, e) => InputValidationHelper.AllowOnlyNumbersAndDecimal(s, e);
            }
        }

        // دالة تربط صناديق نصوص بالأرقام الصحيحة فقط
        private void AttachIntegerValidation(params TextBox[] textBoxes)
        {
            foreach (var txt in textBoxes)
            {
                txt.KeyPress += (s, e) => InputValidationHelper.AllowOnlyNumbers(s, e);
            }
        }

        private void txtTaxVal_KeyPress(object sender, KeyPressEventArgs e)
        {
            // أرقام + فاصلة عشرية
            InputValidationHelper.AllowOnlyNumbersAndDecimal(sender, e);
        }

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // أرقام + فاصلة عشرية
            InputValidationHelper.AllowOnlyNumbersAndDecimal(sender, e);
        }

        private void txtValueAdded_KeyPress(object sender, KeyPressEventArgs e)
        {
            // أرقام + فاصلة عشرية
            InputValidationHelper.AllowOnlyNumbersAndDecimal(sender, e);
        }

        private void txtPayment_Cash_KeyPress(object sender, KeyPressEventArgs e)
        {
            // أرقام + فاصلة عشرية
            InputValidationHelper.AllowOnlyNumbersAndDecimal(sender, e);
        }

        private void txtPayment_Electronic_KeyPress(object sender, KeyPressEventArgs e)
        {
            // أرقام + فاصلة عشرية
            InputValidationHelper.AllowOnlyNumbersAndDecimal(sender, e);
        }

        #endregion
    }
}


/* if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(txtSeaarchProd.Text))

 السيناريو المطلوب الوصول اليه فى الحالة 

   هذا التكست يتعامل مع الرقم الموجود بداخله فى عدة حالات 
   -----------------------------------------------------------------------------------------
   lblTypeInvID=1 اولا : اذا كانت الفاتورة مبيعات 
   -----------------------------------------------------------------------------------------
   1-يقوم بجلب بيانات الصنف المدرج كوده بالدالة GetProd(code);
   2-يقوم بجلب قطع هذا الصنف بالدالة  LoadPieceData();
   وهنا يوجد طريقان اما ان يكون وحدة الصنف 
        a:  if (unit_ID == 1) // المنتج يقبل القص 
             فيتم اظهار الكمبوبكس المحمل بالقطع الموجودة لهذا الصنف والانتقال اليها 
             وفتح القائمة للاختيار منها القطعة المراد القص منها للبيع

        b:  else المنتج غير قابل للقص فينتقل التركيز مباشرة الى تكست الكمية txtAmount بعد تحديد رقم القطعة الافتراضية له

   -----------------------------------------------------------------------------------------
   lblTypeInvID=2 ثانيا : اذا كانت الفاتورة مبيعات مرتدة
   -----------------------------------------------------------------------------------------
   وهنا التصرف مختلف قليلا  وله حالتان
   1-rdoInvoice.Checked = true; وتعنى ان الرقم الذى كتب يعبر عن رقم فاتورة المبيعات الاصلية
        فيذهب ويفتحها فى الشاشة frm_ReturnedInvoice
       ويمرر رقمان من خلال الدالة 
       a:الرقم الذى كتب وهو مسلسل للفاتورة الاصليةInv_Counter
           وبه يتم التوصل الى معرف فاتورة المبيعات المراد ارجاع الصنف منها Inv_ID
           فيتم فتحها على اساسه بتفاصيلها فى الفورم frm_ReturnedInvoice

       b: ويتم تمرير ايضا الرقم المعرف الحالى لفاتورة المبيعات المرتدة والمخزن فى lblInv_ID 
           والذى سيستخدم لاحقا فى انشاء فاتورة المبيعات المرتدة الحالية

   2-rdoFree.Checked = true; وهذه الحالة الثانية 
       يعنى ان الرقم الذى كتب هو كود صنف مراد ارجاعه بدون فاتورة اصلية للمبيعات

       فيتم جلب بياناته بواسطة الدالة GetProd(code); وعرضها 
       ثم يقوم بجلب قطع هذا الصنف بالدالة  LoadPieceData();
       وهنا توجد حالتان     
        a:  if (unit_ID == 1) // المنتج يقبل القص 
               فيتم ادراج قطعة جديدة فى جدول القطع ثم استدعاء رقمها لتحديث طولها من خلال txtAmount و
               والذى سيتم الانتقال اليه مباشرة ولا يتم اظهار الكمبوبكس

        b:  else المنتج غير قابل للقص فينتقل التركيز مباشرة الى تكست الكمية txtAmount بعد تحديد رقم القطعة الافتراضية له

   -----------------------------------------------------------------------------------------
   lblTypeInvID=3 ثالثا : اذا كانت الفاتورة مشتريات 
   -----------------------------------------------------------------------------------------
       يعنى ان الرقم الذى كتب هو كود صنف مراد شراء كمية منه
       فيتم جلب بياناته بواسطة الدالة GetProd(code); وعرضها 
       ثم يقوم بجلب قطع هذا الصنف بالدالة  LoadPieceData();
       وهنا توجد حالتان     
        a:  if (unit_ID == 1) // المنتج يقبل القص 
               فيتم ادراج قطعة جديدة فى جدول القطع ثم استدعاء رقمها لتحديث طولها من خلال txtAmount
               والذى سيتم الانتقال اليه مباشرة ولا يتم اظهار الكمبوبكس

        b:  else المنتج غير قابل للقص فينتقل التركيز مباشرة الى تكست الكمية txtAmount بعد تحديد رقم القطعة الافتراضية له

   -----------------------------------------------------------------------------------------
   lblTypeInvID=4 رابعا : اذا كانت الفاتورة مشتريات مرتدة 
   -----------------------------------------------------------------------------------------
   وهنا التصرف مختلف قليلا  وله حالتان
   1-rdoInvoice.Checked = true;هذه الحالة وتعنى ان الرقم الذى كتب يعبر عن رقم فاتورة المشتريات الاصلية
        فيذهب ويفتحها فى الشاشة frm_ReturnedInvoice
       ويمرر رقمان من خلال الدالة 
       a: الرقم الذى كتب وهو مسلسل للفاتورة الاصلية Inv_Counter
           وبه يتم التوصل الى معرف فاتورة المشتريات المراد ارجاع الصنف منها Inv_ID
           فيتم فتحها على اساسه بتفاصيلها فى الفورم frm_ReturnedInvoice

       b: ويتم تمرير ايضا الرقم المعرف الحالى لفاتورة المشتريات المرتدة والمخزن فى lblInv_ID 
           والذى سيستخدم لاحقا فى انشاء فاتورة المشتريات المرتدة الحالية

   2-rdoFree.Checked = true; وهذه الحالة الثانية 
       يعنى ان الرقم الذى كتب هو كود صنف مراد ارجاعه بدون فاتورة اصلية للمشتريات
       فيتم جلب بياناته بواسطة الدالة GetProd(code); وعرضها 
       ثم يقوم بجلب قطع هذا الصنف بالدالة  LoadPieceData();
       وهنا توجد حالتان     
        a:  if (unit_ID == 1) // المنتج يقبل القص 
             فيتم اظهار الكمبوبكس المحمل بالقطع الموجودة لهذا الصنف والانتقال اليها 
             وفتح القائمة للاختيار منها القطعة المراد الارجاع منها للمورد

        b:  else المنتج غير قابل للقص فينتقل التركيز مباشرة الى تكست الكمية txtAmount بعد تحديد رقم القطعة الافتراضية له


   -----------------------------------------------------------------------------------------
   lblTypeInvID =5 خامسا : اذن تصحيح مخزون صنف عن طريق  جرده وتسجيل كميته الفعلية 
   -----------------------------------------------------------------------------------------
       يعنى ان الرقم الذى كتب هو كود صنف مراد جرد كميته ان كان غير قابل للقص او ضبط طوله ان كان قابل للقص
       فيتم جلب بياناته بواسطة الدالة GetProd(code); وعرضها 
       ثم يقوم بجلب قطع هذا الصنف بالدالة  LoadPieceData();
       وهنا توجد حالتان     
        a:  if (unit_ID == 1) // المنتج يقبل القص 
             فيتم اظهار الكمبوبكس المحمل بالقطع الموجودة لهذا الصنف والانتقال اليها 
             وفتح القائمة للاختيار منها القطعة المراد تصحيح طولها بالنقص او الزيادة

        b:  else المنتج غير قابل للقص فينتقل التركيز مباشرة الى تكست الكمية txtAmount بعد تحديد رقم القطعة الافتراضية له

   -----------------------------------------------------------------------------------------
   lblTypeInvID =6 سادسا : اذن خصم مخزون صنف فى حالة الضياع او الهدر او فساده 
   -----------------------------------------------------------------------------------------
       يعنى ان الرقم الذى كتب هو كود صنف مراد  انقاص طوله ان كان قابل للقصكهدر جزئى من الطول الاصلى
       فيتم جلب بياناته بواسطة الدالة GetProd(code); وعرضها 
       ثم يقوم بجلب قطع هذا الصنف بالدالة  LoadPieceData();
       وهنا توجد حالتان     
        a:  if (unit_ID == 1) // المنتج يقبل القص 
             فيتم اظهار الكمبوبكس المحمل بالقطع الموجودة لهذا الصنف والانتقال اليها 
             وفتح القائمة للاختيار منها القطعة المراد انقاص طولها كهدر من الطول الاصلى

        b:  else اظهار رسالة بان هذا الصنف غير قابل للقص ولا يتم التعامل معه هنا ويمكن التعامل معه فى شاشة الجرد
            ثم يعيد التركيز على txtSeaarchProd ومسح الرقم




   -----------------------------------------------------------------------------------------
   lblTypeInvID =7 سابعا : اذن اضافة مخزون صنف عموما ليس له مورد 
   -----------------------------------------------------------------------------------------
       يعنى ان الرقم الذى كتب هو كود صنف مراد  اضافة طول الى  طوله ان كان قابل للقص كتصحيح جزئى للطول الاصلى
       فيتم جلب بياناته بواسطة الدالة GetProd(code); وعرضها 
       ثم يقوم بجلب قطع هذا الصنف بالدالة  LoadPieceData();
       وهنا توجد حالتان     
        a:  if (unit_ID == 1) // المنتج يقبل القص 
             فيتم اظهار الكمبوبكس المحمل بالقطع الموجودة لهذا الصنف والانتقال اليها 
             وفتح القائمة للاختيار منها القطعة المراد انقاص طولها كهدر من الطول الاصلى

        b:  else اظهار رسالة بان هذا الصنف غير قابل للقص ولا يتم التعامل معه هنا ويمكن التعامل معه فى شاشة الجرد
            ثم يعيد التركيز على txtSeaarchProd ومسح الرقم

   -----------------------------------------------------------------------------------------

   واخيرا: كيف نعالج كل هذه الامور فيما يخص الحدث انتر في txtSeaarchProd_KeyDown
   وما ينقص من دوال يمكن اضافتها واخرى يمكن حذفها لكى يكون كود منظم وجيد للتطوير ويغطى كل الحالات          


 */
