// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
//
// Fulfillment of solution's implementation - Carina Imburgia for CS 3500, September 2018.
//                                          (Unit tests for complete code coverage added)
//                                          (Methods fully implemented)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// DLL for what will later be a spreadsheet program
/// </summary>
namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<String, HashSet<String>> Dependents;
        private Dictionary<String, HashSet<String>> Dependees;
        private int size = 0;
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            Dependents = new Dictionary<string, HashSet<String>>();
            Dependees = new Dictionary<string, HashSet<String>>();
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {
                return size;
            }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// Returns 0 if string has no dependees.
        /// </summary>
        public int this[string s]
        {
            get
            {
                if (Dependees.ContainsKey(s))
                {
                    return Dependees[s].Count;
                }
                return 0;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// Dependents will be non-empty if they exist in the dependents dictionary.
        /// </summary>
        public bool HasDependents(string s)
        {
            return Dependents.ContainsKey(s);
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// Dependents will be non-empty if they exist in the dependees dictionary.
        /// </summary>
        public bool HasDependees(string s)
        {
            return Dependees.ContainsKey(s);
        }


        /// <summary>
        /// Enumerates dependents(s). This makes a copy of the HashSet to protect data from
        /// being altered
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (Dependents.ContainsKey(s))
            {
                return new HashSet<String>(Dependents[s]);
            }
            return new HashSet<String>();
        }

        /// <summary>
        /// Enumerates dependees(s). This makes a copy of the HashSet to protect data from
        /// being altered
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (Dependees.ContainsKey(s))
            {
                return new HashSet<String>(Dependees[s]);
            }
            return new HashSet<String>();
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            //Checks s is not in the Dependents dictionary. Automatically create hash add if absent.
            if (!Dependents.ContainsKey(s))
            {
                HashSet<String> dependentsHash = new HashSet<string>();
                dependentsHash.Add(t);
                size++;
                Dependents.Add(s, dependentsHash);
            }
            else
            {
                AddToDependentsHash(s, t);
            }

            //Checks if t is not in the Dependees dictionary. Create hash if absent.
            if (!Dependees.ContainsKey(t))
            {
                HashSet<String> dependeesHash = new HashSet<string>();
                dependeesHash.Add(s);
                Dependees.Add(t, dependeesHash);
            }
            else
            {
                AddToDependeesHash(s, t);
            }

        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            //Dependency to remove should exist in both dictionaries
            if (!Dependents.ContainsKey(s) || !Dependees.ContainsKey(t))
            {
                return;
            }
            else
            {
                Dependents[s].Remove(t);
                Dependees[t].Remove(s);
                size--;
                CheckEmptyHash(s, Dependents);
                CheckEmptyHash(t, Dependees);
            }

        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (!Dependents.ContainsKey(s))
            {
                // Return if non-existant dependent and empty hash
                if(newDependents.Count<String>() == 0)
                {
                    return;
                }
                LoopAddDependents(s, newDependents.ToList<String>());
            }
            else
            {
                foreach(String d in Dependees.Keys.ToList<String>())
                {
                    // Remove the dependees reference to its dependent
                    if (Dependees[d].Contains(s))
                    {
                        Dependees[d].Remove(s);
                        CheckEmptyHash(d, Dependees);
                    }
                }
                size = size - Dependents[s].Count();
                Dependents.Remove(s);
                LoopAddDependents(s, newDependents.ToList<String>());
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if (!Dependees.ContainsKey(s))
            {
                // Return if non-existant dependee and empty hash
                if (newDependees.Count<String>() == 0)
                {
                    return;
                }
                LoopAddDependees(s, newDependees.ToList<String>());
            }
            else
            {
                foreach (String d in Dependents.Keys.ToList<String>())
                {
                    // Remove the dependent's reference to its dependee
                    if (Dependents[d].Contains(s))
                    {
                        Dependents[d].Remove(s);
                        size--;
                        CheckEmptyHash(d, Dependents);
                    }
                }
                Dependees.Remove(s);
                LoopAddDependees(s, newDependees.ToList<String>());
            }
        }

        /// <summary>
        /// Makes a copy of the dependee enumerable. Then, while iterating through, adds each new
        /// dependency created by the replacement.
        /// </summary>
        /// <param name="dent">The dependent that is receiving new dependees</param>
        /// <param name="newDependees">The dependees to add</param>
        private void LoopAddDependees(String dent, IEnumerable<String> newDependees)
        {
            foreach(String dependee in newDependees.ToList<String>())
            {
                AddDependency(dependee, dent);
            }
        }

        /// <summary>
        /// Makes a copy of the dependent enumerable. Then, while iterating through, adds each new
        /// dependency created by the replacement.
        /// </summary>
        /// <param name="dee">The dependee that is receiving new dependents</param>
        /// <param name="newDependents">The dependents to add</param>
        private void LoopAddDependents(String dee, IEnumerable<String> newDependents)
        {
            foreach (String dependent in newDependents.ToList<String>())
            {
                AddDependency(dee, dependent);
            }
        }

        /// <summary>
        /// This private helper method will check if the given key in the dictionary
        /// contains a HashSet with zero items in it. If this it the case, the key will
        /// be deleted.
        /// 
        /// This method will be called whenever a dependency is removed.
        /// </summary>
        /// <param name="s">String key to check hashsest size</param>
        /// <param name="d">Dictionary to remove from</param>
        private void CheckEmptyHash(String s, Dictionary<String, HashSet<String>> d)
        {
            if (d[s].Count == 0)
            {
                d.Remove(s);
            }
        }

        /// <summary>
        /// Checks an existing key and add the dependent to its dependee if not already
        /// in the HashSet. 
        /// </summary>
        /// <param name="s">String dependent</param>
        /// <param name="t">String dependee</param>
        private void AddToDependentsHash(String s, String t)
        {
            if (Dependents[s].Contains(t))
            {
                return;
            }
            else
            {
                Dependents[s].Add(t);
                size++;
            }
        }

        /// <summary>
        /// Checks an existing key and adds the dependee to its dependent if not already
        /// in the HashSet
        /// </summary>
        /// <param name="s">String dependent</param>
        /// <param name="t">String dependee</param>
        private void AddToDependeesHash(String s, String t)
        {
            if (Dependees[t].Contains(s))
            {
                return;
            }
            else
            {
                Dependees[t].Add(s);
            }
        }
    }
}