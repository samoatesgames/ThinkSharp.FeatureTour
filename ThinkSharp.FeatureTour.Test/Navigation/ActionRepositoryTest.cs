// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using ThinkSharp.FeatureTouring.Navigation;

namespace ThinkSharp.FeatureTouring.Test.Navigation
{
    [TestClass]
    public class ActionRepositoryTest
    {
        [TestMethod]
        public void TestContains()
        {
            var repo = new ActionRepository();
            repo.AddAction("Hello", _ => { });
            Assert.IsTrue(repo.Contains("Hello"));
            Assert.IsTrue(repo.Contains("hello"));
            Assert.IsFalse(repo.Contains(" hello"));
        }

        [TestMethod]
        public void TestAddNull()
        {
            var repo = new ActionRepository();
            try
            {
                repo.AddAction("Hello", null);
                Assert.Fail();
            }
            catch (ArgumentNullException) { }

            Assert.IsFalse(repo.Contains("Hello"));
            repo.Execute("Hello", null);
        }

        [TestMethod]
        public void TestExecute()
        {
            int i = 0;
            var repo = new ActionRepository();
            repo.AddAction("add", _ => i++);
            repo.AddAction("add2", _ => i++, _ => true);
            repo.Execute("add", null);
            Assert.AreEqual(1, i);
            repo.Execute("add2", null);
            Assert.AreEqual(2, i);
        }

        [TestMethod]
        public void TestCanExecute()
        {
            var repo = new ActionRepository();
            repo.AddAction("true", _ => { }, _ => true);
            repo.AddAction("false", _ => { }, _ => false);
            Assert.IsTrue(repo.CanExecute("true", null));
            Assert.IsFalse(repo.CanExecute("false", null));
            Assert.IsFalse(repo.CanExecute("notExisting", null));
        }

        [TestMethod]
        public void TestReleaseExecute()
        {
            int i = 0;
            var repo = new ActionRepository();
            var release = repo.AddAction("a", _ => i++);
            Assert.IsTrue(repo.Contains("a"));
            repo.Execute("a", null);
            Assert.AreEqual(1, i);
            release.Release();
            Assert.IsFalse(repo.Contains("a"));
            repo.Execute("a", null);
            Assert.AreEqual(1, i);
        }

        [TestMethod]
        public void TestReleaseActionCanExecute()
        {
            int i = 0;
            var repo = new ActionRepository();
            var release = repo.AddAction("a", _ => i++, _ => true);
            Assert.IsTrue(repo.Contains("a"));
            repo.Execute("a", null);
            Assert.AreEqual(1, i);
            Assert.IsTrue(repo.CanExecute("a", null));
            release.Release();
            Assert.IsFalse(repo.Contains("a"));
            repo.Execute("a", null);
            Assert.AreEqual(1, i);
            Assert.IsFalse(repo.CanExecute("a", null));
        }

        [TestMethod]
        public void TestAddMultiTimes()
        {
            int a = 0;
            int b = 0;
            var repo = new ActionRepository();
            repo.AddAction("add", _ => a++);
            repo.AddAction("add", _ => b++, _ => true);
            repo.Execute("add", null);
            Assert.AreEqual(0, a);
            Assert.AreEqual(1, b);
        }

        [TestMethod]
        public void TestClear()
        {
            var repo = new ActionRepository();
            repo.AddAction("add", _ => { }, _ => true);
            Assert.AreEqual(1, repo.CanExecuteCount);
            Assert.AreEqual(1, repo.ExecuteCount);
            repo.Clear();
            Assert.AreEqual(0, repo.CanExecuteCount);
            Assert.AreEqual(0, repo.ExecuteCount);
        }
    }
}
