﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* LocalFstProcessor.cs --
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

using AM;

using CodeJam;

using JetBrains.Annotations;

using ManagedIrbis.Client;
using ManagedIrbis.Pft.Infrastructure;

using MoonSharp.Interpreter;

#endregion

namespace ManagedIrbis.Fst
{
    /// <summary>
    /// Local FST processor.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public sealed class LocalFstProcessor
    {
        #region Properties

        /// <summary>
        /// Environment.
        /// </summary>
        //public PftEnvironmentAbstraction Environment { get; private set; }
        public AbstractClient Environment { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalFstProcessor
            (
                [NotNull] string rootPath,
                [NotNull] string database
            )
        {
            Code.NotNullNorEmpty(rootPath, "rootPath");
            Code.NotNullNorEmpty(database, "database");

            //PftLocalEnvironment environment
            //    = new PftLocalEnvironment(rootPath)
            //    {
            //        Database = database
            //    };
            LocalClient environment
                = new LocalClient(rootPath)
                {
                    Database = database
                };
            Environment = environment;
        }

        #endregion

        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Read FST file from local file.
        /// </summary>
        [CanBeNull]
        public FstFile ReadFile
            (
                [NotNull] string fileName
            )
        {
            Code.NotNullNorEmpty(fileName, "fileName");

            string content = File.ReadAllText
                (
                    fileName,
                    IrbisEncoding.Ansi
                );
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            StringReader reader = new StringReader(content);
            FstFile result = FstFile.ParseStream(reader);
            if (result.Lines.Count == 0)
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Transform record.
        /// </summary>
        [NotNull]
        public MarcRecord TransformRecord
            (
                [NotNull] MarcRecord record,
                [NotNull] string format
            )
        {
            Code.NotNull(record, "record");
            Code.NotNullNorEmpty(format, "format");

            PftLexer lexer = new PftLexer();
            PftTokenList tokenList = lexer.Tokenize(format);
            PftParser parser = new PftParser(tokenList);
            PftContext context = new PftContext(null)
            {
                Record = record
            };
            context.SetEnvironment(Environment);
            PftProgram program = parser.Parse();
            program.Execute(context);
            string transformed = context.Text;

            MarcRecord result = new MarcRecord
            {
                Database = Environment.Database
            };
            string[] lines = transformed.Split((char) 0x07);
            string[] separators = {"\r\n", "\r", "\n"};
            foreach (string line in lines)
            {
#if !WINMOBILE && !PocketPC

                string[] parts = line.Split
                    (
                        separators,
                        StringSplitOptions.RemoveEmptyEntries
                    );

#else

                string[] parts = line
                    .Replace("\r", string.Empty)
                    .Split('\n');

#endif

                if (parts.Length == 0)
                {
                    continue;
                }
                string tag = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    string body = parts[i];
                    if (string.IsNullOrEmpty(body))
                    {
                        continue;
                    }
                    RecordField field
                        = RecordFieldUtility.Parse(tag, body);

                    SubField[] badSubFields
                        = field.SubFields
                        .Where(sf => string.IsNullOrEmpty(sf.Value))
                        .ToArray();
                    foreach (SubField subField in badSubFields)
                    {
                        field.SubFields.Remove(subField);
                    }

                    if (!string.IsNullOrEmpty(field.Value)
                        || field.SubFields.Count != 0)
                    {
                        result.Fields.Add(field);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Transform the record.
        /// </summary>
        [NotNull]
        public MarcRecord TransformRecord
            (
                [NotNull] MarcRecord record,
                [NotNull] FstFile fstFile
            )
        {
            Code.NotNull(record, "record");
            Code.NotNull(fstFile, "fstFile");

            string format = fstFile.ConcatenateFormat();
            MarcRecord result = TransformRecord
                (
                    record,
                    format
                );

            return result;
        }

        #endregion
    }
}
