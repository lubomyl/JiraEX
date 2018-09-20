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
        /// <summary>
        /// Async method to delete issue's attachment by it's id.
        /// </summary>
        /// <returns>Task representing api call.</returns>
        Task DeleteAttachmentByIdAsync(string attachmentId);

    }
}
