using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Enbridge.ArcObjects
{
    /// <summary>
    /// Summary description for EditSession
    /// </summary>
    public class EditSession : IDisposable
    {
        IWorkspaceEdit _workspaceEdit = null;

        public IWorkspaceEdit WorkspaceEdit
        {
            get { return _workspaceEdit; }
            set { _workspaceEdit = value; }
        }

        public EditSession(IWorkspace workspace, bool withUndoRedo)
        {
            if (workspace != null)
            {
                _workspaceEdit = (IWorkspaceEdit)workspace;
            }
        }

        public static EditSession Start(IWorkspace workspace, bool withUndoRedo)
        {
            EditSession editSession = new EditSession(workspace, withUndoRedo);
            editSession.Start(withUndoRedo);
            return editSession;
        }

        public void Start(bool withUndoRedo)
        {
            if (_workspaceEdit.IsBeingEdited() == false)
            {
                _workspaceEdit.StartEditing(withUndoRedo);
                _workspaceEdit.StartEditOperation();
            }
        }

        public void SaveAndStop()
        {
            Stop(true);
        }



        public void Stop(bool save)
        {
            if (_workspaceEdit.IsBeingEdited() == true)
            {
                _workspaceEdit.StopEditOperation();
                _workspaceEdit.StopEditing(save);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_workspaceEdit != null && _workspaceEdit.IsBeingEdited())
            {
                Stop(false);
            }
        }

        #endregion
    }

}