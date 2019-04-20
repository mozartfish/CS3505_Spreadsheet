/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last Revision: 4/15/19
 *
 * Contains all function definitions for a spreadsheet object
 */

#include <string>
#include <stack>
#include <vector>
#include <unordered_map>
#include <unordered_set>
#include <sstream>
#include "spreadsheet_j.h"
#include <queue>
#include <iostream>
#include "jsoncpp-master/dist/json/json.h"

/*
 *  Constructs a new spreadsheet of the given name, with empty cells
 *  and no users
 */
spreadsheet::spreadsheet(std::string name)
{
this->name = name;
this->listeners = new std::unordered_set<int>();
sp_root = new Json::Value();

}

/*
 * Destructor for this spreadsheet
 */
spreadsheet::~spreadsheet()
{
delete this->sp_root;
delete this-> listeners;
}

/*
 * Adds a socket fd listener to the spreadsheet
 */
void spreadsheet::add_listener(int fd)
{
  this->listeners->insert(fd);
}

/*
 * Removes a socket fd listener from the spreadsheet
 */
void spreadsheet::remove_listener(int fd)
{
  this->listeners->erase(fd);
}

/*
 * Returns the list of all current listeners to this spreadsheet
 */
const std::unordered_set<int> & spreadsheet::get_listeners()
{
  return *(this->listeners);
}

/*
 *  Adds a user to this spreadsheet with the given username and password, or changes password if it is different
 *  Returns true if the user did not exist previously
 */
bool spreadsheet::add_user(std::string user, std::string pass)
{
(*sp_root)["users"][user] = pass;
  return true;
}

/*
 * Returns whether or not the specified user could be removed from
 * the list of users
 */
bool spreadsheet::remove_user(std::string user)
{
(*sp_root)["users"].removeMember(user);
  return true;
}

/*
 * Returns a map of users and passwords for this spreadsheet
 */
std::unordered_map<std::string, std::string> & spreadsheet::get_users()
{
  return ;
}

/*
 *  Applies an edit to a cell in this spreadsheet with the given
 *  cell and new contents, returns true if the edit can be processed
 *
 */
bool spreadsheet::change_cell(std::string cell, std::string contents, std::vector<std::string> * dep_list)
{
  

  std::unordered_set<std::string> * dep_set = new std::unordered_set<std::string>();

  //check for circular dependencies
  if (contents[0] == '=')
  {
    if (CircularDependency(cell, dep_list))
      return false;
    
    for (std::string cell_dep : *dep_list)
      dep_set->insert(cell_dep);
    
  }

  this->dependencies->ReplaceDependents(cell, *dep_set);
  
  this->spd_history->push_back(cell);
  (*(this->cell_history))[cell_idx]->push_back(contents);
  
  return true;
}

/*
 *  Performs an undo on this spreadsheet, and returns the contents of
 *  the last edit, with the cell name attached to the beginning separated by \t
 *  Returns empty string if no previous edits
 */
std::string spreadsheet::undo()
{
  std::string empty("");
  if (this->spd_history->empty())
    return empty;

  std::string cell = this->spd_history->back();
  this->spd_history->pop_back();

  // Revert the cell that had the most recent edit
  return cell + "\t" + this->revert(cell);
}

/*
 * Reverts the contents of a cell to what they previously were, 
 * returning those contents, or \t (unusable character for clients) if the revert failed
 */
std::string spreadsheet::revert(std::string cell)
{
if ((*sp_root)[cell].empty())
  return "";

std::unordered_set<std::string> * dep_set = new std::unordered_set<std::string>();

  //TODO use dependency graph to make sure circ dep won't exist
  std::string curr_cont = cell_hist->back();
  cell_hist->pop_back();
  if (cell_hist[cell_idx].back()[0] == '=')
  {
    std::vector<std::string> * deps = cells_from_formula((*cell_hist).back());
    if (CircularDependency(cell, deps))
    {
      cell_hist->push_back(curr_cont);
      return "\t";
    }

    for (std::string cell_dep : *deps)
      dep_set->insert(cell_dep);
    
  }

  dependencies->ReplaceDependents(cell, *dep_set);
  
  // Return the current top contents
  return cell_hist->back();
}

/*
 * Turns a cell name into a numerical index
 * returns -1 if any characters are not alphanumeric
 */
int spreadsheet::cell_to_index(std::string cell)
{
  int ret_idx = 0;
  bool in_nums = false;
  std::string row;

  for (int i = 0; i < cell.length(); i++)
    {
      int curr = (int)cell[i];
      
      //a->z
      if (curr >= 97 && curr <= 122 && !in_nums)
	ret_idx += (curr - 97);

      //A->Z
      else if (curr >= 65 && curr <= 90 && !in_nums)
	ret_idx += (curr - 65);

      //0->9
      else if (curr >= 48 && curr <= 57 && i > 0)
	{
	  row = cell.substr(i, cell.size());
	  break;
	}

	    // Undefined cell
	  else
	    return -1;
    } 

  // Multiply alphabetical index by numerical index
  int row_i = std::stoi(row) - 1;
  ret_idx = ret_idx * 100 + row_i;
  return ret_idx;
}

/*
 * Returns a list of all cells in the given formula
 */
std::vector<std::string> *  spreadsheet::cells_from_formula(std::string formula)
{
  std::vector<std::string> * dependencies = new std::vector<std::string>();

  // Find any letter occurances
  int idx = formula.find_first_of("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
  while(idx != -1)
    {
      int num_idx;
      
      // Find length of digits (Finds inclusive index of last num)
      for (num_idx = formula.find_first_of("0123456789", idx + 1); 
	   (formula.find_first_of("0123456789", num_idx + 1) - num_idx) == 1; num_idx++);

      // Get cell as substring
      std::string cell = formula.substr(idx, num_idx + 1);
      
      // Push back cell, find next cell
      dependencies->push_back(cell);
      idx = formula.find_first_of("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", idx + 1);
    }
  
  return &(*dependencies);
  
}

//Function that detects circular dependcies in the spreadsheet
bool spreadsheet::CircularDependency(std::string cell, std::vector<std::string> * dependencies)
{
  //https://stackoverflow.com/questions/16029324/c-splitting-a-string-into-an-array
  
  /*std::string string_tokens[Formula.length()];
  int j = 0;
  std:: stringstream read_tokens(Formula);
  while (read_tokens.good() && j < Formula.length())
  {
    read_tokens >> string_tokens[j];
    j++;
    } */
  
  for (std::string s : *dependencies)
  {
    if (Visit(s, cell))
      return true;
  }
  
  return false;
  
}

//Helper function that performs a DFS on the dependency graph looking for circular dependcies
bool spreadsheet::Visit(std::string start, std::string goal)
{
  std::queue <std::string> queue = std::queue <std::string>();
  queue.push(start);
  
  while (!queue.empty())
  {
    std::string start = queue.front();
    if (start == goal)
      return true;
    else
    {
      std::unordered_set<std::string> get_dependents = this->dependencies->GetDependents(start);
      for (std::string s : get_dependents)
      {
	queue.push(s);
      }
    }
  }
  
  return false;
}



/*
 * Returns the contents of the specified cell, or null if non existant
 */
std::string spreadsheet::get_cell_contents(std::string cell)
{
  

  
}


/*
 * Returns the name of this spreadsheet
 */
std::string spreadsheet::get_name()
{
  return (*sp_root)["name"].asString();
}

/*
 * Returns the json_encoding of this spreadsheet
 */
std::string get_json_encoding()
{
  return sp_root->toStyledString();
}
