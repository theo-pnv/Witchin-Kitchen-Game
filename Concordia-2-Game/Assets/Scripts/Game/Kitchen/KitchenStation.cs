﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public abstract class KitchenStation : MonoBehaviour
    {
        private ACookingMinigame m_miniGame;
        protected Ingredient m_storedIngredient;
        private RecipeManager m_recipeManager;
        private PlayerManager m_owner;

        protected abstract void OnAwake();
        public abstract bool ShouldAcceptIngredient(Ingredient type);
        protected abstract void OnCollectIngredient();
        public abstract void ProcessIngredient();
        
        private void Awake()
        {
            m_miniGame = GetComponent<ACookingMinigame>();
            m_recipeManager = GetComponent<RecipeManager>();
            m_storedIngredient = Ingredient.NOT_AN_INGREDIENT;
            OnAwake();
        }

        private void OnCollisionEnter(Collision collision)
        {
            PickableObject ingredient = collision.gameObject.GetComponent<PickableObject>();
            if (ingredient && !ingredient.IsHeld())
            {
                if (ShouldAcceptIngredient(ingredient.m_ingredientType))
                {
                    OnCollectIngredient();
                    m_storedIngredient = ingredient.m_ingredientType;
                    Destroy(collision.gameObject);
                    m_miniGame.StartMinigame();
                }
            }
        }

        public void SetOwner(PlayerManager owner)
        {
            m_owner = owner;
            m_miniGame.SetStationOwner(owner, this);
        }

        public PlayerManager GetOwner() => m_owner;

        public bool IsStoringIngredient()
        {
            return m_storedIngredient != Ingredient.NOT_AN_INGREDIENT;
        }
    }

}