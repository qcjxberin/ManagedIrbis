﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* QualityRule.cs -- абстрактный базовый класс для правил
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AM;

using CodeJam;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure;
using ManagedIrbis.Menus;

using MoonSharp.Interpreter;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Quality
{
    /// <summary>
    /// Абстрактный базовый класс для правил.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public abstract class QualityRule
    {
        #region Properties

        ///// <summary>
        ///// Заголовок правила.
        ///// </summary>
        //public abstract string Title { get; }

        /// <summary>
        /// Затрагиваемые поля.
        /// </summary>
        [NotNull]
        public abstract string FieldSpec { get; }

        /// <summary>
        /// Клиент.
        /// </summary>
        [NotNull]
        public IrbisConnection Connection
        {
            get { return Context.Connection; }
        }

        /// <summary>
        /// Текущий контекст.
        /// </summary>
        [NotNull]
        public RuleContext Context { get; protected set; }

        /// <summary>
        /// Текущая проверяемая запись.
        /// </summary>
        [NotNull]
        public MarcRecord Record { get { return Context.Record; } }

        /// <summary>
        /// Накопленный отчёт.
        /// </summary>
        [NotNull]
        public RuleReport Report { get; protected set; }

        /// <summary>
        /// Рабочий лист.
        /// </summary>
        [CanBeNull]
        public string Worksheet
        {
            get { return Record.FM("920"); }
        }

        #endregion

        #region Private members

        /// <summary>
        /// Add detected defect.
        /// </summary>
        protected void AddDefect
            (
                string tag,
                int damage,
                string format,
                params object[] args
            )
        {
            FieldDefect defect = new FieldDefect
            {
                Field = tag,
                Damage = damage,
                Message = string.Format(format, args)
            };
            Report.Defects.Add(defect);
        }

        /// <summary>
        /// Add detected defect.
        /// </summary>
        protected void AddDefect
            (
                RecordField field,
                int damage,
                string format,
                params object[] args
            )
        {
            FieldDefect defect = new FieldDefect
            {
                Field = field.Tag,
                FieldRepeat = field.Repeat,
                Value = field.ToText(),
                Damage = damage,
                Message = string.Format(format, args)
            };
            Report.Defects.Add(defect);
        }

        /// <summary>
        /// Add detected defect.
        /// </summary>
        protected void AddDefect
            (
                RecordField field,
                SubField subfield,
                int damage,
                string format,
                params object[] args
            )
        {
            FieldDefect defect = new FieldDefect
            {
                Field = field.Tag,
                FieldRepeat = field.Repeat,
                Subfield = subfield.Code.ToString(),
                Value = subfield.Value,
                Damage = damage,
                Message = string.Format(format, args)
            };
            Report.Defects.Add(defect);
        }

        /// <summary>
        /// Add detected defect.
        /// </summary>
        protected void BeginCheck
            (
                RuleContext context
            )
        {
            Context = context;
            Report = new RuleReport();
        }

        /// <summary>
        /// Cache the menu.
        /// </summary>
        protected MenuFile CacheMenu
            (
                string name,
                MenuFile menu
            )
        {
            menu = menu ?? 
                MenuFile.ReadFromServer
                (
                    Connection,
                    new FileSpecification(IrbisPath.MasterFile, name)
                );

            return menu;
        }

        /// <summary>
        /// Check value against menu.
        /// </summary>
        protected bool CheckForMenu
            (
                MenuFile menu,
                string value
            )
        {
            if (menu == null)
            {
                return true;
            }
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
            MenuEntry entry = menu.GetEntrySensitive(value);

            return entry != null;
        }

        /// <summary>
        /// Get text at specified position in the string.
        /// </summary>
        protected static string GetTextAtPosition
            (
                string text,
                int position
            )
        {
            int length = text.Length;
            int start = Math.Max(0, position - 1);
            int stop = Math.Min(length - 1, position + 2);
            while ((start >= 0) && (text[start] == ' '))
            {
                start--;
            }
            while ((start >= 0) && (text[start] != ' '))
            {
                start--;
            }
            start = Math.Max(0, start);
            while ((stop < length) && (text[stop] == ' '))
            {
                stop++;
            }
            while ((stop < length) && (text[stop] != ' '))
            {
                stop++;
            }
            stop = Math.Min(length - 1, stop);

            return text.Substring
            (
                start,
                stop - start + 1
            ).Trim();
        }

        /// <summary>
        /// Show double whitespace.
        /// </summary>
        protected static string ShowDoubleWhiteSpace
            (
                string text
            )
        {
            int position = text.IndexOf("  ");

            return GetTextAtPosition
                (
                    text,
                    position
                );
        }

        /// <summary>
        /// Check for whitespace.
        /// </summary>
        protected void CheckWhitespace
            (
                RecordField field,
                SubField subfield
            )
        {
            string text = subfield.Value;
            
            if (string.IsNullOrEmpty(text))
            {
                AddDefect
                    (
                        field,
                        subfield,
                        1,
                        "Пустое подполе {0}^{1}",
                        field.Tag,
                        subfield.Code
                    );
                return;
            }

            if (text.StartsWith(" "))
            {
                AddDefect
                    (
                        field,
                        subfield,
                        1,
                        "Подполе {0}^{1} начинается с пробела",
                        field.Tag,
                        subfield.Code
                    );
            }

            if (text.EndsWith(" "))
            {
                AddDefect
                    (
                        field,
                        subfield,
                        1,
                        "Подполе {0}^{1} оканчивается пробелом",
                        field.Tag,
                        subfield.Code
                    );
            }

            if (text.Contains("  "))
            {
                AddDefect
                    (
                        field,
                        subfield,
                        1,
                        "Подполе {0}^{1} содержит двойной пробел: {2}",
                        field.Tag,
                        subfield.Code,
                        ShowDoubleWhiteSpace(text)
                    );
            }
        }

        /// <summary>
        /// Check for whitespace.
        /// </summary>
        protected void CheckWhitespace
            (
                RecordField field
            )
        {
            string text = field.Value;
            if (!string.IsNullOrEmpty(text))
            {
                if (text.StartsWith(" "))
                {
                    AddDefect
                        (
                            field,
                            1,
                            "Поле {0} начинается с пробела",
                            field.Tag
                        );
                }
                if (text.EndsWith(" "))
                {
                    AddDefect
                        (
                            field,
                            1,
                            "Поле {0} оканчивается пробелом",
                            field.Tag
                        );
                }
                if (text.Contains("  "))
                {
                    AddDefect
                        (
                            field,
                            1,
                            "Поле {0} содержит двойной пробел: {1}",
                            field.Tag,
                            ShowDoubleWhiteSpace(text)
                        );
                }
            }

            foreach (SubField subfield in field.SubFields)
            {
                CheckWhitespace
                    (
                        field,
                        subfield
                    );
            }
        }

        /// <summary>
        /// End the checking.
        /// </summary>
        protected RuleReport EndCheck()
        {
            Report.Damage = Report.Defects
                .Sum(defect => defect.Damage);

            return Report;
        }

        /// <summary>
        /// ASP?
        /// </summary>
        protected bool IsAsp()
        {
            return Worksheet.SameString("ASP");
        }

        /// <summary>
        /// PAZK, SPEC or PVK?
        /// </summary>
        protected bool IsBook()
        {
            string worksheet = Worksheet;
            return (worksheet.SameString("PAZK")
                    || worksheet.SameString("SPEC")
                    || worksheet.SameString("PVK"));
        }

        /// <summary>
        /// PAZK?
        /// </summary>
        protected bool IsPazk()
        {
            return Worksheet.SameString("PAZK");
        }

        /// <summary>
        /// SPEC?
        /// </summary>
        protected bool IsSpec()
        {
            return Worksheet.SameString("SPEC");
        }

        /// <summary>
        /// Get fields for the rule.
        /// </summary>
        protected RecordField[] GetFields()
        {
            return Record.Fields
                .GetFieldBySpec(FieldSpec);
        }

        /// <summary>
        /// Must not contain subfields.
        /// </summary>
        protected void MustNotContainSubfields
            (
                RecordField field
            )
        {
            if (field.SubFields.Count != 0)
            {
                AddDefect
                    (
                        field,
                        20,
                        "Поле {0} содержит подполя",
                        field.Tag
                    );
            }
        }

        /// <summary>
        /// Must not contain plain text value.
        /// </summary>
        protected void MustNotContainText
            (
                RecordField field
            )
        {
            if (!string.IsNullOrEmpty(field.Value))
            {
                AddDefect
                    (
                        field,
                        20,
                        "Поле {0} должно состоять только из подполей",
                        field.Tag
                    );
            }
        }

        /// <summary>
        /// Must not repeat subfields.
        /// </summary>
        protected void MustNotRepeatSubfields
            (
                RecordField field
            )
        {
            var grouped = field.SubFields
                .GroupBy(sf => sf.CodeString.ToLowerInvariant());
            foreach (var grp in grouped)
            {
                if (grp.Count() != 1)
                {
                    AddDefect
                        (
                            field,
                            20,
                            "Подполе {0}^{1} повторяется",
                            field.Tag,
                            grp.Key
                        );
                }
            }
        }

        /// <summary>
        /// Must be unique field.
        /// </summary>
        protected void MustBeUniqueField
            (
                RecordField[] fields
            )
        {
            var grouped = fields
                .GroupBy(f => f.Value.ToLowerInvariant());
            foreach (var grp in grouped)
            {
                if (grp.Count() != 1)
                {
                    AddDefect
                        (
                            grp.First(),
                            20,
                            "Поле {0} содержит повторяющееся значение {1}",
                            grp.First().Tag,
                            grp.Key
                        );
                }
            }
        }

        /// <summary>
        /// Must be non-emptu subfield.
        /// </summary>
        protected void MustBeNonEmptySubfield
            (
                RecordField field,
                char code
            )
        {
            var selected = field.SubFields
                .GetSubField(new[] {code})
                .Where(sf => string.IsNullOrEmpty(sf.Value));
            foreach (SubField subField in selected)
            {
                AddDefect
                    (
                        field,
                        subField,
                        5,
                        "Подполе {0}^{1} пустое",
                        field.Tag,
                        subField.Code
                    );
            }
        }

        /// <summary>
        /// Must be unique subfield.
        /// </summary>
        protected void MustBeUniqueSubfield
            (
                RecordField[] fields,
                char code
            )
        {
            var grouped = fields
                .SelectMany(f => f.SubFields)
                .GetSubField(new[] {code})
                .GroupBy(sf => sf.Value.ToLowerInvariant());
            foreach (var grp in grouped)
            {
                if (grp.Count() != 1)
                {
                    AddDefect
                        (
                            fields[0],
                            grp.First(),
                            5,
                            "Подполе {0}^{1} содержит неуникальное значение {2}",
                            fields[0].Tag,
                            grp.First().Code,
                            grp.Key
                        );
                }
            }
        }

        /// <summary>
        /// Must be unique subfield.
        /// </summary>
        protected void MustBeUniqueSubfield
            (
                RecordField[] fields,
                params char[] codes
            )
        {
            foreach (char code in codes)
            {
                MustBeUniqueSubfield
                    (
                        fields,
                        code
                    );
            }
        }

        /// <summary>
        /// Must not contain whitespace.
        /// </summary>
        protected void MustNotContainWhitespace
            (
                RecordField field
            )
        {
            string text = field.Value;
            if (!string.IsNullOrEmpty(text)
                && text.ContainsWhitespace())
            {
                AddDefect
                    (
                        field,
                        3,
                        "Поле {0} содержит пробельные символы",
                        field.Tag
                    );
            }
        }

        /// <summary>
        /// Must not contain whitespace.
        /// </summary>
        protected void MustNotContainWhitespace
            (
                RecordField field,
                SubField subField
            )
        {
            string text = subField.Value;
            if (!string.IsNullOrEmpty(text)
                && text.ContainsWhitespace())
            {
                AddDefect
                    (
                        field,
                        subField,
                        3,
                        "Подполе {0}^{1} содержит пробельные символы",
                        field.Tag,
                        subField.Code
                    );
            }
        }

        /// <summary>
        /// Must not contain whitespace.
        /// </summary>
        protected void MustNotContainWhitespace
            (
                RecordField field,
                params char[] codes
            )
        {
            foreach (char code in codes)
            {
                SubField[] subFields = field.GetSubField(code);
                foreach (SubField subField in subFields)
                {
                    MustNotContainWhitespace
                        (
                            field,
                            subField
                        );
                }
            }
        }

        /// <summary>
        /// Must not contain bad characters.
        /// </summary>
        protected void MustNotContainBadCharacters
            (
                RecordField field
            )
        {
            string text = field.Value;
            if (!string.IsNullOrEmpty(text))
            {
                int position = RuleUtility.BadCharacterPosition(text);
                if (position >= 0)
                {
                    AddDefect
                        (
                            field,
                            3,
                            "Поле {0} содержит запрещённые символы: {1}",
                            GetTextAtPosition(text, position)
                        );
                }
            }
        }

        /// <summary>
        /// Must not contain bad characters.
        /// </summary>
        protected void MustNotContainBadCharacters
            (
                RecordField field,
                SubField subField
            )
        {
            string text = subField.Value;
            if (!string.IsNullOrEmpty(text))
            {
                int position = RuleUtility.BadCharacterPosition(text);
                if (position >= 0)
                {
                    AddDefect
                        (
                            field,
                            subField,
                            3,
                            "Подполе {0}^{1} содержит запрещённые символы: {2}",
                            field.Tag,
                            subField.Code,
                            GetTextAtPosition(text, position)
                        );
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Проверка записи.
        /// </summary>
        [NotNull]
        public abstract RuleReport CheckRecord
            (
                [NotNull] RuleContext context
            );

        #endregion
    }
}
