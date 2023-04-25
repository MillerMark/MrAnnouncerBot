using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using SheetsPersist;

namespace BotCore
{
    public class AllViewerSettings : AutoSaveSettings
    {
        static Dictionary<string, ViewerSettings> allViewerSettings;

        static AllViewerSettings()
        {

        }

        static AllViewerSettings instance;
        public static AllViewerSettings Instance
        {
            get
            {
                if (instance == null)
                    instance = new AllViewerSettings();

                return instance;
            }
        }

        protected override bool HasData(string settingName) => allViewerSettings != null;

        public override void Invalidate()
        {
            allViewerSettings = null;
        }

        protected override void LoadSettings(string settingName = null) 
        {
            List<ViewerSettings> loadedViewerSettings = GoogleSheets.Get<ViewerSettings>();
            loadedViewerSettings.ForEach(y => y.SetLive());
            allViewerSettings = loadedViewerSettings.ToDictionary(x => x.ViewerID);
        }

        protected override void SaveSettings()
        {
            ViewerSettings[] dirtySettings = allViewerSettings.Values.Where(x => x.IsDirty).ToArray();
            if (dirtySettings.Length > 0)
                GoogleSheets.SaveChanges(dirtySettings);
        }

        public ViewerSettings GetViewerSettings(string userId)
        {
            LoadIfNeeded();
            if (!allViewerSettings.ContainsKey(userId))
                allViewerSettings.Add(userId, new ViewerSettings(userId));

            return allViewerSettings[userId];

        }
    }
}
