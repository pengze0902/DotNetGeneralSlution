namespace AuxiliaryLibrary.SiteDeployment
{
    /// <summary>
    /// 站点信息
    /// </summary>
    public class NewWebSiteInfo
    {
        /// <summary>
        /// 主机IP
        /// </summary>
        private readonly string _hostIp;

        /// <summary>
        /// 网站端口号
        /// </summary>
        private readonly string _portNum;

        /// <summary>
        /// 网站表示。一般为网站的网站名。例如"www.dns.com.cn"
        /// </summary>
        private readonly string _descOfWebSite;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="hostIp"></param>
        /// <param name="portNum"></param>
        /// <param name="descOfWebSite"></param>
        /// <param name="commentOfWebSite"></param>
        /// <param name="webPath"></param>
        public NewWebSiteInfo(string hostIp, string portNum, string descOfWebSite, string commentOfWebSite, string webPath)
        {
            _hostIp = hostIp;
            _portNum = portNum;
            _descOfWebSite = descOfWebSite;
            CommentOfWebSite = commentOfWebSite;
            WebPath = webPath;
        }

        /// <summary>
        ///  //网站标识（IP,端口，主机头值）
        /// </summary>
        public string BindString => $"{_hostIp}:{_portNum}:{_descOfWebSite}";

        /// <summary>
        /// 端口号
        /// </summary>
        public string PortNum => _portNum;

        public string CommentOfWebSite { get; }

        /// <summary>
        /// 站点路径
        /// </summary>
        public string WebPath { get; }
    }
}
