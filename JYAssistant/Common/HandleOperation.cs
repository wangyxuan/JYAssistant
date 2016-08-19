using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace JYAssistant
{
    class HandleOperation
    {
        private static readonly HandleOperation instance = new HandleOperation();

        public static HandleOperation Instance
        {
            get
            {
                return instance;
            }
        }

        private HandleOperation()
        {

        }

        private static IntPtr GHWND;//窗口句柄

        #region DllImport
        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point point);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        private static extern void SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hwnd, ref RECT lprect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {//左上角，右下角
            public int rLeft;
            public int rTop;
            public int rRight;
            public int rBottom;
        }


        #endregion

        //设置待激活窗口句柄
        public void SetGameHandle(string str)
        {
            int i = int.Parse(str);
            GHWND = new IntPtr(i);
        }
        //获得待激活窗口句柄
        public IntPtr GetGameHandle()
        {
            if (GHWND == IntPtr.Zero)
            {
                GHWND = Process.GetProcessesByName("fxgame")[0].MainWindowHandle;
                return GHWND;
            }
            else
            {
                return GHWND;
            }
        }

        //获得鼠标所在位置坐标
        public Point GetCursorPoint()
        {
            Point p;
            GetCursorPos(out p);
            return p;
        }

        //设置鼠标坐标
        public void SetCursorPoint(Point p)
        {
            SetCursorPos(p.X, p.Y);
        }

        //设置为活动窗口
        public void SetWindowActive()
        {
            IntPtr p = GetGameHandle();
            if (p == IntPtr.Zero)
                return;
            SetActiveWindow(p);
            SetForegroundWindow(p);
        }

        //获取鼠标所在处窗口的句柄
        public IntPtr GetCursorHandle()
        {
            return WindowFromPoint(GetCursorPoint());
        }

        //获取窗口的大小
        public RECT GetWindowRect()
        {
            RECT rc = new RECT();
            GetWindowRect(GetGameHandle(), ref rc);
            //int width = rc.rRight - rc.rLeft; //窗口的宽度
            //int height = rc.rBottom - rc.rTop; //窗口的高度
            return rc;
        }

        //窗口客户区大小
        public RECT GetClientRect()
        {
            RECT rc = new RECT();
            GetClientRect(GetGameHandle(), ref rc);
            return rc;
        }
    }
}
