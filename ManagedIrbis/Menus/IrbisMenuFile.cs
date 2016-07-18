﻿/* IrbisMenuFile.cs -- MNU file handling.
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using AM;
using AM.Collections;
using AM.IO;
using AM.Runtime;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Menus
{
    /// <summary>
    /// MNU file handling.
    /// </summary>
    [PublicAPI]
    [XmlRoot("menu")]
    [MoonSharpUserData]
    [JsonConverter(typeof(MenuConverter))]
    public sealed class IrbisMenuFile
        : IHandmadeSerializable
    {
        #region Constants

        /// <summary>
        /// End of menu marker.
        /// </summary>
        public const string StopMarker = "*****";

        #endregion

        #region Nested classes

        /// <summary>
        /// Menu sorting.
        /// </summary>
        [PublicAPI]
        public enum Sort
        {
            /// <summary>
            /// None sorting.
            /// </summary>
            None,

            /// <summary>
            /// Sort by code.
            /// </summary>
            ByCode,

            /// <summary>
            /// Sort by comment.
            /// </summary>
            ByComment
        }

        /// <summary>
        /// Menu entry. Represents two lines.
        /// </summary>
        [PublicAPI]
        [XmlRoot("entry")]
        [MoonSharpUserData]
        [DebuggerDisplay("{Code} = {Comment}")]
        public sealed class Entry
            : IHandmadeSerializable
        {
            #region Properties

            /// <summary>
            /// First line -- the code.
            /// </summary>
            [NotNull]
            [XmlAttribute("code")]
            [JsonProperty("code")]
            // ReSharper disable NotNullMemberIsNotInitialized
            public string Code { get; set; }
            // ReSharper restore NotNullMemberIsNotInitialized

            /// <summary>
            /// Second line -- the comment.
            /// </summary>
            [CanBeNull]
            [XmlAttribute("comment")]
            [JsonProperty("comment")]
            public string Comment { get; set; }

            #endregion

            #region IHandmadeSerializable

            /// <summary>
            /// Restore object state from given stream.
            /// </summary>
            public void RestoreFromStream
                (
                    BinaryReader reader
                )
            {
                // ReSharper disable AssignNullToNotNullAttribute
                Code = reader.ReadNullableString();
                Comment = reader.ReadNullableString();
                // ReSharper restore AssignNullToNotNullAttribute
            }

            /// <summary>
            /// Save object state to the stream.
            /// </summary>
            public void SaveToStream
                (
                    BinaryWriter writer
                )
            {
                writer
                    .WriteNullable(Code)
                    .WriteNullable(Comment);
            }

            #endregion

            #region Public methods

            /// <summary>
            /// Should JSON serialize the comment?
            /// </summary>
            public bool ShouldSerializeComment()
            {
                return !string.IsNullOrEmpty(Comment);
            }

            #endregion

            #region Object members

            /// <summary>
            /// Returns a <see cref="System.String" />
            /// that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" />
            /// that represents this instance.</returns>
            public override string ToString()
            {
                return string.Format
                    (
                        "Code: {0}, Comment: {1}",
                        Code,
                        Comment
                    );
            }

            #endregion
        }
        
        /// <summary>
        /// Converts the <see cref="IrbisMenuFile"/> to JSON.
        /// </summary>
        public sealed class MenuConverter
            : JsonConverter
        {
            #region JsonConverter members

            /// <summary>
            /// Writes the JSON representation of the object.
            /// </summary>
            /// <param name="writer">The
            /// <see cref="T:Newtonsoft.Json.JsonWriter" />
            /// to write to.</param>
            /// <param name="value">The value.</param>
            /// <param name="serializer">The calling serializer.
            /// </param>
            public override void WriteJson
                (
                    JsonWriter writer,
                    object value,
                    JsonSerializer serializer
                )
            {
                IrbisMenuFile menu = (IrbisMenuFile) value;
                serializer.Serialize(writer, menu.Entries);
            }

            /// <summary>
            /// Reads the JSON representation of the object.
            /// </summary>
            /// <param name="reader">The
            /// <see cref="T:Newtonsoft.Json.JsonReader" />
            /// to read from.</param>
            /// <param name="objectType">Type of the object.</param>
            /// <param name="existingValue">The existing value
            /// of object being read.</param>
            /// <param name="serializer">The calling serializer.
            /// </param>
            /// <returns>The object value.</returns>
            public override object ReadJson
                (
                    JsonReader reader,
                    Type objectType,
                    object existingValue,
                    JsonSerializer serializer
                )
            {
                IrbisMenuFile menu = (IrbisMenuFile) existingValue;
                NonNullCollection<Entry> entries = serializer
                    .Deserialize<NonNullCollection<Entry>>
                        (
                            reader
                        );
                menu._entries.AddRange(entries);

                return menu;
            }

            /// <summary>
            /// Determines whether this instance can convert
            /// the specified object type.
            /// </summary>
            /// <param name="objectType">Type of the object.</param>
            /// <returns><c>true</c> if this instance can convert
            /// the specified object type; otherwise, <c>false</c>.
            /// </returns>
            public override bool CanConvert
                (
                    Type objectType
                )
            {
                return objectType == typeof(IrbisMenuFile);
            }

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of menu file -- for identification
        /// purposes only.
        /// </summary>
        [JsonIgnore]
        public string FileName { get; set; }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        [NotNull]
        [XmlElement("entry")]
        [JsonProperty("entries")]
        public NonNullCollection<Entry> Entries
        {
            get
            {
                return _entries;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public IrbisMenuFile()
        {
            _entries = new NonNullCollection<Entry>();
        }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal IrbisMenuFile
            (
                NonNullCollection<Entry> entries
            )
        {
            _entries = entries;
        }

        #endregion

        #region Private members

        private readonly NonNullCollection<Entry> _entries;

        #endregion

        #region Public methods

        /// <summary>
        /// Adds the specified code and comment.
        /// </summary>
        [NotNull]
        public IrbisMenuFile Add
            (
                [NotNull] string code,
                [CanBeNull] string comment
            )
        {
            Code.NotNull(code, "code");

            Entry entry = new Entry
            {
                Code = code,
                Comment = comment
            };
            _entries.Add(entry);

            return this;
        }


        /// <summary>
        /// Trims the code.
        /// </summary>
        [NotNull]
        public static string TrimCode
            (
                [NotNull] string code
            )
        {
            Code.NotNull(code, "code");

            code = code.Trim();
            string[] parts = code.Split(' ', '-', '=', ':');
            if (parts.Length != 0)
            {
                code = parts[0];
            }

            return code;
        }

        /// <summary>
        /// Finds the entry.
        /// </summary>
        [CanBeNull]
        public Entry FindEntry
            (
                [NotNull] string code
            )
        {
            return _entries.FirstOrDefault
                (
                    entry => entry.Code.SameString(code)
                );
        }

        /// <summary>
        /// Finds the entry (case sensitive).
        /// </summary>
        [CanBeNull]
        public Entry FindEntrySensitive
            (
                [NotNull] string code
            )
        {
            return _entries.FirstOrDefault
                (
                    entry => entry.Code.SameStringSensitive(code)
                );
        }

        /// <summary>
        /// Finds the entry.
        /// </summary>
        [CanBeNull]
        public Entry GetEntry
            (
                [NotNull] string code
            )
        {
            Code.NotNull(code, "code");

            Entry candidate = FindEntry(code);
            if (candidate != null)
            {
                return candidate;
            }

            code = code.Trim();
            candidate = FindEntry(code);
            if (candidate != null)
            {
                return candidate;
            }

            code = TrimCode(code);
            candidate = FindEntry(code);
            // ReSharper disable UseNullPropagation
            if (candidate != null)
            {
                return candidate;
            }
            // ReSharper restore UseNullPropagation

            return null;
        }


        /// <summary>
        /// Finds the entry (case sensitive).
        /// </summary>
        [CanBeNull]
        public Entry GetEntrySensitive
            (
                [NotNull] string code
            )
        {
            Code.NotNull(code, "code");

            Entry candidate = FindEntrySensitive(code);
            if (candidate != null)
            {
                return candidate;
            }

            code = code.Trim();
            candidate = FindEntrySensitive(code);
            if (candidate != null)
            {
                return candidate;
            }

            code = TrimCode(code);
            candidate = FindEntrySensitive(code);
            // ReSharper disable UseNullPropagation
            if (candidate != null)
            {
                return candidate;
            }
            // ReSharper restore UseNullPropagation

            return null;
        }

        /// <summary>
        /// Finds comment by the code.
        /// </summary>
        [CanBeNull]
        public string GetString
            (
                [NotNull] string code,
                [CanBeNull] string defaultValue
            )
        {
            Code.NotNull(code, "code");

            Entry found = FindEntry(code);

            return found == null
                ? defaultValue
                : found.Comment;
        }

        /// <summary>
        /// Finds comment by the code.
        /// </summary>
        [CanBeNull]
        public string GetString
            (
                [NotNull] string code
            )
        {
            return GetString(code, null);
        }

        /// <summary>
        /// Finds comment by the code (case sensitive).
        /// </summary>
        [CanBeNull]
        public string GetStringSensitive
            (
                [NotNull] string code,
                [CanBeNull] string defaultValue
            )
        {
            Code.NotNull(code, "code");

            Entry found = FindEntrySensitive(code);

            return found == null
                ? defaultValue
                : found.Comment;
        }

        /// <summary>
        /// Finds comment by the code (case sensitive).
        /// </summary>
        [CanBeNull]
        public string GetStringSensitive
            (
                [NotNull] string code
            )
        {
            return GetStringSensitive(code, null);
        }

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        [NotNull]
        public static IrbisMenuFile ParseStream
            (
                [NotNull] TextReader reader
            )
        {
            Code.NotNull(reader, "reader");

            IrbisMenuFile result = new IrbisMenuFile();

            while (true)
            {
                string code = reader.RequireLine();
                if (string.IsNullOrEmpty(code))
                {
                    throw new FormatException();
                }
                if (code.StartsWith(StopMarker))
                {
                    break;
                }

                string comment = reader.RequireLine();
                Entry entry = new Entry
                {
                    Code = code,
                    Comment = comment
                };
                result._entries.Add(entry);

            }

            return result;
        }

        /// <summary>
        /// Parses the local file.
        /// </summary>
        [NotNull]
        public static IrbisMenuFile ParseLocalFile
            (
                [NotNull] string fileName,
                [NotNull] Encoding encoding
            )
        {
            Code.NotNullNorEmpty(fileName, "fileName");
            Code.NotNull(encoding, "encoding");

            using (StreamReader reader = new StreamReader
                    (
                        File.OpenRead(fileName),
                        encoding
                    ))
            {
                IrbisMenuFile result = ParseStream(reader);
                result.FileName = Path.GetFileName(fileName);

                return result;
            }
        }

        /// <summary>
        /// Parses the local file.
        /// </summary>
        [NotNull]
        public static IrbisMenuFile ParseLocalFile
            (
                [NotNull] string fileName
            )
        {
            return ParseLocalFile
                (
                    fileName,
                    IrbisEncoding.Ansi
                );
        }

        /// <summary>
        /// Sorts the entries.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public Entry[] SortEntries
            (
                Sort sortBy
            )
        {
            List<Entry> copy = new List<Entry>(_entries);
            switch (sortBy)
            {
                case Sort.ByCode:
                    copy = copy.OrderBy(entry => entry.Code).ToList();
                    break;
                case Sort.ByComment:
                    copy = copy.OrderBy(entry => entry.Comment).ToList();
                    break;
            }

            return copy.ToArray();
        }

        /// <summary>
        /// Builds text representation.
        /// </summary>
        public string ToText()
        {
            StringBuilder result = new StringBuilder();

            foreach (Entry entry in _entries)
            {
                result.AppendLine(entry.Code);
                result.AppendLine(entry.Comment);
            }
            result.AppendLine(StopMarker);

            return result.ToString();
        }

        #endregion

        #region IHandmadeSerializable members

        /// <summary>
        /// Restore object state from the stream.
        /// </summary>
        public void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            FileName = reader.ReadNullableString();
            reader.ReadCollection(_entries);
        }

        /// <summary>
        /// Save object state to the stream.
        /// </summary>
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            writer.WriteNullable(FileName);
            writer.WriteCollection(_entries);
        }

        #endregion

        #region Object members

        #endregion
    }
}