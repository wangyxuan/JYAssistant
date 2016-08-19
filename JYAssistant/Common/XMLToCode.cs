using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Drawing;

namespace JYAssistant
{
    class XMLToCode
    {
        private static readonly XMLToCode instance = new XMLToCode();
        XmlDocument doc = new XmlDocument();

        public static XMLToCode Instance
        {
            get
            {
                return instance;
            }
        }

        private XMLToCode()
        {
            
        }

        //xml快捷键组转化为Code
        public void GetShortcutsToCode(ref List<string> listCode, ref List<int> listCD, string nodePath)
        {
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\shortcuts.xml");
            //从xml获取自定义的快捷键
            List<string> listKey = new List<string>();
            XmlNode xn = doc.SelectSingleNode(nodePath);
            XmlNodeList xnlShortcuts = xn.ChildNodes;
            foreach(XmlNode xntemp in xnlShortcuts)
            {
                listKey.Add(xntemp.FirstChild.InnerText);
                listCD.Add(int.Parse(xntemp.FirstChild.NextSibling.InnerText));
            }

            //键位字符串转换为键盘码
            GetKeyToCode(listKey, ref listCode);
        }

        ////kickboss快捷键转化为Code
        //public void GetShortcutsToCode(ref List<string> listCode)
        //{
        //    doc.Load(System.Windows.Forms.Application.StartupPath + "\\shortcuts.xml");
        //    XmlNodeList xnlKey = doc.SelectNodes("shortcuts/kickboss/keyboard/key");
        //    //获取出自定义的快捷键
        //    List<string> listKey = new List<string>();
        //    foreach(XmlNode node in xnlKey)
        //    {
        //        listKey.Add(node.InnerText);
        //    }

        //    //键位字符串转化成Code
        //    GetKeyToCode(listKey, ref listCode);
        //}

        //键位字符串转换为Code
        public void GetKeyToCode(List<string> listKey, ref List<string> listCode)
        {
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\shortcuts.xml");
            XmlNodeList xnlCode = doc.SelectNodes("shortcuts/keytocode/convert/key");
            XmlNode xn = null;
            foreach(string str in listKey)
            {
                foreach(XmlNode node in xnlCode)
                {
                    if(node.InnerText == str)
                    {
                        xn = node.ParentNode;
                        listCode.Add(xn.LastChild.InnerText);
                        break;
                    }
                }
            }
        }

        //读取图片指纹
        public void GetImageToKey(ref Dictionary<string, string> keyHash, string path)
        {
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\shortcuts.xml");
            XmlNodeList xnlCode = doc.SelectNodes(path);
            foreach(XmlNode xn in xnlCode)
            {
                keyHash.Add(xn.FirstChild.InnerText, xn.LastChild.InnerText);
            }
        }

        //获取修炼截图参数
        public void GetGroupexerPara(ref List<int> listPara, ref bool onOff, string path)
        {
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\shortcuts.xml");
            XmlNode xnCode = doc.SelectSingleNode(path);
            if (xnCode.Attributes["key"].Value == "on")
            {
                onOff = true;
                XmlNodeList xnl = xnCode.ChildNodes;
                foreach (XmlNode xn in xnl)
                {
                    listPara.Add(int.Parse(xn.InnerText.ToString()));
                }
            }
            else
            {
                onOff = false;
            }
        }

        //获取弹琴截图参数
        public void GetTqPara(ref List<Point> listPoint, ref bool onOff, string path)
        {
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\shortcuts.xml");
            XmlNode xnCode = doc.SelectSingleNode(path);
            if(xnCode.Attributes["key"].Value == "on")
            {
                onOff = true;
                XmlNodeList xnl = xnCode.ChildNodes;
                foreach(XmlNode xn in xnl)
                {
                    listPoint.Add(new Point(int.Parse(xn.FirstChild.InnerText.ToString()), int.Parse(xn.LastChild.InnerText.ToString())));
                }
            }
        }

        //修炼写XML文件
        public void SetGroupexePara(List<int> listPara)
        {
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\shortcuts.xml");
            XmlNode xnCode = doc.SelectSingleNode("shortcuts/groupexercise");
            XmlNodeList xnl = xnCode.ChildNodes;
            for(int i = 0; i <　xnl.Count; i++)
            {
                xnl[i].InnerText = listPara[i].ToString();
            }
            doc.Save(System.Windows.Forms.Application.StartupPath + "\\shortcuts.xml");
        }

        //弹琴写XML文件
        public void SettqPara(List<Point> listPoint, bool b)
        {
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\shortcuts.xml");
            XmlNode xnCode = doc.SelectSingleNode("shortcuts/tqpara");
            XmlNodeList xnl = xnCode.ChildNodes;
            if (b == true)
            {//写入ij级别的按钮坐标
                for (int i = 0; i < 2; i++)
                {
                    xnl[i + 2].FirstChild.InnerText = listPoint[i].X.ToString();
                    xnl[i + 2].LastChild.InnerText = listPoint[i].Y.ToString();
                }
                doc.Save(System.Windows.Forms.Application.StartupPath + "\\shortcuts.xml");
            }
            else if( b == false)
            {//写入az级别的按钮坐标
                for (int i = 0; i < 2; i++)
                {
                    xnl[i].FirstChild.InnerText = listPoint[i].X.ToString();
                    xnl[i].LastChild.InnerText = listPoint[i].Y.ToString();
                }
                doc.Save(System.Windows.Forms.Application.StartupPath + "\\shortcuts.xml");

            }
        }
    }
}
