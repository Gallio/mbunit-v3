using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Gallio.Icarus.Interfaces;

using MbUnit.Framework;

namespace Gallio.Icarus.Tests
{
    [TestFixture]
    public class AssemblyListTest : MockTest
    {
        private AssemblyList assemblyList;

        [SetUp]
        public void SetUp()
        {
            IProjectAdapterView projectAdapterView = mocks.CreateMock<IProjectAdapterView>();
            mocks.ReplayAll();
            assemblyList = new AssemblyList(projectAdapterView);
        }

        [Test]
        public void DataBind_Test()
        {
            ListViewItem[] listViewItems = new ListViewItem[0];
            assemblyList.DataBind(listViewItems);
        }
    }
}
