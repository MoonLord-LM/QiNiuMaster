using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Media;

namespace 七牛云存储批量管理工具
{
        public class PictureButton : PictureBox
        {
            private int MyHalfStretchSize1;
            private int MyHalfStretchSize2;
            private Image ImageNomal;
            private Image ImageMouseHower;
            private Image ImageMouseDown;
            private int MouseHowerSize;
            private int MouseDownSize;
            public PictureButton()
            {
                base.MouseEnter += new EventHandler(this.PictureButton_MouseEnter);
                base.MouseLeave += new EventHandler(this.PictureButton_MouseLeave);
                base.MouseDown += new MouseEventHandler(this.PictureButton_MouseDown);
                base.MouseUp += new MouseEventHandler(this.PictureButton_MouseUp);
                this.InitializeComponent();
            }
            private void InitializeComponent()
            {
                this.SuspendLayout();
                this.BackColor = Color.Transparent;
                this.BackgroundImageLayout = ImageLayout.Stretch;
                this.ResumeLayout(false);
            }
            private void PictureButton_MouseDown(object sender, MouseEventArgs e)
            {
                this.BackgroundImage = this.ImageMouseDown;
                this.Height -= this.MouseDownSize;
                this.Width -= this.MouseDownSize;
                this.Left += this.MyHalfStretchSize2;
                this.Top += this.MyHalfStretchSize2;
                try { (new SoundPlayer(My.ResourcesStream("down.wav"))).Play(); }
                catch (Exception) { }
            }
            private void PictureButton_MouseEnter(object sender, EventArgs e)
            {
                this.BackgroundImage = this.ImageMouseHower;
                this.Height += this.MouseHowerSize;
                this.Width += this.MouseHowerSize;
                this.Left -= this.MyHalfStretchSize1;
                this.Top -= this.MyHalfStretchSize1;
                try { (new SoundPlayer(My.ResourcesStream("hower.wav"))).Play(); }
                catch (Exception) { }
            }
            private void PictureButton_MouseLeave(object sender, EventArgs e)
            {
                this.Height -= this.MouseHowerSize;
                this.Width -= this.MouseHowerSize;
                this.Left += this.MyHalfStretchSize1;
                this.Top += this.MyHalfStretchSize1;
                this.BackgroundImage = this.ImageNomal;
            }
            private void PictureButton_MouseUp(object sender, MouseEventArgs e)
            {
                this.BackgroundImage = this.ImageMouseHower;
                this.Height += this.MouseDownSize;
                this.Width += this.MouseDownSize;
                this.Left -= this.MyHalfStretchSize2;
                this.Top -= this.MyHalfStretchSize2;
            }
            [Description("按钮正常显示的图片"), Category("外观"),DefaultValue("(None)"),  Browsable(true)]
            public Image Nomal_Image
            {
                get
                {
                    return this.ImageNomal;
                }
                set
                {
                    this.ImageNomal = value;
                }
            }
            [Description("鼠标悬停时显示的图片"), Category("外观"), DefaultValue("(None)"), Browsable(true)]
            public Image MouseHower_Image
            {
                get
                {
                    return this.ImageMouseHower;
                }
                set
                {
                    this.ImageMouseHower = value;
                }
            }
            [Description("鼠标按下时显示的图片"), Category("外观"), DefaultValue("(None)"), Browsable(true)]
            public Image MouseDown_Image
            {
                get
                {
                    return this.ImageMouseDown;
                }
                set
                {
                    this.ImageMouseDown = value;
                }
            }
            [Description("鼠标悬停时增加的大小，建议为偶数"), Category("外观"), DefaultValue(0), Browsable(true)]
            public int Size_MouseHover
            {
                get
                {
                    return this.MouseHowerSize;
                }
                set
                {
                    this.MouseHowerSize = value;
                    this.MyHalfStretchSize1 = (int)Math.Round((double)(((double)this.MouseHowerSize) / 2.0));
                }
            }
            [Description("鼠标按下时减少的大小，建议为偶数"), Category("外观"), DefaultValue(0), Browsable(true)]
            public int Size_MouseDown
            {
                get
                {
                    return this.MouseDownSize;
                }
                set
                {
                    this.MouseDownSize = value;
                    this.MyHalfStretchSize2 = (int)Math.Round((double)(((double)this.MouseDownSize) / 2.0));
                }
            }
        }

        public class ButtonPicture : Button
        {
            private int MyHalfStretchSize1;
            private int MyHalfStretchSize2;
            private Image ImageNomal;
            private Image ImageMouseHower;
            private Image ImageMouseDown;
            private int MouseHowerSize;
            private int MouseDownSize;
            public ButtonPicture()
            {
                base.MouseEnter += new EventHandler(this.PictureButton_MouseEnter);
                base.MouseLeave += new EventHandler(this.PictureButton_MouseLeave);
                base.MouseDown += new MouseEventHandler(this.PictureButton_MouseDown);
                base.MouseUp += new MouseEventHandler(this.PictureButton_MouseUp);
                this.InitializeComponent();
            }
            private void InitializeComponent()
            {
                this.SuspendLayout();
                this.BackColor = Color.Transparent;
                this.BackgroundImageLayout = ImageLayout.Stretch;
                this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.ResumeLayout(false);
            }
            private void PictureButton_MouseDown(object sender, MouseEventArgs e)
            {
                this.BackgroundImage = this.ImageMouseDown;
                this.Height -= this.MouseDownSize;
                this.Width -= this.MouseDownSize;
                this.Left += this.MyHalfStretchSize2;
                this.Top += this.MyHalfStretchSize2;
                try { (new SoundPlayer(My.ResourcesStream("down.wav"))).Play(); }
                catch (Exception) { }
            }
            private void PictureButton_MouseEnter(object sender, EventArgs e)
            {
                this.BackgroundImage = this.ImageMouseHower;
                this.Height += this.MouseHowerSize;
                this.Width += this.MouseHowerSize;
                this.Left -= this.MyHalfStretchSize1;
                this.Top -= this.MyHalfStretchSize1;
                try { (new SoundPlayer(My.ResourcesStream("hower.wav"))).Play(); }
                catch (Exception) { }
            }
            private void PictureButton_MouseLeave(object sender, EventArgs e)
            {
                this.Height -= this.MouseHowerSize;
                this.Width -= this.MouseHowerSize;
                this.Left += this.MyHalfStretchSize1;
                this.Top += this.MyHalfStretchSize1;
                this.BackgroundImage = this.ImageNomal;
            }
            private void PictureButton_MouseUp(object sender, MouseEventArgs e)
            {
                this.BackgroundImage = this.ImageMouseHower;
                this.Height += this.MouseDownSize;
                this.Width += this.MouseDownSize;
                this.Left -= this.MyHalfStretchSize2;
                this.Top -= this.MyHalfStretchSize2;
            }
            [Description("按钮正常显示的图片"), Category("外观"), DefaultValue("(None)"), Browsable(true)]
            public Image Nomal_Image
            {
                get
                {
                    return this.ImageNomal;
                }
                set
                {
                    this.ImageNomal = value;
                }
            }
            [Description("鼠标悬停时显示的图片"), Category("外观"), DefaultValue("(None)"), Browsable(true)]
            public Image MouseHower_Image
            {
                get
                {
                    return this.ImageMouseHower;
                }
                set
                {
                    this.ImageMouseHower = value;
                }
            }
            [Description("鼠标按下时显示的图片"), Category("外观"), DefaultValue("(None)"), Browsable(true)]
            public Image MouseDown_Image
            {
                get
                {
                    return this.ImageMouseDown;
                }
                set
                {
                    this.ImageMouseDown = value;
                }
            }
            [Description("鼠标悬停时增加的大小，建议为偶数"), Category("外观"), DefaultValue(0), Browsable(true)]
            public int Size_MouseHover
            {
                get
                {
                    return this.MouseHowerSize;
                }
                set
                {
                    this.MouseHowerSize = value;
                    this.MyHalfStretchSize1 = (int)Math.Round((double)(((double)this.MouseHowerSize) / 2.0));
                }
            }
            [Description("鼠标按下时减少的大小，建议为偶数"), Category("外观"), DefaultValue(0), Browsable(true)]
            public int Size_MouseDown
            {
                get
                {
                    return this.MouseDownSize;
                }
                set
                {
                    this.MouseDownSize = value;
                    this.MyHalfStretchSize2 = (int)Math.Round((double)(((double)this.MouseDownSize) / 2.0));
                }
            }
        }
}
