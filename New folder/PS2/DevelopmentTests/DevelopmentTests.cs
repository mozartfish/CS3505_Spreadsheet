using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;


namespace DevelopmentTests
{
    /// <summary>
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyGraphTest
    {
        /// <summary>
        ///Testing this["a"] method on a active dependency
        ///</summary>
        [TestMethod()]
        public void ThisMethodTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t["y"]);
        }
        
        /// <summary>
        ///Testing this["a"] method on a non-existent dependency
        ///</summary>
        [TestMethod()]
        public void ThisMethodTestEmpty()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t["x"]);
        }

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }

        /// <summary>
        ///Graph should add the dependencies.
        ///</summary>
        [TestMethod()]
        public void ReplaceNonExistentDependees()
        {
            DependencyGraph t = new DependencyGraph();
            t.ReplaceDependees("r", new HashSet<string>() { "a", "c" });
            Assert.AreEqual(2, t.Size);
            IEnumerator<String> test = t.GetDependees("r").GetEnumerator();
            Assert.IsTrue(test.MoveNext());
            String s1 = test.Current;
            Assert.IsTrue(test.MoveNext());
            String s2 = test.Current;
            Assert.IsFalse(test.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));
        }

        /// <summary>
        ///Graph should add the dependencies.
        ///</summary>
        [TestMethod()]
        public void ReplaceNonExistentDependents()
        {
            DependencyGraph t = new DependencyGraph();
            t.ReplaceDependents("r", new HashSet<string>() { "a", "c" });
            Assert.AreEqual(2, t.Size);
            IEnumerator<String> test = t.GetDependents("r").GetEnumerator();
            Assert.IsTrue(test.MoveNext());
            String s1 = test.Current;
            Assert.IsTrue(test.MoveNext());
            String s2 = test.Current;
            Assert.IsFalse(test.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));
        }

        /// <summary>
        ///Replacing the same set twice should not change size of dependency graph.
        ///</summary>
        [TestMethod()]
        public void DoubleReplaceDependees()
        {
            DependencyGraph t = new DependencyGraph();
            t.ReplaceDependees("r", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("r", new HashSet<string>() { "a", "c" });
            Assert.AreEqual(2, t.Size);
        }

        /// <summary>
        ///Replacing the same set twice should not change size of dependency graph.
        ///</summary>
        [TestMethod()]
        public void DoubleReplaceDependents()
        {
            DependencyGraph t = new DependencyGraph();
            t.ReplaceDependents("r", new HashSet<string>() { "a", "c" });
            t.ReplaceDependents("r", new HashSet<string>() { "a", "c" });
            Assert.AreEqual(2, t.Size);
        }

        /// <summary>
        /// Self refrencing variable should be added to dependency graph as normal
        /// </summary>
        [TestMethod()]
        public void TestSelfRefrencing()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "a");
            t.AddDependency("a", "b");
            Assert.AreEqual(2, t.Size);
        }

        /// <summary>
        /// If a dependent is a duplicate it should not be added to the hashset
        /// </summary>
        [TestMethod()]
        public void DuplicateReplacementsDependents()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "a", "a", "a" });
            Assert.AreEqual(1, t.Size);
            IEnumerator<String> test = t.GetDependents("a").GetEnumerator();
            Assert.IsTrue(test.MoveNext());
            String s1 = test.Current;
            Assert.IsTrue(s1 == "a");
            Assert.IsFalse(test.MoveNext());
        }

        /// <summary>
        /// If a dependent is a duplicate it should not be added to the hashset
        /// </summary>
        [TestMethod()]
        public void DuplicateReplacementsDependees()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "a", "a" });
            Assert.AreEqual(1, t.Size);
            IEnumerator<String> test = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(test.MoveNext());
            String s1 = test.Current;
            Assert.IsTrue(s1 == "a");
            Assert.IsFalse(test.MoveNext());
        }

        [TestMethod]
        public void HasDependeesCheckEmpty()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.IsFalse(t.HasDependees("x"));
        }

        [TestMethod]
        public void HasDependeesCheck()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "x");
            Assert.IsTrue(t.HasDependees("x"));
        }

        [TestMethod]
        public void HasDependentsCheckEmpty()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.IsFalse(t.HasDependents("x"));
        }

        [TestMethod]
        public void HasDependentsCheck()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "x");
            Assert.IsTrue(t.HasDependents("x"));
        }

        /// <summary>
        /// Empty string should still be added
        /// </summary>
        [TestMethod]
        public void AddEmptyString()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "");
            Assert.AreEqual(1, t.Size);
        }

        /// <summary>
        /// Empty string should still be added
        /// </summary>
        [TestMethod]
        public void RemoveEmptyString()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "");
            t.RemoveDependency("a", "");
            Assert.AreEqual(0, t.Size);
        }

        /// <summary>
        /// Cyclical Dependencies should be added as normal
        /// </summary>
        [TestMethod]
        public void CyclicDependency()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "a");
            Assert.AreEqual(2, t.Size);
            IEnumerator<String> test1 = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(test1.MoveNext());
            String s1 = test1.Current;
            IEnumerator<String> test2 = t.GetDependents("b").GetEnumerator();
            Assert.IsTrue(test2.MoveNext());
            String s2 = test2.Current;
            Assert.AreEqual("a", s1);
            Assert.AreEqual("a", s2);
        }
        
        [TestMethod]
        public void RemoveNonExistantDependency()
        {
            DependencyGraph t = new DependencyGraph();
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }

        [TestMethod]
        public void RemoveValidDependeeInvalidDependent()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            t.RemoveDependency("x", "r");
            Assert.AreEqual(1, t.Size);
            IEnumerator<String> test1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(test1.MoveNext());
            String s1 = test1.Current;
            Assert.AreEqual("x", s1);
            IEnumerator<String> test2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(test2.MoveNext());
            String s2 = test2.Current;
            Assert.AreEqual("y", s2);
        }

        [TestMethod]
        public void RemoveInvalidDependeeValidDependent()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            t.RemoveDependency("r", "y");
            Assert.AreEqual(1, t.Size);
        }

        /// <summary>
        /// Enumerator should still return empty even on non-existent string
        /// </summary>
        [TestMethod]
        public void ReturnEmptyEnumerator()
        {
            DependencyGraph t = new DependencyGraph();
            IEnumerator<String> test1 = t.GetDependees("b").GetEnumerator();
            IEnumerator<String> test2 = t.GetDependents("b").GetEnumerator();
            Assert.IsFalse(test1.MoveNext());
            Assert.IsFalse(test2.MoveNext());
        }

        /// <summary>
        /// An empty hash replacement should still remove
        /// </summary>
        [TestMethod]
        public void ReplaceWithEmptyHashDependents()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.ReplaceDependents("a", new HashSet<string>());
            Assert.AreEqual(0, t.Size);
        }

        /// <summary>
        /// An empty hash replacement should still remove
        /// </summary>
        [TestMethod]
        public void ReplaceWithEmptyHashDependees()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("c", "b");
            t.ReplaceDependees("b", new HashSet<string>());
            Assert.AreEqual(0, t.Size);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullValue()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", null);
        }

        /// <summary>
        /// Test should run in reasonable (linear) ammount of time.
        /// </summary>
        [TestMethod]
        public void ReplaceStressTest()
        {
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 1000;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // Create large hash for replacement
            HashSet<String> testHash = new HashSet<string>();
            for (int i = 0; i < SIZE; i++)
            {
                testHash.Add(letters[i]);
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                }
            }

            //Replace a bunch of dependents and dependees
            for(int i = 0; i < SIZE; i++)
            {
                t.ReplaceDependents(letters[i], testHash);
                t.ReplaceDependees(letters[i], testHash);
            }
        }


        //+++++++++++++++++++++++++++++++Pre-Written Tests+++++++++++++++++++++++++++++++++++++++//
        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyTest2()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }


        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void SimpleReplaceTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }



        ///<summary>
        ///It should be possibe to have more than one DG at a time.
        ///</summary>
        [TestMethod()]
        public void StaticTest()
        {
            DependencyGraph t1 = new DependencyGraph();
            DependencyGraph t2 = new DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest2()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest3()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("a", "b");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            t.AddDependency("c", "b");
            Assert.AreEqual(4, t.Size);
        }





        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest4()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("a", "d");
            t.AddDependency("c", "b");
            t.RemoveDependency("a", "d");
            t.AddDependency("e", "b");
            t.AddDependency("b", "d");
            t.RemoveDependency("e", "b");
            t.RemoveDependency("x", "y");
            Assert.AreEqual(4, t.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest5()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }



        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }
    }
}
