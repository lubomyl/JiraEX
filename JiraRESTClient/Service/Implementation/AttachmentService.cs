using AtlassianConnector.Base;
using AtlassianConnector.Base.Implementation.DevDefined;
using AtlassianConnector.Base.Implementation.RestSharp;
using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service.Implementation
{

    /// <summary>
    /// Concrete implementation of IAttachmentService utilizing <see cref="BaseService"/> as <see cref="IBaseService{T}"/>.
    /// <see cref="IAttachmentService"/>
    /// </summary>
    public class AttachmentService : IAttachmentService
    {

        private IBaseJiraService _baseService;

        public AttachmentService(AuthenticationType type)
        {
            if (type == AuthenticationType.Basic)
            {
                this._baseService = BaseService.JiraInstance;
            } 
            else if (type == AuthenticationType.OAuth)
            {
                this._baseService = BaseOAuthService.JiraInstance;
            }
        }

        /// <summary>
        /// <see cref="IAttachmentService.DeleteAttachmentByIdAsync(string)"/>
        /// </summary>
        public Task DeleteAttachmentByIdAsync(string attachmentId)
        {
            return Task.Run(() =>
            {
                var resource = $"attachment/{attachmentId}";

                this._baseService.DeleteResource(resource);
            });
        }

    }
}
