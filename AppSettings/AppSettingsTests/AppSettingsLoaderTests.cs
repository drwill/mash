using AppSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AppSettingsTests
{
    [TestClass]
    public class AppSettingsLoaderTests
    {
        [TestMethod]
        public void AppSettingsLoader_Load_LoadsBool()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            mockSettingsLoader.Settings.Add("IsTrue", "true");

            var settings = new Settings();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.IsTrue(settings.IsTrue, "Boolean setting not set to true");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_LoadsInt()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            mockSettingsLoader.Settings.Add("Is42", "42");

            var settings = new Settings();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.AreEqual(42, settings.Is42, "Int setting not set");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_LoadsString()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            mockSettingsLoader.Settings.Add("IsFoobar", "Foobar");

            var settings = new Settings();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.AreEqual("Foobar", settings.IsFoobar, "String setting not set");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_LoadsDateTime()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            DateTime today = DateTime.Today;

            mockSettingsLoader.Settings.Add("IsToday", today.ToString());

            var settings = new Settings();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.AreEqual(today, settings.IsToday, "DateTime setting not set");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_LoadsGuid()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            Guid guid = Guid.NewGuid();

            mockSettingsLoader.Settings.Add("IsGuid", guid.ToString());

            var settings = new Settings();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.AreEqual(guid, settings.IsGuid, "Guid setting not set");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_LoadsEnumByName()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            Option option = Option.Option2;

            mockSettingsLoader.Settings.Add("IsOption2", option.ToString());

            var settings = new Settings();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.AreEqual(option, settings.IsOption2, "Enum setting not set");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_LoadsEnumByValue()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            Option option = Option.Option2;

            mockSettingsLoader.Settings.Add("IsOption2", ((int)option).ToString());

            var settings = new Settings();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.AreEqual(option, settings.IsOption2, "Enum setting not set");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_ExceptionOnFailToParse()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            mockSettingsLoader.Settings.Add("Is42", "fourty-two");

            var settings = new Settings();

            bool exceptionCaught = false;

            try
            {
                AppSettingsLoader.Load(mockSettingsLoader, ref settings);
            }
            catch (AggregateException aggEx)
            {
                exceptionCaught = true;
                Assert.AreEqual(1, aggEx.InnerExceptions.Count);
            }

            Assert.IsTrue(exceptionCaught, "An exception was not thrown");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_LoadsUndecoratedPropertyWhenClassIsDecorated()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            mockSettingsLoader.Settings.Add("Is42", "42");

            var settings2 = new Settings2();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings2), "Load returned false");
            Assert.AreEqual(42, settings2.Is42, "Int setting not set");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_UsesKeyOverride()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            mockSettingsLoader.Settings.Add("IsFooBar", "Foobar");

            var settings2 = new Settings2();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings2), "Load returned false");
            Assert.AreEqual("Foobar", settings2.IsFoo, "String setting not set");
        }

        private class Settings
        {
            [AppSetting]
            public bool IsTrue { get; set; }

            [AppSetting]
            public int Is42 { get; set; }

            [AppSetting]
            public string IsFoobar { get; set; }

            [AppSetting]
            public DateTime IsToday { get; set; }

            [AppSetting]
            public Guid IsGuid { get; set; }

            [AppSetting]
            public Option IsOption2 { get; set; }
        }

        [AppSetting]
        private class Settings2
        {
            public int Is42 { get; set; }

            [AppSetting(Key = "IsFooBar")]
            public string IsFoo { get; set; }
        }

        private enum Option
        {
            Option0,
            Option1,
            Option2
        }
    }
}
