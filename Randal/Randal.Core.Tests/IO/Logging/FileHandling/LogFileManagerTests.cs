using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.IO.Logging;
using Randal.Core.IO.Logging.FileHandling;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.IO.Logging.FileHandling
{
	[TestClass, DeploymentItem(Test.Paths.IoLoggingFolder, Test.Paths.IoLoggingFolder)]
	public sealed class LogFileManagerTests : BaseUnitTest<LogFileManagerThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();

			Given.Size = 1024;
			Given.NullSettings = false;
		}

		[TestCleanup]
		public void Teardown()
		{
			if (Then.Manager == null) 
				return;

			Then.Manager.Dispose();
			Then.Writer = null;
			Then.Manager = null;
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ShouldThrowExceptionWhenCreatingGivenNullSettings()
		{
			Given.NullSettings = true;

			When(Creating);
		}

		[TestMethod]
		public void ShouldReturnLogManagerWhenCreating()
		{
			When(Creating);

			Then.Manager.Should().NotBeNull().And.BeAssignableTo<ILogFileManager>();
			Then.Manager.LogFileName.Should().BeNull();
		}

		[TestMethod]
		public void ShouldReturnOpenedWriterWhenGettingStreamWriter()
		{
			When(Creating, GettingStreamWriter);

			Then.Writer.Should().NotBeNull();
			Then.Manager.LogFileName.Should().EndWith("_001.log");
		}

		[TestMethod]
		public void ShouldReturnAnotherInstanceOfWriterWhenGettingStreamGivenAnExhaustedLog()
		{
			Given.Size = 2;
			Given.TextToWrite = "Hi";

			When(Creating, GettingStreamWriter, WritingText, GettingStreamWriter);

			Then.Writer.Should().NotBeNull();
			Then.Writer.BaseStream.CanWrite.Should().BeTrue();
			Then.Manager.LogFileName.Should().EndWith("_002.log");
		}

		private void GettingStreamWriter()
		{
			Then.Writer = Then.Manager.GetStreamWriter();
		}

		private void Creating()
		{
			if (Then.Manager != null)
				return;

			IFileLoggerSettings settings = null;
			if(Given.NullSettings == false)
				settings = new FileLoggerSettings(Test.Paths.IoLoggingFolder, "LFMT", Given.Size, false);

			Then.Manager = new LogFileManager(settings);
		}

		private void WritingText()
		{
			Then.Writer.WriteLine(Given.TextToWrite);	
		}
	}

	public sealed class LogFileManagerThens
	{
		public LogFileManager Manager;
		public StreamWriter Writer;
	}
}