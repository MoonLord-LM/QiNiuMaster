using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms.VisualStyles;

//最后编辑：2014.12.30
//MoonLord
//这个类用于在窗体上显示玻璃效果
//示例用法：
//private void FormMain_Load(object sender, EventArgs e)
//{
//            MyGlassEffect Glass = new MyGlassEffect();
//            Glass.ShowEffect(this);
//            if (GlassEffect.GlassEnabled)
//            {
//                Rectangle ScreenArea = System.Windows.Forms.Screen.GetWorkingArea(this);
//                Glass.TopBarSize = ScreenArea.Height;
//                Glass.LeftBarSize = ScreenArea.Width;
//                Glass.BottomBarSize = ScreenArea.Height;
//                Glass.RightBarSize = ScreenArea.Width;
//                Glass.ShowEffect(this);
//            }
//}

namespace 七牛云存储批量管理工具
{
    public class MyGlassEffect
    {
        // Fields
        private static List<WeakReference> __ENCList = new List<WeakReference>();
        [AccessedThroughProperty("mParentForm")]
        private  Form _mParentForm;
        [AccessedThroughProperty("WindowListener")]
        private APIs.HookWindow _WindowListener;
        private bool IsGlassEnabled;
        private Point Last;
        public PictureBox[] GlassPictureBox;
        public Label[] GlassLabel;
        public int LeftBarSize;
        public int RightBarSize;
        public int TopBarSize;
        public int BottomBarSize;
        private bool UseHandCursorOnTitle;

        // Methods
        public MyGlassEffect()
        {
            __ENCAddToList(this);
            this.WindowListener = new APIs.HookWindow();
            this.IsGlassEnabled = GlassEnabled;
            this.UseHandCursorOnTitle = true;
            this.Last = Point.Empty;
        }

        [DebuggerNonUserCode]
        private static void __ENCAddToList(object value)
        {
            List<WeakReference> list = __ENCList;
            lock (list)
            {
                if (__ENCList.Count == __ENCList.Capacity)
                {
                    int index = 0;
                    int num3 = __ENCList.Count - 1;
                    for (int i = 0; i <= num3; i++)
                    {
                        WeakReference reference = __ENCList[i];
                        if (reference.IsAlive)
                        {
                            if (i != index)
                            {
                                __ENCList[index] = __ENCList[i];
                            }
                            index++;
                        }
                    }
                    __ENCList.RemoveRange(index, __ENCList.Count - index);
                    __ENCList.Capacity = __ENCList.Count;
                }
                __ENCList.Add(new WeakReference(RuntimeHelpers.GetObjectValue(value)));
            }
        }

        public static void DrawTextGlow(Graphics Graphics, string text, Font fnt, Rectangle bounds, Color Clr, TextFormatFlags flags)
        {
            IntPtr zero = IntPtr.Zero;
            IntPtr hObject = IntPtr.Zero;
            IntPtr hdc = Graphics.GetHdc();
            IntPtr hDC = APIs.CreateCompatibleDC(hdc);
            APIs.BITMAPINFO pbmi = new APIs.BITMAPINFO();
            APIs.RECT pRect = new APIs.RECT(0, 0, (bounds.Right - bounds.Left) + 30, (bounds.Bottom - bounds.Top) + 30);
            APIs.RECT rect = new APIs.RECT(bounds.Left - 15, bounds.Top - 15, bounds.Right + 15, bounds.Bottom + 15);
            IntPtr ptr = fnt.ToHfont();
            try
            {
                APIs.S_DTTOPTS s_dttopts = new APIs.S_DTTOPTS();
                VisualStyleRenderer renderer = new VisualStyleRenderer(VisualStyleElement.Window.Caption.Active);
                pbmi.bmiHeader.biSize = Marshal.SizeOf(pbmi.bmiHeader);
                pbmi.bmiHeader.biWidth = bounds.Width + 30;
                pbmi.bmiHeader.biHeight = (0 - bounds.Height) - 30;
                pbmi.bmiHeader.biPlanes = 1;
                pbmi.bmiHeader.biBitCount = 0x20;
                IntPtr ptr6 = APIs.CreateDIBSection(hdc, ref pbmi, 0, 0, IntPtr.Zero, 0);
                zero = APIs.SelectObject(hDC, ptr6);
                hObject = APIs.SelectObject(hDC, ptr);
                s_dttopts = new APIs.S_DTTOPTS
                {
                    dwSize = Marshal.SizeOf(s_dttopts),
                    dwFlags = 0x2801,
                    crText = ColorTranslator.ToWin32(Clr),
                    iGlowSize = 0x12
                };
                APIs.DrawThemeTextEx(renderer.Handle, hDC, 0, 0, text, -1, (int)flags, ref pRect, ref s_dttopts);
                APIs.BitBlt(hdc, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, hDC, 0, 0, 0xcc0020);
                APIs.SelectObject(hDC, hObject);
                APIs.SelectObject(hDC, zero);
                APIs.DeleteDC(hDC);
                APIs.DeleteObject(ptr);
                APIs.DeleteObject(ptr6);
                Graphics.ReleaseHdc(hdc);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
            }
        }

        private void Parent_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Location.Y <= this.TopBarSize)
            {
                this.Last = e.Location;
            }
            else
            {
                this.Last = Point.Empty;
            }
        }

        private void Parent_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.Last.Equals(Point.Empty) && (e.Button == MouseButtons.Left))
            {
                Point point4 = new Point((this.ParentForm.Left + e.Location.X) - this.Last.X, (this.ParentForm.Top + e.Location.Y) - this.Last.Y);
                this.ParentForm.Location = point4;
            }
            if (this.UseHandCursorOnTitle)
            {
                if (e.Location.Y < this.TopBarSize)
                {
                    if (!this.ParentForm.Cursor.Equals(Cursors.Hand))
                    {
                        this.ParentForm.Cursor = Cursors.Hand;
                    }
                }
                else if (!this.ParentForm.Cursor.Equals(Cursors.Default))
                {
                    this.ParentForm.Cursor = Cursors.Default;
                }
            }
        }

        public void Parent_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (GlassEnabled)
                {
                    Rectangle rectangle;
                    if (this.TopBarSize > 0)
                    {
                        rectangle = new Rectangle(0, 0, this.ParentForm.ClientSize.Width, this.TopBarSize);
                        e.Graphics.FillRectangle(Brushes.Black, rectangle);
                    }
                    if (this.BottomBarSize > 0)
                    {
                        rectangle = new Rectangle(0, this.ParentForm.ClientSize.Height - this.BottomBarSize, this.ParentForm.ClientSize.Width, this.BottomBarSize);
                        e.Graphics.FillRectangle(Brushes.Black, rectangle);
                    }
                    if (this.RightBarSize > 0)
                    {
                        rectangle = new Rectangle(this.ParentForm.ClientSize.Width - this.RightBarSize, 0, this.RightBarSize, this.ParentForm.ClientSize.Height);
                        e.Graphics.FillRectangle(Brushes.Black, rectangle);
                    }
                    if (this.LeftBarSize > 0)
                    {
                        rectangle = new Rectangle(0, 0, this.LeftBarSize, this.ParentForm.ClientSize.Height);
                        e.Graphics.FillRectangle(Brushes.Black, rectangle);
                    }

                    foreach (Label HeaderLabel in this.GlassLabel)
                    {
                        if ((HeaderLabel != null) && (HeaderLabel.Text.Length > 0))
                        {
                            Console.WriteLine(GlassLabel.Length + "【" + HeaderLabel.Text + "】");
                            HeaderLabel.Visible = false;
                            //_mParentForm.Update();
                            DrawTextGlow(e.Graphics, HeaderLabel.Text, HeaderLabel.Font, HeaderLabel.Bounds, HeaderLabel.ForeColor, TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                        }
                    }

                    for (int i = 0; i < this.GlassPictureBox.Length; i++)
                    {
                        //My.Show(i + "+" + this.GlassPictureBox.Length + GlassPictureBox[i].Image.ToString());
                        if ((GlassPictureBox[i] != null) && (GlassPictureBox[i].Image != null))
                        {
                            GlassPictureBox[i].Visible = false;
                            e.Graphics.DrawImage(GlassPictureBox[i].Image, GlassPictureBox[i].Bounds);
                        }
                        if ((GlassPictureBox[i] != null) && (GlassPictureBox[i].BackgroundImage != null))
                        {
                            GlassPictureBox[i].Visible = false;
                            e.Graphics.DrawImage(GlassPictureBox[i].BackgroundImage, GlassPictureBox[i].Bounds);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Console.WriteLine(ex.ToString());
            }
        }

        private void ParentForm_Resize(object sender, EventArgs e)
        {
            this.ParentForm.Invalidate();
        }

        public void SetGlassEffect(int fromTop = 0, int fromRight = 0, int fromBottom = 0, int fromLeft = 0)
        {
            SetGlassEffect(this.ParentForm, fromTop, fromRight, fromBottom, fromLeft);
            this.ParentForm.Invalidate();
        }

        public static void SetGlassEffect(Form Frm, int fromTop = 0, int fromRight = 0, int fromBottom = 0, int fromLeft = 0)
        {
            if (GlassEnabled && (Frm != null))
            {
                APIs.MARGINS margins = new APIs.MARGINS
                {
                    Top = fromTop,
                    Right = fromRight,
                    Left = fromLeft,
                    Bottom = fromBottom
                };
                APIs.DwmExtendFrameIntoClientArea(Frm.Handle, ref margins);
                Frm.Invalidate();
            }
        }

        public void ShowEffect(Form Parent)
        {
            this.ParentForm = Parent;
            if (this.GlassLabel == null) { this.GlassLabel = new Label[0]; }
            if (this.GlassPictureBox == null) { this.GlassPictureBox = new PictureBox[0]; }
            SetGlassEffect(this.ParentForm, this.TopBarSize, this.RightBarSize, this.BottomBarSize, this.LeftBarSize);
        }

        public void ShowEffect(Form Parent, Label[] HeaderLabel)
        {
            this.ParentForm = Parent;
            this.GlassLabel = HeaderLabel;
            if (this.GlassPictureBox == null) { this.GlassPictureBox = new PictureBox[0]; }
            SetGlassEffect(this.ParentForm, this.TopBarSize, this.RightBarSize, this.BottomBarSize, this.LeftBarSize);
        }

        public void ShowEffect(Form Parent, PictureBox[] HeaderImage)
        {
            this.ParentForm = Parent;
            if (this.GlassLabel == null) { this.GlassLabel = new Label[0]; }
            this.GlassPictureBox = HeaderImage;
            SetGlassEffect(this.ParentForm, this.TopBarSize, this.RightBarSize, this.BottomBarSize, this.LeftBarSize);
        }
        public void ShowEffect(Form Parent, PictureBox HeaderImage)
        {
            this.ParentForm = Parent;
            if (this.GlassLabel == null) { this.GlassLabel = new Label[0]; }
            this.GlassPictureBox =new PictureBox[] {HeaderImage};
            SetGlassEffect(this.ParentForm, this.TopBarSize, this.RightBarSize, this.BottomBarSize, this.LeftBarSize);
        }

        public void ShowEffect(Form Parent, Label[] HeaderLabel, PictureBox[] HeaderImage)
        {
            this.ParentForm = Parent;
            this.GlassLabel = HeaderLabel;
            this.GlassPictureBox = HeaderImage;
            SetGlassEffect(this.ParentForm, this.TopBarSize, this.RightBarSize, this.BottomBarSize, this.LeftBarSize);
        }

        private void WindowListener_MessageArrived(object sender, EventArgs e)
        {
            bool glassEnabled = GlassEnabled;
            if (glassEnabled && !this.IsGlassEnabled)
            {
                GlassEffectEnabledEventHandler glassEffectEnabledEvent = this.GlassEffectEnabled;
                if (glassEffectEnabledEvent != null)
                {
                    glassEffectEnabledEvent(this, new EventArgs());
                }
            }
            else if (!glassEnabled && this.IsGlassEnabled)
            {
                GlassEffectDisabledEventHandler glassEffectDisabledEvent = this.GlassEffectDisabled;
                if (glassEffectDisabledEvent != null)
                {
                    glassEffectDisabledEvent(this, new EventArgs());
                }
            }
            this.IsGlassEnabled = glassEnabled;
        }

        public static bool GlassEnabled
        {
            get
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    bool flag3 = false;
                    APIs.DwmIsCompositionEnabled(ref flag3);
                    return flag3;
                }
                return false;
            }
        }

        private Form mParentForm
        {
            [DebuggerNonUserCode]
            get
            {
                return this._mParentForm;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                EventHandler handler = new EventHandler(this.ParentForm_Resize);
                MouseEventHandler handler2 = new MouseEventHandler(this.Parent_MouseMove);
                MouseEventHandler handler3 = new MouseEventHandler(this.Parent_MouseDown);
                PaintEventHandler handler4 = new PaintEventHandler(this.Parent_Paint);
                if (this._mParentForm != null)
                {
                    this._mParentForm.Resize -= handler;
                    this._mParentForm.MouseMove -= handler2;
                    this._mParentForm.MouseDown -= handler3;
                    this._mParentForm.Paint -= handler4;
                }
                this._mParentForm = value;
                if (this._mParentForm != null)
                {
                    this._mParentForm.Resize += handler;
                    this._mParentForm.MouseMove += handler2;
                    this._mParentForm.MouseDown += handler3;
                    this._mParentForm.Paint += handler4;
                }
            }
        }

        public Form ParentForm
        {
            get
            {
                return this.mParentForm;
            }
            set
            {
                this.mParentForm = value;
            }
        }

        private APIs.HookWindow WindowListener
        {
            [DebuggerNonUserCode]
            get
            {
                return this._WindowListener;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                APIs.HookWindow.MessageArrivedEventHandler handler = new APIs.HookWindow.MessageArrivedEventHandler(this.WindowListener_MessageArrived);
                if (this._WindowListener != null)
                {
                    this._WindowListener.MessageArrived -= handler;
                }
                this._WindowListener = value;
                if (this._WindowListener != null)
                {
                    this._WindowListener.MessageArrived += handler;
                }
            }
        }

        // Nested Types
        public class APIs
        {
            // Fields
            public const int DTT_COMPOSITED = 0x2000;
            public const int DTT_GLOWSIZE = 0x800;
            public const int DTT_TEXTCOLOR = 1;
            public const int SRCCOPY = 0xcc0020;
            public const int WM_SYSCOLORCHANGE = 0x15;

            // Methods
            [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint iUsage, int ppvBits, IntPtr hSection, uint dwOffset);
            [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern bool DeleteDC(IntPtr hdc);
            [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("UxTheme.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
            public static extern int DrawThemeTextEx(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref RECT pRect, ref S_DTTOPTS pOptions);
            [DllImport("dwmapi.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
            [DllImport("dwmapi.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern void DwmIsCompositionEnabled(ref bool IsIt);
            [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

            // Nested Types
            [StructLayout(LayoutKind.Sequential)]
            public struct BITMAPINFO
            {
                public MyGlassEffect.APIs.BITMAPINFOHEADER bmiHeader;
                public MyGlassEffect.APIs.RGBQUAD bmiColors;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct BITMAPINFOHEADER
            {
                public int biSize;
                public int biWidth;
                public int biHeight;
                public short biPlanes;
                public short biBitCount;
                public int biCompression;
                public int biSizeImage;
                public int biXPelsPerMeter;
                public int biYPelsPerMeter;
                public int biClrUsed;
                public int biClrImportant;
            }

            public class HookWindow : NativeWindow
            {
                // Fields
                private static List<WeakReference> __ENCList = new List<WeakReference>();

                // Events
                public event MessageArrivedEventHandler MessageArrived;

                // Methods
                public HookWindow()
                {
                    __ENCAddToList(this);
                    CreateParams cp = new CreateParams();
                    this.CreateHandle(cp);
                }

                [DebuggerNonUserCode]
                private static void __ENCAddToList(object value)
                {
                    List<WeakReference> list = __ENCList;
                    lock (list)
                    {
                        if (__ENCList.Count == __ENCList.Capacity)
                        {
                            int index = 0;
                            int num3 = __ENCList.Count - 1;
                            for (int i = 0; i <= num3; i++)
                            {
                                WeakReference reference = __ENCList[i];
                                if (reference.IsAlive)
                                {
                                    if (i != index)
                                    {
                                        __ENCList[index] = __ENCList[i];
                                    }
                                    index++;
                                }
                            }
                            __ENCList.RemoveRange(index, __ENCList.Count - index);
                            __ENCList.Capacity = __ENCList.Count;
                        }
                        __ENCList.Add(new WeakReference(RuntimeHelpers.GetObjectValue(value)));
                    }
                }

                protected override void WndProc(ref Message m)
                {
                    if (m.Msg == 0x15)
                    {
                        MessageArrivedEventHandler messageArrivedEvent = this.MessageArrived;
                        if (messageArrivedEvent != null)
                        {
                            messageArrivedEvent(this, new EventArgs());
                        }
                    }
                    base.WndProc(ref m);
                }

                // Nested Types
                public delegate void MessageArrivedEventHandler(object sender, EventArgs e);
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MARGINS
            {
                public int Left;
                public int Right;
                public int Top;
                public int Bottom;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
                public RECT(int iLeft, int iTop, int iRight, int iBottom)
                {
                    this = new MyGlassEffect.APIs.RECT();
                    this.Left = iLeft;
                    this.Top = iTop;
                    this.Right = iRight;
                    this.Bottom = iBottom;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RGBQUAD
            {
                public byte rgbBlue;
                public byte rgbGreen;
                public byte rgbRed;
                public byte rgbReserved;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct S_DTTOPTS
            {
                public int dwSize;
                public int dwFlags;
                public int crText;
                public int crBorder;
                public int crShadow;
                public int iTextShadowType;
                public Point ptShadowOffset;
                public int iBorderSize;
                public int iFontPropId;
                public int iColorPropId;
                public int iStateId;
                public bool fApplyOverlay;
                public int iGlowSize;
                public int pfnDrawTextCallback;
                public IntPtr lParam;
            }
        }

        // Events
        public event GlassEffectDisabledEventHandler GlassEffectDisabled;
        public event GlassEffectEnabledEventHandler GlassEffectEnabled;
        public delegate void GlassEffectDisabledEventHandler(object sender, EventArgs e);
        public delegate void GlassEffectEnabledEventHandler(object sender, EventArgs e);
    }
}
