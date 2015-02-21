using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace 七牛云存储批量管理工具
{
    public partial class FormLoading : Form
    {
        public FormLoading()
        {
            InitializeComponent();
        }
        //执行其它线程的委托
        public delegate void UIDelegate();
        //初始化
        private void FormLoading_Load(object sender, EventArgs e)
        {
            //玻璃效果
            MyGlassEffect Glass = new MyGlassEffect();
            if (MyGlassEffect.GlassEnabled)
            {
                Rectangle ScreenArea = System.Windows.Forms.Screen.GetWorkingArea(this);
                Glass.TopBarSize = ScreenArea.Height;
                Glass.LeftBarSize = ScreenArea.Width;
                Glass.BottomBarSize = ScreenArea.Height;
                Glass.RightBarSize = ScreenArea.Width;
                Glass.GlassLabel = new Label[] { label1 };
                Glass.ShowEffect(this);
            }
            //窗体属性
            this.TopMost = true;
            this.TopMost = false;
            //this.ControlBox = false;
            this.Icon = Properties.Resources.logo;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "【" + My.MyProjectName + "】" + "  正在启动……";
            //动图Loading
            pictureBox1.Image = Properties.Resources.loading;
            this.Size = new Size(pictureBox1.Image.Width + 20, pictureBox1.Image.Height + 100);
            this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
            pictureBox1.Width = pictureBox1.Image.Width;
            pictureBox1.Height = pictureBox1.Image.Height;
            pictureBox1.Left = (this.Width - pictureBox1.Width) / 2 - 5;
            pictureBox1.Top = (this.Height - pictureBox1.Height) / 2 - 40;
            label1.Top = pictureBox1.Top + pictureBox1.Height + 20;
            //检查网络，实际Loading
            label1.Text = "正在检查网络连接状态……";
            label1.Left = (this.Width - label1.Width) / 2;
            new CheckInternet1();
        }
        //最大化时保持布局不变
        private void FormLoading_Resize(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Width = pictureBox1.Image.Width;
                pictureBox1.Height = pictureBox1.Image.Height;
                this.Size = new Size(pictureBox1.Image.Width + 20, pictureBox1.Image.Height + 100);
                this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
                this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
                pictureBox1.Left = (this.Width - pictureBox1.Width) / 2 - 5;
                pictureBox1.Top = (this.Height - pictureBox1.Height) / 2 - 40;
                label1.Left = (this.Width - label1.Width) / 2;
                label1.Top = pictureBox1.Top + pictureBox1.Height + 20;
            }
        }
        //检查网络
        public class CheckInternet1
        {
            private Thread MyThread;
            public CheckInternet1()
            {
                MyThread = new Thread(new ThreadStart(delegate
                {
                    string Temp = "";
                    Temp = My.GetWebCodeByWebRequest("http://www.baidu.com/img/bdlogo.png");
                    if (Temp == "")
                    {
                        Temp = My.GetWebCodeByWebRequest("http://www.baidu.com/img/bdlogo.png");
                    }
                    if (Temp == "")
                    {
                        Temp = My.GetWebCodeByWebRequest("http://www.baidu.com/img/bdlogo.png");
                    }
                    if (Temp == "")
                    {
                        My.LoadingForm.TopMost = true;
                        My.LoadingForm.TopMost = false;
                        if (MessageBox.Show("网络连接错误，请检查网络设置。","提示",MessageBoxButtons.OK,MessageBoxIcon.Exclamation) == DialogResult.OK) {
                            Application.Exit();
                        }
                    }
                    //else { My.Show(Temp.Length); }
                    //5KB
                    try { My.LoadingForm.Invoke(new FormLoading.UIDelegate(DelegateMethod)); }
                    catch (Exception) { }
                    finally { MyThread.Abort(); }
                }
                ));
                MyThread.IsBackground = true;
                MyThread.Start();
            }
            public void DelegateMethod()
            {
                My.LoadingForm.TopMost = true;
                My.LoadingForm.TopMost = false;
                My.LoadingForm.label1.Text = "正在检查更新版本信息……";
                My.LoadingForm.label1.Left = (My.LoadingForm.Width - My.LoadingForm.label1.Width) / 2;
                My.LoadingForm.Refresh();
                new CheckInternet2();
            }
        }
        //检查版本
        public class CheckInternet2
        {
            private Thread MyThread;
            public CheckInternet2()
            {
                MyThread = new Thread(new ThreadStart(delegate
                {
                    string Temp = "";
                    Temp = My.GetWebCodeByWebRequest("http://moonlordapi.sinaapp.com/%E4%B8%83%E7%89%9B%E4%BA%91%E5%AD%98%E5%82%A8%E6%89%B9%E9%87%8F%E7%AE%A1%E7%90%86%E5%B7%A5%E5%85%B7.php");
                    if (Temp == "")
                    {
                        Temp = My.GetWebCodeByWebRequest("http://moonlordapi.sinaapp.com/%E4%B8%83%E7%89%9B%E4%BA%91%E5%AD%98%E5%82%A8%E6%89%B9%E9%87%8F%E7%AE%A1%E7%90%86%E5%B7%A5%E5%85%B7.php");
                    }
                    if (Temp == "")
                    {
                        Temp = My.GetWebCodeByWebRequest("http://moonlordapi.sinaapp.com/%E4%B8%83%E7%89%9B%E4%BA%91%E5%AD%98%E5%82%A8%E6%89%B9%E9%87%8F%E7%AE%A1%E7%90%86%E5%B7%A5%E5%85%B7.php");
                    }
                    if (Temp != "" && Temp != "SKIP")//强制打开浏览器（返回字符串为URL）
                    {
                        if (Temp.Contains("?"))
                        {
                            //System.Diagnostics.Process.Start("IEXPLORE.EXE", Temp);
                            System.Diagnostics.Process.Start("EXPLORER.EXE", "\"" + Temp + "\"");
                        }
                        else
                        {
                            System.Diagnostics.Process.Start("EXPLORER.EXE", Temp);
                        }
                    }
                    if (Temp == "") {
                        if (MessageBox.Show("更新版本信息获取失败，请联系www.moonlord.cn。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                        {
                            Application.Exit();
                        }
                    }
                    //else { My.Show(Temp.Length); }

                    try { My.LoadingForm.Invoke(new FormLoading.UIDelegate(DelegateMethod)); }
                    catch (Exception) { }
                    finally { MyThread.Abort(); }
                }
                ));
                MyThread.IsBackground = true;
                MyThread.Start();
            }
            public void DelegateMethod()
            {
                My.LoadingForm.TopMost = true;
                My.LoadingForm.TopMost = false;
                My.LoadingForm.label1.Text = "正在连接七牛云服务器，请在弹出的窗口中登录……";
                My.LoadingForm.label1.Left = (My.LoadingForm.Width - My.LoadingForm.label1.Width) / 2;
                My.LoadingForm.Refresh();
                new CheckInternet3();
            }
        }
        //账号登录
        public class CheckInternet3
        {
            public Thread MyThread;
            public CheckInternet3()
            {
                MyThread = new Thread(new ThreadStart(delegate
                {
                    try { My.LoadingForm.Invoke(new FormLoading.UIDelegate(DelegateMethod1)); }
                    catch (Exception) { }
                    while (My.Logined == 0)
                    {
                        Thread.Sleep(100);
                    }
                    try { My.LoadingForm.Invoke(new FormLoading.UIDelegate(DelegateMethod2)); }
                    catch (Exception) { }
                    while (My.Logined == 1)
                    {
                        Thread.Sleep(100);
                    }
                    try { My.LoadingForm.Invoke(new FormLoading.UIDelegate(DelegateMethod3)); }
                    catch (Exception) { }
                    MyThread.Abort(); 
                }
                ));
                MyThread.IsBackground = true;
                MyThread.Start();
            }
            public void DelegateMethod1()
            {
                My.LoginForm.Visible = true;
                My.LoginForm.TopMost = true;
                My.LoginForm.TopMost = false;
            }
            public void DelegateMethod2()
            {
                My.LoadingForm.TopMost = true;
                My.LoadingForm.TopMost = false;
                My.LoadingForm.label1.Text = "正在获取登录账号的七牛云存储信息……";
                My.LoadingForm.label1.Left = (My.LoadingForm.Width - My.LoadingForm.label1.Width) / 2;
                My.LoadingForm.Refresh();
            }
            public void DelegateMethod3()
            {
                My.LoadingForm.TopMost = true;
                My.LoadingForm.TopMost = false;
                My.LoadingForm.label1.Text = "登录完成，正杂切换界面……";
                My.LoadingForm.label1.Left = (My.LoadingForm.Width - My.LoadingForm.label1.Width) / 2;
                My.LoadingForm.Refresh();
                My.LoadingForm.Visible = false;
                My.MainForm.Visible = true;
            }
        }
        //关闭窗体则退出程序
        private void FormLoading_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;（加上这句就永远Exit()结束不了了）
            //My.Show("FormLoading_FormClosing");
            //My.MainForm.Dispose();
            //My.LoginForm.Dispose();
            //My.LoadingForm.Dispose();
            Application.Exit();
            //必须将多线程指定为MyThread.IsBackground = true;，Application.Exit();才能完全退出程序。
            //强制退出的方法：
            //Application.ExitThread();
            //System.Environment.Exit(0);
            //System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
