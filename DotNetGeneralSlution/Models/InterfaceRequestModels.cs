using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetGeneralSlution.Models
{
    public class InterfaceRequestModels
    {
        /// <summary>
        /// 请求RUL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public string RequestMethod { get; set; } = "POST";

        /// <summary>
        /// 参数集合
        /// </summary>

        public Dictionary<string, string> ParameterDictionary { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 请求次数
        /// </summary>
        public int NumberRequests { get; set; } = 1;

        /// <summary>
        /// 参数数据
        /// </summary>
        public string ReturnData { get; set; }
    }
}