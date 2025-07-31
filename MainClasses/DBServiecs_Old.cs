using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Net;
using System.Security.Cryptography;
using Microsoft.Reporting.WinForms;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Drawing;
namespace MizanOriginalSoft.MainClasses
{
    public  class DBServiecs_Old
    {
        /*           الاكواد السابقة
         *           
        #region @@@@@@@ frmLogIn @@@@@

        // جلب حركة المنتج حسب المعرف
        public static DataTable DailyFee_GetJournalEntry(int Bill_No,int InvTypeID)
        {
            return dbHelper.ExecuteSelectQuery("DailyFee_GetJournalEntry", cmd =>
            {
                cmd.Parameters.Add("@Bill_No", SqlDbType.Int).Value = Bill_No;
                cmd.Parameters.Add("@InvTypeID", SqlDbType.Int).Value = InvTypeID;
            });
        }


        // جلب المستخدمين المفعّلين فقط لتسجيل الدخول
        public static DataTable GetUsers()//@@@
        {
            return dbHelper.ExecuteSelectQuery("UsersGet", command => { });
        }

 

        /// التحقق من اسم المستخدم وكلمة المرور
        public static DataTable UsersVarify(string username, string password)//@@@
        {
            return dbHelper.ExecuteSelectQuery("UsersVerify", command => Prm_UsersVarify(username, password, command));
        }

        /// جلب صلاحيات المستخدم عند الدخول
        public static DataTable GetPermissions(int Us_ID)//@@@
        {
            return dbHelper.ExecuteSelectQuery("User_GetPermissions", command => Prm_GetPermissions(Us_ID, command));
        }


        // تحديد المعاملات الخاصة بالتحقق من المستخدم
        private static void Prm_UsersVarify(string username, string password, SqlCommand command)//@@@
        {
            command.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
            command.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
        }

        /// تحديد معاملات صلاحيات المستخدم
        private static void Prm_GetPermissions(int Us_ID, SqlCommand command)//@@@
        {
            command.Parameters.Add("@Us_ID", SqlDbType.Int).Value = Us_ID;
        }


        /// تحديث كلمة مرور المستخدم مع رسالة راجعة من الإجراء المخزن
        public static string Updat_UzersPass(int IDUser, string pass_Word)//@@@
        {
            return dbHelper.ExecuteNonQueryWithLogging("Uzers_UpdatPass", command =>
            {
                command.Parameters.Add("@IDUser", SqlDbType.Int).Value = IDUser;
                command.Parameters.Add("@pass_Word", SqlDbType.NVarChar).Value = pass_Word;
            }, expectMessageOutput: true);
        }
        #endregion
        
        #region @@@@@@@@  frm_MainAccounts  @@@@@@@@@@@
        //احضار الحسابات النهائية للتعامل معها
        public static DataTable MainAcc_LoadFinalAccounts(int AccID, string FilterType)//لماذا لا يجلب 106 و  والكل للمديونيات
        {
            string query = "MainAcc_LoadFinalAccounts";
            return dbHelper.ExecuteSelectQuery(query, cmd =>
            {
                cmd.Parameters.Add(new SqlParameter("@AccID", SqlDbType.Int) { Value = AccID });
                cmd.Parameters.Add(new SqlParameter("@FilterType", SqlDbType.NVarChar) { Value = FilterType });
            });
        }
        //احضار الحسابات الفرعية للحساب الممرر
        public static DataTable MainAcc_LoadFollowers(int AccID)//@@@
        {
            return dbHelper.ExecuteSelectQuery("MainAcc_LoadFollowers",
                command => command.Parameters.Add("@AccID", SqlDbType.Int).Value = AccID);
        }

        //احضار الحسابات الفرعية للحساب الممرر والحساب الاصلى لتعبئة كمبوبكس
        public static DataTable MainAcc_LoadFollowersAndParent(int AccID)//@@@
        {
            return dbHelper.ExecuteSelectQuery("MainAcc_LoadFollowersAndParent",
                command => command.Parameters.Add("@AccID", SqlDbType.Int).Value = AccID);
        }

        //احضار الحساب الرئيسى الممرر فقط
        public static DataTable MainAcc_LoadTopByID(int AccID)//@@@
        {
            return dbHelper.ExecuteSelectQuery("MainAcc_LoadTopByID",
                command => command.Parameters.Add("@AccID", SqlDbType.Int).Value = AccID);
        }
        
   

        // دالة حذف حساب او تصنيف
        public static bool MainAcc_DeleteCatogryOrAcc(
           int accID,
           out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithParamsAndMessage(
                "MainAcc_DeleteCatogryOrAcc",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@AccID", accID);
                },
                expectMessageOutput: true
            );

            // اعتبر أن النجاح إذا الرسالة تبدأ بـ "تم"
            return resultMessage.StartsWith("تم");
        }


        //دالة ادراج او تعديل تصنيف فرعى
        public static bool MainAcc_InsertSubCat(
           int accID,
           int? parentAccID,
           string accName,
           out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithParamsAndMessage(
                "MainAcc_InsertSubCat",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@AccID", accID);
                    cmd.Parameters.AddWithValue("@ParentAccID", parentAccID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccName", accName);
                },
                expectMessageOutput: true
            );

            // اعتبر أن النجاح إذا الرسالة تبدأ بـ "تم"
            return resultMessage.StartsWith("تم");
        }

        //دالة تعديل تصنيف فرعى
        public static bool MainAcc_UpdateSubCat(
           int accID,
           string accName,
           out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithParamsAndMessage(
                "MainAcc_UpdateSubCat",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@AccID", accID);
                    cmd.Parameters.AddWithValue("@AccName", accName);
                },
                expectMessageOutput: true
            );

            // اعتبر أن النجاح إذا الرسالة تبدأ بـ "تم"
            return resultMessage.StartsWith("تم");
        }





        #endregion 
        
        #region @@@@@@ frmAccounts_Mangment @@@@@@
        // جلب قائمة الموردين بدون شروط
        public static DataTable Accounts_GetSupplier()//@@@
        {
            return dbHelper.ExecuteSelectQuery("Accounts_GetSupplier");
        }
        //


        //اختبار وجود رقم الحساب      
        public static DataTable AccountTestID(int AccID )//@@@
        {
            string query = "AccountTestID";
            return dbHelper.ExecuteSelectQuery(query, cmd =>
            {
                cmd.Parameters.Add(new SqlParameter("@AccID", SqlDbType.Int) { Value = AccID });
            });
        }

        // دالة لتحديث حالة الأصول الثابتة لحساب معين
        public static bool AccountUpdatAssetsEnd(int AccID, bool IsEndedFixedAssets)
        {
            string resultMessage = dbHelper.ExecuteNonQueryWithLogging(
                "AccountUpdatAssetsEnd",
                cmd =>
                {
                    cmd.Parameters.Add("@AccID", SqlDbType.Int).Value = AccID;
                    cmd.Parameters.Add("@IsEndedFixedAssets", SqlDbType.Bit).Value = IsEndedFixedAssets;
                });

            // اعتبر أن العملية ناجحة إذا لم تكن الرسالة تحتوي على كلمة "خطأ"
            return !resultMessage.Contains("خطأ");
        }

        //دالة حذف حساب
        public static string Account_Delelet(int AccID)//@@@
        {
            return dbHelper.ExecuteNonQueryWithParamsAndMessage(
                "Account_Delelet",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@AccID", AccID);

                    // إضافة بارامتر الإخراج @IsDeleted، ولو مش حتستخدمه الآن
                    var isDeletedParam = new SqlParameter("@IsDeleted", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(isDeletedParam);
                },
                expectMessageOutput: true
            );
        }

        //دالة اضافة او تحديث حساب
        public static string Account_UpdateOrInsert(
            int AccID,
            int? ID_TopAcc = null,
            int? ClassID = null,
            string AccName = null,
            string FirstPhon = null,
            string AntherPhon = null,
            string AccNote = null,
            bool? HiddenAcc = null,
            string ClientEmail = null,
            string ClientAddress = null,
            float? FixedAssetsValue = null,
            float? DepreciationRateAnnually = null,
            int? FixedAssetsAge = null,
            float? AnnuallyInstallment = null,
            float? MonthlyInstallment = null,
            bool? IsEndedFixedAssets = null,
            DateTime? FixedAssetsEndDate = null
        )
        {
            return dbHelper.ExecuteNonQueryWithParamsAndMessage(
                "Account_UpdateOrInsert",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@AccID", AccID);
                    cmd.Parameters.AddWithValue("@ID_TopAcc", ID_TopAcc != null ? (object)ID_TopAcc : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ClassID", ClassID != null ? (object)ClassID : DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccName", AccName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FirstPhon", FirstPhon ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AntherPhon", AntherPhon ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccNote", AccNote ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@HiddenAcc", HiddenAcc != null ? (object)HiddenAcc : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ClientEmail", ClientEmail ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ClientAddress", ClientAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FixedAssetsValue", FixedAssetsValue != null ? (object)FixedAssetsValue : DBNull.Value);
                    cmd.Parameters.AddWithValue("@DepreciationRateAnnually", DepreciationRateAnnually != null ? (object)DepreciationRateAnnually : DBNull.Value);
                    cmd.Parameters.AddWithValue("@FixedAssetsAge", FixedAssetsAge != null ? (object)FixedAssetsAge : DBNull.Value);
                    cmd.Parameters.AddWithValue("@AnnuallyInstallment", AnnuallyInstallment != null ? (object)AnnuallyInstallment : DBNull.Value);
                    cmd.Parameters.AddWithValue("@MonthlyInstallment", MonthlyInstallment != null ? (object)MonthlyInstallment : DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsEndedFixedAssets", IsEndedFixedAssets != null ? (object)IsEndedFixedAssets : DBNull.Value);
                    cmd.Parameters.AddWithValue("@FixedAssetsEndDate", FixedAssetsEndDate != null ? (object)FixedAssetsEndDate : DBNull.Value);
                },
                expectMessageOutput: true
            );
        }
        #endregion
        
        #region @@@@@@@ Categories Table @@@@@
        // جلب جميع التصنيفات على شكل شجرة  ###
        public static DataTable Categories_GetAll()//@@
        {
            return dbHelper.ExecuteSelectQuery("Categories_GetAll");
        }
  
        // ✅ إدراج تصنيف جديد
        public static string Categories_Insert(string CategoryName, int ParentID, int UserID)//@@@
        {
            return dbHelper.ExecuteNonQueryWithLogging("Categories_Insert",
                cmd =>
                {
                    cmd.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = CategoryName;
                    cmd.Parameters.Add("@ParentID", SqlDbType.Int).Value = ParentID;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                }
            // نحذف expectMessageOutput لأنه لا يوجد @Message OUTPUT في الإجراء المخزن
            );
        }

        // ✅ تحديث اسم التصنيف
        public static string Categories_UpdateName(int CategoryID, string OldName, string CategoryName, int UserID)//@@@
        {
            return dbHelper.ExecuteNonQueryWithLogging("Categories_UpdateName",
                cmd =>
                {
                    cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Value = CategoryID;
                    cmd.Parameters.Add("@OldName", SqlDbType.NVarChar, 100).Value = OldName;
                    cmd.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = CategoryName;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                }
            //          expectMessageOutput: true
            );
        }

        #endregion

        #region @@@@@@@ Product Table @@@@@
        // جلب معرف المنتج من خلال كود الصنف
        public static DataTable Product_GetIDByCode(string ProductCode)//@@@
        {
            return dbHelper.ExecuteSelectQuery("Product_GetIDByCode", cmd =>
            {
                cmd.Parameters.Add("@ProductCode", SqlDbType.VarChar).Value = ProductCode;
            });
        }

        //جلب جميع الاصناف
        public static DataTable Product_GetAll()//@@@
        {
            return dbHelper.ExecuteSelectQuery("Product_GetAll");
        }

        // جلب قائمة وحدات القياس
        public static DataTable ProductGetUnits()//@@@
        {
            return dbHelper.ExecuteSelectQuery("ProductGetUnits");
        }

        // جلب بيانات صنف  محدد بالمعرفID_Product
        public static DataTable Product_GetOneID(int ID_Product)//@@@
        {
            return dbHelper.ExecuteSelectQuery("Product_GetOneID", cmd =>
            {
                cmd.Parameters.Add("@ID_Product", SqlDbType.Int).Value = ID_Product;
            });
        }

        //دالة لحذف صنف ليس له حركات
        public static string Product_Delete(int ID_Product)//@@@
        {
            return dbHelper.ExecuteNonQueryWithLogging("Product_Delete", command =>
            {
                command.Parameters.Add("@ID_Product", SqlDbType.Int).Value = ID_Product;
            }, expectMessageOutput: true);
        }

        // ✅  نقل  مجموعة من الأصناف فى تصنيف محدد اخر
        public static string Product_UpdateCategory(DataTable ProductIDs, int CategoryID, int UserID)//@@@
        {
            return dbHelper.ExecuteNonQueryWithLogging("Product_UpdateCategory",
                cmd =>
                {
                    var productIDsParam = cmd.Parameters.AddWithValue("@ProductIDs", ProductIDs);
                    productIDsParam.SqlDbType = SqlDbType.Structured;
                    productIDsParam.TypeName = "dbo.ProductIDList";

                    cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Value = CategoryID;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                }
            // لا حاجة لـ expectMessageOutput: true هنا
            );
        }

        // جلب اعلى قيمة لمعرف الصنف
        public static DataTable Product_GetMaxID()//@@@
        {
            return dbHelper.ExecuteSelectQuery("Product_GetMaxID");
        }

        // جلب اعلى قيمة كود الصنف
        public static DataTable Product_GetMaxCode()//@@@
        {
            return dbHelper.ExecuteSelectQuery("Product_GetMaxCode");
        }

        //جلب الاصناف التى تم تحديدها فى الداتا تيبل
        public static DataTable Product_GetSelected(DataTable ProductIDs)//@@@
        {
            return dbHelper.ExecuteSelectQuery("Product_GetSelected",
                cmd =>
                {
                    var param = cmd.Parameters.AddWithValue("@ProductIDs", ProductIDs);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = "dbo.ProductIDList"; // تأكد أن هذا الاسم مطابق لتعريف نوع الجدول في SQL Server
                }
            );
        }

        //دالة اضافة  صنف جديد
        public static int Product_InsertItem(
            string ProdName,
            int UnitID,
            float B_Price,
            float U_Price,
            string ProdCodeOnSuplier,
            float MinLenth,
            float MinStock,
            int Category_id,
            int SuplierID,
            string NoteProduct,
            string PicProduct)//@@@
        {
            return dbHelper.ExecuteScalarQuery<int>("Product_InsertItem", cmd =>
            {
                cmd.Parameters.Add("@ProdName", SqlDbType.NVarChar, 60).Value = ProdName;
                cmd.Parameters.Add("@UnitID", SqlDbType.Int).Value = UnitID;
                cmd.Parameters.Add("@B_Price", SqlDbType.Real).Value = B_Price;
                cmd.Parameters.Add("@U_Price", SqlDbType.Real).Value = U_Price;
                cmd.Parameters.Add("@ProdCodeOnSuplier", SqlDbType.NVarChar, 50).Value = ProdCodeOnSuplier;
                cmd.Parameters.Add("@MinLenth", SqlDbType.Real).Value = MinLenth;
                cmd.Parameters.Add("@MinStock", SqlDbType.Real).Value = MinStock;
                cmd.Parameters.Add("@Category_id", SqlDbType.Int).Value = Category_id;
                cmd.Parameters.Add("@SuplierID", SqlDbType.Int).Value = SuplierID;
                cmd.Parameters.Add("@NoteProduct", SqlDbType.NVarChar, 50).Value = NoteProduct;
                cmd.Parameters.Add("@PicProduct", SqlDbType.NVarChar, 50).Value = PicProduct;
            });
        }

        //دالة تعديل  صنف 
        public static int Product_UpdateItem(int ID_Product,
            string ProdName,
            int UnitID,
            float B_Price,
            float U_Price,
            bool HiddinProd,
            string ProdCodeOnSuplier,
            float MinLenth,
            float MinStock,
            int Category_id,
            int SuplierID,
            string NoteProduct)//@@@
        {
            return dbHelper.ExecuteScalarQuery<int>("Product_UpdateItem", cmd =>
            {
                cmd.Parameters.Add("@ID_Product", SqlDbType.Int).Value = ID_Product;
                cmd.Parameters.Add("@ProdName", SqlDbType.NVarChar, 50).Value = ProdName;
                cmd.Parameters.Add("@UnitID", SqlDbType.Int).Value = UnitID;
                cmd.Parameters.Add("@B_Price", SqlDbType.Real).Value = B_Price;
                cmd.Parameters.Add("@U_Price", SqlDbType.Real).Value = U_Price;
                cmd.Parameters.Add("@HiddinProd", SqlDbType.Real).Value = HiddinProd;

                cmd.Parameters.Add("@ProdCodeOnSuplier", SqlDbType.NVarChar, 50).Value = ProdCodeOnSuplier;
                cmd.Parameters.Add("@MinLenth", SqlDbType.Real).Value = MinLenth;
                cmd.Parameters.Add("@MinStock", SqlDbType.Real).Value = MinStock;
                cmd.Parameters.Add("@Category_id", SqlDbType.Int).Value = Category_id;
                cmd.Parameters.Add("@SuplierID", SqlDbType.Int).Value = SuplierID;
                cmd.Parameters.Add("@NoteProduct", SqlDbType.NVarChar, 50).Value = NoteProduct;
            });
        }
 
        
        //تعديل مجمع للاصناف
        public static bool Products_UpdateSelectedItems(
            DataTable productIdTable,
            float? uPrice = null,
            float? bPrice = null,
            int? supplierId = null,
            int? categoryId = null,
            int? unitId = null)
        {
            if (productIdTable == null || !productIdTable.Columns.Contains("ProductID"))
            {
                throw new ArgumentException("DataTable must contain a 'ProductID' column.");
            }

            return dbHelper.ExecuteNonQueryWithParameterSetter("Products_UpdateSelectedItems", cmd =>
            {
                // TVP
                var tvpParam = new SqlParameter("@ProductIDs", SqlDbType.Structured)
                {
                    TypeName = "dbo.ProductIDList",
                    Value = productIdTable
                };
                cmd.Parameters.Add(tvpParam);

                // Optional parameters with DBNull handling
                cmd.Parameters.AddWithValue("@U_Price", uPrice.HasValue ? (object)uPrice.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@B_Price", bPrice.HasValue ? (object)bPrice.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@SuplierID", supplierId.HasValue ? (object)supplierId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Category_id", categoryId.HasValue ? (object)categoryId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@UnitID", unitId.HasValue ? (object)unitId.Value : DBNull.Value);
            });
        }
        #endregion

        #region rpt_Products @#@
        // دالة استدعاء تقرير كارت صنف رقم 0    @@@ 
        public static DataTable GetProductCard_Set(int ID_Product, int Warehouse_Id, DateTime  StartDate, DateTime EndDate)//@@@
        {
            return dbHelper.ExecuteSelectQuery("rpt_GetProductCard_Set", cmd =>
            {
                cmd.Parameters.Add("@ID_Product", SqlDbType.Int ).Value = ID_Product;
                cmd.Parameters.Add("@Warehouse_Id", SqlDbType.Int ).Value = Warehouse_Id;
                cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = StartDate;
                cmd.Parameters.Add("@EndDate", SqlDbType.Date ).Value = EndDate;
            });
        }
        public static DataTable GetProductCard()//@@@
        {
            return dbHelper.ExecuteSelectQuery("rpt_GetProductCard", cmd => { });
        }

        // دالة استدعاء تقرير تفصيلى لحركة صنف     @@@  مشتريات او مبيعات
        public static DataTable rpt_GetDetailedMovementsReport(int ID_Prod, int Warehouse_Id, DateTime StartDate, DateTime EndDate,string WayMove)//@@@
        {
            return dbHelper.ExecuteSelectQuery("rpt_GetDetailedMovementsReport", cmd =>
            {
                cmd.Parameters.Add("@ID_Prod", SqlDbType.Int).Value = ID_Prod;
                cmd.Parameters.Add("@Warehouse_Id", SqlDbType.Int).Value = Warehouse_Id;
                cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = StartDate;
                cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value = EndDate;
                cmd.Parameters.Add("@WayMove", SqlDbType.NVarChar).Value = WayMove;
            });//@@@
        }
        #endregion



        #region @@@@@@@ Reports Table @@@@@

        // تمت المراجعة لاستدعاء قائمة التقارير للحسابات    ### 
        public static DataTable RepMenu_Accounts(bool ForAccounts, bool IsForGroupAccounts, int TopAcc)//@@@
        {
            return dbHelper.ExecuteSelectQuery("RepMenu_Accounts", cmd =>
            {
                cmd.Parameters.Add("@ForAccounts", SqlDbType.Bit).Value = ForAccounts;
                cmd.Parameters.Add("@IsForGroupAccounts", SqlDbType.Bit).Value = IsForGroupAccounts;
                cmd.Parameters.Add("@TopAcc", SqlDbType.Int ).Value = TopAcc;

            });
        }

        // تمت المراجعة لاستدعاء قائمة التقارير للاصناف    ### 
        public static DataTable RepMenu_Products(bool ForItems, bool ForItemsGroup)//@@@
        {
            return dbHelper.ExecuteSelectQuery("RepMenu_Products", cmd =>
            {
                cmd.Parameters.Add("@ForItems", SqlDbType.Bit).Value = ForItems;
                cmd.Parameters.Add("@ForItemsGroup", SqlDbType.Bit).Value = ForItemsGroup;
            });
        }

        #endregion


        #region @@@@@ BarCode @@@@

        // دالة جلب بيانات جدول الباركود الى الداتا جريد فيو
        public static DataTable BarcodesGet()//@@@
        {
            return dbHelper.ExecuteSelectQuery("BarcodesGet");
        }
        // دالة جلب بيانات جدول الباركود لطباعتها
        public static DataTable sp_GetBarcodesToPrint()
        {
            return dbHelper.ExecuteSelectQuery("sp_GetBarcodesToPrint");
        }
        //دالة اضافة كود لطباعة الباركود
        public static bool sp_InsertBarcodesToPrint(int productId, int count)//@@@
        {
            return dbHelper.ExecuteNonQueryWithParameterSetter("sp_InsertBarcodesToPrint", cmd =>
            {
                cmd.Parameters.Add("@Product_id", SqlDbType.Int).Value = productId;
                cmd.Parameters.Add("@Count", SqlDbType.Int).Value = count;
            });
        }

        //دالة حذف مجموعة ارقام من جدول الباركود
        public static string sp_DeleteBarcodesByProductIDs(DataTable ProductIDsTable)//@@@
        {
            return dbHelper.ExecuteNonQueryWithLogging("sp_DeleteBarcodesByProductIDs",
                cmd =>
                {
                    var productIDsParam = cmd.Parameters.AddWithValue("@ProductIDsTable", ProductIDsTable);
                    productIDsParam.SqlDbType = SqlDbType.Structured;
                    productIDsParam.TypeName = "dbo.IntListTableType";

                }
            // لا حاجة لـ expectMessageOutput: true هنا
            );
        }


        #endregion

        #region @@@@@@@@@@@@@@  Faturalar  @@@@@@@@@@@











        #endregion

        #region ########################### Faturalar Details  @@@@@@@@@@@
        //Product_GetOrCreatePieces 




 




        // جلب حركة المنتج حسب المعرف
        public static DataTable Product_Movement(int ID_Product)
        {
            return dbHelper.ExecuteSelectQuery("Product_Movement", cmd =>
            {
                cmd.Parameters.Add("@ID_Product", SqlDbType.Int).Value = ID_Product;
            });
        }





        //



        // جلب معرف القطعة لمنتج
        public static DataTable Product_GetPieceID(int ID_Prod)
        {
            return dbHelper.ExecuteSelectQuery("Product_GetPieceID", cmd =>
            {
                cmd.Parameters.Add("@ID_Prod", SqlDbType.Int).Value = ID_Prod;
            });
        }


        //PieceMaxID
        // جلب مندوبي المبيعات
        public static DataTable PieceMaxID()
        {
            return dbHelper.ExecuteSelectQuery("PieceMaxID");
        }

        /// <summary>
        /// إدخال قطعة جديدة للمنتج
        /// </summary>
        public static string Pieces_Insert(float Piece_Length, int ID_Prod, float Gem_Discount, float CommisionPiece)
        {
            // تنفيذ الإجراء المخزن مع تمكين انتظار الرسائل المرجعية
            string result = dbHelper.ExecuteNonQueryWithLogging("Piece_insert", cmd =>
            {
                cmd.Parameters.Add("@Piece_Length", SqlDbType.Real).Value = Piece_Length;
                cmd.Parameters.Add("@ID_Prod", SqlDbType.Int).Value = ID_Prod;
                cmd.Parameters.Add("@Gem_Discount", SqlDbType.Real).Value = Gem_Discount;
                cmd.Parameters.Add("@CommisionPiece", SqlDbType.Real).Value = CommisionPiece;
            }, expectMessageOutput: true);

            // إذا كانت هناك رسالة مرجعة من الإجراء المخزن، يتم إرجاعها مباشرة
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            // إذا لم يتم إرجاع أي رسالة، يتم إرجاع رسالة نجاح افتراضية
            return "تم إدخال القطعة بنجاح";
        }

        /// <summary>
        /// إعادة إدراج قطعة مرتبطة بفاتورة
        /// </summary>
        public static string Piece_ReInsaer(int Piece_ID, int Inv_IDd)
        {
            // تنفيذ الإجراء المخزن مع تمكين انتظار الرسائل المرجعية
            string result = dbHelper.ExecuteNonQueryWithLogging("Piece_ReInsaer", cmd =>
            {
                cmd.Parameters.Add("@Piece_ID", SqlDbType.Int).Value = Piece_ID;
                cmd.Parameters.Add("@Inv_IDd", SqlDbType.Int).Value = Inv_IDd;
            }, expectMessageOutput: true);

            // إذا كانت هناك رسالة مرجعة من الإجراء المخزن، يتم إرجاعها مباشرة
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            // إذا لم يتم إرجاع أي رسالة، يتم إرجاع رسالة نجاح افتراضية
            return "تم إعادة إدراج القطعة بنجاح";
        }

        #endregion













        #region ########## frmReportWay ####################
        // حفظ آخر القيم المستخدمة لاستخدامها لاحقًا
        /// <summary>
        /// حفظ بيانات التقرير الافتراضي (الطابعة، المخزن، والفترة) وإرجاع رسالة من الإجراء المخزن
        /// </summary>

        public static string GenralData_SaveDefRepData(
    DateTime DefStartPeriod,
    DateTime DefEndPeriod,
    string DefaultPrinter,
    string DefaultWarehouse,
    string DefEndRdoChecked)
        {
            return dbHelper.ExecuteNonQueryWithParamsAndMessage(
                "GenralData_SaveDefRepData",
                cmd => {
                    cmd.Parameters.AddWithValue("@DefStartPeriod", DefStartPeriod);
                    cmd.Parameters.AddWithValue("@DefEndPeriod", DefEndPeriod);
                    cmd.Parameters.AddWithValue("@DefaultPrinter", DefaultPrinter);
                    cmd.Parameters.AddWithValue("@DefaultWarehouse", DefaultWarehouse);
                    cmd.Parameters.AddWithValue("@DefEndRdoChecked", DefEndRdoChecked);
                },
                expectMessageOutput: true
            );
        }
        // تحديد المعاملات للإجراء المخزن
        private static void Prm_SaveDefRepData(
            DateTime DefStartPeriod,
            DateTime DefEndPeriod,
            string DefaultPrinter,
            string DefaultWarehouse,
            string DefEndRdoChecked,
            SqlCommand command)
        {
            command.Parameters.Add("@DefStartPeriod", SqlDbType.Date).Value = DefStartPeriod;
            command.Parameters.Add("@DefEndPeriod", SqlDbType.Date).Value = DefEndPeriod;
            command.Parameters.Add("@DefaultPrinter", SqlDbType.NVarChar).Value = DefaultPrinter;
            command.Parameters.Add("@DefaultWarehouse", SqlDbType.NVarChar).Value = DefaultWarehouse;
            command.Parameters.Add("@DefEndRdoChecked", SqlDbType.NVarChar).Value = DefEndRdoChecked;
        }




        #endregion
        
        #region ############# frmReport_Preview ##################

        // جلب القيم الافتراضية لتقارير الشاشة (بدون معاملات)
        public static DataTable GenralData_GetDefRepData()//@@@
        {
            return dbHelper.ExecuteSelectQuery("GenralData_GetDefRepData", cmd => { }); // لا توجد معاملات مطلوبة
        }

        // جلب تاريخ بداية الحسابات (بدون معاملات)
        public static DataTable GenralData_GetStartAccountsDate()
        {
            return dbHelper.ExecuteSelectQuery("GenralData_GetStartAccountsDate", cmd => { }); // لا توجد معاملات مطلوبة
        }

        // جلب الفروع الرئيسية (المخازن) المعروضة في الشاشة (بدون معاملات)
        public static DataTable GenralData_GetWarehouse()//@@@
        {
            return dbHelper.ExecuteSelectQuery("GenralData_GetWarehouse", cmd => { }); // لا توجد معاملات مطلوبة
        }

        #endregion

        #region ############### Main Services ########################

 
        // تحديث أرصدة الحسابات وإرجاع رسالة من الإجراء المخزن
        public static string Ex_UpdateBalance()//@@@
        {
            // تنفيذ الإجراء المخزن بدون معاملات، مع انتظار رسالة OUTPUT من SQL
            return dbHelper.ExecuteNonQueryWithLogging("UpdateAllBalances", cmd => { }, expectMessageOutput: true);
        }



        // استدعاء بيانات تقرير معين حسب الفئة (يبدو أن هذا يكرر الإجراء السابق)
        public static DataTable sp_GetReportData(int ForAccounts)
        {
            return dbHelper.ExecuteSelectQuery("sp_GetReportsByCategory", cmd =>
            {
                cmd.Parameters.Add("@ForAccounts", SqlDbType.Int).Value = ForAccounts;
            });
        }

        #endregion

        #region ################ Accounts #######################

        // جلب تقرير المصروف اليومي لحساب معين بين تاريخين
        public static DataTable DailyFee_AccountByDate(int AccID, DateTime StartDate, DateTime EndDate )
        {
            return dbHelper.ExecuteSelectQuery("DailyFee_AccountByDate", cmd =>
            {
                cmd.Parameters.Add("@AccID", SqlDbType.Int).Value = AccID;
                cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
            });
        }

        // جلب كشف أرصدة الحسابات الفرعية المرتبطة بحساب رئيسي
        public static DataTable Account_Balance(int ID_TopAcc)
        {
            return dbHelper.ExecuteSelectQuery("Account_Balance", cmd =>
            {
                cmd.Parameters.Add("@ID_TopAcc", SqlDbType.Int).Value = ID_TopAcc;
            });
        }

        // جلب تقرير الحسابات الرئيسية
        public static DataTable rep_AccTopMain(int ID_TopAcc)
        {
            return dbHelper.ExecuteSelectQuery("rep_AccTopMain", cmd =>
            {
                cmd.Parameters.Add("@ID_TopAcc", SqlDbType.Int).Value = ID_TopAcc;
            });
        }

        // جلب كشف حساب لحساب فرعي
        public static DataTable rep_AccountStatement(int AccID)
        {
            return dbHelper.ExecuteSelectQuery("rep_AccountStatement", cmd =>
            {
                cmd.Parameters.Add("@AccID", SqlDbType.Int).Value = AccID;
            });
        }

        // جلب تقرير المصروف اليومي لحساب فرعي
        public static DataTable DailyFee_Account(int AccID)
        {
            return dbHelper.ExecuteSelectQuery("DailyFee_Account", cmd =>
            {
                cmd.Parameters.Add("@AccID", SqlDbType.Int).Value = AccID;
            });
        }


        //   
        public static DataTable AccTop_TestTopID(int ID_TopAcc)
        {
            return dbHelper.ExecuteSelectQuery("AccTop_TestTopID", cmd =>
            {
                cmd.Parameters.Add("@ID_TopAcc", SqlDbType.Int).Value = ID_TopAcc;
            });
        }


        /// <summary>
        /// تعديل الحساب الرئيسي فقط لحساب معين مع إرجاع رسالة من الإجراء المخزن
        /// </summary>
        public static string Ex_AccountUpdat_TopOnly(int AccID, int ID_TopAcc)
        {
            return dbHelper.ExecuteNonQueryWithLogging("AccountUpdat_TopOnly", cmd =>
            {
                // تمرير معرف الحساب والمعرف الجديد للحساب الرئيسي كمعاملات
                cmd.Parameters.Add("@AccID", SqlDbType.Int).Value = AccID;
                cmd.Parameters.Add("@ID_TopAcc", SqlDbType.Int).Value = ID_TopAcc;
            }, expectMessageOutput: true);
        }

        // جلب أكبر رقم معرف للحسابات الرئيسية
        public static DataTable AccTop_GetNewID()//@@@
        {
            return dbHelper.ExecuteSelectQuery("AccTop_GetNewID");
        }

        /// <summary>
        /// إدخال حساب رئيسي جديد مع إرجاع رسالة من الإجراء المخزن
        /// </summary>
        public static string AccTop_Insert(int AccTopID, string AccTopName, int TopAccFollowerID,
                                           bool Is_HasDetails, bool EndUser, bool InvAcc,
                                           bool ChequeEnabled, bool Is_BillAcc, bool Is_FixedAssets)
        {
            return dbHelper.ExecuteNonQueryWithLogging("AccTop_Insert", cmd =>
            {
                // تمرير معاملات إدخال الحساب الرئيسي
                AddAccTopParams(cmd, AccTopID, AccTopName, TopAccFollowerID,
                    Is_HasDetails, EndUser, InvAcc, ChequeEnabled, Is_BillAcc, Is_FixedAssets);
            }, expectMessageOutput: true); // تفعيل استقبال الرسالة من الإجراء
        }



        /// <summary>
        /// تحديث بيانات حساب رئيسي مع إرجاع رسالة من الإجراء المخزن
        /// </summary>
        public static string AccTop_Update(int AccTopID, string AccTopName, int TopAccFollowerID,
                                           bool Is_HasDetails, bool EndUser, bool InvAcc,
                                           bool ChequeEnabled, bool Is_BillAcc, bool Is_FixedAssets)
        {
            return dbHelper.ExecuteNonQueryWithLogging("AccTop_Update", cmd =>
            {
                // تمرير معاملات تحديث الحساب الرئيسي
                AddAccTopParams(cmd, AccTopID, AccTopName, TopAccFollowerID,
                    Is_HasDetails, EndUser, InvAcc, ChequeEnabled, Is_BillAcc, Is_FixedAssets);
            }, expectMessageOutput: true); // استقبال رسالة راجعة من الإجراء
        }

        // بارامترات إدخال أو تعديل حساب رئيسي
        private static void AddAccTopParams(SqlCommand cmd, int AccTopID, string AccTopName, int TopAccFollowerID,
                                      bool Is_HasDetails, bool EndUser, bool InvAcc,
                                      bool ChequeEnabled, bool Is_BillAcc, bool Is_FixedAssets)
        {
            cmd.Parameters.Add("@AccTopID", SqlDbType.Int).Value = AccTopID;
            cmd.Parameters.Add("@AccTopName", SqlDbType.NVarChar, 100).Value = AccTopName;
            cmd.Parameters.Add("@TopAccFollowerID", SqlDbType.Int).Value = TopAccFollowerID;
            cmd.Parameters.Add("@Is_HasDetails", SqlDbType.Bit).Value = Is_HasDetails;
            cmd.Parameters.Add("@EndUser", SqlDbType.Bit).Value = EndUser;
            cmd.Parameters.Add("@InvAcc", SqlDbType.Bit).Value = InvAcc;
            cmd.Parameters.Add("@ChequeEnabled", SqlDbType.Bit).Value = ChequeEnabled;
            cmd.Parameters.Add("@Is_BillAcc", SqlDbType.Bit).Value = Is_BillAcc;
            cmd.Parameters.Add("@Is_FixedAssets", SqlDbType.Bit).Value = Is_FixedAssets;
        }

        #endregion

        #region ############### إدارة المستخدمين Users #####################3333

        // استرجاع بيانات مستخدم بناءً على رقم المعرف
        public static DataTable UzersID_Test(int IDUser)//@@@
        {
            return dbHelper.ExecuteSelectQuery("UzerID_Test", command =>
            {
                command.Parameters.Add("@IDUser", SqlDbType.Int).Value = IDUser;
            });
        }

        /// <summary>
        /// استرجاع جميع أدوار المستخدمين
        /// </summary>
        public static DataTable UzerRoles()
        {
            return dbHelper.ExecuteSelectQuery("UzerRoles", command => { });
        }

        /// <summary>
        /// استرجاع جميع المستخدمين
        /// </summary>
        public static DataTable UzersGetAll()
        {
            return dbHelper.ExecuteSelectQuery("UzersGetAll", command => { });
        }

        // استرجاع أكبر رقم مستخدم حالياً (لإدخال جديد)
        public static DataTable UzersGetMax()//@@@
        {
            return dbHelper.ExecuteSelectQuery("UzersGetMax", command => { });
        }

        /// إدراج مستخدم جديد في النظام مع رسالة راجعة من الإجراء المخزن
        public static string Insert_Uzers(int IDUser, string UserName, bool Stoped)//@@@
        {
            return dbHelper.ExecuteNonQueryWithLogging("Uzers_Insert", command =>
            {
                command.Parameters.Add("@IDUser", SqlDbType.Int).Value = IDUser;
                command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = UserName;
                command.Parameters.Add("@Stoped", SqlDbType.Bit).Value = Stoped;
            }, expectMessageOutput: false );
        }

        /// <summary>
        /// تحديث بيانات مستخدم موجود مع رسالة راجعة من الإجراء المخزن
        /// </summary>
        public static string Updat_Uzers(int IDUser, string UserName, bool Stoped)//@@@
        {
            return dbHelper.ExecuteNonQueryWithLogging("Uzers_Updat", command =>
            {
                command.Parameters.Add("@IDUser", SqlDbType.Int).Value = IDUser;
                command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = UserName;
                command.Parameters.Add("@Stoped", SqlDbType.Bit).Value = Stoped;
            }, expectMessageOutput: false );
        }

        // حذف مستخدم بناءً على الرقم التعريفي مع رسالة راجعة من الإجراء المخزن
        public static string Uzers_Delete(int IDUser)//@@@
        {
            // تنفيذ الإجراء المخزن لحذف المستخدم
            string result = dbHelper.ExecuteNonQueryWithLogging("Uzers_Delete", command =>
            {
                command.Parameters.Add("@IDUser", SqlDbType.Int).Value = IDUser;
            }, expectMessageOutput: false );

            // إذا كانت هناك رسالة من الإجراء المخزن، إرجاعها
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            // تنفيذ حذف صلاحيات المستخدم بعد الحذف
            string permissionsResult = User_DeletePermissions(IDUser);

            // إذا كانت هناك رسالة من حذف الصلاحيات، إرجاعها
            if (!string.IsNullOrEmpty(permissionsResult))
            {
                return permissionsResult;
            }

            return "تم الحذف بنجاح"; // إذا لم توجد أي رسائل
        }

        /// <summary>
        /// حذف صلاحيات المستخدم المرتبط بالأزرار بعد حذفه مع رسالة راجعة من الإجراء المخزن
        /// </summary>
        public static string User_DeletePermissions(int IDUser)
        {
            return dbHelper.ExecuteNonQueryWithLogging("User_DeletePermissions", command =>
            {
                command.Parameters.Add("@IDUser", SqlDbType.Int).Value = IDUser;
            }, expectMessageOutput: true);
        }

        #endregion

        #region################ الصلاحيات والمستخدمين Useres and Permissions ########################

        /// <summary>
        /// دالة لجلب كلمة المرور الخاصة بالمستخدم الإدمن
        /// </summary>
        public static DataTable GetAdminPass()//@@@
        {
            return dbHelper.ExecuteSelectQuery("GetAdminPass", command => { });
        }

        /// <summary>
        /// اختبار إذا كان للمستخدم حركات سابقة في الفواتير
        /// </summary>
        public static DataTable UsersTest_InInv(int Us_ID)//@@@
        {
            return dbHelper.ExecuteSelectQuery("UsersTest_InInv", command => Prm_UsersPerimssion(Us_ID, command));
        }










        /// جلب حالة جميع الأزرار (مفعل أو غير مفعل) للمستخدم
        public static DataTable UsersBtnChecked(int Us_ID)//@@@
        {
            return dbHelper.ExecuteSelectQuery("UsersBtnChecked", command => Prm_UsersPerimssion(Us_ID, command));
        }

        // جلب جميع صلاحيات المستخدم لعرضها في واجهة الإدارة
        public static DataTable UsersPerimssionList(int Us_ID)//@@@
        {
            return dbHelper.ExecuteSelectQuery("UsersPerimssion", command => Prm_UsersPerimssion(Us_ID, command));
        }

        // إعداد معاملات جلب أو تحديث صلاحيات المستخدم
        private static void Prm_UsersPerimssion(int Us_ID, SqlCommand command)//@@@
        {
            command.Parameters.Add("@Us_ID", SqlDbType.Int).Value = Us_ID;
        }

        /// جلب جميع المستخدمين (بما فيهم المتوقفين)
        public static DataTable UsersGetAll()//@@@
        {
            return dbHelper.ExecuteSelectQuery("UsersGetAll", command => { });
        }


        /// تحديث صلاحية زر معين للمستخدم
        /// تحديث صلاحيات المستخدم بالنسبة للأزرار (تحديد ما إذا كان مسموحًا للمستخدم بالوصول إلى زر معين).
        public static string UpdatCHK(int Us_ID, string Button, bool IsAllowed)//@@@
        {
            // تنفيذ الإجراء المخزن بدون انتظار رسالة راجعة
            bool success = dbHelper.ExecuteNonQueryWithParameterSetter("Users_PermissionsUpdate",
                command => Prm_UpdatCHK(Us_ID, Button, IsAllowed, command));

            // إرجاع رسالة بسيطة بناءً على نجاح التنفيذ
            if (success)
                return "تم تحديث صلاحيات الزر بنجاح";
            else
                return "حدث خطأ أثناء تحديث الصلاحيات.";
        }

        /// إعداد معاملات تحديث صلاحية الزر
        private static void Prm_UpdatCHK(int Us_ID, string Button, bool IsAllowed, SqlCommand command)//@@@
        {
            command.Parameters.Add("@Us_ID", SqlDbType.Int).Value = Us_ID;
            command.Parameters.Add("@Button", SqlDbType.NVarChar).Value = Button;
            command.Parameters.Add("@IsAllowed", SqlDbType.Bit).Value = IsAllowed;
        }


        #endregion

        #region ################# الأسعار اليومية PriceToday ######################333

        /// <summary>
        /// جلب الأسعار اليومية حسب نوع الصنف
        /// </summary>
        public static DataTable GetPriceToday(int ItemType)
        {
            return dbHelper.ExecuteSelectQuery("PriceToday", command => Prm_PriceToday(ItemType, command));
        }

        /// <summary>
        /// تحديد معاملات جلب الأسعار اليومية حسب نوع الصنف
        /// </summary>
        private static void Prm_PriceToday(int ItemType, SqlCommand command)
        {
            command.Parameters.Add("@ItemType", SqlDbType.NVarChar).Value = ItemType;
        }

        /// <summary>
        /// تحديث ترتيب عرض الصنف في الأسعار اليومية
        /// </summary>
        public static string Updat_PriceTodaySaveSort(int ItemID, int ItemSort)
        {
            // تنفيذ الإجراء المخزن مع تمكين انتظار الرسائل الراجعة
            string result = dbHelper.ExecuteNonQueryWithLogging("PriceTodaySaveSort",
                command => Prm_PriceTodaySaveSort(ItemID, ItemSort, command), expectMessageOutput: true);

            // إذا كانت هناك رسالة مرجعة من الإجراء المخزن، يتم إرجاعها مباشرة
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            // إذا لم يتم إرجاع أي رسالة، يتم إرجاع رسالة النجاح الافتراضية
            return "تم تحديث ترتيب عرض الصنف بنجاح";
        }

        /// <summary>
        /// تحديد معاملات تحديث ترتيب عرض الصنف
        /// </summary>
        private static void Prm_PriceTodaySaveSort(int ItemID, int ItemSort, SqlCommand command)
        {
            command.Parameters.Add("@ItemID", SqlDbType.Int).Value = ItemID;
            command.Parameters.Add("@ItemSort", SqlDbType.NVarChar).Value = ItemSort;
        }

        /// <summary>
        /// تحديث سعر البيع وسعر الشراء لصنف معين في الأسعار اليومية.
        /// </summary>
        public static string Updat_PriceToday(int ItemID, float PriceToday_S, float PriceToday_B)
        {
            // تنفيذ الإجراء المخزن مع تمكين انتظار الرسائل الراجعة
            string result = dbHelper.ExecuteNonQueryWithLogging("PriceToday_UpDatePrice",
                command => Prm_PriceToday(ItemID, PriceToday_S, PriceToday_B, command), expectMessageOutput: true);

            // إذا كانت هناك رسالة مرجعة من الإجراء المخزن، يتم إرجاعها مباشرة
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            // إذا لم يتم إرجاع أي رسالة، يتم إرجاع رسالة النجاح الافتراضية
            return "تم تحديث الأسعار بنجاح";
        }

        /// <summary>
        /// تحديد معاملات تحديث سعر البيع وسعر الشراء
        /// </summary>
        private static void Prm_PriceToday(int ItemID, float PriceToday_S, float PriceToday_B, SqlCommand command)
        {
            command.Parameters.Add("@ItemID", SqlDbType.Int).Value = ItemID;
            command.Parameters.Add("@PriceToday_S", SqlDbType.Real).Value = PriceToday_S;
            command.Parameters.Add("@PriceToday_B", SqlDbType.Real).Value = PriceToday_B;
        }

        /// <summary>
        /// جلب التغييرات السعرية لصنف معين خلال فترة زمنية
        /// </summary>
        public static DataTable GetPriceChanges(int ItemID, DateTime StartDate, DateTime EndDate)
        {
            return dbHelper.ExecuteSelectQuery("Price_GetChangesDateRange",
                command => Prm_PriceChanges(ItemID, StartDate, EndDate, command));
        }

        /// <summary>
        /// تحديد معاملات جلب التغييرات السعرية خلال فترة زمنية
        /// </summary>
        private static void Prm_PriceChanges(int ItemID, DateTime StartDate, DateTime EndDate, SqlCommand command)
        {
            command.Parameters.Add("@ItemID", SqlDbType.Int).Value = ItemID;
            command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
            command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
        }

        #endregion

        #region ##################### Search #######################

        public static DataTable AccountSearch_GetType(int ID_TopAcc)
        {
            string query = "EXEC AccountSearch_GetType @ID_TopAcc";
            return dbHelper.ExecuteSelectQuery(query, cmd =>
            {
                cmd.Parameters.Add(new SqlParameter("@ID_TopAcc", SqlDbType.Int) { Value = ID_TopAcc });
            });
        }
        //






        #endregion

        #region ################ Products ##############################



        //Product_GetAll

        // جلب الأصناف التي تم تحديدها
        public static DataTable Products_GetSelectedItims()
        {
            return dbHelper.ExecuteSelectQuery("Products_GetSelectedItims");
        }

        /// <summary>
        /// إلغاء تحديد جميع الأصناف المحددة مسبقًا
        /// </summary>
        public static string Products_UnSelected()
        {
            // تنفيذ الإجراء المخزن مع تمكين انتظار الرسائل المرجعية
            string result = dbHelper.ExecuteNonQueryWithLogging("Products_UnSelected", cmd => { }, expectMessageOutput: true);

            // إذا كانت هناك رسالة مرجعة من الإجراء المخزن، يتم إرجاعها مباشرة
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            // إذا لم يتم إرجاع أي رسالة، يتم إرجاع رسالة نجاح افتراضية
            return "تم إلغاء تحديد جميع الأصناف بنجاح";
        }

        // عدّ الأصناف التي تم تحديدها
        public static DataTable Product_SelectedCount()
        {
            return dbHelper.ExecuteSelectQuery("Product_SelectedCount");
        }

        /// <summary>
        /// تحديد أو إلغاء تحديد صنف معين حسب المعامل المرسل
        /// </summary>
        public static string Product_Selected(int ID_Product, bool SelectItem)
        {
            // تنفيذ الإجراء المخزن مع تمكين انتظار الرسائل المرجعية
            string result = dbHelper.ExecuteNonQueryWithLogging("Product_Selected", cmd =>
            {
                cmd.Parameters.Add("@ID_Product", SqlDbType.Int).Value = ID_Product;
                cmd.Parameters.Add("@SelectItem", SqlDbType.Bit).Value = SelectItem;
            }, expectMessageOutput: true);

            // إذا كانت هناك رسالة مرجعية من الإجراء المخزن، يتم إرجاعها مباشرة
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            // إذا لم يتم إرجاع أي رسالة، يتم إرجاع رسالة نجاح افتراضية
            return "تم تحديث حالة الصنف بنجاح";
        }



        #endregion

        #region ################### Tree Categories ##########################





        // ✅ حذف تصنيف معين ويُرجع رسالة
        public static string Categories_Delete(int CategoryID, int UserID)
        {
            return dbHelper.ExecuteNonQueryWithLogging("Categories_Delete",
                cmd =>
                {
                    cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Value = CategoryID;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                },
                expectMessageOutput: true
            );
        }



        #endregion

        #region #################### Bills ##########################3

        /// <summary>
        /// جلب بيانات الصناديق من قاعدة البيانات
        /// </summary>
        public static DataTable Acc_GetBox()
        {
            return dbHelper.ExecuteSelectQuery("Acc_GetBox");
        }

        /// <summary>
        /// جلب بيانات الحسابات المستخدمة في الفواتير
        /// </summary>
        public static DataTable Acc_GetListForBills()
        {
            return dbHelper.ExecuteSelectQuery("Acc_GetListForBills");
        }


        #endregion

        #region ======== Reports ======================

        /// <summary>
        /// جلب أسماء التقارير المختلفة حسب كود النوع
        /// </summary>
        /// <param name="CodeType">كود نوع التقرير</param>
        /// <returns>جدول يحتوي على أسماء التقارير</returns>
        public static DataTable ReportCode(int CodeType)
        {
            return dbHelper.ExecuteSelectQuery("ReportCode", cmd =>
            {
                cmd.Parameters.Add("@CodeType", SqlDbType.Int).Value = CodeType;
            });
        }

        /// <summary>
        /// جلب إعدادات التقارير حسب كود النوع
        /// </summary>
        /// <param name="CodeType">كود نوع الإعدادات</param>
        /// <returns>جدول يحتوي على إعدادات التقرير</returns>
        public static DataTable GetReportSettings(int CodeType)
        {
            return dbHelper.ExecuteSelectQuery("GetReportSettings", cmd =>
            {
                cmd.Parameters.Add("@CodeType", SqlDbType.Int).Value = CodeType;
            });
        }

        #endregion
     
        #region @@@@@@@ CashTransaction Table @@@@@
   
        // جلب الحسابات الرئيسية على شكل شجرة  ###
        public static DataTable MainAcc_GetTopAccountTree()//@@
        {
            return dbHelper.ExecuteSelectQuery("MainAcc_GetTopAccountTree");
        }

        // جلب السندات حسب النوع
        public static DataTable CashTransactions_GetBillByType(int OperationType_ID)
        {
            return dbHelper.ExecuteSelectQuery("CashTransactions_GetBillByType", cmd =>
                cmd.Parameters.Add("@OperationType_ID", SqlDbType.Int).Value = OperationType_ID
            );
        }

        //دالة ادراج او تعديل سند تحصيل او دفع او تسوية 
        public static bool CashTransactions_InsertOrUpdate(
            int TransactionID,
            string VoucherNumber,
            DateTime? TransactionDate,
            int? OperationType_ID,
            int? AccountID,
            int? AccBox, // ✅ جديد
            float? Amount,
            int? PaymentMethodID,
            string DescriptionNote,
            int? CreatedByUsID,
            string SaveTransaction,
            out string resultMessage)
        {
            resultMessage = null;

            SqlParameter outputMessage = new SqlParameter("@ResultMessage", SqlDbType.NVarChar, 100)
            {
                Direction = ParameterDirection.Output
            };

            resultMessage = dbHelper.ExecuteNonQueryWithParamsAndMessage(
                "CashTransactions_InsertOrUpdate",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                    cmd.Parameters.AddWithValue("@VoucherNumber", VoucherNumber);
                    cmd.Parameters.AddWithValue("@TransactionDate", TransactionDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OperationType_ID", OperationType_ID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccountID", AccountID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccBox", (OperationType_ID == 8 || OperationType_ID == 9 && AccBox.HasValue)
                        ? (object)AccBox.Value
                        : DBNull.Value); cmd.Parameters.AddWithValue("@Amount", Amount ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PaymentMethodID", PaymentMethodID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DescriptionNote", string.IsNullOrWhiteSpace(DescriptionNote) ? (object)DBNull.Value : DescriptionNote);
                    cmd.Parameters.AddWithValue("@CreatedByUsID", CreatedByUsID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SaveTransaction", SaveTransaction ?? "");
                    cmd.Parameters.Add(outputMessage);
                },
                expectMessageOutput: false
            );

            if (outputMessage.Value != DBNull.Value)
                resultMessage = outputMessage.Value.ToString();

            return !string.IsNullOrEmpty(resultMessage);
        }



        //جلب كود جديد للسند
        public static string CashTransactions_GetNewVoucherNumber(int operationTypeID)
        {
            DataTable dt = dbHelper.ExecuteSelectQuery("CashTransactions_GetNewVoucherNumber", cmd =>
            {
                cmd.Parameters.AddWithValue("@OperationType_ID", operationTypeID);
            });

            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["NewVoucherNumber"].ToString();
            else
                return ""; // أو قيمة افتراضية
        }

        public static int CashTransactions_GetNextTransactionID()
        {
            DataTable dt = dbHelper.ExecuteSelectQuery("CashTransactions_GetNextTransactionID");

            if (dt != null && dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0]["NextTransactionID"]);
            else
                return 1; // في حال لا توجد سجلات
        }

        #endregion

        #region @@@@ Cheque Batches @@@@

        //// جلب الحافظات حسب النوع
        //public static DataTable ChequeBatches_GetByMovType(int MovTypeID)
        //{
        //    return dbHelper.ExecuteSelectQuery("ChequeBatches_GetByMovType", cmd =>
        //        cmd.Parameters.Add("@MovTypeID", SqlDbType.Int).Value = MovTypeID
        //    );
        //}



        //// جلب الحافظات حسب كودها
        //public static DataTable ChequeBatches_GetByBatchID(int BatchID)
        //{
        //    return dbHelper.ExecuteSelectQuery("ChequeBatches_GetByBatchID", cmd =>
        //        cmd.Parameters.Add("@BatchID", SqlDbType.Int).Value = BatchID
        //    );
        //}
        //هذه الدالة تقوم بعمل الدالتين ChequeBatches_GetByBatchID   -  ChequeBatches_GetByMovType  السابقتين ويمكن الاستغنا عنهما
        public static DataTable ChequeBatches_Search(string searchBy, int value)
        {
            return dbHelper.ExecuteSelectQuery("ChequeBatches_Search", cmd =>
            {
                cmd.Parameters.Add("@SearchBy", SqlDbType.NVarChar, 20).Value = searchBy;
                cmd.Parameters.Add("@Value", SqlDbType.Int).Value = value;
            });
        }


        //دالة ادراج او تعديل حافظة الشيكات 
 
        public static bool ChequeBatches_InsertOrUpdate(
            int BatchID,
            DateTime? BatchDate,
            int? AccID,
            int? MovTypeID,
            string NoteBatch,
            float? TotalBatch,
            string SaveStatus,
            string BatchCode,
            int Us_ID,
            out string resultMessage)
            {
            resultMessage = null;

            SqlParameter outputMessage = new SqlParameter("@ResultMessage", SqlDbType.NVarChar, 100)
            {
                Direction = ParameterDirection.Output
            };

            resultMessage = dbHelper.ExecuteNonQueryWithParamsAndMessage(
                "ChequeBatches_InsertOrUpdate",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@BatchID", BatchID);
                    cmd.Parameters.AddWithValue("@BatchDate", BatchDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccID", AccID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MovTypeID", MovTypeID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoteBatch", string.IsNullOrWhiteSpace(NoteBatch) ? (object)DBNull.Value : NoteBatch);
                    cmd.Parameters.AddWithValue("@TotalBatch", TotalBatch ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SaveStatus", SaveStatus ?? "");
                    cmd.Parameters.AddWithValue("@BatchCode", BatchCode ?? "");
                    cmd.Parameters.AddWithValue("@Us_ID", Us_ID);
                    cmd.Parameters.Add(outputMessage);
                },
                expectMessageOutput: false
            );

            if (outputMessage.Value != DBNull.Value)
                resultMessage = outputMessage.Value.ToString();

            return !string.IsNullOrEmpty(resultMessage);
        }



        //جلب كود جديد للحافظة
        public static string ChequeBatches_GetNewBatchCode(int MovTypeID)
        {
            DataTable dt = dbHelper.ExecuteSelectQuery("ChequeBatches_GetNewBatchCode", cmd =>
            {
                cmd.Parameters.AddWithValue("@MovTypeID", MovTypeID);
            });

            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["NewBatchCode"].ToString();
            else
                return ""; // أو قيمة افتراضية
        }

       //جلب معرف للحافظة
        public static int ChequeBatches_GetNextBatchID()
        {
            DataTable dt = dbHelper.ExecuteSelectQuery("ChequeBatches_GetNextBatchID");

            if (dt != null && dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0]["NextBatchID"]);
            else
                return 1; // في حال لا توجد سجلات
        }


        //احضار شكات الحافظة
        public static DataTable Cheques_GetByBatchID(int BatchID)
        {
            return dbHelper.ExecuteSelectQuery("Cheques_GetByBatchID", cmd =>
                cmd.Parameters.Add("@BatchID", SqlDbType.Int).Value = BatchID
            );
        }

        //[Cheques_Insert]
        public static bool Cheques_Insert(
    int? Batch_ID,
    string ChequeNumber,
    DateTime? DueDate,
    float? Amount,
    string BankName,
    string Branch,
    string Notes,
    int? StatusCode,
    out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithParamsAndMessage(
                "Cheques_Insert",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Batch_ID", Batch_ID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ChequeNumber", ChequeNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DueDate", DueDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Amount", Amount ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankName", BankName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Branch", Branch ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Notes", Notes ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@StatusCode", StatusCode ?? (object)DBNull.Value);
                },
                expectMessageOutput: false
            );

            // نحكم بنجاح العملية بناءً على النتيجة
            return resultMessage.StartsWith("تم") || string.IsNullOrEmpty(resultMessage);
        }

        public static bool Cheques_Delete(int chequeID, out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithParamsAndMessage(
                "Cheques_Delete",
                cmd => cmd.Parameters.AddWithValue("@ChequeID", chequeID),
                expectMessageOutput: false
            );
            return true;
        }

        public static bool Cheques_UpdateStatus(
           int chequeID,
            string status,
            DateTime? statusDate,
            string rejectReason,
            int? statusCode,
           out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithParamsAndMessage(
                "Cheques_UpdateStatus",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@ChequeID", chequeID);
                    cmd.Parameters.AddWithValue("@Status", status ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@StatusDate", statusDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RejectReason", rejectReason ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@StatusCode", statusCode ?? (object)DBNull.Value);
                },
                expectMessageOutput: false
            );

            // اعتبر أن النجاح إذا الرسالة تبدأ بـ "تم"
            return resultMessage.StartsWith("تم");
        }



        public static bool Cheques_UpdateDueDate(
                    int chequeID,
                    string status,
                    DateTime? newDueDate,
                    string rejectReason,
                    int? statusCode,
                   out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithParamsAndMessage(
                "Cheques_UpdateDueDate",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@ChequeID", chequeID);
                    cmd.Parameters.AddWithValue("@Status", status ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@newDueDate", newDueDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RejectReason", rejectReason ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@StatusCode", statusCode ?? (object)DBNull.Value);
                },
                expectMessageOutput: false
            );

            // اعتبر أن النجاح إذا الرسالة تبدأ بـ "تم"
            return resultMessage.StartsWith("تم");
        }


        #endregion

        */


    }
}
