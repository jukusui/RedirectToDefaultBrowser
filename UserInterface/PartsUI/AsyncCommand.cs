using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UserInterface.PartsUI
{
    public class AsyncCommand : ICommand
    {

        public AsyncCommand(Func<object, Task> func, ExecutableKey key = null) : this(func, null, key) { }

        public AsyncCommand(Func<object, Task> command, Func<object, bool> checker, ExecutableKey key = null)
        {
            Command = command;
            Checker = checker;
            if (key == null)
                Key = new ExecutableKey();
            else
                Key = key;
            key.StateChanged += Key_StateChanged;
        }
        
        private void Key_StateChanged(ExecutableKey arg1, bool arg2) =>
            CanExecuteChanged(this, new EventArgs());

        public Func<object, Task> Command { get; }
        public Func<object, bool> Checker { get; }
        public ExecutableKey Key { get; }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) =>
            (Checker?.Invoke(parameter) ?? true) && Key.Executable;

        public async void Execute(object parameter)
        {
            if (Checker?.Invoke(parameter) == false)
                return;
            if (!Key.GetKey())
                return;
            try
            {
                await Command(parameter);
            }
            finally
            {
                Key.ReturnKey();
            }
        }
    }

    public class ExecutableKey
    {

        public event Action<ExecutableKey, bool> StateChanged;

        public bool Executable { get; private set; } = true;

        public bool GetKey()
        {
            if (!Executable)
                return false;
            Executable = false;
            StateChanged?.Invoke(this, false);
            return true;
        }

        public void ReturnKey()
        {
            Executable = true;
            StateChanged?.Invoke(this, true);
        }

    }
}
