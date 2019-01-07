using JiraEX.Main;
using JiraEX.View;
using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel.Navigation
{

    public class RefreshableWorklogViewModel : ViewModelBase
    {

        private JiraWorklogToolWindow _parent;

        private WorklogView _selectedView;

        public RefreshableWorklogViewModel(JiraWorklogToolWindow parent)
        {
            this._parent = parent;
        }

        public void OpenCreateWorklogForIssue(Issue issue)
        {
            this.SelectedView = new WorklogView(issue);
        }

        public WorklogView SelectedView
        {
            get { return this._selectedView; }
            set
            {
                this._selectedView = value;
                OnPropertyChanged("SelectedView");
            }
        }

    }
}
