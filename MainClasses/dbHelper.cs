using System.Data;
using Microsoft.Data.SqlClient;

namespace MizanOriginalSoft.MainClasses
{
    /// <summary>
    /// هذا الكلاس يحتوي على الوظائف العامة للتعامل مع قاعدة البيانات
    /// مثل الاتصال، تنفيذ الإجراءات المخزنة، الاستعلامات، إلخ.
    /// </summary>
    internal static class dbHelper
    {
        #region 🟡 إعداد الاتصال بقاعدة البيانات

        private static SqlConnection? connection;
        private static readonly string? connectionString = LoadConnectionString();

        /// <summary>
        /// تحميل جملة الاتصال من ملف نصي خارجي خاص بكل عميل
        /// </summary>
        private static string? LoadConnectionString()
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "serverConnectionSettings.txt");

                if (!File.Exists(filePath))
                    throw new FileNotFoundException("ملف الإعدادات غير موجود", filePath);

                string server = "", database = "";

                foreach (var line in File.ReadAllLines(filePath))
                {
                    if (line.StartsWith("serverName="))
                        server = line.Substring("serverName=".Length).Trim();
                    else if (line.StartsWith("DBName="))
                        database = line.Substring("DBName=".Length).Trim();
                }

                if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database))
                    throw new Exception("بيانات الاتصال غير مكتملة.");

                string connectionString = $"Data Source={server};Initial Catalog={database};Integrated Security=True;TrustServerCertificate=True";
                return connectionString;
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في ملف الاتصال: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return null;
            }
        }

        private static void EnsureConnectionOpen()
        {
            if (connection == null)
                connection = new SqlConnection(connectionString);

            if (connection?.State != ConnectionState.Open)
                connection?.Open();
        }

        private static void EnsureConnectionClosed()
        {
            if (connection?.State == ConnectionState.Open)
                connection.Close();
        }

        private static SqlCommand CreateCommand(string procedureName)
        {
            return new SqlCommand(procedureName, connection!)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        #endregion

        #region 🔷 دوال تنفيذية بدون إرجاع بيانات

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


        public static bool ExecuteNonQuery(string procedureName, Action<SqlCommand> setParams)
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(procedureName, con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                setParams?.Invoke(cmd);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء التنفيذ: " + ex.Message);
                return false;
            }
        }

        //دالة  تقبل باراميترات وتعيد الرسالة:
        public static string ExecuteStoredProcedureWithOutputMessage(
            string procedureName,
            Action<SqlCommand> setParams,
            int outputMessageSize = 500)
        {
            try
            {
                EnsureConnectionOpen();
                string result = "تم التنفيذ.";

                using (SqlCommand cmd = CreateCommand(procedureName))
                {
                    // إنشاء باراميتر الإخراج
                    var msgParam = new SqlParameter("@Message", SqlDbType.NVarChar, outputMessageSize)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(msgParam);

                    // إضافة باقي الباراميترات
                    setParams?.Invoke(cmd);

                    cmd.ExecuteNonQuery();

                    result = cmd.Parameters["@Message"].Value?.ToString() ?? result;

                    return result;
                }
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


        #endregion

        #region 🟢 دوال جلب بيانات (SELECT)

        /// <summary>
        /// تنفيذ إجراء مخزن لجلب البيانات مع إمكانية الحصول على رسالة من SQL.
        /// - يمكن استخدامها في الحالات التي تتطلب فقط جلب بيانات.
        /// - ويمكن أيضًا استخدامها في الحالات التي تعيد رسالة باستخدام بارامتر OUTPUT باسم @Message.
        /// تُعيد الرسالة الخارجة من الإجراء في حال كان هناك بارامتر @Message OUTPUT، 
        /// أو رسالة الخطأ في حال حدوث استثناء، أو تكون null إن لم يتم طلب رسالة.
        /// <param name="expectMessageOutput">هل الإجراء يحتوي على بارامتر @Message OUTPUT؟</param>
        /// <returns>جدول يحتوي على البيانات المطلوبة، أو جدول فارغ في حال حدوث خطأ</returns>
        public static DataTable ExecuteSelectQueryFlexible(
            string procedureName,
            Action<SqlCommand>? setParams,
            out string? message,
            bool expectMessageOutput = false)
        {
            var dt = new DataTable();
            message = null;

            try
            {
                EnsureConnectionOpen();

                using (var cmd = CreateCommand(procedureName))
                {
                    // تمرير البارامترات
                    setParams?.Invoke(cmd);

                    // إضافة بارامتر @Message في حال توقع وجوده
                    if (expectMessageOutput)
                    {
                        var msgParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(msgParam);
                    }

                    // تعبئة الجدول من نتائج الإجراء
                    using var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);// عاد هنا برقم مختلف 10001

                    // قراءة الرسالة إن وُجدت
                    if (expectMessageOutput)
                    {
                        message = cmd.Parameters["@Message"].Value?.ToString();
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                message = "خطأ أثناء جلب البيانات: " + ex.Message;
                MessageBox.Show(message);
                return new DataTable(); // لا نعيد null إطلاقًا
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }
        /*يمكن استخدام الدالة السابقة  ExecuteSelectQueryFlexible
         * بديلا عن الدالتين
        ExecuteSelectQuery
        ExecuteSelectQueryWithMessage
         */

        public static DataTable? ExecuteSelectQuery(string procedureName, Action<SqlCommand>? setParams = null)
        {
            var dt = new DataTable();

            try
            {
                EnsureConnectionOpen();

                using (var cmd = CreateCommand(procedureName))
                {
                    setParams?.Invoke(cmd);

                    using var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء جلب البيانات: " + ex.Message);
                
                return null;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }

        public static DataTable? ExecuteSelectQueryWithMessage(
            string procedureName,
            Action<SqlCommand> setParams,
            out string message,
            bool expectMessageOutput = false)
        {
            var dt = new DataTable();
            message = "تم التنفيذ.";

            try
            {
                EnsureConnectionOpen();

                using (var cmd = CreateCommand(procedureName))
                {
                    setParams?.Invoke(cmd);

                    if (expectMessageOutput)
                    {
                        var msgParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(msgParam);
                    }

                    using var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);

                    if (expectMessageOutput)
                        message = cmd.Parameters["@Message"].Value?.ToString() ?? message;
                }

                return dt;
            }
            catch (Exception ex)
            {
                message = "خطأ أثناء جلب البيانات: " + ex.Message;
                MessageBox.Show(message);
                return null;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }

        #endregion

        #region 🔴 دوال إرجاع قيمة واحدة (SCALAR)

        public static T? ExecuteScalar<T>(string procedureName, Action<SqlCommand> setParams)
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(procedureName, con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                setParams(cmd);
                con.Open();
                object? result = cmd.ExecuteScalar();
                return result is null ? default : (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في ExecuteScalar: " + ex.Message);
                return default;
            }
        }

        #endregion

        public static string ExecuteNonQueryNoParamsWithMessage(string procedureName, bool expectMessageOutput = false)
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

                    cmd.ExecuteNonQuery();

                    if (expectMessageOutput)
                        result = cmd.Parameters["@Message"].Value?.ToString() ?? result;
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






    }
}
