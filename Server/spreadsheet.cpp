/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last Revision: 4/1/19
 *
 * Contains all function definitions for a spreadsheet object
 */

#include <string>
#include <stack>
#include <vector>
#include <unordered_map>
#include <sstream>
#include "spreadsheet.h"
#include <queue>

/*
 *  Constructs a new spreadsheet of the given name, with empty cells
 *  and no users
 */
spreadsheet::spreadsheet(std::string name)
{
  this->name = name;
  this->spd_history = new std::vector<std::string>();
  this->users = new std::unordered_map<std::string, std::string>();
  this->dependencies = new DependencyGraph();
  this->cell_history = new std::vector<std::string>* [DEFAULT_CELL_COUNT];

  for (int i = 0; i < DEFAULT_CELL_COUNT; i++)
    {
      this->cell_history[i] = new std::vector<std::string>();
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

  //check for circular dependencies
  if (contents[0] == '=')
  {
    if (CircularDependency(cell, contents))
      return false;
  }
  else
  {
    spd_history->push_back(cell);
    cell_history[cell_idx]->push_back(contents);
    return true;
  }
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

  std::string cell = spd_history->back();
  spd_history->pop_back();

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

  std::vector<std::string> * cell_hist = cell_history[cell_idx];
  
  //TODO use dependency graph to make sure circ dep won't exist
  std::string curr_cont = cell_hist->back();
  cell_hist->pop_back();
  if (
  
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

//Function that detects circular dependcies in the spreadsheet
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
  
  return *cell_history[cell_as_num];
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
  int index = cell_to_index(cell);

  if (index < 0)
    return NULL;

  if (cell_history[index]->size() == 0)
    return NULL;

  return cell_history[index]->back();
}

/*
 * Sets the history of the spreadsheet directly to the provided vector
 * Undefined histories cause undefined behavior, so use of this method
 * outisde of the specified format is not recommended
 */
void spreadsheet::add_direct_sheet_history(std::vector<std::string> hist)
{
  delete spd_history;
  std::vector<std::string> * new_hist = &hist;
  spd_history = new_hist;
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
  delete cell_history[cell];
  std::vector<std::string> * cell_hist = &hist;
  cell_history[cell] = cell_hist;
}


/*
 * Returns the name of this spreadsheet
 */
std::string spreadsheet::get_name()
{
  return this->name;
}
