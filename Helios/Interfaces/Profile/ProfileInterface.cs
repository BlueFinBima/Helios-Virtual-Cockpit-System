//  Copyright 2014 Craig Courtney
//    
//  Helios is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Helios is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace GadrocsWorkshop.Helios.Interfaces.Profile
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using GadrocsWorkshop.Helios.ComponentModel;

    [HeliosInterface("Helios.Base.ProfileInterface", "Profile", null, typeof(UniqueHeliosInterfaceFactory), AutoAdd = true)]
    public class ProfileInterface : HeliosInterface, IStatusReportNotify, IResetMonitorsObserver
    {
        // observers of our status report that need to be told when we change status
        private readonly HashSet<IStatusReportObserver> _observers = new HashSet<IStatusReportObserver>();

        public ProfileInterface()
            : base("Profile")
        {
            HeliosAction resetAction = new HeliosAction(this, "", "", "reset", "Resets the profile to default state.");
            resetAction.Execute += new HeliosActionHandler(ResetAction_Execute);
            Actions.Add(resetAction);

            HeliosAction stopAction = new HeliosAction(this, "", "", "stop", "Stops the profile from running.");
            stopAction.Execute += new HeliosActionHandler(StopAction_Execute);
            Actions.Add(stopAction);

            HeliosAction showControlCenter = new HeliosAction(this, "", "", "show control center", "Shows the control center.");
            showControlCenter.Execute += new HeliosActionHandler(ShowAction_Execute);
            Actions.Add(showControlCenter);

            HeliosAction hideControlCenter = new HeliosAction(this, "", "", "hide control center", "Shows the control center.");
            hideControlCenter.Execute += new HeliosActionHandler(HideAction_Execute);
            Actions.Add(hideControlCenter);

            HeliosAction launchApplication = new HeliosAction(this, "", "", "launch application", "Launches an external application", "Full path to appliation or document you want to launch or URL to a web page.", BindingValueUnits.Text);
            launchApplication.Execute += LaunchApplication_Execute;
            Actions.Add(launchApplication);
        }

        void LaunchApplication_Execute(object action, HeliosActionEventArgs e)
        {
            try
            {
                Process.Start(e.Value.StringValue);
            }
            catch (Exception ex)
            {
                ConfigManager.LogManager.LogError("Error caught launching external application (path=\"" + e.Value.StringValue + "\")", ex);
            }
        }

        void HideAction_Execute(object action, HeliosActionEventArgs e)
        {
            if (Profile != null)
            {
                Profile.HideControlCenter();
            }
        }

        void ShowAction_Execute(object action, HeliosActionEventArgs e)
        {
            if (Profile != null)
            {
                Profile.ShowControlCenter();
            }
        }

        void StopAction_Execute(object action, HeliosActionEventArgs e)
        {
            if (Profile != null)
            {
                Profile.Stop();
            }
        }

        void ResetAction_Execute(object action, HeliosActionEventArgs e)
        {
            if (Profile != null)
            {
                Profile.Reset();
            }
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            // No-Op
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            // No-Op
        }

        public void Subscribe(IStatusReportObserver observer)
        {
            _observers.Add(observer);
        }

        public void Unsubscribe(IStatusReportObserver observer)
        {
            _observers.Remove(observer);
        }

        public void InvalidateStatusReport()
        {
            if (_observers.Count < 1)
            {
                return;
            }
            IList<StatusReportItem> newReport = new List<StatusReportItem>();
            if (Profile != null)
            {
                if (Profile.IsValidMonitorLayout)
                {
                    newReport.Add(new StatusReportItem
                    {
                        Status = "The monitor arrangement saved in this profile matches this computer",
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate | StatusReportItem.StatusFlags.Verbose
                    });
                }
                else
                {
                    newReport.Add(new StatusReportItem
                    {
                        Status = "The monitor arrangement saved in this profile does not match this computer",
                        Recommendation = "Perform Reset Monitors from the Profile menu",
                        Severity = StatusReportItem.SeverityCode.Error,
                        Link = StatusReportItem.ProfileEditor,
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate | StatusReportItem.StatusFlags.Verbose
                    });

                }
            }
            PublishStatusReport(newReport);
        }

        public void PublishStatusReport(IEnumerable<StatusReportItem> statusReport)
        {
            IList<StatusReportItem> statusReportItems = statusReport.ToList();
            foreach (IStatusReportObserver observer in _observers)
            {
                observer.ReceiveStatusReport("Profile Interface", "Interface to Helios Control Center itself.", statusReportItems);
            }
        }

        public void NotifyResetMonitorsComplete()
        {
            InvalidateStatusReport();
        }
    }
}
