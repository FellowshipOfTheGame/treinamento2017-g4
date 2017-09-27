using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
#if UNITY_EDITOR

using UnityEditor;

//criar um editor customizado para mostrar variavel quando selecionado
[CustomEditor(typeof(EnemyControllerBossOne))]
[CanEditMultipleObjects]
public class EnemyControllerBossOne_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields

        EnemyControllerBossOne script = (EnemyControllerBossOne)target;

        // draw checkbox for the bool
        //script.isGunsEqualDistribution = EditorGUILayout.Toggle("Is Guns Equal Distribution", script.isGunsEqualDistribution);
        if (!script.isGunsEqualDistribution) // if bool is true, show other fields
        {
            script.distanceBetweenGuns = EditorGUILayout.FloatField("Distance Between Guns", script.distanceBetweenGuns);
            script.initialAngle = EditorGUILayout.FloatField("Initial Angle", script.initialAngle);
        }

        if (script.canBeImmune) // if bool is true, show other fields
        {
            script.timeReceiveDamage = EditorGUILayout.FloatField("Time Receive Damage", script.timeReceiveDamage);
            script.timeImmune = EditorGUILayout.FloatField("Time Immune", script.timeImmune);
            script.colorReceiveDamage = EditorGUILayout.ColorField("Color Receive Damage", script.colorReceiveDamage);
            script.colorImmune = EditorGUILayout.ColorField("Color Immune", script.colorImmune);
        }
    }
}

#else
public class EnemyControllerBossOne_Editor : MonoBehavior
{
}
#endif**/
