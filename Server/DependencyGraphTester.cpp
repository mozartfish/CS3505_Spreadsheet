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
  
  {
    DependencyGraph *t = new DependencyGraph();
    cout << "called the dependency graph constructor" << endl;
    t->AddDependency("x", "y");
    cout << "called the add dependency function" << endl;
    
    int x = t->Size();
    
    cout << x << endl;
  }
  
  
  return 0;
  
}
