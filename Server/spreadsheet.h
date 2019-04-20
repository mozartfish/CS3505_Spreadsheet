/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last Revision: 4/15/19
 *
 * A header class for representing spreadsheet objects
 */

#include <stack>
#include <string>
#include <sstream>
#include <unordered_map>
#include <unordered_set>
#include <vector>
#include "DependencyGraph.h"


#ifndef SPRD
#define SPRD

#define DEFAULT_CELL_COUNT 2600 //26x100 spreadsheet

class spreadsheet {

 private:
  std::string name;
  std::unordered_set<int> * listeners;
  std::vector<std::string> * spd_history;
  std::vector<std::vector<std::string>*> * cell_history;
  std::unordered_map<std::string, std::string> * users;
  DependencyGraph * dependencies;

  

 public:
  spreadsheet(std::string name);
  ~spreadsheet();

  void add_listener(int fd);
  void remove_listener(int fd);
  const std::unordered_set<int> & get_listeners();

  bool add_user(std::string user, std::string pass);
  bool remove_user(std::string user);
  bool change_user(std::string user, std::string new_pass);
  bool change_cell(std::string cell, std::string contents, std::vector<std::string> * dependencies);
  std::string undo();
  std::string revert(std::string cell);
  std::unordered_map<std::string, std::string> & get_users();
  std::vector<std::string> & get_cell_history(int cell_as_num);
  std::vector<std::string> & get_sheet_history();
  std::string get_cell_contents(std::string cell);

  void add_direct_sheet_history(std::vector<std::string> hist);
  void add_direct_cell_history(int cell, std::vector<std::string> & hist);
  std::string get_name();
  
  bool CircularDependency(std::string cell, std::vector<std::string> * dependencies);
  std::vector<std::string> * cells_from_formula(std::string formula);
  bool Visit (std::string start, std::string goal);

  static int cell_to_index(std::string cell);
};

#endif
