using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobbletBot
{
    public class CommandBase
    {
        public string commandName;
        public string commandId;
        public string commandDescription;
        public string commandSyntax;
        public CommandBase(string id, string description, string syntax, string name)
        {
            commandId = id;
            commandDescription = description;
            commandSyntax = syntax;
            commandName = name;
        }
    }
    public class Command : CommandBase
    {
        private Action command;
        public Command(string id, string description, string syntax, Action command) : base(id, description, syntax, id)
        {
            this.command = command;
        }
        public Command(string name, string id, string description, string syntax, Action command) : base(id, description, syntax, name)
        {
            this.command = command;
        }
        public void Invoke()
        {
            command.Invoke();
        }

    }
    public class Command<T1> : CommandBase
    {
        private Action<T1> command;
        public Command(string id, string description, string syntax, Action<T1> command) : base(id, description, syntax, id)
        {
            this.command = command;
        }
        public Command(string name, string id, string description, string syntax, Action<T1> command) : base(id, description, syntax, name)
        {
            this.command = command;
        }
        public void Invoke(T1 value1)
        {
            command.Invoke(value1);
        }
    }
    public class Command<T1, T2> : CommandBase
    {
        private Action<T1, T2> command;
        public Command(string id, string description, string syntax, Action<T1, T2> command) : base(id, description, syntax, id)
        {
            this.command = command;
        }
        public Command(string name, string id, string description, string syntax, Action<T1, T2> command) : base(id, description, syntax, name)
        {
            this.command = command;
        }
        public void Invoke(T1 value1, T2 value2)
        {
            command.Invoke(value1, value2);
        }
    }
    public class Command<T1, T2, T3> : CommandBase
    {
        private Action<T1, T2, T3> command;
        public Command(string id, string description, string syntax, Action<T1, T2, T3> command) : base(id, description, syntax, id)
        {
            this.command = command;
        }
        public Command(string name, string id, string description, string syntax, Action<T1, T2, T3> command) : base(id, description, syntax, name)
        {
            this.command = command;
        }
        public void Invoke(T1 value1, T2 value2, T3 value3)
        {
            command.Invoke(value1, value2, value3);
        }
    }
}
