using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ability), false)]
public class AbilityGUI : Editor
{

    #region Serialized Properties
    //ABILITY NAME
    SerializedProperty abilityName;

    //ABILITY ENUMS
    SerializedProperty abilityType;
    SerializedProperty playerEffects;
    SerializedProperty activationMethod;
    SerializedProperty abilityAppliance;

    //ABILITY TIME
    SerializedProperty activeTime;
    SerializedProperty cooldownTime;
    SerializedProperty delayTime;

    //ABILITY CHECK BOOLEANS
    SerializedProperty isInstantAbility;
    SerializedProperty hasDelay;

    //WHICH ABILITY
    SerializedProperty whichAbility;

    //ACTIVATION METHOD
    SerializedProperty radius;
    SerializedProperty projectileSpeed;
    SerializedProperty bulletColor;

    //STATUS EFFECTS
    SerializedProperty statusEffectData;

    //DISPLAY BOOLEANS
    bool displayAbilityEnums, displayAbilityTime;
    #endregion

    private void OnEnable()
    {
        #region Serialized Properties
        //ABILITY NAME
        abilityName = serializedObject.FindProperty("abilityName");

        //ABILITY ENUMS
        abilityType      = serializedObject.FindProperty("abilityType");
        playerEffects    = serializedObject.FindProperty("playerEffects");
        activationMethod = serializedObject.FindProperty("activationMethod");
        abilityAppliance = serializedObject.FindProperty("abilityAppliance");

        //ABILITY TIME
        activeTime   = serializedObject.FindProperty("activeTime");
        cooldownTime = serializedObject.FindProperty("cooldownTime");
        delayTime    = serializedObject.FindProperty("delayTime");

        //ABILITY BOOLEANS
        isInstantAbility = serializedObject.FindProperty("isInstantAbility");
        hasDelay = serializedObject.FindProperty("hasDelay");

        //ACTIVATION METHOD
        radius = serializedObject.FindProperty("radius");
        projectileSpeed = serializedObject.FindProperty("projectileSpeed");
        bulletColor = serializedObject.FindProperty("bulletColor");

        //STATUS EFFECTS
        statusEffectData = serializedObject.FindProperty("statusEffectData");

        //WHICH ABILITY TO USE
        whichAbility = serializedObject.FindProperty("whichAbility");
        #endregion
    }


    public override void OnInspectorGUI()
    {
        Ability _ability = (Ability)target;
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        //ABILITY NAME
        EditorGUILayout.PropertyField(abilityName);

        #region Ability Enums
        //ABILITY ENUMS
        displayAbilityEnums = EditorGUILayout.BeginFoldoutHeaderGroup(displayAbilityEnums, "Ability Specifications");
        if (displayAbilityEnums)
        {
            #region DISPLAY BOOLEANS
            //BOOLS JUST TO CHECK DISPLAY ORDER
            bool canDisplayAbilityType      = true;
            bool canDisplayPlayerEffects    = false;
            bool canDisplayActivationMethod = false;
            bool canDisplayAbilityAppliance = false;
            #endregion

            #region ABILITY TYPE
            if (canDisplayAbilityType)
            {
                EditorGUILayout.PropertyField(abilityType); //ACTUALLY DISPLAYS THE 'AbilityType' ENUM
                if(abilityType.enumValueIndex != -1) //DISPLAY NEXT ABILITY PROPERTY
                {
                    canDisplayPlayerEffects = true;
                }

                //SWITCH CASE TO FIND OUT WHICH OBJECT TYPE TO FIELD IN THE INSPECTOR
                switch (abilityType.enumValueIndex)
                {
                    case (int)AbilityClass.DAMAGE: //DAMAGE ABILITY
                        EditorGUILayout.ObjectField(whichAbility, typeof(DamageAbility));
                        break;
                    case (int)AbilityClass.HEALING: //HEALING ABILITY
                        EditorGUILayout.ObjectField(whichAbility, typeof(HealingAbility));
                        break;
                    case (int)AbilityClass.MOVEMENT://MOVEMENT ABILITY
                        EditorGUILayout.ObjectField(whichAbility, typeof(MovementAbility));
                        break;
                    case (int)AbilityClass.FOV: //FOV ABILITY
                        EditorGUILayout.ObjectField(whichAbility, typeof(FOVAbility));
                        break;
                    default: //ANY ABILITY
                        EditorGUILayout.ObjectField(whichAbility, typeof(Ability));
                        break;
                }
            }
            #endregion

            #region PLAYER EFFECTS
            if (canDisplayPlayerEffects)
            {
                EditorGUILayout.PropertyField(playerEffects);
                if (playerEffects.enumValueIndex != -1) //DISPLAY NEXT ABILITY PROPERTY
                {
                    canDisplayActivationMethod = true;
                }

                //SWITCH CASE TO FIND OUT WHAT THE ABILITY WILL DO
                switch (playerEffects.enumValueIndex)
                {
                    case (int)PlayerEffects.PLAYER: //AFFECTS THE PLAYER

                        break;
                    case (int)PlayerEffects.ENEMY: //AFFECTS ENEMIES (TEAMMATES, ENEMIES, etc.)
                         
                        break;
                    case (int)PlayerEffects.TEAM: //AFFECTS TEAMMATES (TEAMMATES, ENEMIES, etc.)

                        break;
                    default: //AFFECTS THE PLAYER
                        
                        break;
                }
            }
            #endregion

            #region ACTIVATION METHOD
            if (canDisplayActivationMethod)
            {
                EditorGUILayout.PropertyField(activationMethod);
                if (activationMethod.enumValueIndex != -1) //DISPLAY NEXT ABILITY PROPERTY
                {
                    canDisplayAbilityAppliance = true;
                }

                //SWITCH CASE TO FIND OUT HOW THE ABILITY WILL BE ACTIVATED
                switch (activationMethod.enumValueIndex)
                {
                    case (int)ActivationMethod.HITSCAN: //THE ABILITY WILL BE SHOT OUT OF A RAYCAST, MUCH LIKE A BULLET
                        //EditorGUILayout.PropertyField(bulletColor);
                        break;
                    case (int)ActivationMethod.PROJECTILE: //THE ABILITY WILL BE SHOT OUT OF A MOVING PROJECTILE
                        //EditorGUILayout.PropertyField(projectileSpeed);
                        break;
                    case (int)ActivationMethod.ON_SELF: //THE ABILITY WILL BE CASTED ON THE PLAYER ITSELF

                        break;
                    case (int)ActivationMethod.AURA: //THE ABILITY WILL SPAWN A 2D CIRCLE COLLIDER AND ANYTHING WITHIN THE COLLIDER WILL BE AFFECTED
                        //EditorGUILayout.PropertyField(radius);
                        if(abilityType.enumValueIndex == (int)AbilityClass.DAMAGE || abilityType.enumValueIndex == (int)AbilityClass.HEALING)
                        {
                            //IF THE CHOSEN INDEX IS DAMAGE,
                            EditorGUILayout.ObjectField(statusEffectData, typeof(DOTStatusEffect));
                        }
                        else if(abilityType.enumValueIndex == (int)AbilityClass.MOVEMENT)
                        {
                            EditorGUILayout.ObjectField(statusEffectData, typeof(MovementStatusEffect));
                        }
                        break;
                    default: //THE ABILITY WILL BE CASTED ON THE PLAYER ITSELF

                        break;
                }
            }
            #endregion

            #region ABILITY APPLIANCE
            //if (canDisplayAbilityAppliance)
            //    EditorGUILayout.PropertyField(abilityAppliance);
            #endregion
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(5f);
        #endregion


        #region ABILITY TIME
        //ABILITY TIME
        displayAbilityTime = EditorGUILayout.BeginFoldoutHeaderGroup(displayAbilityTime, "Ability Time");
        if (displayAbilityTime)
        {
            EditorGUILayout.PropertyField(isInstantAbility);
            EditorGUILayout.PropertyField(hasDelay);
            EditorGUILayout.PropertyField(cooldownTime);
            if (!_ability.isInstantAbility)
            {
                EditorGUILayout.PropertyField(activeTime);
            }

            if (_ability.hasDelay)
            {
                EditorGUILayout.PropertyField(delayTime);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(5f);
        #endregion


        //APPLIES CHANGES FROM 'SerializedProperty' TO ACTUAL 'Ability' CLASS DATA
        serializedObject.ApplyModifiedProperties();
    }

    #region In Progress
    private void makeFoldoutGroup(bool isDisplayed, string foldoutName, List<SerializedProperty> data)
    {
        EditorGUILayout.Space(5f);
        isDisplayed = EditorGUILayout.BeginFoldoutHeaderGroup(isDisplayed, foldoutName);
        if (isDisplayed)
        {
            setEditorGUILayout(data);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    private void setEditorGUILayout(List<SerializedProperty> data)
    {
        foreach (SerializedProperty var in data)
        {
            EditorGUILayout.PropertyField(var);
        }
    }
    #endregion
}
