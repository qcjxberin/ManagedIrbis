﻿/* IrbisRecord.cs -- MARC-record
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AM;
using AM.IO;
using AM.Runtime;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

using Newtonsoft.Json;

#endregion

namespace ManagedClient
{
    /// <summary>
    /// MARC-record
    /// </summary>
    [PublicAPI]
    [Serializable]
    [MoonSharpUserData]
    [DebuggerDisplay("[{Database}] MFN={Mfn} ({Version})")]
    public sealed class IrbisRecord
        : IHandmadeSerializable,
        IReadOnly<IrbisRecord>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        [CanBeNull]
        [JsonProperty("database")]
        public string Database
        {
            get { return _database; }
            set { SetDatabase(value); }
        }

        /// <summary>
        /// MFN записи
        /// </summary>
        [JsonProperty("mfn")]
        public int Mfn
        {
            get { return _mfn; }
            set { SetMfn(value); }
        }

        /// <summary>
        /// Статус записи: удалена, блокирована и т.д.
        /// </summary>
        [JsonProperty("status")]
        public RecordStatus Status
        {
            get { return _status; }
            set { SetStatus(value); }
        }

        /// <summary>
        /// Версия записи. Нумеруется с нуля.
        /// </summary>
        [JsonProperty("version")]
        public int Version
        {
            get { return _version; }
            set { SetVersion(value); }
        }

        /// <summary>
        /// Смещение предыдущей версии записи.
        /// </summary>
        [JsonIgnore]
        public long PreviousOffset { get; set; }

        /// <summary>
        /// Поля записи.
        /// </summary>
        [JsonProperty("fields")]
        public RecordFieldCollection Fields
        {
            get { return _fields; }
        }

        /// <summary>
        /// Признак удалённой записи.
        /// </summary>
        [JsonIgnore]
        public bool Deleted
        {
            get { return ((Status & RecordStatus.LogicallyDeleted) != 0); }
            set
            {
                if (value)
                {
                    Status |= RecordStatus.LogicallyDeleted;
                }
                else
                {
                    Status &= ~RecordStatus.LogicallyDeleted;
                }
            }
        }

        /// <summary>
        /// Библиографическое описание.
        /// </summary>
        [CanBeNull]
        [JsonIgnore]
        public string Description { get; set; }

        /// <summary>
        /// Используется при сортировке записей.
        /// </summary>
        [CanBeNull]
        [JsonIgnore]
        public string SortKey { get; set; }

        /// <summary>
        /// Индекс документа.
        /// Используется для идентификации записей.
        /// </summary>
        [CanBeNull]
        [JsonIgnore]
        public string Index { get; set; }

        /// <summary>
        /// Произвольные пользовательские данные.
        /// </summary>
        [CanBeNull]
        [JsonIgnore]
        public object UserData { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Конструктор
        /// </summary>
        public IrbisRecord()
        {
            _fields = new RecordFieldCollection()
                ._SetRecord(this);
        }

        /// <summary>
        /// Конструктор для клонирования.
        /// </summary>
        private IrbisRecord
            (
                [NotNull] IrbisRecord other
            )
        {
            Database = other.Database;
            Mfn = other.Mfn;
            Status = other.Status;
            Version = other.Version;
            PreviousOffset = other.PreviousOffset;
            _fields = other.Fields.Clone();
            _fields._SetRecord(this);
            Description = other.Description;
            SortKey = other.SortKey;
            Index = other.Index;
            UserData = other.UserData;
        }

        #endregion

        #region Private members

        private readonly RecordFieldCollection _fields;

        private string _database;

        private int _mfn, _version;

        private RecordStatus _status;

        #endregion

        #region Public methods

        /// <summary>
        /// Создание "глубокой" копии записи.
        /// </summary>
        [NotNull]
        public IrbisRecord Clone()
        {
            IrbisRecord result = new IrbisRecord(this);
            
            return result;
        }

        /// <summary>
        /// Compares two records.
        /// </summary>
        public static int Compare
            (
                [NotNull] IrbisRecord record1,
                [NotNull] IrbisRecord record2
            )
        {
            Code.NotNull(() => record1);
            Code.NotNull(() => record2);

            int result = (int) record1.Status - (int) record2.Status;
            if (result != 0)
            {
                return result;
            }
            result = record1.Fields.Count - record2.Fields.Count;
            if (result != 0)
            {
                return result;
            }
            for (int i = 0; i < record1.Fields.Count; i++)
            {
                RecordField field1 = record1.Fields[i];
                RecordField field2 = record2.Fields[i];

                result = RecordField.Compare
                    (
                        field1,
                        field2
                    );
                if (result != 0)
                {
                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// Получить текст поля до разделителей подполей
        /// первого повторения поля с указанной меткой.
        /// </summary>
        /// <param name="tag">Метка поля.</param>
        /// <returns>Значение поля или <c>null</c>.</returns>
        [CanBeNull]
        public string FM
            (
                [NotNull] string tag
            )
        {
            Code.NotNull(tag, "tag");

            return Fields.GetFirstFieldValue(tag);
        }

        /// <summary>
        /// Текст всех полей с указанным тегом.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public string[] FMA
            (
                [NotNull] string tag
            )
        {
            Code.NotNull(tag, "tag");

            return Fields.GetFieldValue(tag);
        }

        /// <summary>
        /// Текст первого подполя с указанным тегом и кодом.
        /// </summary>
        [CanBeNull]
        public string FM
            (
                [NotNull] string tag,
                char code
            )
        {
            Code.NotNull(tag, "tag");

            return Fields.GetFirstSubFieldValue(tag, code);
        }

        /// <summary>
        /// Текст всех подполей с указанным тегом и кодом.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public string[] FMA
            (
                [NotNull] string tag,
                char code
            )
        {
            Code.NotNull(tag, "tag");

            return Fields.GetSubFieldValue(tag, code);
        }

        /// <summary>
        /// Простейшее форматирование поля/подполя.
        /// </summary>
        [CanBeNull]
        public string FR
            (
                [NotNull] string format
            )
        {
            Code.NotNull(format, "format");

            FieldReference reference = FieldReference.Parse(format);
            string result = reference.FormatSingle(this);

            return result;
        }

        /// <summary>
        /// Простейшее форматирование поля/подполя.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public string[] FRA
            (
                [NotNull] string format
            )
        {
            Code.NotNull(format, "format");

            FieldReference reference = FieldReference.Parse(format);
            string[] result = reference.Format(this);

            return result;
        }

        [NotNull]
        public IrbisRecord SetDatabase
            (
                [CanBeNull] string newDatabase
            )
        {
            ThrowIfReadOnly();

            return this;
        }

        [NotNull]
        public IrbisRecord SetMfn
            (
                int newMfn
            )
        {
            ThrowIfReadOnly();

            _mfn = newMfn;

            return this;
        }

        [NotNull]
        public IrbisRecord SetStatus
            (
                RecordStatus newStatus
            )
        {
            ThrowIfReadOnly();

            _status = newStatus;

            return this;
        }

        [NotNull]
        public IrbisRecord SetVersion
            (
                int newVersion
            )
        {
            ThrowIfReadOnly();

            _version = newVersion;

            return this;
        }

        /// <summary>
        /// For Newtonsoft.Json.
        /// </summary>
        public bool ShouldSerializeDatabase()
        {
            return !string.IsNullOrEmpty(Database);
        }

        /// <summary>
        /// For Newtonsoft.Json.
        /// </summary>
        public bool ShouldSerializeMfn()
        {
            return Mfn != 0;
        }

        /// <summary>
        /// For Newtonsoft.Json.
        /// </summary>
        public bool ShouldSerializeStatus()
        {
            return Status != 0;
        }

        /// <summary>
        /// For Newtonsoft.Json.
        /// </summary>
        public bool ShouldSerializeVersion()
        {
            return Version != 0;
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
            Code.NotNull(reader, "reader");

            Database = reader.ReadNullableString();
            Mfn = reader.ReadPackedInt32();
            Status = (RecordStatus) reader.ReadByte();
            Version = reader.ReadPackedInt32();
            Fields.RestoreFromStream(reader);
            Description = reader.ReadNullableString();
            SortKey = reader.ReadNullableString();
            Index = reader.ReadNullableString();
        }

        /// <summary>
        /// Просим объект сохранить себя в потоке.
        /// </summary>
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            Code.NotNull(writer, "writer");

            writer.WriteNullable(Database);
            writer.WritePackedInt32(Mfn);
            writer.Write((byte) Status);
            writer.WritePackedInt32(Version);
            Fields.SaveToStream(writer);
            writer.WriteNullable(Description);
            writer.WriteNullable(SortKey);
            writer.WriteNullable(Index);
        }

        #endregion

        #region IReadOnly<T> members

        [NonSerialized]
        internal bool _readOnly;

        /// <summary>
        /// Whether the record read-only?
        /// </summary>
        [JsonIgnore]
        public bool ReadOnly { get { return _readOnly; } }

        /// <summary>
        /// Creates read-only clone of the record.
        /// </summary>
        [NotNull]
        public IrbisRecord AsReadOnly()
        {
            IrbisRecord result = Clone();
            result.SetReadOnly();

            return result;
        }

        /// <summary>
        /// Marks the record as read-only.
        /// </summary>
        public void SetReadOnly()
        {
            _readOnly = true;
            Fields.SetReadOnly();
        }

        /// <summary>
        /// Throws if read only.
        /// </summary>
        public void ThrowIfReadOnly()
        {
            if (ReadOnly)
            {
                throw new ReadOnlyException();
            }
        }

        #endregion

        #region Object members

        #endregion
    }
}
