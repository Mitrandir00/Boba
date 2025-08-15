using UnityEngine;
using System;

public enum Ingredient
{
    // Topping
    Tapioca,
    Mango,
    Fragola,
    Pesca,
    Limone,
    Arancia,

    // Bevanda
    TeVerde,
    Latte,

    // Altro
    SciroppoMango,
    SciroppoFragola,
    SciroppoPesca,
    SciroppoLimone,
    SciroppoArancia,
    Cioccolato,
    Panna
}

// 0..3 = No / Light / Normal / Extra  (parti pure da 2=Normal)
[Serializable]
public class IngredientSpec
{
    public Ingredient ingredient;
    [Range(0,3)] public int amount = 2;
}
