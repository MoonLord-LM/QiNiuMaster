using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Net;
using System.Web.Security;
using System.Security.Cryptography;
using Microsoft.VisualBasic.CompilerServices;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Win32;

namespace 七牛云存储批量管理工具
{
    public static class My
    {
        //账号信息
        public static int Logined;//0未登录1获取账号信息中2登陆完成
        public static String LoginedEmail;
        public static Buckets[] LoginedBuckets = new Buckets[0];
        public class Buckets
        {
            public String Name = "";
            public String[] Domain = new String[0];
            public File[] FileArray;
            public Buckets()
            {
                Name = "";
                Domain = new String[0];
                FileArray = new File[0];
            }
        }
        public class File {
            public String Key;
            public String MimeType;
            public Decimal Fsize;
            public DateTime PutTime;
            public String Hash;
            public String Url;
            public object[] ToArray(){
                if (Key == null) { Key="Null";}
                if (MimeType == null) { MimeType="Null";}
                if (Fsize == decimal.Zero) { Fsize = 0; }
                if (PutTime == null) { PutTime=DateTime.Now;}
                if (Hash == null) { Hash="Null";}
                if (Url == null) { Url = "Null"; }
                return new object[] { Key, MimeType, Fsize, PutTime, Hash, Url };
            }
        }

        //将Unix时间戳转换为DateTime类型时间
        public static System.DateTime ConvertIntDateTime(double d)
        {
            System.DateTime time = System.DateTime.MinValue;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            time = startTime.AddSeconds(d);
            return time;
        }
        //将c# DateTime时间格式转换为Unix时间戳格式
        public static double ConvertDateTimeInt(System.DateTime time)
        {
            double intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = (time - startTime).TotalSeconds;
            return intResult;
        }

        //窗体静态引用
        public static FormLoading LoadingForm;
        public static FormMain MainForm;
        public static FormLogin LoginForm;

        //临时文件目录
        public static String MyTempPath = (Environment.SystemDirectory).ToString().Replace("system32", "") + "Temp";
        //程序集的名称
        public static String MyProjectName = Assembly.GetExecutingAssembly().GetName().Name;

        // 将资源文件/字符串写入磁盘
        public static void WriteResources(string ResourceName, string NewFilePath)
        {
            try
            {
                //foreach(var FileName in Assembly.GetExecutingAssembly().GetManifestResourceNames()){MessageBox.Show(FileName);}
                //当作为一个资源被嵌入后，资源的完整名称会由项目的默认命名空间与文件名组成。
                Stream ResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(MyProjectName + ".Resources." + ResourceName);
                //这里使用了强制类型转换，所以，资源文件的大小不能超过Int32的最大值，即2G。
                int Count = Convert.ToInt32(ResourceStream.Length);
                //C#的数组和VB.NET不同，new byte[Count]表示Count个元素的数组，数组的最大下标为Count-1。
                byte[] TempBuffer = new byte[Count];
                ResourceStream.Read(TempBuffer, 0, Count);
                ResourceStream.Dispose();
                //及时关闭IO流
                if (System.IO.File.Exists(NewFilePath)) { System.IO.File.Delete(NewFilePath); }
                //文件已存在，则重新写入
                FileStream TempFileStream = new FileStream(NewFilePath, FileMode.OpenOrCreate);
                //这里可能需要新建文件，也可能出现Exception。
                TempFileStream.Write(TempBuffer, 0, Count);
                TempFileStream.Dispose();
                //及时关闭IO流
            }
            catch (Exception) { }
        }
        public static void WriteString(string ResourceString, string NewFilePath)
        {
            try
            {
                StreamWriter SA = new StreamWriter(NewFilePath, false, System.Text.Encoding.GetEncoding("utf-8"));
                SA.Write(ResourceString);
                SA.Dispose();
            }
            catch (Exception) { }
        }
        public static void WriteGBKString(string ResourceString, string NewFilePath)
        {
            try
            {
                StreamWriter SA = new StreamWriter(NewFilePath, false, System.Text.Encoding.GetEncoding("gbk"));
                SA.Write(ResourceString);
                SA.Dispose();
            }
            catch (Exception EX) { My.Show(EX); }
        }
        //资源流
        public static Stream ResourcesStream(string ResourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(MyProjectName + ".Resources." + ResourceName);
        }

        //获取网页源代码By：HttpWebRequest
        public static string GetWebCodeByHttpWebRequest(string url)
        {
            string strHTML = "";
            try
            {
                Uri uri = new Uri(url);
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(uri);
                myReq.UserAgent = "User-Agent:Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705";
                myReq.Accept = "*/*";
                myReq.KeepAlive = true;
                myReq.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                HttpWebResponse result = (HttpWebResponse)myReq.GetResponse();
                Stream receviceStream = result.GetResponseStream();
                StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("utf-8"));
                strHTML = readerOfStream.ReadToEnd();
                readerOfStream.Close();
                receviceStream.Close();
                result.Close();
                return strHTML;
            }
            catch (Exception) { }
            return strHTML;
        }
        //获取网页源代码By：WebRequest
        public static string GetWebCodeByWebRequest(string url)
        {
            string strHTML = "";
            try
            {
                Uri uri = new Uri(url);
                WebRequest myReq = WebRequest.Create(uri);
                WebResponse result = myReq.GetResponse();
                Stream receviceStream = result.GetResponseStream();
                StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("UTF-8"));
                strHTML = readerOfStream.ReadToEnd();
                readerOfStream.Close();
                receviceStream.Close();
                result.Close();
                return strHTML;
            }
            catch (Exception) { }
            return strHTML;
        }
        public static string GetGBKWebCodeByWebRequest(string url)
        {
            string strHTML = "";
            try
            {
                Uri uri = new Uri(url);
                WebRequest myReq = WebRequest.Create(uri);
                WebResponse result = myReq.GetResponse();
                Stream receviceStream = result.GetResponseStream();
                StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("GBK"));
                strHTML = readerOfStream.ReadToEnd();
                readerOfStream.Close();
                receviceStream.Close();
                result.Close();
            }
            catch (Exception) { }
            return strHTML;
        }
        //获取网页源代码By：WebClient
        public static string GetWebCodeByWebClient(string url)
        {
            WebClient myWebClient = new WebClient();
            try
            {
                Stream myStream = myWebClient.OpenRead(url);
                StreamReader sr = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
                string strHTML = sr.ReadToEnd();
                sr.Close();
                myStream.Close();
                if (strHTML == "")
                {
                    myStream = myWebClient.OpenRead(url);
                    sr = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
                    strHTML = sr.ReadToEnd();
                    sr.Close();
                    myStream.Close();
                }
                myWebClient.Dispose();
                return strHTML;
            }
            catch (Exception)
            {
                myWebClient.Dispose();
                return "";
            }
        }

        //从末尾搜索开始和结束字符串进行切割
        public static string BackSeekCode(string SourceCode, string BeginString, string EndString)
        {
            if (!SourceCode.Contains(EndString))
            {
                return "";
            }
            SourceCode = SourceCode.Substring(0, SourceCode.LastIndexOf(EndString));
            if (!SourceCode.Contains(BeginString))
            {
                return "";
            }
            SourceCode = SourceCode.Substring(SourceCode.LastIndexOf(BeginString) + BeginString.Length);
            return SourceCode;
        }
        //从开头搜索开始和结束字符串字符串进行切割（返回第一个/所有的结果）
        public static string SeekCode(string SourceCode, string BeginString, string EndString)
        {
            if (!SourceCode.Contains(BeginString))
            {
                return "";
            }
            SourceCode = SourceCode.Substring(SourceCode.IndexOf(BeginString) + BeginString.Length);
            if (!SourceCode.Contains(EndString))
            {
                return "";
            }
            SourceCode = SourceCode.Substring(0, SourceCode.IndexOf(EndString));
            return SourceCode;
        }
        public static string[] SeekCodeArray(string SourceCode, string BeginString, string EndString)
        {
            string[] temp = new string[0];
            while (SourceCode.Contains(BeginString) && SourceCode.Substring(SourceCode.IndexOf(BeginString) + BeginString.Length).Contains(EndString))
            {
                //SourceCode.Substring(SourceCode.IndexOf(BeginString) + BeginString.Length)
                SourceCode = SourceCode.Substring(SourceCode.IndexOf(BeginString) + BeginString.Length);
                //SourceCode = SourceCode.Substring(0, SourceCode.IndexOf(EndString));
                AddItem(ref temp, SourceCode.Substring(0, SourceCode.IndexOf(EndString)));
                SourceCode = SourceCode.Substring(SourceCode.IndexOf(EndString) + EndString.Length);
            }
            return temp;
        }
        //从开头搜索开始字符串，从末尾搜索结束字符串进行切割（是否包含开始和结束字符串）
        public static string ExtractCode(string SourceCode, string BeginString, string EndString)
        {
            if (!SourceCode.Contains(BeginString))
            {
                return "";
            }
            SourceCode = SourceCode.Substring(SourceCode.IndexOf(BeginString) + BeginString.Length);
            if (!SourceCode.Contains(EndString))
            {
                return "";
            }
            SourceCode = SourceCode.Substring(0, SourceCode.LastIndexOf(EndString));
            return SourceCode;
        }
        public static string ExtractCodeContains(string SourceCode, string BeginString, string EndString)
        {
            if (!SourceCode.Contains(BeginString))
            {
                return "";
            }
            SourceCode = SourceCode.Substring(SourceCode.IndexOf(BeginString));
            if (!SourceCode.Contains(EndString))
            {
                return "";
            }
            SourceCode = SourceCode.Substring(0, SourceCode.LastIndexOf(EndString) + EndString.Length);
            return SourceCode;
        }

        //Base64编码转换
        public static string ChangeIntoBase64(string Source)
        {
            string str = "";
            try
            {
                str = Convert.ToBase64String(Encoding.UTF8.GetBytes(Source));
            }
            catch (Exception) { }
            return str;
        }
        public static string ChangeOutofBase64(string Source)
        {
            string str = "";
            try
            {
                str = Encoding.UTF8.GetString(Convert.FromBase64String(Source));
            }
            catch (Exception) { }
            return str;
        }
        public static string ChangeIntoSafeBase64(string Source)
        {
            string str = "";
            try
            {
                str = Convert.ToBase64String(Encoding.UTF8.GetBytes(Source));
                str = str.Replace("+", "-");
                str = str.Replace("/", "_");
                str = str.Replace("=", "%3D");
            }
            catch (Exception) { }
            return str;
        }
        public static string ChangeOutofSafeBase64(string Source)
        {
            string str = "";
            try
            {
                str = str.Replace("-", "+");
                str = str.Replace("_", "/");
                str = str.Replace("%3D", "=");
                str = Encoding.UTF8.GetString(Convert.FromBase64String(Source));
            }
            catch (Exception) { }
            return str;
        }

        //URL编码转换
        public static string ChangeIntoURL(string Source)
        {
            string str = "";
            try
            {
                str = System.Web.HttpUtility.UrlEncode(Source, System.Text.Encoding.GetEncoding("UTF-8"));
            }
            catch (Exception) { }
            return str;
        }
        public static string ChangeOutofURL(string Source)
        {
            string str = "";
            try
            {
                str = System.Web.HttpUtility.UrlEncode(Source, System.Text.Encoding.GetEncoding("UTF-8"));
            }
            catch (Exception) { }
            return str;
        }

        //删除HTML标签本身（不删除本身中含有内容的标签）
        public static void DeleteHtmlTagsSimple(ref string Source, string HtmlTag)
        {
            Source = Source.Replace("<" + HtmlTag + ">", "");
            Source = Source.Replace("</" + HtmlTag + ">", "");
        }
        //删除HTML标签本身（删除本身中含有内容的标签）
        public static void DeleteHtmlTagsAll(ref string Source, string HtmlTag)
        {
            Source = Source.Replace("<" + HtmlTag + ">", "");
            Source = Source.Replace("</" + HtmlTag + ">", "");
            while (SeekCode(Source, "<" + HtmlTag, ">") != "")
            {
                Source = Source.Replace("<" + HtmlTag + SeekCode(Source, "<" + HtmlTag, ">") + ">", "");
            }
            while (SeekCode(Source, "</" + HtmlTag, ">") != "")
            {
                Source = Source.Replace("</" + HtmlTag + SeekCode(Source, "</" + HtmlTag, ">") + ">", "");
            }
        }
        //尝试将转义的HTML字符转换为其指代的字符
        public static void ChangeToTrueString(ref string Source)
        {
            Source = Source.TrimStart(Convert.ToChar("\"")).TrimEnd(Convert.ToChar("\""));
            Source = Source.Replace(@"\/", "/");
            Source = Source.Replace(@"\\", @"\");
            Source = Source.Replace(@"\\b", @"\b");
            Source = Source.Replace(@"\\f", @"\f");
            Source = Source.Replace(@"\\n", @"\n");
            Source = Source.Replace(@"\\r", @"\r");
            Source = Source.Replace(@"\\t", @"\t");
            Source = Source.Replace("&nbsp;", " ");
            Source = Source.Replace("&lt;", "<");
            Source = Source.Replace("&gt;", ">");
            Source = Source.Replace("&amp;", "&");
            Source = Source.Replace("&quot;", "\"");
            Source = Source.Replace("&apos;", "'");
            Source = Source.Replace("&cent;", "￠");
            Source = Source.Replace("&pound;", "£");
            Source = Source.Replace("&yen;", "¥");
            Source = Source.Replace("&euro;", "€");
            Source = Source.Replace("&sect;", "§");
            Source = Source.Replace("&copy;", "©");
            Source = Source.Replace("&reg;", "®");
            Source = Source.Replace("&trade;", "™");
            Source = Source.Replace("&times;", "×");
            Source = Source.Replace("&divide;", "÷");
            Source = Source.Replace(@"\u003cbr\u003e", "\r\n");
        }
        public static string ChangeToTrueString(string Source)
        {
            Source = Source.TrimStart(Convert.ToChar("\"")).TrimEnd(Convert.ToChar("\""));
            Source = Source.Replace(@"\/", "/");
            Source = Source.Replace(@"\\", @"\");
            Source = Source.Replace(@"\\b", @"\b");
            Source = Source.Replace(@"\\f", @"\f");
            Source = Source.Replace(@"\\n", @"\n");
            Source = Source.Replace(@"\\r", @"\r");
            Source = Source.Replace(@"\\t", @"\t");
            Source = Source.Replace("&nbsp;", " ");
            Source = Source.Replace("&lt;", "<");
            Source = Source.Replace("&gt;", ">");
            Source = Source.Replace("&amp;", "&");
            Source = Source.Replace("&quot;", "\"");
            Source = Source.Replace("&apos;", "'");
            Source = Source.Replace("&cent;", "￠");
            Source = Source.Replace("&pound;", "£");
            Source = Source.Replace("&yen;", "¥");
            Source = Source.Replace("&euro;", "€");
            Source = Source.Replace("&sect;", "§");
            Source = Source.Replace("&copy;", "©");
            Source = Source.Replace("&reg;", "®");
            Source = Source.Replace("&trade;", "™");
            Source = Source.Replace("&times;", "×");
            Source = Source.Replace("&divide;", "÷");
            Source = Source.Replace(@"\u003cbr\u003e", "\r\n");
            return Source;
        }
        //清除空白字符和多余的换行符
        public static void CleanHTML(ref string Source)
        {
            Source = Source.Replace("↑返回页顶↑", "");
            Source = Source.Replace("<!--分割数据的空行begin-->", "");
            Source = Source.Replace("<!--分割数据的空行end-->", "");
            Source = Source.Replace("--<br>", "");
            Source = Source.Replace("\t", "");
            Source = Source.Replace(" ", "");
            while (Source.Contains("\r\n\r\n"))
            {
                Source = Source.Replace("\r\n\r\n", "\r\n");
            }
            Source = Source.TrimStart("\r\n".ToCharArray()).TrimEnd("\r\n".ToCharArray());
        }
        //对字符串进行自动换行操作
        public static void CutString(ref string Source)
        {
            int LineCount = (int)Math.Ceiling(Source.Length * 0.05);
            //取大于运算结果的最小整数
            string[] temp = new string[0];
            for (int i = 0; i < LineCount - 1; i++)
            {
                AddItem(ref temp, Source.Substring(i * 20, 20));
            }
            AddItem(ref temp, Source.Substring((LineCount - 1) * 20));
            //Show(temp);
            Source = String.Join("\r\n", temp);
        }

        //从HTML源码中分离出引用路径
        public static string[] GetHrefCode(string SourceCode)
        {
            SourceCode = SourceCode.Replace(" Href=\"", " href=\"");
            SourceCode = SourceCode.Replace(" HRef=\"", " href=\"");
            SourceCode = SourceCode.Replace(" HrEf=\"", " href=\"");
            SourceCode = SourceCode.Replace(" HreF=\"", " href=\"");
            SourceCode = SourceCode.Replace(" HREf=\"", " href=\"");
            SourceCode = SourceCode.Replace(" HReF=\"", " href=\"");
            SourceCode = SourceCode.Replace(" HrEF=\"", " href=\"");
            SourceCode = SourceCode.Replace(" HREF=\"", " href=\"");
            SourceCode = SourceCode.Replace(" hRef=\"", " href=\"");
            SourceCode = SourceCode.Replace(" hrEf=\"", " href=\"");
            SourceCode = SourceCode.Replace(" hreF=\"", " href=\"");
            SourceCode = SourceCode.Replace(" hREf=\"", " href=\"");
            SourceCode = SourceCode.Replace(" hrEF=\"", " href=\"");
            SourceCode = SourceCode.Replace(" hReF=\"", " href=\"");
            SourceCode = SourceCode.Replace(" hREF=\"", " href=\"");
            string[] strArray2 = new string[0];
            while (SourceCode.Contains(" href=\""))
            {
                strArray2 = (string[])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray((Array)strArray2, new string[strArray2.Length + 1]);
                strArray2[strArray2.Length - 1] = SeekCode(SourceCode, " href=\"", "\"");
                SourceCode = SourceCode.Substring(SourceCode.IndexOf(" href=\"") + 7);
            }
            return strArray2;
        }
        public static string[] GetSrcCode(string SourceCode)
        {
            SourceCode = SourceCode.Replace(" SRC=\"", " src=\"");
            SourceCode = SourceCode.Replace(" sRC=\"", " src=\"");
            SourceCode = SourceCode.Replace(" SrC=\"", " src=\"");
            SourceCode = SourceCode.Replace(" SRc=\"", " src=\"");
            SourceCode = SourceCode.Replace(" srC=\"", " src=\"");
            SourceCode = SourceCode.Replace(" Src=\"", " src=\"");
            SourceCode = SourceCode.Replace(" sRc=\"", " src=\"");
            string[] strArray2 = new string[0];
            while (SourceCode.Contains(" src=\""))
            {
                strArray2 = (string[])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray((Array)strArray2, new string[strArray2.Length + 1]);
                strArray2[strArray2.Length - 1] = SeekCode(SourceCode, " src=\"", "\"");
                SourceCode = SourceCode.Substring(SourceCode.IndexOf(" src=\"") + 6);
            }
            return strArray2;
        }
        public static string[] GetResourcesCode(string SourceCode)
        {
            string[] strArray3 = new string[0];
            string[] strArray2 = new string[] { ".swf", ".mp3", ".jpg", ".gif", ".png", ".js", ".css" };
            int num2 = strArray2.Length - 1;
            for (int i = 0; i <= num2; i++)
            {
                for (string str = SourceCode; str.Contains(strArray2[i] + "\""); str = str.Substring(0, str.LastIndexOf(strArray2[i] + "\"")))
                {
                    strArray3 = (string[])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray((Array)strArray3, new string[strArray3.Length + 1]);
                    strArray3[strArray3.Length - 1] = BackSeekCode(str, "\"", strArray2[i] + "\"") + strArray2[i];
                }
            }
            return strArray3;
        }

        //字符串加密函数
        public static string GetMD5(string Source, int CodeLength, bool Upper = false)
        {
            switch (CodeLength)
            {
                case 0x10:
                    if (Upper)
                    {
                        return FormsAuthentication.HashPasswordForStoringInConfigFile(Source, "MD5").ToUpper().Substring(8, 0x10);
                    }
                    return FormsAuthentication.HashPasswordForStoringInConfigFile(Source, "MD5").ToLower().Substring(8, 0x10);

                case 0x20:
                    if (Upper)
                    {
                        return FormsAuthentication.HashPasswordForStoringInConfigFile(Source, "MD5").ToUpper();
                    }
                    return FormsAuthentication.HashPasswordForStoringInConfigFile(Source, "MD5").ToLower();
            }
            if (Upper)
            {
                return FormsAuthentication.HashPasswordForStoringInConfigFile(Source, "MD5").ToUpper();
            }
            return FormsAuthentication.HashPasswordForStoringInConfigFile(Source, "MD5").ToLower();
        }
        public static string GetSHA(string Source, int CodeLength, bool Upper = false)
        {
            switch (CodeLength)
            {
                case 1:
                    {
                        SHA1CryptoServiceProvider provider = new SHA1CryptoServiceProvider();
                        if (Upper)
                        {
                            return BitConverter.ToString(provider.ComputeHash(Encoding.UTF8.GetBytes(Source))).ToUpper();
                        }
                        return BitConverter.ToString(provider.ComputeHash(Encoding.UTF8.GetBytes(Source))).ToLower();
                    }
                case 0x100:
                    {
                        SHA256Managed managed = new SHA256Managed();
                        if (Upper)
                        {
                            return BitConverter.ToString(managed.ComputeHash(Encoding.UTF8.GetBytes(Source))).ToUpper();
                        }
                        return BitConverter.ToString(managed.ComputeHash(Encoding.UTF8.GetBytes(Source))).ToLower();
                    }
                case 0x180:
                    {
                        SHA384Managed managed2 = new SHA384Managed();
                        if (Upper)
                        {
                            return BitConverter.ToString(managed2.ComputeHash(Encoding.UTF8.GetBytes(Source))).ToUpper();
                        }
                        return BitConverter.ToString(managed2.ComputeHash(Encoding.UTF8.GetBytes(Source))).ToLower();
                    }
                case 0x200:
                    {
                        SHA512Managed managed3 = new SHA512Managed();
                        if (Upper)
                        {
                            return BitConverter.ToString(managed3.ComputeHash(Encoding.UTF8.GetBytes(Source))).ToUpper();
                        }
                        return BitConverter.ToString(managed3.ComputeHash(Encoding.UTF8.GetBytes(Source))).ToLower();
                    }
            }
            SHA512Managed managed4 = new SHA512Managed();
            if (Upper)
            {
                return BitConverter.ToString(managed4.ComputeHash(Encoding.UTF8.GetBytes(Source))).ToUpper();
            }
            return BitConverter.ToString(managed4.ComputeHash(Encoding.UTF8.GetBytes(Source))).ToLower();
        }
        public static string GetSHA1(ref string Source)
        {
            SHA1CryptoServiceProvider provider = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(provider.ComputeHash(Encoding.UTF8.GetBytes(Source))).ToLower().Replace("-", "");
        }

        //获取网页源码，URL，源码，（POST数据），编码格式
        private static WebClient MyClient = new WebClient();
        private static StreamReader MyReader;
        public static bool GetWebCode(string UniformResourceLocator, ref string MyWebCode, Encoding MyEncoding)
        {
            bool flag;
            try
            {
                if (MyEncoding == null)
                {
                    MyReader = new StreamReader(MyClient.OpenRead(UniformResourceLocator), Encoding.Default);
                }
                else
                {
                    MyReader = new StreamReader(MyClient.OpenRead(UniformResourceLocator), MyEncoding);
                }
                MyWebCode = MyReader.ReadToEnd();
                MyReader.Close();
                if (MyWebCode == "")
                {
                    return false;
                }
                flag = true;
            }
            catch (Exception)
            {
                flag = false;
                return flag;
            }
            return flag;
        }
        public static bool GetWebString(string UniformResourceLocator, ref  string MyWebCode, Encoding MyEncoding)
        {
            bool flag;
            try
            {
                if (MyEncoding == null)
                {
                    MyClient.Encoding = Encoding.Default;
                }
                else
                {
                    MyClient.Encoding = MyEncoding;
                }
                MyWebCode = MyClient.DownloadString(UniformResourceLocator);
                if (MyWebCode == "")
                {
                    return false;
                }
                flag = true;
            }
            catch (Exception)
            {
                flag = false;
                return flag;
            }
            return flag;
        }
        public static bool GetWebString(string URL, ref  string MyWebCode, string Data, Encoding MyEncoding)
        {
            bool flag;
            try
            {
                if (MyEncoding == null)
                {
                    MyClient.Encoding = Encoding.Default;
                }
                else
                {
                    MyClient.Encoding = MyEncoding;
                }
                MyClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                byte[] bytes = MyClient.UploadData(URL, "POST", Encoding.UTF8.GetBytes(Data));
                MyWebCode = Encoding.UTF8.GetString(bytes);
                if (MyWebCode == "")
                {
                    return false;
                }
                flag = true;
            }
            catch (Exception)
            {
                flag = false;
                return flag;
            }
            return flag;
        }

        //合并两个数组（将后一个添加到前一个后面）
        public static bool CombineArray(ref byte[] StringArray1, ref byte[] StringArray2)
        {
            double length = StringArray2.Length;
            if (length == 0.0)
            {
                return false;
            }
            double num2 = StringArray1.Length;
            StringArray1 = (byte[])Utils.CopyArray((Array)StringArray1, new byte[((int)Math.Round((double)((num2 + length) - 1.0))) + 1]);
            double num4 = length - 1.0;
            for (double i = 0.0; i <= num4; i++)
            {
                Application.DoEvents();
                StringArray1[(int)Math.Round((double)(num2 + i))] = StringArray2[(int)Math.Round(i)];
            }
            return true;
        }
        public static bool CombineArray(ref byte[] StringArray1, byte[] StringArray2)
        {
            double length = StringArray2.Length;
            if (length == 0.0)
            {
                return false;
            }
            double num2 = StringArray1.Length;
            StringArray1 = (byte[])Utils.CopyArray((Array)StringArray1, new byte[((int)Math.Round((double)((num2 + length) - 1.0))) + 1]);
            double num4 = length - 1.0;
            for (double i = 0.0; i <= num4; i++)
            {
                Application.DoEvents();
                StringArray1[(int)Math.Round((double)(num2 + i))] = StringArray2[(int)Math.Round(i)];
            }
            return true;
        }
        public static bool CombineArray(ref string[] StringArray1, ref string[] StringArray2)
        {
            double length = StringArray2.Length;
            if (length == 0.0)
            {
                return false;
            }
            double num2 = StringArray1.Length;
            StringArray1 = (string[])Utils.CopyArray((Array)StringArray1, new string[((int)Math.Round((double)((num2 + length) - 1.0))) + 1]);
            double num4 = length - 1.0;
            for (double i = 0.0; i <= num4; i++)
            {
                Application.DoEvents();
                StringArray1[(int)Math.Round((double)(num2 + i))] = StringArray2[(int)Math.Round(i)];
            }
            return true;
        }

        //将数组的重复项删除
        public static bool DistinctArray(ref string[] StringArray)
        {
            string[] Temp = new string[0];
            foreach (string temp1 in StringArray)
            {
                Boolean Contains = false;
                foreach (string temp2 in Temp)
                {
                    if (temp1 == temp2) { Contains = true; break; }
                }
                if (Contains == false) { AddItem(ref Temp, temp1); }
            }
            StringArray = Temp;
            return true;
        }

        //给数组添加值（添加到末尾或插入到指定位置）
        public static void AddItem(ref string[] StringArray, string Item)
        {
            double length = StringArray.Length;
            StringArray = (string[])Utils.CopyArray((Array)StringArray, new string[((int)Math.Round(length)) + 1]);
            StringArray[(int)Math.Round(length)] = Item;
        }
        public static void AddItem(ref PictureBox[] PictureBoxArray, PictureBox Item)
        {
            PictureBox[] NewArray = new PictureBox[PictureBoxArray.Length + 1];
            for (int i = 0; i < PictureBoxArray.Length; i++) { NewArray[i] = PictureBoxArray[i]; }
            NewArray[PictureBoxArray.Length] = Item;
            PictureBoxArray = NewArray;
        }
        public static void AddItem(ref Label[] PictureBoxArray, Label Item)
        {
            Label[] NewArray = new Label[PictureBoxArray.Length + 1];
            for (int i = 0; i < PictureBoxArray.Length; i++) { NewArray[i] = PictureBoxArray[i]; }
            NewArray[PictureBoxArray.Length] = Item;
            PictureBoxArray = NewArray;
        }
        public static void AddItem(ref Label[] PictureBoxArray, Label Item, int Index)
        {
            Label[] NewArray = new Label[PictureBoxArray.Length + 1];
            for (int i = 0; i < Index; i++) { NewArray[i] = PictureBoxArray[i]; }
            NewArray[Index] = Item;
            for (int i = Index + 1; i < NewArray.Length; i++) { NewArray[i] = PictureBoxArray[i - 1]; }
            PictureBoxArray = NewArray;
        }
        public static void AddItem(ref string[] StringArray, string Item, int Index)
        {
            string[] NewArray = new string[StringArray.Length + 1];
            for (int i = 0; i < Index; i++) { NewArray[i] = StringArray[i]; }
            NewArray[Index] = Item;
            for (int i = Index + 1; i < NewArray.Length; i++) { NewArray[i] = StringArray[i - 1]; }
            StringArray = NewArray;
        }

        //给数组删除值（删除指定位置）
        public static void DeleteItem(ref string[] StringArray, int Index)
        {
            try
            {
                string[] NewArray = new string[StringArray.Length - 1];
                for (int i = 0; i < Index; i++) { NewArray[i] = StringArray[i]; }
                for (int i = Index; i < NewArray.Length; i++) { NewArray[i] = StringArray[i + 1]; }
                StringArray = NewArray;
            }
            catch (Exception e)
            {
                Show(e);
            }
        }
        public static void DeleteItem(ref Label[] StringArray, int Index)
        {
            try
            {
                Label[] NewArray = new Label[StringArray.Length - 1];
                for (int i = 0; i < Index; i++) { NewArray[i] = StringArray[i]; }
                for (int i = Index; i < NewArray.Length; i++) { NewArray[i] = StringArray[i + 1]; }
                StringArray = NewArray;
            }
            catch (Exception e)
            {
                Show(e);
            }
        }

        //弹出对话框（输出各种类型的变量的值）
        public static void Show()
        {
            System.Windows.Forms.MessageBox.Show("", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static void Show(string Msg)
        {
            System.Windows.Forms.MessageBox.Show(Msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static void Show(Version Msg)
        {
            System.Windows.Forms.MessageBox.Show("【Version类型】\r\n" + Msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static void Show(int Msg)
        {
            System.Windows.Forms.MessageBox.Show("【int类型】\r\n" + Msg.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static void Show(double Msg)
        {
            System.Windows.Forms.MessageBox.Show("【Double类型】\r\n" + Msg.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static void Show(Boolean Msg)
        {
            System.Windows.Forms.MessageBox.Show("【Boolean类型】\r\n" + Msg.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static void Show(Exception Msg)
        {
            System.Windows.Forms.MessageBox.Show("【Exception类型】\r\n" + Msg.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static void Show(Size Msg)
        {
            System.Windows.Forms.MessageBox.Show("【Size类型】\r\n" + Msg.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static void Show(Uri Msg)
        {
            System.Windows.Forms.MessageBox.Show("【Uri类型】\r\n" + Msg.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static void Show(string[] Msg)
        {
            System.Text.StringBuilder temp = new StringBuilder();
            for (int i = 0; i < Msg.Length; i++)
            {
                temp.Append("【" + i + "】" + Msg[i] + "\r\n");
            }
            System.Windows.Forms.MessageBox.Show("【字符串数组，长度" + Msg.Length + "】\r\n" + temp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        //不会阻塞线程的对话框
        public static void ThreadShow(string Msg)
        {
            System.Threading.Thread MyThread = null;
            MyThread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                    {
                        System.Windows.Forms.MessageBox.Show(Msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        MyThread.Abort();
                    }
                    ));
            MyThread.IsBackground = true;
            MyThread.Start();
        }
        //字符串分析
        public static void ShowString(string Msg)
        {
            System.Windows.Forms.MessageBox.Show("【字符串，长度" + Msg.Length + "】\r\n" + Msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static void ShowStringDetail(string Msg)
        {
            System.Text.StringBuilder temp = new StringBuilder();
            for (int i = 0; i < Msg.Length; i++)
            {
                temp.Append("【" + i + "】" + Convert.ToInt32(Msg[i]) + "\r\n");
            }
            System.Windows.Forms.MessageBox.Show("【字符串，长度" + Msg.Length + "】\r\n" + temp.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        //设置IE浏览器的默认解析方法
        public static void ResetWebbrowser()
        {
            //Show(MyProjectName);
            try//32位设置
            {
                RegistryKey RK = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION", true);
                RK.SetValue(MyProjectName + ".exe", 10000);//默认IE10，然后按照网页上!DOCTYPE指令来显示网页
                //RK.SetValue("YourApplication.exe", 10001);//强制IE10
                //IE9：9000或9999
                //My.Show(RK.GetValue(MyProjectName+ ".exe").ToString());
                RK.Close();
            }
            catch (Exception EX) { My.Show(EX); }
            try//64位设置
            {
                RegistryKey RK = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION", true);
                RK.SetValue(MyProjectName + ".exe", 10000);//默认IE10，然后按照网页上!DOCTYPE指令来显示网页
                //RK.SetValue("YourApplication.exe", 10001);//强制IE10
                //IE9：9000或9999
                //My.Show(RK.GetValue(MyProjectName+ ".exe").ToString());
                RK.Close();
            }
            catch (Exception EX) { My.Show(EX); }
            try//显示JSON而不是提示下载
            {
                RegistryKey RK = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type", true);
                RK.CreateSubKey("application/json");
                RK = RK.OpenSubKey("application/json", true);
                RK.SetValue("CLSID", "{25336920-03F9-11cf-8FD0-00AA00686F13}");
                RK.SetValue("Encoding", "dword:00080000");
                RK.Close();
            }
            catch (Exception EX) { My.Show(EX); }
            try//显示JSON而不是提示下载
            {
                RegistryKey RK = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type", true);
                RK.CreateSubKey("text/json");
                RK = RK.OpenSubKey("text/json", true);
                RK.SetValue("CLSID", "{25336920-03F9-11cf-8FD0-00AA00686F13}");
                RK.SetValue("Encoding", "dword:00080000");
                RK.Close();
            }
            catch (Exception EX) { My.Show(EX); }
        }

        //下载链接转换
        public static string ThunderURL(string URL) {
            return "thunder://" + My.ChangeIntoBase64("AA" + URL + "ZZ");
        }
        public static string FlashGetURL(string URL)
        {
            return "Flashget://" + My.ChangeIntoBase64("[FLASHGET]" + URL + "[FLASHGET]");
        }
        public static string QQdlURL(string URL)
        {
            return "qqdl://" + My.ChangeIntoBase64( URL );
        }

    }

}
