using System;
using System.Collections.Generic;
using System.IO;
using Gallio.Common.IO;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Properties;
using Gallio.UI.Events;
using ITestRunnerExtensionFactory = Gallio.Icarus.Utilities.ITestRunnerExtensionFactory;

namespace Gallio.Icarus.ProjectProperties
{
    public class Controller : IController, Handles<ProjectLoaded>
    {
        private readonly IFileSystem fileSystem;
        private readonly IModel model;
        private readonly IProjectController projectController;
        private readonly ITestRunnerExtensionFactory testRunnerExtensionFactory;

        public Controller(IModel model, IFileSystem fileSystem, IProjectController projectController,
            ITestRunnerExtensionFactory testRunnerExtensionFactory)
        {
            this.model = model;
            this.fileSystem = fileSystem;
            this.projectController = projectController;
            this.testRunnerExtensionFactory = testRunnerExtensionFactory;
        }

        public void Handle(ProjectLoaded @event)
        {
            BindModel();
        }

        private void BindModel()
        {
            model.ApplicationBaseDirectory.Value = projectController.ApplicationBaseDirectory;

            UpdateHintDirectories(projectController.HintDirectories);

            model.ReportDirectory.Value = projectController.ReportDirectory;
            model.ReportNameFormat.Value = projectController.ReportNameFormat;
            model.ShadowCopy.Value = projectController.ShadowCopy;

            UpdateTestRunnerExtensionSpecifications(projectController.TestRunnerExtensionSpecifications);

            model.WorkingDirectory.Value = projectController.WorkingDirectory;
        }

        private void UpdateTestRunnerExtensionSpecifications(IEnumerable<string> testRunnerExtensionSpecifications)
        {
            model.TestRunnerExtensionSpecifications.Clear();
            foreach (var testRunnerExtension in testRunnerExtensionSpecifications)
            {
                model.TestRunnerExtensionSpecifications.Add(testRunnerExtension);
            }
        }

        private void UpdateHintDirectories(IEnumerable<DirectoryInfo> hintDirectories)
        {
            model.HintDirectories.Clear();
            foreach (var hintDirectory in hintDirectories)
            {
                model.HintDirectories.Add(hintDirectory.ToString());
            }
        }

        public void AddHintDirectory(string hintDirectory)
        {
            if (DirectoryExists(hintDirectory) == false)
                throw new Exception(Resources.Controller_Invalid_hint_directory_path_);

            projectController.AddHintDirectory(hintDirectory);
            model.HintDirectories.Add(hintDirectory);
        }

        public void AddTestRunnerExtensionSpecification(string testRunnerExtensionSpecification)
        {
            testRunnerExtensionFactory.CreateFromSpecification(testRunnerExtensionSpecification);

            projectController.AddTestRunnerExtensionSpecification(testRunnerExtensionSpecification);
            model.TestRunnerExtensionSpecifications.Add(testRunnerExtensionSpecification);
        }

        public void Load()
        {
            BindModel();
        }

        public void RemoveHintDirectory(string hintDirectory)
        {
            projectController.RemoveHintDirectory(hintDirectory);
            model.HintDirectories.Remove(hintDirectory);
        }

        public void RemoveTestRunnerExtensionSpecification(string testRunnerExtensionSpecification)
        {
            projectController.RemoveTestRunnerExtensionSpecification(testRunnerExtensionSpecification);
            model.TestRunnerExtensionSpecifications.Remove(testRunnerExtensionSpecification);
        }

        public void SetApplicationBaseDirectory(string applicationBaseDirectory)
        {
            if (DirectoryExists(applicationBaseDirectory) == false)
                throw new Exception(Resources.Controller_Invalid_application_base_directory);

            projectController.SetApplicationBaseDirectory(applicationBaseDirectory);
        }

        private bool DirectoryExists(string directory)
        {
            return string.IsNullOrEmpty(directory) || fileSystem.DirectoryExists(directory);
        }

        public void SetReportNameFormat(string reportNameFormat)
        {
            projectController.SetReportNameFormat(reportNameFormat);
        }

        public void SetShadowCopy(bool shadowCopy)
        {
            projectController.SetShadowCopy(shadowCopy);
        }

        public void SetWorkingDirectory(string workingDirectory)
        {
            if (DirectoryExists(workingDirectory) == false)
                throw new Exception(Resources.Controller_Invalid_working_directory);

            projectController.SetWorkingDirectory(workingDirectory);
        }
    }
}
