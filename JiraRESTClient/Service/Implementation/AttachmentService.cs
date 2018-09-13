using AtlassianConnector.Base.Implementation.DevDefined;
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

        private JiraService _baseService;

        public AttachmentService()
        {
            this._baseService = BaseService.JiraInstance;
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
