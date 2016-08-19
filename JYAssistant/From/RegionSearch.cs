using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JYAssistant
{
    //定义委托判断是否开启选择区域
    public delegate void IsRegion(bool isRegion);
    //定义委托回传rect参数并回调函数使用rect参数进行搜索
    public delegate void OperationRectPara(Rectangle rect);
    //声明主窗体显隐的委托
    public delegate void SetMainFromShow();

    public partial class RegionSearch : Form
    {
        //声明IsRegion的事件
        public event IsRegion IsRegionEvent;
        //声明OperationRectPara的事件
        public event OperationRectPara ORPEvent;
        //声明主窗体显示的事件
        public event SetMainFromShow SetMFSEvent;

        //鼠标位置枚举
        private enum MouseLocation
        {
            LeftUpPoint,
            LeftDownPoint,
            RightUpPoint,
            RightDownPoint,
            LeftLine,
            RightLine,
            UpLine,
            DownLine,
            InRectangle,
            OutOfRectangle,
        }
        private MouseLocation mouseLocation;
        //屏幕原始图片
        private Bitmap originBmp;
        //选定区域是否可以调节
        private bool isAdjust;
        //选定区域矩形框
        private Rectangle rect;
        //按下鼠标左键的坐标
        private Point mouseDownPoint;
        //调节选定区域矩形框是的固定不动点
        private Point fixedPoint;
        //截图状态
        private bool isCapture;
        //绘制状态
        private bool isDraw;

        public RegionSearch()
        {
            isCapture = false;
            isDraw = false;
            InitializeComponent();
            //TopMost = false;
        }

        //加载选择区域时进行的处理
        private void RegionSearch_Load(object sender, EventArgs e)
        {
            //设置区域选择开始
            IsRegionEvent(true);
            //以当前窗体大小创建空白图片
            originBmp = new Bitmap(Width, Height);
            //截屏图片作为画板
            using (Graphics gs = Graphics.FromImage(originBmp))
            {
                //复制当前屏幕到画板上
                gs.CopyFromScreen(0, 0, 0, 0, Size);
            }
            //截屏图片作为当前窗体背景
            BackgroundImage = new Bitmap(originBmp);
            //在窗体上绘制黑色半透明遮罩
            using (Graphics blackgs = Graphics.FromImage(BackgroundImage))
            {
                using (SolidBrush backbrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                {
                    blackgs.FillRectangle(backbrush, 0, 0, Width, Height);
                }
            }
            //显示并激活当前窗体
            Show();
            Activate();
        }

        //取消
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        //确定
        private void button2_Click(object sender, EventArgs e)
        {
            ORPEvent(rect);
            Close();
        }

        //窗口关闭之前传递选取区域状态为否
        private void RegionSearch_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsRegionEvent(false);
            SetMFSEvent();
        }

        //添加ESC退出
        private void RegionSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyValue == 27)
            {
                Close();
            }
        }
        
        //鼠标左键按下时
        private void RegionSearch_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                //没有截图时，截图状态为可以截图
                if(isCapture == false)
                {
                    //设置允许绘制
                    isDraw = true;
                }
                else
                {
                    //有截图再按下左键是调节矩形框的大小
                    if(MousePosition.X > rect.Left - 3 && MousePosition.X < rect.Right + 3 && MousePosition.Y > rect.Top - 3 && MousePosition.Y < rect.Bottom + 3)
                    {
                        isAdjust = true;
                    }
                }
                //保存鼠标左键的坐标
                mouseDownPoint = new Point(e.X, e.Y);
            }
            //保存鼠标位置形状
            CursorLocation();
        }

        //鼠标拖动时
        private void RegionSearch_MouseMove(object sender, MouseEventArgs e)
        {
            //移动鼠标绘制矩形框
            if(isDraw == true)
            {
                //鼠标按下并移动设定iscapture为true
                isCapture = true;
                //保存矩形框左上角点的坐标
                Point leftUpPoint = new Point(mouseDownPoint.X, mouseDownPoint.Y);
                //鼠标往xy的逆方向移动，产生的坐标值小于原值
                if(e.X < mouseDownPoint.X)
                {
                    leftUpPoint.X = e.X;
                }
                if(e.Y < mouseDownPoint.Y)
                {
                    leftUpPoint.Y = e.Y;
                }
                //获取矩形的长和宽
                int rectWidth = Math.Abs(mouseDownPoint.X - e.X);
                int rectHeight = Math.Abs(mouseDownPoint.Y - e.Y);
                //避免长宽为零的截图区域
                if(rectWidth == 0)
                {
                    rectWidth++;
                }
                if(rectHeight == 0)
                {
                    rectHeight++;
                }
                //保存绘制的矩形框
                rect = new Rectangle(leftUpPoint.X, leftUpPoint.Y, rectWidth, rectHeight);
                Refresh();
            }
            //调节矩形框
            else
            {
                if(isCapture == true)
                {
                    //截图完成，显示左键在当前矩形框上的形状
                    if(isAdjust == false)
                    {
                        CursorLocation();
                    }
                    else
                    {
                        //调节矩形框的大小和形状
                        Point leftUpPoint = new Point(fixedPoint.X, fixedPoint.Y);
                        if(e.X < fixedPoint.X)
                        {
                            leftUpPoint.X = e.X;
                        }
                        if(e.Y < fixedPoint.Y)
                        {
                            leftUpPoint.Y = e.Y;
                        }
                        int width = Math.Abs(fixedPoint.X - e.X);
                        int height = Math.Abs(fixedPoint.Y - e.Y);
                        switch(mouseLocation)
                        {
                            case MouseLocation.InRectangle:
                                leftUpPoint.X = fixedPoint.X + e.X - mouseDownPoint.X;
                                leftUpPoint.Y = fixedPoint.Y + e.Y - mouseDownPoint.Y;
                                width = rect.Width;
                                height = rect.Height;
                                //防止矩形移除屏幕外
                                if(leftUpPoint.X < 0)
                                {
                                    leftUpPoint.X = 0;
                                }
                                if(leftUpPoint.Y < 0)
                                {
                                    leftUpPoint.Y = 0;
                                }
                                if(leftUpPoint.X + width >= Width)
                                {
                                    leftUpPoint.X = Width - width - 1;
                                }
                                if(leftUpPoint.Y + height >= Height)
                                {
                                    leftUpPoint.Y = Height - height - 1;
                                }
                                break;
                            case MouseLocation.LeftLine:
                            case MouseLocation.RightLine:
                                leftUpPoint.Y = rect.Y;
                                height = rect.Height;
                                break;
                            case MouseLocation.UpLine:
                            case MouseLocation.DownLine:
                                leftUpPoint.X = rect.X;
                                width = rect.Width;
                                break;
                        }
                        //防止边长为0的矩形区域
                        if(width == 0)
                        {
                            width++;
                        }
                        if(height == 0)
                        {
                            height++;
                        }
                        //重新绘制矩形框
                        rect = new Rectangle(leftUpPoint.X, leftUpPoint.Y, width, height);
                        Refresh();
                    }
                }
            }
        }

        //鼠标弹起时
        private void RegionSearch_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                if(rect != null && rect.X != 0 && rect.Y != 0)
                {
                    panel1.Visible = true;
                }
                isDraw = false;
                isAdjust = false;
                Refresh();
            }
        }

        //判断并保存鼠标与矩形框的位置关系，改变鼠标形状
        private void CursorLocation()
        {
            int mouseX = MousePosition.X;
            int mouseY = MousePosition.Y;
            //鼠标在矩形框外
            if(mouseX < rect.Left -2 || mouseX > rect.Right + 2 || mouseY < rect.Top - 2 || mouseY > rect.Bottom + 2)
            {
                Cursor.Current = Cursors.Default;
                this.mouseLocation = MouseLocation.OutOfRectangle;
            }
            //鼠标在矩形框内
            else if(mouseX > rect.Left + 2 && mouseX < rect.Right - 2 && mouseY > rect.Top + 2 && mouseY < rect.Bottom - 2)
            {
                Cursor.Current = Cursors.SizeAll;
                if(isAdjust == true)
                {
                    mouseLocation = MouseLocation.InRectangle;
                    fixedPoint = new Point(rect.Left, rect.Top);
                }
            }
            //鼠标在矩形框上
            else
            {
                //鼠标在矩形框的左边框以及顶点上
                if(mouseX > rect.Left - 3 && mouseX < rect.Left + 3)
                {
                    //左上角
                    if(mouseY > rect.Top - 3 && mouseY < rect.Top + 3)
                    {
                        Cursor.Current = Cursors.SizeNWSE;
                        if(isAdjust == true)
                        {
                            mouseLocation = MouseLocation.LeftUpPoint;
                            fixedPoint = new Point(rect.Right, rect.Bottom);
                        }
                    }
                    //左下角
                    else if(mouseY > rect.Bottom - 3 && mouseY < rect.Bottom + 3)
                    {
                        Cursor.Current = Cursors.SizeNESW;
                        if(isAdjust ==  true)
                        {
                            mouseLocation = MouseLocation.LeftDownPoint;
                            fixedPoint = new Point(rect.Right, rect.Top);
                        }
                    }
                    //左边框
                    else
                    {
                        Cursor.Current = Cursors.SizeWE;
                        if(isAdjust == true)
                        {
                            mouseLocation = MouseLocation.LeftLine;
                            fixedPoint = new Point(rect.Right, rect.Top);
                        }
                    }
                }
                //鼠标在矩形框的右边框以及顶点上
                else if(mouseX > rect.Right - 3 && mouseX < rect.Right + 3)
                {
                    //右上角
                    if(mouseY > rect.Top - 3 && mouseY < rect.Top + 3)
                    {
                        Cursor.Current = Cursors.SizeNESW;
                        if(isAdjust == true)
                        {
                            mouseLocation = MouseLocation.RightUpPoint;
                            fixedPoint = new Point(rect.Left, rect.Bottom);
                        }
                    }
                    //右下角
                    else if(mouseY > rect.Bottom - 3 && mouseY < rect.Bottom + 3)
                    {
                        Cursor.Current = Cursors.SizeNWSE;
                        if(isAdjust == true)
                        {
                            mouseLocation = MouseLocation.RightDownPoint;
                            fixedPoint = new Point(rect.Left, rect.Top);
                        }
                    }
                    //右边框
                    else
                    {
                        Cursor.Current = Cursors.SizeWE;
                        if(isAdjust == true)
                        {
                            mouseLocation = MouseLocation.RightLine;
                            fixedPoint = new Point(rect.Left, rect.Top);
                        }
                    }
                }
                //鼠标在矩形框的上下边框上
                else
                {
                    //鼠标在矩形的上边框上
                    if(mouseY > rect.Top - 3 && mouseY < rect.Top + 3)
                    {
                        Cursor.Current = Cursors.SizeNS;
                        if(isAdjust == true)
                        {
                            mouseLocation = MouseLocation.UpLine;
                            fixedPoint = new Point(rect.Left, rect.Bottom);
                        }
                    }
                    //鼠标在矩形的下边框上
                    else if(mouseY > rect.Bottom - 3 && mouseY < rect.Bottom + 3)
                    {
                        Cursor.Current = Cursors.SizeNS;
                        if(isAdjust == true)
                        {
                            mouseLocation = MouseLocation.DownLine;
                            fixedPoint = new Point(rect.Left, rect.Top);
                        }
                    }
                }
            }
        }

        //重载Onpaint()方法，窗体重绘时调用
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if(rect.Width != 0)
            {
                //窗体添加画板，绘制矩形
                Graphics gs = e.Graphics;
                //绘制矩形框选中部分到当前画板，覆盖在黑色遮罩上面
                gs.DrawImage(originBmp, rect, rect, GraphicsUnit.Pixel);
                //定义画笔，绘制矩形框上
                using (Pen linePen = new Pen(Color.FromArgb(255, 0, 174, 255), 1))
                {
                    gs.DrawRectangle(linePen, rect);
                }
                //定义画刷，绘制矩形框上的八个点
                using (SolidBrush  pointBrush = new SolidBrush(Color.FromArgb(255, 0, 174, 255)))
                {
                    //顶点
                    gs.FillRectangle(pointBrush, rect.X - 2, rect.Y - 2, 4, 4);
                    gs.FillRectangle(pointBrush, rect.Right - 2, rect.Y - 2, 4, 4);
                    gs.FillRectangle(pointBrush, rect.X - 2, rect.Bottom - 2, 4, 4);
                    gs.FillRectangle(pointBrush, rect.Right - 2, rect.Bottom - 2, 4, 4);
                    //中点
                    gs.FillRectangle(pointBrush, rect.X - 2, (rect.Y + rect.Bottom) / 2 - 2, 4, 4);
                    gs.FillRectangle(pointBrush, (rect.X + rect.Right) / 2 - 2, rect.Y - 2, 4, 4);
                    gs.FillRectangle(pointBrush, rect.Right - 2, (rect.Y + rect.Bottom) / 2 - 2, 4, 4);
                    gs.FillRectangle(pointBrush, (rect.X + rect.Right) / 2 - 2, rect.Bottom - 2, 4, 4);
                }
                //确定panel的位置
                //label显示坐标以及分辨率
                label1.Text = "x:" + rect.Left.ToString() + " y:" + rect.Top.ToString() + " " + rect.Width.ToString() + "*" + rect.Height.ToString();
                //矩形框下面离屏幕下方距离大于panel的高度
                if (this.Height - this.rect.Bottom > panel1.Height)
                {
                    //矩形框右边离屏幕左边距离大于panel的宽度
                    if (this.rect.Right > panel1.Width)
                    {
                        panel1.Location = new Point(this.rect.Right - panel1.Width, this.rect.Bottom + 5);
                    }
                    //矩形框右边离屏幕左边距离小于panel的宽度
                    else
                    {
                        panel1.Location = new Point(this.rect.X, this.rect.Bottom + 5);
                    }
                }
                //矩形框下面离屏幕下方距离小于panel的高度
                else
                {
                    //矩形框右边离屏幕左边距离大于panel的宽度
                    if (this.rect.Right > panel1.Width)
                    {
                        panel1.Location = new Point(this.rect.Right - panel1.Width, this.rect.Y - panel1.Height - 5);
                    }
                    //矩形框右边离屏幕左边距离小于panel的宽度
                    else
                    {
                        panel1.Location = new Point(this.rect.X, this.rect.Y - panel1.Height - 5);
                    }
                }
            }
        }
    }
}
