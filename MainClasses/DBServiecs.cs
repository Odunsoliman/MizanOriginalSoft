using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace MizanOriginalSoft.MainClasses
{
    internal class DBServiecs
    {
        #region ********************  Log In Form ********************
        //تحديث بيانات قاعدة البيانات
        public static string A_UpdateAllDataBase()//@@@ 
        {
            string result = dbHelper.ExecuteNonQueryNoParamsWithMessage("A_UpdateAllDataBase", expectMessageOutput: true);
            if (!string.IsNullOrEmpty(result))
                return result;

            return "تم التحديث بنجاح";
        }


        #endregion
    
        #region ********** Users ****************************

        // احضار كل المستخدمين المتاحين فقط 

        public static DataTable User_GetActiv()
        {
            return dbHelper.ExecuteSelectQuery("User_GetActiv", command =>
            {

            }) ?? new DataTable();
        }

        //احضار كل المستخدمين
        public static DataTable User_GetAll()
        {
            return dbHelper.ExecuteSelectQuery("User_GetAll", command =>
            {
               
            }) ?? new DataTable();
        }
     
        // اختبار وجود مستخدم وكلمة مروره صحيحة
        public static DataTable User_Varify(string username, string password)
        {
            return dbHelper.ExecuteSelectQuery("User_Varify", command => Prm_User_Varify(username, password, command)) ?? new DataTable();
        }
        private static void Prm_User_Varify(string username, string password, SqlCommand command)
        {
            command.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;
            command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password;
        }
        // جلب بيانات مستخدم واحد
        public static DataTable User_GetOne(int IDUser)
        {
            return dbHelper.ExecuteSelectQuery("User_GetOne", command =>
            {
                command.Parameters.Add("@IDUser", SqlDbType.Int).Value = IDUser;
            }) ?? new DataTable();
        }

        // تنفيذ الإجراء: User_ChangePassword
        public static string User_ChangePassword(int userId, string newPassword)
        {
            return dbHelper.ExecuteNonQueryWithLogging("User_ChangePassword", command =>
            {
                command.Parameters.Add("@IDUser", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@pass_Word", SqlDbType.NVarChar).Value = newPassword;
            }, expectMessageOutput: true);
        }


        // إضافة مستخدم جديد بكلمة مرور افتراضية "00"
        public static string User_Add(string username, string fullName, int createdByUserId)
        {
            return dbHelper.ExecuteNonQueryWithLogging("User_Add", command =>
            {
                command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = username;
                command.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = fullName;
                command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = createdByUserId;

                var paramNewId = new SqlParameter("@NewUserID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(paramNewId);
            }, expectMessageOutput: true);
        }


        // تحديث بيانات المستخدم
        public static string User_Update(int userId, string username, string fullName, bool isAdmin, bool isActive, int modifiedBy)
        {
            return dbHelper.ExecuteNonQueryWithLogging("User_Update", command =>
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = username;
                command.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = fullName;
                command.Parameters.Add("@IsAdmin", SqlDbType.Bit).Value = isAdmin;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive;
                command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = modifiedBy;
            }, expectMessageOutput: true);
        }


        // إعادة تعيين كلمة المرور إلى "00"
        public static string User_ResetPassword(int userId)
        {
            return dbHelper.ExecuteNonQueryWithLogging("User_ResetPassword", command =>
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
            }, expectMessageOutput: true);
        }

        // حذف المستخدم في حال لا توجد له معاملات
        public static string User_DeleteIfAllowed(int userId)
        {
            return dbHelper.ExecuteNonQueryWithLogging("User_DeleteIfAllowed", command =>
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
            }, expectMessageOutput: true);
        }






        #endregion

        #region ******** Permissions *******************

        // تنفيذ الإجراء: Permission_SetForUser
        public static string Permission_SetForUser(int userId, int permissionId, bool isAllowed, bool canAdd, bool canEdit, bool canDelete, int? warehouseId)
        {
            return dbHelper.ExecuteNonQueryWithLogging("Permission_SetForUser", command =>
            {
                command.Parameters.Add("@Us_ID", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@PermissionID", SqlDbType.Int).Value = permissionId;
                command.Parameters.Add("@IsAllowed", SqlDbType.Bit).Value = isAllowed;
                command.Parameters.Add("@CanAdd", SqlDbType.Bit).Value = canAdd;
                command.Parameters.Add("@CanEdit", SqlDbType.Bit).Value = canEdit;
                command.Parameters.Add("@CanDelete", SqlDbType.Bit).Value = canDelete;
                command.Parameters.Add("@WarehouseID", SqlDbType.Int).Value = (object?)warehouseId ?? DBNull.Value;
            }, expectMessageOutput: false );
        }

        // تنفيذ الإجراء: Permission_DeleteForUser
        public static string Permission_DeleteForUser(int userId, int permissionId)
        {
            return dbHelper.ExecuteNonQueryWithLogging("Permission_DeleteForUser", command =>
            {
                command.Parameters.Add("@Us_ID", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@PermissionID", SqlDbType.Int).Value = permissionId;
            }, expectMessageOutput: true);
        }

        // تنفيذ الإجراء: Permission_GrantAllToUser
        public static string Permission_GrantAllToUser(int userId)
        {
            return dbHelper.ExecuteNonQueryWithLogging("Permission_GrantAllToUser", command =>
            {
                command.Parameters.Add("@Us_ID", SqlDbType.Int).Value = userId;
            }, expectMessageOutput: true);
        }

        // تنفيذ الإجراء: Permission_RevokeAllFromUser
        public static string Permission_RevokeAllFromUser(int userId)
        {
            return dbHelper.ExecuteNonQueryWithLogging("Permission_RevokeAllFromUser", command =>
            {
                command.Parameters.Add("@Us_ID", SqlDbType.Int).Value = userId;
            }, expectMessageOutput: true);
        }

        // تنفيذ الإجراء: Permission_GetByUser
        public static DataTable Permission_GetByUser(int userId, int warehouseId)
        {
            return dbHelper.ExecuteSelectQuery("Permission_GetByUser", command =>
            {
                command.Parameters.Add("@Us_ID", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@WarehouseID", SqlDbType.Int).Value = warehouseId;
            }) ?? new DataTable();
        }

        // تنفيذ الإجراء: Permission_GetAllDefinedPermissions
        public static DataTable Permission_GetAllDefinedPermissions()
        {
            return dbHelper.ExecuteSelectQuery("Permission_GetAllDefinedPermissions", command => { }) ?? new DataTable();
        }


        public static DataTable Permission_GetFullForUser(int userId)
        {
            return dbHelper.ExecuteSelectQuery("Permission_GetFullForUser", command =>
            {
                command.Parameters.Add("@Us_ID", SqlDbType.Int).Value = userId;
            }) ?? new DataTable();
        }

        #endregion

        #region *******  Main Methods ************
        // جلب قائمة الموردين بدون شروط
        public static DataTable MainAcc_GetAccounts()//@@@
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("MainAcc_GetAccounts");
            return result ?? new DataTable(); // إذا كانت null نُرجع DataTable فارغ
        }

        // جلب قائمة الاباء الهرميين
        public static DataTable MainAcc_GetHierarchy()//@@@
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("MainAcc_GetHierarchy");
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

        public static DataTable MainAcc_GetParentAccounts(string accountType)
        {
            DataTable? result = dbHelper.ExecuteSelectQuery(
                "MainAcc_GetParentAccounts",
                cmd =>
                {
                    cmd.CommandType = CommandType.StoredProcedure; // تأكد أن النوع SP
                    cmd.Parameters.Add("@AccountType", SqlDbType.NVarChar, 50).Value = accountType;
                });

            return result ?? new DataTable(); // fallback في حالة null
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


        // جلب الأصناف المحددة
        public static DataTable Products_GetByIDs(List<int> ids)
        {
            // نحول القائمة إلى نص مفصول بفواصل: "101,102,103"
            string idList = string.Join(",", ids);

            // استدعاء الإجراء مع تمرير الباراميتر بالطريقة الصحيحة
            DataTable? result = dbHelper.ExecuteSelectQuery("Products_GetByIDs", cmd =>
            {
                cmd.Parameters.AddWithValue("@IDs", idList);
            });

            return result ?? new DataTable();
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
        public static int Product_InsertItem(string ProdName, int UnitID, float B_Price, float U_Price,float D_Price,
                                           string ProdCodeOnSuplier, float MinLenth, float MinStock,
                                           int Category_id, int SuplierID, string NoteProduct, string PicProduct)
        {
            return dbHelper.ExecuteScalar<int>("Product_InsertItem", cmd =>
            {
                cmd.Parameters.Add("@ProdName", SqlDbType.NVarChar, 60).Value = ProdName;
                cmd.Parameters.Add("@UnitID", SqlDbType.Int).Value = UnitID;
                cmd.Parameters.Add("@B_Price", SqlDbType.Real).Value = B_Price;
                cmd.Parameters.Add("@U_Price", SqlDbType.Real).Value = U_Price;
                cmd.Parameters.Add("@D_Price", SqlDbType.Real).Value = D_Price;
                cmd.Parameters.Add("@ProdCodeOnSuplier", SqlDbType.NVarChar, 50).Value = ProdCodeOnSuplier;
                cmd.Parameters.Add("@MinLenth", SqlDbType.Real).Value = MinLenth;
                cmd.Parameters.Add("@MinStock", SqlDbType.Real).Value = MinStock;
                cmd.Parameters.Add("@Category_id", SqlDbType.Int).Value = Category_id;
                cmd.Parameters.Add("@SuplierID", SqlDbType.Int).Value = SuplierID;
                cmd.Parameters.Add("@NoteProduct", SqlDbType.NVarChar, 50).Value = NoteProduct;
                cmd.Parameters.Add("@PicProduct", SqlDbType.NVarChar, 50).Value = PicProduct;
            });
        }


        //دالة اضافة صورة صنف جديدة
        public static int Product_AddPhoto( int ProductID,string ImagePath, bool IsDefault)
        {
            return dbHelper.ExecuteScalar<int>("Product_AddPhoto", cmd =>
            {
                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = ProductID;
                cmd.Parameters.Add("@ImagePath", SqlDbType.NVarChar).Value = ImagePath;
                cmd.Parameters.Add("@IsDefault", SqlDbType.Bit ).Value = IsDefault;
            });
        }


        //دالة تعديل  صورة افتراضية
        public static int Product_SetDefaultPhoto(int ProductID,int PhotoID)
        {
            return dbHelper.ExecuteScalar<int>("Product_SetDefaultPhoto", cmd =>
            {
                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = ProductID;
                cmd.Parameters.Add("@PhotoID", SqlDbType.Int ).Value = PhotoID;
            });
        }

        // دالة حذف صورة 
        public static bool Product_DeletePhoto(int photoId)
        {
            return dbHelper.ExecuteNonQuery("Product_DeletePhoto", cmd =>
            {
                cmd.Parameters.Add("@PhotoID", SqlDbType.Int).Value = photoId;
            });
        }


        // دالة جلب  صور المنتج
        public static DataTable Product_GetPhotos(int ProductID)
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Product_GetPhotos", cmd =>
            {
                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = ProductID;
            });

            return result ?? new DataTable();
        }

        /*هذه الدوال تم وضعها فى كلاس DBServiecs ليتم استدعائها من الفورم*/






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
        public static DataTable NewInvoice_GetAcc(string invoiceType)
        {
            var result = dbHelper.ExecuteSelectQuery("NewInvoice_GetAcc", cmd =>
            {
                // 👌 إرسال نوع الفاتورة بدلاً من أرقام الحسابات
                cmd.Parameters.AddWithValue("@InvoiceType", invoiceType);
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
            string payment_Note,
            float? remainingOnAcc,
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
                    cmd.Parameters.AddWithValue("@Payment_Note", payment_Note ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RemainingOnAcc", remainingOnAcc ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoteInvoice", noteInvoice ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Saved", saved ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Warehouse_Id", Warehouse_Id);
                },
                expectMessageOutput: false // إذا لم يكن الإجراء يرجع OUTPUT message
            );

            return true; 
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
        public static int Product_CreateNewPiece(int prodID,int UpPiece_ID)
        {
            DataTable dt = dbHelper.ExecuteSelectQueryFlexible("Product_CreateNewPiece", cmd =>
            {
                cmd.Parameters.Add("@ID_Prod", SqlDbType.Int).Value = prodID;
                cmd.Parameters.Add("@UpPiece_ID", SqlDbType.Int).Value = UpPiece_ID;
            }, out _); // لا نحتاج إلى رسالة

            if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                return Convert.ToInt32(dt.Rows[0][0]);
            else
                return 0; // قيمة احتياطية في حال عدم وجود نتائج
        }

        // جلب القطع التابعة لمنتج[]
        public static DataTable Product_GetOrCreatePieces(int ID_Prod)//وصل هنا صحيح ID_Prod=10000
        {
            return dbHelper.ExecuteSelectQueryFlexible("Product_GetOrCreatePieces", cmd =>
            {
                cmd.Parameters.Add("@ID_Prod", SqlDbType.Int).Value = ID_Prod;
            }, out _); // تجاهل الرسالة لأنها غير مطلوبة
        }
  

        // ادراج قطعة جديدة
        public static DataTable Product_InsertNewPiece(int ID_Prod)
        {
            return dbHelper.ExecuteSelectQueryFlexible("Product_InsertNewPiece", cmd =>
            {
                cmd.Parameters.Add("@ID_Prod", SqlDbType.Int).Value = ID_Prod;
            }, out _); // تجاهل الرسالة لأنها غير مطلوبة
        }

        //حذف اخر سطر جرد لقطعة معينة
        public static string Product_DeleteLastRowInventry(int PieceID_fk )
        {
            // تنفيذ الإجراء المخزن مع تمكين انتظار الرسائل المرجعية
            string result = dbHelper.ExecuteNonQueryWithLogging("Product_DeleteLastRowInventry", cmd =>
            {
                cmd.Parameters.Add("@PieceID_fk", SqlDbType.Int).Value = PieceID_fk ;
            }, expectMessageOutput: false);

            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            return "تم حذف السطر بنجاح";
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
          int usID, // ⬅️ المستخدم الذي قام بالتعديل
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

                    cmd.Parameters.AddWithValue("@Us_ID", usID); // ⬅️ تمرير معرف المستخدم
                },
                expectMessageOutput: true
            );

            return resultMessage.StartsWith("تم");
        }
        public static bool MainAcc_UpdateAccount(
    int accID,
    string accName,
    bool isHidden,
    float? fixedAssetsValue,
    float? depreciationRateAnnually,
    int? fixedAssetsAge,
    bool? isEndedFixedAssets,
    string firstPhon,
    string antherPhon,
    string accNote,
    string clientEmail,
    string clientAddress,
    int usID,
    out string Message)
        {
            Message = dbHelper.ExecuteNonQueryWithLogging(
                procedureName: "MainAcc_UpdateAccount",
                setParams: cmd =>
                {
                    cmd.Parameters.AddWithValue("@AccID", accID);
                    cmd.Parameters.AddWithValue("@AccName", accName);
                    cmd.Parameters.AddWithValue("@IsHidden", isHidden);

                    cmd.Parameters.AddWithValue("@FixedAssetsValue", fixedAssetsValue.HasValue ? fixedAssetsValue.Value : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DepreciationRateAnnually", depreciationRateAnnually.HasValue ? depreciationRateAnnually.Value : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FixedAssetsAge", fixedAssetsAge.HasValue ? fixedAssetsAge.Value : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsEndedFixedAssets", isEndedFixedAssets.HasValue ? isEndedFixedAssets.Value : (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@FirstPhon", string.IsNullOrWhiteSpace(firstPhon) ? (object)DBNull.Value : firstPhon);
                    cmd.Parameters.AddWithValue("@AntherPhon", string.IsNullOrWhiteSpace(antherPhon) ? (object)DBNull.Value : antherPhon);
                    cmd.Parameters.AddWithValue("@AccNote", string.IsNullOrWhiteSpace(accNote) ? (object)DBNull.Value : accNote);
                    cmd.Parameters.AddWithValue("@ClientEmail", string.IsNullOrWhiteSpace(clientEmail) ? (object)DBNull.Value : clientEmail);
                    cmd.Parameters.AddWithValue("@ClientAddress", string.IsNullOrWhiteSpace(clientAddress) ? (object)DBNull.Value : clientAddress);

                    cmd.Parameters.AddWithValue("@UserID", usID);
                },
                expectMessageOutput: true
            );

            return Message.Trim().Contains("تم");


        }

        /*كيف يكون ضبط هذه على الدالة الجديدة بدون رسائل*/
        #endregion

        #region @@@@@@@ Reports Table @@@@@

        // تمت المراجعة لاستدعاء قائمة التقارير للحسابات    ### 
        public static DataTable RepMenu_Accounts(bool ForAccounts, bool IsForGroupAccounts, int TopAcc)//@@@
        {
            return dbHelper.ExecuteSelectQuery("RepMenu_Accounts", cmd =>
            {
                cmd.Parameters.Add("@ForAccounts", SqlDbType.Bit).Value = ForAccounts;
                cmd.Parameters.Add("@IsForGroupAccounts", SqlDbType.Bit).Value = IsForGroupAccounts;
                cmd.Parameters.Add("@TopAcc", SqlDbType.Int).Value = TopAcc;

            }) ?? new DataTable();
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
            }) ?? new DataTable();
        }

        public static DataTable MainAcc_GetAccountsByID(int AccID)//لماذا لا يجلب 106 و  والكل للمديونيات
        {
            string query = "MainAcc_GetAccountsByID";
            return dbHelper.ExecuteSelectQuery(query, cmd =>
            {
                cmd.Parameters.Add(new SqlParameter("@AccID", SqlDbType.Int) { Value = AccID });
            }) ?? new DataTable();
        }

        //احضار الحسابات الفرعية للحساب الممرر
        public static DataTable MainAcc_LoadFollowers(int AccID)//@@@
        {
            string query = "MainAcc_LoadFollowers";
            return dbHelper.ExecuteSelectQuery(query, cmd =>
            {
                cmd.Parameters.Add(new SqlParameter("@AccID", SqlDbType.Int) { Value = AccID });
            }) ?? new DataTable();


        }

        //

        //احضار الحسابات الفرعية للحساب الممرر والحساب الاصلى لتعبئة كمبوبكس
        public static DataTable MainAcc_LoadFollowersAndParent(int AccID)//@@@
        {
            string query = "MainAcc_LoadFollowersAndParent";
            return dbHelper.ExecuteSelectQuery(query, cmd =>
            {
                cmd.Parameters.Add(new SqlParameter("@AccID", SqlDbType.Int) { Value = AccID });
            }) ?? new DataTable();

        }

        //احضار الحساب الرئيسى الممرر فقط
        public static DataTable MainAcc_LoadTopByID(int AccID)//@@@
        {
            string query = "MainAcc_LoadTopByID";
            return dbHelper.ExecuteSelectQuery(query, cmd =>
            {
                cmd.Parameters.Add(new SqlParameter("@AccID", SqlDbType.Int) { Value = AccID });
            }) ?? new DataTable();
        }

        //ادراج تصنيف فرعى
        public static bool MainAcc_InsertSubCat(
           int accID,
           int? parentAccID,
           string accName,
           out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithLogging(
                "MainAcc_InsertSubCat",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@AccID", accID);
                    cmd.Parameters.AddWithValue("@ParentAccID", parentAccID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccName", accName);
                },
                expectMessageOutput: true
            );

            return resultMessage.StartsWith("تم");
        }

        public static bool MainAcc_UpdateSubCat(
           int accID,
           string accName,
           out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithLogging (
                "MainAcc_UpdateSubCat",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@AccID", accID);
                    cmd.Parameters.AddWithValue("@AccName", accName);
                },
                expectMessageOutput: true
            );

            return resultMessage.StartsWith("تم");
        }

        public static bool MainAcc_DeleteCatogryOrAcc(
           int accID,
           out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithLogging(
                "MainAcc_DeleteCatogryOrAcc",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@AccID", accID);
                },
                expectMessageOutput: true
            );

            return resultMessage.StartsWith("تم");
        }

        /*
             @AccID INT,
    @Direction NVARCHAR(10)  -- 'UP' or 'DOWN'
         */

        public static void MainAcc_MoveSortTree(int AccID, string Direction)
        {
            dbHelper.ExecuteNonQueryWithLogging(
                "MainAcc_MoveSortTree",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@AccID", AccID);
                    cmd.Parameters.AddWithValue("@Direction", Direction);
                }
            );
        }


        //وهذا كود الكلاس DBServiecs
        public static bool MainAcc_ChangAccCat(int newParentID, string accIDs, out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithLogging(
                "MainAcc_ChangAccCat",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@NewParentID", newParentID);
                    cmd.Parameters.AddWithValue("@AccIDs", accIDs);
                    // 👈 لا تضيف @Message هنا، الـ dbHelper بيعملها
                },
                expectMessageOutput: true
            );

            return resultMessage.StartsWith("تم");
        }

        /*
         
         وهذا كود الكلاس dbHelper
               public static string ExecuteNonQueryWithLogging(
            string procedureName,
            Action<SqlCommand> setParams,
            string? logProcedureName = null,
            Action<SqlCommand>? logParams = null,
            bool expectMessageOutput = false)
        {
            try
            {
                EnsureConnectionOpen();
                string result = "تم التنفيذ.";

                using (SqlCommand cmd = CreateCommand(procedureName))
                {
                    if (expectMessageOutput)
                    {
                        var msgParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(msgParam);
                    }

                    setParams?.Invoke(cmd);
                    cmd.ExecuteNonQuery();

                    if (expectMessageOutput)
                        result = cmd.Parameters["@Message"].Value?.ToString() ?? result;
                }

                if (!string.IsNullOrEmpty(logProcedureName) && logParams != null)
                {
                    using (SqlCommand logCmd = CreateCommand(logProcedureName))
                    {
                        logParams(logCmd);
                        logCmd.ExecuteNonQuery();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء التنفيذ: " + ex.Message);
                return "فشل في التنفيذ.";
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }




        وهذا الاجراء

        ALTER PROCEDURE dbo.MainAcc_ChangAccCat
    @NewParentID INT,             
    @AccIDs NVARCHAR(MAX),        
    @Message NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH CTE AS
    (
        SELECT TRY_CAST(value AS INT) AS AccID
        FROM STRING_SPLIT(@AccIDs, ',')
        WHERE TRY_CAST(value AS INT) IS NOT NULL
    )
    UPDATE M
    SET M.ParentAccID = @NewParentID
    FROM dbo.MainAccounts M
    INNER JOIN CTE C ON M.AccID = C.AccID;

    SET @Message = N'تم نقل ' + CAST(@@ROWCOUNT AS NVARCHAR) + N' حساب/حسابات.';
END


        فلماذ يرجع خطأ ويفشل فى التنفيذ 
         */
        #endregion

        #region ############# frmReport_Preview ##################

        // جلب القيم الافتراضية لتقارير الشاشة (بدون معاملات)
        public static DataTable GenralData_GetDefRepData()//@@@
        {
            return dbHelper.ExecuteSelectQuery("GenralData_GetDefRepData", command => { }) ?? new DataTable();
        }

        // جلب تاريخ بداية الحسابات (بدون معاملات)
        public static DataTable GenralData_GetStartAccountsDate()
        {
            return dbHelper.ExecuteSelectQuery("GenralData_GetStartAccountsDate", command => { }) ?? new DataTable();
        }

        #endregion

        #region *****************  Programer ****************
        /*وازر فى الفورم 
                private void btnDelete_Click(object sender, EventArgs e)
        {
            // تحقق أن هناك تقرير محدد
            if (selectedReportID <= 0)
            {
                MessageBox.Show("الرجاء اختيار تقرير لحذفه.", "تنبيه",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // رسالة تأكيد قبل الحذف
            DialogResult confirm = MessageBox.Show(
                "هل أنت متأكد أنك تريد حذف هذا التقرير؟",
                "تأكيد الحذف",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes)
                return;

            // تنفيذ الحذف
            string msg;
            bool success = DBServiecs.ReportsMaster_Delete(selectedReportID, out msg);

            // عرض النتيجة
            MessageBox.Show(msg, "نتيجة العملية",
                            MessageBoxButtons.OK,
                            success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (success)
            {
                // إعادة تحميل التقارير للقائمة الحالية
                if (cbxID_TopAcc.SelectedValue != null && int.TryParse(cbxID_TopAcc.SelectedValue.ToString(), out int idTopAcc))
                {
                    LoadReports(idTopAcc);
                }

                // مسح الحقول بعد الحذف
                btnNew_Click(null, null);
            }
        }

        وهذه الدالة فى كلاس DBServiecs
*/
        //
        public static string ReportsMaster_Delete(int ReportID)
        {
            return dbHelper.ExecuteNonQueryWithLogging("ReportsMaster_Delete", command =>
            {
                command.Parameters.Add("@ReportID", SqlDbType.Int).Value = ReportID;
            }, expectMessageOutput: true);
        }

        // استدعاء التقارير حسب الحساب الأعلى
        // ShowAll = true → يرجع كل التقارير
        // ShowAll = false → يرجع فقط المفعلة
        public static DataTable Reports_GetByTopAcc(int ID_TopAcc, bool ShowAll)
        {
            string query = "Reports_GetByTopAcc";
            return dbHelper.ExecuteSelectQuery(query, cmd =>
            {
                cmd.Parameters.Add(new SqlParameter("@ID_TopAcc", SqlDbType.Int) { Value = ID_TopAcc });
                cmd.Parameters.Add(new SqlParameter("@ShowAll", SqlDbType.Bit) { Value = ShowAll });
            }) ?? new DataTable();
        }



        public static bool ReportsMaster_Save(
            int ReportID,
            string ReportDisplayName,
            int ID_TopAcc,
            string ReportCodeName,
            bool IsGrouped,
            string Notes,
            bool IsActivRep,
            out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithLogging(
                "ReportsMaster_Save",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@ReportID", ReportID);
                    cmd.Parameters.AddWithValue("@ReportDisplayName", ReportDisplayName);
                    cmd.Parameters.AddWithValue("@ID_TopAcc", ID_TopAcc);
                    cmd.Parameters.AddWithValue("@ReportCodeName", ReportCodeName);
                    cmd.Parameters.AddWithValue("@IsGrouped", IsGrouped);
                    cmd.Parameters.AddWithValue("@Notes", Notes);
                    cmd.Parameters.AddWithValue("@IsActivRep", IsActivRep);
                },
                expectMessageOutput: true // مهم
            );

            return resultMessage.StartsWith("تم");
        }


        public static bool ReportsMaster_UpdateSortRep(
    int ID_TopAcc,
    DataTable newOrderTable, // يحتوي على أعمدة: ReportID, NewSortRep
    out string resultMessage)
        {
            resultMessage = dbHelper.ExecuteNonQueryWithLogging(
                "ReportsMaster_UpdateSortRep",
                cmd =>
                {
                    SqlParameter tvpParam = new SqlParameter("@NewOrder", SqlDbType.Structured);
                    tvpParam.TypeName = "dbo.ReportSortTableType"; // اسم نوع الجدول في SQL
                    tvpParam.Value = newOrderTable;
                    cmd.Parameters.Add(tvpParam);

                    cmd.Parameters.AddWithValue("@ID_TopAcc", ID_TopAcc);
                },
                expectMessageOutput: true
            );

            return resultMessage.StartsWith("تم");
        }

        #endregion

        #region @@@@@@@ CashTransaction Table @@@@@

        // جلب الحسابات الرئيسية على شكل شجرة
        public static DataTable? MainAcc_GetTopAccountTree()
        {
            // ترجع DataTable أو null إذا حدث خطأ
            return dbHelper.ExecuteSelectQuery("MainAcc_GetTopAccountTree");
        }

        // جلب السندات حسب النوع (تحصيل، دفع، تسوية...)
        public static DataTable? CashTransactions_GetBillByType(int operationTypeID)
        {
            return dbHelper.ExecuteSelectQuery(
                "CashTransactions_GetBillByType",
                cmd => cmd.Parameters.Add("@OperationType_ID", SqlDbType.Int).Value = operationTypeID
            );
        }

        // إدراج أو تعديل سند تحصيل / دفع / تسوية
        public static bool CashTransactions_InsertOrUpdate(
            int transactionID,
            string voucherNumber,
            DateTime? transactionDate,
            int? operationTypeID,
            int? accountID,
            int? accBox,             // رقم الصندوق أو الحساب النقدي (اختياري)
            float? amount,
            int? paymentMethodID,
            string? descriptionNote, // يمكن أن يكون null
            int? createdByUsID,
            string? saveTransaction, // يمكن أن يكون null
            out string resultMessage // رسالة ناتجة من الإجراء المخزن
        )
        {
            resultMessage = string.Empty; // تهيئة لمنع التحذير

            SqlParameter outputMessage = new SqlParameter("@ResultMessage", SqlDbType.NVarChar, 100)
            {
                Direction = ParameterDirection.Output
            };

            resultMessage = dbHelper.ExecuteStoredProcedureWithOutputMessage(
                "CashTransactions_InsertOrUpdate",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@TransactionID", transactionID);
                    cmd.Parameters.AddWithValue("@VoucherNumber", voucherNumber);
                    cmd.Parameters.AddWithValue("@TransactionDate", transactionDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OperationType_ID", operationTypeID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccountID", accountID ?? (object)DBNull.Value);

                    // تصحيح شرط التحقق من النوع 8 أو 9 مع التأكد من وجود accBox
                    cmd.Parameters.AddWithValue("@AccBox",
                        ((operationTypeID == 8 || operationTypeID == 9) && accBox.HasValue)
                        ? (object)accBox.Value
                        : DBNull.Value
                    );

                    cmd.Parameters.AddWithValue("@Amount", amount ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PaymentMethodID", paymentMethodID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DescriptionNote",
                        string.IsNullOrWhiteSpace(descriptionNote) ? (object)DBNull.Value : descriptionNote
                    );
                    cmd.Parameters.AddWithValue("@CreatedByUsID", createdByUsID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SaveTransaction", saveTransaction ?? string.Empty);

                    cmd.Parameters.Add(outputMessage);
                },
                outputMessageSize: 100
            );

            if (outputMessage.Value != DBNull.Value && outputMessage.Value != null)
                resultMessage = outputMessage.Value.ToString() ?? string.Empty;

            return !string.IsNullOrEmpty(resultMessage);
        }

        // جلب رقم جديد للسند
        public static string CashTransactions_GetNewVoucherNumber(int operationTypeID)
        {
            DataTable? dt = dbHelper.ExecuteSelectQuery(
                "CashTransactions_GetNewVoucherNumber",
                cmd => cmd.Parameters.AddWithValue("@OperationType_ID", operationTypeID)
            );

            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["NewVoucherNumber"]?.ToString() ?? string.Empty;
            else
                return string.Empty;
        }

        // جلب رقم المعاملة التالي
        public static int CashTransactions_GetNextTransactionID()
        {
            DataTable? dt = dbHelper.ExecuteSelectQuery("CashTransactions_GetNextTransactionID");

            if (dt != null && dt.Rows.Count > 0 && int.TryParse(dt.Rows[0]["NextTransactionID"]?.ToString(), out int nextID))
                return nextID;
            else
                return 1; // في حال عدم وجود بيانات
        }

        #endregion

        #region ********   شجرة الحسابات  ************
        //==========================================
        //  1 حسابات الشجرة الاساسية
        //==========================================
        // جلب جميع  شجرة الحسابات ###
        public static DataTable Acc_GetChart()//@@
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Acc_GetChart");
            return result ?? new DataTable();
        }

        // احضار الابناء الذين ليس لهم ابناء اخر
        public static DataTable Acc_GetLeafChildren(int ParentAccID)
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Acc_GetLeafChildren", command =>
            {
                command.Parameters.Add("@ParentAccID", SqlDbType.Int).Value = ParentAccID;
            });

            return result ?? new DataTable();
        }

        // إضافة حساب جديد
        public static string Acc_AddAccount(string AccName, int? ParentTreeAccCode, int? CreateByUserID)
        {
            return dbHelper.ExecuteNonQueryWithLogging("Acc_AddAccount", command =>
            {
                command.Parameters.Add("@AccName", SqlDbType.NVarChar).Value = AccName;
                command.Parameters.Add("@ParentTreeAccCode", SqlDbType.Int).Value = (object?)ParentTreeAccCode ?? DBNull.Value;
                command.Parameters.Add("@CreateByUserID", SqlDbType.Int).Value = (object?)CreateByUserID ?? DBNull.Value;
            }, expectMessageOutput: false);
        }

        // وظيفة حذف حساب
        public static string Acc_DeleteAccount(int? AccID)
        {
            return dbHelper.ExecuteNonQueryWithLogging("Acc_DeleteAccount", command =>
            {
                command.Parameters.Add("@AccID", SqlDbType.Int).Value = (object?)AccID ?? DBNull.Value;

                // لاحظ هنا لازم تخلي expectMessageOutput = false
                var outputParam = new SqlParameter("@OutputMsg", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);

            }, expectMessageOutput: false); // ❌ عشان ما يضيفش @Message
        }
        
        // جلب حساب ###
        public static DataTable Acc_GetData(int accID)
        {
            return dbHelper.ExecuteSelectQuery("Acc_GetData", command =>
            {
                command.Parameters.Add("@AccID", SqlDbType.Int).Value = accID;
            }) ?? new DataTable();
        }

        // تحديث بيانات حساب
        public static string Acc_UpdateAccount(int? accID, string accName, bool isHidden)
        {
            return dbHelper.ExecuteNonQueryWithLogging("Acc_UpdateAccount", command =>
            {
                command.Parameters.Add("@AccID", SqlDbType.Int).Value = (object?)accID ?? DBNull.Value;
                command.Parameters.Add("@AccName", SqlDbType.NVarChar, 200).Value = (object?)accName ?? DBNull.Value;
                command.Parameters.Add("@IsHidden", SqlDbType.Bit).Value = isHidden;

                var outputParam = new SqlParameter("@OutputMsg", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);

            }, expectMessageOutput: false);
        }

        //========================================
        //  2-   تفاصيل الحساب الشخصية
        //========================================
        // إضافة أو تعديل تفاصيل الحساب
        public static string Acc_SaveDetails(int? DetailID, int AccID, string? ContactName,
                                             string? Phone, string? Mobile, string? Email,
                                             string? Address, string? Notes)
        {
            return dbHelper.ExecuteNonQueryWithLogging("Acc_SaveDetails", command =>
            {
                command.Parameters.Add("@DetailID", SqlDbType.Int).Value = (object?)DetailID ?? DBNull.Value;
                command.Parameters.Add("@AccID", SqlDbType.Int).Value = AccID;
                command.Parameters.Add("@ContactName", SqlDbType.NVarChar).Value = (object?)ContactName ?? DBNull.Value;
                command.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = (object?)Phone ?? DBNull.Value;
                command.Parameters.Add("@Mobile", SqlDbType.NVarChar).Value = (object?)Mobile ?? DBNull.Value;
                command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = (object?)Email ?? DBNull.Value;
                command.Parameters.Add("@Address", SqlDbType.NVarChar).Value = (object?)Address ?? DBNull.Value;
                command.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = (object?)Notes ?? DBNull.Value;
            }, expectMessageOutput: false ); // ✅ هنا true عشان نقرأ رسالة Msg الراجعة من الإجراء
        }

        // حذف تفاصيل
        public static string Acc_DeleteDetails(int DetailID)
        {
            return dbHelper.ExecuteNonQueryWithLogging("Acc_DeleteDetails", command =>
            {
                command.Parameters.Add("@DetailID", SqlDbType.Int).Value = DetailID;
            }, expectMessageOutput: false);
        }

        // جلب جميع التفاصيل لحساب معين
        public static DataTable Acc_GetDetails(int AccID)
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Acc_GetDetails", command =>
            {
                command.Parameters.Add("@AccID", SqlDbType.Int).Value = AccID;
            });
            return result ?? new DataTable();
        }

        // تحديث تفاصيل
        public static string Acc_UpdateDetails(int DetailID, string? ContactName, string? Phone,
                                               string? Mobile, string? Email, string? Address, string? Notes)
        {
            return dbHelper.ExecuteNonQueryWithLogging("Acc_UpdateDetails", command =>
            {
                command.Parameters.Add("@DetailID", SqlDbType.Int).Value = DetailID;
                command.Parameters.Add("@ContactName", SqlDbType.NVarChar).Value = (object?)ContactName ?? DBNull.Value;
                command.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = (object?)Phone ?? DBNull.Value;
                command.Parameters.Add("@Mobile", SqlDbType.NVarChar).Value = (object?)Mobile ?? DBNull.Value;
                command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = (object?)Email ?? DBNull.Value;
                command.Parameters.Add("@Address", SqlDbType.NVarChar).Value = (object?)Address ?? DBNull.Value;
                command.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = (object?)Notes ?? DBNull.Value;
            }, expectMessageOutput: false);
        }


        //========================================
        //  3 بيانات الاصول الثابتة 
        //========================================
        //جلب جميع البيانات التفصيلية عن الاصول الثابتة
        public static DataTable Acc_AssetsGetAll()
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Acc_AssetsGetAll", command =>
            {
 
            });
            return result ?? new DataTable();
        }


        // جلب جميع تفاصيل حساب اصول ثابتة
        public static DataTable Acc_AssetsGetByAccountID(int AccID)
        {
            DataTable? result = dbHelper.ExecuteSelectQuery("Acc_AssetsGetByAccountID", command =>
            {
                command.Parameters.Add("@AccID", SqlDbType.Int).Value = AccID;
            });
            return result ?? new DataTable();
        }

        // ✅ تحديث أو إدراج تفاصيل بيانات أصل ثابت
        public static string Acc_AssetsSave(
            int accID,
            int? responsiblePersonID = null,
            int? locationID = null,
            DateTime? purchaseDate = null,
            decimal? purchaseValue = null,
            int? usefulLifeMonths = null,
            decimal? residualValue = null,
            string? notes = null)
        {
            return dbHelper.ExecuteNonQueryWithLogging("Acc_AssetsSave", command =>
            {
                // 🔹 المعاملات المطلوبة
                command.Parameters.Add("@AccID", SqlDbType.Int).Value = accID;

                // 🔹 المعاملات الاختيارية
                command.Parameters.Add("@ResponsiblePersonID", SqlDbType.Int).Value = (object?)responsiblePersonID ?? DBNull.Value;
                command.Parameters.Add("@LocationID", SqlDbType.Int).Value = (object?)locationID ?? DBNull.Value;
                command.Parameters.Add("@PurchaseDate", SqlDbType.Date).Value = (object?)purchaseDate ?? DBNull.Value;
                command.Parameters.Add("@PurchaseValue", SqlDbType.Decimal).Value = (object?)purchaseValue ?? DBNull.Value;
                command.Parameters.Add("@UsefulLifeMonths", SqlDbType.Int).Value = (object?)usefulLifeMonths ?? DBNull.Value;
                command.Parameters.Add("@ResidualValue", SqlDbType.Decimal).Value = (object?)residualValue ?? DBNull.Value;
                command.Parameters.Add("@Notes", SqlDbType.NVarChar, 500).Value = (object?)notes ?? DBNull.Value;

                // 🔹 معامل الإخراج
                var outputParam = new SqlParameter("@OutputMsg", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);

            }, expectMessageOutput: true); // ✅ علشان يرجع الرسالة من @OutputMsg
        }

        // ✅ حذف تفاصيل اصل
        public static string Acc_AssetsDelete(int AccID)
        {
            return dbHelper.ExecuteNonQueryWithLogging("Acc_AssetsDelete", command =>
            {
                command.Parameters.Add("@AccID", SqlDbType.Int).Value = AccID;
            }, expectMessageOutput: false);
        }

        // ✅ تحديث جميع الأصول الثابتة بالقيمة الدفترية الحالية مع إرجاع عدد الصفوف المحدثة
        public static int Acc_AssetsUpdateDepreciation()
        {
            int rowsAffected = 0;

            dbHelper.ExecuteNonQueryWithLogging("Acc_AssetsUpdateDepreciation", command =>
            {
                var outputParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);

                // ✅ بعد التنفيذ تقدر تاخد القيمة
                command.ExecuteNonQuery();
                rowsAffected = (int)(command.Parameters["@RowsAffected"].Value ?? 0);

            }, expectMessageOutput: false);

            return rowsAffected;
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
            DataTable? result = dbHelper.ExecuteSelectQuery("ChequeBatches_Search", cmd =>
            {
                cmd.Parameters.Add("@SearchBy", SqlDbType.NVarChar, 20).Value = searchBy;
                cmd.Parameters.Add("@Value", SqlDbType.Int).Value = value;
            });

            // إذا كانت النتيجة null، نرجع جدول فارغ
            return result ?? new DataTable();
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
            out string? resultMessage)
        {
            resultMessage = null;

            SqlParameter outputMessage = new SqlParameter("@ResultMessage", SqlDbType.NVarChar, 100)
            {
                Direction = ParameterDirection.Output
            };

            resultMessage = dbHelper.ExecuteNonQueryWithLogging(
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


            // جلب كود جديد للحافظة
            public static string ChequeBatches_GetNewBatchCode(int MovTypeID)
            {
                DataTable? dt = dbHelper.ExecuteSelectQuery("ChequeBatches_GetNewBatchCode", cmd =>
                {
                    cmd.Parameters.AddWithValue("@MovTypeID", MovTypeID);
                });

                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["NewBatchCode"] != DBNull.Value)
                {
                    return dt.Rows[0]["NewBatchCode"].ToString() ?? string.Empty;
                }
                else
                {
                    return string.Empty; // قيمة افتراضية لو مفيش نتيجة
                }
            }

            // جلب معرف جديد للحافظة
            public static int ChequeBatches_GetNextBatchID()
            {
                DataTable? dt = dbHelper.ExecuteSelectQuery("ChequeBatches_GetNextBatchID");

                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["NextBatchID"] != DBNull.Value)
                {
                    return Convert.ToInt32(dt.Rows[0]["NextBatchID"]);
                }
                else
                {
                    return 1; // قيمة افتراضية في حال لا توجد سجلات
                }
            }

        // احضار شيكات الحافظة
        // احضار شيكات الحافظة بطريقة آمنة من null
        public static DataTable Cheques_GetByBatchID(int BatchID)
        {
            // ExecuteSelectQuery قد ترجع null، لذلك نجعل المتغير nullable
            DataTable? result = dbHelper.ExecuteSelectQuery("Cheques_GetByBatchID", cmd =>
            {
                cmd.Parameters.Add("@BatchID", SqlDbType.Int).Value = BatchID;
            });

            // إذا كانت النتيجة null، نعيد جدول جديد فارغ
            return result ?? new DataTable();
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
            resultMessage = dbHelper.ExecuteNonQueryWithLogging(
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
            resultMessage = dbHelper.ExecuteNonQueryWithLogging(
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
            resultMessage = dbHelper.ExecuteNonQueryWithLogging(
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
            resultMessage = dbHelper.ExecuteNonQueryWithLogging(
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

    }
}

