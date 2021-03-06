﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* FoundItem.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using AM;
using AM.IO;
using AM.Runtime;

using CodeJam;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure;

using MoonSharp.Interpreter;

#endregion

namespace ManagedIrbis.Search
{
    /// <summary>
    /// Found item.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
#if !WINMOBILE && !PocketPC
    [DebuggerDisplay("{Mfn} {Text}")]
#endif
    public sealed class FoundItem
        : IHandmadeSerializable,
        IVerifiable
    {
        #region Constants

        /// <summary>
        /// Delimiter.
        /// </summary>
        public const char Delimiter = '#';

        #endregion

        #region Properties

        /// <summary>
        /// Text.
        /// </summary>
        [CanBeNull]
        public string Text { get; set; }

        /// <summary>
        /// MFN.
        /// </summary>
        public int Mfn { get; set; }

        #endregion

        #region Construction

        #endregion

        #region Private members

        // ReSharper disable InconsistentNaming
        private static readonly char[] _delimiters = {Delimiter};
        // ReSharper restore InconsistentNaming

        #endregion

        #region Public methods

        /// <summary>
        /// Convert to MFN array.
        /// </summary>
        [NotNull]
        public static int[] ConvertToMfn
            (
                [NotNull][ItemNotNull] List<FoundItem> found
            )
        {
            Code.NotNull(found, "found");

            int[] result = new int[found.Count];
            for (int i = 0; i < found.Count; i++)
            {
                result[i] = found[i].Mfn;
            }

            return result;
        }

        /// <summary>
        /// Convert to string array.
        /// </summary>
        [NotNull]
        [ItemCanBeNull]
        public static string[] ConvertToText
            (
                [NotNull][ItemNotNull] List<FoundItem> found
            )
        {
            Code.NotNull(found, "found");

            string[] result = new string[found.Count];
            for (int i = 0; i < found.Count; i++)
            {
                result[i] = found[i].Text.EmptyToNull();
            }

            return result;
        }

        /// <summary>
        /// Parse text line.
        /// </summary>
        [NotNull]
        public static FoundItem ParseLine
            (
                [NotNull] string line
            )
        {
            Code.NotNull(line, "line");

#if !WINMOBILE && !PocketPC && !SILVERLIGHT

            string[] parts = line.Split(_delimiters, 2);

#else

            // TODO Implement properly

            string[] parts = line.Split(_delimiters);

#endif

            FoundItem result = new FoundItem
            {
                Mfn = int.Parse(parts[0])
            };
            if (parts.Length > 1)
            {
                string text = parts[1].EmptyToNull();
                text = IrbisText.IrbisToWindows(text);
                result.Text = text;
            }

            return result;
        }

        /// <summary>
        /// Parse server response.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static List<FoundItem> ParseServerResponse
            (
                [NotNull] ServerResponse response,
                int sizeHint
            )
        {
            Code.NotNull(response, "response");

            List<FoundItem> result = (sizeHint > 0)
                ? new List<FoundItem>(sizeHint)
                : new List<FoundItem>();

            string line;
            while ((line = response.GetUtfString()) != null)
            {
                FoundItem item = ParseLine(line);
                result.Add(item);
            }

            return result;
        }

        #endregion

        #region IHandmadeSerializable members

        /// <summary>
        /// Restore the object state from the specified stream.
        /// </summary>
        public void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            Code.NotNull(reader, "reader");

            Mfn = reader.ReadPackedInt32();
            Text = reader.ReadNullableString();
        }

        /// <summary>
        /// Save the object state to the specified stream.
        /// </summary>
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            Code.NotNull(writer, "writer");

            writer
                .WritePackedInt32(Mfn)
                .WriteNullable(Text);
        }

        #endregion

        #region IVerifiable members

        /// <summary>
        /// Verify the object state.
        /// </summary>
        public bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<FoundItem> verifier = new Verifier<FoundItem>
                (
                    this,
                    throwOnError
                );

            verifier
                .Assert(Mfn > 0, "Mfn > 0")
                .NotNullNorEmpty(Text, "Text");

            return verifier.Result;
        }

        #endregion

        #region Object members

        /// <summary>
        /// Returns a <see cref="System.String" />
        /// that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return string.Format
                (
                    "[{0}] {1}",
                    Mfn,
                    Text.NullableToVisibleString()
                );
        }

        #endregion
    }
}
