using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace MizanOriginalSoft.MainClasses
{
    internal class DBServiecs
    {
        #region ********************  Log In Form ********************
        //تحديث بيانات قاعدة البيانات
        public static string A_UpdateAllDataBase()//@@@ 
        {
            string result = dbHelper.ExecuteNonQueryNoParamsWithMessage("A_UpdateAllDataBase", expectMessageOutput: true);
            /*'dbHelper' does not contain a definition for 'ExecuteNonQueryNoParamsWithMessage'*/
            if (!string.IsNullOrEmpty(result))
                return result;

            return "تم التحديث بنجاح";
        }


        // جلب المستخدمين المفعّلين فقط لتسجيل الدخول
        public static DataTable LogIn_GetUsers()
        {
            return dbHelper.ExecuteSelectQuery("LogIn_GetUsers", command => { }) ?? new DataTable();
        }
        
        // التحقق من اسم المستخدم وكلمة المرور
        public static DataTable LogIn_UsersVarify(string username, string password)
        {
            return dbHelper.ExecuteSelectQuery("LogIn_UsersVarify", command => Prm_UsersVarify(username, password, command)) ?? new DataTable();
        }
        private static void Prm_UsersVarify(string username, string password, SqlCommand command)//@@@
        {
            command.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
            command.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
        }

        // جلب صلاحيات المستخدم عند الدخول
        public static DataTable LogIn_GetPermissions(int Us_ID)
        {
            return dbHelper.ExecuteSelectQuery("LogIn_GetPermissions", command => Prm_GetPermissions(Us_ID, command)) ?? new DataTable();
        }
        private static void Prm_GetPermissions(int Us_ID, SqlCommand command)//@@@
        {
            command.Parameters.Add("@Us_ID", SqlDbType.Int).Value = Us_ID;
        }

        // تحديث كلمة مرور المستخدم مع رسالة راجعة من الإجراء المخزن
        public static string LogIn_UsersUpdatPass(int IDUser, string pass_Word)//@@@
        {
            return dbHelper.ExecuteNonQueryWithLogging("LogIn_UsersUpdatPass", command =>
            {
                command.Parameters.Add("@IDUser", SqlDbType.Int).Value = IDUser;
                command.Parameters.Add("@pass_Word", SqlDbType.NVarChar).Value = pass_Word;
            }, expectMessageOutput: true);
        }


        #endregion

        #region *******  Main Methods ************
        // جلب قائمة الموردين بدون شروط
        public static DataTable MainAcc_GetAccounts()//@@@
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("MainAcc_GetAccounts");
            return result ?? new DataTable(); // إذا كانت null نُرجع DataTable فارغ
        }

        // جلب الفواتير السابقة حسب النوع
        public static DataTable NewInvoice_GetOldInvoicesByType(int InvType_ID)
        {
            
            DataTable? result = dbHelper.ExecuteSelectQuery("NewInvoice_GetOldInvoicesByType", cmd =>
            {
                cmd.Parameters.Add("@InvType_ID", SqlDbType.Int ).Value = InvType_ID;
            });

            return result ?? new DataTable(); // تأمين ضد null
        }

        /// دالة لجلب جميع المنتجات بدون أي فلترة.
        public static DataTable ProductSearch_GetAll()
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("ProductSearch_GetAll");
            return result ?? new DataTable(); // إذا كانت null نُرجع DataTable فارغ

        }

        public static DataTable MainAcc_GetAccounts(int ParentAccID)
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("MainAcc_GetAccounts", cmd =>
            {
                cmd.Parameters.Add("@ParentAccID", SqlDbType.Int).Value = ParentAccID;
            });

            return result ?? new DataTable(); // تأمين ضد null
          
        }
        #endregion 

        #region ************  ProductModify *****************
        // جلب معرف المنتج من خلال كود الصنف
        public static DataTable Product_GetIDByCode(string ProductCode)
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Product_GetIDByCode", cmd =>
            {
                cmd.Parameters.Add("@ProductCode", SqlDbType.VarChar).Value = ProductCode;
            });

            return result ?? new DataTable(); // تأمين ضد null
        }

        //جلب جميع الاصناف
        public static DataTable Product_GetAll()
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Product_GetAll");
            return result ?? new DataTable(); // إذا كانت null نُرجع DataTable فارغ
        }

        // جلب قائمة وحدات القياس
        public static DataTable ProductGetUnits()//@@@
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("ProductGetUnits");
            return result ?? new DataTable(); // إذا كانت null نُرجع DataTable فارغ
        }

        // جلب بيانات صنف  محدد بالمعرفID_Product
        public static DataTable Product_GetOneID(int ID_Product)
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Product_GetOneID", cmd =>
            {
                cmd.Parameters.Add("@ID_Product", SqlDbType.Int).Value = ID_Product;
            });

            return result ?? new DataTable();
        }


        // جلب قطع صنف  محدد بالمعرفID_Product
        public static DataTable Product_GetPiecesByProductID(int ID_Product)
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Product_GetPiecesByProductID", cmd =>
            {
                cmd.Parameters.Add("@ID_Product", SqlDbType.Int).Value = ID_Product;
            });

            return result ?? new DataTable();
        }


        //
        //دالة لحذف كل قطع صنف وكل حركاته
        public static string Product_DeletePiecesAndMovementsByProductID(
            int ID_Product,
            string adminPassword,
            int ExecutedByID,
            string reason)
        {
            return dbHelper.ExecuteStoredProcedureWithOutputMessage("Product_DeletePiecesAndMovementsByProductID", command =>
            {
                command.Parameters.Add("@ID_Product", SqlDbType.Int).Value = ID_Product;
                command.Parameters.Add("@AdminPassword", SqlDbType.NVarChar, 100).Value = adminPassword;
                command.Parameters.Add("@ExecutedByID", SqlDbType.Int).Value = ExecutedByID;
                command.Parameters.Add("@Reason", SqlDbType.NVarChar, 500).Value = reason;
            });
        }


        //
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
            DataTable? result = dbHelper.ExecuteSelectQuery("Product_GetMaxID");
            return result ?? new DataTable(); // إذا كانت null نُرجع DataTable فارغ
        }

        // جلب اعلى قيمة كود الصنف
        public static DataTable Product_GetMaxCode()
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Product_GetMaxCode");
            return result ?? new DataTable();
        }


        //جلب الاصناف التى تم تحديدها فى الداتا تيبل
        public static DataTable Product_GetSelected(DataTable ProductIDs)
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Product_GetSelected", cmd =>
            {
                var param = cmd.Parameters.AddWithValue("@ProductIDs", ProductIDs);
                param.SqlDbType = SqlDbType.Structured;
                param.TypeName = "dbo.ProductIDList"; // تأكد أن اسم النوع متطابق مع SQL Server
            });

            return result ?? new DataTable();
        }


        //دالة اضافة  صنف جديد
        public static int Product_InsertItem(string ProdName, int UnitID, float B_Price, float U_Price,
                                           string ProdCodeOnSuplier, float MinLenth, float MinStock,
                                           int Category_id, int SuplierID, string NoteProduct, string PicProduct)
        {
            return dbHelper.ExecuteScalar<int>("Product_InsertItem", cmd =>
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
        public static int Product_UpdateItem(
            int ID_Product,
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
            string NoteProduct,
            string PicProduct)
        {
            return dbHelper.ExecuteScalar<int>("Product_UpdateItem", cmd =>
            {
                cmd.Parameters.Add("@ID_Product", SqlDbType.Int).Value = ID_Product;
                cmd.Parameters.Add("@ProdName", SqlDbType.NVarChar, 50).Value = ProdName;
                cmd.Parameters.Add("@UnitID", SqlDbType.Int).Value = UnitID;
                cmd.Parameters.Add("@B_Price", SqlDbType.Real).Value = B_Price;
                cmd.Parameters.Add("@U_Price", SqlDbType.Real).Value = U_Price;
                cmd.Parameters.Add("@HiddinProd", SqlDbType.Bit).Value = HiddinProd;
                cmd.Parameters.Add("@ProdCodeOnSuplier", SqlDbType.NVarChar, 50).Value = ProdCodeOnSuplier;
                cmd.Parameters.Add("@MinLenth", SqlDbType.Real).Value = MinLenth;
                cmd.Parameters.Add("@MinStock", SqlDbType.Real).Value = MinStock;
                cmd.Parameters.Add("@Category_id", SqlDbType.Int).Value = Category_id;
                cmd.Parameters.Add("@SuplierID", SqlDbType.Int).Value = SuplierID;
                cmd.Parameters.Add("@NoteProduct", SqlDbType.NVarChar, 50).Value = NoteProduct;
                cmd.Parameters.Add("@PicProduct", SqlDbType.NVarChar, 200).Value = PicProduct ?? "";
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
                throw new ArgumentException("DataTable must contain a 'ProductID' column.");

            return dbHelper.ExecuteNonQuery("Products_UpdateSelectedItems", cmd =>
            {
                var tvpParam = new SqlParameter("@ProductIDs", SqlDbType.Structured)
                {
                    TypeName = "dbo.ProductIDList",
                    Value = productIdTable
                };
                cmd.Parameters.Add(tvpParam);

                cmd.Parameters.AddWithValue("@U_Price", uPrice ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@B_Price", bPrice ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SuplierID", supplierId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Category_id", categoryId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UnitID", unitId ?? (object)DBNull.Value);
            });
        }

        // جلب قائمة الموردين بدون شروط
        public static DataTable Accounts_GetSupplier()//@@@
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Accounts_GetSupplier");
            return result ?? new DataTable();
        }

        // جلب جميع التصنيفات على شكل شجرة  ###
        public static DataTable Categories_GetAll()//@@
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Categories_GetAll");
            return result ?? new DataTable();
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
        //استدعاء تفاصيل الصورة للعرض المفصل
        public static DataTable Product_GetPicDetailsByID(int idProduct)
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Product_GetPicDetailsByID", cmd =>
            {
                var param = cmd.Parameters.AddWithValue("@ID_Product", idProduct);
            });

            return result ?? new DataTable();
        }


        // دالة جلب بيانات جدول الباركود الى الداتا جريد فيو
        public static DataTable BarcodesGet()//@@@
        {
            return dbHelper.ExecuteSelectQuery("BarcodesGet", command => { }) ?? new DataTable();
        }


        //دالة اضافة كود لطباعة الباركود
        public static int sp_InsertBarcodesToPrint(int productId, int count)
        {
            return dbHelper.ExecuteScalar<int>("sp_InsertBarcodesToPrint", cmd =>
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

        public static DataTable sp_GetBarcodesToPrint()
        {
            return dbHelper.ExecuteSelectQuery("sp_GetBarcodesToPrint", command => { }) ?? new DataTable();
        }

        // تمت المراجعة لاستدعاء قائمة التقارير للاصناف    ### 
        public static DataTable RepMenu_Products(bool ForItems, bool ForItemsGroup)
        {
            return dbHelper.ExecuteSelectQuery("RepMenu_Products", cmd =>
            {
                cmd.Parameters.Add("@ForItems", SqlDbType.Bit).Value = ForItems;
                cmd.Parameters.Add("@ForItemsGroup", SqlDbType.Bit).Value = ForItemsGroup;
            }) ?? new DataTable();
        }


        /// حفظ بيانات التقرير الافتراضي (الطابعة، المخزن، والفترة) وإرجاع رسالة من الإجراء المخزن
        public static string GenralData_SaveDefRepData(
    DateTime DefStartPeriod,
    DateTime DefEndPeriod,
    string DefaultPrinter,
    string DefaultWarehouse,
    string DefEndRdoChecked)
        {
            return dbHelper.ExecuteNonQueryWithLogging(
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


        #endregion


        #region MainViewer for reports @#@
        // دالة استدعاء تقرير كارت صنف رقم 0    @@@ 
        public static DataTable GetProductCard_Set(int ID_Product, int Warehouse_Id, DateTime StartDate, DateTime EndDate)//@@@
        {
            return dbHelper.ExecuteSelectQuery("rpt_GetProductCard_Set", cmd =>
            {
                cmd.Parameters.Add("@ID_Product", SqlDbType.Int).Value = ID_Product;
                cmd.Parameters.Add("@Warehouse_Id", SqlDbType.Int).Value = Warehouse_Id;
                cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = StartDate;
                cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value = EndDate;
            }) ?? new DataTable();
        }

        public static DataTable GetProductCard()//@@@
        {
            return dbHelper.ExecuteSelectQuery("rpt_GetProductCard", command => { }) ?? new DataTable();
        }

        // دالة استدعاء تقرير تفصيلى لحركة صنف     @@@  مشتريات او مبيعات
        public static DataTable rpt_GetDetailedMovementsReport(int ID_Prod, int Warehouse_Id, DateTime StartDate, DateTime EndDate, string WayMove)//@@@
        {
            return dbHelper.ExecuteSelectQuery("rpt_GetDetailedMovementsReport", cmd =>
            {
                cmd.Parameters.Add("@ID_Prod", SqlDbType.Int).Value = ID_Prod;
                cmd.Parameters.Add("@Warehouse_Id", SqlDbType.Int).Value = Warehouse_Id;
                cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = StartDate;
                cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value = EndDate;
                cmd.Parameters.Add("@WayMove", SqlDbType.NVarChar).Value = WayMove;
            }) ?? new DataTable();
        }

        // جلب كشف أرصدة الحسابات الفرعية المرتبطة بحساب رئيسي
        public static DataTable Account_Balance(int ID_TopAcc)
        {
            return dbHelper.ExecuteSelectQuery("Account_Balance", cmd =>
            {
                cmd.Parameters.Add("@ID_TopAcc", SqlDbType.Int).Value = ID_TopAcc;
            }) ?? new DataTable();
        }

        // جلب تقرير المصروف اليومي لحساب فرعي
        public static DataTable DailyFee_Account(int AccID)
        {
            return dbHelper.ExecuteSelectQuery("DailyFee_Account", cmd =>
            {
                cmd.Parameters.Add("@AccID", SqlDbType.Int).Value = AccID;
            }) ?? new DataTable();
        }

        // جلب تقرير المصروف اليومي لحساب معين بين تاريخين
        public static DataTable DailyFee_AccountByDate(int AccID, DateTime StartDate, DateTime EndDate)
        {
            return dbHelper.ExecuteSelectQuery("DailyFee_AccountByDate", cmd =>
            {
                cmd.Parameters.Add("@AccID", SqlDbType.Int).Value = AccID;
                cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
            }) ?? new DataTable();
        }

        //public static DataTable GenralData_GetDefRepData()//@@@
        //{
        //    return dbHelper.ExecuteSelectQuery("GenralData_GetDefRepData", command => { }) ?? new DataTable();
        //}


        //// جلب تاريخ بداية الحسابات (بدون معاملات)
        //public static DataTable GenralData_GetStartAccountsDate()
        //{
        //    return dbHelper.ExecuteSelectQuery("GenralData_GetStartAccountsDate", command => { }) ?? new DataTable();
        //}

        // جلب الفروع الرئيسية (المخازن) المعروضة في الشاشة (بدون معاملات)
        public static DataTable Warehouse_GetAll()//@@@
        {
            return dbHelper.ExecuteSelectQuery("Warehouse_GetAll", command => { }) ?? new DataTable();
        }
        
        // جلب حركة المنتج حسب المعرف
        public static DataTable DailyFee_GetJournalEntry(int Bill_No, int InvTypeID)
        {
            return dbHelper.ExecuteSelectQuery("DailyFee_GetJournalEntry", cmd =>
            {
                cmd.Parameters.Add("@Bill_No", SqlDbType.Int).Value = Bill_No;
                cmd.Parameters.Add("@InvTypeID", SqlDbType.Int).Value = InvTypeID;
            }) ?? new DataTable(); // تجنب إرجاع null
        }
        #endregion



    }
}
