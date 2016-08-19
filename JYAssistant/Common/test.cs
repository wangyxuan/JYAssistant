using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JYAssistant
{
    class test
    {
        #region 键鼠编码
        public enum Key
        {
            // mouse movements  
            move = 0x0001,
            leftdown = 0x0002,
            leftup = 0x0004,
            rightdown = 0x0008,
            rightup = 0x0010,
            middledown = 0x0020,
            //keyboard stuff  
            VK_LBUTTON = 1,
            VK_RBUTTON = 2,
            VK_CANCEL = 3,
            VK_MBUTTON = 4,
            VK_BACK = 8,
            VK_TAB = 9,
            VK_CLEAR = 12,
            VK_RETURN = 13,
            VK_SHIFT = 16,
            VK_CONTROL = 17,
            VK_MENU = 18,
            VK_PAUSE = 19,
            VK_CAPITAL = 20,
            VK_ESCAPE = 27,
            VK_SPACE = 32,
            VK_PRIOR = 33,
            VK_NEXT = 34,
            VK_END = 35,
            VK_HOME = 36,
            VK_LEFT = 37,
            VK_UP = 38,
            VK_RIGHT = 39,
            VK_DOWN = 40,
            VK_SELECT = 41,
            VK_PRINT = 42,
            VK_EXECUTE = 43,
            VK_SNAPSHOT = 44,
            VK_INSERT = 45,
            VK_DELETE = 46,
            VK_HELP = 47,
            VK_NUM0 = 48, //0  
            VK_NUM1 = 49, //1  
            VK_NUM2 = 50, //2  
            VK_NUM3 = 51, //3  
            VK_NUM4 = 52, //4  
            VK_NUM5 = 53, //5  
            VK_NUM6 = 54, //6  
            VK_NUM7 = 55, //7  
            VK_NUM8 = 56, //8  
            VK_NUM9 = 57, //9  
            VK_A = 65, //A  
            VK_B = 66, //B  
            VK_C = 67, //C  
            VK_D = 68, //D  
            VK_E = 69, //E  
            VK_F = 70, //F  
            VK_G = 71, //G  
            VK_H = 72, //H  
            VK_I = 73, //I  
            VK_J = 74, //J  
            VK_K = 75, //K  
            VK_L = 76, //L  
            VK_M = 77, //M  
            VK_N = 78, //N  
            VK_O = 79, //O  
            VK_P = 80, //P  
            VK_Q = 81, //Q  
            VK_R = 82, //R  
            VK_S = 83, //S  
            VK_T = 84, //T  
            VK_U = 85, //U  
            VK_V = 86, //V  
            VK_W = 87, //W  
            VK_X = 88, //X  
            VK_Y = 89, //Y  
            VK_Z = 90, //Z  
            VK_NUMPAD0 = 96, //0  
            VK_NUMPAD1 = 97, //1  
            VK_NUMPAD2 = 98, //2  
            VK_NUMPAD3 = 99, //3  
            VK_NUMPAD4 = 100, //4  
            VK_NUMPAD5 = 101, //5  
            VK_NUMPAD6 = 102, //6  
            VK_NUMPAD7 = 103, //7  
            VK_NUMPAD8 = 104, //8  
            VK_NUMPAD9 = 105, //9  
            VK_NULTIPLY = 106,
            VK_ADD = 107,
            VK_SEPARATOR = 108,
            VK_SUBTRACT = 109,
            VK_DECIMAL = 110,
            VK_DIVIDE = 111,
            VK_F1 = 112,
            VK_F2 = 113,
            VK_F3 = 114,
            VK_F4 = 115,
            VK_F5 = 116,
            VK_F6 = 117,
            VK_F7 = 118,
            VK_F8 = 119,
            VK_F9 = 120,
            VK_F10 = 121,
            VK_F11 = 122,
            VK_F12 = 123,
            VK_NUMLOCK = 144,
            VK_SCROLL = 145,
            middleup = 0x0040,
            xdown = 0x0080,
            xup = 0x0100,
            wheel = 0x0800,
            virtualdesk = 0x4000,
            absolute = 0x8000
        }
        #endregion
        static Ols ols = null;

        [DllImport("user32.dll")]
        public static extern int MapVirtualKey(uint Ucode, uint uMapType);
        //0：代表ucode是一虚拟键码且被翻译为一扫描码。若一虚拟键码不区分左右，则返回左键的扫描码。若未进行翻译，则函数返回o。
        //1：代表ucode是一扫描码且被翻译为一虚拟键码，且此虚拟键码不区分左右。若未进行翻译，则函数返回0。
        //2：代表ucode为一虚拟键码且被翻译为一未被移位的字符值存放于返回值的低序字中。死键（发音符号）则通过设置返回值的最高位来表示。若未进行翻译，则函数返回0。
        //3：代表ucode为一扫描码且被翻译为区分左右键的一虚拟键码。若未进行翻译，则函数返回0。
        //返回值：返回值可以是一扫描码，或一虚拟键码，或一字符值，这完全依赖于不同的ucode和umaptype的值。若未进行翻译，则函数返回0。

        public static Boolean init()
        {
            ols = new Ols();
            return ols.GetStatus() == (uint)Ols.Status.NO_ERROR;
        }

        private static void KBCWait4IBE()//等待键盘缓冲区为空
        {
            byte dwVal = 0;
            do
            {
                ols.ReadIoPortByteEx(0x64, ref dwVal);
            }
            while ((dwVal & 0x2) > 0);
        }
        public static void KeyDown(Char ch)
        {
            int btScancode = MapVirtualKey((uint)(Key)ch, 0);
            KBCWait4IBE();
            ols.WriteIoPortByte(0x64, 0xd2);
            KBCWait4IBE();
            ols.WriteIoPortByte(0x60, (byte)btScancode);
        }

        public static void KeyUp(Char ch)
        {
            int btScancode = MapVirtualKey((uint)(Key)ch, 0);
            KBCWait4IBE();
            ols.WriteIoPortByte(0x64, 0xd2);
            KBCWait4IBE();
            ols.WriteIoPortByte(0x60, (byte)(btScancode | 0x80));
        }

        ///// key down  
        //public static void MykeyDown(VKKey vKeyCoad)
        //{  
        //    if (!IsInitialize) return;  

        //    int btScancode = 0;  
        //    btScancode = MapVirtualKey((uint)vKeyCoad, 0);  
        //    KBCWait4IBE();  
        //    SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);  
        //    KBCWait4IBE();  
        //    SetPortVal(KBC_KEY_DATA, (IntPtr)0x60, 1);  
        //    KBCWait4IBE();  
        //    SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);  
        //    KBCWait4IBE();  
        //    SetPortVal(KBC_KEY_DATA, (IntPtr)btScancode, 1);  
        //}  
        ///// Key up  
        //public static void MykeyUp(VKKey vKeyCoad)
        //{  
        //    if (!IsInitialize) return;  

        //    int btScancode = 0;  
        //    btScancode = MapVirtualKey((uint)vKeyCoad, 0);  
        //    KBCWait4IBE();  
        //    SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);  
        //    KBCWait4IBE();  
        //    SetPortVal(KBC_KEY_DATA, (IntPtr)0x60, 1);  
        //    KBCWait4IBE();  
        //    SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);  
        //    KBCWait4IBE();  
        //    SetPortVal(KBC_KEY_DATA, (IntPtr)(btScancode | 0x80), 1);  
        //}  

        ///// Simulate mouse down  
        //public static void MyMouseDown(int vKeyCoad)
        //{  
        //    int btScancode = 0;  
        //    btScancode = MapVirtualKey((byte)vKeyCoad, 0);  
        //    KBCWait4IBE(); // 'wait for buffer gets empty  
        //    SetPortVal(KBC_KEY_CMD, (IntPtr)0xD3, 1);// 'send write command  
        //    KBCWait4IBE();  
        //    SetPortVal(KBC_KEY_DATA, (IntPtr)(btScancode | 0x80), 1);// 'write in io  
        //}  
        ///// Simulate mouse up  
        //public static void MyMouseUp(int vKeyCoad)
        //{  
        //    int btScancode = 0;  
        //    btScancode = MapVirtualKey((byte)vKeyCoad, 0);  
        //    KBCWait4IBE(); // 'wait for buffer gets empty  
        //    SetPortVal(KBC_KEY_CMD, (IntPtr)0xD3, 1); //'send write command  
        //    KBCWait4IBE();  
        //    SetPortVal(KBC_KEY_DATA, (IntPtr)(btScancode | 0x80), 1);// 'write in io  
        //}  


        public void test1()
        {
            String pwd = "123456";
            foreach (char chr in pwd)
            {
                test.init();
                test.KeyDown(chr);
                Thread.Sleep(100);
                test.KeyUp(chr);
            }
            Thread.CurrentThread.Join(100);
        }
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    int hwnd = FindWindow(null, "PAL4-Application");
        //    IntPtr p = new IntPtr(hwnd);
        //    if (p == IntPtr.Zero)
        //        return;
        //    SetActiveWindow(p);
        //    //设置为活动窗体，防止被其他窗口挡住  
        //    SetForegroundWindow(p);
        //    Thread.Sleep(1000);
        //    int i = 0;
        //    while (i < 5)
        //    {
        //        WinIo.MykeyDown(VKKey.VK_SPACE);
        //        Thread.Sleep(100);
        //        WinIo.MykeyUp(VKKey.VK_SPACE);
        //        Thread.Sleep(100);
        //        WinIo.MykeyDown(VKKey.VK_S);
        //        Thread.Sleep(100);
        //        WinIo.MykeyUp(VKKey.VK_S);
        //        Thread.Sleep(500);
        //        WinIo.MykeyDown(VKKey.VK_D);
        //        Thread.Sleep(100);
        //        WinIo.MykeyUp(VKKey.VK_D);
        //        Thread.Sleep(500);
        //        WinIo.MykeyDown(VKKey.VK_A);
        //        Thread.Sleep(100);
        //        WinIo.MykeyUp(VKKey.VK_A);
        //        i++;
        //    }
        //}


    }
}
