using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConveyorSystem
{
    public class ConveyorController : MonoBehaviour, IConveyorItemGate
    {
        #region Temp
        [Header("Temporary Things", order = 0)]
        public ItemAsset testItem;
        public string consecutiveFactoryOrConveyorName;
        #endregion

        #region Fields
        [Header("Fields", order = 1)]


        private IConveyorItemGate _consecutiveFactoryOrConveyor;

        public ItemTransmission itemTransmission = new ItemTransmission();
        [SerializeField] private float _itemHalfwayLength = 0.6f;

        #endregion

        #region Functions
        public ConveyorController GetConveyor()
        {
            return this;
        }
        public bool CanReceiveItem(ItemAsset itemAsset, float distanceToEnd)
        {
            if (itemTransmission.itemsProgress.Count > 0)
            {
                float distanceBetweenThisLastAndStart = itemTransmission.totalDistance - (itemTransmission.totalDistance - itemTransmission.itemsProgress[itemTransmission.itemsProgress.Count - 1]);

                return distanceBetweenThisLastAndStart + distanceToEnd >= _itemHalfwayLength;
            }
            return true;
        }      
        #endregion



        #region Methods
        void Update()
        {
            itemTransmission.Update();
        }
        public void ReceiveItem(ItemAsset itemAsset, Transform itemTransform)
        {
            itemTransmission.AddItem(itemTransform, itemAsset);
        }
        public void Setup(bool isDirectionReversed, OrientedPoints orientedPoints, IConveyorItemGate consecutiveFactoryOrConveyor, float speed)
        {
            Vector3[] positions = OrientedPoints.PositionsLocalToWorld(orientedPoints.positions, transform);
            itemTransmission.CreatePath(isDirectionReversed, speed, positions, orientedPoints, orientedPoints.totalDistance, _itemHalfwayLength);
            AssignConsecutiveItemGate(consecutiveFactoryOrConveyor);
        }
        public void AssignConsecutiveItemGate(IConveyorItemGate conveyorItemGate)
        {
            _consecutiveFactoryOrConveyor = conveyorItemGate;
            itemTransmission.consecutiveFactoryOrConveyor = conveyorItemGate;
            if (conveyorItemGate != null)
            {
                itemTransmission.SmoothPathTransitionToConsecutive(conveyorItemGate.GetConveyor());
            }
            if (_consecutiveFactoryOrConveyor != null)
            {
                consecutiveFactoryOrConveyorName = conveyorItemGate.ToString();
            }
        }
        [ContextMenu("TestSpawnItem")]
        public void TestSpawnItem()
        {
            GameObject test = Instantiate(testItem.prefab);
            itemTransmission.AddItem(test.transform, testItem);
        }
        [ContextMenu("Test Destroy")]
        public void TestDestroy()
        {

            Destroy(gameObject);
        }
        #endregion
    }
}
