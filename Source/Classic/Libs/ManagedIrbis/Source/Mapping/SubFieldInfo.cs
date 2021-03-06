﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SubFieldInfo.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace ManagedIrbis.Mapping
{
    /// <summary>
    /// Информация о маппинге.
    /// </summary>
    public sealed class SubFieldInfo
    {
        #region Properties

        /// <summary>
        /// Код подполя.
        /// </summary>
        public char Code { get; set; }

        /// <summary>
        /// Повторение подполя
        /// </summary>
        public int Occurrence { get; set; }

        /// <summary>
        /// Метод-маппер.
        /// </summary>
        public Func<SubField, object> Mapper { get; set; }

        #endregion
    }
}
