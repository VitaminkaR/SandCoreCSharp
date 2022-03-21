using System;
using System.Threading;

namespace SandCoreCSharp.Utils
{
    public class SimpleTimer
    {
        public Timer @object { get; private set; }

        public delegate void Callback(object obj);
        private Callback callback;

        public SimpleTimer(int time, Callback _callback, object obj)
        {
            callback = _callback;
            @object = new Timer(MainCallback, obj, time, 0);
        }

        private void MainCallback(object state)
        {
            callback.Invoke(state);
            @object.Dispose();
        }

        public void Stop() => @object.Dispose();
    }
}
