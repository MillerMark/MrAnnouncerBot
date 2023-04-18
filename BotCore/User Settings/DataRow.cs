using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using SheetsPersist;
using Microsoft.Extensions.Primitives;
using System.Reflection;

namespace BotCore
{
    [Document(DocumentNames.ViewerSettings)]
    [Sheet("WillBeOverridden")]
    public class DataRow
    {
        [Indexer]
        [Column]
        public string ViewerID { get; set; }

        [Column]
        public string UserName { get; set; }

        [Column]
        public string Item0 { get; set; }

        [Column]
        public string Item1 { get; set; }

        [Column]
        public string Item2 { get; set; }
        
        [Column]
        public string Item3 { get; set; }
        
        [Column]
        public string Item4 { get; set; }
        
        [Column]
        public string Item5 { get; set; }
        
        [Column]
        public string Item6 { get; set; }
        
        [Column]
        public string Item7 { get; set; }
        
        [Column]
        public string Item8 { get; set; }
        
        [Column]
        public string Item9 { get; set; }

        public string SettingName { get; set; }

        List<string> items;
        
        bool isDirty;
        internal bool IsDirty => isDirty;

        public DataRow(string viewerID, string userName)
        {
            ViewerID = viewerID;
            UserName = userName;
        }

        public DataRow()
        {

        }

        void LoadInternalList()
        {
            if (items != null)
                return;
            items = new List<string>();
            items.Add(Item0);
            items.Add(Item1);
            items.Add(Item2);
            items.Add(Item3);
            items.Add(Item4);
            items.Add(Item5);
            items.Add(Item6);
            items.Add(Item7);
            items.Add(Item8);
            items.Add(Item9);
        }

        public void GetReadyForSave()
        {
            if (items.Count <= 9)
                return;
            Item0 = items[0];
            Item1 = items[1];
            Item2 = items[2];
            Item3 = items[3];
            Item4 = items[4];
            Item5 = items[5];
            Item6 = items[6];
            Item7 = items[7];
            Item8 = items[8];
            Item9 = items[9];
        }

        void Add(int index, string value)
        {
            items[index] = value;
            SetDirty();
        }

        public void Delete(int index)
        {
            items[index] = string.Empty;
            SetDirty();
        }

        private void SetDirty()
        {
            isDirty = true;
            AllViewerListSettings.SetDirty();
        }

        public void DataHasBeenSaved()
        {
            isDirty = false;
        }

        public bool HasEmptySlot()
        {
            return GetIndexOfFirstEmptySlot() >= 0;
        }

        public int GetIndexOfFirstEmptySlot()
        {
            LoadInternalList();
            for (int i = 0; i < items.Count; i++)
                if (string.IsNullOrEmpty(items[i]))
                    return i;
            return -1;  
        }

        /// <summary>
        /// Adds the specified setting to the first open slot. If there are no open slots, then 
        /// the value is not added.
        /// </summary>
        /// <param name="value">The string value to add to the slot.</param>
        public void Add(string value)
        {
            int index = GetIndexOfFirstEmptySlot();
            if (index < 0)
                return;
            Add(index, value);
        }

        string GetValue(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "[empty]";
            return str;
        }

        public string GetSettingReport()
        {
            LoadInternalList();
            StringBuilder report = new StringBuilder();
            for (int i = 0; i < items.Count; i++)
            {
                report.Append($"{i}. {GetValue(items[i])}");
                 if (i < items.Count - 1)
                    report.Append(", ");
            }

            return report.ToString();
        }

        public void DeleteAll()
        {
            for (int i = 0; i < 9; i++)
                items[i] = string.Empty;
            SetDirty();
        }

        public string SelectRandom()
        {
            LoadInternalList();
            List<string> availableValues = items.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            if (availableValues.Count == 0)
                return null;
            Random random = new Random();
            int index = random.Next(availableValues.Count);
            return availableValues[index];
        }
    }
}
