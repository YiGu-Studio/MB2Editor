using System;
using UnityEngine;

namespace MB2Editor.Model
{
    [Serializable]
    [CreateAssetMenu(fileName = "NewModuleData", menuName = "MB2ModuleData")]
    public class BaseModel : ScriptableObject
    {
        [HideInInspector]
        public int version = 0x00010000;    // for version migration

        public string serilizedData;

        public string id;

        public string NameSpace = "MB2Editor";

        public string element = "Unassigned";
    }
}
