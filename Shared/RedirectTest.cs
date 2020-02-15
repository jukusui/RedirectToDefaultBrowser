using System;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Shared
{
    public class RedirectTest : Jukusui.Notify.NotifyBase, INotifyDataErrorInfo
    {

        public RedirectTest()
        {
            Input = Output = Test = "";
        }

        private string input;
        public string Input
        {
            get => input;
            set
            {
                if (OnPropertyChanged(ref input, value))
                {
                    if (string.IsNullOrEmpty(input))
                    {
                        inputRegex = null;
                        InputErrorText = Properties.ExResources.RExInputEmpty;
                    }
                    else
                    {
                        try
                        {
                            inputRegex = new Regex(value);
                            InputErrorText = "";
                        }
                        catch (Exception ex)
                        {
                            inputRegex = null;
                            InputErrorText = ex.Message;
                        }
                    }
                    RunTest();
                }
            }
        }
        private Regex inputRegex = null;

        private string output;
        public string Output
        {
            get => output;
            set
            {
                if (OnPropertyChanged(ref output, value) && !HasErrors)
                    RunTest();
            }
        }
        private string test;
        public string Test
        {
            get => test;
            set
            {
                if (OnPropertyChanged(ref test, value) && !HasErrors)
                    RunTest();
            }
        }

        private string testRes;
        public string TestRes { get => testRes; private set => OnPropertyChanged(ref testRes, value); }


        private void RunTest()
        {
            if (HasErrors || inputRegex == null)
                TestRes = "Error";
            else
            {
                TestRes = inputRegex.Replace(Test, Output);
            }
        }


        private bool hasErrors;

        public bool HasErrors
        {
            get => hasErrors;
        }

        private void UpdateErrorState()
        {
            var newState = inputErrorText != "";
            if (newState != hasErrors)
            {
                hasErrors = newState;
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
            }
        }

        private string inputErrorText = "";
        string InputErrorText
        {
            get => inputErrorText;
            set
            {
                if (OnPropertyChanged(ref inputErrorText, value))
                {
                    UpdateErrorState();
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Input)));
                }
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if ((propertyName == null || propertyName == nameof(Input)) &&
                HasErrors)
            {
                yield return InputErrorText;
            }
        }
    }
}
