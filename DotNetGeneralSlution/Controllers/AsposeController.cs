using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aspose.Words;
using Aspose.Words.Saving;

namespace DotNetGeneralSlution.Controllers
{
    public class AsposeController : Controller
    {
        // GET: Aspose
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            var fileName = file.FileName;
            var filePath = Server.MapPath(string.Format("~/{0}", "File"));
            file.SaveAs(Path.Combine(filePath, fileName));
            return Json(filePath);
        }

        [HttpPost]
        public void AsposeDocument()
        {
            string tempPath = Server.MapPath("~/File/劳务用工合同范本.doc");
            string outputPath = Server.MapPath("~/File/正式合同.doc");
            var doc = new Document(tempPath);
            doc.MailMerge.Execute(CreateDataTable());
            doc.Save(outputPath);
        }


        [HttpPost]
        public void DocForPDF()
        {
            string tempPath = Server.MapPath("~/File/正式合同.doc");
            string outputPath = Server.MapPath("~/File/正式合同.pdf");

            Document doc = new Document(new FileStream(tempPath, FileMode.Open));

            doc.Save(outputPath,SaveFormat.Pdf);
        }


        /// <summary>
        /// 将Word文档转换为图片      
        /// </summary>
        /// <param name="wordInputPath">Word文件路径</param>
        /// <param name="imageOutputDirPath">图片输出路径，如果为空，默认值为Word所在路径</param>      
        /// <param name="startPageNum">从PDF文档的第几页开始转换，如果为0，默认值为1</param>
        /// <param name="endPageNum">从PDF文档的第几页开始停止转换，如果为0，默认值为Word总页数</param>
        /// <param name="imageFormat">设置所需图片格式，如果为null，默认格式为PNG</param>
        /// <param name="resolution">设置图片的像素，数字越大越清晰，如果为0，默认值为128，建议最大值不要超过1024</param>
        private void ConvertToImage(string wordInputPath, string imageOutputDirPath, int startPageNum, int endPageNum, ImageFormat imageFormat, int resolution)
        {
            Document doc = new Document(wordInputPath);
            string imageName = Path.GetFileNameWithoutExtension(wordInputPath);
            ImageSaveOptions imageSaveOptions = new ImageSaveOptions(SaveFormat.Png);
            imageSaveOptions.Resolution = resolution;
            for (int i = startPageNum; i <= endPageNum; i++)
            {
                MemoryStream stream = new MemoryStream();
                imageSaveOptions.PageIndex = i - 1;
                string imgPath = Path.Combine(imageOutputDirPath, imageName) + "_" + i.ToString("000") + "." + imageFormat.ToString();
                doc.Save(stream, imageSaveOptions);

            }
        }


        public DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn("甲方姓名", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("乙方姓名", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("工程名称", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("用工方式", Type.GetType("System.String"));
            DataColumn dc5 = new DataColumn("工期", Type.GetType("System.String"));
            DataColumn dc6 = new DataColumn("签署日期", Type.GetType("System.String"));

            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dt.Columns.Add(dc5);
            dt.Columns.Add(dc6);


            DataRow row = dt.NewRow();
            row["甲方姓名"] = "深圳明源云";
            row["乙方姓名"] = "跟投项目组";
            row["工程名称"] = "信息工程建设";
            row["用工方式"] = "聘用";
            row["工期"] = "10年";
            row["签署日期"] = "2017-10-01";
            dt.Rows.Add(row);

            return dt;
        }
    }
}