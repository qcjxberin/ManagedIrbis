﻿using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AM.Runtime;

using ManagedClient;

namespace UnitTests.ManagedClient
{
    [TestClass]
    public class IrbisConnectionTest
    {
        [TestMethod]
        public void TestIrbisConnectionConstructor()
        {
            using (IrbisConnection client = new IrbisConnection())
            {
                Assert.IsNotNull(client);
            }
        }

        private void _TestSerialization
            (
                IrbisConnection first
            )
        {
            byte[] bytes = first.SaveToMemory();

            IrbisConnection second = bytes
                .RestoreObjectFromMemory<IrbisConnection>();

            Assert.IsNotNull(second);
        }

        [TestMethod]
        public void TestIrbisConnectionSerialization()
        {
            IrbisConnection client = new IrbisConnection();
            _TestSerialization(client);
        }

        [TestMethod]
        [Ignore]
        public void TestIrbisConnectionConnect()
        {
            using (IrbisConnection client = new IrbisConnection())
            {
                client.ParseConnectionString("host=127.0.0.1;port=5555;user=1;password=1;");
                client.Connect();

                client.NoOp();

                //Thread.Sleep(10 * 1024);
            }
        }

        [TestMethod]
        [Ignore]
        public void TestIrbisConnectionGetServerVersion()
        {
            using (IrbisConnection client = new IrbisConnection())
            {
                client.ParseConnectionString("host=127.0.0.1;user=1;password=1;");
                client.Connect();

                IrbisVersion version = client.GetServerVersion();
                Assert.IsNotNull(version);

                //Thread.Sleep(10 * 1024);
            }
        }

        [TestMethod]
        [Ignore]
        public void TestIrbisConnectionReadRecord()
        {
            using (IrbisConnection client = new IrbisConnection())
            {
                client.ParseConnectionString("host=127.0.0.1;port=5555;user=1;password=1;");
                client.Connect();

                IrbisRecord record = client.ReadRecord(1);
                Assert.IsNotNull(record);

                //Thread.Sleep(10 * 1024);
            }
        }
    }
}