/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last Revision: 4/1/19
 *
 * A header class for representing spreadsheet objects
 */

#include <stack>
#include <string>

class spreadsheet {

 private:
  std::stack<std::string> spd_history;
  std::stack<std::string> * cell_history;

 public:
  spreadsheet();
  ~spreadsheet();
};
