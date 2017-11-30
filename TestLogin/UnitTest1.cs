using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestLogin
{
    [TestClass]
    public class UnitTest1
    {
        private string host = "http://localhost/social_panel/admindesigns.com/demos/absolute/1.1";
        [TestMethod]
        public void TestLogin()
        {
          HwidSystem.HwidSystem hwidSystem = new HwidSystem.HwidSystem(host,"V1");
          var result = hwidSystem.CheckLogin("admin", "password");
        }
    }
}
