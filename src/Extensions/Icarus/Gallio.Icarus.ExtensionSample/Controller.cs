using System;
using Gallio.Icarus.Events;
using Gallio.UI.Events;

namespace Gallio.Icarus.ExtensionSample
{
    public class Controller : IController, Handles<TestStepFinished>
    {
        public event EventHandler<UpdateEventArgs> Update = (s, e) => { };
        
        public void Handle(TestStepFinished @event)
        {
            var text = string.Format("TestStepFinished: {0}", @event.TestStepRun.Result.Outcome);
            Update(this, new UpdateEventArgs(text));
        }
    }
}