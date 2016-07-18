﻿/* DebtorManager.cs -- работа с задолжниками
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Readers
{
    /// <summary>
    /// Работа с задолжниками
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public sealed class DebtorManager
    {
        #region Properties

        /// <summary>
        /// Клиент.
        /// </summary>
        public IrbisConnection Client { get; private set; }

        /// <summary>
        /// Кафедра обслуживания.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// С какой даты задолженность?
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// По какую дату задолженность.
        /// </summary>
        public DateTime? ToDate { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Конструктор
        /// </summary>
        public DebtorManager
            (
                [NotNull] IrbisConnection client
            )
        {
            if (ReferenceEquals(client, null))
            {
                throw new ArgumentNullException("client");
            }

            Client = client;
        }

        #endregion

        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Получение списка задолжников.
        /// </summary>
        public DebtorInfo[] GetDebtors()
        {
            return new DebtorInfo[0];
        }

        #endregion

        #region Object members

        #endregion
    }
}