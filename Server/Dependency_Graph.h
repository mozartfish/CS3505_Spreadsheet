/*
 *Authors: Thomas Ady, Pranav Rajan, Professor Daniel Kopta, Professor Joe Zachary
 *Last Updated: April 5, 2019
 *
 *Header file for dependency graph class for keeping track of dependencies in spreadsheets in the server
 *These function and class declarations are based upon the CS 3500 dependency graph assignment designed by Professor Daniel Kopta and Professor Joe Zachary
 */

#include <unordered_set>
#include <unordered_map>

#ifndef DEPENDENCY_GRAPH_H
#define DEPENDENCY_GRAPH_H
class DependencyGraph
{
 private:

  // An integer used to keep track of the number of ordered pairs in the Dependency Graph
  int numPairs;
  //Key: String that the value set depends on
  //Value: A set of strings that depend on the Key
  std::unordered_map<std::string, std::unordered_set<std::string>> dependentsMap;

  //Key:The string that depends on the value set
  //Value: A set of strings that the Key depends on 
  std::unordered_map<std::string, std::unordered_set<std::string>> dependeesMap;

 public:
  DependencyGraph(); // constructor
  ~DependencyGraph(); // destructor
  int Size(); // function that returns the number of ordered pairs in the dependency graph
  int Dependees_Size(std::string s); // returns the size of the dependees
  bool Has_Dependents(std::string s); // reports whether dependents is non-empty
  bool Has_Dependees(std::string s); // reports whether dependees is non-empty
  std::unordered_set<std::string> Get_Dependents(std::string); // enumerates dependents
  std::unordered_set<std::string>Get_Dependees(std::string); // enumerates dependees
  void Add_Dependency(std::string s, std::string t); // function that adds the ordered pair (s, t) if it doesn't exist
  void Remove_Dependency(std::string s, std::string t); // removes the ordered pair (s, t) if it exists
  void Replace_Dependents(std::string s, std::unordered_set<std::string> new_Dependents); // function that removes all ordered pairs of the form (s, r). Then, for each t in new_Dependents, adds the ordered pair (s, t)
  void Replace_Dependees(std::string s, std::unordered_set<std::string> new_Dependees); // function that removes all existsing ordered pairs of the form (r, s). Then, for each t in new_Dependees, adds the ordered pair (t, s)
};
#endif

