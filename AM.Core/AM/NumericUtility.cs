﻿/* NumericUtility.cs --
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Class NumericUtility.
    /// </summary>
    [PublicAPI]
    public static class NumericUtility
    {
        #region Public methods

        /// <summary>
        /// Представляет ли строка положительное целое число.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsPositiveInteger
            (
                this string text
            )
        {
            return (text.SafeParseInt32(0) > 0);
        }

        /// <summary>
        /// Безопасный парсинг целого числа.
        /// </summary>
        /// <param name="text">Строка, подлежащая парсингу.</param>
        /// <param name="defaultValue">Значение по умолчанию.</param>
        /// <returns>Разобранное целое число или значение по умолчанию.</returns>
        public static int SafeParseInt32
            (
                this string text,
                int defaultValue
            )
        {
            int result = defaultValue;

            try
            {
                result = int.Parse(text);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
                // Do nothing
            }

            //if (!Int32.TryParse(text, out result))
            //{
            //    result = defaultValue;
            //}
            return result;
        }

        /// <summary>
        /// Безопасный парсинг целого числа.
        /// </summary>
        /// <param name="text">Строка, подлежащая парсингу.</param>
        /// <returns>Разобранное целое число или значение по умолчанию.</returns>
        public static int SafeParseInt32
            (
                this string text
            )
        {
            return SafeParseInt32
                (
                    text,
                    0
                 );
        }

        /// <summary>
        /// Преобразование числа в строку по правилам инвариантной 
        /// (не зависящей от региона) культуры.
        /// </summary>
        /// <param name="value">Число для преобразования.</param>
        /// <returns>Строковое представление числа.</returns>
        public static string ToInvariantString
            (
                this int value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToInvariantString
            (
                this char value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Форматирование диапазона целых чисел.
        /// </summary>
        /// <remarks>Границы диапазона могут совпадать, однако
        /// левая не должна превышать правую.</remarks>
        /// <param name="first">Левая граница диапазона.</param>
        /// <param name="last">Правая граница диапазона.</param>
        /// <returns>Строковое представление диапазона.</returns>
        public static string FormatRange
            (
                int first,
                int last
            )
        {
            if (first == last)
            {
                return first.ToInvariantString();
            }
            if (first == (last - 1))
            {
                return (first.ToInvariantString() + ", " + last.ToInvariantString());
            }
            return (first.ToInvariantString() + "-" + last.ToInvariantString());
        }

        /// <summary>
        /// Преобразование набора целых чисел в строковое представление,
        /// учитывающее возможное наличие цепочек последовательных чисел,
        /// которые форматируются как диапазоны.
        /// </summary>
        /// <param name="n">Источник целых чисел.</param>
        /// <remarks>Источник должен поддерживать многократное считывание.
        /// Числа предполагаются предварительно упорядоченные. Повторения чисел
        /// не допускаются. Пропуски в последовательностях допустимы.
        /// Числа допускаются только неотрицательные.
        /// </remarks>
        /// <returns>Строковое представление набора чисел.</returns>
        public static string CompressRange
            (
                IEnumerable<int> n
            )
        {
            if (n == null)
            {
                return string.Empty;
            }

            // ReSharper disable PossibleMultipleEnumeration
            if (!n.Any())
            {
                return String.Empty;
            }

            var result = new StringBuilder();
            var first = true;
            var previous = n.First();
            var last = previous;
            foreach (var i in n.Skip(1))
            {
                if (i != (last + 1))
                {
                    result.AppendFormat("{0}{1}", (first ? "" : ", "),
                        FormatRange(previous, last));
                    previous = i;
                    first = false;
                }
                last = i;
            }
            result.AppendFormat("{0}{1}", (first ? "" : ", "),
                FormatRange(previous, last));

            return result.ToString();
            // ReSharper restore PossibleMultipleEnumeration
        }

        #endregion
    }
}
