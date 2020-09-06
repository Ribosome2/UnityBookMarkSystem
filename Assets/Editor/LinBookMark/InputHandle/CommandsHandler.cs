using System;

namespace LinBookMark
{
    public static class CommandsHandler
    {
        public static void ExecuteCommand<T>(params Object[] targets) where T:CommandBase
        {
            var command = Activator.CreateInstance<T>();
            command.Execute(targets);
        }
    }
}