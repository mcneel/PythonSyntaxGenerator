using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.XPath;
using System.Runtime.InteropServices;
using System.Resources;

using Sandcastle.Core;
using Sandcastle.Core.BuildAssembler.SyntaxGenerator;

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguage("en")]

namespace PythonSyntax
{
    public class PythonSyntaxGenerator : SyntaxGeneratorTemplate
    {
        #region Syntax generator factory
        private const string LanguageName = "Python", StyleIdName = "py";

        [SyntaxGeneratorExport(
            "Python",
            LanguageName,
            StyleIdName,
            SortOrder = 500,
            AlternateIds = "Python, python, py"
        )]
        public sealed class Factory : ISyntaxGeneratorFactory
        {
            public string ResourceItemFileLocation => Path.Combine(ComponentUtilities.AssemblyFolder(
                Assembly.GetExecutingAssembly()), "SyntaxContent");

            public SyntaxGeneratorCore Create()
            {
                return new PythonSyntaxGenerator { Language = LanguageName, StyleId = StyleIdName };
            }
        }
        #endregion

        #region Abstract method implementations
        public override void WriteNamespaceSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
        }

        public override void WriteStructureSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            WriteClassSyntax(reflection, writer);
        }

        public override void WriteClassSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            string name = reflection.Evaluate(apiNameExpression).ToString();
            writer.WriteKeyword("class");
            writer.WriteString(" ");
            writer.WriteIdentifier(name);
        }

        public override void WriteEnumerationSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            string name = reflection.Evaluate(apiNameExpression).ToString();
            writer.WriteKeyword("class");
            writer.WriteString(" ");
            writer.WriteIdentifier(name);
            writer.WriteString("(");
            writer.WriteIdentifier("enum.Enum");
            writer.WriteString(")");
        }

        public override void WriteConstructorSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            WriteFunctionSyntax(reflection, writer, new FunctionOptions { Name = "__init__", IsStatic = false, IncludeReturn = false });
        }

        public override void WriteMethodSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            WriteFunctionSyntax(reflection, writer, new FunctionOptions());
        }

        public override void WriteNormalMethodSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            WriteFunctionSyntax(reflection, writer, new FunctionOptions());
        }

        public override void WriteDelegateSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteString("# delegate syntax is not supported");
        }

        public override void WriteEventSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteString("# event syntax is not supported");
        }

        public override void WriteInterfaceSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteString("# interface syntax is not supported");
        }

        public override void WriteFieldSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            string name = (string)reflection.Evaluate(apiNameExpression);
            XPathNavigator type = reflection.SelectSingleNode(apiReturnTypeExpression);
            XPathNavigator argument = reflection.SelectSingleNode(parameterArgumentExpression);

            WriteParameter(name, type, argument, writer);
        }

        public override void WritePropertySyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            string name = (string)reflection.Evaluate(apiNameExpression);
            XPathNavigator type = reflection.SelectSingleNode(apiReturnTypeExpression);

            bool isGettable = (bool)reflection.Evaluate(apiIsReadPropertyExpression);
            bool isSettable = (bool)reflection.Evaluate(apiIsWritePropertyExpression);

            writer.WriteKeyword("@property");
            writer.WriteLine();

            if (isGettable)
                WriteFunctionSyntax(reflection, writer, new FunctionOptions { Name = name, IsStatic = false });

            if (isSettable)
            {
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteKeyword($"@{name}.setter");
                writer.WriteLine();

                WriteFunctionSyntax(reflection, writer,
                    new FunctionOptions
                    {
                        Name = name,
                        IsStatic = false,
                        IncludeReturn = false,
                        ParametersBefore = new FunctionParam[] {
                            new FunctionParam
                            {
                                Name = "value",
                                Type = type
                            }
                        }
                    }
                );
            }
        }

        protected override void WriteTypeReference(XPathNavigator reference, SyntaxWriter writer)
        {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            switch (reference.LocalName)
            {
                case "arrayOf":
                    // write "Array[Type]" instead of "Type[]"
                    int rank = Convert.ToInt32(reference.GetAttribute("rank", string.Empty), CultureInfo.InvariantCulture);
                    XPathNavigator navigator = reference.SelectSingleNode(typeExpression);

                    for (int i = 1; i < rank + 1; i++)
                    {
                        writer.WriteReferenceLink("T:System.Array");
                        writer.WriteString("[");
                    }

                    WriteTypeReference(navigator, writer);

                    for (int i = 1; i < rank + 1; i++)
                    {
                        writer.WriteString("]");
                    }

                    break;

                case "type":
                    string id = reference.GetAttribute("api", string.Empty);
                    XPathNodeIterator typeModifiers = reference.Select(typeModifiersExpression);

                    if (id.StartsWith("T:System.ValueTuple`", StringComparison.Ordinal))
                    {
                        writer.WriteString("(");
                        while (typeModifiers.MoveNext())
                        {
                            XPathNodeIterator args = typeModifiers.Current.Select(specializationArgumentsExpression);

                            while (args.MoveNext())
                            {
                                if (args.CurrentPosition > 1)
                                    writer.WriteString(", ");

                                WriteTypeReference(args.Current, writer);
                                var elementName = args.Current.GetAttribute("elementName", String.Empty);
                                if (elementName != null)
                                {
                                    writer.WriteString(" ");
                                    writer.WriteString(elementName);
                                }
                            }
                        }

                        writer.WriteString(")");
                    }
                    else
                    {
                        WriteNormalTypeReference(id, writer);
                        while (typeModifiers.MoveNext())
                            WriteTypeReference(typeModifiers.Current, writer);
                    }
                    break;

                case "template":
                    string name = reference.GetAttribute("name", string.Empty);
                    writer.WriteString(name);
                    XPathNodeIterator modifiers = reference.Select(typeModifiersExpression);

                    while (modifiers.MoveNext())
                        WriteTypeReference(modifiers.Current, writer);

                    break;

                case "specialization":
                    writer.WriteString("[");
                    XPathNodeIterator arguments = reference.Select(specializationArgumentsExpression);

                    while (arguments.MoveNext())
                    {
                        if (arguments.CurrentPosition > 1)
                            writer.WriteString(", ");

                        WriteTypeReference(arguments.Current, writer);
                    }

                    writer.WriteString("]");
                    break;

                case "referenceTo":
                    XPathNavigator referTo = reference.SelectSingleNode(typeExpression);
                    WriteTypeReference(referTo, writer);
                    break;

                case "parameter":
                    XPathNavigator paramType = reference.SelectSingleNode(parameterTypeExpression);
                    WriteTypeReference(paramType, writer);
                    break;
            }
        }

        protected override void WriteNormalTypeReference(string api, SyntaxWriter writer)
        {
            if (api == null)
                throw new ArgumentNullException(nameof(api));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            switch (api)
            {
                case "T:System.Char":
                    writer.WriteReferenceLink(api, "str");
                    break;

                case "T:System.Byte":
                case "T:System.SByte":
                case "T:System.Int16":
                case "T:System.Int32":
                case "T:System.Int64":
                case "T:System.UInt16":
                case "T:System.UInt32":
                case "T:System.UInt64":
                    writer.WriteReferenceLink(api, "int");
                    break;

                case "T:System.Single":
                case "T:System.Double":
                    writer.WriteReferenceLink(api, "float");
                    break;

                case "T:System.Decimal":
                    writer.WriteReferenceLink(api, "Decimal");
                    break;

                case "T:System.Boolean":
                    writer.WriteReferenceLink(api, "bool");
                    break;

                default:
                    writer.WriteReferenceLink(api);
                    break;
            }
        }
        #endregion

        class FunctionOptions
        {
            public string Name { get; set; }
            public bool? IsStatic { get; set; }
            public bool IncludeReturn { get; set; } = true;
            public FunctionParam[] ParametersBefore { get; set; } = Array.Empty<FunctionParam>();
        }

        class FunctionParam
        {
            public string Name { get; set; }
            public XPathNavigator Type { get; set; }
            public XPathNavigator DefaultValue { get; set; }
        }

        private void WriteFunctionSyntax(XPathNavigator reflection, SyntaxWriter writer, FunctionOptions opts)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            WriteFunctionDefinition(reflection, writer, opts);
            XPathNavigator[] outputs = WriteMethodParameters(reflection, writer, opts);
            if (opts.IncludeReturn)
                WriteReturnValue(reflection, writer, outputs);
        }

        private static void WriteFunctionDefinition(XPathNavigator reflection, SyntaxWriter writer, FunctionOptions opts)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            string name = opts.Name ?? (string)reflection.Evaluate(apiNameExpression);
            bool isStatic = opts.IsStatic.HasValue ? opts.IsStatic.Value : (bool)reflection.Evaluate(apiIsStaticExpression);

            if (isStatic)
            {
                writer.WriteKeyword("@staticmethod");
                writer.WriteLine();
            }

            writer.WriteKeyword("def");
            writer.WriteString(" ");
            writer.WriteIdentifier(name);
        }

        private XPathNavigator[] WriteMethodParameters(XPathNavigator reflection, SyntaxWriter writer, FunctionOptions opts)
        {
            XPathNodeIterator parameters = reflection.Select(apiParametersExpression);

            writer.WriteString("(");
            XPathNavigator[] outputs = WriteParameters(parameters, writer, opts);
            writer.WriteString(")");

            return outputs;
        }

        private XPathNavigator[] WriteParameters(XPathNodeIterator parameters, SyntaxWriter writer, FunctionOptions opts)
        {
            bool expand = false;
            if (parameters.Count > 0)
                expand = true;

            uint prevParams = 0;

            if (expand)
                writer.WriteLine();

            if (opts.IsStatic.HasValue && !opts.IsStatic.Value)
            {
                if (expand)
                    writer.WriteString("\t");
                writer.WriteIdentifier("self");
                prevParams++;
            }

            if (opts.ParametersBefore.Length > 0)
            {
                foreach (FunctionParam param in opts.ParametersBefore)
                {
                    if (prevParams > 0)
                    {
                        writer.WriteString(", ");
                        if (expand)
                            writer.WriteLine();
                    }

                    if (expand)
                        writer.WriteString("\t");

                    WriteParameter(param.Name, param.Type, param.DefaultValue, writer);
                    prevParams++;
                }
            }

            var outputs = new List<XPathNavigator>();

            if (parameters.Count > 0)
            {
                if (prevParams > 0)
                {
                    writer.WriteString(", ");
                    if (expand)
                        writer.WriteLine();
                }

                while (parameters.MoveNext())
                {
                    XPathNavigator parameter = parameters.Current;
                    bool isOut = (bool)parameter.Evaluate(parameterIsOutExpression);

                    if (isOut)
                    {
                        outputs.Add(parameter);
                        continue;
                    }

                    if (expand)
                        writer.WriteString("\t");

                    WriteParameter(parameter, writer);

                    if (parameters.CurrentPosition < parameters.Count)
                        writer.WriteString(",");

                    if (expand)
                        writer.WriteLine();
                }
            }

            return outputs.ToArray();
        }

        private void WriteParameter(XPathNavigator parameter, SyntaxWriter writer)
        {
            string name = (string)parameter.Evaluate(parameterNameExpression);
            XPathNavigator type = parameter.SelectSingleNode(parameterTypeExpression);
            XPathNavigator argument = parameter.SelectSingleNode(parameterArgumentExpression);

            WriteParameter(name, type, argument, writer);
        }

        private void WriteParameter(string name, XPathNavigator type, XPathNavigator argument, SyntaxWriter writer)
        {
            writer.WriteParameter(name);
            writer.WriteString(": ");
            WriteTypeReference(type, writer);

            // write parameter default value, if any
            if (argument != null)
            {
                writer.WriteString("=");
                WriteValue(argument, writer);
            }
        }

        private void WriteReturnValue(XPathNavigator reflection, SyntaxWriter writer, XPathNavigator[] outputs)
        {
            XPathNavigator type = reflection.SelectSingleNode(apiReturnTypeExpression);

            writer.WriteString(" -> ");

            if (outputs.Length > 0)
            {
                writer.WriteString("(");

                if (type == null)
                    writer.WriteKeyword("None");
                else
                    WriteTypeReference(type, writer);

                foreach (XPathNavigator output in outputs)
                {
                    writer.WriteString(", ");
                    WriteTypeReference(output, writer);
                }

                writer.WriteString(")");
            }
            else
            {
                if (type == null)
                    writer.WriteKeyword("None");
                else
                    WriteTypeReference(type, writer);
            }
        }

        private void WriteValue(XPathNavigator parent, SyntaxWriter writer)
        {
            XPathNavigator type = parent.SelectSingleNode(attributeTypeExpression);
            XPathNavigator value = parent.SelectSingleNode(valueExpression);

            switch (value.LocalName)
            {
                case "nullValue":
                    writer.WriteKeyword("None");
                    break;

                case "defaultValue":
                    break;

                case "typeValue":
                    break;

                case "enumValue":
                    XPathNodeIterator fields = value.SelectChildren(XPathNodeType.Element);
                    while (fields.MoveNext())
                    {
                        string name = fields.Current.GetAttribute("name", string.Empty);
                        if (fields.CurrentPosition > 1)
                            writer.WriteString("|");
                        WriteTypeReference(type, writer);
                        writer.WriteString(".");
                        writer.WriteString(name);
                    }
                    break;

                case "value":
                    string text = value.Value;
                    string typeId = type.GetAttribute("api", string.Empty);

                    switch (typeId)
                    {
                        case "T:System.Char":
                        case "T:System.String":
                            writer.WriteString("\"");
                            writer.WriteString(text);
                            writer.WriteString("\"");
                            break;

                        case "T:System.Boolean":
                            writer.WriteKeyword(Convert.ToBoolean(text, CultureInfo.InvariantCulture) ?
                                "True" : "False");
                            break;

                        case "T:System.Byte":
                        case "T:System.Single":
                        case "T:System.Double":
                        case "T:System.SByte":
                        case "T:System.Int16":
                        case "T:System.Int64":
                        case "T:System.Int32":
                        case "T:System.UInt16":
                        case "T:System.UInt32":
                        case "T:System.UInt64":
                            writer.WriteString(text);
                            break;

                        default:
                            writer.WriteString(type.LocalName);
                            break;
                    }
                    break;
            }
        }
    }
}
