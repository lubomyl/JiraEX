using AtlassianConnector.Model.Exceptions;
using JiraEX.Common;
using JiraEX.ViewModel.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JiraEX.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<MvvmMessageBoxEventArgs> MessageBoxRequest;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, args);
            }
        }

        protected void MessageBox_Show(Action<MessageBoxResult> resultAction, string messageBoxText, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.None, MessageBoxOptions options = MessageBoxOptions.None)
        {
            if (this.MessageBoxRequest != null)
            {
                this.MessageBoxRequest(this, new MvvmMessageBoxEventArgs(resultAction, messageBoxText, caption, button, icon, defaultResult, options));
            }
        }

        protected void ShowErrorMessages(JiraException ex, IJiraToolWindowNavigatorViewModel parent)
        {
            parent.StopLoading();

            string errorMessage = "";

            if (ex.ErrorResponse.ErrorMessages.Length != 0)
            {
                foreach (string errorMsg in ex.ErrorResponse.ErrorMessages)
                {
                    errorMessage += errorMsg;
                }
            }

            foreach (KeyValuePair<string, string> error in ex.ErrorResponse.Errors)
            {
                errorMessage += error.Value;
            }

            parent.SetErrorMessage(errorMessage);
        }
    }
}
