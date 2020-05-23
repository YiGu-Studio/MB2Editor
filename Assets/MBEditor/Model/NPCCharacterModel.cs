using System;
using UnityEngine;

namespace MB2Editor.Model
{
    [CreateAssetMenu(fileName = "NPCCharacter", menuName = "MB2ModuleData/NPCCharacter")]
    [Serializable]
    public class NPCCharacterModel : BaseModel
    {
        [Serializable]
        public class Equipment
        {
            public string slot;
            public string id;
            public int amount;
        }

        public string id;
        public string default_group;
        public string voice;
        public string is_hero;
        public string is_female;
        public string is_basic_troop;
        public string is_template;
        public string is_child_template;
        public string upgrade_requires;
    }
}
