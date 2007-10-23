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
using MbUnit.Runner;
using MbUnit.Framework;
using MbUnit.Tasks.NAnt;
using MbUnit.TestResources.MbUnit2;
using NAnt.Core;
using NAnt.Core.Types;
using Rhino.Mocks;

namespace MbUnit.Tasks.NAnt.Tests
{
    /// <summary>
    /// A set of unit tests for the MbUnitNAntTask class (a custom MbUnit task for NAnt).
    /// </summary>
    /// <remarks>
    /// If you modify these tests please make sure to review the tests in the
    /// MbUnitTaskUnitTests fixture, since the both the tasks for NAnt and MSBuild
    /// should exhibit a similar behavior and features set.
    /// </remarks>
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(MbUnitTask))]
    [Category("UnitTests")]
    public class MbUnitTaskUnitTest
    {
        #region Private Members

        private INAntLogger stubbedNAntLogger;
        private FileSet[] assemblies;
        private readonly string resultProperty = "ExitCode";
        private readonly string resultPropertiesPrefix = "MbUnit.";

        #endregion

        #region SetUp and TearDown

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            stubbedNAntLogger = MockRepository.GenerateStub<INAntLogger>();
            FileSet fs = new FileSet();
            string testAssemblyPath = new Uri(typeof(SimpleTest).Assembly.CodeBase).LocalPath;
            fs.FileNames.Add(testAssemblyPath);
            assemblies = new FileSet[] { fs };
        }

        #endregion

        #region Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InstantiateWithNullArgument()
        {
            new MbUnitTask(null);
        }
        
        [Test]
        public void RunWithNoArguments()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.Execute();
            AssertResult(task, ResultCode.NoTests);
            // If nothing ran then all the statistics properties should be set to zero
            AssertResultProperty(task, "TestCount", 0);
            AssertResultProperty(task, "PassCount", 0);
            AssertResultProperty(task, "FailureCount", 0);
            AssertResultProperty(task, "IgnoreCount", 0);
            AssertResultProperty(task, "InconclusiveCount", 0);
            AssertResultProperty(task, "RunCount", 0);
            AssertResultProperty(task, "SkipCount", 0);
            AssertResultProperty(task, "AssertCount", 0);
            AssertResultProperty(task, "Duration", 0);
        }

        [Test]
        public void NullReportTypes()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.ReportTypes = null;
            // Just make sure it doesn't crash
            task.Execute();
        }

        [Test]
        public void EmptyReportTypes()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.ReportTypes = String.Empty;
            // Just make sure it doesn't crash
            task.Execute();
        }

        [Test]
        public void NullReportDirectory()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.ReportDirectory = null;
            // Just make sure it doesn't crash
            task.Execute();
        }

        [Test]
        public void NullReportNameFormat()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.ReportNameFormat = null;
            // Just make sure it doesn't crash
            task.Execute();
        }

        [Test]
        [ExpectedException(typeof(BuildException))]
        public void RunAssembly()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.Assemblies = assemblies;
            task.FailOnError = true;
            try
            {
                task.Execute();
            }
            catch (BuildException)
            {
                AssertResult(task, ResultCode.Failure);
                throw;
            }
        }

        [Test]
        public void RunAssemblyAndIgnoreFailures()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Execute();
            AssertResult(task, ResultCode.Failure);
        }

        [Test]
        public void RunType()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type=MbUnit.TestResources.MbUnit2.PassingTests";
            task.Execute();
            AssertResult(task, ResultCode.Success);
            AssertResultProperty(task, "TestCount", 4);
            AssertResultProperty(task, "PassCount", 4);
            AssertResultProperty(task, "FailureCount", 0);
            AssertDurationIsGreaterThanZero(task);
            // The assert count is not reliable but we should be fine with simple asserts
            AssertResultProperty(task, "AssertCount", 3);
        }

        [Test]
        public void RunFailingFixture()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type=MbUnit.TestResources.MbUnit2.FailingFixture";
            task.Execute();
            AssertResult(task, ResultCode.Failure);
            AssertResultProperty(task, "TestCount", 2);
            AssertResultProperty(task, "PassCount", 1);
            AssertResultProperty(task, "FailureCount", 1);
            AssertDurationIsGreaterThanZero(task);
            // The assert count is not reliable but we should be fine with simple asserts
            AssertResultProperty(task, "AssertCount", 0);
        }

        [Test]
        public void RunSingleTest()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type=MbUnit.TestResources.MbUnit2.PassingTests;Member=Pass";
            task.Execute();
            AssertResult(task, ResultCode.Success);
            AssertResultProperty(task, "TestCount", 1);
            AssertResultProperty(task, "PassCount", 1);
            AssertResultProperty(task, "FailureCount", 0);
            AssertDurationIsGreaterThanZero(task);
            // The assert count is not reliable but we should be fine with simple asserts
            AssertResultProperty(task, "AssertCount", 3);
        }

        [Test]
        public void RunSingleFailingTest()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type=MbUnit.TestResources.MbUnit2.FailingFixture;Member=Fail";
            task.Execute();
            AssertResult(task, ResultCode.Failure);
            AssertResultProperty(task, "TestCount", 1);
            AssertResultProperty(task, "PassCount", 0);
            AssertResultProperty(task, "FailureCount", 1);
            AssertDurationIsGreaterThanZero(task);
            // The assert count is not reliable but we should be fine with simple asserts
            AssertResultProperty(task, "AssertCount", 0);
        }

        [Test]
        public void RunIgnoredTests()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type=MbUnit.TestResources.MbUnit2.IgnoredTests";
            task.Execute();
            AssertResult(task, ResultCode.Success);
            AssertResultProperty(task, "TestCount", 2);
            AssertResultProperty(task, "IgnoreCount", 2);
            AssertDurationIsGreaterThanZero(task);
        }

        [Test]
        public void AddHintAndPluginDirectories()
        {
            InstrumentedMbUnitTask task = CreateTask();
            DirSet ds = new DirSet();
            ds.FileNames.Add(@"C:\Windows");
            task.HintDirectories = new DirSet[] { ds };
            task.PluginDirectories = new DirSet[] { ds };
            task.Execute();
        }

        #endregion

        #region Private Methods

        private InstrumentedMbUnitTask CreateTask()
        {
            InstrumentedMbUnitTask task = new InstrumentedMbUnitTask(stubbedNAntLogger);
            task.InitializeTaskConfiguration();
            task.FailOnError = false;
            task.ResultProperty = resultProperty;
            task.ResultPropertiesPrefix = resultPropertiesPrefix;

            return task;
        }

        private void AssertResult(Element task, int expectedValue)
        {
            Assert.AreEqual(task.Properties[resultProperty], expectedValue.ToString());
        }

        private void AssertResultProperty(Element task, string name, int expectedValue)
        {
            Assert.AreEqual(task.Properties[resultPropertiesPrefix + name], expectedValue.ToString());
        }

        private void AssertDurationIsGreaterThanZero(Element task)
        {
            Assert.GreaterThan(task.Properties[resultPropertiesPrefix + "Duration"], 0.ToString());
        }

        #endregion
    }
}
