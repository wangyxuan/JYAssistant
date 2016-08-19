using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Drawing;
//using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace JYAssistant
{
    class ImageProcessing
    {     
        Image sourceImg;

        #region 图片指纹对比
        //sourceImg初始化
        public void SourceImg(string filePath)
        {
            sourceImg = Image.FromFile(filePath);
        }

        //public SimilarPhoto(Stream stream)
        //{
        //    SourceImg = Image.FromStream(stream);
        //}

        public String GetHash()
        {
            Image image = ReduceSize(8, 8, sourceImg);
            Byte[] grayValues = ReduceColor(image);
            Byte average = CalcAverage(grayValues);
            String reslut = ComputeBits(grayValues, average);
            return reslut;
        }

        // Step 1 : Reduce size to 8*8
        //第一步 缩小图片尺寸,将图片缩小到8x8的尺寸, 总共64个像素.这一步的作用是去除各种图片尺寸和图片比例的差异, 只保留结构、明暗等基本信息.
        private Image ReduceSize(int width, int height, Image SourceImg)
        {
            Image image = SourceImg.GetThumbnailImage(width, height, () => { return false; }, IntPtr.Zero);
            return image;
        }

        // Step 2 : Reduce Color
        //第二步 转为灰度图片,将缩小后的图片, 转为64级灰度图片.
        private Byte[] ReduceColor(Image image)
        {
            Bitmap bitMap = new Bitmap(image);
            Byte[] grayValues = new Byte[image.Width * image.Height];
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color color = bitMap.GetPixel(x, y);
                    byte grayValue = (byte)((color.R * 30 + color.G * 59 + color.B * 11) / 100);
                    grayValues[x * image.Width + y] = grayValue;
                }
            }
            return grayValues;
        }

        // Step 3 : Average the colors
        //第三步 计算灰度平均值,计算图片中所有像素的灰度平均值
        private Byte CalcAverage(byte[] values)
        {
            int sum = 0;
            for (int i = 0; i < values.Length; i++)
            {
                sum += (int)values[i];
            }
            return Convert.ToByte(sum / values.Length);
        }

        // Step 4 : Compute the bits
        //第四步 比较像素的灰度,将每个像素的灰度与平均值进行比较, 如果大于或等于平均值记为1, 小于平均值记为0.
        private String ComputeBits(byte[] values, byte averageValue)
        {
            char[] result = new char[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] < averageValue)
                {
                    result[i] = '0';
                }
                else
                {
                    result[i] = '1';
                }
            }
            return new String(result);
        }

        // Compare hash
        //第六步 对比图片指纹,得到图片的指纹后, 就可以对比不同的图片的指纹, 计算出64位中有多少位是不一样的.如果不相同的数据位数不超过5, 就说明两张图片很相似, 如果大于10, 说明它们是两张不同的图片.
        public static Int32 CalcSimilarDegree(string a, string b)
        {
            if (a.Length != b.Length)
            {
                throw new ArgumentException();
            }
            int count = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    count++;
                }
            }
            return count;
        }
        #endregion

        #region 屏幕截图及关键区域截取
        public void CaptureImage(Point p1, Point p2, Size sz)
        {
            //创建图象，保存截取的图象
            Bitmap image = new Bitmap(sz.Width, sz.Height);
            Graphics imgGraphics = Graphics.FromImage(image);
            //设置截屏区域
            //源图片左上角坐标，目标图片左上角坐标，截图区域大小
            imgGraphics.CopyFromScreen(p1, p2, sz);
            //保存
            sourceImg = image;
        }
        
        public void CaptureImage(Point p1, Point p2, Size sz, out Image oImage)
        {
            //创建图象，保存截取的图象
            Bitmap image = new Bitmap(sz.Width, sz.Height);
            Graphics imgGraphics = Graphics.FromImage(image);
            //设置截屏区域
            //源图片左上角坐标，目标图片左上角坐标，截图区域大小
            imgGraphics.CopyFromScreen(p1, p2, sz);
            //保存
            sourceImg = image;
            oImage = image;
        }
        #endregion

        //截图hash码对比
        public bool CompareHash(string CaptureImageHash ,ref string vk, string path)
        {
            #region 按键hash码
            Dictionary<string, string> vKKeyHash = new Dictionary<string, string>();
            XMLToCode.Instance.GetImageToKey(ref vKKeyHash, path);
            #endregion
            foreach(var di in vKKeyHash)
            {
                if(CalcSimilarDegree(CaptureImageHash, di.Value) <= 3)
                {
                    vk = di.Key;
                    return true;
                }
            }
            return false;
        }

        public bool CompareHash(string CaptureImageHash, ref int count, string path)
        {
            #region 按键hash码
            Dictionary<string, string> vKKeyHash = new Dictionary<string, string>();
            XMLToCode.Instance.GetImageToKey(ref vKKeyHash, path);
            #endregion
            foreach (var di in vKKeyHash)
            {
                if (CalcSimilarDegree(CaptureImageHash, di.Value) <= 3)
                {
                    count = CalcSimilarDegree(CaptureImageHash, di.Value);
                    return true;
                }
            }
            return false;
        }

        public bool CompareHash(string a, string b)
        {
            if(CalcSimilarDegree(a, b) <= 3)
            {
                return true;
            }
            return false;
        }
    }
}
