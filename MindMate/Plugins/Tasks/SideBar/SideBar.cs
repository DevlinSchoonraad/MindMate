﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MindMate.Plugins.Tasks.SideBar
{
    public partial class SideBar : UserControl
    {

        public SideBar()
        {
            controlGroups = new ControlGroupCollection(this);

            MyInitializeComponent();
        }

        private ControlGroupCollection controlGroups; 
        public ControlGroupCollection ControlGroups 
        { 
            get
            {
                return controlGroups;
            }
        }

        private void AdjustMainPanelHeight()
        {
            Control lastTaskGroup = GetLastControl();
            if (lastTaskGroup != null)
                this.tablePanelMain.Height = lastTaskGroup.Location.Y + lastTaskGroup.Size.Height + lastTaskGroup.Margin.Bottom;
            else
                this.tablePanelMain.Height = 20;
        }

        internal void OnControlAdded(object sender, ControlEventArgs e)
        {
            AdjustMainPanelHeight();

            lblNoTasks.Visible = false;            
        }

        internal void OnControlRemoved(object sender, ControlEventArgs e)
        {
            AdjustMainPanelHeight();

            if (GetLastControl() == null) lblNoTasks.Visible = true;
        }

        //public Control GetControl(int index)
        //{
        //    int currentIndex = index;
        //    for(int i = 0; i < ControlGroups.Count; i++)
        //    {
        //        ControlGroup panel = ControlGroups[i];
        //        if (panel != null)
        //        {
        //            if (currentIndex < panel.Count)
        //            {
        //                Control ctrl = panel[currentIndex];
        //                return ctrl;                        
        //            }
        //            else
        //            {
        //                currentIndex -= panel.Count;
        //            }
        //        }
        //    }

        //    return null;
        //}

        public Control GetFirstControl()
        {
            foreach(var cg in ControlGroups)
            {
                if (cg.Count > 0)
                    return cg[0];
            }
            return null;
        }

        public new Control GetNextControl(Control control, bool withinSameGroup = false)
        {
            // return next control in group
            Control nextCtrl = GetNextControlInGroup(control);
            if (nextCtrl != null) 
                return nextCtrl;
            
            // return first control from next visible group
            if (!withinSameGroup)
            {
                ControlGroup cg = GetControlGroup(control);
                if (cg != null)
                {
                    while ((cg = GetNextControlGroup(cg)) != null)
                    {
                        if (cg.Count > 0)
                            return cg[0];
                    }
                }
            }
            
            // if nothing found
            return null;
        }

        public new Control GetPreviousControl(Control control, bool withinSameGroup = false)
        {
            // return next control in group
            Control previousCtrl = GetPreviousControlInGroup(control);
            if (previousCtrl != null)
                return previousCtrl;

            // return first control from next visible group
            if (!withinSameGroup)
            {
                ControlGroup cg = GetControlGroup(control);
                if (cg != null)
                {
                    while ((cg = GetPreviousControlGroup(cg)) != null)
                    {
                        if (cg.Count > 0)
                            return cg[cg.Count - 1];
                    }
                }
            }

            // if nothing found
            return null;
        }

        public int GetControlCount()
        {
            int count = 0;
            foreach(var c in ControlGroups)
            {
                count += c.Count;
            }
            return count;
        }

        public ControlGroup GetControlGroup(Control control)
        {
            return (ControlGroup)control.Parent.Parent;
        }

        private Control GetNextControlInGroup(Control tv)
        {
            TableLayoutPanel table = (TableLayoutPanel)tv.Parent;
            var cell = table.GetPositionFromControl(tv);
            return table.GetControlFromPosition(cell.Column, cell.Row + 1);
        }

        private Control GetPreviousControlInGroup(Control tv)
        {
            TableLayoutPanel table = (TableLayoutPanel)tv.Parent;
            var cell = table.GetPositionFromControl(tv);
            if (cell.Row > 0)
                return table.GetControlFromPosition(cell.Column, cell.Row - 1);
            else
                return null;
        }

        public ControlGroup GetNextControlGroup(ControlGroup taskGroup)
        {
            int row = tablePanelMain.GetRow(taskGroup) + 1;

            if (row < tablePanelMain.RowCount)
                return ControlGroups[row];
            else
                return null;
        }

        public ControlGroup GetPreviousControlGroup(ControlGroup taskGroup)
        {
            int row = tablePanelMain.GetRow(taskGroup) - 1;

            if (row > -1)
                return ControlGroups[row];
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>return null if no Task Group is visible</returns>
        private Control GetLastControl()
        {
            for(int i = this.tablePanelMain.RowCount - 1; i >= 0; i--)
            {
                Control ctrl = this.tablePanelMain.GetControlFromPosition(0, i);

                // check for ctrl is the last visible Task Group
                if (ctrl != null // control is null in case when it is never made visible  
                    &&
                    (ctrl.Visible || // visible is false when it is the first control ever made visible in TaskList
                    ((TableLayoutPanel)ctrl.Controls[1]).RowCount > 0) // finds if there is any rows inside
                    )
                {
                    return ctrl;
                }
            }

            return null;
        }
        
    }
}