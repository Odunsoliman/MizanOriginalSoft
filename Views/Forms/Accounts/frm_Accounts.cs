using MizanOriginalSoft.MainClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.Accounts
{
    public partial class frm_Accounts : Form
    {
        public frm_Accounts()
        {
            InitializeComponent();
        }

        private void frm_Accounts_Load(object sender, EventArgs e)
        {
            LoadAccountsTree();
        }

        //اريد نسخة منقحة
        private void LoadAccountsTree()
        {
            treeViewAccounts.Nodes.Clear();

            DataTable dt = DBServiecs.Acc_GetChart() ?? new DataTable();
            if (dt.Rows.Count == 0) return;

            foreach (DataRow row in dt.Rows)
            {
                // تجاهل الصفوف التي تحتوي على قيم غير صالحة
                if (row["FullPath"] == DBNull.Value || row["AccName"] == DBNull.Value || row["AccID"] == DBNull.Value)
                    continue;

                string fullPath = row["FullPath"] as string ?? string.Empty;
                string accName = row["AccName"] as string ?? string.Empty;

                if (string.IsNullOrWhiteSpace(fullPath) || string.IsNullOrWhiteSpace(accName))
                    continue;

                int level = GetLevelFromFullPath(fullPath);
                TreeNode node = new TreeNode(accName)
                {
                    Tag = row // حفظ كل البيانات لاستخدامها لاحقاً
                };

                if (level == 0)
                {
                    treeViewAccounts.Nodes.Add(node);
                }
                else
                {
                    TreeNode? parentNode = FindParentNode(treeViewAccounts.Nodes, fullPath, level - 1);
                    if (parentNode is not null) // التحقق باستخدام is not null
                    {
                        parentNode.Nodes.Add(node);
                    }
                    else
                    {
                        treeViewAccounts.Nodes.Add(node); // fallback
                    }
                }

            }

          //  treeViewAccounts.ExpandAll();
            // بدلاً من ExpandAll، نغلق كل العقد
            treeViewAccounts.CollapseAll();
        }

        // دالة مساعدة لإيجاد الأب حسب FullPath
        private TreeNode? FindParentNode(TreeNodeCollection nodes, string fullPath, int targetLevel)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag is DataRow row)
                {
                    // تحقق من أن القيمة صالحة
                    if (row["FullPath"] == DBNull.Value)
                        continue;

                    string nodePath = row["FullPath"] as string ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(nodePath))
                        continue;

                    int nodeLevel = GetLevelFromFullPath(nodePath);

                    if (nodeLevel == targetLevel && fullPath.StartsWith(nodePath))
                        return node;

                    TreeNode? found = FindParentNode(node.Nodes, fullPath, targetLevel);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }


        // دالة لحساب المستوى من FullPath
        private int GetLevelFromFullPath(string fullPath)
        {
            return fullPath.Split(new string[] { "→" }, StringSplitOptions.None).Length - 1;
        }


    }
}
