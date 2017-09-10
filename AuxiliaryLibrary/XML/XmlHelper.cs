using System;
using System.IO;
using System.Xml;

namespace AuxiliaryLibrary.XML
{
    public class XmlHelper
    {

        #region 字段
        /// <summary>
        /// xml文件物理路径
        /// </summary>
        private readonly string _filePath = string.Empty;

        /// <summary>
        /// xml文档
        /// </summary>
        private XmlDocument _xml;

        /// <summary>
        /// xml文档根节点
        /// </summary>
        private XmlElement _element;

        #endregion
        public XmlHelper()
        {
            
        }

        /// <summary>
        /// 给xml文档路径赋值
        /// </summary>
        /// <param name="xmlFilePath"></param>

        public XmlHelper(string xmlFilePath)
        {
            _filePath = xmlFilePath;
        }

        /// <summary>
        /// 获取指定路径节点
        /// </summary>
        /// <param name="xPath"></param>
        /// <returns></returns>

        public static XmlNode GetXmlNode(string xmlFileName, string xPath)
        {
            XmlDocument xmldocument = new XmlDocument();
            //加载xml文档
            xmldocument.Load(xmlFileName);
            try
            {
                XmlNode xmlnode = xmldocument.SelectSingleNode(xPath);
                return xmlnode;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定路径节点下孩子节点列表
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>

        public static XmlNodeList GetXmlNodeList(string xmlFileName, string xPath)

        {
            XmlDocument xmldocument = new XmlDocument();
            //加载xml文档
            xmldocument.Load(xmlFileName);
            try
            {
                XmlNodeList xmlnodelist = xmldocument.SelectNodes(xPath);
                return xmlnodelist;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定路径节点的属性与指定属性名匹配
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xPath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
        /// <param name="attributeName">指定的属性名称</param>
        /// <returns></returns>

        public static XmlAttribute GetXmlAttribute(string xmlFileName, string xPath, string attributeName)
        {
            XmlAttribute xmlattribute = null;
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(xmlFileName);
            try
            {
                XmlNode xmlnode = xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    if (xmlnode.Attributes.Count > 0)
                    {
                        xmlattribute = xmlnode.Attributes[attributeName];
                    }
                }
            }
            catch (Exception err)
            {
                throw err;
            }
            return xmlattribute;
        }



        /// <summary>
        /// 获取指定节点的属性集合
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>

        public static XmlAttributeCollection GetNodeAttributes(string xmlFileName, string xPath)

        {
            XmlAttributeCollection xmlattributes = null;
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(xmlFileName);
            try
            {
                XmlNode xmlnode = xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    if (xmlnode.Attributes.Count > 0)
                    {
                        xmlattributes = xmlnode.Attributes;
                    }
                }
            }
            catch (Exception err)
            {
                throw err;
            }
            return xmlattributes;
        }

        /// <summary>
        /// 更新指定节点的某一属性设定其属性值value
        /// </summary>
        /// <param name="xmlFileName">xml文档路径</param>
        /// <param name="xPath"></param>
        /// <param name="attributeOldeName">旧属性名称</param>
        /// <param name="attributeNewName">新属性名称</param>
        /// <param name="value">属性值</param>
        /// <returns>成功返回true,失败返回false</returns>

        public static bool UpdateAttribute(string xmlFileName, string xPath, string attributeName, string value)
        {
            bool isSuccess = false;
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(xmlFileName);
            try
            {
                XmlNode xmlnode = xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    foreach (XmlAttribute attribute in xmlnode.Attributes)
                    {
                        if (attribute.Name.ToLower() == attributeName.ToLower())
                        {
                            isSuccess = true;
                            attribute.Value = value;
                            xmldocument.Save(xmlFileName);
                            break;
                        }

                    }

                }

            }
            catch (Exception err)
            {
                throw err;
            }
            return isSuccess;
        }



        /// <summary>
        /// 删除指定节点的所有属性
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xPath"></param>
        /// <returns>成功返回true,失败返回false</returns>

        public static bool DeleteAttributes(string xmlFileName, string xPath)

        {
            bool isSuccess = false;
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(xmlFileName);

            try
            {
                XmlNode xmlnode = xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    if (xmlnode.Attributes.Count > 0)
                    {
                        xmlnode.Attributes.RemoveAll();
                        xmldocument.Save(xmlFileName);
                        isSuccess = true;
                    }

                }

            }
            catch (Exception err)
            {
                throw err;
            }
            return isSuccess;

        }



        /// <summary>
        /// 删除匹配属性名称的指定节点的属性
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xPath"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>

        public static bool DeleteOneAttribute(string xmlFileName, string xPath, string attributeName)

        {
            bool isSuccess = false;
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(xmlFileName);
            XmlAttribute xmlAttribute = null;
            try
            {
                XmlNode xmlnode = xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    if (xmlnode.Attributes.Count > 0)
                    {
                        foreach (XmlAttribute attribute in xmlnode.Attributes)
                        {
                            if (attribute.Name.ToLower() == attributeName.ToLower())
                            {
                                xmlAttribute = attribute;
                                break;
                            }

                        }

                    }

                    if (xmlAttribute != null)
                    {
                        xmlnode.Attributes.Remove(xmlAttribute);
                        xmldocument.Save(xmlFileName);
                        isSuccess = true;
                    }

                }

            }
            catch (Exception err)
            {
                throw err;
            }
            return isSuccess;
        }



        /// <summary>
        /// 创建指定节点的属性,如果属性存在则不创建
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xPath"></param>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>

        public static bool AddAttribute(string xmlFileName, string xPath, string attributeName, string value)
        {
            bool isSuccess = false;
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(xmlFileName);
            try
            {
                XmlNode xmlnode = xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    if (xmlnode.Attributes.Count > 0)//遍历判断有无此属性
                    {
                        foreach (XmlAttribute attribute in xmlnode.Attributes)
                        {
                            if (attribute.Name.ToLower() == attributeName.ToLower())
                            {

                                //有则不改，直接返回true;
                                return true;

                            }

                        }

                    }

                    XmlAttribute xmlAttribute = xmldocument.CreateAttribute(attributeName);
                    xmlAttribute.Value = value;
                    xmlnode.Attributes.Append(xmlAttribute);
                    xmldocument.Save(xmlFileName);
                    isSuccess = true;

                }

            }
            catch (Exception err)
            {
                throw err;
            }
            return isSuccess;

        }



        /// <summary>
        /// 为某一指定路径节点下添加新的节点，如果该节点存在，则不添加
        /// </summary>
        /// <param name="xmlFileName">xml文档路径</param>
        /// <param name="xPath">需要添加节点的路径</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="innerText">节点文本值</param>
        /// <returns>成功返回true,存在返回false</returns>
        public static bool AddNode(string xmlFileName, string xPath, string nodeName, string innerText)

        {
            bool isSuccess = false;
            bool isExisitNode = false;
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(xmlFileName);
            try
            {
                XmlNode xmlnode = xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    isExisitNode = true;
                }

                if (!isExisitNode)
                {
                    XmlElement subElement = xmldocument.CreateElement(nodeName);
                    subElement.InnerText = innerText;
                    xmlnode.AppendChild(subElement);
                    isSuccess = true;
                    xmldocument.Save(xmlFileName);
                }
            }
            catch (Exception err)
            {
                throw err;
            }
            return isSuccess;

        }



        /// <summary>
        /// 查找指定的节点，更新其节点值
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xPath"></param>
        /// <param name="nodeName"></param>
        /// <param name="innerText"></param>
        /// <returns></returns>

        public static bool UpdateNode(string xmlFileName, string xPath, string nodeName, string innerText)

        {
            bool isSuccess = false;
            bool isExisitNode = false;
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(xmlFileName);
            XmlNode xmlnode = xmldocument.SelectSingleNode(xPath);
            try
            {
                if (xmlnode != null)
                {
                    isExisitNode = true;
                }

                if (!isExisitNode)
                {
                    xmlnode.InnerText = innerText;
                    isSuccess = true;
                    xmldocument.Save(xmlFileName);
                }
            }
            catch (Exception err)
            {
                throw err;
            }
            return isSuccess;

        }



        /// <summary>
        /// 删除指定节点名称为nodeName的所有节点，如果该节点有子节点，则不能删除
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xPath"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>

        public static bool deleteNode(string xmlFileName, string xPath, string nodeName)
        {
            bool isSuccess = false;
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(xmlFileName);
            try
            {
                XmlNode xmlnode = xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    if (xmlnode.HasChildNodes)
                    {
                        isSuccess = false;

                    }
                    else
                    {
                        xmlnode.ParentNode.RemoveChild(xmlnode);//删除节点
                        isSuccess = true;
                        xmldocument.Save(xmlFileName);
                    }

                }

            }
            catch (Exception err)
            {
                throw err;
            }
            return isSuccess;

        }

        /// <summary>
        /// 根据指定节点名称更新其下指定的子节点的值
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xPath"></param>
        /// <param name="nodeName"></param>
        /// <param name="innerText"></param>
        /// <returns></returns>

        public static bool UpdateChildNode(string xmlFileName, string xPath, string nodeName, string childName, string innerText)
        {
            bool isSuccess = false;
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(xmlFileName);
            try
            {
                XmlNode xmlnode = xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    foreach (XmlNode node in xmlnode.ChildNodes)
                    {
                        if (node.Name.ToLower() == childName.ToLower())
                        {
                            node.InnerText = innerText;
                            xmldocument.Save(xmlFileName);
                            isSuccess = true;
                        }

                    }

                }

            }
            catch (Exception err)
            {
                throw err;
            }
            return isSuccess;

        }



        #region 创建XML的根节点

        /// <summary>
        /// 创建XML的根节点
        /// </summary>

        private void CreateXMLElement()
        {
            //创建一个XML对象
            _xml = new XmlDocument();
            if (File.Exists(_filePath))
            {
                //加载XML文件
                _xml.Load(this._filePath);
            }

            //为XML的根节点赋值
            _element = _xml.DocumentElement;
        }

        #endregion


        #region 保存XML文件

        /// <summary>
        /// 保存XML文件
        /// </summary>        

        public void Save()
        {
            //创建XML的根节点
            //CreateXMLElement();
            //保存XML文件
            _xml.Save(this._filePath);

        }

        #endregion //保存XML文件


        #region XML文档创建和节点或属性的添加、修改

        /// <summary>
        /// 创建一个XML文档
        /// </summary>
        /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        /// <param name="rootNodeName">XML文档根节点名称(须指定一个根节点名称)</param>
        /// <param name="version">XML文档版本号(必须为:"1.0")</param>
        /// <param name="encoding">XML文档编码方式</param>
        /// <param name="standalone">该值必须是"yes"或"no",如果为null,Save方法不在XML声明上写出独立属性</param>
        /// <returns>成功返回true,失败返回false</returns>

        public static bool CreateXmlDocument(string xmlFileName, string rootNodeName, string version, string encoding, string standalone)
        {
            bool isSuccess = false;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration(version, encoding, standalone);
                XmlNode root = xmlDoc.CreateElement(rootNodeName);
                xmlDoc.AppendChild(xmlDeclaration);
                xmlDoc.AppendChild(root);
                xmlDoc.Save(xmlFileName);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                throw ex; //这里可以定义你自己的异常处理
            }
            return isSuccess;
        }

        #endregion

    }

}