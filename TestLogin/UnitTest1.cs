using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestLogin
{
    [TestClass]
    public class UnitTest1
    {
        readonly HwidSystem.HwidSystem hwidSystem = new HwidSystem.HwidSystem("http://locacell.com/api/", "V1");
        [TestMethod]
        public void TestLogin()
        {
       
          Assert.AreEqual(hwidSystem.CheckLogin("admin", "password"),1);
        }
        [TestMethod]

        public void TestRegisterUser()
        {
            Assert.AreEqual(hwidSystem.RegisterUser("admin", "password", "AZERTY"), 1);
        }
    }
}
