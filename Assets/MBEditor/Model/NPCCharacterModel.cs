using System;
using System.Xml.Serialization;
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

        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string default_group;
        [XmlAttribute]
        public string voice;
        [XmlAttribute]
        public string is_hero;
        [XmlAttribute]
        public string is_female;
        [XmlAttribute]
        public string is_basic_troop;
        [XmlAttribute]
        public string is_template;
        [XmlAttribute]
        public string is_child_template;
        [XmlAttribute]
        public string culture;
        [XmlAttribute]
        public string banner_symbol_mesh_name;
        [XmlAttribute]
        public string banner_symbol_color;
        [XmlAttribute]
        public string banner_key;
        [XmlAttribute]
        public string occupation;
        [XmlAttribute]
        public string civilianTemplate;
        [XmlAttribute]
        public string battleTemplate;
        [XmlAttribute]
        public string is_companion;
        [XmlAttribute]
        public string offset;
        [XmlAttribute]
        public string level;
        [XmlAttribute]
        public string age;
        [XmlAttribute]
        public string is_mercenary;
        [XmlAttribute]
        public string formation_position_preference;
        [XmlAttribute]
        public string default_equipment_set;
        [XmlAttribute]
        public string skill_template;
        [XmlAttribute]
        public string upgrade_requires;
    }
}
