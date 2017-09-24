﻿// (c) Copyright 2016 Hewlett Packard Enterprise Development LP
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Connector;
using Hpe.Nga.Api.Core.Connector.Exceptions;
using MicroFocus.Ci.Tfs.Octane.Configuration;
using MicroFocus.Ci.Tfs.Octane.Tools;
using Newtonsoft.Json.Serialization;

namespace MicroFocus.Ci.Tfs.Octane
{
    public class OctaneManager
    {
        #region Const Defenitions

        private const int DEFAULT_POLLING_GET_TIMEOUT = 20 * 1000; //20 seconds
        private const int DEFAULT_POLLING_INTERVAL = 5;

        #endregion

        private static readonly RestConnector _restConnector = new RestConnector();
        private static ConnectionDetails _connectionConf;
        
        private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(DEFAULT_POLLING_INTERVAL);
        private readonly int _pollingGetTimeout;
        private readonly UriResolver _uriResolver;
        private Task _taskPollingThread;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken _polingCancelationToken;


        public OctaneManager(int servicePort,int pollingTimeout= DEFAULT_POLLING_GET_TIMEOUT)
        {
            _pollingGetTimeout = pollingTimeout;
            var hostName = Dns.GetHostName();
            var domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            _connectionConf = ConfigurationManager.Read();
            var instanceDetails = new InstanceDetails(_connectionConf.InstanceId,$"http://{hostName}:{servicePort}");

            _uriResolver = new UriResolver(_connectionConf.SharedSpace,instanceDetails);

        }

        public void Init()
        {   
            
            var connected = _restConnector.Connect(_connectionConf.Host,
                new APIKeyConnectionInfo(_connectionConf.ClientId, _connectionConf.ClientSecret));
            if (!connected)
            {
               throw new Exception("Could not connect to octane webapp"); 
            }            

            InitTaskPolling();
        }

        public void ShutDown()
        {            
            _cancellationTokenSource.Cancel();
            _restConnector.Disconnect();
        }

        public void WaitShutdown()
        {
            _taskPollingThread.Wait();
        }

        private void InitTaskPolling()
        {
            _taskPollingThread = Task.Factory.StartNew(()=>PollOctaneTasks(_cancellationTokenSource.Token), TaskCreationOptions.LongRunning);            
        }

        private void PollOctaneTasks(CancellationToken token)
        {
            do
            {
                ResponseWrapper res = null;
                try
                {
                    res =
                        _restConnector.ExecuteGet(_uriResolver.GetTasksUri(), _uriResolver.GetTaskQueryParams(),
                            _pollingGetTimeout);                    
                }
                catch (Exception ex)
                {
                    var innerEx = (WebException) ex.InnerException;
                    var mqmEx = ex as MqmRestException;
                    if (innerEx?.Status == WebExceptionStatus.Timeout)
                    {
                        Trace.WriteLine("Timeout");
                        Trace.WriteLine($"Exception Info {mqmEx?.Description}");
                    }
                    else
                    {
                        Trace.WriteLine("Error getting status from octane : " + ex.Message);
                    }
                }
                finally
                {
                    Trace.WriteLine($"Time: {DateTime.Now.ToLongTimeString()}");
                    if (res == null)
                    {
                        Trace.WriteLine("No tasks");
                    }
                    else
                    {
                        Trace.WriteLine("Recieved Task:");
                        Trace.WriteLine($"Get status : {res?.StatusCode}");
                        Trace.WriteLine($"Get data : {res?.Data}");

                    }
                    
                    
                }
                Thread.Sleep(_pollingInterval);
            } while (!token.IsCancellationRequested);            
        }

        private bool SendTaskResultToOctane(string resultId,string resultObj)
        {
            var res = _restConnector.ExecutePost(_uriResolver.PostTaskResultUri(resultId), null, resultObj);

            if (res.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            Trace.WriteLine($"Error sending result {resultId} with object {resultObj} to server");
            Trace.WriteLine($"Error desc: {res?.StatusCode}, {res?.Data}");

            return false;
        }
    }
}
