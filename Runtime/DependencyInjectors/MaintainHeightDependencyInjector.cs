using UnityEngine;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.helperInterfaces;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class MaintainHeightDependencyInjector : MonoBehaviour
    {
        [SerializeField] GameObject sensor;
        [SerializeField] float maxStepHeight = 0.3f;

        private void Awake()
        {
            MaintainHeight maintainHeight = GetComponent<MaintainHeight>();
            maintainHeight.sensor = sensor.GetComponent<IPointSensor>();
            maintainHeight.maxStepHeight = maxStepHeight;
        }
    }
}
