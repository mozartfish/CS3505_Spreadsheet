/*
 *Authors: Thomas Ady, Pranav Rajan, Professor Daniel Kopta, Professor Joe Zachary
 *
 *(s1, t1) is an ordered pair of strings
 *t1 depends on s1; s1 must be evaluated before t1
 * A Dependency_Graph can be modeled as a set of ordered pairs of strings. Two ordered pairs
 *(s1, t1) and (s2, t2) are considered equal if and only if s1 equals s2 and t1 equals t2
 *Recall that sets never contain duplicates. If an attempt is made to add an element to a 
 *set, and the element is already in the set, the set remains unchanged.
 *Given a Dependency_Graph DG:
 *1) If s is a string, the set of all strings t such that (s, t) is in DG is called dependnts(s) -> (the set of things that depend on s)
 *2) If s is a string, the set of all strings t such that (t, s) is in DG is called dependees(s) -> (THe set of things that s depends on)
 */

#include "Dependency_Graph.h" // header containing all function declarations
#include <iostream> // for debugging purposes
#include <unordered_set> // C++ implementation of a hash set
#include <unordered_map> // C++ implementation of a hash map

//constructor
Dependency_Graph::Dependency_Graph()
{
  this->numPairs = 0;
  this->dependentsMap = std::unordered_map<std::string, std::unordered_set<std::string>>();
  this->dependeesMap = std::unordered_map<std::string, std::unordered_set<std::string>>();
}

//destructor
//Dependency_Graph::~Dependency_Graph()
//{
//}

//Returns the number of ordered pairs in the dependency graph
int Dependency_Graph::Size()
{
  return this->numPairs;
}

//Returns the size of the number of dependees associated with the string s
int Dependency_Graph::Dependees_Size(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got = dependeesMap.find(s); // create a new iterator to find the string
  if (got == dependeesMap.end())
    return 0;
  else
    return got->size();
}

//Returns whether dependents is non-empty
bool Dependency_Graph::Has_Dependents(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got = dependentsMap.find(s); // create a new iterator to find the string
  if (got == dependentsMap.end())
    return false;
  else
  {
    if (got->size() > 0)
      return true;
    else
      return false;
  }
}

//Returns whether dependees is non-empty
bool Dependency_Graph::Has_Dependees(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got = dependeesMap.find(s); // create a new iterator to find the string
  if (got == dependeesMap.end())
    return false;
  else
  {
    if (got->size() > 0)
      return true;
    else
      return false;
  }
}

//Enumerates dependents
std::unordered_set<std::string> &Dependency_Graph::Get_Dependents(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got = dependentsMap.find(s); // create a new iterator to find the string
  if (got == dependentsMap.end())
    return std::unordered_set<std::string>();
  else
  {
    std::unordered_set<std::string> getDependents = got;
    return getDependents;
  }
}

//Enumerates dependees
std::unordered_set<std::string> &Dependency_Graph::Get_Dependees(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got = dependeesMap.find(s); // create a new iterator to find the string
  if (got == dependeesMap.end())
    return std::unordered_set<std::string>();
  else
  {
    std::unordered_set<std::string> getDependees = got;
    return getDependees;
  }
}

//Adds an ordered pair
void Add_Dependency (std::string s, std::string t)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got_dependees = dependeesMap.find(t); // create a new iterator to find the string
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got_dependents = dependentsMap.find(s); // create a new iterator to find the string

  //CASE 1: THE DEPENDENCY GRAPH DOES NOT CONTAIN S OR T
  if (got_dependees == dependeesMap.end() && got_dependents == dependentsMap.end())
  {
    std::unordered_set<std::string> dependeesSet = std::unordered_set<std::string>();
    std::unordered_set<std::string> dependentsSet = std::unordered_set<std::string>();
    
    dependeesSet.insert(s);
    dependentsSet.insert(t);

    dependentsMap.insert(s, dependentsMap);
    dependeesMap.insert(t, dependeesMap);
    
    this->numPairs++;

    return;
  }

  //CASE 2: S akreadyexists in the dependentsMap but t is not in the set mapped to s
  else if (got_dependees == dependeesMap.end())
  {
    got->insert(t);
    
    std::unordered_set<std::string> newDependeesSet = std::unordered_set<std::string>();
    
    new DependeesSet.insert(s);
    
    dependeesMap.insert(t, newDependeesSet);
    dependentsMap.insert(t);
  } 

  
}

