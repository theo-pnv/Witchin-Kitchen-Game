﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class SharedCamera : MonoBehaviour
    {
        [Range(0.0f, 1.0f)]
        public float smoothTime = 0.3f;

        public float m_zoomFactor = 0.1f;

        private Vector3 velocity = Vector3.zero;

        private Vector3 camPos;

        private GameObject[] m_players;
        private Vector3 m_initialDistanceFromCenter;
        
        void Start()
        {
            // Snap at start to avoid interpolation
            m_initialDistanceFromCenter = transform.position;
        }

        public void SetPlayers(GameObject[] players)
        {
            m_players = players;
        }

        void LateUpdate()
        {
            camPos = getNewCamTargetPos();
            transform.position = Vector3.SmoothDamp(transform.position, camPos, ref velocity, smoothTime);
        }

        private Vector3 getNewCamTargetPos()
        {
            var middle = Vector3.zero;
            var furthestFromMiddle = Vector3.zero;

            foreach (var t in m_players)
            {
                var playerPosition = t.transform.position;
                middle += playerPosition;
                if (playerPosition.magnitude > furthestFromMiddle.magnitude)
                    furthestFromMiddle = playerPosition;
            }

            //Average the positions with the x3 to compensate for the middle
            middle /= (3*m_players.Length);
            middle += m_zoomFactor*m_initialDistanceFromCenter.normalized*furthestFromMiddle.magnitude;  //zoom
            middle += m_initialDistanceFromCenter;  //Keeps camera above the arena, pointed at 'middle'
            return middle;
        }

    }

}
