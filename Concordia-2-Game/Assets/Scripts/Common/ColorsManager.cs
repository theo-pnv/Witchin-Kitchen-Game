﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    public class ColorsManager : MonoBehaviour
    {
        static private ColorsManager Instance;

        static public ColorsManager Get()
        {
            return Instance;
        }


        public Color[] PlayerMeshColors = new Color[4];

        public Color[] CauldronLiquidColors = new Color[4];

        [ColorUsageAttribute(true, true)]
        public Color[] CauldronBubblesColors = new Color[4];

        void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
