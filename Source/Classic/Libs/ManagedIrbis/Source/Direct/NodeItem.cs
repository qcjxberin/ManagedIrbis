﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* NodeItem.cs
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    /// Справочник в N01/L01 является таблицей, определяющей
    /// поисковый термин. Каждый ключ переменной длины, который
    /// есть в записи, представлен в справочнике одним входом,
    /// формат которого описывает следующая структура
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
#if !WINMOBILE && !PocketPC
    [DebuggerDisplay("Length={Length}, KeyOffset={KeyOffset}, Text={Text}")]
#endif
    public sealed class NodeItem
    {
        #region Properties

        /// <summary>
        /// Длина ключа
        /// </summary>
        public short Length { get; set; }

        /// <summary>
        /// Смещение ключа от начала записи
        /// </summary>
        public short KeyOffset { get; set; }

        /// <summary>
        /// Младшее слово смещения
        /// </summary>
        public int LowOffset { get; set; }

        /// <summary>
        /// Старшее слово смещения
        /// </summary>
        public int HighOffset { get; set; }

        /// <summary>
        /// Полное смещение
        /// </summary>
        public long FullOffset
        {
            get { return unchecked ((((long) HighOffset) << 32) + LowOffset); }
        }

        /// <summary>
        /// Ссылается на лист?
        /// </summary>
        public bool RefersToLeaf
        {
            get { return (LowOffset < 0); }
        }

        /// <summary>
        /// Текстовое значение ключа
        /// </summary>
        public string Text { get; set; }

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
                    "Length: {0}, KeyOffset: {1}, "
                    + "LowOffset: {2}, HighOffset: {3}, "
                    + "FullOffset: {4}, RefersToLeaf: {5}, "
                    + "Text: {6}", 
                    Length, 
                    KeyOffset, 
                    LowOffset, 
                    HighOffset, 
                    FullOffset, 
                    RefersToLeaf,
                    Text
                );
        }

        #endregion
    }
}
