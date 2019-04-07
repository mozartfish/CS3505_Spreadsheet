/*
 *Authors: Thomas Ady, Pranav Rajan, Professor Daniel Kopta, Professor Joe Zachary
 *Last update: April 6, 2019
 *
 *(s1, t1) is an ordered pair of strings
 *t1 depends on s1; s1 must be evaluated before t1
 * A DependencyGraph can be modeled as a set of ordered pairs of strings. Two ordered pairs
 *(s1, t1) and (s2, t2) are considered equal if and only if s1 equals s2 and t1 equals t2
 *Recall that sets never contain duplicates. If an attempt is made to add an element to a 
 *set, and the element is already in the set, the set remains unchanged.
 *Given a DependencyGraph DG:
 *1) If s is a string, the set of all strings t such that (s, t) is in DG is called dependnts(s) -> (the set of things that depend on s)
 *2) If s is a string, the set of all strings t such that (t, s) is in DG is called dependees(s) -> (The set of things that s depends on)
 */

#include "DependencyGraph.h"
#include <iostream> // for debugging purposes
#include <unordered_set>
#include <unordered_map>
#include <string>

//constructor
DependencyGraph::DependencyGraph()
{
  this->num_pairs = 0;
  this->dependents_map = std::unordered_map<std::string, std::unordered_set<std::string>>();
  this->dependees_map = std::unordered_map<std::string, std::unordered_set<std::string>>();
}

//destructor (if needed)
//DependencyGraph::~DependencyGraph()
//{
//}

//The number of ordered pairs in the dependency graph
int DependencyGraph::Size()
{
  return this->num_pairs;
}

//The size of dependees(s)
int DependencyGraph::DependeesSize(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator dependees_size = this->dependees_map.find(s); // create a new iterator to find key in map
  
  if (dependees_size == this->dependees_map.end())
    return 0;
  else
    return dependees_size->second.size();
}
