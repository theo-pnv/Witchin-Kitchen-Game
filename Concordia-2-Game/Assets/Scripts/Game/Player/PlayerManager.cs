﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

namespace con2.game
{

    public class PlayerManager : MonoBehaviour
    {
        #region Attributes

        private int _ID;
        public int ID
        {
            get => _ID;
            set
            {
                _ID = value;
                GetComponent<PlayerInputController>()?.SetPlayerIndex(_ID);
            }
        }

        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                gameObject.name = _Name;
            }
        }

        public Texture Texture {
            set {
                // Ignore hair skin renderers
                var skinRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
                SkinnedMeshRenderer skinRenderer = null;
                foreach (var r in skinRenderers)
                {
                    if (r.gameObject.layer != 15)
                    {
                        skinRenderer = r;
                        break;
                    }
                }

                skinRenderer.material.mainTexture = value;
            }
        }

        public Texture NormalTexture {
            set {
                // Ignore hair skin renderers
                var skinRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
                SkinnedMeshRenderer skinRenderer = null;
                foreach (var r in skinRenderers)
                {
                    if (r.gameObject.layer != 15)
                    {
                        skinRenderer = r;
                        break;
                    }
                }

                skinRenderer.material.SetTexture("_BumpMap", value);
            }
        }

        public Texture OcclusionTexture {
            set {
                // Ignore hair skin renderers
                var skinRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
                SkinnedMeshRenderer skinRenderer = null;
                foreach (var r in skinRenderers)
                {
                    if (r.gameObject.layer != 15)
                    {
                        skinRenderer = r;
                        break;
                    }
                }

                skinRenderer.material.SetTexture("_OcclusionMap", value);
            }
        }

        public Texture RoughnessTexture {
            set {
                // Ignore hair skin renderers
                var skinRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
                SkinnedMeshRenderer skinRenderer = null;
                foreach (var r in skinRenderers)
                {
                    if (r.gameObject.layer != 15)
                    {
                        skinRenderer = r;
                        break;
                    }
                }

                skinRenderer.material.SetTexture("_MetallicGlossMap", value);
            }
        }

        private int _CompletedPotionCount;
        public int CompletedPotionCount
        {
            get => _CompletedPotionCount;
            set
            {
                _CompletedPotionCount = value;
                if (PlayerHUD)
                {
                    PlayerHUD.Score.text = _CompletedPotionCount.ToString();
                }
            }
        }

        public int CollectedIngredientCount { get; set; }

        public Rank PlayerRank { get; set; }

        public enum Rank
        {
            FIRST,
            MIDDLE,
            LAST
        }

        public PlayerHUD PlayerHUD { get; set; }

        public void SendMessageToPlayerInHUD(string message, Color color, bool stick = false)
        {
            PlayerHUD.Message.color = color;
            PlayerHUD.Message.text = message;
            if (!stick)
            {
                StartCoroutine(RemoveMessage());
            }
        }

        private IEnumerator RemoveMessage()
        {
            yield return new WaitForSeconds(5f);
            PlayerHUD.Message.text = "";
        }

        void OnDisable()
        {
            if (PlayerHUD)
            {
                Destroy(PlayerHUD.gameObject);
            }
        }

        #endregion

    }

}
