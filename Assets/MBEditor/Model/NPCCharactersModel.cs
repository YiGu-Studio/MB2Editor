using System;
using System.Xml.Serialization;
using UnityEngine;

namespace MB2Editor.Model
{
    [Serializable]
    [CreateAssetMenu(fileName = "NPCCharacters", menuName = "MB2ModuleData/NPCCharacters")]
    public class NPCCharactersModel : BaseModel
    {
        [XmlElement("NPCCharacter")]
        public NPCCharacterModel[] NPCCharacters;

    }
}
