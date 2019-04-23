/*
 *This file tests the compilation and function of the dependency graph
 *Authors:Thomas Ady, Pranav Rajan
 *Last Updated: April 6, 2019
 */

#include "DependencyGraph.h"
#include <unordered_set>
#include <unordered_map>
#include <string>
#include <iostream>

using namespace std;

int main()
{
  /*
//Empty Graph should contain nothing
  {
    DependencyGraph *t = new DependencyGraph();
    bool b;
    
    b = (t->Size() == 0);
    
    if (b)
    {
      cout << "Test zero size passed" << endl;
    }
    else
      cout << "Test zero size failed" << endl;
  }
  
//Empty Graph should have nothing
  {
    DependencyGraph *t = new DependencyGraph();
    bool b;
    bool c;
    
    b = (t->HasDependees("x"));
    c = (t->HasDependents("x"));

    if (b == 0)
      cout << "HasDependees function works" << endl;
    else
      cout << "HasDependents function fails"<< endl;
    
    if (c == 0)
      cout << "HasDependents function works" << endl;
    else
      cout << "HasDependents function fails" << endl;
    
  }
  
//Remove Dependency Test
  {
    DependencyGraph *t = new DependencyGraph();
    t->RemoveDependency("x", "y");
    cout << "called the remove function on an empty set" << endl;
  }

//Replace on an empty DG shouldn't fail
  {
    DependencyGraph *t = new DependencyGraph();
    std::unordered_set<std::string> empty_set = std::unordered_set<std::string>();
    t->ReplaceDependents("x", empty_set);
    t->ReplaceDependees("y", empty_set);
  }
  */
  DependencyGraph g;
  string cell("B5");
  cout << "printing deps" << endl;
  
  for (string h : *(g.GetDependents(cell)))
    cout << h << endl;

  cout << "adding deps" << endl;

  unordered_set<string> deps;
  deps.insert("B6");

  g.ReplaceDependents(cell, deps);

  for (string h : *(g.GetDependents(cell)))
    cout << h << endl;

  cout << "replacing deps" << endl;

  unordered_set<string> * dep2 = new unordered_set<string>();
  g.ReplaceDependents(cell, *dep2);

  for (string h : *(g.GetDependents(cell)))
    cout << h << endl;
  
  dep2->insert("B7");

  cout << "replacing deps" << endl;

  g.ReplaceDependents(cell, *dep2);

  for (string h : *(g.GetDependents(cell)))
    cout << h << endl;
  
  return 0;
  
}
