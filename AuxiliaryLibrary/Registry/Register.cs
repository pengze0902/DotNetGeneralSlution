using System;
using Microsoft.Win32;

namespace AuxiliaryLibrary.Registry
{
    ///注册表操作类  
    public class Register
    {
        /// <summary>    
        /// 设置注册表项名称    
        /// </summary>   
        public string SubKey { get; set; }

        /// <summary>    
        /// 注册表基项域    
        /// </summary>    
        public RegDomain Domain { get; set; }

        /// <summary>    
        /// 注册表键值    
        /// </summary>    
        public string RegeditKey { get; set; }
  
        /// <summary>
        /// 构造函数
        /// </summary>
        public Register()
        {
            //默认注册表项名称    
            SubKey = "software\\";
            //默认注册表基项域    
            Domain = RegDomain.LocalMachine;
        }

        /// <summary>    
        /// 构造函数    
        /// </summary>    
        /// <param name="subKey">注册表项名称</param>    
        /// <param name="regDomain">注册表基项域</param>    
        public Register(string subKey, RegDomain regDomain)
        {
            //设置注册表项名称    
            SubKey = subKey;

            //设置注册表基项域   
            Domain = regDomain;
        }

        #region 公有方法    
        #region 创建注册表项    

        /// <summary>    
        /// 创建注册表项，默认创建在注册表基项 HKEY_LOCAL_MACHINE下面（请先设置SubKey属性）    
        /// 虚方法，子类可进行重写    
        /// </summary>    
        public virtual void CreateSubKey()
        {
            //判断注册表项名称是否为空，如果为空，返回false    
            if (string.IsNullOrEmpty(SubKey))
            {
                return;
            }

            //创建基于注册表基项的节点  
            RegistryKey key = GetRegDomain(Domain);

            //要创建的注册表项的节点   
            RegistryKey sKey;
            if (!IsSubKeyExist())
            {
                sKey = key.CreateSubKey(SubKey);
            }
            //sKey.Close();   
            //关闭对注册表项的更改  
            key.Close();
        }

        /// <summary>   
        /// 创建注册表项，默认创建在注册表基项 HKEY_LOCAL_MACHINE下面   
        /// 虚方法，子类可进行重写   
        /// 例子：如subkey是software\\higame\\，则将创建HKEY_LOCAL_MACHINE\\software\\higame\\注册表项   
        /// </summary>   
        /// <param name="subKey">注册表项名称</param>   
        public virtual void CreateSubKey(string subKey)
        {
            //判断注册表项名称是否为空，如果为空，返回false   
            if (string.IsNullOrEmpty(subKey))
            {
                return;
            }

            //创建基于注册表基项的节点   
            RegistryKey key = GetRegDomain(Domain);

            //要创建的注册表项的节点   
            if (!IsSubKeyExist(subKey))
            {
                key.CreateSubKey(subKey);
            }  
            //关闭对注册表项的更改   
            key.Close();
        }

        /// <summary>   
        /// 创建注册表项，默认创建在注册表基项 HKEY_LOCAL_MACHINE下面   
        /// 虚方法，子类可进行重写   
        /// </summary>   
        /// <param name="regDomain">注册表基项域</param>   
        public virtual void CreateSubKey(RegDomain regDomain)
        {
            //判断注册表项名称是否为空，如果为空，返回false   
            if (string.IsNullOrEmpty(SubKey))
            {
                return;
            }

            //创建基于注册表基项的节点  
            RegistryKey key = GetRegDomain(regDomain);

            //要创建的注册表项的节点   
            if (!IsSubKeyExist(regDomain))
            {
                key.CreateSubKey(SubKey);
            }

            //关闭对注册表项的更改  
            key.Close();
        }

        /// <summary>  
        /// 创建注册表项（请先设置SubKey属性）  
        /// 虚方法，子类可进行重写  
        /// 例子：如regDomain是HKEY_LOCAL_MACHINE，subkey是software\\higame\\，则将创建HKEY_LOCAL_MACHINE\\software\\higame\\注册表项   
        /// </summary>   
        /// <param name="subKey">注册表项名称</param>  
        /// <param name="regDomain">注册表基项域</param>   
        public virtual void CreateSubKey(string subKey, RegDomain regDomain)
        {
            //判断注册表项名称是否为空，如果为空，返回false  
            if (string.IsNullOrEmpty(subKey))
            {
                return;
            }

            //创建基于注册表基项的节点   
            RegistryKey key = GetRegDomain(regDomain);

            //要创建的注册表项的节点   
            if (!IsSubKeyExist(subKey, regDomain))
            {
                key.CreateSubKey(subKey);
            }
 
            //关闭对注册表项的更改  
            key.Close();
        }
        #endregion

        #region 判断注册表项是否存在  
        /// <summary>   
        /// 判断注册表项是否存在，默认是在注册表基项HKEY_LOCAL_MACHINE下判断（请先设置SubKey属性）   
        /// 虚方法，子类可进行重写   
        /// 例子：如果设置了Domain和SubKey属性，则判断Domain\\SubKey，否则默认判断HKEY_LOCAL_MACHINE\\software\\   
        /// </summary>   
        /// <returns>返回注册表项是否存在，存在返回true，否则返回false</returns>   
        public virtual bool IsSubKeyExist()
        {
            //判断注册表项名称是否为空，如果为空，返回false  
            if (string.IsNullOrEmpty(SubKey))
            {
                return false;
            }

            //检索注册表子项   
            //如果sKey为null,说明没有该注册表项不存在，否则存在  
            RegistryKey sKey = OpenSubKey(SubKey, Domain);
            if (sKey == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>   
        /// 判断注册表项是否存在，默认是在注册表基项HKEY_LOCAL_MACHINE下判断   
        /// 虚方法，子类可进行重写   
        /// 例子：如subkey是software\\higame\\，则将判断HKEY_LOCAL_MACHINE\\software\\higame\\注册表项是否存在  
        /// </summary>  
        /// <param name="subKey">注册表项名称</param>  
        /// <returns>返回注册表项是否存在，存在返回true，否则返回false</returns>   
        public virtual bool IsSubKeyExist(string subKey)
        {
            //判断注册表项名称是否为空，如果为空，返回false  
            if (string.IsNullOrEmpty(subKey))
            {
                return false;
            }

            //检索注册表子项   
            //如果sKey为null,说明没有该注册表项不存在，否则存在   
            RegistryKey sKey = OpenSubKey(subKey);
            if (sKey == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>   
        /// 判断注册表项是否存在  
        /// 虚方法，子类可进行重写  
        /// 例子：如regDomain是HKEY_CLASSES_ROOT，则将判断HKEY_CLASSES_ROOT\\SubKey注册表项是否存在   
        /// </summary>  
        /// <param name="regDomain">注册表基项域</param>   
        /// <returns>返回注册表项是否存在，存在返回true，否则返回false</returns>  
        public virtual bool IsSubKeyExist(RegDomain regDomain)
        {
            //判断注册表项名称是否为空，如果为空，返回false   
            if (string.IsNullOrEmpty(SubKey))
            {
                return false;
            }

            //检索注册表子项   
            //如果sKey为null,说明没有该注册表项不存在，否则存在   
            RegistryKey sKey = OpenSubKey(SubKey, regDomain);
            if (sKey == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>  
        /// 判断注册表项是否存在（请先设置SubKey属性）   
        /// 虚方法，子类可进行重写   
        /// 例子：如regDomain是HKEY_CLASSES_ROOT，subkey是software\\higame\\，则将判断HKEY_CLASSES_ROOT\\software\\higame\\注册表项是否存在  
        /// </summary>   
        /// <param name="subKey">注册表项名称</param>  
        /// <param name="regDomain">注册表基项域</param>   
        /// <returns>返回注册表项是否存在，存在返回true，否则返回false</returns>  
        public virtual bool IsSubKeyExist(string subKey, RegDomain regDomain)
        {
            //判断注册表项名称是否为空，如果为空，返回false   
            if (string.IsNullOrEmpty(subKey))
            {
                return false;
            }

            //检索注册表子项   
            //如果sKey为null,说明没有该注册表项不存在，否则存在   
            RegistryKey sKey = OpenSubKey(subKey, regDomain);
            if (sKey == null)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 删除注册表项   
        /// <summary>   
        /// 删除注册表项（请先设置SubKey属性）  
        /// 虚方法，子类可进行重写   
        /// </summary>   
        /// <returns>如果删除成功，则返回true，否则为false</returns>   
        public virtual bool DeleteSubKey()
        {
            //返回删除是否成功   
            bool result = false;

            //判断注册表项名称是否为空，如果为空，返回false   
            if (string.IsNullOrEmpty(SubKey))
            {
                return false;
            }

            //创建基于注册表基项的节点   
            RegistryKey key = GetRegDomain(Domain);

            if (IsSubKeyExist())
            {
                try
                {
                    //删除注册表  
                    key.DeleteSubKey(SubKey);
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }
            //关闭对注册表项的更改   
            key.Close();
            return result;
        }

        /// <summary>   
        /// 删除注册表项（请先设置SubKey属性）   
        /// 虚方法，子类可进行重写  
        /// </summary>   
        /// <param name="subKey">注册表项名称</param>   
        /// <returns>如果删除成功，则返回true，否则为false</returns>  
        public virtual bool DeleteSubKey(string subKey)
        {
            //返回删除是否成功   
            bool result = false;

            //判断注册表项名称是否为空，如果为空，返回false  
            if (string.IsNullOrEmpty(subKey))
            {
                return false;
            }

            //创建基于注册表基项的节点   
            RegistryKey key = GetRegDomain(Domain);

            if (IsSubKeyExist())
            {
                try
                {
                    //删除注册表项  
                    key.DeleteSubKey(subKey);
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }
            //关闭对注册表项的更改  
            key.Close();
            return result;
        }

        /// <summary>  
        /// 删除注册表项   
        /// 虚方法，子类可进行重写  
        /// </summary>   
        /// <param name="subKey">注册表项名称</param>  
        /// <param name="regDomain">注册表基项域</param>   
        /// <returns>如果删除成功，则返回true，否则为false</returns>  
        public virtual bool DeleteSubKey(string subKey, RegDomain regDomain)
        {
            //返回删除是否成功   
            bool result = false;

            //判断注册表项名称是否为空，如果为空，返回false   
            if (string.IsNullOrEmpty(subKey))
            {
                return false;
            }

            //创建基于注册表基项的节点                
            RegistryKey key = GetRegDomain(regDomain);

            if (IsSubKeyExist(subKey, regDomain))
            {
                try
                {
                    //删除注册表项   
                    key.DeleteSubKey(subKey);
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }
            //关闭对注册表项的更改   
            key.Close();
            return result;
        }
        #endregion

        #region 判断键值是否存在   
        /// <summary>  
        /// 判断键值是否存在（请先设置SubKey和RegeditKey属性）   
        /// 虚方法，子类可进行重写   
        /// .如果RegeditKey为空、null，则返回false   
        /// .如果SubKey为空、null或者SubKey指定的注册表项不存在，返回false   
        /// </summary>  
        /// <returns>返回键值是否存在，存在返回true，否则返回false</returns>  
        public virtual bool IsRegeditKeyExist()
        {
            //返回结果  
            bool result = false;

            //判断是否设置键值属性   
            if (string.IsNullOrEmpty(RegeditKey))
            {
                return false;
            }

            //判断注册表项是否存在  
            if (IsSubKeyExist())
            {
                //打开注册表项   
                RegistryKey key = OpenSubKey();
                //键值集合   
                //获取键值集合  
                var regeditKeyNames = key.GetValueNames();
                //遍历键值集合，如果存在键值，则退出遍历   
                foreach (string regeditKey in regeditKeyNames)
                {
                    if (String.Compare(regeditKey, RegeditKey, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        result = true;
                        break;
                    }
                }
                //关闭对注册表项的更改   
                key.Close();
            }
            return result;
        }

        /// <summary>   
        /// 判断键值是否存在（请先设置SubKey属性）   
        /// 虚方法，子类可进行重写   
        /// 如果SubKey为空、null或者SubKey指定的注册表项不存在，返回false   
        /// </summary>   
        /// <param name="name">键值名称</param>   
        /// <returns>返回键值是否存在，存在返回true，否则返回false</returns>  
        public virtual bool IsRegeditKeyExist(string name)
        {
            //返回结果   
            bool result = false;

            //判断是否设置键值属性   
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            //判断注册表项是否存在  
            if (IsSubKeyExist())
            {
                //打开注册表项   
                RegistryKey key = OpenSubKey();
                //键值集合   
                //获取键值集合   
                var regeditKeyNames = key.GetValueNames();
                //遍历键值集合，如果存在键值，则退出遍历  
                foreach (string regeditKey in regeditKeyNames)
                {
                    if (String.Compare(regeditKey, name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        result = true;
                        break;
                    }
                }
                //关闭对注册表项的更改   
                key.Close();
            }
            return result;
        }

        /// <summary>   
        /// 判断键值是否存在   
        /// 虚方法，子类可进行重写   
        /// </summary>   
        /// <param name="name">键值名称</param>  
        /// <param name="subKey">注册表项名称</param>  
        /// <returns>返回键值是否存在，存在返回true，否则返回false</returns>  
        public virtual bool IsRegeditKeyExist(string name, string subKey)
        {
            //返回结果  
            bool result = false;

            //判断是否设置键值属性   
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            //判断注册表项是否存在   
            if (IsSubKeyExist())
            {
                //打开注册表项   
                RegistryKey key = OpenSubKey(subKey);
                //键值集合  
                //获取键值集合   
                var regeditKeyNames = key.GetValueNames();
                //遍历键值集合，如果存在键值，则退出遍历   
                foreach (string regeditKey in regeditKeyNames)
                {
                    if (String.Compare(regeditKey, name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        result = true;
                        break;
                    }
                }
                //关闭对注册表项的更改   
                key.Close();
            }
            return result;
        }

        /// <summary>   
        /// 判断键值是否存在   
        /// 虚方法，子类可进行重写   
        /// </summary>   
        /// <param name="name">键值名称</param>   
        /// <param name="subKey">注册表项名称</param>   
        /// <param name="regDomain">注册表基项域</param>   
        /// <returns>返回键值是否存在，存在返回true，否则返回false</returns>   
        public virtual bool IsRegeditKeyExist(string name, string subKey, RegDomain regDomain)
        {
            //返回结果   
            bool result = false;

            //判断是否设置键值属性   
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            //判断注册表项是否存在  
            if (IsSubKeyExist())
            {
                //打开注册表项  
                RegistryKey key = OpenSubKey(subKey, regDomain);
                //键值集合   
                //获取键值集合   
                var regeditKeyNames = key.GetValueNames();
                //遍历键值集合，如果存在键值，则退出遍历   
                foreach (string regeditKey in regeditKeyNames)
                {
                    if (String.Compare(regeditKey, name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        result = true;
                        break;
                    }
                }
                //关闭对注册表项的更改  
                key.Close();
            }
            return result;
        }
        #endregion

        #region 设置键值内容   
        /// <summary>   
        /// 设置指定的键值内容，不指定内容数据类型（请先设置RegeditKey和SubKey属性）   
        /// 存在改键值则修改键值内容，不存在键值则先创建键值，再设置键值内容   
        /// </summary>   
        /// <param name="content">键值内容</param>   
        /// <returns>键值内容设置成功，则返回true，否则返回false</returns>   
        public virtual bool WriteRegeditKey(object content)
        {
            //返回结果   
            bool result;

            //判断是否设置键值属性   
            if (string.IsNullOrEmpty(RegeditKey))
            {
                return false;
            }

            //判断注册表项是否存在，如果不存在，则直接创建   
            if (!IsSubKeyExist(SubKey))
            {
                CreateSubKey(SubKey);
            }

            //以可写方式打开注册表项  
            RegistryKey key = OpenSubKey(true);

            //如果注册表项打开失败，则返回false   
            if (key == null)
            {
                return false;
            }

            try
            {
                key.SetValue(RegeditKey, content);
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                //关闭对注册表项的更改   
                key.Close();
            }
            return result;
        }

        /// <summary>   
        /// 设置指定的键值内容，不指定内容数据类型（请先设置SubKey属性）   
        /// 存在改键值则修改键值内容，不存在键值则先创建键值，再设置键值内容   
        /// </summary>   
        /// <param name="name">键值名称</param>   
        /// <param name="content">键值内容</param>   
        /// <returns>键值内容设置成功，则返回true，否则返回false</returns>   
        public virtual bool WriteRegeditKey(string name, object content)
        {
            //返回结果   
            bool result;

            //判断键值是否存在   
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            //判断注册表项是否存在，如果不存在，则直接创建   
            if (!IsSubKeyExist(SubKey))
            {
                CreateSubKey(SubKey);
            }

            //以可写方式打开注册表项   
            RegistryKey key = OpenSubKey(true);

            //如果注册表项打开失败，则返回false   
            if (key == null)
            {
                return false;
            }

            try
            {
                key.SetValue(name, content);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                //关闭对注册表项的更改   
                key.Close();
            }
            return result;
        }

        /// <summary>   
        /// 设置指定的键值内容，指定内容数据类型（请先设置SubKey属性）  
        /// 存在改键值则修改键值内容，不存在键值则先创建键值，再设置键值内容   
        /// </summary>  
        /// <param name="name">键值名称</param>   
        /// <param name="content">键值内容</param>  
        /// <returns>键值内容设置成功，则返回true，否则返回false</returns>   
        public virtual bool WriteRegeditKey(string name, object content, RegValueKind regValueKind)
        {
            //返回结果  
            bool result;

            //判断键值是否存在   
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            //判断注册表项是否存在，如果不存在，则直接创建   
            if (!IsSubKeyExist(SubKey))
            {
                CreateSubKey(SubKey);
            }

            //以可写方式打开注册表项   
            RegistryKey key = OpenSubKey(true);

            //如果注册表项打开失败，则返回false   
            if (key == null)
            {
                return false;
            }

            try
            {
                key.SetValue(name, content, GetRegValueKind(regValueKind));
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                //关闭对注册表项的更改   
                key.Close();
            }
            return result;
        }
        #endregion

        #region 读取键值内容   
        /// <summary>   
        /// 读取键值内容（请先设置RegeditKey和SubKey属性）   
        /// .如果RegeditKey为空、null或者RegeditKey指示的键值不存在，返回null  
        /// .如果SubKey为空、null或者SubKey指示的注册表项不存在，返回null  
        /// .反之，则返回键值内容  
        /// </summary>   
        /// <returns>返回键值内容</returns>   
        public virtual object ReadRegeditKey()
        {
            //键值内容结果   
            object obj = null;

            //判断是否设置键值属性   
            if (string.IsNullOrEmpty(RegeditKey))
            {
                return null;
            }

            //判断键值是否存在   
            if (IsRegeditKeyExist(RegeditKey))
            {
                //打开注册表项  
                RegistryKey key = OpenSubKey();
                if (key != null)
                {
                    obj = key.GetValue(RegeditKey);
                }
                //关闭对注册表项的更改   
                key.Close();
            }
            return obj;
        }

        /// <summary>   
        /// 读取键值内容（请先设置SubKey属性）  
        /// .如果SubKey为空、null或者SubKey指示的注册表项不存在，返回null   
        /// .反之，则返回键值内容   
        /// </summary>   
        /// <param name="name">键值名称</param>  
        /// <returns>返回键值内容</returns>  
        public virtual object ReadRegeditKey(string name)
        {
            //键值内容结果   
            object obj = null;

            //判断是否设置键值属性   
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            //判断键值是否存在   
            if (IsRegeditKeyExist(name))
            {
                //打开注册表项  
                RegistryKey key = OpenSubKey();
                if (key != null)
                {
                    obj = key.GetValue(name);
                }
                //关闭对注册表项的更改  
                key.Close();
            }
            return obj;
        }

        /// <summary>   
        /// 读取键值内容  
        /// </summary>  
        /// <param name="name">键值名称</param>  
        /// <param name="subKey">注册表项名称</param>  
        /// <returns>返回键值内容</returns>   
        public virtual object ReadRegeditKey(string name, string subKey)
        {
            //键值内容结果   
            object obj = null;

            //判断是否设置键值属性  
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            //判断键值是否存在   
            if (IsRegeditKeyExist(name))
            {
                //打开注册表项  
                RegistryKey key = OpenSubKey(subKey);
                if (key != null)
                {
                    obj = key.GetValue(name);
                }
                //关闭对注册表项的更改  
                key.Close();
            }
            return obj;
        }

        /// <summary>   
        /// 读取键值内容  
        /// </summary>  
        /// <param name="name">键值名称</param>  
        /// <param name="subKey">注册表项名称</param>   
        /// <param name="regDomain">注册表基项域</param>   
        /// <returns>返回键值内容</returns>   
        public virtual object ReadRegeditKey(string name, string subKey, RegDomain regDomain)
        {
            //键值内容结果  
            object obj = null;

            //判断是否设置键值属性   
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            //判断键值是否存在   
            if (IsRegeditKeyExist(name))
            {
                //打开注册表项   
                RegistryKey key = OpenSubKey(subKey, regDomain);
                if (key != null)
                {
                    obj = key.GetValue(name);
                }
                //关闭对注册表项的更改   
                key.Close();
            }
            return obj;
        }
        #endregion

        #region 删除键值   
        /// <summary>   
        /// 删除键值（请先设置RegeditKey和SubKey属性）  
        /// .如果RegeditKey为空、null或者RegeditKey指示的键值不存在，返回false   
        /// .如果SubKey为空、null或者SubKey指示的注册表项不存在，返回false   
        /// </summary>  
        /// <returns>如果删除成功，返回true，否则返回false</returns>   
        public virtual bool DeleteRegeditKey()
        {
            //删除结果  
            bool result = false;

            //判断是否设置键值属性，如果没有设置，则返回false   
            if (string.IsNullOrEmpty(RegeditKey))
            {
                return false;
            }

            //判断键值是否存在   
            if (IsRegeditKeyExist(RegeditKey))
            {
                //以可写方式打开注册表项  
                RegistryKey key = OpenSubKey(true);
                if (key != null)
                {
                    try
                    {
                        //删除键值  
                        key.DeleteValue(RegeditKey);
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                    finally
                    {
                        //关闭对注册表项的更改   
                        key.Close();
                    }
                }
            }

            return result;
        }

        /// <summary>   
        /// 删除键值（请先设置SubKey属性）   
        /// 如果SubKey为空、null或者SubKey指示的注册表项不存在，返回false   
        /// </summary>   
        /// <param name="name">键值名称</param>   
        /// <returns>如果删除成功，返回true，否则返回false</returns>   
        public virtual bool DeleteRegeditKey(string name)
        {
            //删除结果   
            bool result = false;

            //判断键值名称是否为空，如果为空，则返回false   
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            //判断键值是否存在   
            if (IsRegeditKeyExist(name))
            {
                //以可写方式打开注册表项   
                RegistryKey key = OpenSubKey(true);
                if (key != null)
                {
                    try
                    {
                        //删除键值   
                        key.DeleteValue(name);
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                    finally
                    {
                        //关闭对注册表项的更改  
                        key.Close();
                    }
                }
            }

            return result;
        }

        /// <summary>  
        /// 删除键值   
        /// </summary>   
        /// <param name="name">键值名称</param>  
        /// <param name="subKey">注册表项名称</param>   
        /// <returns>如果删除成功，返回true，否则返回false</returns>  
        public virtual bool DeleteRegeditKey(string name, string subKey)
        {
            //删除结果   
            bool result = false;

            //判断键值名称和注册表项名称是否为空，如果为空，则返回false  
            if (string.IsNullOrEmpty(name) || subKey == string.Empty || subKey == null)
            {
                return false;
            }

            //判断键值是否存在   
            if (IsRegeditKeyExist(name))
            {
                //以可写方式打开注册表项   
                RegistryKey key = OpenSubKey(subKey, true);
                if (key != null)
                {
                    try
                    {
                        //删除键值  
                        key.DeleteValue(name);
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                    finally
                    {
                        //关闭对注册表项的更改   
                        key.Close();
                    }
                }
            }

            return result;
        }

        /// <summary>  
        /// 删除键值  
        /// </summary>  
        /// <param name="name">键值名称</param>  
        /// <param name="subKey">注册表项名称</param>  
        /// <param name="regDomain">注册表基项域</param>  
        /// <returns>如果删除成功，返回true，否则返回false</returns>  
        public virtual bool DeleteRegeditKey(string name, string subKey, RegDomain regDomain)
        {
            //删除结果  
            bool result = false;

            //判断键值名称和注册表项名称是否为空，如果为空，则返回false  
            if (string.IsNullOrEmpty(name) || subKey == string.Empty || subKey == null)
            {
                return false;
            }

            //判断键值是否存在  
            if (IsRegeditKeyExist(name))
            {
                //以可写方式打开注册表项  
                RegistryKey key = OpenSubKey(subKey, regDomain, true);
                if (key != null)
                {
                    try
                    {
                        //删除键值  
                        key.DeleteValue(name);
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                    finally
                    {
                        //关闭对注册表项的更改  
                        key.Close();
                    }
                }
            }

            return result;
        }
        #endregion
        #endregion

        #region 受保护方法  
        /// <summary>  
        /// 获取注册表基项域对应顶级节点  
        /// 例子：如regDomain是ClassesRoot，则返回Registry.ClassesRoot  
        /// </summary>  
        /// <param name="regDomain">注册表基项域</param>  
        /// <returns>注册表基项域对应顶级节点</returns>  
        protected RegistryKey GetRegDomain(RegDomain regDomain)
        {
            //创建基于注册表基项的节点  
            RegistryKey key;

            #region 判断注册表基项域  
            switch (regDomain)
            {
                case RegDomain.ClassesRoot:
                    key = Microsoft.Win32.Registry.ClassesRoot;
                    break;
                case RegDomain.CurrentUser:
                    key = Microsoft.Win32.Registry.CurrentUser;
                    break;
                case RegDomain.LocalMachine:
                    key = Microsoft.Win32.Registry.LocalMachine;
                    break;
                case RegDomain.User:
                    key = Microsoft.Win32.Registry.Users;
                    break;
                case RegDomain.CurrentConfig:
                    key = Microsoft.Win32.Registry.CurrentConfig;
                    break;
                case RegDomain.DynDa:
                    key = Microsoft.Win32.Registry.DynData;
                    break;
                case RegDomain.PerformanceData:
                    key = Microsoft.Win32.Registry.PerformanceData;
                    break;
                default:
                    key = Microsoft.Win32.Registry.LocalMachine;
                    break;
            }
            #endregion

            return key;
        }

        /// <summary>  
        /// 获取在注册表中对应的值数据类型  
        /// 例子：如regValueKind是DWord，则返回RegistryValueKind.DWord  
        /// </summary>  
        /// <param name="regValueKind">注册表数据类型</param>  
        /// <returns>注册表中对应的数据类型</returns>  
        protected RegistryValueKind GetRegValueKind(RegValueKind regValueKind)
        {
            RegistryValueKind regValueK;

            #region 判断注册表数据类型  
            switch (regValueKind)
            {
                case RegValueKind.Unknown:
                    regValueK = RegistryValueKind.Unknown;
                    break;
                case RegValueKind.String:
                    regValueK = RegistryValueKind.String;
                    break;
                case RegValueKind.ExpandString:
                    regValueK = RegistryValueKind.ExpandString;
                    break;
                case RegValueKind.Binary:
                    regValueK = RegistryValueKind.Binary;
                    break;
                case RegValueKind.DWord:
                    regValueK = RegistryValueKind.DWord;
                    break;
                case RegValueKind.MultiString:
                    regValueK = RegistryValueKind.MultiString;
                    break;
                case RegValueKind.QWord:
                    regValueK = RegistryValueKind.QWord;
                    break;
                default:
                    regValueK = RegistryValueKind.String;
                    break;
            }
            #endregion
            return regValueK;
        }

        #region 打开注册表项  
        /// <summary>  
        /// 打开注册表项节点，以只读方式检索子项  
        /// 虚方法，子类可进行重写  
        /// </summary>  
        /// <returns>如果SubKey为空、null或者SubKey指示注册表项不存在，则返回null，否则返回注册表节点</returns>  
        protected virtual RegistryKey OpenSubKey()
        {
            //判断注册表项名称是否为空  
            if (string.IsNullOrEmpty(SubKey))
            {
                return null;
            }

            //创建基于注册表基项的节点  
            RegistryKey key = GetRegDomain(Domain);

            //要打开的注册表项的节点  
            //打开注册表项  
            var sKey = key.OpenSubKey(SubKey);
            //关闭对注册表项的更改  
            key.Close();
            //返回注册表节点  
            return sKey;
        }

        /// <summary>  
        /// 打开注册表项节点  
        /// 虚方法，子类可进行重写  
        /// </summary>  
        /// <param name="writable">如果需要项的写访问权限，则设置为 true</param>  
        /// <returns>如果SubKey为空、null或者SubKey指示注册表项不存在，则返回null，否则返回注册表节点</returns>  
        protected virtual RegistryKey OpenSubKey(bool writable)
        {
            //判断注册表项名称是否为空  
            if (string.IsNullOrEmpty(SubKey))
            {
                return null;
            }

            //创建基于注册表基项的节点  
            RegistryKey key = GetRegDomain(Domain);

            //要打开的注册表项的节点  
            RegistryKey sKey = null;
            //打开注册表项  
            sKey = key.OpenSubKey(SubKey, writable);
            //关闭对注册表项的更改  
            key.Close();
            //返回注册表节点  
            return sKey;
        }

        /// <summary>  
        /// 打开注册表项节点，以只读方式检索子项  
        /// 虚方法，子类可进行重写  
        /// </summary>  
        /// <param name="subKey">注册表项名称</param>  
        /// <returns>如果SubKey为空、null或者SubKey指示注册表项不存在，则返回null，否则返回注册表节点</returns>  
        protected virtual RegistryKey OpenSubKey(string subKey)
        {
            //判断注册表项名称是否为空  
            if (string.IsNullOrEmpty(subKey))
            {
                return null;
            }

            //创建基于注册表基项的节点  
            RegistryKey key = GetRegDomain(Domain);

            //要打开的注册表项的节点  
            RegistryKey sKey = null;
            //打开注册表项  
            sKey = key.OpenSubKey(subKey);
            //关闭对注册表项的更改  
            key.Close();
            //返回注册表节点  
            return sKey;
        }

        /// <summary>  
        /// 打开注册表项节点，以只读方式检索子项  
        /// 虚方法，子类可进行重写  
        /// </summary>  
        /// <param name="subKey">注册表项名称</param>  
        /// <param name="writable">如果需要项的写访问权限，则设置为 true</param>  
        /// <returns>如果SubKey为空、null或者SubKey指示注册表项不存在，则返回null，否则返回注册表节点</returns>  
        protected virtual RegistryKey OpenSubKey(string subKey, bool writable)
        {
            //判断注册表项名称是否为空  
            if (string.IsNullOrEmpty(subKey))
            {
                return null;
            }

            //创建基于注册表基项的节点  
            RegistryKey key = GetRegDomain(Domain);

            //要打开的注册表项的节点  
            RegistryKey sKey = null;
            //打开注册表项  
            sKey = key.OpenSubKey(subKey, writable);
            //关闭对注册表项的更改  
            key.Close();
            //返回注册表节点  
            return sKey;
        }

        /// <summary>  
        /// 打开注册表项节点，以只读方式检索子项  
        /// 虚方法，子类可进行重写  
        /// </summary>  
        /// <param name="subKey">注册表项名称</param>  
        /// <param name="regDomain">注册表基项域</param>  
        /// <returns>如果SubKey为空、null或者SubKey指示注册表项不存在，则返回null，否则返回注册表节点</returns>  
        protected virtual RegistryKey OpenSubKey(string subKey, RegDomain regDomain)
        {
            //判断注册表项名称是否为空  
            if (string.IsNullOrEmpty(subKey))
            {
                return null;
            }

            //创建基于注册表基项的节点  
            RegistryKey key = GetRegDomain(regDomain);

            //要打开的注册表项的节点  
            RegistryKey sKey = null;
            //打开注册表项  
            sKey = key.OpenSubKey(subKey);
            //关闭对注册表项的更改  
            key.Close();
            //返回注册表节点  
            return sKey;
        }

        /// <summary>  
        /// 打开注册表项节点  
        /// 虚方法，子类可进行重写  
        /// </summary>  
        /// <param name="subKey">注册表项名称</param>  
        /// <param name="regDomain">注册表基项域</param>  
        /// <param name="writable">如果需要项的写访问权限，则设置为 true</param>  
        /// <returns>如果SubKey为空、null或者SubKey指示注册表项不存在，则返回null，否则返回注册表节点</returns>  
        protected virtual RegistryKey OpenSubKey(string subKey, RegDomain regDomain, bool writable)
        {
            //判断注册表项名称是否为空  
            if (string.IsNullOrEmpty(subKey))
            {
                return null;
            }

            //创建基于注册表基项的节点  
            RegistryKey key = GetRegDomain(regDomain);

            //要打开的注册表项的节点  
            RegistryKey sKey = null;
            //打开注册表项  
            sKey = key.OpenSubKey(subKey, writable);
            //关闭对注册表项的更改  
            key.Close();
            //返回注册表节点  
            return sKey;
        }
        #endregion
        #endregion
    }  
}
