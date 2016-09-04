﻿/* Utility.cs -- bunch of useful routines.
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

#endregion

namespace AM
{
    /// <summary>
    /// Bunch of useful routines.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public static class Utility
    {
        #region Private members

        #endregion

        #region Public methods

        // =========================================================

#if !NETCORE

        /// <summary>
        /// Compare two sequences.
        /// </summary>
        /// <remarks>Borrowed from StackOverflow:
        /// http://stackoverflow.com/questions/1680602/what-is-the-algorithm-used-by-the-memberwise-equality-test-in-net-structs
        /// </remarks>
        public static bool EnumerableEquals
            (
                [CanBeNull] IEnumerable left,
                [CanBeNull] IEnumerable right
            )
        {
            if (ReferenceEquals(left, null)
                || ReferenceEquals(right, null))
            {
                return false;
            }
            if (ReferenceEquals(left, right))
            {
                return false;
            }

            IEnumerator rightEnumerator = right.GetEnumerator();
            rightEnumerator.Reset();

            foreach (object leftItem in left)
            {
                // unequal amount of items
                if (!rightEnumerator.MoveNext())
                {
                    return false;
                }
                {
                    if (!MemberwiseEquals
                    (
                        leftItem,
                        rightEnumerator.Current
                    ))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Implementation of a memberwise comparison
        /// for objects.
        /// </summary>
        /// <remarks>Borrowed from StackOverflow:
        /// http://stackoverflow.com/questions/1680602/what-is-the-algorithm-used-by-the-memberwise-equality-test-in-net-structs
        /// </remarks>
        public static bool MemberwiseEquals
            (
                [CanBeNull] object left,
                [CanBeNull] object right
            )
        {
            if (ReferenceEquals(left, null)
                || ReferenceEquals(right, null))
            {
                return false;
            }
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            Type type = left.GetType();
            if (type != right.GetType())
            {
                return false;
            }
            if (type.IsValueType)
            {
                return left.Equals(right);
            }
            if (type == type.GetMethod("Equals").DeclaringType)
            {
                return left.Equals(right);
            }

            IEnumerable leftEnumerable = left as IEnumerable;
            IEnumerable rightEnumerable = right as IEnumerable;
            if (!ReferenceEquals(leftEnumerable, null))
            {
                return EnumerableEquals
                    (
                        leftEnumerable,
                        rightEnumerable
                    );
            }

            // compare each property
            foreach (PropertyInfo info in type.GetProperties
                (
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.GetProperty
                ))
            {
                // TODO: need to special-case indexable properties
                if (!MemberwiseEquals
                    (
                        info.GetValue(left, null),
                        info.GetValue(right, null)
                    ))
                {
                    return false;
                }
            }

            // compare each field
            foreach (FieldInfo info in type.GetFields
                (
                    BindingFlags.GetField |
                    BindingFlags.NonPublic |
                    BindingFlags.Public |
                    BindingFlags.Instance
                ))
            {
                if (!MemberwiseEquals
                    (
                        info.GetValue(left),
                        info.GetValue(right))
                )
                {
                    return false;
                }
            }

            return true;
        }

#endif

        // =========================================================

        /// <summary>
        /// Выборка элемента из массива.
        /// </summary>
        public static T GetItem<T>
            (
                [NotNull] this T[] array,
                int index,
                [CanBeNull] T defaultValue
            )
        {
            Code.NotNull(array, "array");

            index = (index >= 0)
                ? index
                : array.Length + index;
            T result = ((index >= 0) && (index < array.Length))
                ? array[index]
                : defaultValue;

            return result;
        }

        /// <summary>
        /// Выборка элемента из массива.
        /// </summary>
        public static T GetItem<T>
            (
                [NotNull] this T[] array,
                int index
            )
        {
            return GetItem(array, index, default(T));
        }

        /// <summary>
        /// Выборка элемента из списка.
        /// </summary>
        public static T GetItem<T>
            (
                [NotNull] this IList<T> list,
                int index,
                [CanBeNull] T defaultValue
            )
        {
            Code.NotNull(list, "list");

            index = (index >= 0)
                ? index
                : list.Count + index;
            T result = ((index >= 0) && (index < list.Count))
                ? list[index]
                : defaultValue;

            return result;
        }

        /// <summary>
        /// Выборка элемента из массива.
        /// </summary>
        public static T GetItem<T>
            (
                [NotNull] this IList<T> list,
                int index
            )
        {
            return GetItem(list, index, default(T));
        }

        /// <summary>
        /// Determines whether is one of the specified values.
        /// </summary>
        public static bool IsOneOf<T>
            (
                T value,
                params T[] array
            )
            where T : IComparable<T>
        {
            foreach (T one in array)
            {
                if (value.CompareTo(one) == 0)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Determines whether given object
        /// is default value.
        /// </summary>
        public static bool NotDefault<T>
            (
                this T obj
            )
        {
            return !EqualityComparer<T>.Default.Equals
                (
                    obj,
                    default(T)
                );
        }

        /// <summary>
        /// Returns given value instead of
        /// default(T) if happens.
        /// </summary>
        public static T NotDefault<T>
            (
                this T obj,
                T value
            )
        {
            return EqualityComparer<T>.Default.Equals
                (
                    obj,
                    default(T)
                )
                ? value
                : obj;
        }

        /// <summary>
        /// Преобразование любого значения в строку.
        /// </summary>
        /// <returns>Для <c>null</c> возвращается <c>null</c>.
        /// </returns>
        [CanBeNull]
        public static string NullableToString<T>
            (
                [CanBeNull] this T value
            )
            where T : class
        {
            return ReferenceEquals(value, null)
                ? null
                : value.ToString();
        }

        /// <summary>
        /// Преобразование любого значения в строку.
        /// </summary>
        /// <returns>Для <c>null</c> возвращается "(null)".
        /// </returns>
        [NotNull]
        public static string NullableToVisibleString<T>
            (
                [CanBeNull] this T value
            )
            where T : class
        {
            string text = value.NullableToString();
            return text.ToVisibleString();
        }

        /// <summary>
        /// Throw <see cref="ArgumentNullException"/>
        /// if given value is <c>null</c>.
        /// </summary>
        [NotNull]
        public static T ThrowIfNull<T>
            (
                [CanBeNull] this T value
            )
            where T : class
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentException("value");
            }

            return value;
        }

        /// <summary>
        /// Throw <see cref="ArgumentNullException"/>
        /// if given value is <c>null</c>.
        /// </summary>
        [NotNull]
        public static T ThrowIfNull<T>
            (
                [CanBeNull] this T value,
                [NotNull] string message
            )
            where T : class
        {
            Code.NotNull(message, "message");

            if (ReferenceEquals(value, null))
            {
                throw new ArgumentException(message);
            }

            return value;
        }

        #endregion
    }
}
