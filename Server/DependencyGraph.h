/*
 *Authors: Thomas Ady, Pranav Rajan, Professor Daniel Kopta, Professor Joe Zachary
 *Last Updated: April 21, 2019
 *
 *Header file for dependency graph class for keeping track of dependencies in spreadsheets in the server
 *These function and class declarations are based upon the CS 3500 dependency graph assignment designed 
 *by Professor Daniel Kopta and Professor Joe Zachary
 */

#include <string>
#include <unordered_map>
#include <unordered_set>

#ifndef DEPENDENCYGRAPH_H
#define DEPENDENCYGRAPH_H

class DependencyGraph
{
 private:
  int num_pairs; // integer used for keeping track of the number of ordered pairs in the dependency graph
  
  //Key: String that the value set depends on
  //Value:: A set of strings that depend on the key
  std::unordered_map<std::string, std::unordered_set<std::string>> dependents_map;
  
 public:
  DependencyGraph();
  //~DependencyGraph();
  int Size();
  bool HasDependents(std::string s);
  std::unordered_set<std::string> * GetDependents(std::string s);
  void AddDependency(std::string s, std::string t);
  void RemoveDependency(std::string s, std::string t);
  void ReplaceDependents(std::string s, std::unordered_set<std::string> new_dependents);
  
};
#endif
