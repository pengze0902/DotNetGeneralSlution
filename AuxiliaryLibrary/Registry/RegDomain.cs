namespace AuxiliaryLibrary.Registry
{
    /// <summary>
    /// 注册表基项静态域 
    /// </summary>
    public enum RegDomain
    {
        //对应于 HKEY_CLASSES_ROOT 主键  
        ClassesRoot = 0,

        //对应于 HKEY_CURRENT_USER 主键  
        CurrentUser = 1,

        //对应于HKEY_LOCAL_MACHINE 主键  
        LocalMachine = 2,

        //对应于HKEY_USER 主键  
        User = 3,

        //对应于 HEKY_CURRENT_CONFIG 主键  
        CurrentConfig = 4,

        //对应于 HKEY_DYN_DATA 主键  
        DynDa = 5,

        //对应于 HKEY_PERFORMANCE_DATA 主键  
        PerformanceData = 6,
    }
}
