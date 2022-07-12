using UnityEditor;
using UnityEngine;

namespace EnemyLogic.Camera.Rotating
{
    [CustomEditor(typeof(RotatingAmplitudeCamera))]
    public class RotatingAmplitudeCameraEditor : Editor
    {
        private float _angle = 360;

        private void OnSceneGUI()
        {
            var fow = (RotatingAmplitudeCamera)target;
            
            Handles.color = Color.green;
            
            var fowTransform = fow.transform;
            var fowPosition = fowTransform.position;
            
            Handles.DrawWireArc(fowPosition, fowTransform.up, Vector3.right, _angle, fow.ViewRadius);
            
            var viewAngleA = fow.GetDirectionFromAngle(-fow.RotationAplitude / 2, false);
            var viewAngleB = fow.GetDirectionFromAngle(fow.RotationAplitude / 2, false);

            var camPos = fow._currentCamera.transform.position;
            
            Handles.DrawLine(camPos, fowPosition + viewAngleA * fow.ViewRadius);
            Handles.DrawLine(camPos, fowPosition + viewAngleB * fow.ViewRadius);
        }
    }
}