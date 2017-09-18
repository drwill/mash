using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace Mash.AppSettings.Tests
{
    [TestClass]
    [DeploymentItem("SettingsTestData.json")]
    public class JsonSettingLoaderTests
    {
        private static JsonSettings _settings;
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void JsonSettingLoader_LoadsSettings(TestContext testContext)
        {
            string jsonFile = Path.Combine(testContext.TestDeploymentDir, "SettingsTestData.json");
            ISettingLoader jsonSettingLoader = AppSettingsFactory.GetJsonSettingLoader(jsonFile);

            _settings = new JsonSettings();
            AppSettingsLoader.Load(jsonSettingLoader, ref _settings);
        }

        [TestMethod]
        public void JsonSettingLoader_LoadsBool()
        {
            Assert.AreEqual(true, _settings.True1);
        }

        [TestMethod]
        public void JsonsettingLoader_LoadsInt()
        {
            Assert.AreEqual(1, _settings.Number1);
        }

        [TestMethod]
        public void JsonSettingLoader_LoadsList()
        {
            CollectionAssert.AreEqual(new List<string> { "One", "Two", "Three" }, _settings.List1);
        }

        [TestMethod]
        public void JsonSettingLoader_LoadsString()
        {
            Assert.AreEqual("Value1", _settings.Setting1);
        }
    }

    [AppSetting]
    public class JsonSettings
    {
        public List<string> List1 { get; set; }
        public int Number1 { get; set; }
        public string Setting1 { get; set; }
        public bool True1 { get; set; }
    }
}