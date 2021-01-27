using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Showdoc
{
    public class Program
    {

        public static void Main(string[] args)
        {
            if (Debug.IsTest)
            {
                Debug.LogTest("测试模式");
                args = new string[] { "F:/Showdoc/Summary/bin/Debug/Summary.xml" };
            }
            if (args != null && args.Length != 0)
            {
                foreach (string arg in args)
                {
                    if (string.IsNullOrEmpty(arg))
                    {
                        continue;
                    }
                    //1.加载xml
                    XmlDocument xml;
                    if (!TryLoadXml(arg, out xml))
                    {
                        continue;
                    }
                    //2.解析xml
                    Showdoc[] showdocs;
                    if (!TryAnalyzeXml(xml, out showdocs))
                    {
                        continue;
                    }
                    //3.加载dll
                    string path = arg.Replace(".xml", ".dll");
                    Assembly assembly;
                    if (TryLoadAssembly(path, out assembly))
                    {
                        //4.解析dll
                        TryAnalyzeAssembly(assembly, ref showdocs);
                    }
                    //5.逐目录生成showdoc
                    if (Debug.IsTest)
                    {
                        for (int i = 0; i < showdocs.Length; i++)
                        {
                            Showdoc showdoc = showdocs[i];
                            Debug.LogTest("================================================================");
                            Debug.LogTest(string.Format("Catalog:{0}    |Title:{1}", showdocs[i].catalog, showdocs[i].title));
                            for (int j = 0; j < showdoc.classes.Count; j++)
                            {
                                ClassNode classNode = showdoc.classes[j];
                                Debug.LogTest(string.Format("    T{0}:{1}   |Summary:{2}", j + 1, classNode.name, classNode.summary));
                                for (int k = 0; k < classNode.properties.Count; k++)
                                {
                                    PropertyNode propertyNode = classNode.properties[k];
                                    Debug.LogTest(string.Format("       P{0}:{1}    |Summary:{2}", k + 1, propertyNode.name, propertyNode.summary));
                                }
                                for (int k = 0; k < classNode.fields.Count; k++)
                                {
                                    FieldNode fieldNode = classNode.fields[k];
                                    Debug.LogTest(string.Format("       F{0}:{1}    |Summary:{2}", k + 1, fieldNode.name, fieldNode.summary));
                                }
                                for (int k = 0; k < classNode.methods.Count; k++)
                                {
                                    MethodNode methodNode = classNode.methods[k];
                                    Debug.LogTest(string.Format("       M{0}:{1}    |Summary:{2}", k + 1, methodNode.name, methodNode.summary));
                                }
                            }
                            Debug.LogTest();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < showdocs.Length; i++)
                        {
                            CreateShowdoc(ConfigurationManager.AppSettings["api_key"], ConfigurationManager.AppSettings["api_token"], showdocs[i]);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("命令行输入参数未空");
            }
            Debug.Log("结束");
            Debug.ReadKey();
        }

        public static bool TryLoadXml(string path, out XmlDocument xml)
        {
            xml = new XmlDocument();
            try
            {
                xml.Load(path);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                xml = null;
                return false;
            }
        }

        public static bool TryAnalyzeXml(XmlDocument xml, out Showdoc[] showdocs)
        {
            //1.XPath搜索类,属性,字段,方法的注释
            XmlNodeList classMembers = xml.SelectNodes("/doc//member[contains(@name,\"T:\")]");
            XmlNodeList propertyMembers = xml.SelectNodes("/doc//member[contains(@name,\"P:\")]");
            XmlNodeList fieldMembers = xml.SelectNodes("/doc//member[contains(@name,\"F:\")]");
            XmlNodeList methodMembers = xml.SelectNodes("/doc//member[contains(@name,\"M:\")]");
            bool a = classMembers == null || classMembers.Count == 0;
            bool b = propertyMembers == null || propertyMembers.Count == 0;
            bool c = fieldMembers == null || fieldMembers.Count == 0;
            bool d = methodMembers == null || methodMembers.Count == 0;
            //类 is null 或成员 is null
            if (a || (b && c && d))
            {
                Debug.LogError("类/成员注释缺失");
                showdocs = null;
                return false;
            }
            //2.获取类名到类的映射字典
            Dictionary<string, ClassNode> classNodes = new Dictionary<string, ClassNode>();
            for (int i = 0; i < classMembers.Count; i++)
            {
                XmlNode node = classMembers.Item(i);
                ClassNode classNode = MemberToClassNode(node);
                classNodes.Add(classNode.name, classNode);
            }
            //3.获取所有路径
            HashSet<string> paths = new HashSet<string>();
            XmlNodeList catalogs = xml.SelectNodes("//catalog");
            for (int i = 0; i < catalogs.Count; i++)
            {
                string path = GetInnerText(catalogs.Item(i));
                paths.Add(path);
            }
            //4.逐路径创建showodc
            Dictionary<string, Showdoc> showdocDic = new Dictionary<string, Showdoc>();
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
                showdocDic.Add(path, showdoc);
            }
            //5.填充属性
            FillShowdocProperty(propertyMembers, classNodes, showdocDic);
            //6.填充字段
            FillShowdocField(fieldMembers, classNodes, showdocDic);
            //7.填充方法
            FillShowdocMethod(methodMembers, classNodes, showdocDic);
            showdocs = new Showdoc[showdocDic.Count];
            int count = 0;
            foreach (var pair in showdocDic)
            {
                showdocs[count] = pair.Value;
                count++;
            }
            return true;
        }

        public static bool TryLoadAssembly(string path, out Assembly assembly)
        {
            try
            {
                assembly = Assembly.LoadFile(path);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                assembly = null;
                return false;
            }
        }

        public static void TryAnalyzeAssembly(Assembly assembly, ref Showdoc[] showdocs)
        {
            //遍历每一份showdoc
            for (int i = 0; i < showdocs.Length; i++)
            {
                Showdoc showdoc = showdocs[i];
                //遍历每一个类
                for (int j = 0; j < showdoc.classes.Count; j++)
                {
                    ClassNode classNode = showdoc.classes[j];
                    Type type = assembly.GetType(classNode.name);
                    if (type == null)
                    {
                        continue;
                    }
                    if (classNode.properties != null && classNode.properties.Count != 0)
                    {
                        //补充属性数据
                        for (int k = 0; k < classNode.properties.Count; k++)
                        {
                            PropertyNode propertyNode = classNode.properties[k];
                            PropertyInfo propertyInfo = type.GetProperty(propertyNode.name);
                            if (propertyInfo == null)
                            {
                                continue;
                            }
                            propertyNode.type = propertyInfo.PropertyType.Name;
                            int a = propertyInfo.GetMethod == null ? 0 : 1;
                            int b = propertyInfo.SetMethod != null ? 0 : 1;
                            propertyNode.accessors = (Accessors)(b << 1) + a;
                        }
                    }
                    if (classNode.fields != null && classNode.fields.Count != 0)
                    {
                        //补充字段数据
                        for (int k = 0; k < classNode.fields.Count; k++)
                        {
                            FieldNode fieldNode = classNode.fields[k];
                            FieldInfo fieldInfo = type.GetField(fieldNode.name);
                            if (fieldInfo == null)
                            {
                                continue;
                            }
                            fieldNode.type = fieldInfo.FieldType.Name;
                        }
                    }
                    if (classNode.methods != null && classNode.methods.Count != 0)
                    {
                        //补充方法数据
                        MethodInfo[] methodInfos = type.GetMethods();
                        for (int k = 0; k < classNode.methods.Count; k++)
                        {
                            MethodNode methodNode = classNode.methods[k];
                            for (int l = 0; l < methodInfos.Length; l++)
                            {
                                MethodInfo methodInfo = methodInfos[l];
                                //过滤方法名不同的,剩下同名方法
                                int index = methodNode.name.IndexOf('(');
                                index = index == -1 ? methodNode.name.Length : index;
                                string name = methodNode.name.Substring(0, index);
                                if (methodInfo.Name != name)
                                {
                                    continue;
                                }
                                //过滤参数列表数量不同的,剩下同名同参数数量方法
                                ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                                if (parameterInfos.Length != methodNode.args.Count)
                                {
                                    continue;
                                }
                                bool isEquals = true;
                                //逐参数类型判断
                                for (int m = 0; m < parameterInfos.Length; m++)
                                {
                                    int radex = methodNode.args[m].type.LastIndexOf('.');
                                    radex = radex == -1 ? 0 : radex + 1;
                                    string argName = methodNode.args[m].type.Substring(radex);
                                    if (parameterInfos[m].ParameterType.Name != argName)
                                    {
                                        isEquals = false;
                                        break;
                                    }
                                }
                                //找到该方法
                                if (isEquals)
                                {
                                    string key = methodInfo.ReturnType.Name;
                                    string value = methodNode.returns.Value;
                                    methodNode.returns = new KeyValuePair<string, string>(key, value);
                                    Debug.Log(string.Format("修正方法{0}.{1}", classNode.name, methodNode.name));
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void FillShowdocProperty(XmlNodeList list, Dictionary<string, ClassNode> nodes, Dictionary<string, Showdoc> showdocs)
        {
            if (list == null || list.Count == 0)
            {
                Debug.Log("未找到属性注释");
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                string name = list.Item(i).Attributes["name"].Value.Replace("P:", string.Empty);
                int index = name.LastIndexOf('.');
                string className = name.Substring(0, index);
                if (!nodes.ContainsKey(className))
                {
                    nodes.Add(className, new ClassNode(className));
                }
                ClassNode classNode = nodes[className];
                PropertyNode propertyNode = MemberToPropertyNode(list.Item(i));
                if (!classNode.showdoc && !propertyNode.showdoc)
                {
                    continue;
                }
                string catalog = null;
                if (!string.IsNullOrEmpty(classNode.catalog))
                {
                    catalog = classNode.catalog;
                }
                else if (!string.IsNullOrEmpty(propertyNode.catalog))
                {
                    catalog = propertyNode.catalog;
                }
                if (string.IsNullOrEmpty(catalog))
                {
                    continue;
                }
                Showdoc showdoc = showdocs[catalog];
                if (!showdoc.classes.Contains(classNode))
                {
                    showdoc.classes.Add(classNode);
                }
                classNode.properties.Add(propertyNode);
                Debug.Log(string.Format("找到属性{0}.{1}", classNode.name, propertyNode.name));
            }
        }

        public static void FillShowdocField(XmlNodeList list, Dictionary<string, ClassNode> nodes, Dictionary<string, Showdoc> showdocs)
        {
            if (list == null || list.Count == 0)
            {
                Debug.Log("未找到字段注释");
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                //找到对应类
                string name = list.Item(i).Attributes["name"].Value.Replace("F:", string.Empty);
                int index = name.LastIndexOf('.');
                string className = name.Substring(0, index);
                if (!nodes.ContainsKey(className))
                {
                    nodes.Add(className, new ClassNode(className));
                }
                ClassNode classNode = nodes[className];
                FieldNode fieldNode = MemberToFieldNode(list.Item(i));
                //判断showdoc
                if (!classNode.showdoc && !fieldNode.showdoc)
                {
                    continue;
                }
                //判断catalog,class有则用class的,否则本身有用本身的,否则为缺省
                string catalog = null;
                if (!string.IsNullOrEmpty(classNode.catalog))
                {
                    catalog = classNode.catalog;
                }
                else if (!string.IsNullOrEmpty(fieldNode.catalog))
                {
                    catalog = fieldNode.catalog;
                }
                if (string.IsNullOrEmpty(catalog))
                {
                    continue;
                }
                //补齐对应showdoc
                Showdoc showdoc = showdocs[catalog];
                if (!showdoc.classes.Contains(classNode))
                {
                    showdoc.classes.Add(classNode);
                }
                classNode.fields.Add(fieldNode);
                Debug.Log(string.Format("找到字段{0}.{1}", classNode.name, fieldNode.name));
            }
        }

        public static void FillShowdocMethod(XmlNodeList list, Dictionary<string, ClassNode> nodes, Dictionary<string, Showdoc> showdocs)
        {
            if (list == null || list.Count == 0)
            {
                Debug.Log("未找到方法注释");
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                //找到对应类
                string name = list.Item(i).Attributes["name"].Value.Replace("M:", string.Empty);
                int index = name.IndexOf('(');
                name = index > 0 ? name.Substring(0, index) : name;
                index = name.LastIndexOf('.');
                string className = name.Substring(0, index);
                if (!nodes.ContainsKey(className))
                {
                    nodes.Add(className, new ClassNode(className));
                }
                ClassNode classNode = nodes[className];
                MethodNode methodNode = MemberToMethodNode(list.Item(i));
                //判断showdoc
                if (!classNode.showdoc && !methodNode.showdoc)
                {
                    continue;
                }
                //判断catalog,class有则用class的,否则本身有用本身的,否则为缺省
                string catalog = null;
                if (!string.IsNullOrEmpty(classNode.catalog))
                {
                    catalog = classNode.catalog;
                }
                else if (!string.IsNullOrEmpty(methodNode.catalog))
                {
                    catalog = methodNode.catalog;
                }
                if (string.IsNullOrEmpty(catalog))
                {
                    continue;
                }
                //补齐对应showdoc
                Showdoc showdoc = showdocs[catalog];
                if (!showdoc.classes.Contains(classNode))
                {
                    showdoc.classes.Add(classNode);
                }
                classNode.methods.Add(methodNode);
                Debug.Log(string.Format("找到方法{0}.{1}", classNode.name, methodNode.name));
            }
        }

        public static ClassNode MemberToClassNode(XmlNode node)
        {
            //类名
            string name = node.Attributes["name"].Value.Replace("T:", string.Empty);
            //描述
            string summary = GetInnerText(node.SelectSingleNode("summary"));
            //目录
            string catalog = GetInnerText(node.SelectSingleNode("catalog"));
            //是否生成注释文档
            bool showdoc = GetValueByKey(node, "showdoc") == "true";
            return new ClassNode(name, summary, catalog, showdoc);
        }

        public static PropertyNode MemberToPropertyNode(XmlNode node)
        {
            //类名
            string name = node.Attributes["name"].Value.Replace("P:", string.Empty);
            int index = name.LastIndexOf(".");
            name = name.Substring(index + 1);
            //字段类型
            string type = GetInnerText(node.SelectSingleNode("remarks"));
            //访问器
            Accessors accessors = Accessors.None;
            string accessorsStr = GetInnerText(node.SelectSingleNode("accessors"));
            if (int.TryParse(accessorsStr, out int accessorsInt))
            {
                if (accessorsInt >= 0 || accessorsInt <= 3)
                {
                    accessors = (Accessors)accessorsInt;
                }
            }
            //描述
            string summary = GetInnerText(node.SelectSingleNode("summary"));
            //目录
            string catalog = GetInnerText(node.SelectSingleNode("catalog"));
            //是否参与生成注释文档
            bool showdoc = GetValueByKey(node, "showdoc") == "true";
            return new PropertyNode(name, type, summary, catalog, showdoc, accessors);
        }

        public static FieldNode MemberToFieldNode(XmlNode node)
        {
            //字段名
            string name = node.Attributes["name"].Value.Replace("F:", string.Empty).Replace("P:", string.Empty);
            int index = name.LastIndexOf(".");
            name = name.Substring(index + 1);
            //字段类型
            string type = GetInnerText(node.SelectSingleNode("remarks"));
            //描述
            string summary = GetInnerText(node.SelectSingleNode("summary"));
            //目录
            string catalog = GetInnerText(node.SelectSingleNode("catalog"));
            //是否参与生成注释文档
            bool showdoc = GetValueByKey(node, "showdoc") == "true";
            return new FieldNode(name, type, summary, catalog, showdoc);
        }

        public static MethodNode MemberToMethodNode(XmlNode node)
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
            //去掉方法名和小括号
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
                args.Add(new FieldNode(argNameList?.Item(i)?.Attributes["name"]?.Value, argTypes[i], GetInnerText(argNameList.Item(i))));
            }
            //描述
            string summary = GetInnerText(node.SelectSingleNode("summary"));
            string returnName = GetInnerText(node.SelectSingleNode("returns"));
            string returnType = node.SelectSingleNode("returns")?.Attributes["type"]?.Value;
            KeyValuePair<string, string> returns = default(KeyValuePair<string, string>);
            if (!string.IsNullOrEmpty(returnName))
            {
                returns = new KeyValuePair<string, string>(returnType, returnName);
            }
            string catalog = GetInnerText(node.SelectSingleNode("catalog"));
            bool showdoc = GetValueByKey(node, "showdoc") == "true";
            return new MethodNode(name, summary, catalog, showdoc, args, returns);
        }

        public static string GetInnerText(XmlNode node)
        {
            if (node == null)
            {
                return null;
            }
            return Trim(node.InnerText);
        }

        public static string Trim(string str)
        {
            return str.Replace("\n", string.Empty).Replace("\r", string.Empty).Trim();
        }

        public static string GetValueByKey(XmlNode node, string key)
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
            string inner = GetInnerText(node.SelectSingleNode(key));
            if (!string.IsNullOrEmpty(inner))
            {
                //节点无该属性才用节点文本
                value = inner;
            }
            return value;
        }

        public static void CreateShowdoc(string apiKey, string apiToken, Showdoc showdoc)
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
                int radex = classNode.name.LastIndexOf('.');
                string className = classNode.name.Substring(radex + 1);
                sb.AppendFormat("# {0} {1}\n", i + 1, className);
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
            Dictionary<string, string> body = new Dictionary<string, string>();
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

        private static string ToStringByDefault(string value)
        {
            return string.IsNullOrEmpty(value) ? "无" : value;
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

    }

}
