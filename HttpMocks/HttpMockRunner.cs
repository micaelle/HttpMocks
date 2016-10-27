﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpMocks.Implementation;
using HttpMocks.Verifications;

namespace HttpMocks
{
    internal class HttpMockRunner : IHttpMockRunner
    {
        private readonly List<StartedHttpMock> startedHttpMocks;

        public HttpMockRunner()
        {
            startedHttpMocks = new List<StartedHttpMock>();
        }

        public void RunMock(HttpMock httpMock)
        {
            var startedHttpMock = new StartedHttpMock(httpMock);
            startedHttpMock.Start();

            startedHttpMocks.Add(startedHttpMock);
        }

        public void VerifyAll()
        {
            var stopMockTasks = startedHttpMocks
                .Select(startedHttpMock => startedHttpMock.StopAsync())
                .ToArray();

            var verificationMockResults = stopMockTasks.SelectMany(t => t.Result).ToArray();

            if (verificationMockResults.Length == 0)
            {
                return;
            }
            
            var resultsString = new StringBuilder();
            foreach (var verificationMockResult in verificationMockResults)
            {
                resultsString.AppendLine(verificationMockResult.Message);
            }
            throw new AssertHttpMockException(resultsString.ToString());
        }
    }
}