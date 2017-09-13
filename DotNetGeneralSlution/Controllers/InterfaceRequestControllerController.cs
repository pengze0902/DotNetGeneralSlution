using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using AuxiliaryLibrary.HTTP;
using DotNetGeneralSlution.Models;

namespace DotNetGeneralSlution.Controllers
{
    public class InterfaceRequestControllerController : Controller
    {
        // GET: InterfaceRequestController
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 请求接口
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="requestMethod">请求方法</param>
        /// <param name="requestNumber">请求次数</param>
        /// <param name="requestData">请求数据</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReqInter(string requestUrl, string requestMethod, string requestNumber, string requestData)
        {
            if (IsInt(requestNumber) == false)
            {
                return Json("请填写正确的请求次数！");
            }
            if (IsUrl(requestUrl) == false)
            {
                return Json("请填写正确的请求URL！");
            }
            if (string.IsNullOrEmpty(requestData))
            {
                return Json("请填写请求参数！");
            }
            InterfaceRequestModels interfaceRequest = new InterfaceRequestModels
            {
                Url = requestUrl,
                NumberRequests = Convert.ToInt16(requestNumber),
                ParameterDictionary = ParameterDic(requestData)
            };
            return Json(InterfaceRequest(interfaceRequest));
        }

        /// <summary>
        /// HTTP请求
        /// </summary>
        /// <param name="interfaceRequest"></param>
        /// <returns></returns>
        public string InterfaceRequest(InterfaceRequestModels interfaceRequest)
        {
            if (interfaceRequest == null)
            {
                throw new ArgumentNullException(nameof(interfaceRequest));
            }
            try
            {
                if (interfaceRequest.NumberRequests == 1)
                {
                    interfaceRequest.ReturnData = new WebUtils().DoPost(interfaceRequest.Url, interfaceRequest.ParameterDictionary);
                }
                else
                {
                    for (int i = 0; i < interfaceRequest.NumberRequests; i++)
                    {
                        new Thread(() => new WebUtils().DoPost(interfaceRequest.Url, interfaceRequest.ParameterDictionary)).Start();
                    }
                }
            }
            catch (Exception ex)
            {
                interfaceRequest.ReturnData = ex.Message;
                throw ex;
            }
            return interfaceRequest.ReturnData;

        }

        /// <summary>
        /// 请求参数转化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Dictionary<string, string> ParameterDic(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (Regex.IsMatch(data, "{") == false || Regex.IsMatch(data, "}") == false)
            {
                return null;
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var dataDic in data.Split('#'))
            {
                foreach (var str in dataDic.Replace("{", "").Replace("}", "").Split(':'))
                {
                    dic[str[0].ToString()] = str[1].ToString();
                }
            }
            return dic;
        }

        /// <summary>
        /// 是否数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        /// <summary>
        /// 是否int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }

        /// <summary>
        /// 判断一个字符串是否为url
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsUrl(string str)
        {
            try
            {
                string Url = @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
                return Regex.IsMatch(str, Url);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}