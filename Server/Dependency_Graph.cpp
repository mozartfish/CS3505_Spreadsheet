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
