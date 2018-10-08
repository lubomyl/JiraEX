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

        private IJiraToolWindowNavigatorViewModel _parent;
        private Project _project;
        private Issue _parentIssue;

        private IIssueService _issueService;

        private bool _isEditingType = false;
        private bool _isCreatingSubTask = false;

        private string _summary;
        private string _description;
        private IssueType _selectedType;

        private ObservableCollection<IssueType> _typesList;
        private IssueType _subTask; 

        public DelegateCommand CancelCreateIssueCommand { get; private set; }
        public DelegateCommand ConfirmCreateIssueCommand { get; private set; }

        public DelegateCommand EditTypeCommand { get; private set; }
        public DelegateCommand CancelEditTypeCommand { get; private set; }

        private CreateIssueViewModel(Project project, IIssueService issueService)
        {
            this._typesList = new ObservableCollection<IssueType>();

            foreach (IssueType it in project.CreatableIssueTypesList)
            {
                if (!it.Subtask)
                {
                    this.TypesList.Add(it);
                }
                else
                {
                    this._subTask = it;
                }
            }

            this._issueService = issueService;

            this.CancelCreateIssueCommand = new DelegateCommand(CancelCreateIssue);
            this.ConfirmCreateIssueCommand = new DelegateCommand(ConfirmCreateIssue);

            this.EditTypeCommand = new DelegateCommand(EnableEditType);
            this.CancelEditTypeCommand = new DelegateCommand(CancelEditType);
        }

        public CreateIssueViewModel(IJiraToolWindowNavigatorViewModel parent, Project project, IIssueService issueService) : this(project, issueService)
        {
            this._parent = parent;
            this._project = project;

            this.SelectedType = this._typesList[0];
        }

        public CreateIssueViewModel(IJiraToolWindowNavigatorViewModel parent, Issue parentIssue, Project project, IIssueService issueService) : this(project, issueService)
        {
            this._parent = parent;
            this._parentIssue = parentIssue;
            this._project = project;

            this.SelectedType = this._subTask;

            this.IsCreatingSubTask = true;
        }

        private async void ConfirmCreateIssue(object sender)
        {
            Issue fullyCreatedIssue = null;

            if (this.IsCreatingSubTask) {
                Issue createdIssue = await this._issueService.CreateSubTaskIssueAsync(this._project.Id, this.Summary, this.Description, this.SelectedType.Id, this._parentIssue.Key);

                fullyCreatedIssue = await this._issueService.GetIssueByIssueKeyAsync(createdIssue.Key);
            }
            else
            {
                Issue createdIssue = await this._issueService.CreateIssueAsync(this._project.Id, this.Summary, this.Description, this.SelectedType.Id);

                fullyCreatedIssue = await this._issueService.GetIssueByIssueKeyAsync(createdIssue.Key);
            }

            this._parent.ShowIssueDetail(fullyCreatedIssue, this._project);
        }

        private void CancelCreateIssue(object sender)
        {
            if (this._parentIssue != null)
            {
                this._parent.ShowIssueDetail(this._parentIssue, this._project);
            }
            else
            {
                this._parent.ShowIssuesOfProject(this._project);
            }
        }

        private void EnableEditType(object parameter)
        {
            if (!this.IsCreatingSubTask)
            {
                this.IsEditingType = true;
            }
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

        public IssueType SelectedType
        {
            get { return this._selectedType; }
            set
            {
                this._selectedType = value;
                OnPropertyChanged("SelectedType");
            }
        }

        public ObservableCollection<IssueType> TypesList
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

        public bool IsCreatingSubTask
        {
            get { return this._isCreatingSubTask; }
            set
            {
                this._isCreatingSubTask = value;
                OnPropertyChanged("IsCreatingSubTask");
            }
        }

        public Issue ParentIssue
        {
            get { return this._parentIssue; }
            set
            {
                this._parentIssue = value;
                OnPropertyChanged("ParentIssue");
            }
        }
    }
}
