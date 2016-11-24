using Mash.AppSettings.DevSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Mash.AppSettings.Tests
{
    [TestClass]
    public class DevSettingLoaderTests
    {
        [TestMethod]
        public void DevSettingLoader_Ctor_PathDefaultsToCurrentDir()
        {
            string file = "foo.json";
            string expected = Path.Combine(Environment.CurrentDirectory, file);
            var devSettingLoader = new DevSettingLoader(devSettingsFile: file);

            Assert.AreEqual(expected, devSettingLoader.DevSettingFile);
        }

        [TestMethod]
        public void DevSettingLoader_Ctor_FileDefaultsToUser()
        {
            string path = "c:\\foo";
            string expected = Path.Combine(path, $"{Environment.UserName}.json");
            var devSettingLoader = new DevSettingLoader(path);

            Assert.AreEqual(expected, devSettingLoader.DevSettingFile);
        }

        [TestMethod]
        public void DevSettingLoader_Ctor_FileAsSpecified()
        {
            string path = "c:\\foo";
            string file = "foo.json";
            string expected = Path.Combine(path, file);
            var devSettingLoader = new DevSettingLoader(path, file);

            Assert.AreEqual(expected, devSettingLoader.DevSettingFile);
        }
    }
}
