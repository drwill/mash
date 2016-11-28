using Mash.AppSettings.DevSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mash.AppSettings.Tests
{
    [TestClass]
    public class DevSettingLoaderTests
    {
        private const string _jsonFileType = ".json";

        [TestMethod]
        public void DevSettingLoader_Ctor_PathDefaultsToCurrentDir()
        {
            var fileName = $"{Path.GetRandomFileName()}{_jsonFileType}";
            var fullPath = Path.Combine(
                    Environment.CurrentDirectory,
                    fileName);

            FileStream file = null;
            try
            {
                file = File.Create(
                    fullPath,
                    1,
                    FileOptions.WriteThrough);
                file.Close();
                var devSettingLoader = new DevSettingLoader(fileName: fileName);

                Assert.AreEqual(fullPath, devSettingLoader.DevSettingFile);
            }
            finally
            {
                File.Delete(fullPath);
            }
        }

        [TestMethod]
        public void DevSettingLoader_Ctor_FileDefaultsToUser()
        {
            var dir = Path.GetTempPath();
            var fullPath = Path.Combine(
                    dir,
                    $"{Environment.UserName}{_jsonFileType}");

            FileStream file = null;
            try
            {
                file = File.Create(
                    fullPath,
                    1,
                    FileOptions.WriteThrough);
                file.Close();
                var devSettingLoader = new DevSettingLoader(dir: dir);

                Assert.AreEqual(fullPath, devSettingLoader.DevSettingFile);
            }
            finally
            {
                File.Delete(fullPath);
            }
        }

        [TestMethod]
        public void DevSettingLoader_Ctor_FileAsSpecified()
        {
            var dir = Path.GetTempPath();
            var fileName = $"{Path.GetRandomFileName()}{_jsonFileType}";
            var fullPath = Path.Combine(
                    dir,
                    fileName);

            FileStream file = null;
            try
            {
                file = File.Create(
                    fullPath,
                    1,
                    FileOptions.WriteThrough);
                file.Close();
                var devSettingLoader = new DevSettingLoader(dir, fileName);

                Assert.AreEqual(fullPath, devSettingLoader.DevSettingFile);
            }
            finally
            {
                File.Delete(fullPath);
            }
        }

        [TestMethod]
        public void DevSettingLoader_Ctor_ParsesContentsToDictionary()
        {
            string setting = "setting1";
            string expected = "value1";

            string json = $"{{ \"{setting}\": \"{expected}\" }}";
            var devSettingLoader = new DevSettingLoader(json: json);

            string value = devSettingLoader.GetSetting(setting);

            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        public void DevSettingLoader_GetSetting_ReturnsNullWhenMiss()
        {
            string setting = "setting1";
            string expected = "value1";

            string json = $"{{ \"{setting}\": \"{expected}\" }}";
            var devSettingLoader = new DevSettingLoader(json: json);

            string value = devSettingLoader.GetSetting("setting2");

            Assert.AreEqual(null, value);
        }

        [TestMethod]
        public void DevSettingLoader_GetConnectionStrings_ReturnsAll()
        {
            IDictionary<string, string> connectionStrings = new Dictionary<string, string>
            {
                { "connection1", "value1" },
                { "connection2", "value2" },
            };

            StringBuilder json = new StringBuilder("{ \"ConnectionStrings\": {");
            foreach (var kvp in connectionStrings)
            {
                json.AppendFormat("\"{0}\": \"{1}\",", kvp.Key, kvp.Value);
            }
            json.Remove(json.Length - 1, 1); // remove the last comma
            json.Append("}}"); // ending braces

            var devSettingLoader = new DevSettingLoader(json: json.ToString());

            IDictionary<string, string> actual = devSettingLoader.GetConnectionStrings();

            Assert.AreEqual(connectionStrings.Count, actual.Count);
            foreach (var kvp in connectionStrings)
            {
                Assert.AreEqual(kvp.Value, actual[kvp.Key]);
            }
        }

        [TestMethod]
        public void DevSettingLoader_GetConnectionStrings_HandlesEmpty()
        {
            var devSettingLoader = new DevSettingLoader(json: "");

            var actual = devSettingLoader.GetConnectionStrings();

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void DevSettingLoader_GetConnectionString_ReturnsValue()
        {
            string name = "connection1";
            string expected = "value1";

            string json = $"{{ \"ConnectionStrings\": {{\"{name}\": \"{expected}\"}}}}";

            var devSettingLoader = new DevSettingLoader(json: json.ToString());

            string actual = devSettingLoader.GetConnectionString(name);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DevSettingLoader_GetConnectionString_HandlesEmpty()
        {
            var devSettingLoader = new DevSettingLoader(json: "");

            string actual = devSettingLoader.GetConnectionString("connection1");

            Assert.AreEqual(null, actual);
        }
    }
}
