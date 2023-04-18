using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;
using System.Threading;
using TwitchLib.Client;
using BotCore;
using System.Data;

namespace BotCore
{
    /// <summary>
    /// Static class to manage all string list settings.
    /// </summary>
    public static class AllViewerListSettings
    {
        private const double OneMinuteInMs = 1000 * 60;
        static System.Timers.Timer autoSaveTimer = new System.Timers.Timer(OneMinuteInMs);
        static Dictionary<string, DataSheet> allDataSheets = new Dictionary<string, DataSheet>();
        static Dictionary<string, ListSettingMeta> allSettingMeta = new Dictionary<string, ListSettingMeta>();
        static bool registeredDocument;
        static bool isDirty;
        static DateTime lastSave;

        static AllViewerListSettings()
        {
            autoSaveTimer.Elapsed += AutoSaveTimer_Elapsed;
        }

        private static void AutoSaveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        static void Save()
        {
            lastSave = DateTime.UtcNow;
        }

        public static void SetDirty()
        {
            isDirty = true;
            autoSaveTimer.Stop();

            TimeSpan timeSinceLastSave = DateTime.UtcNow - lastSave;
            if (timeSinceLastSave.TotalMinutes > 1)
                SaveIfNeeded();
            else
                autoSaveTimer.Start();
        }

        public static void Invalidate()
        {
            SaveIfNeeded();
            allDataSheets.Clear();
            allSettingMeta?.Clear();
        }

        static void SaveIfNeeded()
        {
            if (!isDirty)
                return;

            RegisterDocumentIfNeeded();

            isDirty = false;
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

        public static DataRow GetViewerSetting(string viewerId, string userName, string settingName)
        {
            DataSheet listSettings = GetListSettings(settingName);
            return listSettings?.GetDataRow(viewerId, userName);
        }

        public static DataSheet GetListSettings(string settingName)
        {
            LoadIfNeeded(settingName);
            allDataSheets.TryGetValue(settingName.ToLower(), out DataSheet value);
            return value;
        }

        public static bool HasViewerSettings(string settingName)
        {
            LoadIfNeeded(settingName);
            allDataSheets.TryGetValue(settingName.ToLower(), out DataSheet value);
            return value != null;
        }

        static void LoadIfNeeded(string settingName)
        {
            string settingNameLowerCase = settingName.ToLower();
            RegisterDocumentIfNeeded();


            //allSettings.Clear();

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

        static void RegisterDocumentIfNeeded()
        {
            if (registeredDocument)
                return;
            registeredDocument = true;
            SheetsPersist.GoogleSheets.RegisterDocumentID(DocumentNames.ViewerSettings, "1CLyl0k0her-IsUAf_rAUkIQLIYI5-ig0V4kWR8428Us");
        }

        public static void AddViewerSetting(string settingName, DataRow viewerSetting)
        {
            DataSheet listSettings = GetListSettings(settingName);
            if (listSettings == null)
                return;

            listSettings.AddDataRow(viewerSetting);
        }
    }
}
