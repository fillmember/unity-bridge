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
        public float octreeInitialNodeSize = 50;
        public float visibleDistance = 25;
        public float manageGameObjectInterval = 0.5f;
        private PointOctree<ItemData> itemTree;
        private Dictionary<string, ItemData> itemDictionary = new Dictionary<string, ItemData>();
        private IEnumerator coroutine;

        void Start()
        {
            itemTree = new PointOctree<ItemData>(octreeInitialNodeSize, Camera.main.transform.position, 1);
            UnityBridgeManager.Watch(
                this.system.GetEventName( this.system.listEventTemplate ),
                transform,
                "OnData"
            );
        }

        void OnDestroy() {
            UnityBridgeManager.Unwatch(
                this.system.GetEventName( this.system.listEventTemplate ),
                transform,
                "OnData"
            );
        }

        public void UpdateData(string action, string key, string psr) {
            bool isItemInDictionary = itemDictionary.ContainsKey(key);
            if (action == "child_added" || action == "child_changed") {
                if (isItemInDictionary) {
                    // Set existing data entry
                    ItemData existingItem;
                    bool exists = itemDictionary.TryGetValue(key, out existingItem);
                    // re-add the item again into the tree, as it is not possible to move it directly to another position
                    itemTree.Remove(existingItem);
                    existingItem.CalculatePSR( psr );
                    itemTree.Add(existingItem, existingItem.Position);
                    // set existing item game object, because we want nearby data update to be immediate
                    if (existingItem.DistanceTo(Camera.main.transform) <= visibleDistance) {
                        SetChild( existingItem );
                    }
                } else {
                    // Add data entry
                    var item = new ItemData(key, psr);
                    itemTree.Add(item, item.Position);
                    itemDictionary.Add(item.Key, item);
                    // create object if it is nearby
                    if (item.DistanceTo(Camera.main.transform) <= visibleDistance) {
                        CreateChild( item );
                    }
                }
            } else if (action == "child_removed") {
                if (isItemInDictionary) {
                    var existingItem = itemDictionary[key];
                    itemTree.Remove(existingItem);
                    itemDictionary.Remove(key);
                    RemoveChild( existingItem );
                }
            }
            // Start Proximity Check If not yet
            if (coroutine == null) {
                coroutine = CoroutineSpawnDespawnChildren(manageGameObjectInterval);
                StartCoroutine(coroutine);
            }
        }

        public void OnData(object input) {
            if (input is string) {
                Hashtable parsedJson = (Hashtable)Procurios.Public.JSON.JsonDecode((string)input);
                string action = (string)parsedJson["action"];
                Hashtable data = (Hashtable)parsedJson["data"];
                string key = (string)data["key"];
                string psr = (string)data["value"];
                UpdateData(action, key, psr);
            } else if (input is Dictionary<string, string>) {
                Dictionary<string,string> dict = (Dictionary<string,string>) input;
                UpdateData(dict["action"],dict["key"],dict["value"]);
            }
        }

        private IEnumerator CoroutineSpawnDespawnChildren(float interval) {
            while(true) {
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
