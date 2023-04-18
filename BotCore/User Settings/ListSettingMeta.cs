using System;
using System.Linq;
using SheetsPersist;

namespace BotCore
{
    [Document(DocumentNames.ViewerSettings)]
    [Sheet("ListSettingMeta")]
    public class ListSettingMeta
    {
        public string SheetName { get; set; }
        public int MaxEntries { get; set; } = 10;
        public ListSettingMeta()
        {

        }
    }
}
