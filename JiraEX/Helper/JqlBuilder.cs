using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.Helper
{
    public class JqlBuilder
    {

        private static string jql = "";

        public static string Build(Sprint[] sprints, User[] assignees, Priority[] priorities, Status[] statuses, Project[] projects, string searchText)
        {
            jql = "";
            
            ProcessPriorities(priorities);
            ProcessSearchText(searchText);

            return jql;
        }

        private static void ProcessPriorities(Priority[] priorities)
        {
            int counter = 0;

            if (priorities.Length > 0)
            {
                AppendParameterNameIn("priority");

                foreach (Priority p in priorities)
                {
                    if (p.CheckedStatus)
                    {
                        counter++;
                        AddParameterValueIn(p.Name);
                    }
                }

                CloseParameterValuesIn();

                if (counter == 0)
                {
                    jql = "";
                }
            }
        }

        private static void ProcessSearchText(string searchText)
        {
            if (searchText != null && !searchText.Equals(""))
            {
                AppendParameterNameLike("text");
                AddParameterValueLike(searchText);
                CloseParameterValuesLike();
            }
        }

        private static void AppendParameterNameIn(string parameterName)
        {
            if(jql.Length == 0)
            {
                jql += parameterName + " in (";
            } else
            {
                jql += "AND " + parameterName + " in (";
            }
        }

        private static void AppendParameterNameLike(string parameterName)
        {
            if (jql.Length == 0)
            {
                jql += parameterName + " ~ \"";
            }
            else
            {
                jql += "AND " + parameterName + " ~ \"";
            }
        }

        private static void AddParameterValueIn(string value)
        {
            if(jql[jql.Length - 1] == '(')
            {
                jql += value;
            } else
            {
                jql += "," + value;
            }
        }

        private static void AddParameterValueLike(string value)
        {
           jql += value;
        }

        private static void CloseParameterValuesIn()
        {
            jql += ")";
        }

        private static void CloseParameterValuesLike()
        {
            jql += "\"";
        }
    }
}
