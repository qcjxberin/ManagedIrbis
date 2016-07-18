﻿/* GblUtility.cs -- utility routines for GBL handling
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AM.Collections;
using AM.IO;
using AM.Runtime;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace ManagedIrbis.Gbl
{
    /// <summary>
    /// Utility routines for GBL file handling.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public static class GblUtility
    {
        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Restore <see cref="GblFile"/> from JSON.
        /// </summary>
        [NotNull]
        public static GblFile FromJson
            (
                [NotNull] string text
            )
        {
            Code.NotNullNorEmpty(text, "text");

            GblFile result = JsonConvert
                .DeserializeObject<GblFile>(text);

            return result;
        }

        /// <summary>
        /// Restore <see cref="GblFile"/> from JSON.
        /// </summary>
        [NotNull]
        public static GblFile FromXml
            (
                [NotNull] string text
            )
        {
            Code.NotNull(text, "text");

            XmlSerializer serializer = new XmlSerializer(typeof(GblFile));
            using (StringReader reader = new StringReader(text))
            {
                GblFile result = (GblFile) serializer.Deserialize(reader);

                return result;
            }
        }

        /// <summary>
        /// Parses the local JSON file.
        /// </summary>
        [NotNull]
        public static GblFile ParseLocalJsonFile
            (
                [NotNull] string fileName
            )
        {
            Code.NotNullNorEmpty(fileName, "fileName");

            string text = File.ReadAllText
                (
                    fileName,
                    IrbisEncoding.Utf8
                );
            GblFile result = FromJson(text);

            return result;
        }

        /// <summary>
        /// Parses the local XML file.
        /// </summary>
        [NotNull]
        public static GblFile ParseLocalXmlFile
            (
                [NotNull] string fileName
            )
        {
            Code.NotNullNorEmpty(fileName, "fileName");

            string text = File.ReadAllText
                (
                    fileName,
                    IrbisEncoding.Utf8
                );
            GblFile result = FromXml(text);

            return result;
        }

        /// <summary>
        /// Saves <see cref="GblFile"/> to local JSON file.
        /// </summary>
        public static void SaveLocalJsonFile
            (
                [NotNull] this GblFile gbl,
                [NotNull] string fileName
            )
        {
            Code.NotNull(gbl, "gbl");
            Code.NotNullNorEmpty(fileName, "fileName");

            string contents = JArray.FromObject(gbl)
                .ToString(Formatting.Indented);

            File.WriteAllText
                (
                    fileName,
                    contents,
                    IrbisEncoding.Utf8
                );
        }


        /// <summary>
        /// Convert <see cref="GblFile"/> to JSON.
        /// </summary>
        [NotNull]
        public static string ToJson
            (
                [NotNull] this GblFile gbl
            )
        {
            Code.NotNull(gbl, "gbl");

            string result = JObject.FromObject(gbl)
                .ToString(Formatting.None);

            return result;
        }

        /// <summary>
        /// Converts the <see cref="GblFile"/> to XML.
        /// </summary>
        [NotNull]
        public static string ToXml
            (
                [NotNull] this GblFile gbl
            )
        {
            Code.NotNull(gbl, "gbl");

            XmlSerializer serializer
                = new XmlSerializer(typeof(GblFile));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, gbl);

            return writer.ToString();
        }


        #endregion
    }
}