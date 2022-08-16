using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Rop.Wokflow
{
    public class EventVar
    {
        public bool? Checked { get; private set; }
        public string Value { get; private set; }
        [JsonIgnore]
        private readonly ManualResetEvent _mevent;
        protected EventVar(string initvalue, bool? initstate)
        {
            Value = initvalue;
            Checked = initstate;
            _mevent = new ManualResetEvent(Checked is not null);
        }

        public EventVar() : this("", null){}
        public void Set(string value)
        {
            Checked = true;
            Value = value;
            _mevent.Set();
        }
        public virtual void SetCancel()
        {
            Checked = false;
            Value = "";
            _mevent.Set();
        }
        public bool? WaitOne(CancellationToken ct)
        {
            if (Checked is not null) return Checked;
            _mevent.WaitOne(ct);
            return Checked;
        }

        public void Reset(EventVar other)
        {
            Value = other.Value;
            Checked = other.Checked;
            if (Checked is null)
                _mevent.Reset();
            else
                _mevent.Set();
        }
    }

}
