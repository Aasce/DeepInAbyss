using UnityEngine;

namespace Asce.Game.Entities
{
    [CreateAssetMenu(menuName = "Asce/Entities/Information", fileName = "Entity Information")]
    public class SO_EntityInformation : ScriptableObject
    {
        public static readonly string noName = "Unknown";

        [SerializeField] private string _name = noName;
        [SerializeField, TextArea] private string _description = string.Empty;


        public string Name => _name;
        public string Description => _description;
    }
}