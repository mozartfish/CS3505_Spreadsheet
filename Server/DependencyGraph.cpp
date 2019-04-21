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

//Reports whether dependents(s) is non-empty
bool DependencyGraph::HasDependents(std::string s)
{
  
  if (this->dependents_map.find(s) == this->dependents_map.end())
    return false;
  else
  {
    if (this->dependents_map[s].size() > 0)
      return true;
    else
      return false;
  }
}

//Enumerates dependents(s)
std::unordered_set<std::string> * DependencyGraph::GetDependents(std::string s)
{
 
  if (this->dependents_map.find(s) == this->dependents_map.end())
  {
    
    return new std::unordered_set<std::string>();;
  }

  return &(dependents_map[s]);
}

//Adds the ordered pair (s,t) if it doesn't exist
void DependencyGraph::AddDependency(std::string s, std::string t)
{
 
  
  //CASE 1: the dependency graph does not contain s or t
  if (this->dependents_map.find(s) == this->dependents_map.end())
  {
    std::unordered_set<std::string> * dependents_set = new std::unordered_set<std::string>();
    
    dependents_set->insert(t);
    
    this->dependents_map[s] =  *dependents_set;
    
    this->num_pairs++;
    
    return;
  }

  //CASE 2: S has dependents already
  else 
  {

    this->dependents_map[s].insert(t);
        
    this->num_pairs++;
    
    return;
  }
  
}

//Removes the ordered pair (s, t) if it exists
void DependencyGraph::RemoveDependency(std::string s, std::string t)
{
  // If s in set, just try to remove t
  if (this->dependents_map.find(s) != this->dependents_map.end())
  {
    this->dependents_map[s].erase(t);
  }
}

//Removes all existing ordered pairs of the form (s, r). THen, for each t in new_dependents, adds the ordered pair (s, t)
void DependencyGraph::ReplaceDependents(std::string s, std::unordered_set<std::string> new_dependents)
{
  
  // Erase s from the dependents map regardless (clears all existing dependencies)
  if (this->dependents_map.find(s) != this->dependents_map.end())
  {
    this->dependents_map.erase(s);
  }
  
  // Add all dependencies
  for (std::string t : new_dependents)
    {
      this->AddDependency(s, t);
    }
}
