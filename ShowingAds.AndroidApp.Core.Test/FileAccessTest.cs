using NUnit.Framework;
using ShowingAds.AndroidApp.Core.DataAccess;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.Test
{
    public class FileAccessTest
    {
        private readonly string _fileName = "test";

        private int _counter = 10000;
        private Dictionary<int, int> _data;
        private ConfigFileStore<Dictionary<int, int>> _store;

        [SetUp]
        public void Setup()
        {
            _store = new ConfigFileStore<Dictionary<int, int>>(_fileName);
            _data = new Dictionary<int, int>();
            _data.Add(1, 2);
        }

        [Test]
        public void DataStoreTest()
        {
            _store.Save(_data);
            Task.Run(SaveData);
            LoadData();
        }

        private void SaveData()
        {
            while (Interlocked.Decrement(ref _counter) > 0)
                _store.Save(_data);
        }

        private void LoadData()
        {
            while (_counter > 0)
            {
                var res = _store.Load();
                Assert.AreEqual(_data, res);
            }
        }

        [TearDown]
        public void Down()
        {
            _store.Dispose();
            File.Delete(_fileName);
        }
    }
}
