﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Mash.AppSettings.Tests
{
    [TestClass]
    public class AppSettingsAttributeTests
    {
        [TestMethod]
        public void AppSettingsAttribute_Specified()
        {
            string propertyName = "Override";
            string attributeMemberName = "Key";

            var nameArgument = GetKey(propertyName, attributeMemberName);

            Assert.AreEqual("Overriden", nameArgument);
        }

        [TestMethod]
        public void AppSettingsAttribute_NotSpecified()
        {
            string propertyName = "Default";
            string attributeMemberName = "Name";

            var nameArgument = GetKey(propertyName, attributeMemberName);

            Assert.IsNull(nameArgument);
        }

        private static string GetKey(string propertyName, string attributeMemberName)
        {
            var test = new SettingsWithAttributes();
            var defaultProperty = test.GetType().GetProperty(propertyName);
            var appSettingAttribute = defaultProperty.CustomAttributes
                .Where(a => a.AttributeType == typeof(AppSettingAttribute))
                .FirstOrDefault();
            var nameArgument = appSettingAttribute.NamedArguments
                .Where(arg => arg.MemberName == attributeMemberName)
                .FirstOrDefault();

            return nameArgument.TypedValue.Value != null ?
                nameArgument.TypedValue.Value.ToString() :
                null;
        }

        private class SettingsWithAttributes
        {
            [AppSetting]
            public int Default { get; set; }

            [AppSetting(Key = "Overriden")]
            public int Override { get; set; }
        }
    }
}
