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
  this->revert_history = new std::vector<std::vector<std::string> *>();

  for (int i = 0; i < DEFAULT_CELL_COUNT; i++)
    {
      this->cell_history->push_back(new std::vector<std::string>());
      this->revert_history->push_back(new std::vector<std::string>());
    }

}

/*
 * Destructor for this spreadsheet
 */
spreadsheet::~spreadsheet()
{
  for (int i = 0; i < DEFAULT_CELL_COUNT; i++)
    {
      delete (*this->cell_history)[i];
      delete (*this->revert_history)[i];
    }
  this->cell_history->clear();
  this->revert_history->clear();
  delete this->cell_history;
  delete this->revert_history;
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
 * Changes the password if the user exists, returns false if the same password is given or the user doesn't exist
 */
bool spreadsheet::change_user(std::string user, std::string new_pass)
{
  // Case no user
  if (users->find(user) == users->end())
    return false;

  // Case pass is the same
  if ((*users)[user] == new_pass)
    return false;

  (*users)[user] = new_pass;
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

  // Replace dependents and update cell
  this->dependencies->ReplaceDependents(cell, *dep_set);
 
  this->spd_history->push_back(cell);
  (*(*(this->cell_history))[cell_idx]).push_back(contents);

  delete dep_set;
  
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

  int is_revert = cell.find("_REVERT");

  // Reverts have unique handling
  if (is_revert > 0)
    {
      cell = cell.substr(0, is_revert);
      int idx = cell_to_index(cell);
      std::string prev_revert = (*revert_history)[idx]->back();
      (*revert_history)[idx]->pop_back();
      (*cell_history)[idx]->push_back(prev_revert);
      return cell + "\t" + prev_revert;
    }

  // Just pop the contents of the specified cell
  else 
    {
      int idx = cell_to_index(cell);
      (*cell_history)[idx]->pop_back();

      if ((*cell_history)[idx]->empty())
	return cell + "\t" + "";
      return cell + "\t" + (*cell_history)[idx]->back();
    }
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
  std::vector<std::string> * revert_hist = (*(this->revert_history))[cell_idx];
  std::unordered_set<std::string> * dep_set = new std::unordered_set<std::string>();

  // No history just means 
  if (cell_hist->size() == 0)
    return "";

  //Get recent contents and push to revert history
  std::string curr_cont = cell_hist->back();
  cell_hist->pop_back();
  revert_hist->push_back(curr_cont);
  spd_history->push_back(cell + "_REVERT");

  // No history just means 
  if (cell_hist->size() == 0)
    return "";

  if ((*cell_hist).back()[0] == '=')
  {
    std::vector<std::string> * deps = cells_from_formula((*cell_hist).back());

    if (CircularDependency(cell, deps))
    {
      cell_hist->push_back(curr_cont);
      revert_hist->pop_back();
      spd_history->pop_back();
      return "\t";
    }

    for (std::string cell_dep : *deps)
      dep_set->insert(cell_dep);
    
  }

  dependencies->ReplaceDependents(cell, *dep_set);

  delete dep_set;
  
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
  ret_idx = ret_idx * 99 + row_i;
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
      std::string cell = formula.substr(idx, num_idx);
      
      // Push back cell, find next cell
      dependencies->push_back(cell);
      idx = formula.find_first_of("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", idx + 1);
    }
  
  return &(*dependencies);
  
}

//Function that detects circular dependcies in the spreadsheet
bool spreadsheet::CircularDependency(std::string cell, std::vector<std::string> * dependencies)
{
  
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
  std::queue <std::string> queue;
  queue.push(start);
  
  while (!queue.empty())
  {
  
    std::string start = queue.front();
    queue.pop();
    if (start == goal)
      return true;
    else
    {
      std::unordered_set<std::string> * get_dependents = this->dependencies->GetDependents(start);
      for (std::string s : *get_dependents)
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
std::vector<std::string> * spreadsheet::get_cell_history(int cell_as_num)
{

  // Return empty vector for out of range
  if (cell_as_num < 0 || cell_as_num > DEFAULT_CELL_COUNT)
    return new std::vector<std::string>();
  
  return &(*(*cell_history)[cell_as_num]);
}

/*
 * Returns a vector of the history of reverts for a cell specified as a number
 * Returns an empty vector for out of range cells
 */
std::vector<std::string> * spreadsheet::get_revert_history(int cell_as_num)
{
  // Return empty vector for out of range
  if (cell_as_num < 0 || cell_as_num > DEFAULT_CELL_COUNT)
    return new std::vector<std::string>();
  
  return &(*(*revert_history)[cell_as_num]);
}

/*
 * Returns the history of edits for this spreadsheet
 */
std::vector<std::string> * spreadsheet::get_sheet_history()
{
  return &(*spd_history);
}

/*
 * Returns the contents of the specified cell, or null if non existant
 */
std::string spreadsheet::get_cell_contents(std::string cell)
{
  
  int index = cell_to_index(cell);
  
  if (index < 0)
    {
    return "";
    }
  

  if ((*cell_history)[index]->size() == 0)
    return "";

  
  return (*cell_history)[index]->back();
}

/*
 * Sets the history of the spreadsheet directly to the provided vector
 * Undefined histories cause undefined behavior, so use of this method
 * outisde of the specified format is not recommended
 */
void spreadsheet::add_direct_sheet_history(std::vector<std::string> * hist)
{
  delete this->spd_history;
  this->spd_history = hist;
}

/*
 * Sets the history of the specified cell directly to the provided vector
 * Undefined histories cause undefined behavior, so use of this method
 * outisde of the specified format is not recommended
 *
 * Does nothing for cells outside of the range of the spreadsheet
 */
void spreadsheet::add_direct_cell_history(int cell, std::vector<std::string> * hist)
{
  if (cell < 0 || cell > DEFAULT_CELL_COUNT)
    return;

  // Delete old history and overwrite
  (*(this->cell_history))[cell] = hist;

  std::string contents = hist->back();
  if (contents[0] == '=')
    {
      std::vector<std::string> * deps = cells_from_formula(hist->back());
      char letter = (char)((cell / 99) + 65);
      int row = (cell % 99) + 1;
      std::string cell(1, letter);
      cell += std::to_string(row);
      
      std::unordered_set<std::string> * dep_set = new std::unordered_set<std::string>();
      
      for (std::string cell_dep : *deps)
	dep_set->insert(cell_dep);

      
      dependencies->ReplaceDependents(cell, *dep_set);


    }
}

/*
 * Sets the revert history of the specified cell directly to the provided vector
 * Undefined histories cause undefined behavior, so use of this method
 * outisde of the specified format is not recommended
 *
 * Does nothing for cells outside of the range of the spreadsheet
 */
void spreadsheet::add_direct_revert_history(int cell, std::vector<std::string> * hist)
{
if (cell < 0 || cell > DEFAULT_CELL_COUNT)
    return;

  // Delete old history and overwrite
  (*(this->revert_history))[cell] = hist;
}


/*
 * Returns the name of this spreadsheet
 */
std::string spreadsheet::get_name()
{
  return this->name;
}
