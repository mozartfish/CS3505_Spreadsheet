using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        //[TestMethod]
        //public void TestScrolling()
        //{
        //    // Test Scrolling Function - CurrentStatusList
        //    for (int i = 0; i < 1000; i++)
        //    {
        //        //currentStatusList.Items.Add(i.ToString());
        //    }
        //}

        //#region Model Tests


        //#region Hash Testing

        ////normal hash test
        //[TestMethod]
        //public void UserhashTesting()
        //{
        //    Users users = new Users();
        //    int first = users.GetActive("user","pass");
        //    int second = users.GetActive("user", "pass");
        //    Assert.AreEqual(first, second);

        //    first = users.GetActive("userlol", "pass");
        //    second = users.GetActive("userlol", "pass");
        //    Assert.AreEqual(first, second);

        //    first = users.GetActive("userlolhahahahaha", "pa5s");
        //    second = users.GetActive("userlolhahahahaha", "pa5s");
        //    Assert.AreEqual(first, second);

        //    first = users.GetActive("userlol", "hahahahahapass");
        //    second = users.GetActive("userlol", "hahahahahapass");
        //    Assert.AreEqual(first, second);
        //}

        ////hash test with numbers
        //[TestMethod]
        //public void UserhashTestingNumberUser()
        //{
        //    Users users = new Users();
        //    int first = users.GetActive("1", "pass");
        //    int second = users.GetActive("1", "pass");
        //    Assert.AreEqual(first, second);
        //}

        //[TestMethod]
        //public void UserhashTestingNumberPass()
        //{
        //    Users users = new Users();
        //    int first = users.GetActive("user", "1");
        //    int second = users.GetActive("user", "1");
        //    Assert.AreEqual(first, second);
        //}


        //[TestMethod]
        //public void UserhashTestingMixedNumber()
        //{
        //    Users users = new Users();
        //    int first = users.GetActive("user1", "1");
        //    int second = users.GetActive("user1", "1");
        //    Assert.AreEqual(first, second);
        //}


        ////symbols
        //[TestMethod]
        //public void UserhashTestingSymbols()
        //{
        //    Users users = new Users();
        //    int first = users.GetActive("user1", "1");
        //    int second = users.GetActive("user1", "1");
        //    Assert.AreEqual(first, second);
        //}

        //[TestMethod]
        //public void UserhashTestingDiffUsers()
        //{
        //    Users users = new Users();
        //    int first = users.GetActive("user", "pass");
        //    int second = users.GetActive("userp", "ass");
        //    Assert.AreNotEqual(first, second);
        //}


        //#endregion Hash Testing
        
        //#region Users

        //#region Count Test

        ////
        //[TestMethod]
        //public void TestCount()
        //{
        //    OldUsers users = new OldUsers();

        //    int counter = users.Count();
        //    Assert.AreEqual(0, counter);

        //    users.Add("","");
        //    counter = users.Count();
        //    Assert.AreEqual(1, counter);

        //    users.Remove("","");
        //    counter = users.Count();
        //    Assert.AreEqual(0, counter);
        //}

        //#endregion Count Test





        //#region Test Contains

        ////
        //[TestMethod]
        //public void TestContains()
        //{
        //    OldUsers users = new OldUsers();
        //    bool contain = users.Contains("","");
        //    Assert.IsFalse(contain);

        //    users.Add("","");
        //    contain = users.Contains("", "");
        //    Assert.IsTrue(contain);
        //}

        ////
        //[TestMethod]
        //public void TestContainsAfterRemove()
        //{
        //    OldUsers users = new OldUsers();

        //    users.Add("", "");
        //    bool contain = users.Contains("", "");
        //    Assert.IsTrue(contain);
        //    users.Remove("","");

        //    contain = users.Contains("", "");
        //    Assert.IsFalse(contain);
        //}

        //#endregion Test Contains


        //#region Adding User



        ////
        //[TestMethod]
        //public void TestAdd()
        //{
        //    OldUsers users = new OldUsers();
        //    users.Add("user", "pass");



        //    int first = users.GetActive("user", "pass");
        //    int second = users.GetActive("user", "pass");
        //    Assert.AreEqual(first, second);
        //}

        //#endregion Adding User


        //#endregion Users


        //#endregion Model Tests
    }
}