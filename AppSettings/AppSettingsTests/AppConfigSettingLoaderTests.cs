using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Mash.AppSettings.Tests
{
    [TestClass]
    public class AppConfigSettingLoaderTests
    {
        private AppConfigSettingLoader _loader = new AppConfigSettingLoader();

        [TestMethod]
        public void AppConfigSettingLoader_GetConnectionString_DoesNotExist()
        {
            // Should not throw an exception
            var result = _loader.GetConnectionString("csWhichDoesNotExist");

            Assert.AreEqual(null, result);
        }
    }
}
