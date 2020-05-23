using System;
using UnityEngine;

namespace MB2Editor.Model
{
    [Serializable]
    [CreateAssetMenu(fileName = "NPCCharacters", menuName = "MB2ModuleData/NPCCharacters")]
    public class NPCCharactersModel : BaseModel
    {
        public NPCCharacterModel[] NPCCharacters;

    }
}
