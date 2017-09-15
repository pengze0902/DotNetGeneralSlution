using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace AuxiliaryLibrary.Barcode
{
    /// <summary>
    /// 条码生成工具类：目前只实现了EAN-13
    /// </summary>
    public class BarcodeHelper
    {
        /// <summary>
        /// 造个表
        /// </summary>
        private DataTable MakeTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Type");
            dt.Columns.Add("A");
            dt.Columns.Add("B");
            dt.Columns.Add("C");
            dt.Rows.Add("0", "AAAAAA", "0001101", "0100111", "1110010");
            dt.Rows.Add("1", "AABABB", "0011001", "0110011", "1100110");
            dt.Rows.Add("2", "AABBAB", "0010011", "0011011", "1101100");
            dt.Rows.Add("3", "AABBBA", "0111101", "0100001", "1000010");
            dt.Rows.Add("4", "ABAABB", "0100011", "0011101", "1011100");
            dt.Rows.Add("5", "ABBAAB", "0110001", "0111001", "1001110");
            dt.Rows.Add("6", "ABBBAA", "0101111", "0000101", "1010000");
            dt.Rows.Add("7", "ABABAB", "0111011", "0010001", "1000100");
            dt.Rows.Add("8", "ABABBA", "0110111", "0001001", "1001000");
            dt.Rows.Add("9", "ABBABA", "0001011", "0010111", "1110100");

            return dt;
        }
        private uint _mHeight = 40;
        /// <summary>
        /// 绘制高
        /// </summary>
        public virtual uint Heigth { get { return _mHeight; } set { _mHeight = value; } }
        private byte _mFontSize;
        /// <summary>
        /// 字体大小（宋体）
        /// </summary>
        public virtual byte FontSize { get { return _mFontSize; } set { _mFontSize = value; } }

        private byte _mMagnify;
        /// <summary>     
        /// 放大系数    
        /// </summary>
        public virtual byte Magnify { get { return _mMagnify; } set { _mMagnify = value; } }
        /// <summary>
        /// 根据传入的条码获取二进制码
        /// </summary>
        /// <param name="pText">13位条码</param>
        /// <returns></returns>
        public virtual byte[] GetCodeImageBytes(string pText)
        {
            return BitmapToBytes(GetCodeImage(pText));
        }
        /// <summary>
        /// 根据传入的条码获取位图
        /// </summary>
        /// <param name="pText">13位条码</param>
        /// <returns></returns>
        public virtual Bitmap GetCodeImage(string pText)
        {
            using (DataTable dt = MakeTable())
            {
                if (pText.Length != 13) throw new InvalidOperationException("digits is not 13");
                string codeText = pText.Remove(0, 1);
                string codeIndex = "101";
                char[] leftType = GetValue(dt, pText.Substring(0, 1), "Type").ToCharArray();
                for (int i = 0; i != 6; i++)
                {
                    codeIndex += GetValue(dt, codeText.Substring(0, 1), leftType[i].ToString(CultureInfo.InvariantCulture));
                    codeText = codeText.Remove(0, 1);
                }

                codeIndex += "01010";

                for (int i = 0; i != 6; i++)
                {
                    codeIndex += GetValue(dt, codeText.Substring(0, 1), "C");
                    codeText = codeText.Remove(0, 1);
                }
                codeIndex += "101";
                return GetImage(codeIndex, pText);
            }
        }
        /// <summary>
        /// 根据传入的位图获取二进制码
        /// </summary>
        /// <param name="bitmap">位图</param>
        /// <returns></returns>
        public virtual byte[] BitmapToBytes(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
                byte[] byteImage = ms.ToArray();
                return byteImage;
            }
        }
        /// <summary>
        /// 获取目标对应的数据
        /// </summary>
        /// <param name="dt">构造表</param>
        /// <param name="pValue">类型</param>
        /// <param name="pType"></param>
        /// <returns>编码</returns>
        private string GetValue(DataTable dt, string pValue, string pType)
        {
            if (dt == null) return "";
            DataRow[] row = dt.Select("ID='" + pValue + "'");
            if (row.Length != 1) throw new InvalidOperationException("Invalid code: " + pValue);
            return row[0][pType].ToString();
        }

        /// <summary>
        /// 绘制编码图形
        /// </summary>
        /// <param name="pText">编码</param>
        /// <param name="pViewText"></param>
        /// <returns>图形</returns>
        private Bitmap GetImage(string pText, string pViewText)
        {
            char[] value = pText.ToCharArray();
            int fontWidth = 0;
            Font myFont = null;
            if (_mFontSize != 0)
            {
                #region 获取符合大小的字体
                myFont = new Font(SystemFonts.DialogFont.SystemFontName, _mFontSize);
                using (Bitmap myFontBmp = new Bitmap(_mFontSize, _mFontSize))
                {
                    using (Graphics fontGraphics = Graphics.FromImage(myFontBmp))
                    {
                        for (byte i = _mFontSize; i != 0; i--)
                        {
                            SizeF drawSize = fontGraphics.MeasureString(pViewText.Substring(0, 1), myFont);
                            if (drawSize.Height > _mFontSize)
                            {
                                myFont.Dispose();
                                myFont = new Font(SystemFonts.DialogFont.SystemFontName, i);
                            }
                            else
                            {
                                fontWidth = (int)drawSize.Width;
                                break;
                            }
                        }
                    }
                }
                #endregion
            }
            if (ScanDrawText(myFont, pText, fontWidth) == false)
            {
                fontWidth = 0;
                _mFontSize = 0;
            }
            //宽 == 需要绘制的数量*放大倍数 + 两个字的宽   
            Bitmap codeImage = new Bitmap(value.Length * (_mMagnify + 1) + (fontWidth * 2), (int)_mHeight);
            using (Graphics garphics = Graphics.FromImage(codeImage))
            {
                garphics.FillRectangle(Brushes.White, new Rectangle(0, 0, codeImage.Width, codeImage.Height));

                int lenEx = fontWidth;
                for (int i = 0; i != value.Length; i++)
                {
                    int drawWidth = _mMagnify + 1;
                    int height;
                    if (i == 0 || i == 2 || i == 46 || i == 48 || i == 92 || i == 94)
                    {
                        height = (int)_mHeight;
                    }
                    else
                    {
                        height = (int)_mHeight - _mFontSize;
                    }
                    garphics.FillRectangle(value[i] == '1' ? Brushes.Black : Brushes.White,
                        new Rectangle(lenEx, 0, drawWidth, height));
                    lenEx += drawWidth;
                }
                //绘制文字
                if (fontWidth != 0 && _mFontSize != 0)
                {
                    if (myFont != null)
                    {
                        int starY = (int)_mHeight - myFont.Height;
                        garphics.DrawString(pViewText.Substring(0, 1), myFont, Brushes.Black, 0, starY);
                        int starX = fontWidth + (3 * (_mMagnify + 1));
                        garphics.DrawString(pViewText.Substring(1, 6), myFont, Brushes.Black, starX, starY);
                        starX = fontWidth + (50 * (_mMagnify + 1));
                        garphics.DrawString(pViewText.Substring(7, 6), myFont, Brushes.Black, starX, starY);
                    }
                }
            }
            return codeImage;
        }
        /// <summary>
        /// 判断字体是否大与绘制图形
        /// </summary>
        /// <param name="myFont">字体</param>
        /// <param name="pText">文字</param>
        /// <param name="pWidth">字体的宽</param>
        /// <returns>true可以绘制 False不可以绘制</returns>
        private bool ScanDrawText(Font myFont, string pText, int pWidth)
        {
            if (myFont == null) return false;
            int width = (pText.Length - 6 - 5) * (_mMagnify + 1);
            if ((pWidth * 12) > width) return false;
            return true;
        }
    }
}
