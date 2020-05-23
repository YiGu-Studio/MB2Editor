using System;
using UnityEngine;

namespace MB2Editor.Model
{
    [Serializable]
    public class BaseModel : ScriptableObject
    {
        [HideInInspector]
        public int version = 0x00010000;    // for version migration
    }
}
