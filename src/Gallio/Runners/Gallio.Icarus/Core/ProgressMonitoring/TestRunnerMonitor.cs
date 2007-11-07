// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Monitors;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Core.ProgressMonitoring
{
    public class TestRunnerMonitor : BaseTestRunnerMonitor
    {
        private readonly ReportMonitor reportMonitor;
        private readonly IProjectPresenter presenter;
        private Dictionary<string, string> logStreams;

        public TestRunnerMonitor(IProjectPresenter presenter, ReportMonitor reportMonitor)
        {
            if (presenter == null)
                throw new ArgumentNullException(@"presenter");
            if (reportMonitor == null)
                throw new ArgumentNullException(@"reportMonitor");

            this.presenter = presenter;
            this.reportMonitor = reportMonitor;
            logStreams = new Dictionary<string, string>();
        }

        /// <inheritdoc />
        protected override void OnAttach()
        {
            base.OnAttach();
            reportMonitor.StepFinished += HandleStepFinished;
        }

        /// <inheritdoc />
        protected override void OnDetach()
        {
            base.OnDetach();
            reportMonitor.StepFinished -= HandleStepFinished;
        }

        private void HandleStepFinished(object sender, ReportStepEventArgs e)
        {
            // Ignore tests that aren't test cases & nested test steps.
            TestData testData = e.Report.TestModel.Tests[e.TestRun.TestId];
            if (!testData.IsTestCase || e.StepRun.Step.ParentId != null)
                return;

            switch (e.StepRun.Result.Outcome)
            {
                case TestOutcome.Passed:
                    presenter.Passed(e.TestRun.TestId);
                    break;

                case TestOutcome.Failed:
                    presenter.Failed(e.TestRun.TestId);
                    break;

                case TestOutcome.Inconclusive:
                    presenter.Ignored(e.TestRun.TestId);
                    break;
            }

            // store log streams
            foreach (ExecutionLogStream els in e.StepRun.ExecutionLog.Streams)
            {
                string key = els.Name + e.TestRun.TestId;
                if (logStreams.ContainsKey(key))
                {
                    logStreams[key] += els.ToString();
                }
                else
                {
                    logStreams.Add(key, els.ToString());
                }
            }
        }

        public string GetLogStream(string log)
        {
            if (logStreams.ContainsKey(log))
            {
                return logStreams[log];
            }
            else
            {
                return string.Empty;
            }
        }
    }
}