﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public enum Ingredient
    {
        GHOST_PEPPER,
        NEWT_EYE,
        MUSHROOM,
        FLOWER,
        FISH,
        NOT_AN_INGREDIENT
    }

    public class IngredientType : MonoBehaviour
    {
        public Ingredient m_type = Ingredient.NOT_AN_INGREDIENT;
    }
}
