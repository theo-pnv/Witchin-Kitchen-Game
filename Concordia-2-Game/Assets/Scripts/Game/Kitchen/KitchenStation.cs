﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class KitchenStation : MonoBehaviour
    {
        private ACookingMinigame m_miniGame;
        private Ingredient m_storedIngredient;
        private RecipeManager m_recipeManager;

        private void Awake()
        {
            m_miniGame = GetComponent<ACookingMinigame>();
            m_recipeManager = GetComponent<RecipeManager>();
            m_storedIngredient = Ingredient.NOT_AN_INGREDIENT;
        }

        private void OnCollisionEnter(Collision collision)
        {
            PickableObject ingredient = collision.gameObject.GetComponent<PickableObject>();
            if (ingredient && !ingredient.IsHeld())
            {
                if (m_recipeManager == null || m_recipeManager.CollectIngredient(ingredient.m_ingredientType))    //If is generic station, or is cauldron and needs the ingredient 
                {
                    Debug.Log("Collected a " + ingredient.m_ingredientType);
                    m_storedIngredient = ingredient.m_ingredientType;
                    GameObject.Destroy(collision.gameObject);
                    m_miniGame.StartMinigame();
                }
            }
        }

        public void SetOwner(PlayerManager owner)
        {
            m_miniGame.SetStationOwner(owner, this);

            //Apply player color to station?
        }

        public void ProcessIngredient()
        {
            if (m_recipeManager)    //Is cauldron
            {
                m_recipeManager.ProcessIngredient(m_storedIngredient);  //Add to recipe
            }
            else                //Is some other kitchen station
            {
                //Process ingredient (chop, smash, etc.)
            }

            m_storedIngredient = Ingredient.NOT_AN_INGREDIENT;
        }

        public bool CollectIngredient(Ingredient type)
        {
            return m_recipeManager.CollectIngredient(type);
        }

        public bool IsStoringIngredient()
        {
            return m_storedIngredient != Ingredient.NOT_AN_INGREDIENT;
        }
    }

}
