using System;
using System.Linq;
using SheetsPersist;

namespace BotCore
{
    [Document(DocumentNames.ViewerSettings)]
    [Sheet("Misc")]
    public class ViewerSettings
    {
        bool isDirty;
        bool isLive;

        // TODO: Find out why Column is needed when I specify the Indexer attribute.
        [Indexer]
        [Column]
        public string ViewerID { get; set; }

        string smokeColor;

        
        public void SetLive()
         {
            isLive = true;
        }

        void SetDirty()
        {
            isDirty =true;
            if (isLive)
                AllViewerSettings.Instance.SetDirty();
        }

        [Column]
        public string SmokeColor { get => smokeColor;
            set
            {
                if (smokeColor == value)
                    return;
                smokeColor = value;
                SetDirty();
            }
        }

        string smokeLifetime;
        [Column]
        public string SmokeLifetime { get => smokeLifetime;
            set
            {
                if (smokeLifetime == value)
                    return;
                smokeLifetime = value;
                SetDirty();
            }
        }

        string droneOverrideColor;
        [Column]
        public string DroneOverrideColor { get => droneOverrideColor;
            set
            {
                if (droneOverrideColor == value)
                    return;
                droneOverrideColor = value;
                SetDirty();
            }
        }
        
        public bool IsDirty => isDirty;

        public ViewerSettings()
        {

        }

        public ViewerSettings(string viewerID)
        {
            ViewerID = viewerID;
            SetLive();
        }
    }
}
