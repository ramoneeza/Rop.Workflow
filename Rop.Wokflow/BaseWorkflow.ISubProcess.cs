using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rop.Wokflow.NextCases;

namespace Rop.Wokflow
{
    /// <summary>
    /// Workflow as a ISubProcess
    /// a ISubprocess has a Cancelation Token. A Result. Te workflow and a Parent.
    /// </summary>
    public partial class BaseWorkflow:ISubProcess
    {
        public bool Finished => Status == WorkflowStatus.Terminated;
        BaseWorkflow ISubProcess.Workflow => this;
        ISubProcess ISubProcess.Parent => this;
        NextStatus? ISubProcess.NextResult => Result;
        private readonly WfLinkedTokens _linkedTokens;
        ILinkedTokens ISubProcess.LinkedTokens => _linkedTokens;
        public CancellationToken CancellationToken => _linkedTokens.InternalToken;
    }
}
