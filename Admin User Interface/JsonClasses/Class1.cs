///Joanna Lowry && Cole Jacobs && Aaron Carlisle
///Represents the message classes for the sever-based spreadsheet
///as specified by the SendIt communications protocol
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonClasses
{
    /// <summary>
    /// SpreadsheetList message type
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class SpreadsheetList
    {
        [JsonProperty(PropertyName = "list")]
        public string type { get; set; }
        public string[] spreadsheets { get; set; }
        
        public SpreadsheetList()
        {
            type = "list";
        }
    }

    /// <summary>
    /// Represents the FullSend message type
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class FullSend
    {
        [JsonProperty(PropertyName = "full send")]
        public string type { get; set; }
        public Dictionary<string, string> spreadsheet { get; set; }
        
        public FullSend()
        {
            type = "full send";
            
        }

    }

    /// <summary>
    /// Represents the Open message type
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class Open
    {
        [JsonProperty(PropertyName = "open")]
        public string type { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        public Open()
        {
            type = "open";
        }

        public string ToString()
        {
            string returning = "type : " + type +"\nname : " + name + "\nusername : " + username + "\npassword : " + password + "\n";
            return returning;
        }
    }

    /// <summary>
    /// Represents the Edit message type
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class Edit
    {
        [JsonProperty(PropertyName = "edit")]
        public string type { get; set; }
        public string cell { get; set; }
        public string value { get; set; }
        public string dependencies { get; set; }
        public Edit()
        {
            type = "edit";
        }
    }

    /// <summary>
    /// Represents the Undo message type
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class Undo
    {
        [JsonProperty(PropertyName = "undo")]
        public string type { get; set; }
        public Undo()
        {
            type = "undo";
        }
    }

    /// <summary>
    /// Represents the Revert message type
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class Revert
    {
        [JsonProperty(PropertyName = "revert")]
        public string type { get; set; }
        public string cell { get; set; }
        public Revert()
        {
            type = "revert";
        }
    }

    /// <summary>
    /// Represents the Error message type
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class Error
    {
        [JsonProperty(PropertyName = "error")]
        public string type { get; set; }
        public int code { get; set; }
        public string source { get; set; }

        public Error()
        {
            type = "error";
        }
    }
}
