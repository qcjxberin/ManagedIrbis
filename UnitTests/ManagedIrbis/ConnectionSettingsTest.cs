﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ManagedIrbis;

namespace UnitTests.ManagedIrbis
{
    [TestClass]
    public class ConnectionSettingsTest
    {
        [TestMethod]
        public void TestConnectionSettings_ParseConnectionString()
        {
            ConnectionSettings settings = new ConnectionSettings();
            settings.ParseConnectionString("host=127.0.0.1;port=5555;"
                + "user=john galt;pwd=who is;db=NODB;arm=A");

            Assert.AreEqual("127.0.0.1", settings.Host);
            Assert.AreEqual(5555,settings.Port);
            Assert.AreEqual("john galt", settings.Username);
            Assert.AreEqual("who is", settings.Password);
            Assert.AreEqual("NODB",settings.Database);
            Assert.AreEqual
                (
                    IrbisWorkstation.Administrator,
                    settings.Workstation
                );
        }

        [TestMethod]
        public void TestConnectionSettings_Encode()
        {
            ConnectionSettings settings = new ConnectionSettings
            {
                Host = "127.0.0.1",
                Port = 5555,
                Username = "john galt",
                Password = "who is",
                Database = "NODB",
                Workstation = IrbisWorkstation.Administrator
            };
            string actual = settings.Encode();
            const string expected = "host=127.0.0.1;port=5555;"
                + "database=NODB;username=john galt;password=who is;"
                + "workstation=A;";

            Assert.AreEqual(expected, actual);
        }
    }
}
