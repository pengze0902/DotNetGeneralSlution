using System.Runtime.InteropServices;
using System.Text;

namespace AuxiliaryLibrary.EverythingNet
{
    /// <summary>
    /// API类
    /// </summary>
    public class EverythingApi
    {
        const string EVERYTHING_DLL_NAME = "Everything.dll";

        /// <summary>
        /// 设置搜索
        /// </summary>
        /// <param name="lpSearchString"></param>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern int Everything_SetSearch(string lpSearchString);

        /// <summary>
        /// 设置匹配路径
        /// </summary>
        /// <param name="bEnable"></param>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_SetMatchPath(bool bEnable);

        /// <summary>
        /// 设置匹配大小写
        /// </summary>
        /// <param name="bEnable"></param>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_SetMatchCase(bool bEnable);

        /// <summary>
        /// 设置匹配整个字
        /// </summary>
        /// <param name="bEnable"></param>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_SetMatchWholeWord(bool bEnable);

        /// <summary>
        /// 设定正则表达式
        /// </summary>
        /// <param name="bEnable"></param>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_SetRegex(bool bEnable);

        /// <summary>
        /// 设置最大
        /// </summary>
        /// <param name="dwMax"></param>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_SetMax(int dwMax);

        /// <summary>
        /// 设置偏移
        /// </summary>
        /// <param name="dwOffset"></param>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_SetOffset(int dwOffset);

        /// <summary>
        /// 获得匹配路径
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_GetMatchPath();

        /// <summary>
        /// 获取匹配案例
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_GetMatchCase();

        /// <summary>
        /// 获得整个字
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_GetMatchWholeWord();

        /// <summary>
        /// 获取正则表达式
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_GetRegex();

        /// <summary>
        /// 获取最大
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern uint Everything_GetMax();

        /// <summary>
        /// 获得偏移
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern uint Everything_GetOffset();

        /// <summary>
        /// 获取搜索
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern string Everything_GetSearch();

        /// <summary>
        /// 获取最后一个错误
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern StateCode Everything_GetLastError();

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_Query();

        /// <summary>
        /// 按路径排序结果
        /// </summary>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_SortResultsByPath();

        /// <summary>
        /// 获取数字文件结果
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern int Everything_GetNumFileResults();

        /// <summary>
        /// 获取数字文件夹结果
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern int Everything_GetNumFolderResults();

        /// <summary>
        /// 获取数字结果
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern int Everything_GetNumResults();

        /// <summary>
        /// 获取Tot文件结果
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern int Everything_GetTotFileResults();

        /// <summary>
        /// 获取文件夹结果
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern int Everything_GetTotFolderResults();

        /// <summary>
        /// 获取Tot结果
        /// </summary>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern int Everything_GetTotResults();

        /// <summary>
        /// 文件大小
        /// </summary>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_IsVolumeResult(int nIndex);

        /// <summary>
        /// 文件夹大小
        /// </summary>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_IsFolderResult(int nIndex);

        /// <summary>
        /// 文件结果
        /// </summary>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_IsFileResult(int nIndex);

        /// <summary>
        /// 获取结果全路径名称
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="lpString"></param>
        /// <param name="nMaxCount"></param>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_GetResultFullPathName(int nIndex, StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// 重启
        /// </summary>
        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_Reset();

    }
}
