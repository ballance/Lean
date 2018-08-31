﻿/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using System.IO;
using NUnit.Framework;
using QuantConnect.Packets;
using QuantConnect.Storage;

namespace QuantConnect.Tests.Common.Storage
{
    [TestFixture]
    public class LocalObjectStoreTests
    {
        private readonly LocalObjectStore _store = new LocalObjectStore();

        [TestFixtureSetUp]
        public void Setup()
        {
            _store.Initialize("CSharp-TestAlgorithm", 0, 0, "", new Controls());
        }

        [TestCase("my_key", "./storage/CSharp-TestAlgorithm/9ed6e46a3ff88783ff75296a4ba523f9.dat")]
        [TestCase("test/123", "./storage/CSharp-TestAlgorithm/0a2557f6be73a1b8a6abe84104899591.dat")]
        public void GetFilePathReturnsFileName(string key, string expectedRelativePath)
        {
            var expectedPath = Path.GetFullPath(expectedRelativePath).Replace("\\", "/");

            Assert.AreEqual(expectedPath, _store.GetFilePath(key).Replace("\\", "/"));
        }

        [Test]
        public void SavesAndLoadsText()
        {
            const string expectedText = "12;26";

            Assert.IsTrue(_store.SaveText("my_settings_text", expectedText));

            var actualText = _store.ReadText("my_settings_text");

            Assert.AreEqual(expectedText, actualText);
        }

        [Test]
        public void SavesAndLoadsJson()
        {
            var expected = new TestSettings { EmaFastPeriod = 12, EmaSlowPeriod = 26 };

            Assert.IsTrue(_store.SaveJson("my_settings_json", expected));

            var actual = _store.ReadJson<TestSettings>("my_settings_json");

            Assert.AreEqual(expected.EmaFastPeriod, actual.EmaFastPeriod);
            Assert.AreEqual(expected.EmaSlowPeriod, actual.EmaSlowPeriod);
        }

        [Test]
        public void SavesAndLoadsXml()
        {
            var expected = new TestSettings { EmaFastPeriod = 12, EmaSlowPeriod = 26 };

            Assert.IsTrue(_store.SaveXml("my_settings_xml", expected));

            var actual = _store.ReadXml<TestSettings>("my_settings_xml");

            Assert.AreEqual(expected.EmaFastPeriod, actual.EmaFastPeriod);
            Assert.AreEqual(expected.EmaSlowPeriod, actual.EmaSlowPeriod);
        }

        [Test]
        public void ThrowsIfKeyIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _store.ContainsKey(null));
            Assert.Throws<ArgumentNullException>(() => _store.Read(null));
            Assert.Throws<ArgumentNullException>(() => _store.Save(null, null));
            Assert.Throws<ArgumentNullException>(() => _store.Delete(null));
            Assert.Throws<ArgumentNullException>(() => _store.GetFilePath(null));
        }

        [Test]
        public void DisposeRemovesEmptyStorageFolder()
        {
            using (var store = new LocalObjectStore())
            {
                store.Initialize("unused", 0, 0, "", new Controls());

                Assert.IsTrue(Directory.Exists("./storage/unused"));
            }

            Assert.IsFalse(Directory.Exists("./storage/unused"));
        }

        public class TestSettings
        {
            public int EmaFastPeriod { get; set; }
            public int EmaSlowPeriod { get; set; }
        }
    }
}