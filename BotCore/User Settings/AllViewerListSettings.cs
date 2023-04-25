using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;
using System.Threading;
using TwitchLib.Client;
using BotCore;
using System.Data;
using System.Timers;

namespace BotCore
{
    /// <summary>
    /// Static class to manage all string list settings.
    /// </summary>
    public class AllViewerListSettings: AutoSaveSettings
    {
        static Dictionary<string, DataSheet> allDataSheets = new Dictionary<string, DataSheet>();
        static Dictionary<string, ListSettingMeta> allSettingMeta = new Dictionary<string, ListSettingMeta>();
        
        static AllViewerListSettings()
        {
        }

        static AllViewerListSettings instance;
        public static AllViewerListSettings Instance {
            get
            {
                if (instance == null)
                    instance = new AllViewerListSettings();

                return instance;
            }
        }

        public override void Invalidate()
        {
            SaveIfNeeded();
            allDataSheets.Clear();
            allSettingMeta?.Clear();
        }

        public DataRow GetViewerSetting(string viewerId, string userName, string settingName)
        {
            DataSheet listSettings = GetListSettings(settingName);
            return listSettings?.GetDataRow(viewerId, userName);
        }

        public DataSheet GetListSettings(string settingName)
        {
            LoadIfNeeded(settingName);
            allDataSheets.TryGetValue(settingName.ToLower(), out DataSheet value);
            return value;
        }

        public bool HasViewerSettings(string settingName)
        {
            LoadIfNeeded(settingName);
            allDataSheets.TryGetValue(settingName.ToLower(), out DataSheet value);
            return value != null;
        }


        public void AddViewerSetting(string settingName, DataRow viewerSetting)
        {
            DataSheet listSettings = GetListSettings(settingName);
            if (listSettings == null)
                return;

            listSettings.AddDataRow(viewerSetting);
        }

        protected override void LoadSettings(string settingName = null)
        {
            string settingNameLowerCase = settingName.ToLower();

            if (allSettingMeta == null || allSettingMeta.Count == 0)
            {
                allSettingMeta = new Dictionary<string, ListSettingMeta>();
                var allMeta = GoogleSheets.Get<ListSettingMeta>();
                foreach (ListSettingMeta meta in allMeta)
                    allSettingMeta[meta.SheetName.ToLower()] = meta;
            }

            if (!allSettingMeta.TryGetValue(settingNameLowerCase, out ListSettingMeta listSettingMeta))
            {
                Twitch.Chat(Twitch.CodeRushedClient, $"Setting \"{settingNameLowerCase}\" not found.");
                return;
            }

            if (!allDataSheets.TryGetValue(settingNameLowerCase, out DataSheet dataSheet))
            {
                dataSheet = new DataSheet();
                dataSheet.Meta = listSettingMeta;
                allDataSheets.Add(settingNameLowerCase, dataSheet);
            }

            if (dataSheet.RowCount > 0)
                return;  // We don't need to load.

            var dataRows = GoogleSheets.Get<DataRow>(DocumentNames.ViewerSettings, settingNameLowerCase);

            foreach (DataRow dataRow in dataRows)
                dataSheet.AddDataRow(dataRow);
        }

        protected override void SaveSettings()
        {
            foreach (DataSheet listSettings in allDataSheets.Values)
            {
                IEnumerable<DataRow> dirtySettings = listSettings.GetDirtyDataRows();
                if (dirtySettings.Any())
                {
                    foreach (DataRow viewerSetting in dirtySettings)
                        viewerSetting.GetReadyForSave();
                    GoogleSheets.SaveChanges(DocumentNames.ViewerSettings, listSettings.Meta.SheetName, dirtySettings.ToArray(), typeof(DataRow));
                    foreach (DataRow viewerSetting in dirtySettings)
                        viewerSetting.DataHasBeenSaved();
                }
            }
        }

        protected override bool HasData(string settingName)
        {
            if (allSettingMeta == null)
                return false;

            return allSettingMeta.TryGetValue(settingName.ToLower(), out ListSettingMeta _);
        }
    }
}
