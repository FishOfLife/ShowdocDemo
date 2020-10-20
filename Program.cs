using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Showdoc
{

    public class Program
    {

        private static Dictionary<string, string> body = new Dictionary<string, string>();

        private static void Main(string[] args)
        {

            args = new string[] { "F:/VLabEditor-Realease/CodeRepos/GeneralAbility/VLabGeneralAbility/bin/Debug/VLabGeneralAbility.xml" };
            //1.遍历所有xml路径
            foreach (string arg in args)
            {
                if (!string.IsNullOrEmpty(arg))
                {
                    //2.加载xml
                    XmlDocument xml = new XmlDocument();
                    try
                    {
                        xml.Load(arg);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        continue;
                    }
                    //3.解析xml
                    //获取所有member节点
                    XmlNodeList classMembers = xml.SelectNodes("/doc//member[contains(@name,\"T:\")]");
                    XmlNodeList methodMembers = xml.SelectNodes("/doc//member[contains(@name,\"M:\")]");
                    XmlNodeList fieldMembers = xml.SelectNodes("/doc//member[contains(@name,\"F:\") or contains(@name,\"P:\")]");
                    if (classMembers == null || classMembers.Count == 0)
                    {
                        continue;
                    }
                    if ((methodMembers == null || methodMembers.Count == 0) && (fieldMembers == null || fieldMembers.Count == 0))
                    {
                        continue;
                    }
                    //3.解析xml
                    //获取所有类
                    Dictionary<string, ClassNode> classNodes = new Dictionary<string, ClassNode>();//类名映射类
                    for (int i = 0; i < classMembers.Count; i++)
                    {
                        XmlNode node = classMembers.Item(i);
                        ClassNode classNode = MemberToClassNode(node);
                        classNodes.Add(classNode.name, classNode);
                    }
                    //路径=目录+标题
                    //获取所有路径
                    HashSet<string> paths = new HashSet<string>() { string.Empty };
                    XmlNodeList catalogList = xml.SelectNodes("//catalog");
                    for (int i = 0; i < catalogList.Count; i++)
                    {
                        string path = GetTextByXmlNode(catalogList.Item(i));
                        paths.Add(path);
                    }
                    //一个路径对应一个showdoc
                    Dictionary<string, Showdoc> showdocs = AddPathShowdocMap(paths);
                    AddShowdocField(fieldMembers, classNodes, showdocs);
                    AddShowdocMethod(methodMembers, classNodes, showdocs);
                    foreach (var pair in showdocs)
                    {
                        //逐目录生成showdoc文档
                        CreateShowdoc(ConfigurationManager.AppSettings["api_key"], ConfigurationManager.AppSettings["api_token"], pair.Value);
                    }
                }
            }
            Console.WriteLine("结束");
            Console.ReadKey();
        }

        /// <summary>
        /// 创建路径->文档映射
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private static Dictionary<string, Showdoc> AddPathShowdocMap(HashSet<string> paths)
        {
            Dictionary<string, Showdoc> showdocs = new Dictionary<string, Showdoc>();
            foreach (string path in paths)
            {
                string title = string.Empty;
                string catalog = path;
                if (!string.IsNullOrEmpty(path))
                {
                    int index = path.LastIndexOf("/");
                    title = path.Substring(index + 1);
                    catalog = path.Substring(0, index);
                }

                Showdoc showdoc = new Showdoc(catalog, title);
                showdocs.Add(path, showdoc);
            }
            return showdocs;
        }

        /// <summary>
        /// 添加字段文档
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="classNodes"></param>
        /// <param name="showdocs"></param>
        private static void AddShowdocField(XmlNodeList nodes, Dictionary<string, ClassNode> classNodes, Dictionary<string, Showdoc> showdocs)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                //找到对应类
                string name = nodes.Item(i).Attributes["name"].Value.Replace("F:", string.Empty).Replace("P:", string.Empty);
                int index = name.LastIndexOf('.');
                string className = name.Substring(0, index);
                if (!classNodes.ContainsKey(className))
                {
                    classNodes.Add(className, new ClassNode(className));
                }
                ClassNode classNode = classNodes[className];
                FieldNode fieldNode = MemberToFieldNode(nodes.Item(i));
                //判断showdoc
                if (!classNode.showdoc && !fieldNode.showdoc)
                {
                    continue;
                }
                //判断catalog,class有则用class的,否则本身有用本身的,否则为缺省
                string catalog = string.Empty;
                if (!string.IsNullOrEmpty(classNode.catalog))
                {
                    catalog = classNode.catalog;
                }
                else if (!string.IsNullOrEmpty(fieldNode.catalog))
                {
                    catalog = fieldNode.catalog;
                }
                //补齐对应showdoc
                Showdoc showdoc = showdocs[catalog];
                if (!showdoc.classes.Contains(classNode))
                {
                    showdoc.classes.Add(classNode);
                }
                classNode.fields.Add(fieldNode);
            }
        }

        /// <summary>
        /// 添加方法文档
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="classNodes"></param>
        /// <param name="showdocs"></param>
        private static void AddShowdocMethod(XmlNodeList nodes, Dictionary<string, ClassNode> classNodes, Dictionary<string, Showdoc> showdocs)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                //找到对应类
                string name = nodes.Item(i).Attributes["name"].Value.Replace("M:", string.Empty);
                int index = name.IndexOf('(');
                name = index > 0 ? name.Substring(0, index) : name;
                index = name.LastIndexOf('.');
                string className = name.Substring(0, index);
                if (!classNodes.ContainsKey(className))
                {
                    classNodes.Add(className, new ClassNode(className));
                }
                ClassNode classNode = classNodes[className];
                MethodNode methodNode = MemberToMethodNode(nodes.Item(i));
                //判断showdoc
                if (!classNode.showdoc && !methodNode.showdoc)
                {
                    continue;
                }
                //判断catalog,class有则用class的,否则本身有用本身的,否则为缺省
                string catalog = string.Empty;
                if (!string.IsNullOrEmpty(classNode.catalog))
                {
                    catalog = classNode.catalog;
                }
                else if (!string.IsNullOrEmpty(methodNode.catalog))
                {
                    catalog = methodNode.catalog;
                }
                //补齐对应showdoc
                Showdoc showdoc = showdocs[catalog];
                if (!showdoc.classes.Contains(classNode))
                {
                    showdoc.classes.Add(classNode);
                }
                classNode.methods.Add(methodNode);
            }
        }

        private static ClassNode MemberToClassNode(XmlNode node)
        {
            //类名
            string name = node.Attributes["name"].Value.Replace("T:", string.Empty);
            //描述
            string summary = GetTextByXmlNode(node.SelectSingleNode("summary"));
            //目录
            string catalog = GetTextByXmlNode(node.SelectSingleNode("catalog"));
            //是否生成注释文档
            bool showdoc = GetValueByKey(node, "showdoc") == "true";
            return new ClassNode(name, summary, catalog, showdoc);
        }

        private static FieldNode MemberToFieldNode(XmlNode node)
        {
            //字段名
            string name = node.Attributes["name"].Value.Replace("F:", string.Empty).Replace("P:", string.Empty);
            int index = name.LastIndexOf(".");
            name = name.Substring(index + 1);
            //字段类型
            string type = GetTextByXmlNode(node.SelectSingleNode("remarks"));
            //描述
            string summary = GetTextByXmlNode(node.SelectSingleNode("summary"));
            //目录
            string catalog = GetTextByXmlNode(node.SelectSingleNode("catalog"));
            //是否参与生成注释文档
            bool showdoc = GetValueByKey(node, "showdoc") == "true";
            return new FieldNode(name, type, summary, catalog, showdoc);
        }

        private static MethodNode MemberToMethodNode(XmlNode node)
        {
            //方法名
            string name = node.Attributes["name"].Value.Replace("M:", string.Empty);
            int index = name.IndexOf('(');
            string subName = index > 0 ? name.Substring(0, index) : name;
            index = subName.LastIndexOf('.');
            name = name.Substring(index + 1);
            StringBuilder sb = new StringBuilder(name);
            int count1 = 0;
            int count2 = 0;
            for (int i = 0; i < sb.Length; i++)
            {
                char ch = sb[i];
                switch (ch)
                {
                    case '{':
                        count1++;
                        break;
                    case '}':
                        count1--;
                        break;
                    case '<':
                        count2++;
                        break;
                    case '>':
                        count2--;
                        break;
                    case ',':
                        if (count1 != 0 || count2 != 0)
                        {
                            sb[i] = '#';
                        }
                        break;
                }
            }
            var match = Regex.Match(sb.ToString(), "(?<=\\()\\S+(?=\\))");
            string[] argTypes = match.Value == string.Empty ? new string[0] : match.Value.Split(',');
            for (int i = 0; i < argTypes.Length; i++)
            {
                argTypes[i] = argTypes[i].Replace('#', ',');
            }
            XmlNodeList argNameList = node.SelectNodes("param");
            List<FieldNode> args = new List<FieldNode>();
            for (int i = 0; i < argTypes.Length; i++)
            {
                args.Add(new FieldNode(argNameList?.Item(i)?.Attributes["name"]?.Value, argTypes[i], GetTextByXmlNode(argNameList.Item(i))));
            }
            //描述
            string summary = GetTextByXmlNode(node.SelectSingleNode("summary"));
            string returnName = GetTextByXmlNode(node.SelectSingleNode("returns"));
            string returnType = GetValueByKey(node.SelectSingleNode("returns"), "type");
            KeyValuePair<string, string> returns = default(KeyValuePair<string, string>);
            if (!string.IsNullOrEmpty(returnName))
            {
                returns = new KeyValuePair<string, string>(returnType, returnName);
            }
            string catalog = GetTextByXmlNode(node.SelectSingleNode("catalog"));
            bool showdoc = GetValueByKey(node, "showdoc") == "true";
            return new MethodNode(name, summary, catalog, showdoc, args, returns);
        }

        private static string DictionaryToBody(Dictionary<string, string> dic)
        {
            StringBuilder body = new StringBuilder();
            foreach (var pair in dic)
            {
                body.AppendFormat("{0}={1}&", pair.Key, pair.Value);
            }
            body.Remove(body.Length - 1, 1);
            return body.ToString();
        }

        private static string GetTextByXmlNode(XmlNode node)
        {
            return node == null ? string.Empty : TrimAndLine(node.InnerText);
        }

        private static string TrimAndLine(string str)
        {
            return str.Replace("\n", string.Empty).Replace("\r", string.Empty).Trim();
        }

        private static string GetValueByKey(XmlNode node, string key)
        {
            //返回node节点名为key的属性值或子节点内部字符串
            string value = null;
            if (node == null)
            {
                return value;
            }
            string attr = node.SelectSingleNode("summary")?.Attributes[key]?.Value;
            if (!string.IsNullOrEmpty(attr))
            {
                //优先采用节点属性
                return attr;
            }
            string inner = GetTextByXmlNode(node.SelectSingleNode(key));
            if (!string.IsNullOrEmpty(inner))
            {
                //节点无该属性才用节点文本
                value = inner;
            }
            return value;
        }

        private static string ToStringByDefault(string value)
        {
            return string.IsNullOrEmpty(value) ? "无" : value;
        }

        private static void CreateShowdoc(string apiKey, string apiToken, Showdoc showdoc)
        {
            if (showdoc.classes == null || showdoc.classes.Count == 0)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("[TOC]\n\n");
            for (int i = 0; i < showdoc.classes.Count; i++)
            {
                ClassNode classNode = showdoc.classes[i];
                sb.AppendFormat("# {0} {1}\n", i + 1, classNode.name);
                if (!string.IsNullOrEmpty(classNode.summary))
                {
                    sb.AppendFormat("    {0}\n", classNode.summary);
                }
                int index = 0;
                if (classNode.fields != null && classNode.fields.Count != 0)
                {
                    index++;
                    sb.AppendFormat("## {0}.{1} 属性或公有字段\n\n", i + 1, index);
                    sb.AppendFormat("|名称|类型|描述|\n");
                    sb.AppendFormat("| ------------ | ------------ | ------------ |\n");
                    for (int j = 0; j < classNode.fields.Count; j++)
                    {
                        FieldNode fieldNode = classNode.fields[j];
                        sb.AppendFormat("|{0}|{1}|{2}|\n", fieldNode.name, ToStringByDefault(fieldNode.type), ToStringByDefault(fieldNode.summary));
                    }
                }
                if (classNode.methods != null && classNode.methods.Count != 0)
                {
                    index++;
                    sb.AppendFormat("## {0}.{1} 方法\n", i + 1, index);
                    for (int j = 0; j < classNode.methods.Count; j++)
                    {
                        MethodNode methodNode = classNode.methods[j];
                        sb.AppendFormat("### {0}.{1}.{2} {3}\n", i + 1, index, j + 1, methodNode.name);
                        //方法描述
                        if (!string.IsNullOrEmpty(methodNode.summary))
                        {
                            sb.AppendFormat("    {0}\n", methodNode.summary);
                        }
                        //参数列表
                        AppendArgTable(methodNode.args, sb);
                        //返回值
                        bool keyIsNull = string.IsNullOrEmpty(methodNode.returns.Key);
                        bool valueIsNull = string.IsNullOrEmpty(methodNode.returns.Value);
                        if (!keyIsNull || !valueIsNull)
                        {
                            sb.Append("返回值:\n\n");
                            sb.Append("|类型|描述|\n");
                            sb.Append("| ------------ | ------------ |\n");
                            sb.AppendFormat("|{0}|{1}|\n", ToStringByDefault(methodNode.returns.Key), ToStringByDefault(methodNode.returns.Value));
                        }
                    }
                }
            }
            CreateShowdoc(apiKey, apiToken, showdoc.catalog, showdoc.title, sb.ToString());
        }

        private static void AppendArgTable(List<FieldNode> args, StringBuilder sb)
        {
            if (sb == null || args == null || args.Count == 0)
            {
                return;
            }
            sb.Append("参数列表:\n\n");
            sb.Append("|名称|类型|描述|\n");
            sb.Append("| ------------ | ------------ | ------------ |\n");
            for (int k = 0; k < args.Count; k++)
            {
                FieldNode arg = args[k];
                sb.AppendFormat("|{0}|{1}|{2}|\n", arg.name, arg.type, ToStringByDefault(arg.summary));
            }
        }

        private static void CreateShowdoc(string apiKey, string apiToken, string catalog, string title, string content)
        {
            //生成showdoc文档
            string url = "https://www.showdoc.cc/server/api/item/updateByApi";
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            body.Add("api_key", apiKey);
            body.Add("api_token", apiToken);
            catalog = string.IsNullOrEmpty(catalog) ? "Default" : catalog;
            body.Add("cat_name", catalog);
            title = string.IsNullOrEmpty(title) ? "Default" : title;
            body.Add("page_title", title);
            body.Add("page_content", content);
            string bodyStr = DictionaryToBody(body);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(bodyStr);
            Stream stream = request.GetRequestStream();
            stream.Write(bodyBytes, 0, bodyBytes.Length);
            stream.Close();
            try
            {
                WebResponse response = request.GetResponse();
                StreamReader res = new StreamReader(response.GetResponseStream());
                Console.WriteLine(res.ReadToEnd());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }

}
