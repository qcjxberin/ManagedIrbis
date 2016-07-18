﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using AM;
using AM.Runtime;

using ManagedIrbis;

namespace UnitTests.ManagedIrbis
{
    [TestClass]
    public class MarcRecordTest
    {
        [TestMethod]
        public void TestMarcRecordConstruction()
        {
            MarcRecord record = new MarcRecord();

            Assert.IsNotNull(record);
            Assert.IsNotNull(record.Fields);
            Assert.IsNull(record.Database);
            Assert.IsNull(record.Description);
            Assert.AreEqual(0, record.Version);

            record.Fields.Add(new RecordField());

            Assert.AreEqual(record, record.Fields[0].Record);
        }

        private void _TestSerialization
            (
                MarcRecord record1
            )
        {
            byte[] bytes = record1.SaveToMemory();

            MarcRecord record2 = bytes
                .RestoreObjectFromMemory<MarcRecord>();
            Assert.IsNotNull(record2);
            Assert.AreEqual
                (
                    0,
                    MarcRecord.Compare
                    (
                        record1,
                        record2
                    )
                );
        }

        [TestMethod]
        public void TestMarcRecordSerialization()
        {
            MarcRecord record = new MarcRecord();
            _TestSerialization(record);
            record.Fields.Add(new RecordField("200"));
            _TestSerialization(record);
            record.Fields.Add(new RecordField("300", "Hello"));
            _TestSerialization(record);
        }

        private MarcRecord _GetRecord()
        {
            MarcRecord result = new MarcRecord();

            RecordField field = new RecordField("700");
            field.AddSubField('a', "Иванов");
            field.AddSubField('b', "И. И.");
            result.Fields.Add(field);

            field = new RecordField("701");
            field.AddSubField('a', "Петров");
            field.AddSubField('b', "П. П.");
            result.Fields.Add(field);

            field = new RecordField("200");
            field.AddSubField('a', "Заглавие");
            field.AddSubField('e', "подзаголовочное");
            field.AddSubField('f', "И. И. Иванов, П. П. Петров");
            result.Fields.Add(field);

            field = new RecordField("300", "Первое примечание");
            result.Fields.Add(field);
            field = new RecordField("300", "Второе примечание");
            result.Fields.Add(field);
            field = new RecordField("300", "Третье примечание");
            result.Fields.Add(field);

            return result;
        }

        [TestMethod]
        public void TestMarcRecordFM()
        {
            MarcRecord record = _GetRecord();

            Assert.AreEqual("Иванов", record.FM("700", 'a'));
            Assert.AreEqual("Первое примечание", record.FM("300"));
        }

        [TestMethod]
        public void TestMarcRecordFMA()
        {
            MarcRecord record = _GetRecord();

            string[] actual = record.FMA("700", 'a');
            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("Иванов", actual[0]);

            actual = record.FMA("300");
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("Первое примечание", actual[0]);
            Assert.AreEqual("Второе примечание", actual[1]);
            Assert.AreEqual("Третье примечание", actual[2]);
        }

        [TestMethod]
        public void TestMarcRecordFR()
        {
            MarcRecord record = _GetRecord();

            string actual = record.FR("\"Автор: \"v700^a");
            Assert.AreEqual("Автор: Иванов", actual);
        }

        [TestMethod]
        public void TestMarcRecordFRA()
        {
            MarcRecord record = _GetRecord();

            string[] actual = record.FRA("\"Автор: \"v700^a");
            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("Автор: Иванов", actual[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ReadOnlyException))]
        public void TestIrbisRecordReadOnly()
        {
            MarcRecord record = _GetRecord().AsReadOnly();

            record.Fields.Add(new RecordField());
        }

        [TestMethod]
        public void TestMarcRecordToJson()
        {
            MarcRecord record = _GetRecord();

            string actual = record.ToJson()
                .Replace("\r", "").Replace("\n", "")
                .Replace("\"", "'");
            const string expected = "{'fields':[{'tag':'700','subfields':[{'code':'a','value':'Иванов'},{'code':'b','value':'И. И.'}]},{'tag':'701','subfields':[{'code':'a','value':'Петров'},{'code':'b','value':'П. П.'}]},{'tag':'200','subfields':[{'code':'a','value':'Заглавие'},{'code':'e','value':'подзаголовочное'},{'code':'f','value':'И. И. Иванов, П. П. Петров'}]},{'tag':'300','value':'Первое примечание'},{'tag':'300','value':'Второе примечание'},{'tag':'300','value':'Третье примечание'}]}";

            Assert.AreEqual(expected, actual);
        }
    }
}