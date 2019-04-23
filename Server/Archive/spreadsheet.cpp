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
#include "spreadsheet.h"
#include <queue>
#include <iostream>

/*
 *  Constructs a new spreadsheet of the given name, with empty cells
 *  and no users
 */
spreadsheet::spreadsheet(std::string name)
{
  this->name = name;
  this->listeners = new std::unordered_set<int>();
  this->spd_history = new std::vector<std::string>();
  this->users = new std::unordered_map<std::string, std::string>();
  this->dependencies = new DependencyGraph();
  this->cell_history = new std::vector<std::vector<std::string> *>();

  for (int i = 0; i < DEFAULT_CELL_COUNT; i++)
    {
      this->cell_history->push_back(new std::vector<std::string>());
    }
}

/*
 * Destructor for this spreadsheet
 */
spreadsheet::~spreadsheet()
{
  delete this->cell_history;
  delete this->spd_history;
  delete this-> users;
  delete this-> dependencies;
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
 *  Adds a user to this spreadsheet with the given username and password
 *  Returns true if the user did not exist previously
 */
bool spreadsheet::add_user(std::string user, std::string pass)
{
  // If user is already in map
  if (users->find(user) != users->end())
    return false;
  
  (*users)[user] = pass;
  return true;
}

/*
 * Returns whether or not the specified user could be removed from
 * the list of users
 */
bool spreadsheet::remove_user(std::string user)
{
  // If user is not already in map
  if (users->find(user) == users->end())
    return false;

  users->erase(user);
  return true;
}

/*
 * Returns a map of users and passwords for this spreadsheet
 */
std::unordered_map<std::string, std::string> & spreadsheet::get_users()
{
  return (*users);
}

/*
 *  Applies an edit to a cell in this spreadsheet with the given
 *  cell and new contents, returns true if the edit can be processed
 *
 */
bool spreadsheet::change_cell(std::string cell, std::string contents, std::vector<std::string> * dep_list)
{
  int cell_idx = cell_to_index(cell);
  if (cell_idx < 0 || cell_idx >= DEFAULT_CELL_COUNT)
    return false;

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
  int cell_idx = cell_to_index(cell);
  if (cell_idx < 0 || cell_idx >= DEFAULT_CELL_COUNT)
    return "\t";

  std::vector<std::string> * cell_hist = (*(this->cell_history))[cell_idx];
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
 * Returns a vector of the history of edits for a cell specified as a number
 * Returns an empty vector for out of range cells
 */
std::vector<std::string> & spreadsheet::get_cell_history(int cell_as_num)
{

  // Return empty vector for out of range
  if (cell_as_num < 0 || cell_as_num > DEFAULT_CELL_COUNT)
    return *(new std::vector<std::string>());
  
  return *((*cell_history)[cell_as_num]);
}

/*
 * Returns the history of edits for this spreadsheet
 */
std::vector<std::string> & spreadsheet::get_sheet_history()
{
  return *spd_history;
}

/*
 * Returns the contents of the specified cell, or null if non existant
 */
std::string spreadsheet::get_cell_contents(std::string cell)
{
  std::cout << cell_history->size() << std::endl;
  std::cout << "hi" << std::endl;
  int index = cell_to_index(cell);
  std::cout << index << std::endl;
  if (index < 0)
    {
      std::cout << "bad index " << cell << std::endl;
    return NULL;
    }

  std::cout << index << std::endl;
  

  if ((*cell_history)[index]->size() == 0)
    return "";

  std::cout << index << std::endl;
  return (*cell_history)[index]->back();
}

/*
 * Sets the history of the spreadsheet directly to the provided vector
 * Undefined histories cause undefined behavior, so use of this method
 * outisde of the specified format is not recommended
 */
void spreadsheet::add_direct_sheet_history(std::vector<std::string> hist)
{
  delete this->spd_history;
  std::vector<std::string> * new_hist = &hist;
  this->spd_history = new_hist;
}

/*
 * Sets the history of the specified cell directly to the provided vector
 * Undefined histories cause undefined behavior, so use of this method
 * outisde of the specified format is not recommended
 *
 * Does nothing for cells outside of the range of the spreadsheet
 */
void spreadsheet::add_direct_cell_history(int cell, std::vector<std::string> & hist)
{
  if (cell < 0 || cell > DEFAULT_CELL_COUNT)
    return;

  // Delete old history and overwrite
  std::vector<std::string> * cell_hist = &hist;
  (*(this->cell_history))[cell] = cell_hist;
}


/*
 * Returns the name of this spreadsheet
 */
std::string spreadsheet::get_name()
{
  return this->name;
}
