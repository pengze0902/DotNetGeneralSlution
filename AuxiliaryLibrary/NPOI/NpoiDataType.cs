namespace AuxiliaryLibrary.NPOI
{
    /// <summary>
    /// 枚举(Excel单元格数据类型)
    /// </summary>
    public enum NpoiDataType
    {
        /// <summary>
        /// 字符串类型-值为1
        /// </summary>
        String,
        /// <summary>
        /// 布尔类型-值为2
        /// </summary>
        Bool,
        /// <summary>
        /// 时间类型-值为3
        /// </summary>
        Datetime,
        /// <summary>
        /// 数字类型-值为4
        /// </summary>
        Numeric,
        /// <summary>
        /// 复杂文本类型-值为5
        /// </summary>
        Richtext,
        /// <summary>
        /// 空白
        /// </summary>
        Blank,
        /// <summary>
        /// 错误
        /// </summary>
        Error
    }
}
