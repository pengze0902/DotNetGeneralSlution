using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace AuxiliaryLibrary.NPOI
{
    public class ExcelHelper
    { /// <summary>
      /// 将DataTable数据导入到excel中
      /// </summary>
      /// <param name="data">要导入的数据</param>
      /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
      /// <param name="sheetName">要导入的excel的sheet的名称</param>
      /// <param name="fileName">文件夹路径</param>
      /// <returns>导入数据行数(包含列名那一行)</returns>
        public static int DataTableToExcel(DataTable data, string sheetName, string fileName, bool isColumnWritten = true)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (string.IsNullOrEmpty(sheetName))
            {
                throw new ArgumentNullException(sheetName);
            }
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(fileName);
            }
            IWorkbook workbook = null;
            int count;
            //判断导入文件版本，.xlsx为2007版本，.xls为2003版本
            if (fileName.IndexOf(".xlsx", StringComparison.Ordinal) > 0)
            {
                workbook = new XSSFWorkbook();
            }
            if (fileName.IndexOf(".xls", StringComparison.Ordinal) > 0)
            {
                workbook = new HSSFWorkbook();
            }
            if (workbook == null)
            {
                return -1;
            }
            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                //创建新的工作表，设置工作表名称
                ISheet sheet = workbook.CreateSheet(sheetName);
                int j;
                //写入DataTable的列名，写入单元格中
                if (isColumnWritten)
                {
                    //在工作表中创建一个新行
                    var row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        //将DataTable数据导入创建的表中,设置单元格的字符串值。
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }
                //遍历循环datatable具体数据项
                int i;
                for (i = 0; i < data.Rows.Count; ++i)
                {
                    var row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        //设置具体的数据值
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                //将文件流写入到excel
                workbook.Write(fs);
            }
            return count;
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <param name="fileName">文件路径</param>
        /// <returns>返回的DataTable</returns>
        public static DataTable ExcelToDataTable(string sheetName, string fileName, bool isFirstRowColumn = true)
        {
            if (string.IsNullOrEmpty(sheetName))
            {
                throw new ArgumentNullException(sheetName);
            }
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(fileName);
            }
            var data = new DataTable();
            IWorkbook workbook = null;
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                //判断导入文件版本，.xlsx为2007版本，.xls为2003版本
                if (fileName.IndexOf(".xlsx", StringComparison.Ordinal) > 0)
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (fileName.IndexOf(".xls", StringComparison.Ordinal) > 0)
                {
                    workbook = new HSSFWorkbook(fs);
                }
                if (workbook == null)
                {
                    return data;
                }
                var sheet = workbook.GetSheet(sheetName) ?? workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    return data;
                }
                var firstRow = sheet.GetRow(0);
                //一行最后一个cell的编号 即总的列数
                int cellCount = firstRow.LastCellNum;
                int startRow;
                if (isFirstRowColumn)
                {
                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                    {
                        var cell = firstRow.GetCell(i);
                        //表头必须为字符串
                        var cellValue = cell.StringCellValue;
                        if (cellValue == null) continue;
                        var column = new DataColumn(cellValue);
                        data.Columns.Add(column);
                    }
                    startRow = sheet.FirstRowNum + 1;
                }
                else
                {
                    //获取工作表上的第一行
                    startRow = sheet.FirstRowNum;
                }
                //最后一列的标号
                var rowCount = sheet.LastRowNum;
                for (var i = startRow; i <= rowCount; ++i)
                {
                    var row = sheet.GetRow(i);
                    //没有数据的行默认是null
                    if (row == null) continue;
                    var dataRow = data.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                    {
                        //同理，没有数据的单元格都默认是null
                        if (row.GetCell(j) != null)
                        {
                            dataRow[j] = row.GetCell(j).ToString();
                        }
                    }
                    data.Rows.Add(dataRow);
                }
            }
            return data;
        }

        /// <summary>
        /// 读取Excel文件内容转换为DataSet,列名依次为 "c0"……c[columnlength-1]
        /// </summary>
        /// <param name="fileName">文件绝对路径</param>
        /// <param name="startRow">数据开始行数(1为第一行)</param>
        /// <param name="columnDataType">每列的数据类型</param>
        /// <returns></returns>
        public static DataSet ReadExcel(string fileName, int startRow, params NpoiDataType[] columnDataType)
        {
            var ds = new DataSet("ds");
            var dt = new DataTable("dt");
            var sb = new StringBuilder();
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                //使用接口，自动识别excel2003/2007格式
                var workbook = WorkbookFactory.Create(stream);
                //得到里面第一个sheet
                var sheet = workbook.GetSheetAt(0);
                int j;
                IRow row;
                //ColumnDataType赋值
                if (columnDataType.Length <= 0)
                {
                    //得到第i行
                    row = sheet.GetRow(startRow - 1);
                    columnDataType = new NpoiDataType[row.LastCellNum];
                    for (var i = 0; i < row.LastCellNum; i++)
                    {
                        var hs = row.GetCell(i);
                        columnDataType[i] = GetCellDataType(hs);
                    }
                }
                for (j = 0; j < columnDataType.Length; j++)
                {
                    var tp = GetDataTableType(columnDataType[j]);
                    dt.Columns.Add("c" + j, tp);
                }
                for (var i = startRow - 1; i <= sheet.PhysicalNumberOfRows; i++)
                {
                    //得到第i行
                    row = sheet.GetRow(i);
                    if (row == null) continue;
                    try
                    {
                        var dr = dt.NewRow();

                        for (j = 0; j < columnDataType.Length; j++)
                        {
                            dr["c" + j] = GetCellData(columnDataType[j], row, j);
                        }
                        dt.Rows.Add(dr);
                    }
                    catch (Exception er)
                    {
                        sb.Append(string.Format("第{0}行出错：{1}\r\n", i + 1, er.Message));
                    }
                }
                ds.Tables.Add(dt);
            }
            return ds;
        }

        /// <summary>
        /// 从DataSet导出到MemoryStream流2003
        /// </summary>
        /// <param name="saveFileName">文件保存路径</param>
        /// <param name="sheetName">Excel文件中的Sheet名称</param>
        /// <param name="ds">存储数据的DataSet</param>
        /// <param name="startRow">从哪一行开始写入，从0开始</param>
        /// <param name="datatypes">DataSet中的各列对应的数据类型</param>
        public static bool CreateExcel2003(string saveFileName, string sheetName, DataSet ds, int startRow = 0, params NpoiDataType[] datatypes)
        {
            try
            {
                var wb = new HSSFWorkbook();
                var dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "pkm";
                var si = PropertySetFactory.CreateSummaryInformation();
                si.Title =
                si.Subject = "automatic genereted document";
                si.Author = "pkm";
                wb.DocumentSummaryInformation = dsi;
                wb.SummaryInformation = si;
                var sheet = wb.CreateSheet(sheetName);
                //sheet.SetColumnWidth(0, 50 * 256);
                //sheet.SetColumnWidth(1, 100 * 256);
                ICell cell;
                int j;
                var maxLength = 0;
                var curLength = 0;
                object columnValue;
                var dt = ds.Tables[0];
                if (datatypes.Length < dt.Columns.Count)
                {
                    datatypes = new NpoiDataType[dt.Columns.Count];
                    for (var i = 0; i < dt.Columns.Count; i++)
                    {
                        var dtcolumntype = dt.Columns[i].DataType.Name.ToLower();
                        switch (dtcolumntype)
                        {
                            case "string":
                                datatypes[i] = NpoiDataType.String;
                                break;
                            case "datetime":
                                datatypes[i] = NpoiDataType.Datetime;
                                break;
                            case "boolean":
                                datatypes[i] = NpoiDataType.Bool;
                                break;
                            case "double":
                                datatypes[i] = NpoiDataType.Numeric;
                                break;
                            default:
                                datatypes[i] = NpoiDataType.String;
                                break;
                        }
                    }
                }

                // 创建表头
                var row = sheet.CreateRow(0);
                //样式
                var style1 = wb.CreateCellStyle();
                //字体
                var font1 = wb.CreateFont();
                //字体颜色
                font1.Color = HSSFColor.White.Index;
                //字体加粗样式
                font1.Boldweight = (short)FontBoldWeight.Bold;
                //style1.FillBackgroundColor = HSSFColor.WHITE.index;                                                            
                style1.FillForegroundColor = HSSFColor.Green.Index;
                //GetXLColour(wb, LevelOneColor);// 设置图案色
                //GetXLColour(wb, LevelOneColor);// 设置背景色
                style1.FillPattern = FillPattern.SolidForeground;
                //样式里的字体设置具体的字体样式
                style1.SetFont(font1);
                //文字水平对齐方式
                style1.Alignment = HorizontalAlignment.Center;
                //文字垂直对齐方式
                style1.VerticalAlignment = VerticalAlignment.Center;
                row.HeightInPoints = 25;
                for (j = 0; j < dt.Columns.Count; j++)
                {
                    columnValue = dt.Columns[j].ColumnName;
                    curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                    maxLength = (maxLength < curLength ? curLength : maxLength);
                    var colounwidth = 256 * maxLength;
                    sheet.SetColumnWidth(j, colounwidth);
                    try
                    {
                        //创建第0行的第j列
                        cell = row.CreateCell(j);
                        //单元格式设置样式
                        cell.CellStyle = style1;

                        try
                        {
                            cell.SetCellType(CellType.String);
                            cell.SetCellValue(columnValue.ToString());
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                // 创建每一行
                for (var i = startRow; i < ds.Tables[0].Rows.Count; i++)
                {
                    var dr = ds.Tables[0].Rows[i];
                    //创建第i行
                    row = sheet.CreateRow(i + 1);
                    for (j = 0; j < dt.Columns.Count; j++)
                    {
                        columnValue = dr[j];
                        curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                        maxLength = (maxLength < curLength ? curLength : maxLength);
                        var colounwidth = 256 * maxLength;
                        sheet.SetColumnWidth(j, colounwidth);
                        try
                        {
                            //创建第i行的第j列
                            cell = row.CreateCell(j);
                            // 插入第j列的数据
                            try
                            {
                                var dtype = datatypes[j];
                                switch (dtype)
                                {
                                    case NpoiDataType.String:
                                        {
                                            cell.SetCellType(CellType.Numeric);
                                            cell.SetCellValue(columnValue.ToString());
                                        }
                                        break;
                                    case NpoiDataType.Datetime:
                                        {
                                            cell.SetCellType(CellType.Numeric);
                                            cell.SetCellValue(columnValue.ToString());
                                        }
                                        break;
                                    case NpoiDataType.Numeric:
                                        {
                                            cell.SetCellType(CellType.Numeric);
                                            cell.SetCellValue(Convert.ToDouble(columnValue));
                                        }
                                        break;
                                    case NpoiDataType.Bool:
                                        {
                                            cell.SetCellType(CellType.Numeric);
                                            cell.SetCellValue(Convert.ToBoolean(columnValue));
                                        }
                                        break;
                                    case NpoiDataType.Richtext:
                                        {
                                            cell.SetCellType(CellType.Numeric);
                                            cell.SetCellValue(columnValue.ToString());
                                        }
                                        break;
                                }
                            }
                            catch
                            {
                                cell.SetCellType(CellType.Numeric);
                                cell.SetCellValue(columnValue.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                //生成文件在服务器上
                using (var fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    wb.Write(fs);
                }

                return true;
            }
            catch (Exception er)
            {
                throw er;
            }

        }

        /// <summary>
        /// 从DataSet导出到MemoryStream流2007
        /// </summary>
        /// <param name="saveFileName">文件保存路径</param>
        /// <param name="sheetName">Excel文件中的Sheet名称</param>
        /// <param name="ds">存储数据的DataSet</param>
        /// <param name="startRow">从哪一行开始写入，从0开始</param>
        /// <param name="datatypes">DataSet中的各列对应的数据类型</param>
        public static bool CreateExcel2007(string saveFileName, string sheetName, DataSet ds, int startRow = 0, params NpoiDataType[] datatypes)
        {
            if (string.IsNullOrEmpty(saveFileName))
            {
                throw new ArgumentNullException(saveFileName);
            }
            if (string.IsNullOrEmpty(sheetName))
            {
                throw new ArgumentNullException(sheetName);
            }
            if (ds == null)
            {
                throw new ArgumentNullException("ds");
            }
            var wb = new XSSFWorkbook();
            var sheet = wb.CreateSheet(sheetName);
            var maxLength = 0;
            var dt = ds.Tables[0];
            if (datatypes.Length < dt.Columns.Count)
            {
                datatypes = new NpoiDataType[dt.Columns.Count];
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    var dtcolumntype = dt.Columns[i].DataType.Name.ToLower();
                    switch (dtcolumntype)
                    {
                        case "string":
                            datatypes[i] = NpoiDataType.String;
                            break;
                        case "datetime":
                            datatypes[i] = NpoiDataType.Datetime;
                            break;
                        case "boolean":
                            datatypes[i] = NpoiDataType.Bool;
                            break;
                        case "double":
                            datatypes[i] = NpoiDataType.Numeric;
                            break;
                        default:
                            datatypes[i] = NpoiDataType.String;
                            break;
                    }
                }
            }
            try
            {
                //创建表头
                var row = sheet.CreateRow(0);
                //样式
                var style1 = wb.CreateCellStyle();
                //字体
                var font1 = wb.CreateFont();
                //字体颜色
                font1.Color = HSSFColor.White.Index;
                //字体加粗样式
                font1.Boldweight = (short)FontBoldWeight.Bold;
                //style1.FillBackgroundColor = HSSFColor.WHITE.index;
                //GetXLColour(wb, LevelOneColor);
                // 设置图案色
                style1.FillForegroundColor = HSSFColor.Green.Index;
                //GetXLColour(wb, LevelOneColor);// 设置背景色
                style1.FillPattern = FillPattern.SolidForeground;
                //样式里的字体设置具体的字体样式
                style1.SetFont(font1);
                //文字水平对齐方式
                style1.Alignment = HorizontalAlignment.Center;
                //文字垂直对齐方式
                style1.VerticalAlignment = VerticalAlignment.Center;
                row.HeightInPoints = 25;
                ICell cell;
                int j;
                int curLength;
                object columnValue;
                for (j = 0; j < dt.Columns.Count; j++)
                {
                    columnValue = dt.Columns[j].ColumnName;
                    curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                    maxLength = (maxLength < curLength ? curLength : maxLength);
                    var colounwidth = 256 * maxLength;
                    sheet.SetColumnWidth(j, colounwidth);
                    try
                    {
                        //创建第0行的第j列
                        cell = row.CreateCell(j);
                        //单元格式设置样式
                        cell.CellStyle = style1;

                        try
                        {
                            cell.SetCellValue(columnValue.ToString());
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                // 创建每一行
                for (var i = startRow; i < ds.Tables[0].Rows.Count; i++)
                {
                    var dr = ds.Tables[0].Rows[i];
                    //创建第i行
                    row = sheet.CreateRow(i + 1);
                    for (j = 0; j < dt.Columns.Count; j++)
                    {
                        columnValue = dr[j];
                        curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                        maxLength = (maxLength < curLength ? curLength : maxLength);
                        var colounwidth = 256 * maxLength;
                        sheet.SetColumnWidth(j, colounwidth);
                        try
                        {
                            //创建第i行的第j列
                            cell = row.CreateCell(j);
                            // 插入第j列的数据
                            try
                            {
                                var dtype = datatypes[j];
                                switch (dtype)
                                {
                                    case NpoiDataType.String:
                                        {
                                            cell.SetCellValue(columnValue.ToString());
                                        }
                                        break;
                                    case NpoiDataType.Datetime:
                                        {
                                            cell.SetCellValue(columnValue.ToString());
                                        }
                                        break;
                                    case NpoiDataType.Numeric:
                                        {
                                            cell.SetCellValue(Convert.ToDouble(columnValue));
                                        }
                                        break;
                                    case NpoiDataType.Bool:
                                        {
                                            cell.SetCellValue(Convert.ToBoolean(columnValue));
                                        }
                                        break;
                                    case NpoiDataType.Richtext:
                                        {
                                            cell.SetCellValue(columnValue.ToString());
                                        }
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                cell.SetCellValue(columnValue.ToString());
                                throw ex;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                //生成文件在服务器上
                using (var fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    wb.Write(fs);
                }
                return true;
            }
            catch (Exception er)
            {
                throw er;
            }

        }

        /// <summary>
        /// 读Excel-根据NpoiDataType创建的DataTable列的数据类型
        /// </summary>
        /// <param name="datatype"></param>
        /// <returns></returns>
        private static Type GetDataTableType(NpoiDataType datatype)
        {
            var tp = typeof(string);
            switch (datatype)
            {
                case NpoiDataType.Bool:
                    tp = typeof(bool);
                    break;
                case NpoiDataType.Datetime:
                    tp = typeof(DateTime);
                    break;
                case NpoiDataType.Numeric:
                    tp = typeof(double);
                    break;
                case NpoiDataType.Error:
                    tp = typeof(string);
                    break;
                case NpoiDataType.Blank:
                    tp = typeof(string);
                    break;
            }
            return tp;
        }


        /// <summary>
        /// 读Excel-得到不同数据类型单元格的数据
        /// </summary>
        /// <param name="datatype">数据类型</param>
        /// <param name="row">数据中的一行</param>
        /// <param name="column">哪列</param>
        /// <returns></returns>
        private static object GetCellData(NpoiDataType datatype, IRow row, int column)
        {
            if (row == null)
            {
                throw new ArgumentNullException("row");
            }
            switch (datatype)
            {
                case NpoiDataType.String:
                    try
                    {
                        return row.GetCell(column).DateCellValue;
                    }
                    catch
                    {
                        try
                        {
                            return row.GetCell(column).StringCellValue;
                        }
                        catch
                        {
                            return row.GetCell(column).NumericCellValue;
                        }
                    }
                case NpoiDataType.Bool:
                    try
                    {
                        return row.GetCell(column).BooleanCellValue;
                    }
                    catch
                    {
                        return row.GetCell(column).StringCellValue;
                    }
                case NpoiDataType.Datetime:
                    try
                    {
                        return row.GetCell(column).DateCellValue;
                    }
                    catch
                    {
                        return row.GetCell(column).StringCellValue;
                    }
                case NpoiDataType.Numeric:
                    try
                    {
                        return row.GetCell(column).NumericCellValue;
                    }
                    catch
                    {
                        return row.GetCell(column).StringCellValue;
                    }
                case NpoiDataType.Richtext:
                    try
                    {
                        return row.GetCell(column).RichStringCellValue;
                    }
                    catch
                    {
                        return row.GetCell(column).StringCellValue;
                    }
                case NpoiDataType.Error:
                    try
                    {
                        return row.GetCell(column).ErrorCellValue;
                    }
                    catch
                    {
                        return row.GetCell(column).StringCellValue;
                    }
                case NpoiDataType.Blank:
                    try
                    {
                        return row.GetCell(column).StringCellValue;
                    }
                    catch
                    {
                        return "";
                    }
                default: return "";
            }
        }

        /// <summary>
        /// 获取单元格数据类型
        /// </summary>
        /// <param name="hs">单元格对象</param>
        /// <returns></returns>
        private static NpoiDataType GetCellDataType(ICell hs)
        {
            if (hs == null)
            {
                throw new ArgumentNullException("hs");
            }

            NpoiDataType dtype;
            DateTime t1;
            var cellvalue = string.Empty;

            switch (hs.CellType)
            {
                case CellType.Blank:
                    dtype = NpoiDataType.String;
                    cellvalue = hs.StringCellValue;
                    break;
                case CellType.Boolean:
                    dtype = NpoiDataType.Bool;
                    break;
                case CellType.Numeric:
                    dtype = NpoiDataType.Numeric;
                    cellvalue = hs.NumericCellValue.ToString(CultureInfo.InvariantCulture);
                    break;
                case CellType.String:
                    dtype = NpoiDataType.String;
                    cellvalue = hs.StringCellValue;
                    break;
                case CellType.Error:
                    dtype = NpoiDataType.Error;
                    break;
                default:
                    dtype = NpoiDataType.Datetime;
                    break;
            }
            if (cellvalue != string.Empty && DateTime.TryParse(cellvalue, out t1))
            {
                dtype = NpoiDataType.Datetime;
            }
            return dtype;
        }
    }
}

