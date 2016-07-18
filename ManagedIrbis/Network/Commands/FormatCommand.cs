﻿/* FormatCommand.cs -- format records on IRBIS-server
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AM;
using AM.Text;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Network.Commands
{
    //
    // db_name – имя базы данных
    // MFN – номер записи в базе данных db_name
    // format – есть 5 вариантов определить формат:
    // 1-й вариант  – строка формата;
    // 2-й вариант – имя файла формата расположенного
    // на сервере по 10 пути для базы данных db_name,
    // предваряемого символом @ (например @brief);
    // 3-й вариант – символ @ - в этом случае производится
    // ОПТИМИЗИРОВАННОЕ форматирование,
    // имя формата определяется видом записи;
    // 4-й вариант – символ * - в этом случае производится
    // форматирование как ВЫБОР ПОЛЯ, соответствующего
    // 1-й ссылке каждого термина (например для ссылки
    // в виде 1.200.2.3 берется 2-е[осс] повторение
    // 200-го[метка] поля).
    // 5-й вариант – пустая строка. В этом случае
    // возвращается только список терминов.
    // При любом варианте перед форматированием сервер
    // проделывает следующую операцию - в любом формате
    // специальное сочетание символов вида *** (3 звездочки)
    // заменяется на значение метки поля, взятого
    // из 1-й ссылки для данного термина (например,
    // для ссылки 1.200.1.1 формат вида v***  будет заменен
    // на v200).
    //


    /// <summary>
    /// Format records on IRBIS-server.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public sealed class FormatCommand
        : AbstractCommand
    {
        #region Properties

        /// <summary>
        /// Database name.
        /// </summary>
        [CanBeNull]
        public string Database { get; set; }

        /// <summary>
        /// Format specification.
        /// </summary>
        [CanBeNull]
        public string FormatSpecification { get; set; }

        /// <summary>
        /// List of MFNs to format.
        /// </summary>
        [NotNull]
        public List<int> MfnList { get; private set; }

        /// <summary>
        /// Virtual record to format.
        /// </summary>
        [CanBeNull]
        public MarcRecord VirtualRecord { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public FormatCommand
            (
                [NotNull] IrbisConnection connection
            )
            : base(connection)
        {
            MfnList = new List<int>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get format result from server response.
        /// </summary>
        [NotNull]
        public static string[] GetFormatResult
            (
                [NotNull] IrbisServerResponse response
            )
        {
            Code.NotNull(response, "response");

            List<string> result = new List<string>();

            while (true)
            {
                string line = response.GetUtfString();
                if (ReferenceEquals(line, null))
                {
                    break;
                }
                int index = line.IndexOf('#');
                if (index > 0)
                {
                    string mfnPart = line.Substring(0, index);
                    int mfn = mfnPart.SafeToInt32();
                    if (mfn > 0)
                    {
                        line = line.Substring(index + 1);
                    }
                }

                line = IrbisText.IrbisToWindows(line);
                result.Add(line);
            }

            return result.ToArray();
        }

        #endregion

        #region AbstractCommand members

        /// <summary>
        /// Create client query.
        /// </summary>
        public override IrbisClientQuery CreateQuery()
        {
            IrbisClientQuery result = base.CreateQuery();
            result.CommandCode = CommandCode.FormatRecord;

            return result;
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        public override IrbisServerResponse Execute
            (
                IrbisClientQuery query
            )
        {
            Code.NotNull(query, "query");

            string database = Database ?? Connection.Database;
            query.Add(database);

            string preparedFormat = IrbisFormat.PrepareFormat
                (
                    FormatSpecification
                );

            query.Add
                (
                    new TextWithEncoding
                        (
                            preparedFormat,
                            IrbisEncoding.Ansi
                        )
                );

            if (MfnList.Count == 0)
            {
                query.Add(-2);
                query.Add(VirtualRecord);
            }
            else
            {
                query.Add(MfnList.Count);
                foreach (int mfn in MfnList)
                {
                    query.Add(mfn);
                }
            }

            IrbisServerResponse result = base.Execute(query);

            return result;
        }

        #endregion

        #region IVerifiable members

        /// <summary>
        /// Verify object state.
        /// </summary>
        public override bool Verify
            (
                bool throwOnError
            )
        {
            bool result =
                !string.IsNullOrEmpty(FormatSpecification);

            if (result)
            {
                result = !ReferenceEquals(VirtualRecord, null)
                    || (MfnList.Count > 0);
            }

            if (result)
            {
                result = base.Verify(throwOnError);
            }

            if (!result && throwOnError)
            {
                throw new VerificationException();
            }

            return result;
        }

        #endregion
    }
}