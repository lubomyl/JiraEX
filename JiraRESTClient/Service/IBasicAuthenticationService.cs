﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service
{
    public interface IBasicAuthenticationService
    {

        void InitializeBasicAuthenticationAuthenticator(string baseUrl, string username, string password);

        void DeleteAuthenticationCredentials();
    }
}
