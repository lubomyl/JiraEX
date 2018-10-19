using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ConfluenceEX.ViewModel.Navigation
{
    public class HistoryNavigator
    {
        private const int STACK_SIZE = 6;
        private const int STEP = 1;
        private const int STARTING_INDEX = -1;

        private List<UserControl> _viewStack;

        private int _index;

        public HistoryNavigator()
        {
            this._viewStack = new List<UserControl>();

            _index = STARTING_INDEX;
        }

        public bool CanGoBack()
        {
            if (this._index > 0 && this._viewStack.Count > 1) {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanGoForward()
        {
            if(this._index + 1 < this._viewStack.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public UserControl GetBackView()
        {
            if (CanGoBack())
            {
                UserControl ret = _viewStack[_index - STEP];
                this._index--;

                return ret;
            } else
            {
                //TODO throw exception
                return null;
            }
        }

        public UserControl GetForwardView()
        {
            if (CanGoForward())
            {
                UserControl ret = _viewStack[this._index + STEP];
                this._index++;

                return ret;
            } else
            {
                //TODO throw exception
                return null;
            }
        }

        public void AddView(UserControl view)
        {
            if (!IsSameAsPrevious(view))
            {
                if (this._index != this._viewStack.Count - 1)
                {
                    for (int i = this._viewStack.Count - 1; i > _index; i--)
                    {
                        this._viewStack.RemoveAt(i);
                    }
                }

                if (this._index == STACK_SIZE)
                {
                    ShiftStackLeft();
                }

                this._viewStack.Add(view);
                this._index++;
            }
        }

        private bool IsSameAsPrevious(UserControl view)
        {
            if(this._viewStack.Count > 0 && view == this._viewStack[_index])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ShiftStackLeft()
        {
            for (int i = 1; i < this._viewStack.Count; i++)
            {
                this._viewStack[i - 1] = this._viewStack[i];
            }

            for (int i = _viewStack.Count - 1; i < this._viewStack.Count; i++)
            {
                this._viewStack.RemoveAt(i);
            }

            this._index--;
        }

        public void ClearStack()
        {
            this._viewStack.Clear();
            this._index = STARTING_INDEX;
        }
    }
}
