﻿using System;
using System.Collections.Generic;

namespace GadrocsWorkshop.Helios.Patching.DCS
{
    public class ShadowMonitorEventArgs: EventArgs
    {
        public ShadowMonitor Data;

        public ShadowMonitorEventArgs(ShadowMonitor shadow)
        {
            Data = shadow;
        }
    }

    public class RawMonitorEventArgs : EventArgs
    {
        public Monitor Raw;

        public RawMonitorEventArgs(Monitor monitor)
        {
            Raw = monitor;
        }
    }

    /// <summary>
    /// This class represents a Helios shadow being observed.
    /// It is the model class.
    /// </summary>
    public class ShadowMonitor : ShadowVisual
    {
        public event EventHandler<KeyChangeEventArgs> KeyChanged;

        // the position and size of a shadow is all we care about
        public static string CreateKey(Monitor display) =>
            $"{display.Left}_{display.Top}_{display.Width}_{display.Height}";

        internal static IEnumerable<string> GetAllKeys(Monitor monitor)
        {
            string baseKey = CreateKey(monitor);
            return MonitorViewModel.GetAllKeys(baseKey);
        }

        public class KeyChangeEventArgs : EventArgs
        {
            public KeyChangeEventArgs(string oldKey, string newKey)
            {
                OldKey = oldKey;
                NewKey = newKey;
            }

            public string OldKey { get; }
            public string NewKey { get; }
        }

        /// <summary>
        /// backing field for property Included, contains
        /// true if this shadow is included in the extent used to calculate the DCS resolution
        /// </summary>
        private bool _included;

        /// <summary>
        /// backing field for property Main, contains
        /// true if this shadow is included in Main 3D view
        /// </summary>
        private bool _main;

        /// <summary>
        /// backing field for property UserInterface, contains
        /// true if this shadow is included in UI view showing DCS dialogs and loading screen
        /// </summary>
        private bool _userInterface;

        private int _viewportCount;

        internal ShadowMonitor(IShadowVisualParent parent, Monitor monitor)
            : base(parent, monitor, monitor, false)
        {
            Key = CreateKey(monitor);

            // read settings for monitors matching this geometry
            LoadSettings();
        }

        public string Key { get; private set; }


        private void LoadSettings()
        {
            Included = ConfigManager.SettingsManager.LoadSetting(MonitorSetup.SETTINGS_GROUP, Key, true);
            Main = ConfigManager.SettingsManager.LoadSetting(MonitorSetup.SETTINGS_GROUP, $"{Key}_Main", false);
            Main = ConfigManager.SettingsManager.LoadSetting(MonitorSetup.SETTINGS_GROUP, $"{Key}_UserInterface",
                false);
        }

        private void DeleteSettings()
        {
            ISettingsManager2 settings2 = (ISettingsManager2) ConfigManager.SettingsManager;
            settings2.DeleteSetting(MonitorSetup.SETTINGS_GROUP, Key);
            settings2.DeleteSetting(MonitorSetup.SETTINGS_GROUP, $"{Key}_Main");
            settings2.DeleteSetting(MonitorSetup.SETTINGS_GROUP, $"{Key}_UserInterface");
        }

        /// <summary>
        /// deferred initialization so our factory can index this before we add children to it
        /// </summary>
        internal void Instrument()
        {
            Instrument(Monitor, Monitor);
        }

        protected override void OnVisualModified()
        {
            base.OnVisualModified();

            // the monitor we shadow may have changed enough to where our key
            // doesn't match (during reset monitors)
            // and so we need to reindex in that case and load settings again
            string newKey = CreateKey(_monitor);
            if (newKey != Key)
            {
                string oldKey = Key;
                Key = newKey;
                LoadSettings();
                KeyChanged?.Invoke(this, new KeyChangeEventArgs(oldKey, newKey));
            }

            // need to update all viewports on this shadow
            foreach (ShadowVisual child in _children.Values)
            {
                UpdateChild(child);
            }
        }

        private void UpdateChild(ShadowVisual node)
        {
            if (node.IsViewport)
            {
                // the viewport may have changed as a result
            }

            foreach (ShadowVisual child in node.Children.Values)
            {
                UpdateChild(child);
            }
        }

        internal bool AddViewport()
        {
            _viewportCount++;
            if (_viewportCount == 1)
            {
                return true;
            }

            return false;
        }

        internal bool RemoveViewport()
        {
            _viewportCount--;
            if (_viewportCount == 0)
            {
                return true;
            }

            return false;
        }

        internal int ViewportCount => _viewportCount;

        /// <summary>
        /// true if this shadow is included in Main 3D view
        /// </summary>
        public bool Main
        {
            get => _main;
            set
            {
                if (_main == value)
                {
                    return;
                }

                bool oldValue = _main;
                _main = value;
                ConfigManager.SettingsManager.SaveSetting(MonitorSetup.SETTINGS_GROUP, $"{Key}_Main", value);
                OnPropertyChanged("Main", oldValue, value, true);
            }
        }

        /// <summary>
        /// true if this shadow is included in UI view showing DCS dialogs and loading screen
        /// </summary>
        public bool UserInterface
        {
            get => _userInterface;
            set
            {
                if (_userInterface == value)
                {
                    return;
                }

                bool oldValue = _userInterface;
                _userInterface = value;
                ConfigManager.SettingsManager.SaveSetting(MonitorSetup.SETTINGS_GROUP, $"{Key}_UserInterface", value);
                OnPropertyChanged("UserInterface", oldValue, value, true);
            }
        }

        /// <summary>
        /// true if this shadow is included in the extent used to calculate the DCS resolution
        /// </summary>
        public bool Included
        {
            get => _included;
            set
            {
                if (_included == value)
                {
                    return;
                }

                bool oldValue = _included;
                _included = value;
                ConfigManager.SettingsManager.SaveSetting(MonitorSetup.SETTINGS_GROUP, Key, value);
                OnPropertyChanged("Included", oldValue, value, true);
            }
        }
    }
}