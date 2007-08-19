using System;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Icarus.Core.Interfaces
{
    public interface IProjectAdapter
    {
        event EventHandler<EventArgs> GetTestTree; 
        TestModel TestCollection { set;}
        void DataBind();}
}