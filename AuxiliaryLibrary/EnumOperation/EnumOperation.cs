using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace AuxiliaryLibrary.EnumOperation
{
    /// <summary>  
    /// 枚举操作类  
    /// </summary>  
    public static class EnumOperation
    {
        #region 从枚举中获取Description  

        /// <summary>  
        /// 从枚举中获取Description  
        /// </summary>  
        /// <param name="enumName">需要获取枚举描述的枚举</param>  
        /// <returns>描述内容</returns>  
        public static string GetDescription(System.Enum enumName)
        {
            try
            {
                string _description = string.Empty;
                FieldInfo _fieldInfo = enumName.GetType().GetField(enumName.ToString());
                DescriptionAttribute[] _attributes = GetDescriptAttr(_fieldInfo);
                if (_attributes != null && _attributes.Length > 0)
                {
                    _description = _attributes[0].Description;
                }
                else
                {
                    _description = enumName.ToString();
                }
                return _description;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region 获取字段Description(private)  

        /// <summary>  
        /// 获取字段Description  
        /// </summary>  
        /// <param name="fieldInfo">FieldInfo</param>  
        /// <returns>DescriptionAttribute[] </returns>  
        private static DescriptionAttribute[] GetDescriptAttr(this FieldInfo fieldInfo)
        {
            try
            {
                if (fieldInfo != null)
                {
                    return (DescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region 根据Description获取枚举定义字符串  

        /// <summary>  
        /// 根据Description获取枚举定义字符串  
        /// </summary>  
        /// <typeparam name="T">枚举类型</typeparam>  
        /// <param name="description">枚举描述</param>  
        /// <returns>枚举</returns>  
        public static T GetEnumName<T>(string description)
        {
            Type _type = typeof(T);
            foreach (FieldInfo field in _type.GetFields())
            {
                DescriptionAttribute[] _curDesc = field.GetDescriptAttr();
                if (_curDesc != null && _curDesc.Length > 0)
                {
                    if (_curDesc[0].Description == description)
                    {
                        return (T) field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == description)
                    {
                        return (T) field.GetValue(null);
                    }
                }
            }
            throw new ArgumentException(string.Format("{0} 未能找到对应的枚举.", description), "Description");
        }

        #endregion

        #region 将枚举转换为ArrayList  

        /// <summary>  
        /// 将枚举转换为ArrayList  
        /// 说明：  
        /// 若不是枚举类型，则返回NULL  
        /// </summary>  
        /// <param name="type">枚举类型</param>  
        /// <returns>ArrayList</returns>  
        public static ArrayList ToArrayList(Type type)
        {
            try
            {
                if (type.IsEnum)
                {
                    ArrayList _array = new ArrayList();
                    Array _enumValues = System.Enum.GetValues(type);
                    foreach (System.Enum value in _enumValues)
                    {
                        _array.Add(new KeyValuePair<System.Enum, string>(value, GetDescription(value)));
                    }
                    return _array;
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region 根据枚举值得到属性Description中的描述, 如果没有定义此属性则返回空串  

        /// <summary>    
        /// 根据枚举值得到属性Description中的描述, 如果没有定义此属性则返回空串    
        /// </summary>    
        /// <param name="value"></param>    
        /// <param name="enumType"></param>    
        /// <returns></returns>    
        public static String GetEnumDescriptionString(int value, Type enumType)
        {
            try
            {
                NameValueCollection nvc = GetNVCFromEnumValue(enumType);
                return nvc[value.ToString()];
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region 根据枚举类型得到其所有的值 与 枚举定义Description属性 的集合  

        /// <summary>    
        /// 根据枚举类型得到其所有的 值 与 枚举定义Description属性 的集合    
        /// </summary>    
        /// <param name="enumType"></param>    
        /// <returns></returns>    
        public static NameValueCollection GetNVCFromEnumValue(Type enumType)
        {
            try
            {
                NameValueCollection nvc = new NameValueCollection();
                Type typeDescription = typeof(DescriptionAttribute);
                System.Reflection.FieldInfo[] fields = enumType.GetFields();
                string strText = string.Empty;
                string strValue = string.Empty;
                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType.IsEnum)
                    {
                        strValue = ((int) enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null))
                            .ToString();
                        object[] arr = field.GetCustomAttributes(typeDescription, true);
                        if (arr.Length > 0)
                        {
                            DescriptionAttribute aa = (DescriptionAttribute) arr[0];
                            strText = aa.Description;
                        }
                        else
                        {
                            strText = "";
                        }
                        nvc.Add(strValue, strText);
                    }
                }
                return nvc;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region 根据枚举值得到相应的枚举定义字符串  

        /// <summary>    
        ///根据枚举值得到相应的枚举定义字符串    
        /// </summary>    
        /// <param name="value"></param>    
        /// <param name="enumType"></param>    
        /// <returns></returns>    
        public static String GetEnumString(int value, Type enumType)
        {
            try
            {
                NameValueCollection nvc = GetEnumStringFromEnumValue(enumType);
                return nvc[value.ToString()];
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region 根据枚举类型得到其所有的 值 与 枚举定义字符串 的集合  

        /// <summary>    
        /// 根据枚举类型得到其所有的 值 与 枚举定义字符串 的集合    
        /// </summary>    
        /// <param name="enumType"></param>    
        /// <returns></returns>    
        public static NameValueCollection GetEnumStringFromEnumValue(Type enumType)
        {
            try
            {
                NameValueCollection nvc = new NameValueCollection();
                Type typeDescription = typeof(DescriptionAttribute);
                FieldInfo[] fields = enumType.GetFields();
                string strText = string.Empty;
                string strValue = string.Empty;
                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType.IsEnum)
                    {
                        strValue = ((int) enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null))
                            .ToString();
                        nvc.Add(strValue, field.Name);
                    }
                }
                return nvc;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion
    }
}