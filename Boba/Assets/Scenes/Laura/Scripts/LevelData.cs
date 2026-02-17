using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevel", menuName = "BobaGame/LevelData")]
public class LevelData : ScriptableObject
{
    public bool isStoryMode; // Se true, i clienti non se ne vanno finché non sono serviti
    
    [Header("Sequenza Clienti")]
    public List<GameObject> customerSequence; 
}