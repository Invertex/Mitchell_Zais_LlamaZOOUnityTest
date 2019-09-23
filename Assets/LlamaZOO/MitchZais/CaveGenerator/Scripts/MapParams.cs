﻿using System.Runtime.CompilerServices;
using UnityEngine;

namespace LlamaZOO.MitchZais.CaveGenerator
{
    /// <summary>
    /// Only the essential data needed for reconstruction of the map.
    /// </summary>
    [System.Serializable]
    public class MapParams
    {
        public int seed;
        public int width, height;
        public float wallHeight;

        [Range(0.3f, 1.0f)] public float fillDensity;

        public RefinementStep[] refinementSteps;

        public int SubdividedWidth { get { return SubdividedSize(width, TotalSubdivisions); } }
        public int SubdividedHeight { get { return SubdividedSize(height, TotalSubdivisions); } }

        /// <summary>
        /// Doubles value for each division
        /// </summary>
        /// <param name="val"></param>
        /// <param name="divisions"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int SubdividedSize(int val, int divisions)
        {
            for (int i = 0; i < divisions; i++)
            {
                val += val;
            }
            return val;
        }

        public int TotalSubdivisions
        {
            get
            {
                int subdivs = 0;
                foreach(var step in refinementSteps) { if(step.subdivideFirst){ subdivs++; } }
                return subdivs;
            }
        }

        [Header("Cleanup Parameters")]
        public int smallestRoomArea;
        public int smallestWallArea;

        [System.Serializable]
        public struct RefinementStep
        {
            [Range(0, 20)] public int iterations;
            [Range(1, 7)] public int roomLifeWeight;
            [Range(1, 7)] public int roomDeathWeight;
            public bool subdivideFirst;

            public RefinementStep(int iterations = 1, int roomLifeWeight = 4, int roomDeathWeight = 4, bool subdivide = false)
            {
                this.iterations = Mathf.Clamp(iterations, 1, 20);
                this.roomLifeWeight = Mathf.Clamp(roomLifeWeight, 1, 7);
                this.roomDeathWeight = Mathf.Clamp(roomDeathWeight, 1, 7);
                this.subdivideFirst = subdivide;
            }
        }

        public int GenerateNewSeed()
        {
            return this.seed = Random.Range(int.MinValue, int.MaxValue);
        }

        public MapParams(MapParams mapParams) : this (mapParams.seed, mapParams.width, mapParams.height, mapParams.fillDensity, mapParams.smallestRoomArea, mapParams.smallestWallArea, mapParams.wallHeight)
        {
            if(mapParams.refinementSteps != null)
            {
                this.refinementSteps = mapParams.refinementSteps.Clone() as RefinementStep[];
            }
        }

        public MapParams(int seed = 0, int width = 128, int height = 128, float density = 0.5f, int smallestRoomArea = 24, int smallestWallArea = 12, float wallHeight = 5f, RefinementStep[] refinementSteps = null)
        {
            this.seed = seed;
            this.width = width;
            this.height = height;
            this.fillDensity = density;
            this.smallestRoomArea = smallestRoomArea;
            this.smallestWallArea = smallestWallArea;
            this.wallHeight = wallHeight;
            this.refinementSteps = refinementSteps == null ? new RefinementStep[]{new RefinementStep(5, 4, 4)} : refinementSteps;
        }

        public MapParams() : this(0){ System.Random rand = new System.Random(); seed = rand.Next(); }
    }
}