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

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern int Everything_SetSearch(string lpSearchString);

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_SetMatchPath(bool bEnable);

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_SetMatchCase(bool bEnable);

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_SetMatchWholeWord(bool bEnable);

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_SetRegex(bool bEnable);

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_SetMax(int dwMax);

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_SetOffset(int dwOffset);

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_GetMatchPath();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_GetMatchCase();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_GetMatchWholeWord();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_GetRegex();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern uint Everything_GetMax();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern uint Everything_GetOffset();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern string Everything_GetSearch();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern StateCode Everything_GetLastError();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_Query();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_SortResultsByPath();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern int Everything_GetNumFileResults();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern int Everything_GetNumFolderResults();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern int Everything_GetNumResults();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern int Everything_GetTotFileResults();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern int Everything_GetTotFolderResults();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern int Everything_GetTotResults();

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_IsVolumeResult(int nIndex);

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_IsFolderResult(int nIndex);

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern bool Everything_IsFileResult(int nIndex);

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_GetResultFullPathName(int nIndex, StringBuilder lpString, int nMaxCount);

        [DllImport(EVERYTHING_DLL_NAME)]
        public static extern void Everything_Reset();

    }
}
