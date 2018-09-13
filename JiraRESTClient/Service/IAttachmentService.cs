using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service
{

    /// <summary>
    /// Interface providing methods to access resources connected to Attachment object from Jira app.
    /// </summary>
    public interface IAttachmentService
    {

        Task DeleteAttachmentByIdAsync(string attachmentId);

    }
}
