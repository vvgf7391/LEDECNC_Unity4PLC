// realvirtual.io (formerly game4automation) (R) a Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://realvirtual.io/unternehmen/lizenz  

using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace game4automation
{
    [ExecuteInEditMode]
    public class IKTarget : BehaviorInterface
    {
        [System.Serializable]
        public enum Interploation // your custom enumeration
        {
            PointToPoint,
            PointToPointUnsynced,
            Linear
        };

        [OnValueChanged("ChangedFollowInEditMode")]
        public bool FollowInEditMode=true;

        [Header("Right Handed Robot Coordinates")] [game4automation.ReadOnly]
        public Vector3 PosRH;

        [game4automation.ReadOnly] public Vector3 RotRH;

        [Header("Path Parameters")] [Range(0, 1)]
        public float SpeedToTarget = 1;

        public float LinearAcceleration = 100; 
        public Interploation InterpolationToTarget;
        public float LinearSpeedToTarget = 500;
        [Header("On At Target")]      public PLCInputBool SetSignal;
        public float SetSignalDuration = 0.5f;
        public float WaitForSeconds = 0;
        public PLCOutputBool WaitForSignal;
        [Header("Control")] [Range(0, 7)] public int Solution;
        [game4automation.ReadOnly] public string[] Solutions = new string[8];
        [game4automation.ReadOnly] public float[] AxisPos = new float[6];
        [game4automation.ReadOnly] public RobotIK RobotIK;
        [game4automation.ReadOnly] public bool Reachable;


        private bool waitforsignalnotnull;
        private bool setsignalnotnull;

        public void OnAtTarget()
        {
            if (setsignalnotnull)
                SetSignal.Value = true;
        }

        public void OnLeaveTarget()
        {
            if (setsignalnotnull)
                Invoke("ResetSignal",SetSignalDuration);
        }

        private void ResetSignal()
        {
            SetSignal.Value = false;
        }
        new void Awake()
        {
            waitforsignalnotnull = WaitForSignal != null;
            setsignalnotnull = SetSignal != null;
            base.Awake();
        }
        
        private bool SetRobotIK()
        {
            if (RobotIK == null)
                RobotIK = GetComponentInParent<RobotIK>();
            if (RobotIK == null)
            {
                Error("Path needs to be a child of a RobotIK component");
                return false;
            }
            return true;
        }
        
        void ChangedFollowInEditMode()
        {
            if (FollowInEditMode == false)
                RobotIK.TargetMoveEditMode(false);
        }

        private void OnDrawGizmosSelected()
        {
            if (!Reachable)
            {
                Gizmos.color = new Color(1, 0, 0, 0.6f);
                Gizmos.DrawSphere(transform.position, 0.05f);
            }
        }

        [ExecuteInEditMode]
        private void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                var ikpath = GetComponentInParent<IKPath>();
            
                if (ikpath != null)
                {
                    ikpath.OnTargetDelete(this);
                    
                }
            }
        }

        public void SetAsTarget()
        {
           
            if (SetRobotIK())
            {
                RobotIK.Target = this;
                RobotIK.FollowTarget = true;
                RobotIK.TargetMoveEditMode(true);
            }
        }

        [Button("Drive to target")]
        public void DriveTo()
        {
            var ikpath = GetComponentInParent<IKPath>();
            
            if (ikpath != null)
            {
                ikpath.DriveToTarget(this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!Application.isPlaying)
                if (transform.hasChanged)
                    if (FollowInEditMode)
                    {
                        #if UNITY_EDITOR
                        if (Selection.activeGameObject == this.gameObject)
                            SetAsTarget();
                        #endif
                    }
        }
    }
}