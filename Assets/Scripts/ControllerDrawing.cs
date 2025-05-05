using UnityEngine;

public class ControllerDrawing : MonoBehaviour
{
    public LineRenderer line;
    [SerializeField]public float minDistance = 0.00001f;
    private Vector3 previousPosition;
    private bool controllerActive = false;
    private LineRenderer currentLine;
    

    void Update()
    {
        float triggerRight = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);

        if(triggerRight > 0.9f)
        { 
            Vector3 currentControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            if (!controllerActive)
            {
                controllerActive = true;
                currentLine = Instantiate(line, currentControllerPosition, Quaternion.identity, this.transform);
                currentLine.SetPosition(0, currentControllerPosition);
            }
            if(Vector3.Distance(previousPosition, currentControllerPosition) > 0.01)
            {
                currentLine.positionCount++;
                currentLine.SetPosition(currentLine.positionCount -1, currentControllerPosition);
                previousPosition = currentControllerPosition;
            }
        }
        if(triggerRight < 0.2f)
        {
            controllerActive = false;
        }
    }
}
