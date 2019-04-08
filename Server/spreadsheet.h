/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last Revision: 4/1/19
 *
 * A header class for representing spreadsheet objects
 */

#include <stack>
#include <string>
#include <unordered_map>
#include "DependencyGraph.h"


#ifndef SPRD
#define SPRD

#define DEFAULT_CELL_COUNT 2600 //26x100 spreadsheet

class spreadsheet {

 private:
  std::string name;
  std::stack<std::string> * spd_history;
  std::stack<std::string> ** cell_history;
  std::unordered_map<std::string, std::string> * users;
  DependencyGraph * dependencies;

  static int cell_to_index(std::string cell);

 public:
  spreadsheet(std::string name);
  ~spreadsheet();

  bool add_user(std::string user, std::string pass);
  bool remove_user(std::string user);
  bool change_cell(std::string cell, std::string contents);
  std::string undo();
  std::string revert(std::string cell);
  std::unordered_map<std::string, std::string> & get_users();
};

#endif
