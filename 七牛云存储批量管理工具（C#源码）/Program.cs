using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace 七牛云存储批量管理工具
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            My.LoadingForm = new FormLoading();
            My.LoginForm = new FormLogin();
            My.MainForm = new FormMain();
            Application.Run(My.LoadingForm);
        }
    }
}
