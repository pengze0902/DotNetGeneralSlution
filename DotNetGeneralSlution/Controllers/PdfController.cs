using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotNetGeneralSlution.Controllers
{
    public class PdfController : Controller
    {
        // GET: Pdf
        public ActionResult Index()
        {
            ReamePathName("D:/DotNetGeneralSlution/DotNetGeneralSlution/File");
            return View();
        }

        /// <summary>
        /// 修改PDF文件的文件名
        /// </summary>
        /// <param name="folderPath"></param>
        public void ReamePathName(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentNullException(nameof(folderPath));
            }
            if (Directory.Exists(folderPath) == false)
            {
                throw new ArgumentNullException("当前文件夹不存在！");
            }
            DirectoryInfo folder = new DirectoryInfo(folderPath);
            foreach (FileInfo file in folder.GetFiles("*.pdf"))
            {
                if (System.IO.File.Exists(file.FullName))
                {
                    var filePath = file.FullName.Replace("#", "_");
                    if (System.IO.File.Exists(filePath) == false)
                    {
                        file.MoveTo(filePath);
                    }                  
                }             
            }
        }
    }
}