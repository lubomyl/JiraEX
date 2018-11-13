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

        public static string Build(Sprint[] sprints, bool isAssignedToMe, bool isUnassigned, Priority[] priorities, Status[] statuses, Project[] projects, string searchText)
        {
            jql = "";

            ProcessSprints(sprints);
            ProcessAssignedToMe(isAssignedToMe);
            ProcessUnassigned(isUnassigned);
            ProcessPriorities(priorities);
            ProcessStatuses(statuses);
            ProcessProjects(projects);
            ProcessSearchText(searchText);

            return jql;
        }

        private static void ProcessSprints(Sprint[] sprints)
        {
            if (sprints.Any(sprint => sprint.CheckedStatus))
            {
                AppendParameterNameIn("sprint");

                foreach (Sprint s in sprints)
                {
                    if (s.CheckedStatus)
                    {
                        AddParameterValueIn(s.Id);
                    }
                }

                CloseParameterValuesIn();
            }
        }

        private static void ProcessAssignedToMe(bool isAssignedToMe)
        {
            if (isAssignedToMe)
            {
                AppendParameterNameIn("assignee");
                AddParameterValueAssignedUser("currentUser()");
                CloseParameterValuesIn();
            }
        }

        private static void ProcessUnassigned(bool isUnassigned)
        {
            if (isUnassigned)
            {
                AppendParameterNameIn("assignee");
                AddParameterValueAssignedUser("EMPTY");
                CloseParameterValuesIn();
            }
        }

        private static void ProcessPriorities(Priority[] priorities)
        {
            if (priorities.Any(priority => priority.CheckedStatus))
            {
                AppendParameterNameIn("priority");

                foreach (Priority p in priorities)
                {
                    if (p.CheckedStatus)
                    {
                        AddParameterValueIn(p.Name);
                    }
                }

                CloseParameterValuesIn();
            }
        }

        private static void ProcessStatuses(Status[] statuses)
        {
            if (statuses.Any(status => status.CheckedStatus))
            {
                AppendParameterNameIn("status");

                foreach (Status s in statuses)
                {
                    if (s.CheckedStatus)
                    {
                        AddParameterValueIn(s.Name);
                    }
                }

                CloseParameterValuesIn();
            }
        }

        private static void ProcessProjects(Project[] projects)
        {
            if (projects.Any(project => project.CheckedStatus))
            {
                AppendParameterNameIn("project");

                foreach (Project p in projects)
                {
                    if (p.CheckedStatus)
                    {
                        AddParameterValueIn(p.Name);
                    }
                }

                CloseParameterValuesIn();
            }
        }

        private static void ProcessSearchText(string searchText)
        {
            if (searchText != null && !searchText.Equals(""))
            {
                AppendParameterNameLike("text");
                AddParameterValueLike(searchText + "*");
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
                jql += "\"" + value + "\"";
            } else
            {
                jql += "," + "\"" + value + "\"";
            }
        }

        private static void AddParameterValueAssignedUser(string value)
        {
            if (jql[jql.Length - 1] == '(')
            {
                jql += value;
            }
            else
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
