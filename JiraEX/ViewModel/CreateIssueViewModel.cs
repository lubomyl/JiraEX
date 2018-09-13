using ConfluenceEX.Command;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using JiraRESTClient.Service.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel
{

    public class CreateIssueViewModel : ViewModelBase, ITitleable
    {

        private JiraToolWindowNavigatorViewModel _parent;
        private BoardProject _project;

        private IIssueService _issueService;

        private bool _isEditingType = false;

        private string _summary;
        private string _description;
        private string _selectedType;

        private ObservableCollection<Type> _typesList;

        public DelegateCommand CancelCreateIssueCommand { get; private set; }
        public DelegateCommand ConfirmCreateIssueCommand { get; private set; }

        public DelegateCommand EditTypeCommand { get; private set; }
        public DelegateCommand CancelEditTypeCommand { get; private set; }

        public CreateIssueViewModel(JiraToolWindowNavigatorViewModel parent, BoardProject project)
        {
            this._parent = parent;
            this._project = project;

            this._issueService = new IssueService();

            this._typesList = new ObservableCollection<Type>();

            this.CancelCreateIssueCommand = new DelegateCommand(CancelCreateIssue);
            this.ConfirmCreateIssueCommand = new DelegateCommand(ConfirmCreateIssue);

            this.EditTypeCommand = new DelegateCommand(EnableEditType);
            this.CancelEditTypeCommand = new DelegateCommand(CancelEditType);
        }

        private void ConfirmCreateIssue(object sender)
        {
            this._issueService.CreateIssueAsync(this._project.Location.ProjectId, this.Summary, this.Description);
        }

        private void CancelCreateIssue(object sender)
        {
            throw new NotImplementedException();
        }

        private void EnableEditType(object parameter)
        {
            this.IsEditingType = true;
        }

        private void CancelEditType(object parameter)
        {
            this.IsEditingType= false;
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", this._project.Name);
        }

        public string Summary
        {
            get { return this._summary; }
            set
            {
                this._summary = value;
                OnPropertyChanged("Summary");
            }
        }

        public string Description
        {
            get { return this._description; }
            set
            {
                this._description = value;
                OnPropertyChanged("Description");
            }
        }

        public string SelectedType
        {
            get { return this._selectedType; }
            set
            {
                this._selectedType = value;
                OnPropertyChanged("SelectedType");
            }
        }

        public ObservableCollection<Type> TypesList
        {
            get { return this._typesList; }
            set
            {
                this._typesList = value;
                OnPropertyChanged("TypesList");
            }
        }

        public bool IsEditingType
        {
            get { return this._isEditingType; }
            set
            {
                this._isEditingType = value;
                OnPropertyChanged("IsEditingType");
            }
        }
    }
}
