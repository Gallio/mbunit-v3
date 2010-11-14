using System;
using System.Threading;
using Gallio.Icarus.Events;
using Gallio.UI.Events;

namespace Gallio.Icarus.ExtensionSample
{
    public class Controller : IController, Handles<TestStepFinished>
    {
        public event EventHandler<UpdateEventArgs> Update = (s, e) => { };
        
        public void DoSomeWork()
        {
            SendUpdate("Started work");
            Thread.Sleep(2000);
            SendUpdate("Completed work");
        }

        public void Handle(TestStepFinished @event)
        {
            var text = string.Format("TestStepFinished: {0}", @event.TestStepRun.Result.Outcome);
            SendUpdate(text);
        }

        private void SendUpdate(string text)
        {
            Update(this, new UpdateEventArgs(text));
        }
    }
}