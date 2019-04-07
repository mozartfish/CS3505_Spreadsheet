/*
 *Authors: Thomas Ady, Pranav Rajan, Professor Daniel Kopta, Professor Joe Zachary
 *Last Updated: April 6, 2019
 *
 *Header file for dependency graph class for keeping track of dependencies in spreadsheets in the server
 *These function and class declarations are based upon the CS 3500 dependency graph assignment designed by Professor Daniel Kopta and Professor Joe Zachary
 */

#include <unordered_set>
#include <unordered_map>
#include <string>

#ifndef DEPENDENCYGRAPH_H
#define DEPENDENCYGRAPH_H
class DependencyGraph
{
 private:

  // An integer used to keep track of the number of ordered pairs in the Dependency Graph
  int num_pairs;

  //Key: String that the value set depends on
  //Value: A set of strings that depend on the Key
  std::unordered_map<std::string, std::unordered_set<std::string>> dependents_map;

  //Key:The string that depends on the value set
  //Value: A set of strings that the Key depends on 
  std::unordered_map<std::string, std::unordered_set<std::string>> dependees_map;

 public:
  DependencyGraph(); // constructor
  //~DependencyGraph(); // destructor
  int Size(); // The number of ordered pairs in the DependencyGraph
  int DependeesSize(std::string s); // The size of dependees(s)
  bool HasDependents(std::string s); // Reports whether dependents(s) is non-empty
  bool HasDependees(std::string s); // Reports whether dependees(s) is non-empty
  std::unordered_set<std::string> &GetDependents(std::string s); // Enumerates the dependents(s)
  std::unordered_set<std::string> &GetDependees(std::string s); // Enumerates the dependees(s)
  void AddDependency(std::string s, std::string t); // Adds the ordered pair (s, t), if it doesn't exist
  void RemoveDependency(std::string s, std::string t); // Removes the ordered pair (s,t), if it exists
  void ReplaceDependents(std::string s, std::unordered_set<std::string> new_dependents); // Removes all existing ordered pairs of the form (s, r). Then, for each t in new_dependents, adds the ordered pair (s, t)
  void ReplaceDependees(std::string s, std::unordered_set<std::string> new_dependees); // Removes all existing ordered pairs of the form (r, s). Then, for each t in new_dependees, adds the ordered pair (t, s).
};
#endif

