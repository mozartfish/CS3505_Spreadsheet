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

//Reports whether dependents(s) is non-empty
bool DependencyGraph::HasDependents(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator has_dependents = this->dependents_map.find(s); // create a new iterator to find key in map
  if (has_dependents == this->dependents_map.end())
    return false;
  else
  {
    if (has_dependents->second.size() > 0)
      return true;
    else
      return false;
  }
}

//Reports whether dependees(s) is non-empty
bool DependencyGraph::HasDependees(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator has_dependees = this->dependees_map.find(s); // create a new iterator to find key in map
  if (has_dependees == this->dependees_map.end())
    return false;
  else
  {
    if (has_dependees->second.size() > 0)
      return true;
    else
      return false;
  }
}

//Enumerates dependents(s)
std::unordered_set<std::string> DependencyGraph::GetDependents(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator get_dependents = this->dependents_map.find(s);
  if (get_dependents == this->dependents_map.end())
  {
    std::unordered_set<std::string> empty_set = std::unordered_set<std::string>();
    return empty_set;
  }
  else
  {
    std::unordered_set<std::string> get_dependents_set = get_dependents->second;
    return get_dependents_set;
  }
}

//Enumerates dependees(s)
std::unordered_set<std::string> DependencyGraph::GetDependees(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator get_dependees = this->dependees_map.find(s); // create a new iterator to find key in map
  if (get_dependees == this->dependees_map.end())
  {
    std::unordered_set<std::string> empty_set = std::unordered_set<std::string>();
    return empty_set;
  }
  else
  {
    std::unordered_set<std::string> get_dependees_set = get_dependees->second;
    return get_dependees_set;
  }
}

//Adds the ordered pair (s,t) if it doesn't exist
void DependencyGraph::AddDependency(std::string s, std::string t)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator dependents_find = this->dependents_map.find(s);
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator dependees_find = this->dependees_map.find(t);
  
  //CASE 1: the dependency graph does not contain s or t
  if ((dependents_find == this->dependents_map.end()) && (dependees_find == this->dependees_map.end()))
  {
    std::unordered_set<std::string> dependents_set = std::unordered_set<std::string>();
    std::unordered_set<std::string> dependees_set = std::unordered_set<std::string>();
    
    dependents_set.insert(t);
    dependees_set.insert(s);
    
    this->dependents_map.insert({s, dependents_set});
    this->dependees_map.insert({t, dependees_set});
    
    this->num_pairs++;
    
    return;
  }

  //CASE 2: s already exists in the dependentsMap but t is not in the set mapped to s
  else if ((this->dependents_map.count(s) > 0) && (this->dependees_map.count(t) == 0))
  {
    std::unordered_set<std::string> dependents_set =  dependents_find->second;
    dependents_set.insert(t);
    this->dependents_map.insert({s, dependents_set});
    
    std::unordered_set<std::string> new_dependees_set = std::unordered_set<std::string>();
    new_dependees_set.insert(s);
    this->dependees_map.insert({t, new_dependees_set});
        
    this->num_pairs++;
    
    return;
  }
  
  //CASE 3: t already exists in the dependeesMap but s is not in the set mapped to t
  else if ((this->dependents_map.count(s) == 0) && (this->dependees_map.count(t) == 1))
  {
    std::unordered_set<std::string> dependees_set = dependees_find->second;
    dependees_set.insert(s);
    this->dependees_map.insert({t, dependees_set});
    
    std::unordered_set<std::string> new_dependents_set = std::unordered_set<std::string>();
    new_dependents_set.insert(t);
    this->dependents_map.insert({s, new_dependents_set});
    
    this->num_pairs++;
    
    return;
  }
  
  //CASE 4: s and t already exist in the dependeesMap and dependentsMap
  else
  {
    if ((this->dependents_map.count(s) == 1) && (this->dependees_map.count(t) == 1))
    {
      std::unordered_set<std::string> dependents_set = dependents_find->second;
      std::unordered_set<std::string> dependees_set = dependees_find->second;
      
      if ((dependents_set.count(t) == 1) && (dependees_set.count(s) == 1))
	return;
      else
      {
	dependents_set.insert(t);
	dependees_set.insert(s);
	
	this->dependents_map.insert({s, dependents_set});
	this->dependees_map.insert({t, dependees_set});
	
	this->num_pairs++;
	
	return;
      }
    }
  }
}

//Removes the ordered pair (s, t) if it exists
void DependencyGraph::RemoveDependency(std::string s, std::string t)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator remove_dependents = this->dependents_map.find(s);
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator remove_dependees = this->dependees_map.find(t);
  
  if ((this->dependents_map.count(s)  == 1) && (this->dependees_map.count(t) == 1))
  {
    std::unordered_set<std::string> remove_dependents_set = remove_dependents->second;
    std::unordered_set<std::string> remove_dependees_set = remove_dependees->second;
    
    if ((remove_dependents_set.count(t) == 1) && (remove_dependees_set.count(s) == 1))
    {
      remove_dependents_set.erase(t);
      remove_dependees_set.erase(s);
      
      this->dependents_map.insert({s, remove_dependents_set});
      this->dependees_map.insert({t, remove_dependees_set});
      
      this->num_pairs--;
      
      return;
    }
    else
      return;
  }
  else
    return;
}

//Removes all existing ordered pairs of the form (s, r). THen, for each t in new_dependents, adds the ordered pair (s, t)
void DependencyGraph::ReplaceDependents(std::string s, std::unordered_set<std::string> new_dependents)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator replace_dependents = this->dependents_map.find(s);
  
  //CASE 1: check if s is not a key in the dependentsMap
  if (this->dependents_map.count(s) == 0)
  {
    std::unordered_set<std::string> add_new_dependents_set = new_dependents;
    for (std::string t : add_new_dependents_set)
    {
      this->AddDependency(s, t);
    }
    
    return;
  }
  
  //CASE 2: s is a key in the dependentsMap
  else
  {
    std::unordered_set<std::string> remove_old_dependents_set = this->GetDependents(s);
    for (std::string r : remove_old_dependents_set)
    {
      this->RemoveDependency(s, r);
    }
    
    std::unordered_set<std::string> add_new_dependents_set = new_dependents;
    for (std::string t : add_new_dependents_set)
    {
      this->AddDependency(s, t);
    }
    
    return;
  }
}

//Removes all existing ordered pairs of the form (r, s). Then for each t in new_dependees, adds the ordered pair (t, s)
void DependencyGraph::ReplaceDependees(std::string s, std::unordered_set<std::string> new_dependees)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator replace_dependees = this->dependees_map.find(s);
  //CASE 1: check if s is not a key in the dependeesMap
  if (this->dependees_map.count(s) == 0)
  {
    std::unordered_set<std::string> add_new_dependees_set = new_dependees;
    for (std::string t : add_new_dependees_set)
    {
      this->AddDependency(t, s);
    }
    
    return;
  }
  
  //CASE 2: s is a key in the dependeesMap
  else
  {
    std::unordered_set<std::string> remove_old_dependees_set = this->GetDependees(s);
    for (std::string r : remove_old_dependees_set)
    {
      this->RemoveDependency(r, s);
    }
    
    std::unordered_set<std::string> add_new_dependees_set = new_dependees;
    
    for (std::string t : add_new_dependees_set)
    {
      this->AddDependency(t, s);
    }
    
    return;
  }
}


