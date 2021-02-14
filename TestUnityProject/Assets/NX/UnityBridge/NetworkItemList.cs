using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NX.UnityBridge;
using NX.UnityBridge.Types;

namespace NX.UnityBridge {

    public class NetworkItemList : MonoBehaviour
    {

        public NetworkSystem system;
        public int octreeInitialNodeSize = 50;
        public int visibleDistance = 25;
        public float manageGameObjectInterval = 0.5f;
        private PointOctree<ItemData> itemTree;
        private Dictionary<string, ItemData> itemDictionary = new Dictionary<string, ItemData>();
        private IEnumerator coroutine;

        void Start()
        {
            itemTree = new PointOctree<ItemData>(octreeInitialNodeSize, Camera.main.transform.position, 1);
            UnityBridgeManager.unityWatch(
                this.system.GetEventName( this.system.listEventTemplate ),
                transform.name,
                "OnData"
            );
        }

        void OnDestroy() {
            UnityBridgeManager.unityUnwatch(
                this.system.GetEventName( this.system.listEventTemplate ),
                transform.name,
                "OnData"
            );
        }

        public void OnData(string payload) {
            // Parse JSON
            Hashtable parsedJson = (Hashtable)Procurios.Public.JSON.JsonDecode(payload);
            string action = (string)parsedJson["action"];
            Hashtable data = (Hashtable)parsedJson["data"];
            string key = (string)data["key"];
            string psr = (string)data["value"];
            bool isItemInDictionary = itemDictionary.ContainsKey(key);
            // Add/Update/Remove item data
            if (!isItemInDictionary && action == "child_added") {
                // Add data entry
                var item = new ItemData(key, psr);
                itemTree.Add(item, item.Position);
                // create object if it is nearby
                if (item.DistanceTo(Camera.main.transform) <= visibleDistance) {
                    CreateChild( item );
                }
            } else if (isItemInDictionary && (action == "child_added") || (action == "child_changed")) {
                // Set existing data entry
                var existingItem = itemDictionary[key];
                // re-add the item again into the tree, as it is not possible to move it directly to another position
                itemTree.Remove(existingItem);
                existingItem.CalculatePSR( psr );
                itemTree.Add(existingItem, existingItem.Position);
                // set existing item game object, because we want nearby data update to be immediate
                if (existingItem.DistanceTo(Camera.main.transform) <= visibleDistance) {
                    SetChild( existingItem );
                }
            } else if (isItemInDictionary && (action == "child_removed")) {
                // Remove existing data entry
                var existingItem = itemDictionary[key];
                if (existingItem == null) {
                    return;
                }
                itemTree.Remove(existingItem);
                itemDictionary.Remove(key);
                // Remove game object if nearby
                if (existingItem.DistanceTo(Camera.main.transform) <= visibleDistance) {
                    RemoveChild( existingItem );
                }
            }
            // Start Proximity Check If not yet
            if (coroutine == null) {
                coroutine = CoroutineSpawnDespawnChildren(manageGameObjectInterval);
                StartCoroutine(coroutine);
            }
        }

        private IEnumerator CoroutineSpawnDespawnChildren(float interval) {
            for(;;) {
                ItemData[] list = itemTree.GetNearby(Camera.main.transform.position, visibleDistance);
                foreach( KeyValuePair<string, ItemData> kv in itemDictionary ) {
                    ItemData item = kv.Value;
                    Transform childTransform = transform.Find(item.Key);
                    bool isNearby = list.Contains(item);
                    if (isNearby && childTransform == null) {
                        CreateChild( item );
                    } else if (isNearby && childTransform != null) {
                        SetChild( item );
                    } else {
                        RemoveChild( item );
                    }
                }
                yield return new WaitForSeconds(interval);
            }
        }

        private void CreateChild( ItemData item ) {
            GameObject child = Instantiate(this.system.itemPrefab, new Vector3(0,0,0), Quaternion.identity, this.transform);
            child.name = item.Key;
            item.SetTransform(child.transform);
        }
        private void SetChild( ItemData item ) {
            Transform childTransform = transform.Find(item.Key);
            if (childTransform) {
                item.SetTransform(childTransform);
            }
        }
        private void RemoveChild( ItemData item ) {
            Transform childTransform = transform.Find(item.Key);
            if (childTransform) {
                Destroy(childTransform.gameObject);
            }
        }
    }

}
