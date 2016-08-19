using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Threading;

using System.IO;

namespace JYAssistant
{
    public partial class Form1 : Form
    {
        int matchKeyCount;//匹配或者按键次数次数
        string strInfo;//按键信息
        List<string> listVk;//键盘码
        List<int> listCD;//按键cd
        bool bPointHander;//坐标及句柄
        bool bKickBoss;//撸boss
        bool bDiexuandu;//叠玄毒
        bool bSingleControl;//单键快速操作
        bool bGoupExercise;//团练
        bool bDIY;//自定义按钮
        bool bSearchPiano;//搜索弹琴四个按钮
        bool bPiano;//弹琴
        bool bStartPinao;//快捷键控制开始弹琴
        DispatcherTimer myTimer = new DispatcherTimer();
        List<string> tqImageKey;//读取的弹琴字母按键
        List<string> nowCaptureHash;//弹琴实时截图hash
        List<bool> listBool;//弹琴控制器
        string startPianoHash;//开始弹琴前的实时截图hash
        List<Point> listPara;//弹琴坐标点参数
        List<Point> listNowPoint;//弹琴实时截图坐标点
        bool isRegion;//是否进行区域选择搜索
        //Rectangle rect;//区域选择获取的rectangle

        public Form1()
        {
            InitializeComponent();
            textBox1.ReadOnly = true;
            listVk = new List<string>();
            listCD = new List<int>();
            myTimer.Tick += new EventHandler(MyTimer_Tick);
            myTimer.Stop();
            tqImageKey = new List<string>();
            nowCaptureHash = new List<string>();
            listPara = new List<Point>();
            listNowPoint = new List<Point>();
            isRegion = false;
            //rect = new Rectangle();

            ExitTimer();

            this.CenterToScreen();
        }

        #region 句柄操作
        //点击按钮是否获取坐标及句柄
        private void button6_Click(object sender, EventArgs e)
        {
            if (bPointHander == false)
            {
                ExitTimer();
                bPointHander = true;

                timer1.Interval = 100;
                timer1.Enabled = true;
            }
            else
            {
                ExitTimer();
            }
        }

        //从textbox获取句柄
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != null)
            {
                HandleOperation.Instance.SetGameHandle(textBox2.Text);
            }
            else
            {
                MessageBox.Show("请输入句柄！");
            }
        }

        //窗口失去焦点时，在窗口输出句柄，并把句柄赋值
        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (bPointHander == true)
            {
                textBox2.Text = HandleOperation.Instance.GetCursorHandle().ToString();
                HandleOperation.Instance.SetGameHandle(textBox2.Text);
                bPointHander = false;
                timer1.Enabled = false;
            }
        }

        public void ExitTimer()
        {
            bPointHander = false;
            bKickBoss = false;
            bDiexuandu = false;
            bSingleControl = false;
            bGoupExercise = false;
            bDIY = false;
            bSearchPiano = false;
            bPiano = false;
            bStartPinao = false;
            matchKeyCount = 1;
            myTimer.Stop();
            timer1.Enabled = false;
        }
        #endregion

        //打boss
        private void test_Click(object sender, EventArgs e)
        {
            if (bKickBoss == false)
            {
                try
                {
                    HandleOperation.Instance.SetWindowActive();
                }
                catch
                {
                    MessageBox.Show("输入句柄或启动应用！");
                    return;
                }
                matchKeyCount = 1;
                ExitTimer();

                bKickBoss = true;
                timer1.Interval = 1000;
                timer1.Enabled = true;

                listVk.Clear();
                listCD.Clear();
                XMLToCode.Instance.GetShortcutsToCode(ref listVk, ref listCD, "shortcuts/kickboss");

                if (!(KeyboardMouse.init()))
                {
                    MessageBox.Show("初始化失败！");
                    return;
                }
            }
            else
            {
                ExitTimer();
            }
        }

        //叠玄毒
        private void button1_Click(object sender, EventArgs e)
        {
            if (bDiexuandu == false)
            {
                try
                {
                    HandleOperation.Instance.SetWindowActive();
                }
                catch
                {
                    MessageBox.Show("输入句柄或启动应用！");
                    return;
                }
                ExitTimer();

                bDiexuandu = true;
                timer1.Interval = 8000;
                timer1.Enabled = true;

                listVk.Clear();
                listCD.Clear();
                //XMLToCode.Instance.GetShortcutsToCode(ref listVk);
                XMLToCode.Instance.GetShortcutsToCode(ref listVk, ref listCD, "shortcuts/xuandu");
                if (!(KeyboardMouse.init()))
                {
                    MessageBox.Show("初始化失败！");
                    return;
                }
            }
            else
            {
                ExitTimer();
            }
        }

        //单键循环
        private void button4_Click(object sender, EventArgs e)
        {
            if (bSingleControl == false)
            {
                try
                {
                    HandleOperation.Instance.SetWindowActive();
                }
                catch
                {
                    MessageBox.Show("输入句柄或启动应用！");
                    return;
                }

                ExitTimer();

                bSingleControl = true;
                timer1.Interval = 100;
                timer1.Enabled = true;

                listVk.Clear();
                listCD.Clear();
                XMLToCode.Instance.GetShortcutsToCode(ref listVk, ref listCD, "shortcuts/single");
                if (!(KeyboardMouse.init()))
                {
                    MessageBox.Show("初始化失败！");
                    return;
                }
            }
            else
            {
                ExitTimer();
            }
        }

        //自定义
        private void button5_Click(object sender, EventArgs e)
        {
            if (bDIY == false)
            {
                try
                {
                    HandleOperation.Instance.SetWindowActive();
                }
                catch
                {
                    MessageBox.Show("输入句柄或启动应用！");
                    return;
                }
                ExitTimer();

                bDIY = true;
                timer1.Interval = 1000;
                timer1.Enabled = true;

                listVk.Clear();
                listCD.Clear();
                XMLToCode.Instance.GetShortcutsToCode(ref listVk, ref listCD, "shortcuts/diy");

                if (!(KeyboardMouse.init()))
                {
                    MessageBox.Show("初始化失败！");
                    return;
                }
            }
            else
            {
                ExitTimer();
            }
        }


        #region 团练授业
        //团练授业开始
        private void button3_Click(object sender, EventArgs e)
        {
            if (bGoupExercise == false)
            {
                try
                {
                    HandleOperation.Instance.SetWindowActive();
                }
                catch
                {
                    MessageBox.Show("输入句柄或启动应用！");
                    return;
                }
                ExitTimer();

                textBox3.Text = "修炼开始！";

                if (!(KeyboardMouse.init()))
                {
                    MessageBox.Show("初始化失败！");
                    return;
                }

                bGoupExercise = true;
                timer1.Interval = 500;
                timer1.Enabled = true;
            }
            else
            {
                ExitTimer();
                textBox3.Text = "修炼结束！";
                strInfo = "";
            }
        }

        public void GroupExercise(ref string str)
        {
            //左上角579 644 右下角1023 682
            //偶数：中间右800 奇数：中间780
            #region 截图坐标默认或者从xml读取
            //左上角y坐标644; size 38; istart 600;intervalPixel 20;
            int imgWidth = 38;//按键宽度
            int imgHeight = 38;//按键高度
            int searchIntervalPixel = 20;//带边框按键宽度
            int imgStart = 0;//第一个截图点
            int imgY = 644;

            List<int> listPara = new List<int>();
            bool onOff = false;
            XMLToCode.Instance.GetGroupexerPara(ref listPara, ref onOff, "shortcuts/groupexercise");

            if (onOff == true)
            {
                imgY = listPara[0];
            }
            #endregion

            List<string> imageKey = new List<string>();
            imageKey.Clear();
            string vKKey = "q";
            Size sz = new Size(imgWidth, imgHeight);
            Point p1 = new Point(imgStart, imgY);
            Point p2 = new Point(0, 0);
            ImageProcessing imagePro = new ImageProcessing();

            #region 从中间搜索
            //偶数个按键搜索
            imgStart = Screen.PrimaryScreen.Bounds.Width / 2;
            p1.X = imgStart;
            imagePro.CaptureImage(p1, p2, sz);
            if (imagePro.CompareHash(imagePro.GetHash(), ref vKKey, "shortcuts/imagetokey/tlkeyhash"))
            {
                //往后搜索
                while (true)
                {
                    imagePro.CaptureImage(p1, p2, sz);
                    if (imagePro.CompareHash(imagePro.GetHash(), ref vKKey, "shortcuts/imagetokey/tlkeyhash"))
                    {
                        imageKey.Add(vKKey);
                        p1.X += (searchIntervalPixel * 2);
                    }
                    else
                    {
                        break;
                    }
                }
                //往前搜索
                p1.X = imgStart - searchIntervalPixel * 2;
                while (true)
                {
                    imagePro.CaptureImage(p1, p2, sz);
                    if (imagePro.CompareHash(imagePro.GetHash(), ref vKKey, "shortcuts/imagetokey/tlkeyhash"))
                    {
                        imageKey.Insert(0, vKKey);
                        p1.X -= (searchIntervalPixel * 2);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                //奇数个按键搜索
                imgStart = Screen.PrimaryScreen.Bounds.Width / 2 - searchIntervalPixel;
                p1.X = imgStart;
                imagePro.CaptureImage(p1, p2, sz);
                if (imagePro.CompareHash(imagePro.GetHash(), ref vKKey, "shortcuts/imagetokey/tlkeyhash"))
                {
                    //往后搜索
                    while (true)
                    {
                        imagePro.CaptureImage(p1, p2, sz);
                        if (imagePro.CompareHash(imagePro.GetHash(), ref vKKey, "shortcuts/imagetokey/tlkeyhash"))
                        {
                            imageKey.Add(vKKey);
                            p1.X += (searchIntervalPixel * 2);
                        }
                        else
                        {
                            break;
                        }
                    }
                    //往前搜索
                    p1.X = imgStart - searchIntervalPixel * 2;
                    while (true)
                    {
                        imagePro.CaptureImage(p1, p2, sz);
                        if (imagePro.CompareHash(imagePro.GetHash(), ref vKKey, "shortcuts/imagetokey/tlkeyhash"))
                        {
                            imageKey.Insert(0, vKKey);
                            p1.X -= (searchIntervalPixel * 2);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            //按键操作
            if (imageKey.Count != 0)
            {
                listVk.Clear();
                XMLToCode.Instance.GetKeyToCode(imageKey, ref listVk);

                if (listVk.Count != 0)
                {
                    foreach (var vk in listVk)
                    {
                        KeyBaseOperation(vk, 10);
                    }
                }

                str = "上次按键为";
                foreach (var ik in imageKey)
                {
                    str += ik.ToString() + " ";
                }
            }
            #endregion

            #region 从头搜索
            ////寻找第一个截图点
            //while (true)
            //{
            //    p1.X = imgStart;
            //    imagePro.CaptureImage(p1, p2, sz);
            //    if (imagePro.CompareHash(imagePro.GetHash(), ref vKKey))
            //    {
            //        break;
            //    }
            //    imgStart += searchIntervalPixel;
            //    if (imgStart > (Screen.PrimaryScreen.Bounds.Width / 2))
            //    {
            //        break;
            //    }
            //}

            //if (imgStart < (Screen.PrimaryScreen.Bounds.Width / 2))
            //{
            //    textBox3.Text = "按键次数：" + matchKeyCount + ",第一截图点坐标为" + imgStart.ToString() + ",";
            //    //开始获取按键
            //    while (true)
            //    {
            //        p1.X = imgStart;
            //        imagePro.CaptureImage(p1, p2, sz);
            //        if (imagePro.CompareHash(imagePro.GetHash(), ref vKKey))
            //        {
            //            imageKey.Add(vKKey);
            //            imgStart += (searchIntervalPixel*2);
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }

            //    listVk.Clear();
            //    XMLToCode.Instance.GetKeyToCode(imageKey, ref listVk);

            //    if (listVk.Count != 0)
            //    {
            //        foreach (var vk in listVk)
            //        {
            //            KeyBaseOperation(vk, 100);
            //        }
            //    }

            //    foreach (var ik in imageKey)
            //    {
            //        textBox3.Text += ik.ToString();
            //    }
            //    matchKeyCount++;
            //}
            #endregion
        }

        #region 修炼坐标确定
        private void button10_Click(object sender, EventArgs e)
        {
            #region ////按键图标基本数值设置
            //string vk = "q";
            //Size sz = new Size(38, 38);
            //int imgYStart = Screen.PrimaryScreen.Bounds.Height - 38;
            //int imgX1 = Screen.PrimaryScreen.Bounds.Width / 2 - 20;
            //int imgX2 = Screen.PrimaryScreen.Bounds.Width / 2 + 20;
            //Point ptemp = new Point(0, 0);
            //bool bWriteXML = false;
            //Rectangle rect = new Rectangle();
            #endregion

            #region 选定搜索区域
            if (isRegion == false)
            {
                Hide();
                Thread.Sleep(500);
                RegionSearch rs = new RegionSearch();
                //注册RegionSearch传递参数的事件委托
                rs.IsRegionEvent += new IsRegion(GetIsRegion);
                rs.Show();
                //注册获取选择区域范围的事件委托
                rs.ORPEvent += new OperationRectPara(GettlORPEvent);
                rs.SetMFSEvent += new SetMainFromShow(SetMainFormShow);
            }
            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //代码运行顺序是运行完click整个函数再运行rs.show()的内容，所以不能以
            //顺序的方式写完代码，使用事件委托来进行回调
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

            #region 暴力搜索不可取 缩小搜索区域进行搜索
            //for (int i = imgYStart; i > Screen.PrimaryScreen.Bounds.Height / 2; i--)
            //{
            //    Point p1 = new Point(imgX1, i);
            //    Point p2 = new Point(imgX2, i);
            //    ImageProcessing imgPro = new ImageProcessing();
            //    imgPro.CaptureImage(p1, ptemp, sz);
            //    if (imgPro.CompareHash(imgPro.GetHash(), ref vk, "shortcuts/imagetokey/tlkeyhash") == true)
            //    {
            //        imgYStart = i;
            //        bWriteXML = true;
            //        break;
            //    }
            //    imgPro.CaptureImage(p2, ptemp, sz);
            //    if (imgPro.CompareHash(imgPro.GetHash(), ref vk, "shortcuts/imagetokey/tlkeyhash") == true)
            //    {
            //        imgYStart = i;
            //        bWriteXML = true;
            //        break;
            //    }
            //}

            ////imgYStart写入xml文件
            //if (bWriteXML == true)
            //{
            //    List<int> listPara = new List<int>();
            //    listPara.Add(imgYStart);
            //    XMLToCode.Instance.SetGroupexePara(listPara);
            //    MessageBox.Show("团练坐标确认成功！");
            //}
            //if ((bWriteXML == false) && (imgYStart == (Screen.PrimaryScreen.Bounds.Height - 38)))
            //{
            //    MessageBox.Show("坐标确认失败！");
            //}
            #endregion
        }

        public void GettlORPEvent(Rectangle rect)
        {
            //记录截取图片的hash码与xml文件中相似图片hash码的差
            int count = 100;
            Size sz = new Size(38, 38);
            int imgX = Screen.PrimaryScreen.Bounds.Width / 2 - 20;
            int searchIntervalPixel = 20;//带边框按键宽度
            Point ptemp = new Point(0, 0);
            bool bWriteXML = false;
            int imgYStart = rect.Top;
            Point p = new Point();
            bool bBreak = false;

            //向后搜索
            for (int i = imgX; i < imgX + 20 + 160; i += searchIntervalPixel)
            {
                p.X = i;
                for (int j = imgYStart; j < rect.Bottom; j++)
                {
                    p.Y = j;
                    ImageProcessing imgPro = new ImageProcessing();
                    imgPro.CaptureImage(p, ptemp, sz);
                    if (imgPro.CompareHash(imgPro.GetHash(), ref count, "shortcuts/imagetokey/tlkeyhash") == true)
                    {
                        if (searchIntervalPixel == 20)
                        {
                            searchIntervalPixel = 40;
                        }
                        if (count == 0)
                        {
                            imgYStart = j;
                            bWriteXML = true;
                            bBreak = true;
                            break;
                        }
                    }
                }
                if (bBreak == true)
                {
                    break;
                }
            }
            //当bBreak为true时，不需要进行向前搜索
            if (bBreak == false)
            {
                searchIntervalPixel = 20;
                count = 100;
                //向后搜索
                for (int i = imgX; i > imgX - 20 - 160; i -= searchIntervalPixel)
                {
                    p.X = i;
                    for (int j = imgYStart; j < rect.Bottom; j++)
                    {
                        p.Y = j;
                        ImageProcessing imgPro = new ImageProcessing();
                        imgPro.CaptureImage(p, ptemp, sz);
                        if (imgPro.CompareHash(imgPro.GetHash(), ref count, "shortcuts/imagetokey/tlkeyhash") == true)
                        {
                            if (searchIntervalPixel == 20)
                            {
                                searchIntervalPixel = 40;
                            }
                            if (count == 0)
                            {
                                imgYStart = j;
                                bWriteXML = true;
                                bBreak = true;
                                break;
                            }
                        }
                    }
                    if (bBreak == true)
                    {
                        break;
                    }
                }
            }

            //imgYStart写入xml文件
            if (bWriteXML == true)
            {
                List<int> listPara = new List<int>();
                listPara.Add(imgYStart);
                XMLToCode.Instance.SetGroupexePara(listPara);
                MessageBox.Show("团练坐标确认成功！");
            }
            if (bWriteXML == false)
            {
                MessageBox.Show("坐标确认失败！");
            }
        }

        //处理区域选择状态委托，设置区域选择状态
        public void GetIsRegion(bool isRegion)
        {
            this.isRegion = isRegion;
        }
        //处理主窗体显示事件委托
        public void SetMainFormShow()
        {
            Show();
        }
        #endregion

        //团练截图
        private void button7_Click(object sender, EventArgs e)
        {
            #region 截图坐标默认或者从xml读取
            //左上角y坐标644; size 38; istart 600;intervalPixel 20;
            int imgWidth = 38;//按键宽度
            int imgHeight = 38;//按键高度
            int searchIntervalPixel = 20;//带边框按键宽度
            int imgStart = 0;//第一个截图点
            int imgY = 644;

            List<int> listPara = new List<int>();
            bool onOff = false;
            XMLToCode.Instance.GetGroupexerPara(ref listPara, ref onOff, "shortcuts/groupexercise");

            if (onOff == true)
            {
                imgY = listPara[0];
            }
            #endregion


            Size sz = new Size(imgWidth, imgHeight);
            Point p1 = new Point(imgStart, imgY);
            Point p2 = new Point(0, 0);
            ImageProcessing imagePro = new ImageProcessing();

            Image oImage;
            int temp = 0;
            string oImagePath = System.Windows.Forms.Application.StartupPath;

            if (!Directory.Exists(oImagePath + @"\tlkeyimage"))
            {
                Directory.CreateDirectory(oImagePath + @"\tlkeyimage");
            }

            //往后搜索截图
            imgStart = Screen.PrimaryScreen.Bounds.Width / 2 - searchIntervalPixel;
            for (int i = imgStart; temp < 10; i += searchIntervalPixel)
            {
                p1.X = i;
                imagePro.CaptureImage(p1, p2, sz, out oImage);
                oImage.Save(oImagePath + @"\tlkeyimage\" + temp.ToString() + ".jpg");
                temp++;
            }

            //往前搜索截图
            imgStart = imgStart - searchIntervalPixel;
            for (int i = imgStart; temp < 20; i -= searchIntervalPixel)
            {
                p1.X = i;
                imagePro.CaptureImage(p1, p2, sz, out oImage);
                oImage.Save(oImagePath + @"\tlkeyimage\" + temp.ToString() + ".jpg");
                temp++;
            }
            MessageBox.Show("截图完成!");
        }
        #endregion

        #region 弹琴
        private void button9_Click(object sender, EventArgs e)
        {
            if ((bStartPinao == false) && (bSearchPiano == false) && (bPiano == false))
            {
                try
                {
                    HandleOperation.Instance.SetWindowActive();
                }
                catch
                {
                    MessageBox.Show("输入句柄或启动应用！");
                    return;
                }
                ExitTimer();
                bStartPinao = true;

                #region 获取弹琴参数
                listPara.Clear();
                listNowPoint.Clear();
                bool onOff = false;
                XMLToCode.Instance.GetTqPara(ref listPara, ref onOff, "shortcuts/tqpara");

                if (onOff == true)
                {
                    listNowPoint.Add(new Point(listPara[0].X - 10 + 33 + 33, listPara[0].Y - 10));
                    listNowPoint.Add(new Point(listPara[1].X - 10 + 33 + 33, listPara[1].Y - 10));
                    listNowPoint.Add(new Point(listPara[2].X - 10 - 33 - 1, listPara[2].Y - 10));
                    listNowPoint.Add(new Point(listPara[3].X - 10 - 33 - 1, listPara[3].Y - 10));
                }
                else
                {
                    listPara.Add(new Point(1210, 739));
                    listPara.Add(new Point(1210, 779));
                    listPara.Add(new Point(1459, 662));
                    listPara.Add(new Point(1459, 699));

                    listNowPoint.Add(new Point(1266, 729));
                    listNowPoint.Add(new Point(1266, 769));
                    listNowPoint.Add(new Point(1415, 652));
                    listNowPoint.Add(new Point(1415, 689));
                }
                #endregion

                #region 获取弹琴“前”A点的实时截图hash
                Point ptemp = new Point();
                Size sz = new Size(13, 13);
                ImageProcessing imgPro = new ImageProcessing();
                imgPro.CaptureImage(listPara[0], ptemp, sz);
                startPianoHash = imgPro.GetHash();
                #endregion

                myTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                myTimer.Start();
            }
            else
            {
                myTimer.Stop();
                ExitTimer();
                textBox3.Text += Environment.NewLine;
                textBox3.Text += "弹琴结束";
            }
        }

        //控制快捷键开始弹琴
        public void StartPiano()
        {
            #region 按下快捷键
            listVk.Clear();
            listCD.Clear();
            XMLToCode.Instance.GetShortcutsToCode(ref listVk, ref listCD, "shortcuts/tqkey");
            if (!(KeyboardMouse.init()))
            {
                MessageBox.Show("初始化失败！");
                return;
            }

            for (int i = 0; i < listVk.Count; i++)
            {
                KeyBaseOperation(listVk[i], listCD[i]);
            }
            #endregion

            Thread.Sleep(500);

            #region A点截图对比，相同继续按快捷键，不同则开始搜索弹琴
            Point ptemp = new Point();
            Size sz = new Size(13, 13);
            ImageProcessing imgPro = new ImageProcessing();
            imgPro.CaptureImage(listPara[0], ptemp, sz);
            if (!imgPro.CompareHash(imgPro.GetHash(), startPianoHash))
            {
                ExitTimer();
                bSearchPiano = true;
                myTimer.Stop();
                myTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                myTimer.Start();
            }
            #endregion
        }

        //搜索四个基本按键
        public void SearchPiano()
        {
            //AZIJ顺序
            Point ptemp = new Point(0, 0);
            Size sz = new Size(13, 13);
            ImageProcessing imagePro = new ImageProcessing();
            string vk = "q";
            tqImageKey.Clear();

            imagePro.CaptureImage(listPara[0], ptemp, sz);
            if (imagePro.CompareHash(imagePro.GetHash(), ref vk, "shortcuts/imagetokey/tqkeyhash"))
            {
                foreach (Point p in listPara)
                {
                    imagePro.CaptureImage(p, ptemp, sz);
                    imagePro.CompareHash(imagePro.GetHash(), ref vk, "shortcuts/imagetokey/tqkeyhash");
                    tqImageKey.Add(vk);
                }
                if (tqImageKey.Count == 4)
                {
                    myTimer.Stop();

                    if (!(KeyboardMouse.init()))
                    {
                        MessageBox.Show("初始化失败！");
                        return;
                    }

                    ExitTimer();
                    bPiano = true;
                    textBox3.Text += Environment.NewLine;
                    textBox3.Text += "开始弹琴";
                    //控制器初始化
                    listBool = new List<bool>();
                    for (int i = 0; i < tqImageKey.Count; i++)
                    {
                        listBool.Add(false);
                        textBox3.Text += Environment.NewLine;
                        textBox3.Text += "控制器" + i.ToString();
                    }

                    //四个按键实时截图
                    #region 坐标
                    //字母坐标
                    //listPoint.Add(new Point(1210, 739));
                    //listPoint.Add(new Point(1210, 779));
                    //listPoint.Add(new Point(1459, 662));
                    //listPoint.Add(new Point(1459, 699));
                    //按键点坐标
                    //listPoint.Add(new Point(1200, 729));
                    //                       (1233
                    //listPoint.Add(new Point(1200, 769));
                    //                       (1233
                    //listPoint.Add(new Point(1449, 652));
                    //listPoint.Add(new Point(1449, 689));
                    //实时截图点坐标
                    //listPoint.Add(new Point(1266, 729));
                    //listPoint.Add(new Point(1266, 769));
                    //listPoint.Add(new Point(1415, 652));
                    //listPoint.Add(new Point(1415, 689));
                    #endregion
                    Size szNow = new Size(1, 33);
                    foreach (Point p in listNowPoint)
                    {
                        imagePro.CaptureImage(p, ptemp, szNow);
                        nowCaptureHash.Add(imagePro.GetHash());
                    }

                    myTimer.Interval = new TimeSpan(0, 0, 0, 0, 30);
                    myTimer.Start();
                }
            }
        }

        public void PlayPiano()
        {
            //弹琴点：
            //Ax1242 Ay738
            //Zx1242 Zy778
            //Ix1421 1446 25  Iy655 680 25
            //  1427      13    661     13
            //Jx1427 Jy698
            //List<Point> listPoint = new List<Point>();
            Point ptemp = new Point(0, 0);

            Size szNow = new Size(1, 33);
            ImageProcessing imagePro = new ImageProcessing();
            List<string> imageKey = new List<string>();

            imageKey.Clear();
            listVk.Clear();

            #region 搜索和按键点不同的图片hash
            //获取需要按键的字母字符串
            #region 当实时截图一开始变化时就按下按钮（失败）
            //for (int i = 0; i < tqImageKey.Count; i++)
            //{
            //    imagePro.CaptureImage(listPoint[i], ptemp, szNow);
            //    if (!imagePro.CompareHash(imagePro.GetHash(), nowCaptureHash[i]))
            //    {
            //        textBox3.Text += Environment.NewLine;
            //        textBox3.Text += i.ToString() + "截图不同";
            //        //实时截图与初始实时截图第一次不同
            //        if (listBool[i] == true)
            //        {
            //            imageKey.Add(tqImageKey[i]);
            //            textBox3.Text += "，按下按键" + tqImageKey[i];
            //            listBool[i] = false;
            //        }
            //        else
            //        {
            //            textBox3.Text += "，按键重复，等待重置！";
            //        }
            //    }
            //    if (imagePro.CompareHash(imagePro.GetHash(), nowCaptureHash[i]))
            //    {
            //        textBox3.Text += Environment.NewLine;
            //        textBox3.Text += i.ToString() + "截图相同，置为true";
            //        listBool[i] = true;
            //    }
            //}
            #endregion

            #region 当实时截图恢复初始截图时按下按钮
            for (int i = 0; i < tqImageKey.Count; i++)
            {
                imagePro.CaptureImage(listNowPoint[i], ptemp, szNow);
                if (!imagePro.CompareHash(imagePro.GetHash(), nowCaptureHash[i]))
                {
                    listBool[i] = true;
                }
                if (imagePro.CompareHash(imagePro.GetHash(), nowCaptureHash[i]))
                {
                    if (listBool[i] == true)
                    {
                        imageKey.Add(tqImageKey[i]);
                        listBool[i] = false;
                    }
                }
            }
            #endregion

            if (imageKey.Count != 0)
            {
                XMLToCode.Instance.GetKeyToCode(imageKey, ref listVk);

                //控制键盘按下按键
                if (listVk.Count != 0)
                {
                    textBox3.Text += Environment.NewLine;
                    foreach (var vk in listVk)
                    {
                        KeyBaseOperation(vk, 10);
                    }
                    foreach (var ik in imageKey)
                    {
                        textBox3.Text += ik + " ";
                    }
                    imageKey.Clear();
                    listVk.Clear();
                }
            }
            #endregion
            #region 搜索和按键点相同的图片，匹配失败
            //listPoint.Add(new Point(1242, 738));//按键点坐标
            //listPoint.Add(new Point(1242, 778));
            //listPoint.Add(new Point(1427, 661));
            //listPoint.Add(new Point(1427, 698));
            ////获取弹琴的按键字符串
            //for (int i = 0; i < tqImageKey.Count; i++)
            //{
            //    imagePro.CaptureImage(listPoint[i], ptemp, sz);
            //    if (imagePro.CompareHash(imagePro.GetHash(), ref vk, "shortcuts/imagetokey/hithash"))
            //    {
            //        imageKey.Add(tqImageKey[i]);
            //    }
            //}

            ////弹琴按键字符串转化为键盘码
            //XMLToCode.Instance.GetKeyToCode(imageKey, ref listVk);

            ////控制键盘按下按键
            //if (listVk.Count != 0)
            //{
            //    foreach (var lvk in listVk)
            //    {
            //        KeyBaseOperation(lvk, 100);
            //    }
            //}
            //matchKeyCount++;
            #endregion
        }

        //上弹琴坐标确定
        //1210 739 779
        private void button11_Click(object sender, EventArgs e)
        {
            if (isRegion == false)
            {
                Hide();
                Thread.Sleep(500);
                RegionSearch rs = new RegionSearch();
                //注册RegionSearch传递参数的事件委托
                rs.IsRegionEvent += new IsRegion(GetIsRegion);
                rs.Show();
                //注册获取选择区域范围的事件委托
                rs.ORPEvent += new OperationRectPara(GettqUpORPEvent);
                rs.SetMFSEvent += new SetMainFromShow(SetMainFormShow);
            }
        }

        //下弹琴坐标确认
        //1459 662 699
        private void button12_Click(object sender, EventArgs e)
        {
            if (isRegion == false)
            {
                Hide();
                Thread.Sleep(500);
                RegionSearch rs = new RegionSearch();
                //注册RegionSearch传递参数的事件委托
                rs.IsRegionEvent += new IsRegion(GetIsRegion);
                rs.Show();
                //注册获取选择区域范围的事件委托
                rs.ORPEvent += new OperationRectPara(GettqDownORPEvent);
                rs.SetMFSEvent += new SetMainFromShow(SetMainFormShow);
            }
        }

        //上弹琴按键
        public void GettqUpORPEvent(Rectangle rect)
        {
            //按键图标基本数值设置
            int count = 0;
            //List<Point> az = new List<Point>();
            List<Point> ij = new List<Point>();
            Point ptemp = new Point(0, 0);
            Size sz = new Size(13, 13);
            string vk = "q";
            string bvk = "q";

            //搜索ij级按键坐标参数ij纵坐标相差37
            for (int i = rect.Left; i < rect.Right - 13; i++)
            {
                for (int j = rect.Top; j < rect.Bottom - 13; j++)
                {
                    ImageProcessing imgPro = new ImageProcessing();
                    Point p = new Point(i, j);
                    imgPro.CaptureImage(p, ptemp, sz);
                    if (imgPro.CompareHash(imgPro.GetHash(), ref vk, "shortcuts/imagetokey/tqkeyhash") == true)
                    {
                        if (imgPro.CompareHash(imgPro.GetHash(), ref count, "shortcuts/imagetokey/tqkeyhash") == true)
                        {
                            if (count == 0)
                            {
                                switch (vk)
                                {
                                    case "i":
                                    case "o":
                                    case "p":
                                        bvk = vk;
                                        if (ij.Count == 1)
                                        {
                                            ij.Insert(0, p);
                                        }
                                        else
                                        {
                                            ij.Add(p);
                                        }
                                        break;
                                    case "j":
                                    case "k":
                                    case "l":
                                        bvk = vk;
                                        ij.Add(p);
                                        break;
                                }
                            }
                        }
                    }
                    if (ij.Count == 2)
                    {
                        break;
                    }
                }
                if (ij.Count == 1)
                {
                    switch (bvk)
                    {
                        case "i":
                        case "o":
                        case "p":
                            Point ptemp1 = new Point(ij[0].X, ij[0].Y + 37);
                            ij.Add(ptemp1);
                            break;
                        case "j":
                        case "k":
                        case "l":
                            Point ptemp2 = new Point(ij[0].X, ij[0].Y - 37);
                            ij.Insert(0, ptemp2);
                            break;
                    }
                    break;
                }
                if (ij.Count == 2)
                {
                    break;
                }
            }

            if (ij.Count == 2)
            {
                XMLToCode.Instance.SettqPara(ij, true);
                MessageBox.Show("弹琴坐标确认成功！");
            }
            else
            {
                MessageBox.Show("坐标确认失败！");
            }
        }

        public void GettqDownORPEvent(Rectangle rect)
        {
            //按键图标基本数值设置
            int count = 0;
            List<Point> az = new List<Point>();
            //List<Point> ij = new List<Point>();
            Point ptemp = new Point(0, 0);
            Size sz = new Size(13, 13);
            string vk = "q";
            string bvk = "q";
            //搜索az级坐标参数，az纵坐标相差40
            for (int i = rect.Left; i < rect.Right - 13; i++)
            {
                for (int j = rect.Top; j < rect.Bottom - 13; j++)
                {
                    ImageProcessing imgPro = new ImageProcessing();
                    Point p = new Point(i, j);
                    imgPro.CaptureImage(p, ptemp, sz);
                    if (imgPro.CompareHash(imgPro.GetHash(), ref vk, "shortcuts/imagetokey/tqkeyhash") == true)
                    {
                        if (imgPro.CompareHash(imgPro.GetHash(), ref count, "shortcuts/imagetokey/tqkeyhash") == true)
                        {
                            if (count == 0)
                            {
                                switch (vk)
                                {
                                    case "a":
                                    case "s":
                                    case "d":
                                        bvk = vk;
                                        if (az.Count == 1)
                                        {
                                            az.Insert(0, p);
                                        }
                                        else
                                        {
                                            az.Add(p);
                                        }
                                        break;
                                    case "z":
                                    case "x":
                                    case "c":
                                        bvk = vk;
                                        az.Add(p);
                                        break;
                                }
                            }
                        }
                    }
                    if (az.Count == 2)
                    {
                        break;
                    }
                }
                //搜索完坐标所在列，只搜索到一个按键时
                if (az.Count == 1)
                {
                    switch (bvk)
                    {
                        case "a":
                        case "s":
                        case "d":
                            Point ptemp1 = new Point(az[0].X, az[0].Y + 40);
                            az.Add(ptemp1);
                            break;
                        case "z":
                        case "x":
                        case "c":
                            Point ptemp2 = new Point(az[0].X, az[0].Y - 40);
                            az.Insert(0, ptemp2);
                            break;
                    }
                    break;
                }
                if (az.Count == 2)
                {
                    break;
                }
            }

            //参数写入xml文件
            if (az.Count == 2)
            {
                XMLToCode.Instance.SettqPara(az, false);
                MessageBox.Show("弹琴坐标确认成功！");
            }
            else
            {
                MessageBox.Show("坐标确认失败！");
            }
        }

        //弹琴截图
        private void button8_Click(object sender, EventArgs e)
        {
            //弹琴截图点坐标
            //listPoint.Add(new Point(1210, 739));
            //listPoint.Add(new Point(1210, 779));
            //listPoint.Add(new Point(1459, 662));
            //listPoint.Add(new Point(1459, 699));
            Point ptemp = new Point(0, 0);
            List<Point> listPoint = new List<Point>();
            listPoint.Add(new Point(1210, 739));
            listPoint.Add(new Point(1210, 779));
            listPoint.Add(new Point(1459, 662));
            listPoint.Add(new Point(1459, 699));
            Size sz = new Size(13, 13);
            ImageProcessing imagePro = new ImageProcessing();
            Image oImage;
            string oImagePath = Application.StartupPath;

            if (!Directory.Exists(oImagePath + @"\tqkeyimage"))
            {
                Directory.CreateDirectory(oImagePath + @"\tqkeyimage");
            }

            int temp = 0;
            foreach (Point p in listPoint)
            {
                imagePro.CaptureImage(p, ptemp, sz, out oImage);
                oImage.Save(oImagePath + @"\tqkeyimage\" + temp.ToString() + ".jpg");
                temp++;
            }
            MessageBox.Show("截图完成！");
        }
        #endregion

        private void MyTimer_Tick(object sender, EventArgs e)
        {
            if (bStartPinao == true)
            {
                StartPiano();
            }
            if (bSearchPiano == true)
            {
                SearchPiano();
            }
            if (bPiano == true)
            {
                textBox3.Text += Environment.NewLine;
                textBox3.Text += "正在弹琴！";
                ImageProcessing imgPro = new ImageProcessing();
                Point ptemp = new Point();
                Size sz = new Size(13, 13);
                imgPro.CaptureImage(listPara[0], ptemp, sz);
                //A点初始截图不同，继续弹琴，相同则重新开始按快捷键
                if (!imgPro.CompareHash(imgPro.GetHash(), startPianoHash))
                {
                    PlayPiano();
                }
                else
                {
                    myTimer.Stop();
                    ExitTimer();
                    bStartPinao = true;
                    myTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                    StartPiano();
                    myTimer.Start();
                }
            }
        }

        //Timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bPointHander == true)
            {
                textBox1.Text = "句柄：" + HandleOperation.Instance.GetCursorHandle().ToString() + "；坐标X=" + HandleOperation.Instance.GetCursorPoint().X.ToString() + ",Y=" + HandleOperation.Instance.GetCursorPoint().Y.ToString();
            }
            if (bKickBoss == true)
            {
                KeyBaseOperation(listVk[0], listCD[0]);//玄毒
                if (HandleOperation.Instance.GetCursorHandle() != HandleOperation.Instance.GetGameHandle())
                {
                    ExitTimer();
                    return;
                }
                KeyBaseOperation(listVk[1], listCD[1]);//瘟神
                if (HandleOperation.Instance.GetCursorHandle() != HandleOperation.Instance.GetGameHandle())
                {
                    ExitTimer();
                    return;
                }
                for (int i = 0; i < 6; i++)
                {
                    KeyBaseOperation(listVk[2], listCD[2]);//阴魂
                    if (HandleOperation.Instance.GetCursorHandle() != HandleOperation.Instance.GetGameHandle())
                    {
                        ExitTimer();
                        return;
                    }
                    KeyBaseOperation(listVk[3], listCD[3]);//修罗  
                    if (HandleOperation.Instance.GetCursorHandle() != HandleOperation.Instance.GetGameHandle())
                    {
                        ExitTimer();
                        return;
                    }
                }
                matchKeyCount += 1;
            }
            if (bDiexuandu == true)
            {
                textBox3.Text = "玄毒摧木:" + matchKeyCount.ToString();
                KeyBaseOperation(listVk[0], listCD[0]);
                if (HandleOperation.Instance.GetCursorHandle() != HandleOperation.Instance.GetGameHandle())
                {
                    ExitTimer();
                    return;
                }
                matchKeyCount += 1;
            }
            if (bSingleControl == true)
            {
                KeyBaseOperation(listVk[0], listCD[0]);
                if (HandleOperation.Instance.GetCursorHandle() != HandleOperation.Instance.GetGameHandle())
                {
                    ExitTimer();
                    return;
                }
            }
            if (bGoupExercise == true)
            {
                textBox3.Text = "第" + matchKeyCount + "次扫描" + System.Environment.NewLine;
                GroupExercise(ref strInfo);
                textBox3.Text += strInfo;
                matchKeyCount++;
            }
            if (bDIY == true)
            {
                for (int i = 0; i < listVk.Count; i++)
                {
                    KeyBaseOperation(listVk[i], listCD[i]);
                    if (HandleOperation.Instance.GetCursorHandle() != HandleOperation.Instance.GetGameHandle())
                    {
                        ExitTimer();
                        return;
                    }
                }
            }
            //if (bSearchPiano == true)
            //{
            //    //开始搜索确认四个基本按键
            //    SearchPiano();
            //}
        }

        //键盘基本操作
        public void KeyBaseOperation(string vk, int time)
        {
            //按下的键位
            KeyboardMouse.KeyDown(vk);
            Thread.Sleep(10);
            KeyboardMouse.KeyUp(vk);
            //cd以及延迟
            Thread.Sleep(time);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox3.SelectionStart = textBox3.Text.Length;
            textBox3.ScrollToCaret();
        }
    }
}
