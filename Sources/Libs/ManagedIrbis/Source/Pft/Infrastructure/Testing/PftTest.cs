﻿/* PftTest.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeJam;

using JetBrains.Annotations;

using ManagedIrbis.ImportExport;

using MoonSharp.Interpreter;

#endregion

namespace ManagedIrbis.Pft.Infrastructure.Testing
{
    /// <summary>
    /// Single test for PFT formatting.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public sealed class PftTest
    {
        #region Properties

        /// <summary>
        /// Folder name.
        /// </summary>
        [NotNull]
        public string Folder { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public PftTest
            (
                [NotNull] string folder
            )
        {
            Code.NotNullNorEmpty(folder, "folder");

            Folder = Path.GetFullPath(folder);
        }

        #endregion

        #region Private members

        private string GetFullName
            (
                string shortName
            )
        {
            return Path.Combine
                (
                    Folder,
                    shortName
                );
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Run the test.
        /// </summary>
        public bool Run()
        {
            string recordFile = GetFullName("record.txt");
            MarcRecord record = PlainText.ReadOneRecord
                (
                    recordFile,
                    IrbisEncoding.Utf8
                );

            string pftFile = GetFullName("input.txt");
            string pftText = File.ReadAllText
                (
                    pftFile,
                    IrbisEncoding.Utf8
                );

            PftLexer lexer = new PftLexer();
            PftTokenList tokenList = lexer.Tokenize(pftText);
            PftParser parser = new PftParser(tokenList);
            PftProgram program = parser.Parse();

            string backFormat = program.ToString();
            Console.WriteLine(backFormat);

            return true;
        }

        #endregion
    }
}
