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

        //عمليات الالفروع والمخازن
        // جلب الفروع الرئيسية (المخازن) المعروضة في الشاشة (بدون معاملات)
        public static DataTable Warehouse_GetAll()//@@@
        {
            return dbHelper.ExecuteSelectQuery("Warehouse_GetAll", command => { }) ?? new DataTable();
        }
        // اضافة فرع
        public static string Warehouse_Add(string warehouseName, int userId)
        {
            return dbHelper.ExecuteStoredProcedureWithOutputMessage("Warehouse_Add", cmd =>
            {
                cmd.Parameters.Add("@WarehouseName", SqlDbType.NVarChar).Value = warehouseName;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
            });
        }//Microsoft.Data.SqlClient.SqlException: 'Cannot insert explicit value for identity column in table 'Warehouse' when IDENTITY_INSERT is set to OFF.'
//ما هذه الرسالة

        public static string Warehouse_UpdateName(int warehouseId, string newName, int userId)
        {
            return dbHelper.ExecuteStoredProcedureWithOutputMessage("Warehouse_UpdateName", cmd =>
            {
                cmd.Parameters.Add("@WarehouseId", SqlDbType.Int).Value = warehouseId;
                cmd.Parameters.Add("@NewName", SqlDbType.NVarChar).Value = newName;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
            });
        }

        public static string Warehouse_Delete(int warehouseId, int userId)
        {
            return dbHelper.ExecuteStoredProcedureWithOutputMessage("Warehouse_Delete", cmd =>
            {
                cmd.Parameters.Add("@WarehouseId", SqlDbType.Int).Value = warehouseId;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
            });
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

        #region ********* Invoice    *********************

        //تحديث بيانات قاعدة البيانات
        public static string UpdateAllBalances()//@@@ 
        {
            string result = dbHelper.ExecuteNonQueryNoParamsWithMessage("A_UpdateAllDataBase", expectMessageOutput: true);

            if (!string.IsNullOrEmpty(result))
                return result;

            return "تم التحديث بنجاح";
        }

        //ادراج تفاصيل فاتورة
        public static string InvoiceDetails_Insert(
            int TypeInvID,
            int Inv_ID_fk,
            int PieceID_fk,
            float PriceMove,
            float Amount,
            float TotalRow,
            float GemDisVal,
            float ComitionVal,
            float NetRow,
            int OldInvID

            )
        {
            // تنفيذ الإجراء المخزن مع تمكين انتظار الرسائل المرجعية
            string result = dbHelper.ExecuteNonQueryWithLogging("NewInvoice_InsertDetails", cmd =>
            {
                cmd.Parameters.Add("@TypeInvID", SqlDbType.Int).Value = TypeInvID;
                cmd.Parameters.Add("@Inv_ID_fk", SqlDbType.Int).Value = Inv_ID_fk;
                cmd.Parameters.Add("@PieceID_fk", SqlDbType.Int).Value = PieceID_fk;
                cmd.Parameters.Add("@PriceMove", SqlDbType.Real).Value = PriceMove;
                cmd.Parameters.Add("@Amount", SqlDbType.Real).Value = Amount;
                cmd.Parameters.Add("@TotalRow", SqlDbType.Real).Value = TotalRow;
                cmd.Parameters.Add("@GemDisVal", SqlDbType.Real).Value = GemDisVal;
                cmd.Parameters.Add("@ComitionVal", SqlDbType.Real).Value = ComitionVal;
                cmd.Parameters.Add("@NetRow", SqlDbType.Real).Value = NetRow;
                cmd.Parameters.Add("@OldInvID", SqlDbType.Int).Value = OldInvID;

                //@

            }, expectMessageOutput: false);

            // إذا كانت هناك رسالة مرجعة من الإجراء المخزن، يتم إرجاعها مباشرة
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            // إذا لم يتم إرجاع أي رسالة، يتم إرجاع رسالة نجاح افتراضية
            return "تم إدخال القطعة بنجاح";
        }


        // جلب بيانات المنتج حسب المعرف
        public static DataTable Item_GetProductByCode(string ProductCode, out string message)
        {
            var result = dbHelper.ExecuteSelectQueryWithMessage("Item_GetProductByCode", cmd =>
            {
                cmd.Parameters.Add("@ProductCode", SqlDbType.NVarChar).Value = ProductCode;
            }, out message, expectMessageOutput: true);

            // إرجاع جدول فارغ إذا حدثت مشكلة أو كانت النتيجة null
            return result ?? new DataTable();
        }

        //احضار حسابات التى تعرض فى الفاتورة
        public static DataTable NewInvoice_GetAcc(string accountIDsCsv)
        {
            var result = dbHelper.ExecuteSelectQuery("NewInvoice_GetAcc", cmd =>
            {
                cmd.Parameters.AddWithValue("@IDs", accountIDsCsv);
            });

            return result ?? new DataTable(); // ✅ إرجاع جدول فارغ إذا كانت النتيجة null
        }

        // جلب فاتورة بمسلسلها حسب النوع@
        public static DataTable NewInvoice_GetInvoiceByTypeAndCounter(int InvType_ID, int Inv_Counter, out string? message)
        {
            return dbHelper.ExecuteSelectQueryFlexible("NewInvoice_GetInvoiceByTypeAndCounter", cmd =>
            {
                cmd.Parameters.Add("@InvType_ID", SqlDbType.Int).Value = InvType_ID;
                cmd.Parameters.Add("@Inv_Counter", SqlDbType.Int).Value = Inv_Counter;
            }, out message, expectMessageOutput: true);
        }


        // جلب تفاصيل الفواتير حسب الكود
        public static DataTable NewInvoice_GetInvoiceDetails(int Inv_ID)
        {
            return dbHelper.ExecuteSelectQueryFlexible("NewInvoice_GetInvoiceDetails", cmd =>
            {
                cmd.Parameters.Add("@Inv_ID", SqlDbType.Int).Value = Inv_ID;
            }, out _); // لا نهتم بالرسالة هنا
        }


        // جلب الفواتير حسب النوع
        public static DataTable NewInvoice_GetInvoicesByType(int InvType_ID)
        {
            return dbHelper.ExecuteSelectQueryFlexible("NewInvoice_GetInvoicesByType", cmd =>
            {
                cmd.Parameters.Add("@InvType_ID", SqlDbType.Int).Value = InvType_ID;
            }, out _); // لا نهتم بالرسالة هنا

        }


        //جلب كود جديد للفاتورة
        public static int NewInvoice_GetNewID()
        {
            DataTable dt = dbHelper.ExecuteSelectQueryFlexible("NewInvoice_GetNewID", null, out _);

            if (dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0]["NewInvID"]);
            else
                return 1; // احتياطي في حال عدم وجود بيانات
        }



        //جلب عداد جديد للفاتورة
        public static string NewInvoice_GetNewCounter(int invTypeID)
        {
            DataTable dt = dbHelper.ExecuteSelectQueryFlexible("NewInvoice_GetNewCounter", cmd =>
            {
                cmd.Parameters.AddWithValue("@InvType_ID", invTypeID);
            }, out _); // تجاهل الرسالة لأنها غير مطلوبة

            if (dt.Rows.Count > 0 && dt.Rows[0]["New_Counter"] != DBNull.Value)
                return dt.Rows[0]["New_Counter"]?.ToString() ?? "0000-XX-0000001";
            else
                return "0000-XX-0000001"; // قيمة افتراضية احتياطية
        }


        

        // حفظ الفاتورة الدائم او المؤقت
        public static bool NewInvoice_InsertOrUpdate(
            int invID,
            string invCounter,
            int? invType_ID,
            DateTime? invDate,
            int? seller_ID,
            int? user_ID,
            int? acc_ID,
            float? totalValue,
            float? taxVal,
            float? totalValueAfterTax,
            float? discount,
            float? valueAdded,
            float? netTotal,
            float? payment_Cash,
            float? payment_Electronic,
            float? payment_BankCheck,
            string payment_Note,
            float? remainingOnAcc,
            bool? isReturnable,
            string noteInvoice,
            string saved,
            int Warehouse_Id,
            out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithLogging(
                "NewInvoice_InsertOrUpdate",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Inv_ID", invID);//7
                    cmd.Parameters.AddWithValue("@Inv_Counter", invCounter ?? (object)DBNull.Value);//=1
                    cmd.Parameters.AddWithValue("@InvType_ID", invType_ID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Inv_Date", invDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Seller_ID", seller_ID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@User_ID", user_ID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Acc_ID", acc_ID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TotalValue", totalValue ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TaxVal", taxVal ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TotalValueAfterTax", totalValueAfterTax ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Discount", discount ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ValueAdded", valueAdded ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NetTotal", netTotal ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Payment_Cash", payment_Cash ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Payment_Electronic", payment_Electronic ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Payment_BankCheck", payment_BankCheck ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Payment_Note", payment_Note ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RemainingOnAcc", remainingOnAcc ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsReturnable", isReturnable ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoteInvoice", noteInvoice ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Saved", saved ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Warehouse_Id", Warehouse_Id);
                },
                expectMessageOutput: false // إذا لم يكن الإجراء يرجع OUTPUT message
            );

            // في حال أردت لاحقًا أن ترجع SP رسالة، غير expectMessageOutput إلى true
            return true; // أو يمكنك تعديل المنطق لاحقاً حسب رسالة الإجراء
        }



        //ادراج تفاصيل فاتورة
        public static string NewInvoice_UpdateInvoiceDetail(
            int serInvDetail,
            float PriceMove,
            float Amount,
            float GemDisVal
            )
        {
            // تنفيذ الإجراء المخزن مع تمكين انتظار الرسائل المرجعية
            string result = dbHelper.ExecuteNonQueryWithLogging("NewInvoice_UpdateInvoiceDetail", cmd =>
            {
                cmd.Parameters.Add("@serInvDetail", SqlDbType.Int).Value = serInvDetail;
                cmd.Parameters.Add("@PriceMove", SqlDbType.Real).Value = PriceMove;
                cmd.Parameters.Add("@Amount", SqlDbType.Real).Value = Amount;
                cmd.Parameters.Add("@GemDisVal", SqlDbType.Real).Value = GemDisVal;
            }, expectMessageOutput: false);

            // إذا كانت هناك رسالة مرجعة من الإجراء المخزن، يتم إرجاعها مباشرة
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            // إذا لم يتم إرجاع أي رسالة، يتم إرجاع رسالة نجاح افتراضية
            return "تم إدخال القطعة بنجاح";
        }



        //  الحصول على معرف القطعة (Piece_ID) الجديدة بالتحديد.
        public static int Product_CreateNewPiece(int prodID)
        {
            DataTable dt = dbHelper.ExecuteSelectQueryFlexible("Product_CreateNewPiece", cmd =>
            {
                cmd.Parameters.Add("@ID_Prod", SqlDbType.Int).Value = prodID;
            }, out _); // لا نحتاج إلى رسالة

            if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                return Convert.ToInt32(dt.Rows[0][0]);
            else
                return 0; // قيمة احتياطية في حال عدم وجود نتائج
        }

        // جلب القطع التابعة لمنتج[]
        public static DataTable Product_GetOrCreatePieces(int ID_Prod)
        {
            return dbHelper.ExecuteSelectQueryFlexible("Product_GetOrCreatePieces", cmd =>
            {
                cmd.Parameters.Add("@ID_Prod", SqlDbType.Int).Value = ID_Prod;
            }, out _); // تجاهل الرسالة لأنها غير مطلوبة
        }



        //  تُرجع القطعة الوحيدة الموجودة أو تنشئها إن لم تكن موجودة.
        public static DataTable Product_GetOrCreate_DefaultPiece(int ID_Prod)
        {

            return dbHelper.ExecuteSelectQueryFlexible("Product_GetOrCreate_DefaultPiece", cmd =>
            {
                cmd.Parameters.Add("@ID_Prod", SqlDbType.Int).Value = ID_Prod;
            }, out _); // تجاهل الرسالة لأنها غير مطلوبة

        }

        // جلب رقم جديد للحساب 
        public static DataTable MainAcc_GetNewID()
        {
            return dbHelper.ExecuteSelectQueryFlexible("MainAcc_GetNewID", null, out _);
        }


        //جلب رقم جديد للتصنيف الفرعى
        public static DataTable MainAcc_GetNewIDForCategory()//@@@
        {
            return dbHelper.ExecuteSelectQueryFlexible("MainAcc_GetNewIDForCategory", null, out _);
        }

        //دالة ادراج او تعديل حساب 
        public static bool MainAcc_UpdateOrInsert(
           int accID,
           int? parentAccID,
           string accName,
           bool isHidden,
           float? fixedAssetsValue,
           float? depreciationRateAnnually,
           int? fixedAssetsAge,
           float? annuallyInstallment,
           float? monthlyInstallment,
           bool? isEndedFixedAssets,
           DateTime? fixedAssetsEndDate,
           string firstPhon,
           string antherPhon,
           string accNote,
           string clientEmail,
           string clientAddress,
           out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithLogging(
                "MainAcc_UpdateOrInsert",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@AccID", accID);
                    cmd.Parameters.AddWithValue("@ParentAccID", parentAccID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccName", accName);
                    cmd.Parameters.AddWithValue("@IsHidden", isHidden);
                    cmd.Parameters.AddWithValue("@FixedAssetsValue", fixedAssetsValue ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DepreciationRateAnnually", depreciationRateAnnually ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FixedAssetsAge", fixedAssetsAge ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AnnuallyInstallment", annuallyInstallment ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MonthlyInstallment", monthlyInstallment ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsEndedFixedAssets", isEndedFixedAssets ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FixedAssetsEndDate", fixedAssetsEndDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FirstPhon", string.IsNullOrWhiteSpace(firstPhon) ? (object)DBNull.Value : firstPhon);
                    cmd.Parameters.AddWithValue("@AntherPhon", string.IsNullOrWhiteSpace(antherPhon) ? (object)DBNull.Value : antherPhon);
                    cmd.Parameters.AddWithValue("@AccNote", string.IsNullOrWhiteSpace(accNote) ? (object)DBNull.Value : accNote);
                    cmd.Parameters.AddWithValue("@ClientEmail", string.IsNullOrWhiteSpace(clientEmail) ? (object)DBNull.Value : clientEmail);
                    cmd.Parameters.AddWithValue("@ClientAddress", string.IsNullOrWhiteSpace(clientAddress) ? (object)DBNull.Value : clientAddress);
                },
                expectMessageOutput: true
            );

            // اعتبر أن النجاح إذا الرسالة تبدأ بـ "تم"
            return resultMessage.StartsWith("تم");
        }




        /*وهذه بدون رسالة بالدالة الجديدة*/






        //public static bool NewInvoice_InsertDetails(
        //int accID,
        //int? parentAccID,
        //string accName,
        //out string resultMessage)
        //{
        //    resultMessage = dbHelper.ExecuteNonQueryWithLogging(
        //        "NewInvoice_InsertDetails",
        //        cmd =>
        //        {
        //            cmd.Parameters.AddWithValue("@AccID", accID);
        //            cmd.Parameters.AddWithValue("@ParentAccID", parentAccID ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@AccName", accName);
        //        },
        //        expectMessageOutput: true
        //    );

        //    // اعتبر أن النجاح إذا الرسالة تبدأ بـ "تم"
        //    return resultMessage.StartsWith("تم");
        //}

        ////تنظيف الفواتير الفارغة
        //public static bool NewInvoice_DeleteEmptyInvoicesOnExit()
        //{
        //    try
        //    {
        //        dbHelper.ExecuteNonQueryNoParamsWithMessage("NewInvoice_DeleteEmptyInvoicesOnExit");
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}











        /*كيف يكون ضبط هذه على الدالة الجديدة بدون رسائل*/
        #endregion

    }
}
