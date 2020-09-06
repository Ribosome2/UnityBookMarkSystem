using System;

namespace LinBookMark
{
    public abstract class CommandBase
    {
        protected Object[] targets;
        public virtual void Execute(params Object[] targets)
        {
            this.targets = targets;
        }
    }

}