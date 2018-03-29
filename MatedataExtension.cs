using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.XPath;

namespace ModelXmlReader
{
    /// <summary>
    /// 扩展反射出的元数据能够获取Xml内的注释
    /// </summary>
    public static class MetadataXmlCommentExtension
    {
        private const string MemberXPath = "/doc/members/member[@name='{0}']";
        private const string AssemblyXPath = "/doc/assembly/name";
        private const string SummaryTag = "summary";
        private const string ParamXPath = "param[@name='{0}']";

        private static readonly Dictionary<string,XPathDocument> XPathDocuments=new Dictionary<string, XPathDocument>();

        /// <summary>
        /// 引入Xml文件
        /// </summary>
        /// <param name="xmlpath"></param>
        public static void InculudeXml(string xmlpath)
        {
            var xmldoc=new XPathDocument(xmlpath);

            var assemblyname=xmldoc.CreateNavigator().SelectSingleNode(AssemblyXPath).ExtractContent();

            XPathDocuments.Add(assemblyname,xmldoc);   
        }

        private static XPathNavigator GetXmlDoc(object target)
        {
            var asname = "";
            if (target is Type type)
            {
                asname = type.Assembly.GetName().Name;
            }
            else if(target is MethodInfo method)
            {
                asname = method.DeclaringType.Assembly.GetName().Name;
            }
            else if(target is ParameterInfo parameter)
            {
                asname = parameter.Member.DeclaringType.Assembly.GetName().Name;
            }
            else if(target is PropertyInfo propertyInfo)
            {
                asname = propertyInfo.DeclaringType.Assembly.GetName().Name;
            }

            if (XPathDocuments.ContainsKey(asname))
            {
                return XPathDocuments[asname].CreateNavigator();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取注释
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetComments(this Type type)
        {
            XPathNavigator navigator= GetXmlDoc(type)?.CreateNavigator();
            if (navigator == null)
                return string.Empty;
            var commentId = XmlCommentsIdHelper.GetCommentIdForType(type);
            var typeNode = navigator.SelectSingleNode(string.Format(MemberXPath, commentId));
            if (typeNode != null)
            {
                var summaryNode = typeNode.SelectSingleNode(SummaryTag);
                if (summaryNode != null)
                    return summaryNode.ExtractContent();
            }
            return "";
        }

        /// <summary>
        /// 获取注释
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetComments(this MethodInfo method)
        {
            XPathNavigator navigator = GetXmlDoc(method)?.CreateNavigator();
            if (navigator == null)
                return string.Empty;
            var commentId = XmlCommentsIdHelper.GetCommentIdForMethod(method);
            var node = navigator.SelectSingleNode(string.Format(MemberXPath, commentId));
            if (node != null)
            {
                var summaryNode = node.SelectSingleNode(SummaryTag);
                if (summaryNode != null)
                    return summaryNode.ExtractContent();
            }
            return "";
        }

        /// <summary>
        /// 获取注释
        /// </summary>
        /// <param name="parameterInfo"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetComments(this ParameterInfo parameterInfo, MethodInfo method)
        {
            XPathNavigator navigator = GetXmlDoc(parameterInfo)?.CreateNavigator();
            if (navigator == null)
                return string.Empty;
            var commentId = XmlCommentsIdHelper.GetCommentIdForMethod(method);
            var methodnode = navigator.SelectSingleNode(string.Format(MemberXPath, commentId));
            if (methodnode == null)
                return "";

            var parameternode=methodnode.SelectSingleNode(string.Format(ParamXPath, parameterInfo.Name));

            if (parameternode != null)
            {
                return parameternode.ExtractContent();
            }
            return "";
        }

        /// <summary>
        /// 获取注释
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static string GetComments(this PropertyInfo propertyInfo)
        {
            XPathNavigator navigator = GetXmlDoc(propertyInfo)?.CreateNavigator();
            if (navigator == null)
                return string.Empty;
            var commentId = XmlCommentsIdHelper.GetCommentIdForProperty(propertyInfo);
            var node = navigator.SelectSingleNode(string.Format(MemberXPath, commentId));
            if (node != null)
            {
                var summaryNode = node.SelectSingleNode(SummaryTag);
                if (summaryNode != null)
                    return summaryNode.ExtractContent();
            }
            return "";
        }
    }
}
