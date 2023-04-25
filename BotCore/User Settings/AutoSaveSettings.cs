using System;
using System.Linq;
using System.Timers;
using SheetsPersist;

namespace BotCore
{
    public abstract class AutoSaveSettings
    {
        public const double OneMinuteInMs = 1000 * 60;

        static bool isDirty;
        static Timer autoSaveTimer = new Timer(OneMinuteInMs);

        public AutoSaveSettings()
        {
            autoSaveTimer.Elapsed += AutoSaveTimer_Elapsed;
        }

        void AutoSaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            autoSaveTimer.Stop();
            SaveIfNeeded();
        }

        static void RegisterDocumentIfNeeded()
        {
            if (registeredDocument)
                return;
            registeredDocument = true;
            GoogleSheets.RegisterDocumentID(DocumentNames.ViewerSettings, "1CLyl0k0her-IsUAf_rAUkIQLIYI5-ig0V4kWR8428Us");
        }

        protected void LoadIfNeeded(string settingName = null)
        {
            if (HasData(settingName))
                return;

            RegisterDocumentIfNeeded();
            LoadSettings(settingName);
        }

        public void SaveIfNeeded()
        {
            if (!isDirty)
                return;

            RegisterDocumentIfNeeded();

            SaveSettings();

            lastSave = DateTime.UtcNow;
            isDirty = false;
        }

        public void SetDirty()
        {
            isDirty = true;
            autoSaveTimer.Stop();

            TimeSpan timeSinceLastSave = DateTime.UtcNow - lastSave;
            if (timeSinceLastSave.TotalMinutes > 1)
                SaveIfNeeded();
            else
                autoSaveTimer.Start();
        }

        protected abstract void LoadSettings(string settingName = null);
        protected abstract void SaveSettings();
        protected abstract bool HasData(string settingName = null);
        public abstract void Invalidate();

        static DateTime lastSave;
        static bool registeredDocument;

    }
}
