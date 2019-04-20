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
  
  
  return 0;
  
}
