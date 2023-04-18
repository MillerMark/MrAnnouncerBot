using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace BotCore
{
    [Document(DocumentNames.ViewerSettings)]
    [Sheet("StringLists")]
    public class DataSheet
    {
        public ListSettingMeta Meta { get; set; }
        public int RowCount => allDataRows.Count;

        /// <summary>
        /// A dictionary of all data rows in a sheet, indexed by viewerId.
        /// </summary>
        Dictionary<string, DataRow> allDataRows = new Dictionary<string, DataRow>();

        public DataSheet()
        {

        }

        public DataRow GetDataRow(string viewerId, string userName)
        {
            LoadIfNeeded();
            allDataRows.TryGetValue(viewerId, out DataRow value);
            if (value == null)
            {
                value = new DataRow(viewerId, userName);
                allDataRows.Add(viewerId, value);
            }

            return value;
        }

        private void LoadIfNeeded()
        {
            if (allDataRows.Count != 0)
                return;

            var settings = GoogleSheets.Get<DataRow>(DocumentNames.ViewerSettings, Meta.SheetName);
            foreach (var setting in settings)
                allDataRows.Add(setting.ViewerID, setting);
        }

        public void AddDataRow(DataRow dataRow)
        {
            allDataRows[dataRow.ViewerID] = dataRow;
        }

        public IEnumerable<DataRow> GetDirtyDataRows()
        {
            return allDataRows.Values.Where(x => x.IsDirty);
        }
    }
}
