using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rop.Wokflow
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RunningDescriptionAttribute:Attribute
    {
        public string Description { get; set; }

        public RunningDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
