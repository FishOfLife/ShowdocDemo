using System.Collections.Generic;

namespace Showdoc
{
    public struct Showdoc
    {

        public string catalog;
        public string title;
        public List<ClassNode> classes;

        public Showdoc(string catalog, string title)
        {
            this.catalog = catalog;
            this.title = title;
            this.classes = new List<ClassNode>();
        }

        public Showdoc(string catalog, string title, List<ClassNode> classes)
        {
            this.catalog = catalog;
            this.title = title;
            this.classes = classes;
        }

    }

    public struct ClassNode
    {
        public string name;
        public string summary;
        public string catalog;
        public bool showdoc;
        public List<FieldNode> fields;
        public List<MethodNode> methods;

        public ClassNode(string name)
        {
            this.name = name;
            this.summary = string.Empty;
            this.catalog = string.Empty;
            this.showdoc = false;
            this.fields = new List<FieldNode>();
            this.methods = new List<MethodNode>();
        }

        public ClassNode(string name, string summary, string catalog, bool showdoc)
        {
            this.name = name;
            this.summary = summary;
            this.catalog = catalog;
            this.showdoc = showdoc;
            this.fields = new List<FieldNode>();
            this.methods = new List<MethodNode>();
        }

        public ClassNode(string name, string summary, string catalog, bool showdoc, List<FieldNode> fields, List<MethodNode> methods)
        {
            this.name = name;
            this.summary = summary;
            this.catalog = catalog;
            this.showdoc = showdoc;
            this.fields = fields;
            this.methods = methods;
        }

    }

    public struct FieldNode
    {
        public string name;
        public string type;
        public string summary;
        public string catalog;
        public bool showdoc;

        public FieldNode(string name, string type, string summary)
        {
            this.name = name;
            this.type = type;
            this.summary = summary;
            this.catalog = string.Empty;
            this.showdoc = false;
        }

        public FieldNode(string name, string type, string summary, string catalog, bool showdoc)
        {
            this.name = name;
            this.type = type;
            this.summary = summary;
            this.catalog = catalog;
            this.showdoc = showdoc;
        }

    }

    public struct MethodNode
    {
        public string name;
        public string summary;
        public string catalog;
        public bool showdoc;
        public List<FieldNode> args;
        public KeyValuePair<string, string> returns;

        public MethodNode(string name, string summary, string catalog, bool showdoc, List<FieldNode> args, KeyValuePair<string, string> returns)
        {
            this.name = name;
            this.summary = summary;
            this.catalog = catalog;
            this.showdoc = showdoc;
            this.args = args;
            this.returns = returns;
        }
    }

}
