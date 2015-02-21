using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 七牛云存储批量管理工具
{
    public partial class FormLogin : Form
    {
        private int Index = 0;
        public FormLogin()
        {
            InitializeComponent();
        }
        //加载
        private void FormLogin_Load(object sender, EventArgs e)
        {
            //登录前Cookie：
            //pgv_pvi=3632438272;
            //Hm_lvt_204fcf6777f8efa834fe7c45a2336bf1=1421482609,1421483194;
            //mp_fbfbacb18f1ac527ede6ca993f5fbb18_mixpanel=%7B%22distinct_id%22%3A%20%2214b8710c26d147-0b66ecf27-464c0021-100200-14b8710c26e11a%22%2C%22%24initial_referrer%22%3A%20%22%24direct%22%2C%22%24initial_referring_domain%22%3A%20%22%24direct%22%7D;
            //PORTAL_SESSION=MTdKVkNXRzVTVVc3R1hZUFBBTVNEQlgwMU5NQ1pYN1IsMTQyNDE2NjY0ODIxNTI5MDc3MyxjNDc1NDMzNDljZTgwMTFkZDRhOGQ0MDNkZjMyNTkxMGM1YTkxMjRh;
            //_ga=GA1.2.1889280735.1414002006;
            //pgv_si=s1256732672;
            //PORTAL_FLASH=

            //登录后Cookie：
            //pgv_pvi=3632438272;
            //Hm_lvt_204fcf6777f8efa834fe7c45a2336bf1=1421482609,1421483194;
            //mp_fbfbacb18f1ac527ede6ca993f5fbb18_mixpanel=%7B%22distinct_id%22%3A%20%2214b8710c26d147-0b66ecf27-464c0021-100200-14b8710c26e11a%22%2C%22%24initial_referrer%22%3A%20%22%24direct%22%2C%22%24initial_referring_domain%22%3A%20%22%24direct%22%7D;
            //PORTAL_SESSION=MTdKVkNXRzVTVVc3R1hZUFBBTVNEQlgwMU5NQ1pYN1IsMTQyNDE2NjY0ODIxNTI5MDc3MyxjNDc1NDMzNDljZTgwMTFkZDRhOGQ0MDNkZjMyNTkxMGM1YTkxMjRh;
            //pgv_si=s1256732672;
            //_ga=GA1.2.1889280735.1414002006; 
            //_utm_uidn=true;
            //PORTAL_FLASH=%00username%3A178910432%40qq.com%00
            this.Opacity = 0.01;
            My.ResetWebbrowser();
            webBrowser1.Navigate("http://portal.qiniu.com/signin");
            timer1.Enabled = true;
        }
        //显示界面
        private void webBrowser1_DocumentCompleted1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (this.Opacity == 0.01) { 
                this.Opacity = 1;
                this.TopMost = true;
                this.BringToFront();
                this.TopMost = false;
            }
        }
        private void webBrowser1_DocumentCompleted2(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            CheckGetFile();
            CheckDeleteFile();
        }
        //定时器进行登陆判定和跳转
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (webBrowser1.Url != null && webBrowser1.ReadyState == WebBrowserReadyState.Complete)
            {
                timer1.Enabled = false;
                //My.Show(webBrowser1.Document.Cookie);
                //My.Show(webBrowser1.DocumentText);
                if (webBrowser1.Url != null && webBrowser1.ReadyState == WebBrowserReadyState.Complete && webBrowser1.Url.ToString() == "https://portal.qiniu.com/")
                {
                    if (My.Logined < 2)
                    {
                        this.Opacity = 0;
                        this.ShowInTaskbar = false;
                        My.Logined = 1;
                        My.LoginedEmail = My.SeekCode(webBrowser1.DocumentText, "_mail = \"", "\";");
                        //My.Show(My.LoginEmail);
                        webBrowser1.Navigate("https://portal.qiniu.com/api/rs/buckets");
                    }
                    else
                    {
                        My.LoginForm.Opacity = 1;
                        My.LoginForm.ShowInTaskbar = true;
                        My.LoginForm.TopMost = true;
                        My.LoginForm.BringToFront();
                        My.LoginForm.TopMost = false;
                    }
                }
                if (webBrowser1.Url != null && webBrowser1.ReadyState == WebBrowserReadyState.Complete && webBrowser1.Url.ToString() == "https://portal.qiniu.com/api/rs/buckets")
                {
                    Index = 0;
                    string[] TempString = My.SeekCodeArray(webBrowser1.DocumentText, "\"", "\"");
                    My.LoginedBuckets = new My.Buckets[TempString.Length];
                    for (int i = 0; i < TempString.Length; i++)
                    {
                        My.LoginedBuckets[i] = new My.Buckets();
                        My.LoginedBuckets[i].Name = TempString[i];
                        //My.Show(TempString[i]);
                    }
                    if (TempString.Length > Index)
                    {
                        Index = Index + 1;
                        Byte[] TempBytes = System.Text.Encoding.UTF8.GetBytes("bucket");
                        My.CombineArray(ref TempBytes, System.Text.Encoding.ASCII.GetBytes("="));
                        My.CombineArray(ref TempBytes, System.Text.Encoding.UTF8.GetBytes(My.LoginedBuckets[Index - 1].Name));
                        webBrowser1.Navigate(
                            "https://portal.qiniu.com/api/uc/bucketInfo?name=" + My.LoginedBuckets[Index - 1].Name,
                            null,
                            TempBytes,
                            "Content-Type: application/x-www-form-urlencoded\r\nReferer:https://portal.qiniu.com/bucket/\r\n"
                        );
                    }
                }
                if (webBrowser1.Url != null && webBrowser1.ReadyState == WebBrowserReadyState.Complete && webBrowser1.Url.ToString().Contains("https://portal.qiniu.com/api/uc/bucketInfo?name="))
                {
                    string TempName = webBrowser1.Url.ToString().Replace("https://portal.qiniu.com/api/uc/bucketInfo?name=", "");
                    for (int i = 0; i < My.LoginedBuckets.Length; i++)
                    {
                        if (My.LoginedBuckets[i].Name == TempName)
                        {
                            My.LoginedBuckets[i].Domain = My.SeekCodeArray(webBrowser1.DocumentText.Substring(webBrowser1.DocumentText.IndexOf("\"bind_domains\":") + 15), "\"", "\"");
                            //My.Show(My.LoginedBuckets[i].Domain);
                            //这里的My.Show会阻拦下面的代码执行，导致弹出N个对话框
                            break;
                        }
                    }
                    if (My.LoginedBuckets.Length > Index)
                    {
                        Index = Index + 1;
                        Byte[] TempBytes = System.Text.Encoding.UTF8.GetBytes("bucket");
                        My.CombineArray(ref TempBytes, System.Text.Encoding.ASCII.GetBytes("="));
                        My.CombineArray(ref TempBytes, System.Text.Encoding.UTF8.GetBytes(My.LoginedBuckets[Index - 1].Name));
                        webBrowser1.Navigate(
                            "https://portal.qiniu.com/api/uc/bucketInfo?name=" + My.LoginedBuckets[Index - 1].Name,
                            null,
                            TempBytes,
                            "Content-Type: application/x-www-form-urlencoded\r\nReferer:https://portal.qiniu.com/bucket/\r\n"
                        );
                    }
                    else
                    {
                        webBrowser1.Navigate("https://portal.qiniu.com/bucket/"
                            + My.LoginedBuckets[0].Name
                            + "/files?marker=" + ""
                            + "&limit=" + My.MainForm.Limit
                            + "&prefix=" + My.MainForm.Prefix);
                    }
                }
            if (CheckGetFile() == true)
            {
                My.Logined = 2;
                webBrowser1.DocumentCompleted
                    -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted1);
                webBrowser1.DocumentCompleted
                    += new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted2);
            }
            if (My.Logined < 2) { timer1.Enabled = true; }
            }
        }
        //关闭窗体
        public void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (My.Logined < 2)
            {
                e.Cancel = true;
                this.Opacity = 0.01;
                this.ShowInTaskbar = false;
                My.ResetWebbrowser();
                webBrowser1.Navigate("http://portal.qiniu.com/signin");
                timer1.Enabled = true;
                //My.Show("FormLogin_FormClosing");
            }
            else {
                e.Cancel = true;
                this.Opacity = 0;
                this.ShowInTaskbar = false;
            }
        }
        //检查登录和获取文件
        private Boolean CheckGetFile()
        {
            if (webBrowser1.Url != null && webBrowser1.ReadyState == WebBrowserReadyState.Complete && webBrowser1.Url.ToString().Contains("https://portal.qiniu.com/bucket/") && webBrowser1.Url.ToString().Contains("?marker="))
            {
                My.MainForm.dataGridView1.Columns.Clear();
                My.MainForm.dataGridView1.Rows.Clear();
                DataGridViewTextBoxColumn Column1 = new DataGridViewTextBoxColumn();
                DataGridViewTextBoxColumn Column2 = new DataGridViewTextBoxColumn();
                DataGridViewTextBoxColumn Column3 = new DataGridViewTextBoxColumn();
                DataGridViewTextBoxColumn Column4 = new DataGridViewTextBoxColumn();
                DataGridViewTextBoxColumn Column5 = new DataGridViewTextBoxColumn();
                DataGridViewTextBoxColumn Column6 = new DataGridViewTextBoxColumn();
                Column1.HeaderText = "文件名 Key";
                Column2.HeaderText = "类型 MimeType";
                Column3.HeaderText = "大小 Fsize";
                Column4.HeaderText = "上传时间 PutTime";
                Column5.HeaderText = "散列值 Hash";
                Column6.HeaderText = "下载链接 Url";
                Column1.Width = (My.MainForm.dataGridView1.Width - 58) * 1 / 6;
                Column2.Width = (My.MainForm.dataGridView1.Width - 58) * 1 / 6;
                Column3.Width = (My.MainForm.dataGridView1.Width - 58) * 1 / 6;
                Column4.Width = (My.MainForm.dataGridView1.Width - 58) * 1 / 6;
                Column5.Width = (My.MainForm.dataGridView1.Width - 58) * 1 / 6;
                Column6.Width = (My.MainForm.dataGridView1.Width - 58) * 1 / 6;
                My.MainForm.dataGridView1.Columns.Add(Column1);
                My.MainForm.dataGridView1.Columns.Add(Column2);
                My.MainForm.dataGridView1.Columns.Add(Column3);
                My.MainForm.dataGridView1.Columns.Add(Column4);
                My.MainForm.dataGridView1.Columns.Add(Column5);
                My.MainForm.dataGridView1.Columns.Add(Column6);
                String[] Data = new String[0];
                //无数据：{"bucketType":"public","hasSensitiveWord":false,"items":[]}
                if (webBrowser1.DocumentText == "\"获取空间文件列表失败\"")
                {
                    My.MainForm.label2.Text = "七牛云反馈：获取空间文件列表失败。推测可能为网络或服务器故障，请稍后再试。";
                    My.MainForm.Refresh();
                    My.Show("七牛云反馈：获取空间文件列表失败。推测可能为网络或服务器故障，请稍后再试。");
                }
                else if (webBrowser1.DocumentText.Contains("\"items\":[]"))
                {
                    My.MainForm.label2.Text = "文件列表获取完成，满足条件的文件的数量为0。";
                    My.MainForm.Refresh();
                }
                else if (My.ExtractCode(webBrowser1.DocumentText, "\"items\":[{", "}]")=="")
                {
                    My.MainForm.label2.Text = "文件列表获取失败，请稍后再试。";
                    My.MainForm.Refresh();
                }
                else
                {
                    //My.Show(webBrowser1.DocumentText.Contains("\"items\":[{"));
                    //My.Show(webBrowser1.DocumentText.Contains("}],\"marker\""));
                    Data = System.Text.RegularExpressions.Regex.Split(My.ExtractCode(webBrowser1.DocumentText, "\"items\":[{", "}]"), "},{");
                    //My.Show(Data);
                    for (int i = 0; i < Data.Length; i++)
                    {
                        try
                        {
                            My.MainForm.dataGridView1.Rows.Add(
                                My.SeekCode(Data[i], "\"key\":\"", "\""),
                                My.SeekCode(Data[i], "\"mimeType\":\"", "\""),
                                Convert.ToDecimal(Convert.ToDouble(My.SeekCode(Data[i], "\"fsize\":", ","))),
                                My.ConvertIntDateTime((double)Convert.ToDouble(My.SeekCode(Data[i], "\"putTime\":", ",")) / 10000000),
                                My.SeekCode(Data[i], "\"hash\":\"", "\""),
                                My.SeekCode(Data[i], "\"signed_download_url\":\"", "\"")
                            );
                        }
                        catch (Exception EX)
                        {
                            My.Show(EX.ToString());
                            //My.Show(webBrowser1.DocumentText);
                            //My.Show(Data[i]);
                        }
                    }
                    My.MainForm.dataGridView1.ClearSelection();

                    String Temp = My.MainForm.dataGridView1.Rows[My.MainForm.dataGridView1.RowCount - 2].Cells[0].Value.ToString();
                    Temp = "{\"c\":0,\"k\":\"" + Temp + "\"}";
                    My.MainForm.Marker = My.ChangeIntoSafeBase64(Temp);

                    My.MainForm.label2.Text = "文件列表获取完成，满足条件的文件的数量为" + Data.Length + "。";
                    My.MainForm.Refresh();
                }
                return true;
            }
            return false;
        }
        //删除文件
        private void CheckDeleteFile()
        {
            if (webBrowser1.Url != null && webBrowser1.ReadyState == WebBrowserReadyState.Complete && webBrowser1.Url.ToString().Contains("https://portal.qiniu.com/bucket/moonlord/files/0/delete?bucket="))
            {
                if (webBrowser1.DocumentText.Contains("\"failed_keys\":[]"))
                {
                    My.Show("所有选中的文件都成功删除。");
                }
                else
                {
                    My.Show("选中的部分文件删除失败。");
                }
                My.MainForm.FormMainButton_Click(My.MainForm.FormMainButton[My.MainForm.ChosenButtomIndex], new EventArgs());
            }
        }
    }

    public class SimpleJson
    {
        //存储索引和值的两个数组
        public string[] Index;
        public string[] Value;
        //构造函数
        //JSON格式：{"age":21 ,"bir_d":17 }
        public SimpleJson(string JsonString)
        {
            string[] Temp = JsonString.Trim().
                TrimStart(Convert.ToChar("{")).
                TrimEnd(Convert.ToChar("}")).
                Split((",").ToCharArray());
            //MessageBox.Show(Temp.Length.ToString());
            Index = new string[Temp.Length];
            Value = new string[Temp.Length];
            for (int i = 0; i < Temp.Length; i++)
            {
                string[] TempEX = Temp[i].Trim().
                Split((":").ToCharArray());
                //MessageBox.Show(i.ToString());
                Index[i] = TempEX[0].TrimStart(Convert.ToChar("\"")).TrimEnd(Convert.ToChar("\""));
                Value[i] = Temp[i].Trim().Substring(TempEX[0].Length + 1);
                //MessageBox.Show(Index[i] + "@" + Value[i]);
            }
        }
        //查询函数
        public string GetValue(string IndexString)
        {
            int Temp = -1;
            for (int i = 0; i < Index.Length; i++)
            {
                if (Index[i] == IndexString) { Temp = i; break; }
            }
            if (Temp != -1) { return Value[Temp]; }
            //添加左右的引号再次查询一遍
            else
            {
                IndexString = "\"" + IndexString + "\"";
                for (int i = 0; i < Index.Length; i++)
                {
                    if (Index[i] == IndexString) { Temp = i; break; }
                }
                if (Temp != -1) { return Value[Temp]; }
                //去掉左右的引号再次查询一遍
                else
                {
                    IndexString = IndexString.Replace("\"", "");
                    for (int i = 0; i < Index.Length; i++)
                    {
                        if (Index[i] == IndexString) { Temp = i; break; }
                    }
                    if (Temp != -1) { return Value[Temp]; }
                    //查询无结果，返回空字符串
                    else { return ""; }
                }
            }
        }
        public string GetString(string IndexString)
        {
            return My.ChangeToTrueString(GetValue(IndexString));
        }

    }
}
