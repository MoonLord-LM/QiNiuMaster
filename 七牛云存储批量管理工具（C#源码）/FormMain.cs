using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 七牛云存储批量管理工具
{
    public partial class FormMain : Form
    {
        //执行其它线程的委托
        public delegate void UIDelegate();
        public System.Windows.Forms.Button[] FormMainButton;
        public int ChosenButtomIndex;
        public int ChosenRowIndex;
        public int ChosenColumnIndex;
        public String Marker = "";
        public int Limit = 100;
        public String Prefix = "";
        public FormMain()
        {
            InitializeComponent();
        }
        //初始化
        private void FormMain_Load(object sender, EventArgs e)
        {
            //My.Buckets Buckets1 = new My.Buckets("MWMWMWMWMWMMWMWMWMWMWM", "1111");
            //My.Buckets Buckets2 = new My.Buckets("GHIJKLXYZMNOPQRWMHMNOPQRWMH", "2222");
            //My.Buckets Buckets3 = new My.Buckets("MNOPQRWMHMNOPQRWMH", "3333");
            //My.LoginedBuckets = new My.Buckets[] { Buckets1, Buckets2, Buckets3 };
            //玻璃效果
            MyGlassEffect Glass = new MyGlassEffect();
            if (MyGlassEffect.GlassEnabled)
            {
                Rectangle ScreenArea = System.Windows.Forms.Screen.GetWorkingArea(this);
                Glass.TopBarSize = dataGridView1.Top;
                Glass.LeftBarSize = 0;
                Glass.BottomBarSize = this.Height - dataGridView1.Top - dataGridView1.Height - 38;
                Glass.RightBarSize = 0;
                Glass.GlassLabel = new Label[] { label1, label2 };
                Glass.ShowEffect(this);
            }
            //窗体属性
            this.ControlBox = false;
            this.Icon = Properties.Resources.logo;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "【" + My.MyProjectName + "】" + "  月翼科技";
            this.TopMost = true;
            this.TopMost = false;
            //动态增加控件
            FormMainButton = new System.Windows.Forms.Button[My.LoginedBuckets.Length];
            for (int i = 0; i < My.LoginedBuckets.Length; i++)
            {
                FormMainButton[i] = new System.Windows.Forms.Button();
                FormMainButton[i].BackColor = System.Drawing.Color.Black;
                FormMainButton[i].Dock = System.Windows.Forms.DockStyle.Left;
                FormMainButton[i].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                FormMainButton[i].ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
                FormMainButton[i].Location = new System.Drawing.Point(0, 0);
                FormMainButton[i].Name = "FormMainButton" + i;
                FormMainButton[i].Size = new System.Drawing.Size(150, 44);
                FormMainButton[i].TabIndex = 0;
                if (My.LoginedBuckets[i].Name.Length > 8)//显示的字数长度限制
                {
                    FormMainButton[i].Text = My.LoginedBuckets[i].Name.Substring(0, 8) + "…";
                }
                else
                {
                    FormMainButton[i].Text = My.LoginedBuckets[i].Name;
                }
                this.toolTip1.SetToolTip(FormMainButton[i], My.LoginedBuckets[i].Name);//鼠标悬停的提示
                FormMainButton[i].UseVisualStyleBackColor = false;
                this.panel1.Controls.Add(FormMainButton[i]);
                FormMainButton[i].Click += new System.EventHandler(this.FormMainButton_Click);
            }
            ChosenButtomIndex = 0;
            FormMainButton[ChosenButtomIndex].FlatAppearance.BorderColor = Color.Fuchsia;
            FormMainButton[ChosenButtomIndex].FlatAppearance.BorderSize = 3;
            this.panel1.Width = My.LoginedBuckets.Length * 150;
        }
        //退出程序
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //My.Show("FormMain_FormClosing");
            My.LoadingForm.Dispose();
        }
        //Buckets按钮点击处理
        public void FormMainButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < My.LoginedBuckets.Length; i++)
            {
                if (sender.Equals(FormMainButton[i]))
                {
                    FormMainButton[ChosenButtomIndex].FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(192, 192, 255);
                    FormMainButton[ChosenButtomIndex].FlatAppearance.BorderSize = 1;
                    ChosenButtomIndex = i;
                    FormMainButton[ChosenButtomIndex].FlatAppearance.BorderColor = Color.Fuchsia;
                    FormMainButton[ChosenButtomIndex].FlatAppearance.BorderSize = 2;
                    My.MainForm.dataGridView1.Rows.Clear();
                    My.MainForm.label2.Text = "正在获取Bucket：" + My.LoginedBuckets[ChosenButtomIndex].Name + "的文件列表……";
                    My.MainForm.Refresh();
                    pictureBox1.Image = null;
                    pictureBox1.ImageLocation = null;
                    My.LoginForm.webBrowser1.Navigate("https://portal.qiniu.com/bucket/"
                        + My.LoginedBuckets[ChosenButtomIndex].Name
                        + "/files?marker=" + ""
                        + "&limit=" + Limit
                        + "&prefix=" + Prefix);
                    //My.Show(FormMainButton[i].Text);
                    break;
                }
            }
        }
        //窗体改变大小（重新布局）
        private void FormMain_Resize(object sender, EventArgs e)
        {
            this.dataGridView1.Columns[0].Width = (this.dataGridView1.Width - 58) * 1 / 6;
            this.dataGridView1.Columns[1].Width = (this.dataGridView1.Width - 58) * 1 / 6;
            this.dataGridView1.Columns[2].Width = (this.dataGridView1.Width - 58) * 1 / 6;
            this.dataGridView1.Columns[3].Width = (this.dataGridView1.Width - 58) * 1 / 6;
            this.dataGridView1.Columns[4].Width = (this.dataGridView1.Width - 58) * 1 / 6;
            this.dataGridView1.Columns[5].Width = (this.dataGridView1.Width - 58) * 1 / 6;
        }
        //图表的左键单击（加载预览图）
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //My.Show(e.RowIndex + " * " + e.ColumnIndex + " ： " + dataGridView1.RowCount);
            //空行也占RowCount
            if (e.RowIndex > -1 && e.RowIndex < dataGridView1.RowCount - 1) ChosenRowIndex = e.RowIndex;
            if (e.ColumnIndex > -1) ChosenColumnIndex = e.ColumnIndex;
            Int32 selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                if (dataGridView1.SelectedRows[0].Cells[1].Value != null && (
                    dataGridView1.SelectedRows[0].Cells[1].Value.ToString() == "image/jpeg" ||
                    dataGridView1.SelectedRows[0].Cells[1].Value.ToString() == "image/bmp" ||
                    dataGridView1.SelectedRows[0].Cells[1].Value.ToString() == "image/png" ||
                    dataGridView1.SelectedRows[0].Cells[1].Value.ToString() == "image/gif"
                ))
                {
                    String Temp = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
                    Temp = Temp.Substring(0, Temp.IndexOf("?"));
                    //My.Show(Temp);
                    if (Temp + "?imageView/2/w/200/h/200" != pictureBox1.ImageLocation)//防止重复加载
                    {
                        My.MainForm.label2.Text = "正在加载预览图：" + Temp + "……";
                        My.MainForm.Refresh();
                        pictureBox1.LoadAsync(Temp + "?imageView/2/w/200/h/200");
                    }
                }
                else
                {
                    My.MainForm.label2.Text = "文件总数为：" + (dataGridView1.RowCount - 1) + "，已选中的数量为：" + selectedRowCount + "。（提示：按住Ctrl键可多选，也可以使用Ctrl组合键，如Ctrl+A。）";
                    My.MainForm.Refresh();
                    pictureBox1.Image = null;
                    pictureBox1.ImageLocation = null;
                }
            }
        }
        //预览图加载完成
        private void pictureBox1_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            pictureBox1.Size = new Size(pictureBox1.Image.Width, pictureBox1.Image.Height);
            pictureBox1.Location = new Point((200 - pictureBox1.Image.Width) / 2, (200 - pictureBox1.Image.Height) / 2);
            String Temp = pictureBox1.ImageLocation;
            if (Temp!=null && Temp.Contains("?"))
            {
                Temp = Temp.Substring(0, Temp.IndexOf("?"));
            }
            My.MainForm.label2.Text = "预览图：" + Temp + "加载完成。";
            My.MainForm.Refresh();
        }
        //下载选中的文件
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                String Temp = "";
                for (int i = 0; i < selectedRowCount; i++)
                {
                    if (dataGridView1.SelectedRows[i].Cells[0].Value != null)
                    {
                        Temp += dataGridView1.SelectedRows[i].Cells[5].Value.ToString() + "\r\n";
                    }
                }
                //清空剪切板内容
                Clipboard.Clear();
                //复制内容到剪切板
                Clipboard.SetData(DataFormats.Text, Temp);
            }
        }
        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                String Temp = "";
                for (int i = 0; i < selectedRowCount; i++)
                {
                    if (dataGridView1.SelectedRows[i].Cells[0].Value != null)
                    {
                        Temp += My.ThunderURL(dataGridView1.SelectedRows[i].Cells[5].Value.ToString()) + "\r\n";
                    }
                }
                //清空剪切板内容
                Clipboard.Clear();
                //复制内容到剪切板
                Clipboard.SetData(DataFormats.Text, Temp);
            }
        }
        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                String Temp = "";
                for (int i = 0; i < selectedRowCount; i++)
                {
                    if (dataGridView1.SelectedRows[i].Cells[0].Value != null)
                    {
                        Temp += My.FlashGetURL(dataGridView1.SelectedRows[i].Cells[5].Value.ToString()) + "\r\n";
                    }
                }
                //清空剪切板内容
                Clipboard.Clear();
                //复制内容到剪切板
                Clipboard.SetData(DataFormats.Text, Temp);
            }
        }
        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                String Temp = "";
                for (int i = 0; i < selectedRowCount; i++)
                {
                    if (dataGridView1.SelectedRows[i].Cells[0].Value != null)
                    {
                        Temp += My.QQdlURL(dataGridView1.SelectedRows[i].Cells[5].Value.ToString()) + "\r\n";
                    }
                }
                //清空剪切板内容
                Clipboard.Clear();
                //复制内容到剪切板
                Clipboard.SetData(DataFormats.Text, Temp);
            }
        }
        //删除选中的文件
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DialogResult Temp = MessageBox.Show(
                "是否要在七牛云上永久删除选中的全部文件？删除后不可恢复，请谨慎操作。",
                "删除确认",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1
                );
            if (Temp == System.Windows.Forms.DialogResult.OK)
            {
                Int32 selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if (selectedRowCount > 0)
                {
                    Byte[] TempBytes = System.Text.Encoding.UTF8.GetBytes("bucket");
                    My.CombineArray(ref TempBytes, System.Text.Encoding.ASCII.GetBytes("="));
                    My.CombineArray(ref TempBytes, System.Text.Encoding.UTF8.GetBytes(My.LoginedBuckets[ChosenButtomIndex].Name));

                    Int32 Temp2 = selectedRowCount;
                    for (int i = 0; i < Temp2; i++)
                    {
                        if (dataGridView1.SelectedRows[i].Cells[0].Value != null)
                        {
                            My.CombineArray(ref TempBytes, System.Text.Encoding.ASCII.GetBytes("&"));
                            My.CombineArray(ref TempBytes, System.Text.Encoding.UTF8.GetBytes("keys[]"));
                            My.CombineArray(ref TempBytes, System.Text.Encoding.ASCII.GetBytes("="));
                            My.CombineArray(ref TempBytes, System.Text.Encoding.UTF8.GetBytes(dataGridView1.SelectedRows[i].Cells[0].Value.ToString()));
                        }
                        else {
                            selectedRowCount = selectedRowCount - 1;//选中了末尾的空行
                        }
                    }

                    if (selectedRowCount > 0)
                    {
                        My.MainForm.label2.Text = "正在删除共计" + selectedRowCount + "个文件……";
                        My.MainForm.Refresh();

                        My.LoginForm.webBrowser1.Navigate(
                            "https://portal.qiniu.com/bucket/moonlord/files/0/delete?bucket=" + My.LoginedBuckets[ChosenButtomIndex].Name,
                            null,
                            TempBytes,
                            "Content-Type:application/x-www-form-urlencoded; charset=UTF-8\r\nReferer:https://portal.qiniu.com/bucket/\r\n"
                        );
                    }
                }
            }
        }
        //复制文件名
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //清空剪切板内容
            Clipboard.Clear();
            //复制内容到剪切板
            Clipboard.SetData(DataFormats.Text, dataGridView1.Rows[ChosenRowIndex].Cells[0].Value.ToString());
        }
        //复制类型
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            //清空剪切板内容
            Clipboard.Clear();
            //复制内容到剪切板
            Clipboard.SetData(DataFormats.Text, dataGridView1.Rows[ChosenRowIndex].Cells[1].Value.ToString());
        }
        //复制大小
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            //清空剪切板内容
            Clipboard.Clear();
            //复制内容到剪切板
            Clipboard.SetData(DataFormats.Text, dataGridView1.Rows[ChosenRowIndex].Cells[2].Value.ToString());
        }
        //复制上传时间
        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            //清空剪切板内容
            Clipboard.Clear();
            //复制内容到剪切板
            Clipboard.SetData(DataFormats.Text, dataGridView1.Rows[ChosenRowIndex].Cells[3].Value.ToString());
        }
        //复制散列值
        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            //清空剪切板内容
            Clipboard.Clear();
            //复制内容到剪切板
            Clipboard.SetData(DataFormats.Text, dataGridView1.Rows[ChosenRowIndex].Cells[4].Value.ToString());
        }
        //复制下载链接
        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            //清空剪切板内容
            Clipboard.Clear();
            //复制内容到剪切板
            Clipboard.SetData(DataFormats.Text, dataGridView1.Rows[ChosenRowIndex].Cells[5].Value.ToString());
        }
        //最小化
        private void pictureButton1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        //最大化
        private void pictureButton2_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal) this.WindowState = FormWindowState.Maximized;
            else if (this.WindowState == FormWindowState.Maximized) this.WindowState = FormWindowState.Normal;
            FormMain_Resize(sender, e);
        }
        //关闭
        private void pictureButton3_Click(object sender, EventArgs e)
        {
            FormMain_FormClosing(sender, new FormClosingEventArgs(new CloseReason(), false));
        }
        //刷新首页
        private void button1_Click(object sender, EventArgs e)
        {
            My.MainForm.dataGridView1.Rows.Clear();
            My.MainForm.label2.Text = "正在获取Bucket：" + My.LoginedBuckets[ChosenButtomIndex].Name + "的文件列表……";
            My.MainForm.Refresh();
            pictureBox1.Image = null;
            pictureBox1.ImageLocation = null;
            My.LoginForm.webBrowser1.Navigate("https://portal.qiniu.com/bucket/"
                + My.LoginedBuckets[ChosenButtomIndex].Name
                + "/files?marker=" + ""
                + "&limit=" + Limit
                + "&prefix=" + Prefix);
            Clipboard.Clear();
            Clipboard.SetText("https://portal.qiniu.com/bucket/"
                + My.LoginedBuckets[ChosenButtomIndex].Name
                + "/files?marker=" + ""
                + "&limit=" + Limit
                + "&prefix=" + Prefix);
        }
        //选中全部
        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            My.MainForm.dataGridView1.SelectAll();
            My.MainForm.label2.Text = "文件总数为：" + (dataGridView1.RowCount - 1) + "。";
            My.MainForm.Refresh();
        }
        //清空选择
        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            My.MainForm.dataGridView1.ClearSelection();
            My.MainForm.label2.Text = "文件总数为：" + (dataGridView1.RowCount - 1) + "。";
            My.MainForm.Refresh();
            pictureBox1.Image = null;
            pictureBox1.ImageLocation = null;
        }
        //下一页
        private void button2_Click(object sender, EventArgs e)
        {
            My.MainForm.dataGridView1.Rows.Clear();
            My.MainForm.label2.Text = "正在获取Bucket：" + My.LoginedBuckets[ChosenButtomIndex].Name + "的文件列表……";
            My.MainForm.Refresh();
            pictureBox1.Image = null;
            pictureBox1.ImageLocation = null;
            //Clipboard.Clear();
            //Clipboard.SetData(DataFormats.Text, Marker);
            //My.Show(Marker);
            My.LoginForm.webBrowser1.Navigate("https://portal.qiniu.com/bucket/"
                + My.LoginedBuckets[ChosenButtomIndex].Name
                + "/files?marker=" + Marker
                + "&limit=" + Limit
                + "&prefix=" + Prefix);
        }
        //前缀
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Prefix = My.ChangeIntoURL(textBox1.Text);
            //My.Show(Prefix);
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) {
                button1_Click(sender, e);
            }
        }
        //条数
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Limit = (int)numericUpDown1.Value;
        }
        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }
        //更多功能
        private void button3_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DialogResult Temp  = MessageBox.Show(
                "选是，将打开软件官网获取新版功能，选否，将打开网页版进行网页操作。", 
                "是否打开官网以获取更多功能？",
                MessageBoxButtons.YesNoCancel, 
                MessageBoxIcon.Information, 
                MessageBoxDefaultButton.Button1
                );
            if(Temp== System.Windows.Forms.DialogResult.Yes){
                System.Diagnostics.Process.Start("EXPLORER.EXE", "http://www.moonlord.cn/blog/");
            }
            else if(Temp== System.Windows.Forms.DialogResult.No){
                //My.LoginForm.FormLogin_FormClosing(sender, new FormClosingEventArgs(new CloseReason(), false));
                My.ResetWebbrowser();
                My.LoginForm.Text = "七牛云存储";
                My.LoginForm.webBrowser1.Navigate("http://portal.qiniu.com/signin");
                My.LoginForm.timer1.Enabled = true;
            }
        }





    }
}
