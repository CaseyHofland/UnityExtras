#nullable enable
using System;
using UnityEditor;
using UnityEngine;

namespace UnityExtras.Editor
{
    public static partial class Menus
    {
        [MenuItem("CONTEXT/ModelImporter/Rotate Animations to Move Perfectly Forwards")]
        private static void RotateAnimationsToMovePerfectlyForwards(MenuCommand command)
        {
            var modelImporter = (ModelImporter)command.context;
            var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(modelImporter.assetPath);
            var animationClips = Array.ConvertAll(Array.FindAll(subAssets, subAsset => subAsset is AnimationClip), animationClip => (AnimationClip)animationClip);
            var clipAnimations = modelImporter.clipAnimations;

            for (int i = 0; i < animationClips.Length; i++)
            {
                if (animationClips[i].averageSpeed.sqrMagnitude <= Vector3.kEpsilon * Vector3.kEpsilon)
                {
                    continue;
                }

                clipAnimations[i].rotationOffset -= Vector3.SignedAngle(animationClips[i].averageSpeed, Vector3.forward, Vector3.up);
            }

            modelImporter.clipAnimations = clipAnimations;
        }
    }
}
