/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last Revision: 4/1/19
 *
 * Contains all function definitions for a spreadsheet object
 */

#include <string>
#include <stack>
#include <unordered_map>
#include <sstream>
#include "spreadsheet.h"

/*
 *  Constructs a new spreadsheet of the given name, with empty cells
 *  and no users
 */
spreadsheet::spreadsheet(std::string name)
{
  this->name = name;
  this->spd_history = new std::stack<std::string>();
  this->users = new std::unordered_map<std::string, std::string>();
  this->dependencies = new DependencyGraph();
  this->cell_history = new std::stack<std::string>* [DEFAULT_CELL_COUNT];

  for (int i = 0; i < DEFAULT_CELL_COUNT; i++)
    {
      this->cell_history[i] = new std::stack<std::string>();
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
bool spreadsheet::change_cell(std::string cell, std::string contents)
{
  int cell_idx = cell_to_index(cell);
  if (cell_idx < 0 || cell_idx >= DEFAULT_CELL_COUNT)
    return false;
}

/*
 *  Performs an undo on this spreadsheet, and returns the contents of
 *  the last edit, with the cell name attached to the beginning
 *  Returns NULL if no previous edits
 */
std::string spreadsheet::undo()
{
  if (spd_history->empty())
    return NULL;

  std::string cell = spd_history->top();
  spd_history->pop();

  // Revert the cell that had the most recent edit
  return this->revert(cell);
  
  
}

/*
 * Reverts the contents of a cell to what they previously were, 
 * returning those contents, or NULL if the revert failed
 */
std::string spreadsheet::revert(std::string cell)
{
  int cell_idx = cell_to_index(cell);
  if (cell_idx < 0 || cell_idx >= DEFAULT_CELL_COUNT)
    return NULL;

  std::stack<std::string> * cell_stk = cell_history[cell_idx];
  
  //TODO use dependency graph to make sure circ dep won't exist
  std::string curr_cont = cell_stk->top();
  cell_stk->pop();
  
  // Return the current top contents
  return cell_stk->top();
}

/*
 * Turns a cell name into a numerical index
 * returns -1 if any characters are not alphanumeric
 */
int spreadsheet::cell_to_index(std::string cell)
{
  int ret_idx = 0;
  bool in_nums = false;
  std::string mult = "";

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
	  in_nums = true;
	  mult.push_back(cell[i]);
	}

	    // Undefined cell
	  else
	    return -1;
    } 

      // Multiply alphabetical index by numerical index
      ret_idx *= std::stoi(mult);
      return ret_idx;
}

bool spreadsheet::CircularDependency(std::string cell, std::string Formula)
{
  //https://stackoverflow.com/questions/16029324/c-splitting-a-string-into-an-array
  
  std::string string_tokens[Formula.length()];
  int j = 0;
  std:: stringstream read_tokens(Formula);
  while (read_tokens.good() && j < Formula.length())
  {
    read_tokens >> string_tokens[j];
    j++;
  }
  
  for (std::string s : string_tokens)
  {
    if (s.compare(cell) == 0)
      return true;
  }
  
  return false;
  
}
