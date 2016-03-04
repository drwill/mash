using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mash.AppSettings.Tests
{
    [TestClass]
    public class AppSettingsLoaderTests
    {
        [TestMethod]
        public void AppSettingsLoader_Load_LoadsBool()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            mockSettingsLoader.Settings.Add("IsTrue", "true");

            var settings = new SettingsPrimitives();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.IsTrue(settings.IsTrue, "Boolean setting not set to true");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_LoadsInt()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            mockSettingsLoader.Settings.Add("Is42", "42");

            var settings = new SettingsPrimitives();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.AreEqual(42, settings.Is42, "Int setting not set");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_LoadsString()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            mockSettingsLoader.Settings.Add("IsFoobar", "Foobar");

            var settings = new SettingsPrimitives();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.AreEqual("Foobar", settings.IsFoobar, "String setting not set");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_LoadsDateTime()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            DateTime today = DateTime.Today;

            mockSettingsLoader.Settings.Add("IsToday", today.ToString());

            var settings = new SettingsPrimitives();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.AreEqual(today, settings.IsToday, "DateTime setting not set");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_LoadsGuid()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            Guid guid = Guid.NewGuid();

            mockSettingsLoader.Settings.Add("IsGuid", guid.ToString());

            var settings = new SettingsPrimitives();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.AreEqual(guid, settings.IsGuid, "Guid setting not set");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_LoadsEnumByName()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            Option option = Option.Option2;

            mockSettingsLoader.Settings.Add("IsOption2", option.ToString());

            var settings = new SettingsPrimitives();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.AreEqual(option, settings.IsOption2, "Enum setting not set");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_LoadsEnumByValue()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            Option option = Option.Option2;

            mockSettingsLoader.Settings.Add("IsOption2", ((int)option).ToString());

            var settings = new SettingsPrimitives();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.AreEqual(option, settings.IsOption2, "Enum setting not set");
        }

        [TestMethod]
        public void AppSetingsLoader_Load_LoadsAConnectionString()
        {
            var connectionStringName = "SingleConnectionString";
            var connectionStringValue = "ConnectionStringValue";
            var mockSettingLoader = new SettingLoaderMock();

            mockSettingLoader.ConnectionStrings.Add(connectionStringName, connectionStringValue);

            var settings = new SettingsConnectionString();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingLoader, ref settings), "Load returned flase");
            Assert.AreEqual(connectionStringValue, settings.SingleConnectionString);
        }

        [TestMethod]
        public void AppSetingsLoader_Load_LoadsConnectionStringsWhenPropertyIsDecoratedWithConnectionString()
        {
            var connectionStringName = "Name";
            var connectionStringValue = "Value";
            var mockSettingLoader = new SettingLoaderMock();

            mockSettingLoader.ConnectionStrings.Add(connectionStringName, connectionStringValue);

            var settings = new SettingsConnectionStrings();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingLoader, ref settings), "Load returned flase");
            Assert.IsTrue(settings.ConnectionStrings.ContainsKey(connectionStringName));
        }

        [TestMethod]
        public void AppSettingsLoader_Load_ExceptionOnFailToParse()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            mockSettingsLoader.Settings.Add("Is42", "fourty-two");

            var settings = new SettingsPrimitives();

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

            var settings = new SettingsUndecoratedProperties();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.AreEqual(42, settings.Is42, "Int setting not set");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_UsesKeyOverride()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            mockSettingsLoader.Settings.Add("IsFooBar", "Foobar");

            var settings = new SettingsOverride();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");
            Assert.AreEqual("Foobar", settings.IsFoo, "String setting not set");
        }

        [TestMethod]
        public void AppSettingsLoader_Load_NoExceptionIfHasOptionalSetting()
        {
            var mockSettingsLoader = new SettingLoaderMock();
            // Set the required setting to avoid the exception, but don't set the optional setting
            mockSettingsLoader.Settings.Add("RequiredSetting", "Exists");

            var settings = new SettingsOptional();

            AppSettingsLoader.Load(mockSettingsLoader, ref settings);
        }

        [TestMethod]
        public void AppSettingsLoader_Load_NoExceptionIfHasOptionalSettingOnClass()
        {
            var mockSettingsLoader = new SettingLoaderMock();

            var settings = new SettingsOptionalClass();

            AppSettingsLoader.Load(mockSettingsLoader, ref settings);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void AppSettingsLoader_Load_ExceptionIfRequiredSettingDoesNotExist()
        {
            var mockSettingsLoader = new SettingLoaderMock();

            var settings = new SettingsOptional();

            AppSettingsLoader.Load(mockSettingsLoader, ref settings);
        }

        [TestMethod]
        public void AppSettingsLoader_Load_LoadsCollection()
        {
            var mockSettingsLoader = new SettingLoaderMock();

            mockSettingsLoader.Settings.Add("StringCollection", "string1,string2,string3");
            mockSettingsLoader.Settings.Add("IntCollection", "1,2,3");
            mockSettingsLoader.Settings.Add("EnumCollection", "Option0,Option1,Option2");

            var settings = new SettingsCollections();

            Assert.IsTrue(AppSettingsLoader.Load(mockSettingsLoader, ref settings), "Load returned false");

            CollectionAssert.AreEqual(
                mockSettingsLoader.Settings["StringCollection"].Split(','),
                settings.StringCollection.ToArray(),
                "String collection not set");

            CollectionAssert.AreEqual(
                mockSettingsLoader.Settings["IntCollection"].Split(',').Select(i => Int32.Parse(i)).ToArray(),
                settings.IntCollection.ToArray(),
                "Int collection not set");

            CollectionAssert.AreEqual(
                mockSettingsLoader.Settings["EnumCollection"].Split(',').Select(e => Enum.Parse(typeof(Option), e, false)).ToArray(),
                settings.EnumCollection.ToArray(),
                "Enum collection not set");
        }

        public class SettingsPrimitives
        {
            [AppSetting(Optional = true)]
            public bool IsTrue { get; set; }

            [AppSetting(Optional = true)]
            public int Is42 { get; set; }

            [AppSetting(Optional = true)]
            public string IsFoobar { get; set; }

            [AppSetting(Optional = true)]
            public DateTime IsToday { get; set; }

            [AppSetting(Optional = true)]
            public Guid IsGuid { get; set; }

            [AppSetting(Optional = true)]
            public Option IsOption2 { get; set; }
        }

        public class SettingsConnectionString
        {
            [AppSetting(SettingType = SettingType.Connectionstring)]
            public string SingleConnectionString { get; set; }
        }

        public class SettingsConnectionStrings
        {
            [AppSetting(SettingType = SettingType.Connectionstring)]
            public IReadOnlyDictionary<string, string> ConnectionStrings { get; set; }
        }

        [AppSetting]
        public class SettingsUndecoratedProperties
        {
            public int Is42 { get; set; }
        }

        public class SettingsOverride
        {
            [AppSetting(Key = "IsFooBar")]
            public string IsFoo { get; set; }
        }

        public class SettingsOptional
        {
            [AppSetting(Optional = true)]
            public string OptionalSetting { get; set; }

            [AppSetting]
            public string RequiredSetting { get; set; }
        }

        [AppSetting(Optional = true)]
        public class SettingsOptionalClass
        {
            public string OptionalSetting { get; set; }
        }

        [AppSetting]
        public class SettingsCollections
        {
            public List<string> StringCollection { get; set; }
            public IList<int> IntCollection { get; set; }
            public List<Option> EnumCollection { get; set; } = new List<Option>();
        }

        public enum Option
        {
            Option0,
            Option1,
            Option2,
        }
    }
}
