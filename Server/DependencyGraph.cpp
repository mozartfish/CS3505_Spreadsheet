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

#include "DependencyGraph.h" // header containing all function declarations
#include <iostream> // for debugging purposes
#include <unordered_set> // C++ implementation of a hash set
#include <unordered_map> // C++ implementation of a hash map
#include <string>

//constructor
DependencyGraph::DependencyGraph()
{
  this->num_pairs = 0;
  this->dependents_map = std::unordered_map<std::string, std::unordered_set<std::string>>();
  this->dependees_map = std::unordered_map<std::string, std::unordered_set<std::string>>();
}

//destructor (in case it is needed)
//DependencyGraph::~DependencyGraph()
//{
//}

//The number of ordered pairs in the DependencyGraph
int DependencyGraph::Size()
{
  return this->num_pairs;
}

//The size of dependees(s)
int DependencyGraph::DependeesSize(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got_dependees = this->dependees_map.find(s);
  if (got_dependees == this->dependees_map.end())
    return 0;
  else
    return got_dependees->second.size();
}

//Reports whether dependents(s) is non-empty
bool DependencyGraph::HasDependents(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got_dependents = this->dependents_map.find(s);
  if (got_dependents == this->dependents_map.end())
    return false;
  else
  {
    if (got_dependents->second.size() > 0)
      return true;
    else
      return false;
  }
}

//Reports whether dependees(s) is non-empty
bool DependencyGraph::HasDependees(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got_dependees = this->dependees_map.find(s);
  if (got_dependees == this->dependees_map.end())
    return false;
  else
  {
    if (got_dependees->second.size() > 0)
      return true;
    else
      return false;
  }
}

//Enumerates dependents(s)
std::unordered_set<std::string> & DependencyGraph::GetDependents(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got_dependents = this->dependents_map.find(s);
  if (got_dependents == this->dependents_map.end())
    return new std::unordered_set<std::string>();
  else
  {
    std::unordered_set<std::string> * get_dependents = &got_dependents->second;
    return &get_dependents;
  }
}

//Enumerates dependees(s)
std::unordered_set<std::string> &DependencyGraph::GetDependees(std::string s)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got_dependees = this->dependees_map.find(s);
  if (got_dependees == this->dependees_map.end())
    return std::unordered_set<std::string>();
  else
  {
    std::unordered_set<std::string> get_dependees = got_dependees->second;
    return get_dependees;
  }
}

//Adds the ordered pair (s, t), if it doesn't exist
void DependencyGraph::AddDependency(std::string s, std::string t)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got_dependents = this->dependents_map.find(s);
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got_dependees = this->dependees_map.find(t);
  
  //CASE 1: the dependency graph does not contain s or t
  if ((got_dependents == this->dependents_map.end()) && (got_dependees == this->dependees_map.end()))
  {
    std::unordered_set<std::string> dependees_set = std::unordered_set<std::string>();
    std::unordered_set<std::string> dependents_set = std::unordered_set<std::string>();
    
    dependees_set.insert(s);
    dependents_set.insert(t);
    
    this->dependents_map.insert(s, dependents_set);
    this->dependees_map.insert(t, dependees_set);
    
    this->num_pairs++;
    
    return;
  }

  //CASE 2: s already exists in the dependentsMap but t is not in the set mapped to s
  else if ((got_dependees == this->dependees_map.end()) && (got_dependents != this->dependents_map.end()))
  {
    got_dependents->second.insert(t);
    
    std::unordered_set<std::string> new_dependees_set = std::unordered_set<std::string>();
    new_dependees_set.insert(s);
    this->dependees_map.insert(t, new_dependees_set);
    
    got_dependents->second.insert(t);
    
    this->num_pairs++;
    
    return;
  }
  
  //CASE 3: t already exists in the dependeesMap but s is not in the set mapped to t
  else if ((got_dependents == this->dependents_map.end()) && (got_dependees != this->dependees_map.end()))
  {
    got_dependees->second.insert(s);
    
    std::unordered_set<std::string> new_dependents_set = std::unordered_set<std::string>();
    new_dependents_set.insert(t);
    this->dependents_map.insert(s, new_dependents_set);
    
    got_dependees->second.insert(s);
    
    this->num_pairs++;
    
    return;
  }
  
  //CASE 4: s and t already exist in the dependeesMap and dependentsMap
  else
  {
    if ((got_dependents != this->dependents_map.end()) && (got_dependees != this->dependees_map.end()))
    {
      std::unordered_set<std::string>::const_iterator got_dependents_set = got_dependents->second.find(t);
      std::unordered_set<std::string>::const_iterator got_dependees_set = got_dependees->second.find(s);
      
      if ((got_dependents_set == got_dependents->second.end()) && (got_dependees_set == got_dependees->second.end()))
      {
	got_dependents->second.insert(t);
	got_dependees->second.insert(s);
	
	this->num_pairs++;
	
	return;
      }
      else
	return;
    }
  }
}

void DependencyGraph::RemoveDependency(std::string s, std::string t)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got_dependents = this->dependents_map.find(s);
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got_dependees = this->dependees_map.find(t);
  std::unordered_set<std::string>::const_iterator got_dependents_set = got_dependents->second.find(t);
  std::unordered_set<std::string>::const_iterator got_dependees_set = got_dependees->second.find(s);
  
  if (((got_dependents != this->dependents_map.end()) && (got_dependents_set != got_dependents->second.end())) && 
      ((got_dependees != this->dependes_map.end()) && (got_dependees_set != got_dependees->second.end())))
  {
    got_dependents->second.erase(t);
    got_dependees->second.erase(s);
    
    this->num_pairs--;
    
    return;
  }
  else
    return;
}

void DependencyGraph::ReplaceDependents(std::string s, std::unordered_set<std::string> new_dependents)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got_dependents = this->dependents_map.find(s);
  
  //CASE 1: check if s is not a key in the dependentsMap
  if (got_dependents == this->dependents_map.end())
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

void DependencyGraph::ReplaceDependees(std::string s, std::unordered_set<std::string> new_dependees)
{
  std::unordered_map<std::string, std::unordered_set<std::string>>::const_iterator got_dependees = this->dependees_map.find(s);
  
  //CASE 1: check if s is not a key in the dependeesMap
  if (got_dependees = this->dependees_map.end())
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

