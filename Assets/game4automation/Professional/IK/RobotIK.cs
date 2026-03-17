// realvirtual.io (formerly game4automation) (R) a Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://realvirtual.io/unternehmen/lizenz  

using System;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;

namespace game4automation
{
    [ExecuteInEditMode]
    public class RobotIK : Game4AutomationBehavior
    {
        [ReadOnly] public int Solution;
        [HideInInspector] [ReadOnly] public bool TurnCorrection;
        [ReadOnly] public string[] Solutions = new string[8];
        [HideInInspector] [OnValueChanged("MoveEditMode")]public bool MoveInEditMode = false;
        [HideInInspector] public bool FollowTarget;
        [HideInInspector] [ShowIf("FollowTarget")] public IKTarget Target;
        [HideInInspector] [ShowIf("FollowTarget")] [ReadOnly] public Vector3 TargetPosRightHanded;
        [HideInInspector] [ShowIf("FollowTarget")] [ReadOnly] public Vector3 TargetRotRightHanded;
        [Header("Configuration")] public bool ElbowInUnityX = false;
        [ReorderableList] public Drive[] Axis;
        public GameObject TCP;  
        public IKTarget HomePosOnStart; 
        [Foldout("Kinematic Parameters")]
        [ReadOnly] public Vector3 ToolOffset;
        [Foldout("Kinematic Parameters")][ReadOnly] public double a1;
        [Foldout("Kinematic Parameters")][ReadOnly] public double a2, b, c1, c2, c3, c4;
        [SerializeField] [HideInInspector] private Vector3[] Axis0Pos = new Vector3[6];
        [SerializeField] [HideInInspector] private Quaternion[] Axis0Rot = new Quaternion[6];
        private float[,] iksolution = new float[8, 6];
        private bool[] notreachable = new bool[8];
        private bool editormovemode = false;
        private IKCalculator ikcalculator;

        public Vector3 GetTCPPosGlobal()
        {
            if (TCP != null)
                return TCP.transform.position;
            else
            {
                Debug.LogError("No TCP defined - please define a TCP");
                return Axis[5].transform.position;
            }
             
        }
        
        public Quaternion GetTCPRotGlobal()
        {
            if (TCP != null)
                 return TCP.transform.rotation;
            else
            {
                Debug.LogError("No TCP defined - please define a TCP");
                return Axis[5].transform.rotation;
            }
        }
        
        private Vector3 AxisPos(int axis)
        {
            var global = Axis[axis].transform.position;
            return this.transform.InverseTransformPoint(global);
        }

        private Vector3 AxisPosRH(int axis)
        {
            var global = Axis[axis].transform.position;
            var local = this.transform.InverseTransformPoint(global);
            return new Vector3(local.x, -local.y, local.z);
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            IKEditor.OnStartPlaymode += IKEditorOnOnStartPlaymode;
#endif
   
        }
        
        private void OnDisable()
        {
#if UNITY_EDITOR
            IKEditor.OnStartPlaymode -= IKEditorOnOnStartPlaymode;
#endif
        }

        private void IKEditorOnOnStartPlaymode()
        {
            if (MoveInEditMode)
            {
                MoveInEditMode = false;
                editormovemode = false;
            }
            MoveEditMode();
        }
        
        new void Awake()
        {
            Set0Pos();
            CheckSettings();
            Solutions = new string[8];
            base.Awake();
            if (Axis!=null)
                foreach (var axis in Axis)
                {
                    if (Axis != null)
                    {
                        axis.TargetStartMove = false;
                        axis.TargetPosition = 0;
                        axis.SmoothAcceleration = false;
                    }
                }

         
        }

        private void Start()
        {
            if (HomePosOnStart != null)
            {
                Invoke("ToHomePos",0.05f);
            }
        }

        public void TargetMoveEditMode(bool active)
        {
            if (active)
            {
                if (editormovemode == false)
                {
                    MoveInEditMode = true;
                    MoveEditMode();
                }
                
            } else
            {
                if (editormovemode == true) 
                {
                    MoveInEditMode = false;
                    MoveEditMode();
                }
            }
        }

        public void MoveEditMode()
        {
            if (MoveInEditMode)
            {
              
                editormovemode = true;
                foreach (var axis in Axis)
                {
                    axis.StartEditorMoveMode();
                }
            }

            else
            {
                editormovemode = false;
                foreach (var axis in Axis)
                {
                    axis.EndEditorMoveMode();
                }
                Set0Pos();
            }
        }

        
        [Button("To 0 Pos")]
        private void Set0Pos()
        {
            if (Axis == null)
                return;
            for (int i = 0; i < Axis.Length; i++)
            {
                if (Axis[i] != null)
                {
                    Axis[i].transform.localPosition = Axis0Pos[i];
                    Axis[i].transform.localRotation = Axis0Rot[i];
                }
            }
        }
        
        public void ToHomePos()
        {
            if (HomePosOnStart == null)
                return;
            if (Application.isPlaying)
                    PositionRobotGlobal(HomePosOnStart.transform.position, HomePosOnStart.transform.rotation, HomePosOnStart.Solution);
        }
        
        public void SetTCP(GameObject tcp)
        {
            TCP = tcp;
            if (tcp != null)
            {
                var PosStandardTCP = Axis[5].transform.position;
                var globaltooloffset = TCP.transform.position - PosStandardTCP;
                // transfer tooloffset into robot coordinates
                ToolOffset = TCP.transform.InverseTransformDirection(globaltooloffset);
            }
            else
            {
                ToolOffset = Vector3.zero;
            }
        }

 
        [Button("Calc Kinematic Parameters")]
        private void CalcCinematicParameters()
        {
            CreateIKCalculator();
            ikcalculator.CalcCinematicParameters();
            a1 = ikcalculator.alpha1;
            a2 = ikcalculator.alpha2;
            b =  ikcalculator.b;
            c1 = ikcalculator.c1;
            c2 = ikcalculator.c2;
            c3 = ikcalculator.c3;
            c4 = ikcalculator.c4;
            
            if (Application.isPlaying)
            {
                Warning("Kinematic Parameters needs to be calculated in Editor Mode",this);
            }

            CheckSettings();
            
            SetTCP(TCP);
            
            for (int i = 0; i < 6; i++)
            {
                Axis0Pos[i] = Axis[i].transform.localPosition;
                Axis0Rot[i] = Axis[i].transform.localRotation;
            }

        }

        private void PositionDrives()
        {
            if (ikcalculator.notreachable[Solution])
                return;
            for (int i = 0; i < 6; i++)
            {
                var pos = iksolution[Solution, i];
                if (float.IsNaN(pos)) 
                    Error("Is not a number");
            }
            if (Application.isPlaying)
            {
                Axis[0].CurrentPosition = iksolution[Solution, 0];
                Axis[1].CurrentPosition = iksolution[Solution, 1];
                Axis[2].CurrentPosition = iksolution[Solution, 2];
                Axis[3].CurrentPosition = iksolution[Solution, 3];
                Axis[4].CurrentPosition = iksolution[Solution, 4];
                Axis[5].CurrentPosition = iksolution[Solution, 5];
            }
            else
            {
                Axis[0].SetPositionEditorMoveMode( iksolution[Solution, 0]);
                Axis[1].SetPositionEditorMoveMode( iksolution[Solution, 1]);
                Axis[2].SetPositionEditorMoveMode( iksolution[Solution, 2]);
                Axis[3].SetPositionEditorMoveMode( iksolution[Solution, 3]);
                Axis[4].SetPositionEditorMoveMode( iksolution[Solution, 4]);
                Axis[5].SetPositionEditorMoveMode( iksolution[Solution,5]);
            }
        }

        Quaternion ConvertToRightHanded(Quaternion input)
        {
            return new Quaternion(
                input.x,
                -input.y,
                input.z,
                input.w
            );
        }

        public void SetTargetProperties(IKTarget target)
        {
            target.PosRH = TargetPosRightHanded;
            target.RotRH = TargetRotRightHanded;
            target.Solution = Solution;
            target.Solutions = Solutions;
            target.Reachable = !notreachable[Solution];
            for (int i = 0; i < 6; i++)
            {
                target.AxisPos[i] = iksolution[Solution, i];
            }
        }
        
        private void JumpToTarget()
        {
            if (Target == null)
                return;
            Target.Reachable = false;
            Solution = Target.Solution;
            if (Target != null)
            {
               SolveIK(Target);
            }
            if (notreachable[Solution] != true)
            {
                PositionDrives();
                Target.Reachable = true;
            }
               
        }

        public void SolveIK(IKTarget target)
        {
            var globalpos = target.transform.position;
            var globalrot = target.transform.rotation;
            Quaternion localrot;
            localrot = Quaternion.Inverse(transform.rotation)*globalrot;
            var localpos = this.transform.InverseTransformPoint(globalpos);
            CalcPosition(localpos,localrot);
            SetTargetProperties(target);
        }

        public bool PositionRobotGlobal(Vector3 globalpos, Quaternion globalrot, int Solution)
        { 
            var localrot = Quaternion.Inverse(transform.rotation)*globalrot;
            var localpos = this.transform.InverseTransformPoint(globalpos);
            CalcPosition(localpos,localrot);
            var reachable = !ikcalculator.notreachable[Solution];
            if (reachable)
            {
                this.Solution = Solution;
                PositionDrives();
            }
            else
            {
                Error("Not Reachable");
            }
            return reachable;
        }


        private void CheckSettings()
        {
            if (TCP == null)
                Warning("RobotIK - No TCP defined in Robot ",this );
            if (Axis == null)
            {
                Warning("RobotIK - No robot axis defined",this );
                return;
            }
                
            if (Axis.Length != 6)
                Warning(
                    "RobotIK - Less or more than 6 robot axis defined - IK for robot +  this.name + is only working for 6 Axis robots",this);
            foreach (var axis in Axis)
            {
                if (axis == null)
                {
                    Warning(
                        "RobotIK - Axis is null - please define",this);
                }
            }
        }

        private void CreateIKCalculator()
        {
            ikcalculator = new IKCalculator();
            var i = 0;
            foreach (var drive  in Axis)
            {
                ikcalculator.Axis[i] = drive.gameObject;
                i++;
            }
            ikcalculator.ElbowInUnityX = ElbowInUnityX;
            ikcalculator.ToolOffset = ToolOffset;
            ikcalculator.RobotRoot = this.gameObject;
            ikcalculator.alpha1 = a1;
            ikcalculator.alpha2 = a2;
            ikcalculator.b = b;
            ikcalculator.c1 = c1;
            ikcalculator.c2 = c2;
            ikcalculator.c3 = c3;
            ikcalculator.c4 = c4;
          
         
        }
        
        private void CalcPosition(Vector3 position, Quaternion rotation) // In Unity Position and rotation
        {
            if (ikcalculator == null)
                CreateIKCalculator();

            ikcalculator.ToolOffset = ToolOffset;
            ikcalculator.CalcPosition(position,rotation);

            iksolution = ikcalculator.iksolution;
            
            // Now Check Drive Limits
            var numdrive = 0;
            foreach (var axis in Axis)
            {
                // all solutions
                if (axis.UseLimits)
                {
                
                    for (int i = 0; i < 8; i++)
                    { 
                        var drivepos = ikcalculator.iksolution[i, numdrive];
                        if (float.IsNaN(drivepos))
                            ikcalculator.notreachable[i] = true;
                        else
                        {
                            if (!(drivepos >= axis.LowerLimit && drivepos <= axis.UpperLimit))
                                ikcalculator.notreachable[i] = true;
                        }
                    }
                }
                numdrive++;
            }

            for (int i = 0; i < 8; i++)
            {
                Solutions[i] = "";
                if (ikcalculator.notreachable[i] == true)
                    Solutions[i] = "Not reachable ";
                //if (notreachable[i]!=true)
                    for (int j = 0; j < 6; j++)
                    {
                        Solutions[i] = Solutions[i] + ikcalculator.iksolution[i, j].ToString("0.0");
                        if (j < 5)
                            Solutions[i] = Solutions[i] + " / ";
                    }
            }

            notreachable = ikcalculator.notreachable;
        }

        private void Update()
        {
            
            if (!Application.isPlaying)
            {
                if (Target != null && FollowTarget)
                    JumpToTarget();
                if (FollowTarget) // Target still active
                {   
#if UNITY_EDITOR
                    if (Selection.activeGameObject != null)
                    {
                     
                        var tar = Selection.activeGameObject.GetComponent<IKTarget>();
                        var path = Selection.activeGameObject.GetComponent<IKPath>();
                        if (path == null && tar == null && Target != null && MoveInEditMode == true) 
                        {
                            MoveInEditMode = false;
                            MoveEditMode();
                        }
                        
                    }
#endif  
                }
            }
        }

    }
}