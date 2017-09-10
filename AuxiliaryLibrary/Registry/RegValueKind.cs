namespace AuxiliaryLibrary.Registry
{
    /// <summary>
    /// 指定在注册表中存储值时所用的数据类型，或标识注册表中某个值的数据类型  
    /// </summary>
    public enum RegValueKind
    {
        //指示一个不受支持的注册表数据类型。例如，不支持Microsoft Win32 API  注册表数据类型REG_RESOURCE_LIST。使用此值指定  
        Unknown = 0,

        //指定一个以Null  结尾的字符串。此值与Win32 API  注册表数据类型REG_SZ  等效。  
        String = 1,

        //指定一个以NULL  结尾的字符串，该字符串中包含对环境变量（如%PATH%，当值被检索时，就会展开）的未展开的引用。  
        ExpandString = 2,

        //此值与Win32 API 注册表数据类型REG_EXPAND_SZ  等效。  
        Binary = 3,

        //指定任意格式的二进制数据。此值与Win32 API  注册表数据类型REG_BINARY  等效。  
        DWord = 4,

        //指定一个32  位二进制数。此值与Win32 API  注册表数据类型REG_DWORD  等效。  
        //指定一个以NULL  结尾的字符串数组，以两个空字符结束。此值与Win32 API  注册表数据类型REG_MULTI_SZ  等效。  
        MultiString = 5,

        //指定一个64  位二进制数。此值与Win32 API  注册表数据类型REG_QWORD  等效。  
        QWord = 6,
    }
}
