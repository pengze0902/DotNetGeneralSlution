using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AuxiliaryLibrary.Reflection
{
    /// <summary>
    /// 序列化操作工具类
    /// </summary>
    public class SerializeUtil
    {
        #region ToBytes(将对象序列化到字节流中)
        /// <summary>
        /// 将对象序列化到字节流中
        /// </summary>
        /// <param name="instance">对象</param>
        /// <returns></returns>
        public static byte[] ToBytes(object instance)
        {
            if (instance == null)
            {
                return null;
            }
            BinaryFormatter serializer = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, instance);
                stream.Position = 0;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
        #endregion

        #region FromBytes(将字节流反序列化为对象)
        /// <summary>
        /// 将字节流反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类名</typeparam>
        /// <param name="buffer">字节流</param>
        /// <returns></returns>
        public static T FromBytes<T>(byte[] buffer) where T : class
        {
            if (buffer == null)
            {
                return default(T);
            }
            BinaryFormatter serializer = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(buffer, 0, buffer.Length);
                stream.Position = 0;
                object result = serializer.Deserialize(stream);
                if (result == null)
                {
                    return default(T);
                }
                return (T)result;
            }
        }
        #endregion


        /// <summary>
        /// 图片转为Byte字节数组
        /// </summary>
        ///<param name="filePath">路径</param>
        /// <returns>字节数组</returns>
        private byte[] ImageToByteArray(string filePath)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (System.Drawing.Image imageIn = System.Drawing.Image.FromFile(filePath))
                {
                    using (Bitmap bmp = new Bitmap(imageIn))
                    {
                        bmp.Save(ms, imageIn.RawFormat);
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
