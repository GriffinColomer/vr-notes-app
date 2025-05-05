using UnityEngine;

public class SimpleInteraction : MonoBehaviour
{
    public Rigidbody Ball;
    void Update()
    {
        float triggerLeft = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
        float triggerRight = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);

        if(triggerRight>0.9f)
        {
            Instantiate(Ball, OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), Quaternion.identity);
        }
    } 
}
