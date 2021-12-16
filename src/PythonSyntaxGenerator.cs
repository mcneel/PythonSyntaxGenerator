using Sandcastle.Core;
using Sandcastle.Core.BuildAssembler.SyntaxGenerator;
using System;
using System.IO;
using System.Reflection;
using System.Xml.XPath;

namespace PythonSyntaxGenerator
{
    public class PythonSyntaxGenerator : SyntaxGeneratorTemplate
    {
        #region Syntax generator factory for MEF
        private const string LanguageName = "Python", StyleIdName = "py";

        /// <summary>
        /// This is used to create a new instance of the syntax generator
        /// </summary>
        /// <remarks>The <c>keywordStyleParameter</c> parameter is used to set the keyword style in the
        /// presentation style and should unique to your programming language.  Set the additional attributes as
        /// needed:
        ///
        /// <list type="bullet">
        ///     <item>
        ///         <term>AlternateIds</term>
        ///         <description>Specify a comma-separated list of other language names that can be mapped to
        /// this generator.</description>
        ///     </item>
        ///     <item>
        ///         <term>IsConfigurable</term>
        ///         <description>Set this to true if your syntax generator contains configurable settings.
        /// Designers can use the <c>DefaultConfiguration</c> property to obtain the default configuration.</description>
        ///     </item>
        ///     <item>
        ///         <term>DefaultConfiguration</term>
        ///         <description>If your syntax generator has configurable settings, use this property to specify
        /// the default settings in an XML fragment.</description>
        ///     </item>
        /// </list>
        /// </remarks>
        [SyntaxGeneratorExport(
            "Python",
            LanguageName,
            StyleIdName,
            SortOrder = 500,
            Version = AssemblyInfo.ProductVersion,
            Copyright = AssemblyInfo.Copyright,
            Description = "Generates Python declaration syntax sections",
            AlternateIds = "Python, python, py"
        )]
        public sealed class Factory : ISyntaxGeneratorFactory
        {
            /// <inheritdoc />
            public string ResourceItemFileLocation => Path.Combine(ComponentUtilities.AssemblyFolder(
                Assembly.GetExecutingAssembly()), "SyntaxContent");

            /// <inheritdoc />
            public SyntaxGeneratorCore Create()
            {
                return new PythonSyntaxGenerator { Language = LanguageName, StyleId = StyleIdName };
            }
        }
        #endregion

        #region Abstract method implementations
        //=====================================================================

        // TODO: Each of the following methods must be implemented.  Syntax generation is rather complex.
        // It may be best to copy one of the existing syntax generators if the language is a close match
        // for the one you are trying to implement.

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override void WriteConstructorSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
        }

        /// <inheritdoc />
        public override void WriteDelegateSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
        }

        /// <inheritdoc />
        public override void WriteEnumerationSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
        }

        /// <inheritdoc />
        public override void WriteEventSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
        }

        /// <inheritdoc />
        public override void WriteFieldSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
        }

        /// <inheritdoc />
        public override void WriteInterfaceSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
        }

        /// <inheritdoc />
        public override void WriteNamespaceSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
        }

        /// <inheritdoc />
        public override void WritePropertySyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
        }

        /// <inheritdoc />
        public override void WriteStructureSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if (reflection == null)
                throw new ArgumentNullException(nameof(reflection));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
        }
        #endregion
    }
}
