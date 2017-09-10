using System;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;

namespace AuxiliaryLibrary.Configuration
{
    /// <summary>
    /// 配置信息工具类
    /// </summary>
    public static class ConfigUtil
    {
        #region GetAppSettings(获取AppSettings)
        /// <summary>
        /// 获取AppSettings
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public static string GetAppSettings(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            return ConfigurationManager.AppSettings[key];
        }
        #endregion

        #region SetAppSettings(设置AppSettings)
        /// <summary>
        /// 设置AppSettings
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="value">键值</param>
        public static void SetAppSettings(string key, string value)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;
            if (settings.AllKeys.Contains(key))
            {
                settings[key].Value = value;
            }
            else
            {
                settings.Add(key,value);
            }
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
        #endregion

        #region RemoceAppSettings(移除AppSettings)
        /// <summary>
        /// 移除AppSettings
        /// </summary>
        /// <param name="key">键名</param>
        public static void RemoveAppSettings(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings == null||config.AppSettings.Settings[key]==null)
            {
                return;
            }
            config.AppSettings.Settings.Remove(key);
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
        #endregion

        #region GetConnectionString(获取数据库连接字符串)
        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public static string GetConnectionString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }
        #endregion

        #region SetConnectionString(设置数据库连接字符串)
        /// <summary>
        /// 设置数据库连接字符串
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="connectionStr">数据库连接字符串</param>
        /// <param name="providerName">数据提供程序名称</param>
        public static void SetConnectionString(string key, string connectionStr, string providerName = "")
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionSettings = config.ConnectionStrings.ConnectionStrings;
            if (connectionSettings[key] != null)
            {
                connectionSettings[key].ConnectionString = connectionStr;
                if (!string.IsNullOrEmpty(providerName))
                {
                    connectionSettings[key].ProviderName = providerName;
                }
            }
            else
            {
                connectionSettings.Add(new ConnectionStringSettings()
                {
                    Name = key,
                    ConnectionString = connectionStr,
                    ProviderName = providerName
                });
            }
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");
        }
        #endregion

        #region GetProviderName(获取数据提供程序名称)
        /// <summary>
        /// 获取数据提供程序名称
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public static string GetProviderName(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            return ConfigurationManager.ConnectionStrings[key].ProviderName;
        }
        #endregion
        #region GetSystemWebSection(获取system.web节点)
        /// <summary>
        /// 获取system.web节点
        /// </summary>
        /// <typeparam name="T">配置节点类型</typeparam>
        /// <param name="sections">节点类型</param>
        /// <returns></returns>
        public static T GetSystemWebSection<T>(SystemWebSections sections) where T : class
        {
            switch (sections)
            {
                case SystemWebSections.Authentication:
                    var authenticationSection = WebConfigurationManager.GetSection("system.web/authentication") as AuthenticationSection;
                    return authenticationSection as T;
                case SystemWebSections.Compilation:
                    var compilationSection = WebConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
                    return compilationSection as T;
                case SystemWebSections.CustomErrors:
                    var customErrorsSection = WebConfigurationManager.GetSection("system.web/customErrors") as CustomErrorsSection;
                    return customErrorsSection as T;
                case SystemWebSections.Globalization:
                    var globalizationSection = WebConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection;
                    return globalizationSection as T;
                case SystemWebSections.HttpRuntime:
                    var httpRuntimeSection = WebConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
                    return httpRuntimeSection as T;
                case SystemWebSections.Identity:
                    var identitySection = WebConfigurationManager.GetSection("system.web/identity") as IdentitySection;
                    return identitySection as T;
                case SystemWebSections.Trace:
                    var traceSection = WebConfigurationManager.GetSection("system.web/trace") as TraceSection;
                    return traceSection as T;
                default:
                    return default(T);
            }
        }
        #endregion
    }
}
