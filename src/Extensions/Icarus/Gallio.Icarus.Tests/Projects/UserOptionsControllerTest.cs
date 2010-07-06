using System;
using System.Collections.Generic;
using System.Linq;
using Gallio.Common.IO;
using Gallio.Common.Xml;
using Gallio.Icarus.Events;
using Gallio.Icarus.Projects;
using Gallio.Icarus.Properties;
using Gallio.UI.Common.Policies;
using Gallio.UI.Events;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Projects
{
    public class UserOptionsControllerTest
    {
        private IUnhandledExceptionPolicy unhandledExceptionPolicy;
        private IEventAggregator eventAggregator;
        private IFileSystem fileSystem;
        private IXmlSerializer xmlSerializer;
        private UserOptionsController controller;

        [SetUp]
        public void SetUp()
        {
            eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            fileSystem = MockRepository.GenerateStub<IFileSystem>();
            xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            
            controller = new UserOptionsController(eventAggregator, fileSystem, 
                xmlSerializer, unhandledExceptionPolicy);
        }

        [Test]
        public void Tree_view_category_defaults_to_Namespace()
        {
            Assert.AreEqual("Namespace", controller.TreeViewCategory);
        }

        [Test]
        public void Collapsed_nodes_defaults_to_empty_list()
        {
            Assert.Count(0, controller.CollapsedNodes);
        }

        [Test]
        public void Update_tree_view_category()
        {
            const string treeViewCategory = "test";

            controller.Handle(new TreeViewCategoryChanged(treeViewCategory));

            Assert.AreEqual(treeViewCategory, controller.TreeViewCategory);
        }

        [Test]
        public void Set_collapsed_nodes()
        {
            var collapsedNodes = new string[0];

            controller.SetCollapsedNodes(collapsedNodes);

            Assert.AreEqual(collapsedNodes, controller.CollapsedNodes);
        }

        [Test]
        public void User_options_are_not_loaded_if_the_file_does_not_exist()
        {
            const string projectLocation = "test.gallio";
            fileSystem.Stub(fs => fs.FileExists(projectLocation + UserOptions.Extension))
                .Return(false);

            controller.Handle(new ProjectLoaded(projectLocation));

            xmlSerializer.AssertWasNotCalled(xs => xs.LoadFromXml<UserOptions>(Arg<string>.Is.Anything));
        }

        [Test]
        public void Default_treeview_category_is_used_if_the_file_does_not_exist()
        {
            fileSystem.Stub(fs => fs.FileExists(Arg<string>.Is.Anything))
                .Return(false);

            controller.Handle(new ProjectLoaded("test.gallio"));

            Assert.AreEqual(UserOptions.DefaultTreeViewCategory, controller.TreeViewCategory);
        }

        [Test]
        public void Empty_collapsed_node_list_is_provided_if_the_file_does_not_exist()
        {
            fileSystem.Stub(fs => fs.FileExists(Arg<string>.Is.Anything))
                .Return(false);

            controller.Handle(new ProjectLoaded("test.gallio"));

            Assert.Count(0, controller.CollapsedNodes);
        }

        [Test]
        public void User_options_are_deserialised_if_the_file_exists()
        {
            const string projectLocation = "test.gallio";
            var userOptionsLocation = projectLocation + UserOptions.Extension;
            fileSystem.Stub(fs => fs.FileExists(Arg<string>.Is.Anything))
                .Return(true);
            xmlSerializer.Stub(xs => xs.LoadFromXml<UserOptions>(Arg<string>.Is.Anything))
                .Return(new UserOptions());

            controller.Handle(new ProjectLoaded(projectLocation));

            xmlSerializer.AssertWasCalled(xs => xs.LoadFromXml<UserOptions>(userOptionsLocation));
        }

        [Test]
        public void Treeview_category_is_updated_if_user_options_are_loaded()
        {
            fileSystem.Stub(fs => fs.FileExists(Arg<string>.Is.Anything))
                .Return(true);
            const string treeViewCategory = "test";
            xmlSerializer.Stub(xs => xs.LoadFromXml<UserOptions>(Arg<string>.Is.Anything))
                .Return(new UserOptions { TreeViewCategory = treeViewCategory });

            controller.Handle(new ProjectLoaded("test.gallio"));

            Assert.AreEqual(treeViewCategory, controller.TreeViewCategory);
        }

        [Test]
        public void Treeview_category_change_is_advertised_if_user_options_are_loaded()
        {
            fileSystem.Stub(fs => fs.FileExists(Arg<string>.Is.Anything))
                .Return(true);
            const string treeViewCategory = "test";
            xmlSerializer.Stub(xs => xs.LoadFromXml<UserOptions>(Arg<string>.Is.Anything))
                .Return(new UserOptions { TreeViewCategory = treeViewCategory });

            controller.Handle(new ProjectLoaded("test.gallio"));

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<TreeViewCategoryChanged>.Matches(tvcc => 
                tvcc.TreeViewCategory == treeViewCategory)));
        }

        [Test]
        public void Treeview_category_is_propagated_if_user_options_are_loaded()
        {
            const string projectLocation = "test.gallio";
            var userOptionsLocation = projectLocation + UserOptions.Extension;
            fileSystem.Stub(fs => fs.FileExists(userOptionsLocation))
                .Return(true);
            const string treeViewCategory = "test";
            xmlSerializer.Stub(xs => xs.LoadFromXml<UserOptions>(Arg<string>.Is.Anything))
                .Return(new UserOptions { TreeViewCategory = treeViewCategory });

            controller.Handle(new ProjectLoaded(projectLocation));

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<TreeViewCategoryChanged>.Matches(tvcc => 
                tvcc.TreeViewCategory == treeViewCategory)));
        }

        [Test]
        public void Collapsed_nodes_are_updated_if_user_options_are_loaded()
        {
            fileSystem.Stub(fs => fs.FileExists(Arg<string>.Is.Anything))
                .Return(true);
            var collapsedNodes = new List<string>();
            xmlSerializer.Stub(xs => xs.LoadFromXml<UserOptions>(Arg<string>.Is.Anything))
                .Return(new UserOptions { CollapsedNodes = collapsedNodes });

            controller.Handle(new ProjectLoaded("test.gallio"));

            Assert.AreEqual(collapsedNodes, controller.CollapsedNodes);
        }

        [Test]
        public void Event_raised_if_user_options_are_loaded()
        {
            fileSystem.Stub(fs => fs.FileExists(Arg<string>.Is.Anything))
                .Return(true);
            xmlSerializer.Stub(xs => xs.LoadFromXml<UserOptions>(Arg<string>.Is.Anything))
                .Return(new UserOptions());

            controller.Handle(new ProjectLoaded("test.gallio"));

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<UserOptionsLoaded>.Is.Anything));
        }

        [Test]
        public void Errors_logged_when_loading_user_options()
        {
            fileSystem.Stub(fs => fs.FileExists(Arg<string>.Is.Anything))
                .Return(true);
            var exception = new Exception();
            xmlSerializer.Stub(xs => xs.LoadFromXml<UserOptions>(Arg<string>.Is.Anything))
                .Throw(exception);

            controller.Handle(new ProjectLoaded("test.gallio"));

            unhandledExceptionPolicy.AssertWasCalled(uep => 
                uep.Report(Resources.UserOptionsController_Failed_to_load_user_options_, exception));
        }

        [Test]
        public void User_options_are_not_saved_with_project_if_not_dirty()
        {
            controller.Handle(new ProjectSaved(""));

            eventAggregator.AssertWasNotCalled(ea => ea.Send(Arg<UserOptionsSaved>.Is.Anything));
        }

        [Test]
        public void User_options_are_saved_with_project_if_dirty()
        {
            controller.Handle(new TreeViewCategoryChanged("test"));

            controller.Handle(new ProjectSaved(""));

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<UserOptionsSaved>.Is.Anything));
        }

        [Test]
        public void When_user_options_are_saved_filename_matches_project()
        {
            controller.Handle(new TreeViewCategoryChanged("test"));
            const string projectLocation = "test.gallio";
            var userOptionsFileName = projectLocation + UserOptions.Extension;

            controller.Handle(new ProjectSaved(projectLocation));

            xmlSerializer.AssertWasCalled(xs => xs.SaveToXml(Arg<UserOptions>.Is.Anything, 
                Arg.Is(userOptionsFileName)));
        }

        [Test]
        public void When_user_options_are_saved_treeview_category_is_correct()
        {
            const string treeViewCategory = "test";
            controller.Handle(new TreeViewCategoryChanged(treeViewCategory));

            controller.Handle(new ProjectSaved(""));

            xmlSerializer.AssertWasCalled(xs => xs.SaveToXml(Arg<UserOptions>.Matches(uo => 
                uo.TreeViewCategory == treeViewCategory), Arg<string>.Is.Anything));
        }

        [Test]
        public void When_user_options_are_saved_collapsed_nodes_are_correct()
        {
            var collapsedNodes = new [] { "test" };
            controller.SetCollapsedNodes(collapsedNodes);

            controller.Handle(new ProjectSaved(""));

            xmlSerializer.AssertWasCalled(xs => xs.SaveToXml(Arg<UserOptions>.Matches(uo =>
                uo.CollapsedNodes.Contains(collapsedNodes[0])), Arg<string>.Is.Anything));
        }
    }
}
