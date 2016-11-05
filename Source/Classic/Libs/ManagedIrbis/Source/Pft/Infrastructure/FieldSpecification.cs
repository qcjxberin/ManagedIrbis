﻿/* FieldSpecification.cs -- спецификация поля/подполя
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Diagnostics;
using System.Globalization;
using System.Text;

using AM;
using AM.Text;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

#endregion

namespace ManagedIrbis.Pft.Infrastructure
{
    /// <summary>
    /// Спецификация поля/подполя.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    [DebuggerDisplay("{Command}{Tag}")]
    public sealed class FieldSpecification
    {
        #region Properties

        /// <summary>
        /// Command.
        /// </summary>
        public char Command { get; set; }

        /// <summary>
        /// Embedded.
        /// </summary>
        [CanBeNull]
        public string Embedded { get; set; }

        /// <summary>
        /// Отступ.
        /// </summary>
        public int Indent { get; set; }

        /// <summary>
        /// Начальный номер повторения.
        /// </summary>
        public int IndexFrom { get; set; }

        /// <summary>
        /// Конечный номер повторения.
        /// </summary>
        public int IndexTo { get; set; }

        /// <summary>
        /// Смещение.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Длина.
        /// </summary>
        public int Length { get; set; }


        /// <summary>
        /// Subfield.
        /// </summary>
        public char SubField { get; set; }

        /// <summary>
        /// Tag.
        /// </summary>
        [CanBeNull]
        public string Tag { get; set; }

        /// <summary>
        /// Сырое представление.
        /// </summary>
        [CanBeNull]
        public string Text { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Parse the specification from text.
        /// </summary>
        public bool Parse
            (
                [NotNull] string text
            )
        {
            Code.NotNullNorEmpty(text, "text");

            TextNavigator navigator = new TextNavigator(text);
            return Parse(navigator);
        }

        /// <summary>
        /// Parse the specification from navigator.
        /// </summary>
        public bool Parse
            (
                [NotNull] TextNavigator navigator
            )
        {
            Code.NotNull(navigator, "navigator");

            int start = navigator.Position;
            TextPosition saved = navigator.SavePosition();
            char c = navigator.ReadChar();
            StringBuilder builder = new StringBuilder();

            switch (c)
            {
                case 'd':
                case 'D':
                    Command = 'd';
                    break;

                case 'g':
                case 'G':
                    Command = 'g';
                    break;

                case 'n':
                case 'N':
                    Command = 'n';
                    break;

                case 'v':
                case 'V':
                    Command = 'v';
                    break;

                default:
                    navigator.RestorePosition(saved);
                    return false;
            } // switch

            c = navigator.ReadChar();
            if (!c.IsArabicDigit())
            {
                return false;
            }
            builder.Append(c);

            while (true)
            {
                c = navigator.PeekChar();
                if (!c.IsArabicDigit())
                {
                    break;
                }
                navigator.ReadChar();
                builder.Append(c);
            }
            Tag = builder.ToString();

            // now c is peeked char

            if (c == '@')
            {
                builder.Length = 0;
                navigator.ReadChar();

                while (true)
                {
                    c = navigator.PeekChar();
                    if (!c.IsArabicDigit())
                    {
                        break;
                    }
                    navigator.ReadChar();
                    builder.Append(c);
                }
                
                if (builder.Length == 0)
                {
                    throw new PftSyntaxException(navigator);
                }
                Embedded = builder.ToString();
            } // c == '@'

            // c still is peeked char

            if (c == '^')
            {
                navigator.ReadChar();
                if (navigator.IsEOF)
                {
                    throw new PftSyntaxException(navigator);
                }
                c = navigator.ReadChar();
                if (!SubFieldCode.IsValidCode(c))
                {
                    throw new PftSyntaxException(navigator);
                }
                SubField = SubFieldCode.Normalize(c);

                c = navigator.PeekChar();
            } // c == '^'

            if (Command != 'v')
            {
                goto DONE;
            }

            if (c == '[')
            {
                navigator.ReadChar();
                navigator.SkipWhitespace();

                string index;

                if (navigator.PeekChar() == '.')
                {
                    navigator.ReadChar();
                    if (navigator.ReadChar() != '.')
                    {
                        throw new PftSyntaxException(navigator);
                    }
                    navigator.SkipWhitespace();
                    index = navigator.ReadInteger();
                    if (string.IsNullOrEmpty(index))
                    {
                        throw new PftSyntaxException(navigator);
                    }
                    int indexTo = int.Parse
                        (
                            index,
                            CultureInfo.InvariantCulture
                        );
                    IndexTo = indexTo;
                }
                else
                {
                    index = navigator.ReadInteger();
                    if (string.IsNullOrEmpty(index))
                    {
                        throw new PftSyntaxException(navigator);
                    }
                    int indexFrom = int.Parse
                        (
                            index,
                            CultureInfo.InvariantCulture
                        );
                    IndexFrom = indexFrom;
                    IndexTo = indexFrom;
                }

                navigator.SkipWhitespace();
                if (navigator.SkipChar('.'))
                {
                    if (navigator.ReadChar() != '.')
                    {
                        throw new PftSyntaxException(navigator);
                    }
                    navigator.SkipWhitespace();
                    index = navigator.ReadInteger();
                    if (string.IsNullOrEmpty(index))
                    {
                        IndexTo = 0;
                    }
                    else
                    {
                        int indexTo = int.Parse
                            (
                                index,
                                CultureInfo.InvariantCulture
                            );
                        IndexTo = indexTo;
                    }
                }

                navigator.SkipWhitespace();
                if (navigator.ReadChar() != ']')
                {
                    throw new PftSyntaxException(navigator);
                }

                c = navigator.PeekChar();
            } // c == '['

            // c still is peeked char

            if (c == '*')
            {
                navigator.ReadChar();
                builder.Length = 0;

                while (true)
                {
                    c = navigator.PeekChar();
                    if (!c.IsArabicDigit())
                    {
                        break;
                    }
                    navigator.ReadChar();
                    builder.Append(c);
                }

                if (builder.Length == 0)
                {
                    throw new PftSyntaxException(navigator);
                }

                Offset = int.Parse
                    (
                        builder.ToString(),
                        CultureInfo.InvariantCulture
                    );
            } // c == '*'

            if (c == '.')
            {
                navigator.ReadChar();
                builder.Length = 0;

                while (true)
                {
                    c = navigator.PeekChar();
                    if (!c.IsArabicDigit())
                    {
                        break;
                    }
                    navigator.ReadChar();
                    builder.Append(c);
                }

                if (builder.Length == 0)
                {
                    throw new PftSyntaxException(navigator);
                }

                Length = int.Parse
                    (
                        builder.ToString(),
                        CultureInfo.InvariantCulture
                    );

                if (navigator.PeekChar() == '*')
                {
                    throw new PftSyntaxException(navigator);
                }
            } // c == '.'

            if (c == '(')
            {
                navigator.ReadChar();
                builder.Length = 0;

                while (true)
                {
                    c = navigator.PeekChar();
                    if (c == ')')
                    {
                        navigator.ReadChar();
                        c = navigator.PeekChar();
                        break;
                    }
                    if (!c.IsArabicDigit())
                    {
                        throw new PftSyntaxException(navigator);
                    }
                    navigator.ReadChar();
                    builder.Append(c);
                }

                if (builder.Length == 0)
                {
                    throw new PftSyntaxException(navigator);
                }
                Indent = int.Parse
                    (
                        builder.ToString(),
                        CultureInfo.InvariantCulture
                    );
            } // c == '('

            // c still is peeked char

            if (c == '#')
            {
                navigator.ReadChar();

                string index = navigator.ReadInteger();
                if (string.IsNullOrEmpty(index))
                {
                    throw new PftSyntaxException(navigator);
                }
                int indexFrom = int.Parse(index);
                IndexFrom = indexFrom;
                IndexTo = indexFrom;
            }

            DONE:

            int length = navigator.Position - start;
            Text = navigator.Substring(start, length);

            return true;
        }

        /// <summary>
        /// Parse the specification from text.
        /// </summary>
        public bool ParseShort
            (
                [NotNull] string text
            )
        {
            Code.NotNullNorEmpty(text, "text");

            TextNavigator navigator = new TextNavigator(text);
            return ParseShort(navigator);
        }

        /// <summary>
        /// Parse short specification from navigator.
        /// </summary>
        public bool ParseShort
            (
                [NotNull] TextNavigator navigator
            )
        {
            Code.NotNull(navigator, "navigator");

            int start = navigator.Position;
            TextPosition saved = navigator.SavePosition();
            char c = navigator.ReadChar();
            StringBuilder builder = new StringBuilder();

            switch (c)
            {
                case 'g':
                case 'G':
                    Command = 'g';
                    break;

                case 'v':
                case 'V':
                    Command = 'v';
                    break;

                default:
                    navigator.RestorePosition(saved);
                    return false;
            } // switch

            c = navigator.ReadChar();
            if (!c.IsArabicDigit())
            {
                return false;
            }
            builder.Append(c);

            while (true)
            {
                c = navigator.PeekChar();
                if (!c.IsArabicDigit())
                {
                    break;
                }
                navigator.ReadChar();
                builder.Append(c);
            }
            Tag = builder.ToString();

            // now c is peeked char

            if (c == '^')
            {
                navigator.ReadChar();
                if (navigator.IsEOF)
                {
                    throw new PftSyntaxException(navigator);
                }
                c = navigator.ReadChar();
                if (!SubFieldCode.IsValidCode(c))
                {
                    throw new PftSyntaxException(navigator);
                }
                SubField = SubFieldCode.Normalize(c);

                c = navigator.PeekChar();
            } // c == '^'

            int length = navigator.Position - start;
            Text = navigator.Substring(start, length);

            return true;
        }

        #endregion

        #region Object members

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.Append(Command);
            result.Append(Tag);
            if (!string.IsNullOrEmpty(Embedded))
            {
                result.Append('@');
                result.Append(Embedded);
            }
            if (SubField != '\0')
            {
                result.Append('^');
                result.Append(SubField);
            }
            if (IndexFrom != 0 || IndexTo != 0)
            {
                result.Append('[');
                if (IndexFrom == IndexTo)
                {
                    result.Append(IndexFrom);
                }
                else
                {
                    if (IndexFrom != 0)
                    {
                        result.Append(IndexFrom);
                    }
                    result.Append('-');
                    if (IndexTo != 0)
                    {
                        result.Append(IndexTo);
                    }
                }
                result.Append(']');
            }
            if (Offset != 0)
            {
                result.Append('*');
                result.Append(Offset);
            }
            if (Length != 0)
            {
                result.Append('.');
                result.Append(Length);
            }
            if (Indent != 0)
            {
                result.Append('(');
                result.Append(Indent);
                result.Append(')');
            }

            return result.ToString();
        }

        #endregion
    }
}
