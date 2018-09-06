using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service
{
    public interface ISprintService
    {

        Task<SprintList> GetAllSprintsOfBoardtAsync(int boardId);

    }
}
