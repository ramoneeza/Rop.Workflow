using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Rop.Wokflow.NextCases;

namespace Rop.Wokflow
{
    public record Step(string Name, MethodInfo StepAction,string? RunningDescription, bool HasParameter,bool HasCancellationToken)
    {
        public NextStatus Run(BaseWorkflow wf,CancellationToken ct, object? parameter=null)
        {
            if (RunningDescription is not null) wf.RunningDescription = RunningDescription;
            if (!HasParameter && !HasCancellationToken)
                return (StepAction.Invoke(wf,null) as NextStatus)??NextStatus.Error($"Step {Name} invalid return");
            if (!HasParameter && HasCancellationToken)
                return (StepAction.Invoke(wf,new object?[]{ct}) as NextStatus)??NextStatus.Error($"Step {Name} invalid return");
            if (HasParameter && !HasCancellationToken)
                return (StepAction.Invoke(wf,new object?[]{parameter}) as NextStatus)??NextStatus.Error($"Step {Name} invalid return");
            return (StepAction.Invoke(wf, new []{ct,parameter}) as NextStatus) ?? NextStatus.Error($"Step {Name} invalid return");
        }

        internal static Step Factory(string name,MethodInfo method)
        {
            if (!method.ReturnType.IsAssignableTo(typeof(NextStatus)))
                throw new Exception("Step must return a NextStatus");
            var att = method.GetCustomAttribute<RunningDescriptionAttribute>();
            var rd = att?.Description;
            
            var np = method.GetParameters();
            if (np.Length >2 )throw new Exception("Step too many parameters");
            if (np.Length == 0) return new Step(name,method,rd,false,false);
            if (np.Length == 1)
            {
                if (np[0].ParameterType == typeof(CancellationToken))
                    return new Step(name, method,rd, false, true);
                else
                    return new Step(name, method,rd, true, false);
            }
            return new Step(name, method,rd, true, true);
        }
    }
}
