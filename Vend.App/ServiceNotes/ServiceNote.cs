using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vend.Lib;

namespace Vend.App.ServiceNotes
{
    public class ServiceNote
    {
        public enum IssueType
        {
            [Description("General")]
            general=0,
            [Description("Can Rack")]
            canrack,
            [Description("Coin Box")]
            coinbox,
            [Description("Purchase Price")]
            purchaseprice,
        }

        public ServiceNote()
        {
            
        }
        public ServiceNote(string note,IssueType issueCategory)
        {
            Note = note;
            IssueCategory = issueCategory;
            Logged = DateTime.Now;
            LoggedBy = Environment.UserName;
        }

        public string Note { get; set; }
        public IssueType IssueCategory { get; set; }
        public DateTime Logged { get; set; }
        public string LoggedBy { get; set; }

        public static Dictionary<IssueType, string> GetIssueTypes()
        {
            var dict = new Dictionary<IssueType, string>();
            foreach(IssueType t in Enum.GetValues(typeof(IssueType)))
            {
                dict.Add(t, Utility.GetFriendlyName<IssueType>(t));
            }

            return dict;

        }
    }
}
