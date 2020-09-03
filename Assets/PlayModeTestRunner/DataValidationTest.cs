using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;
using System.Linq;


namespace Tests
{

    public class DataValidationTest
    {
        LoadManager loadManager;

        [UnityTest]
        public IEnumerator LoadGameDataFromJSONTest()
        {
            GameObject loadManagerGO = new GameObject();

            loadManager = loadManagerGO.AddComponent<LoadManager>();

            yield return TestRoutineRunner.Instance.StartCoroutine(loadManager.LoadInGameData());

            Assert.GreaterOrEqual(loadManager.allBuildingData.Count, 1);
            Assert.GreaterOrEqual(loadManager.allEnemyData.Count, 1);
            Assert.GreaterOrEqual(loadManager.allEquipmentData.Count, 1);
            Assert.GreaterOrEqual(loadManager.allQuestData.Count, 1);
            Assert.GreaterOrEqual(loadManager.allResourceData.Count, 1);

        }



        [UnityTest]
        public IEnumerator ResourceRecipeContainCraftResultData()
        {


            bool isContainResult = true;
            foreach (KeyValuePair<string, Resource> keyValuePair in loadManager.allResourceData)
            {
                Resource resource = keyValuePair.Value;

                if (resource.IsRecipe())
                {
                    var result = loadManager.allResourceData.SingleOrDefault(r => r.Value.Name == (resource.Name.Replace("Recipe:", "")));

                    if (result.Equals(default(KeyValuePair<string, Resource>)))
                    {
                        Debug.LogWarning($"Problems on {resource.Name} : {resource.Name.Replace("Recipe:", "")}");
                        isContainResult = false;

                    }
                }


            }

            Assert.IsTrue(isContainResult);
            yield return null;

           
        }
        /*
        [UnityTest]
        public IEnumerator BuildingDataKeyNotDuplicate()
        {
            yield return null;
            Debug.Log($"{loadManager.allBuildingData.Distinct().ToArray().Length} : {Enum.GetNames(typeof(Building.BuildingType)).Length}");
            Assert.IsTrue(loadManager.allBuildingData.Distinct().ToArray().Length == Enum.GetNames(typeof(Building.BuildingType)).Length);
            

        }*/

    }
    
    public class TestRoutineRunner : MonoBehaviour
    {
        // implementation of the singleton pattern
        private static TestRoutineRunner instance;

        // Get the reference to instance or create it lazy when needed
        public static TestRoutineRunner Instance
        {
            get
            {
                // if instance already exists and is set return it right way
                if (instance) return instance;

                // otherwise find it in the scene
                instance = FindObjectOfType<TestRoutineRunner>();

                // if it is found in the scene return it now
                if (instance) return instance;

                // otherwise create a new one
                instance = new GameObject("TestRoutineRunner").AddComponent<TestRoutineRunner>();

                return instance;
            }
        }

        // Use this for adding a callback what should be done when the routine finishes
        public void TestRoutine(IEnumerator routine, Action whenDone)
        {
            StartCoroutine(RunRoutine(routine, whenDone));
        }

        private IEnumerator RunRoutine(IEnumerator routine, Action whenDone)
        {
            // runs the routine an wait until it is finished
            yield return routine;

            // execute the callback
            whenDone?.Invoke();
        }
    }



}
