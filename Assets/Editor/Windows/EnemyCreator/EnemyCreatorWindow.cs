using Asce.Editors.Templates;
using Asce.Editors.Utils;
using Asce.Game.Entities;
using Asce.Game.Entities.Enemies;
using Asce.Game.Items;
using Asce.Game.UIs.Creatures;
using Asce.Managers.Utils;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Asce.Editors.Windows
{
    public class EnemyCreatorWindow : EditorWindow
    {
        private static readonly string _categoryFolderPrefsKey = "EnemyCreatorWindow_CategoryFolder";
        private static readonly string _prefabTemplatePrefsKey = "EnemyCreatorWindow_PrefabTemplate";

        private DefaultAsset _categoryFolder;
        private GameObject _prefabTemplate;

        private string _enemyGroupName = string.Empty;
        private string _enemyName = "Enemy";

        private bool _isDropSpoils = true;
        private bool _isCreateAIScript = true;

        // Window config
        private Vector2 _mainScrollPosition;

        private string _folderPath;
        private SO_EntityInformation _enemyInformation;
        private SO_EnemyBaseStats _enemyBaseStats;
        private SO_DroppedSpoils _creatureDroppedSpoils;
        private GameObject _enemyPrefab;
        private Animator _enemyAnimator;
        private Material _enemyMaterial;
        private UICreatureCanvas _creatureUI;

        [MenuItem("Asce/Windows/Enemy Creator")]
        public static void ShowWindow()
        {
            GetWindow<EnemyCreatorWindow>(title: "Enemy Creator");
        }

        private void OnEnable()
        {
            string folderPath = EditorPrefs.GetString(_categoryFolderPrefsKey, string.Empty);
            if (!string.IsNullOrEmpty(folderPath))
                _categoryFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderPath);

            string prefabPath = EditorPrefs.GetString(_prefabTemplatePrefsKey, string.Empty);
            if (!string.IsNullOrEmpty(prefabPath))
                _prefabTemplate = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        }

        private void OnGUI()
        {
            using (var scroll = new EditorGUILayout.ScrollViewScope(_mainScrollPosition))
            {
                _mainScrollPosition = scroll.scrollPosition;

                _categoryFolder = EditorLayoutUtils.ObjectFieldSaveRefs(_categoryFolder, _categoryFolderPrefsKey, "Category Folder");
                _prefabTemplate = EditorLayoutUtils.ObjectFieldSaveRefs(_prefabTemplate, _prefabTemplatePrefsKey, "Prefab Template");


                EditorGUILayout.Space();
                _enemyGroupName = EditorGUILayout.TextField("Enemy Group Name", _enemyGroupName);
                _enemyName = EditorGUILayout.TextField("Enemy Name", _enemyName);

                EditorGUILayout.Space();
                _isCreateAIScript = EditorGUILayout.Toggle("Create AI Script", _isCreateAIScript);
                _isDropSpoils = EditorGUILayout.Toggle("Drop Spoils", _isDropSpoils);
                _enemyAnimator = EditorGUILayout.ObjectField("Enemy Animator", _enemyAnimator, typeof(Animator), true) as Animator;
                _enemyMaterial = EditorGUILayout.ObjectField("Enemy Material", _enemyMaterial, typeof(Material), true) as Material;
                _creatureUI = EditorGUILayout.ObjectField("Creature UI", _creatureUI, typeof(UICreatureCanvas), true) as UICreatureCanvas;


                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Enemy Information", EditorStyles.boldLabel);

                GUI.enabled = false;
                _enemyInformation = EditorGUILayout.ObjectField("Enemy Information", _enemyInformation, typeof(SO_EntityInformation), false) as SO_EntityInformation;
                _enemyBaseStats = EditorGUILayout.ObjectField("Enemy Base Stats", _enemyBaseStats, typeof(SO_EnemyBaseStats), false) as SO_EnemyBaseStats;
                _creatureDroppedSpoils = EditorGUILayout.ObjectField("Enemy Dropped Spoils", _creatureDroppedSpoils, typeof(SO_DroppedSpoils), false) as SO_DroppedSpoils;
                _enemyPrefab = EditorGUILayout.ObjectField("Enemy Prefab", _enemyPrefab, typeof(GameObject), false) as GameObject;
                GUI.enabled = true;

            }

            // Create Button
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Script")) this.CreateScript();
            if (GUILayout.Button("Create Enemy")) this.CreateEnemy();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Enemy")) this.LoadEnemy();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        private void CreateScript()
        {
            this.CreateFolder();
            if (string.IsNullOrEmpty(_folderPath)) return;

            this.CreateEnemyScript();
            this.CreateEnemyAIScript();
        }

        private void CreateEnemy()
        {
            this.CreateFolder();
            if (string.IsNullOrEmpty(_folderPath)) return;

            this.CreateEnemyInformation();
            this.CreateEnemyBaseStats();
            this.CreateEnemyDropSpoils();
            this.CreateEnemyPrefab();
            this.AssignEnemyScript();
            this.AssignEnemyData();
            this.LoadAnimator();
            this.LoadUI();
        }

        private void LoadEnemy()
        {
            this.CreateFolder();
            if (string.IsNullOrEmpty(_folderPath)) return;

            string informationAssetName = $"{_enemyName} Information";
            string baseStatsAssetName = $"{_enemyName} Base Stats";
            string dropSpoilsAssetName = $"{_enemyName} Dropped Spoils";
            string prefabAssetName = $"{_enemyName}.prefab";

            _enemyInformation = AssetDatabase.LoadAssetAtPath<SO_EntityInformation>($"{_folderPath}/{informationAssetName}.asset");
            _enemyBaseStats = AssetDatabase.LoadAssetAtPath<SO_EnemyBaseStats>($"{_folderPath}/{baseStatsAssetName}.asset");
            _creatureDroppedSpoils = AssetDatabase.LoadAssetAtPath<SO_DroppedSpoils>($"{_folderPath}/{dropSpoilsAssetName}.asset");
            _enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{_folderPath}/{prefabAssetName}");
        }

        private void CreateFolder()
        {
            if (_categoryFolder == null)
            {
                Debug.LogError("Please assign a category folder to save the script.");
                return;
            }

            string baseFolderPath = AssetDatabase.GetAssetPath(_categoryFolder);
            if (!AssetDatabase.IsValidFolder(baseFolderPath))
            {
                Debug.LogError("Invalid category folder path.");
                return;
            }

            string groupName = string.IsNullOrWhiteSpace(_enemyGroupName)
                ? null
                : StringUtils.SanitizeAndCamelCase(_enemyGroupName);

            string enemyName = StringUtils.SanitizeAndCamelCase(_enemyName);

            string finalFolderPath = baseFolderPath;

            // Create subfolder if group name is provided
            if (!string.IsNullOrEmpty(groupName))
            {
                string groupPath = $"{baseFolderPath}/{groupName}";
                if (!AssetDatabase.IsValidFolder(groupPath))
                {
                    AssetDatabase.CreateFolder(baseFolderPath, groupName);
                }

                finalFolderPath = groupPath;
            }

            // Create enemy folder inside final path
            _folderPath = $"{finalFolderPath}/{enemyName}";
            if (!AssetDatabase.IsValidFolder(_folderPath))
            {
                AssetDatabase.CreateFolder(finalFolderPath, enemyName);
                Debug.Log($"Enemy script folder created at: {_folderPath}");
            }

        }

        private void CreateEnemyScript()
        {
            string enemyClassName = StringUtils.SanitizeAndCamelCase(_enemyName);
            string scriptPath = $"{_folderPath}/{enemyClassName}_Enemy.cs";

            if (System.IO.File.Exists(scriptPath))
            {
                Debug.LogWarning($"Script already exists at {scriptPath}");
                return;
            }

            string scriptContent = EnemyScriptTemplate.GetEnemyScript(enemyClassName);
            System.IO.File.WriteAllText(scriptPath, scriptContent);
            AssetDatabase.Refresh();

            Debug.Log($"Enemy script created at: {scriptPath}");
        }

        private void CreateEnemyAIScript()
        {
            if (!_isCreateAIScript) return; // Skip if not creating AI script

            string enemyClassName = StringUtils.SanitizeAndCamelCase(_enemyName);
            string scriptPath = $"{_folderPath}/{enemyClassName}_EnemyAI.cs";

            if (System.IO.File.Exists(scriptPath))
            {
                Debug.LogWarning($"AI Script already exists at {scriptPath}");
                return;
            }

            string scriptContent = EnemyScriptTemplate.GetEnemyAIScript(enemyClassName);
            System.IO.File.WriteAllText(scriptPath, scriptContent);
            AssetDatabase.Refresh();

            Debug.Log($"Enemy AI script created at: {scriptPath}");
        }

        private void CreateEnemyInformation()
        {
            string assetName = $"{_enemyName} Information";
            string assetPath = $"{_folderPath}/{assetName}.asset";

            // Avoid duplicate asset
            ScriptableObject existing = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
            if (existing != null)
            {
                Debug.LogWarning($"Information SO already exists at {assetPath}", existing);
                return;
            }

            // Create SO instance
            _enemyInformation = ScriptableObject.CreateInstance<SO_EntityInformation>();
            AssetDatabase.CreateAsset(_enemyInformation, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Enemy Information SO created at: {assetPath}", _enemyInformation);
        }

        private void CreateEnemyBaseStats()
        {
            string assetName = $"{_enemyName} Base Stats";
            string assetPath = $"{_folderPath}/{assetName}.asset";

            // Avoid duplicate asset
            ScriptableObject existing = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
            if (existing != null)
            {
                Debug.LogWarning($"Base Stats SO already exists at {assetPath}", existing);
                return;
            }

            // Create SO instance
            _enemyBaseStats = ScriptableObject.CreateInstance<SO_EnemyBaseStats>();
            AssetDatabase.CreateAsset(_enemyBaseStats, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Enemy Base Stats SO created at: {assetPath}", _enemyBaseStats);
        }

        private void CreateEnemyDropSpoils()
        {
            if (!_isDropSpoils) return; // Skip if not dropping spoils
            string assetName = $"{_enemyName} Dropped Spoils";
            string assetPath = $"{_folderPath}/{assetName}.asset";

            // Avoid duplicate asset
            ScriptableObject existing = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
            if (existing != null)
            {
                Debug.LogWarning($"Drop Spoils SO already exists at {assetPath}", existing);
                return;
            }

            // Create SO instance
            _creatureDroppedSpoils = ScriptableObject.CreateInstance<SO_DroppedSpoils>();
            AssetDatabase.CreateAsset(_creatureDroppedSpoils, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Enemy Drop Spoils SO created at: {assetPath}", _creatureDroppedSpoils);
        }

        private void CreateEnemyPrefab()
        {
            if (_prefabTemplate == null)
            {
                Debug.LogWarning("No prefab template assigned.");
                return;
            }

            string prefabName = $"{_enemyName}.prefab";
            string prefabPath = $"{_folderPath}/{prefabName}";

            // Avoid overwriting existing prefab
            if (System.IO.File.Exists(prefabPath))
            {
                Debug.LogWarning($"Prefab already exists at {prefabPath}");
                return;
            }

            // Create a deep copy of the prefab template (NOT a prefab instance)
            GameObject prefabCopy = GameObject.Instantiate(_prefabTemplate);
            prefabCopy.name = _enemyName; // optional: rename it

            // Save as new prefab (not a variant)
            _enemyPrefab = PrefabUtility.SaveAsPrefabAsset(prefabCopy, prefabPath);
            GameObject.DestroyImmediate(prefabCopy);

            if (_enemyPrefab != null) Debug.Log($"Enemy prefab created at: {prefabPath}", _enemyPrefab);
            else Debug.LogError("Failed to save prefab asset.");
        }

        private void AssignEnemyScript()
        {
            string enemyClassName = StringUtils.SanitizeAndCamelCase(_enemyName);
            string scriptPath = $"{_folderPath}/{enemyClassName}_Enemy.cs";
            string scriptAIPath = $"{_folderPath}/{enemyClassName}_EnemyAI.cs";
            string prefabPath = $"{_folderPath}/{_enemyName}.prefab";

            // Load the prefab
            GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
            if (prefabInstance == null)
            {
                Debug.LogError("Failed to load prefab contents.");
                return;
            }

            // Assign main script
            ScriptUtils.AssignScriptToPrefab(prefabInstance, scriptPath);

            // Assign AI script
            if (_isCreateAIScript)
                ScriptUtils.AssignScriptToPrefab(prefabInstance, scriptAIPath);

            // Save and unload the prefab
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabInstance);

            Debug.Log("Assigned enemy and AI scripts to prefab.");
        }

        private void AssignEnemyData()
        {
            if (_enemyPrefab == null)
            {
                Debug.LogWarning("Enemy prefab is not assigned.");
                return;
            }

            string prefabPath = AssetDatabase.GetAssetPath(_enemyPrefab);
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
            if (prefabRoot == null)
            {
                Debug.LogError("Failed to load prefab contents.");
                return;
            }

            if (!prefabRoot.TryGetComponent(out Enemy enemy))
            {
                Debug.LogError("Enemy component not found on prefab root.");
                PrefabUtility.UnloadPrefabContents(prefabRoot);
                return;
            }

            try
            {
                // Assign _information
                FieldInfo infoField = typeof(Enemy).GetField("_information", BindingFlags.NonPublic | BindingFlags.Instance);
                if (infoField != null)
                {
                    infoField.SetValue(enemy, _enemyInformation);
                }
                else Debug.LogWarning("Field _information not found in Enemy class.");
               
                // Assign _baseStats inside enemy.Stats
                if (enemy.Stats != null)
                {
                    FieldInfo baseStatsField = enemy.Stats.GetType().GetField("_baseStats", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (baseStatsField != null)
                    {
                        baseStatsField.SetValue(enemy.Stats, _enemyBaseStats);
                    }
                    else Debug.LogWarning("Field _baseStats not found in Enemy.Stats.");
                }
                else Debug.LogWarning("Enemy.Stats is null.");
                

                // Assign _droppedSpoils inside enemy.Spoils
                if (enemy.Spoils != null)
                {
                    FieldInfo spoilsField = enemy.Spoils.GetType().GetField("_droppedSpoils", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (spoilsField != null)
                    {
                        spoilsField.SetValue(enemy.Spoils, _creatureDroppedSpoils);
                    }
                    else Debug.LogWarning("Field _droppedSpoils not found in Enemy.Spoils.");
                }
                else Debug.LogWarning("Enemy.Spoils is null.");
                
                Debug.Log("Successfully assigned ScriptableObjects to Enemy data.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error while assigning enemy data via reflection: {ex.Message}\n{ex.StackTrace}");
            }

            PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }


        private void LoadAnimator()
        {
            if (_enemyPrefab == null || _enemyAnimator == null)
            {
                Debug.LogWarning("Enemy prefab or animator is not assigned.");
                return;
            }

            string prefabPath = AssetDatabase.GetAssetPath(_enemyPrefab);
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
            if (prefabRoot == null)
            {
                Debug.LogError("Failed to load prefab contents.");
                return;
            }

            bool hasAnimatorChild = false;
            foreach (Transform child in prefabRoot.transform)
            {
                if (PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject) == _enemyAnimator.gameObject)
                {
                    hasAnimatorChild = true;
                    break;
                }
            }

            if (!hasAnimatorChild)
            {
                Enemy enemyComponent = prefabRoot.GetComponent<Enemy>();
                if (enemyComponent == null)
                {
                    Debug.LogError("Enemy component not found on prefab root.");
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                    return;
                }

                GameObject animatorInstance = (GameObject)PrefabUtility.InstantiatePrefab(_enemyAnimator.gameObject, enemyComponent.View.transform);
                animatorInstance.name = "Animator";
                animatorInstance.transform.rotation = Quaternion.Euler(0, 90f, 0);
                SkinnedMeshRenderer renderer = animatorInstance.GetComponentInChildren<SkinnedMeshRenderer>();
                if (renderer != null && _enemyMaterial != null)
                {
                    renderer.sharedMaterial = _enemyMaterial; // Assign material if provided
                }
                animatorInstance.AddComponent<EnemyAnimationEventReceiver>();
                animatorInstance.AddComponent<EntityRootMotionReceiver>();
                animatorInstance.AddComponent<SortingGroup>();

                Debug.Log("Animator prefab instance added to enemy prefab.");
            }
            else
            {
                Debug.Log("Animator prefab already assigned to enemy prefab.");
            }

            PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        private void LoadUI()
        {
            if (_enemyPrefab == null || _creatureUI == null)
            {
                Debug.LogWarning("Enemy prefab or UI is not assigned.");
                return;
            }

            string prefabPath = AssetDatabase.GetAssetPath(_enemyPrefab);
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
            if (prefabRoot == null)
            {
                Debug.LogError("Failed to load prefab contents.");
                return;
            }

            bool hasUIChild = false;
            foreach (Transform child in prefabRoot.GetComponentsInChildren<Transform>(true))
            {
                if (PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject) == _creatureUI.gameObject)
                {
                    hasUIChild = true;
                    break;
                }
            }

            if (!hasUIChild)
            {
                Enemy enemyComponent = prefabRoot.GetComponent<Enemy>();
                if (enemyComponent == null)
                {
                    Debug.LogError("Enemy component not found on prefab root.");
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                    return;
                }

                GameObject uiInstance = (GameObject)PrefabUtility.InstantiatePrefab(_creatureUI.gameObject, enemyComponent.UI.transform);
                uiInstance.name = "UI";
                uiInstance.transform.localPosition = new Vector3(0f, 2.5f, 0f);

                Debug.Log("UI prefab instance added to enemy prefab.");
            }
            else
            {
                Debug.Log("UI prefab already assigned to enemy prefab.");
            }

            PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }
    }
}
