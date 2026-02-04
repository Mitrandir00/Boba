using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevel", menuName = "BobaGame/LevelData")]
public class LevelData : ScriptableObject
{
    public bool isStoryMode; // Se true, i clienti non se ne vanno finché non sono serviti
    public int goalCointAmount; // Obiettivo per superare il livello
    
    [Header("Sequenza Clienti")]
    public List<GameObject> customerSequence; // Trascina qui i prefab dei clienti (anche quelli speciali)
}