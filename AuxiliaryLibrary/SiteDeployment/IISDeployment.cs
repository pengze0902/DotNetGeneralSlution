using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;
using NPOI.SS.Formula.Functions;

namespace AuxiliaryLibrary.SiteDeployment
{
    /// <summary>
    /// IIS站点部署
    /// </summary>
    public class IISDeployment
    {
        /// <summary>
        /// 获取IIS站点信息
        /// </summary>
        /// <returns></returns>
        public string GetIisInfor()
        {
            DirectoryEntry getEntity = new DirectoryEntry("IIS://localhost/W3SVC/INFO");
            return getEntity.Properties["MajorIISVersionNumber"].Value.ToString();
        }

        /// <summary>
        /// 判断程序池是否存在
        /// </summary>
        /// <param name="appPoolName">程序池名称</param>
        /// <returns>true存在 false不存在</returns>
        public bool IsAppPoolName(string appPoolName)
        {
            bool result = false;
            DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
            foreach (DirectoryEntry getdir in appPools.Children)
            {
                if (getdir.Name.Equals(appPoolName))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 删除指定程序池
        /// </summary>
        /// <param name="appPoolName">程序池名称</param>
        /// <returns>true删除成功 false删除失败</returns>
        public bool DeleteAppPool(string appPoolName)
        {
            bool result = false;
            DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
            foreach (DirectoryEntry getdir in appPools.Children)
            {
                if (getdir.Name.Equals(appPoolName))
                {
                    try
                    {
                        getdir.DeleteTree();
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 创建应用程序池IIS7
        /// </summary>
        /// <param name="appPoolName"></param>
        /// <returns></returns>
        public bool CrateAppPool(string appPoolName)
        {
            if (!IsAppPoolName(appPoolName))
            {
                DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
                var newpool = appPools.Children.Add(appPoolName, "IIsApplicationPool");
                newpool.CommitChanges();
            }
            return true;
        }


        /// <summary>
        /// 修改应用程序的配置(包含托管模式及其NET运行版本)IIS7
        /// </summary>
        /// <param name="appPoolName"></param>
        /// <param name="poolVersion">v4.0</param>
        /// <returns></returns>
        public bool ModifyConfigurationApplication(string appPoolName, string poolVersion)
        {
            if (string.IsNullOrEmpty(poolVersion))
            {
                throw new ArgumentNullException(nameof(poolVersion));
            }
            if (!IsAppPoolName(appPoolName))
            {
                ServerManager sm = new ServerManager();
                sm.ApplicationPools[appPoolName].ManagedRuntimeVersion = poolVersion;
                sm.ApplicationPools[appPoolName].ManagedPipelineMode = ManagedPipelineMode.Classic;
                //托管模式Integrated为集成 Classic为经典
                sm.CommitChanges();
            }
            return true;
        }


        /// <summary>
        /// 创建网站
        /// </summary>
        /// <param name="siteInfo"></param>
        public void CreateNewWebSite(NewWebSiteInfo siteInfo)
        {
            string entPath = $"IIS://{"localhost"}/w3svc";
            DirectoryEntry rootEntry = new DirectoryEntry(entPath);
            DirectoryEntry newSiteEntry = rootEntry.Children.Add(siteInfo.PortNum, "IIsWebServer");
            newSiteEntry.CommitChanges();
            newSiteEntry.Properties["ServerBindings"].Value = siteInfo.BindString;
            newSiteEntry.Properties["ServerComment"].Value = siteInfo.CommentOfWebSite;
            newSiteEntry.CommitChanges();
            DirectoryEntry vdEntry = newSiteEntry.Children.Add("root", "IIsWebVirtualDir");
            vdEntry.CommitChanges();
            string changWebPath = siteInfo.WebPath.Trim().Remove(siteInfo.WebPath.Trim().LastIndexOf('\\'), 1);
            vdEntry.Properties["Path"].Value = changWebPath;
            //创建应用程序
            vdEntry.Invoke("AppCreate", true);
            //设置读取权限
            vdEntry.Properties["AccessRead"][0] = true; 
            vdEntry.Properties["AccessWrite"][0] = true;
            //执行权限
            vdEntry.Properties["AccessScript"][0] = true;
            vdEntry.Properties["AccessExecute"][0] = false;
            //设置默认文档
            vdEntry.Properties["DefaultDoc"][0] = "Login.aspx";
            //应用程序名称
            vdEntry.Properties["AppFriendlyName"][0] = "LabManager";
            //0表示不允许匿名访问,1表示就可以3为基本身份验证，7为windows继承身份验证
            vdEntry.Properties["AuthFlags"][0] = 1;
            vdEntry.CommitChanges();

            #region 针对IIS7
            DirectoryEntry getEntity = new DirectoryEntry("IIS://localhost/W3SVC/INFO");
            int version = int.Parse(getEntity.Properties["MajorIISVersionNumber"].Value.ToString());
            if (version > 6)
            {
                #region 创建应用程序池
                string AppPoolName = "LabManager";
                if (!IsAppPoolName(AppPoolName))
                {
                    DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
                    DirectoryEntry newpool = appPools.Children.Add(AppPoolName, "IIsApplicationPool");
                    newpool.CommitChanges();
                }
                #endregion

                #region 修改应用程序的配置(包含托管模式及其NET运行版本)
                ServerManager sm = new ServerManager();
                sm.ApplicationPools[AppPoolName].ManagedRuntimeVersion = "v4.0";
                //托管模式Integrated为集成 Classic为经典
                sm.ApplicationPools[AppPoolName].ManagedPipelineMode = ManagedPipelineMode.Classic;
                sm.CommitChanges();
                #endregion

                vdEntry.Properties["AppPoolId"].Value = AppPoolName;
                vdEntry.CommitChanges();
            }
            #endregion


            //启动aspnet_regiis.exe程序 
            string fileName = Environment.GetEnvironmentVariable("windir") + @"\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe";
            ProcessStartInfo startInfo = new ProcessStartInfo(fileName);
            //处理目录路径 
            string path = vdEntry.Path.ToUpper();
            int index = path.IndexOf("W3SVC", StringComparison.Ordinal);
            path = path.Remove(0, index);
            //启动ASPnet_iis.exe程序,刷新脚本映射 
            startInfo.Arguments = "-s " + path;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            string errors = process.StandardError.ReadToEnd();
            if (errors != string.Empty)
            {
                throw new Exception(errors);
            }

        }       
    }
}
