using System.Threading;

namespace Richter.Parameters
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;

    public static class OptionalAndNamedParameters
    {
        private static int _index = 0;

        public static void RunOptionalAndNamedParametersScenario()
        {
            // 1. Same as: M(9, "A", default(DateTime), new Guid());
            ShowParams();
            // 2. Same as: M(8, "X", default(DateTime), new Guid());
            ShowParams(8, "X");
            // 3. Same as: M(5, "A", DateTime.Now, Guid.NewGuid());
            ShowParams(5, guid: Guid.NewGuid(), dt: DateTime.Now);
            // 4. Same as: M(0, "1", default(DateTime), new Guid());
            ShowParams(++_index, (++_index).ToString());
            // 5. Same as: String t1 = "2"; Int32 t2 = 3;
            // M(t2, t1, default(DateTime), new Guid());
            ShowParams(s: (_index++).ToString(), x: _index++);
        }

        private static void ShowParams([Optional][DefaultParameterValue(9)]int x, string s = "A", DateTime dt = default(DateTime), Guid guid = new Guid(), params object[] objects)
        {
            Console.WriteLine("x={0}, s={1}, dt={2}, guid={3}", x, s, dt, guid);
        }
    }

   public sealed  class EventKey { }

   public sealed class EventSet
   {
       private readonly  Dictionary<EventKey, Delegate> _events = new Dictionary<EventKey, Delegate>();

       public void Add(EventKey eventKey, Delegate handler)
       {
           Monitor.Enter(_events);
           Delegate d;
           _events.TryGetValue(eventKey, out d);
           _events[eventKey] = Delegate.Combine(d, handler);
           Monitor.Exit(_events);
       }

       public void Remove(EventKey eventKey, Delegate handler)
       {
           Monitor.Enter(_events);
           Delegate d;
           if (!_events.TryGetValue(eventKey, out d))
               return;
           d = Delegate.Remove(d, handler);
           if (d == null) _events.Remove(eventKey);
           else _events[eventKey] = d;
           Monitor.Exit(_events);
       }

       public void Raise(EventKey eventKey, Object sender, EventArgs e)
       {
           Delegate d;
           Monitor.Enter(_events);
           _events.TryGetValue(eventKey, out d);
           Monitor.Exit(_events);

           if (d != null)
               d.DynamicInvoke(new object[] {sender, e});
       }
   }

   public class TypeWithLotsOfEvents
   {
       protected readonly EventSet EventSet = new EventSet();
       protected static readonly EventKey FooEventKey = new EventKey();

       public event EventHandler<object> Foo
       {
           add { EventSet.Add(FooEventKey, value);}
           remove { EventSet.Remove(FooEventKey, value);}
       } 
   }
}