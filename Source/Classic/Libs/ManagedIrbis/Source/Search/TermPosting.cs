﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TermPosting.cs -- term posting
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using AM;
using AM.IO;
using AM.Runtime;

using CodeJam;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure;

using MoonSharp.Interpreter;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Search
{
    /// <summary>
    /// Ссылка на терм.
    /// </summary>
    [PublicAPI]
    [XmlRoot("term-link")]
    [MoonSharpUserData]
#if !WINMOBILE && !PocketPC
    [DebuggerDisplay("[{Mfn}] {Tag} {Occurrence} {Count} {Text}")]
#endif
    public sealed class TermPosting
        : IHandmadeSerializable,
        IVerifiable
    {
        #region Properties

        /// <summary>
        /// MFN записи с искомым термом.
        /// </summary>
        [JsonProperty("mfn")]
        [XmlAttribute("mfn")]
        public int Mfn { get; set; }

        /// <summary>
        /// Тег поля с искомым термом.
        /// </summary>
        [JsonProperty("tag")]
        [XmlAttribute("tag")]
        public int Tag { get; set; }

        /// <summary>
        /// Повторение поля.
        /// </summary>
        [JsonProperty("occurrence")]
        [XmlAttribute("occurrence")]
        public int Occurrence { get; set; }

        /// <summary>
        /// Количество повторений.
        /// </summary>
        [JsonProperty("count")]
        [XmlAttribute("count")]
        public int Count { get; set; }

        /// <summary>
        /// Результат форматирования.
        /// </summary>
        [XmlAttribute("text")]
        [JsonProperty("text")]
        [CanBeNull]
        public string Text { get; set; }

        #endregion

        #region Construction

        #endregion

        #region Private members

        private static readonly char[] _separators = { '#' };

        #endregion

        #region Public methods

        /// <summary>
        /// Clone the <see cref="TermPosting"/>.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public TermPosting Clone()
        {
            return (TermPosting) MemberwiseClone();
        }

        /// <summary>
        /// Parse server response.
        /// </summary>
        [NotNull]
        public static TermPosting[] Parse
            (
                [NotNull] ServerResponse response
            )
        {
            Code.NotNull(response, "response");

            // Example return:
            // 169#1510#1#2#Пожаровзрывобезопасность : Науч.- техн. журн. - Журнал

            List<TermPosting> result = new List<TermPosting>();

            while (true)
            {
                string line = response.GetUtfString();
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

#if !WINMOBILE && !PocketPC && !SILVERLIGHT

                string[] parts = line.Split(_separators, 5);

#else

                // TODO Implement properly

                string[] parts = line.Split(_separators);

#endif

                if (parts.Length < 4)
                {
                    break;
                }

                TermPosting item = new TermPosting
                {
                    Mfn = int.Parse(parts[0]),
                    Tag = int.Parse(parts[1]),
                    Occurrence = int.Parse(parts[2]),
                    Count = int.Parse(parts[3]),
                    Text = parts.GetItem(4)
                };
                result.Add(item);
            }

            return result.ToArray();
        }

        #endregion

        #region IHandmadeSerializable members

        /// <summary>
        /// Просим объект восстановить свое состояние из потока.
        /// </summary>
        public void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            Mfn = reader.ReadPackedInt32();
            Tag = reader.ReadPackedInt32();
            Occurrence = reader.ReadPackedInt32();
            Count = reader.ReadPackedInt32();
            Text = reader.ReadNullableString();
        }

        /// <summary>
        /// Save object state to the specified stream.
        /// </summary>
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            writer
                .WritePackedInt32(Mfn)
                .WritePackedInt32(Tag)
                .WritePackedInt32(Occurrence)
                .WritePackedInt32(Count)
                .WriteNullable(Text);
        }

        #endregion

        #region IVerifiable members

        /// <summary>
        /// Verify object state.
        /// </summary>
        public bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<TermPosting> verifier
                = new Verifier<TermPosting>(this, throwOnError);

            return verifier.Result;
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
                    "MFN={0} Tag={1} Occurrence={2} Count={3} Text=\"{4}\"",
                    Mfn,
                    Tag,
                    Occurrence,
                    Count,
                    Text
                );
        }

        #endregion
    }
}
