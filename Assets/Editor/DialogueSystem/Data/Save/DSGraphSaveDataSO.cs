using UnityEngine;
using System;
using System.Collections.Generic;
using DS.Utilities;

namespace DS.Data.Save
{
    public class DSGraphSaveDataSO : ScriptableObject
    {
        [field: SerializeField] public string FileName {  get; set; }
        [field: SerializeField] public List<DSGroupSaveData> Groups {  get; set; }
        [field: SerializeField] public List<DSNodeSaveData> Nodes {  get; set; }
        [field: SerializeField] public List<string> OldGroupdNames {  get; set; }
        [field: SerializeField] public List<string> OldUngroupedNodeNames {  get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupedNodeNames {  get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            Groups = new List<DSGroupSaveData>();
            Nodes = new List<DSNodeSaveData>();
        }
    }

}
