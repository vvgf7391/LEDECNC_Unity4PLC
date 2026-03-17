// realvirtual.io (formerly game4automation) (R) a Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://realvirtual.io/unternehmen/lizenz  

using System.Collections.Generic;
using UnityEngine;
using game4automation;
using NaughtyAttributes;
using UnityEditor;

public class IKPath : BehaviorInterface
{
    // Start is called before the first frame update
   
    [Header ("Path Parameters")]
    [Range(0,1)] public float SpeedOverride = 1;
    public bool SetNewTCP    ;
    [ShowIf("SetNewTCP")] public GameObject TCP;
    public bool DrawPath = true;
    public bool DrawTargets = true;
   
    [Header ("Targets")]
    [ReorderableList] public List<IKTarget> Path = new List<IKTarget>();
    [Header("Start Conditions")] 
    public PLCOutputBool SignalStart;
    public bool StartPath;
    public bool LoopPath;
    [Header("On Path End")] 
    public PLCInputBool SignalIsStarted;
    public PLCInputBool SignalEnded;
    public IKPath StartNextPath;
    [Header ("Status")]
    [game4automation.ReadOnly] public bool PathIsActive = false;
    [game4automation.ReadOnly] public bool LinearPathActive = false;
    [game4automation.ReadOnly] public float LinearPathPos = 0;
    [game4automation.ReadOnly] public IKTarget CurrentTarget;
    [game4automation.ReadOnly] public IKTarget LastTarget;
    [game4automation.ReadOnly] public int NumTarget;
    [game4automation.ReadOnly] public bool WaitForSignal;
    [game4automation.ReadOnly] public RobotIK RobotIK;
    private List<Drive> drivesatposition = new List<Drive>();

    private float linearpathspeed = 0;
    private float linearpathacceleration = 0;
    private bool linearacceleration = false;
    private bool lineardeceleration = false;
    
    private float LinearPathStartTime = 0;
    private Vector3 PositionOnPath;
    private Vector3 LinearPathStartPos;
    private Quaternion LinearPathStartRot;
    private bool startbefore = false;
    private bool signalendednotnull, signalisstartednotnull, signalstartnotnull;
    private PLCOutputBool waitforsignal;
    private bool waitforstart;
   
    [Button("Start Path")]
    void startPath()
    {
        if (PathIsActive)
        {
            Debug.Log("Start not possible because path is currently active!");
            return;
        }
        
        if (SetNewTCP)
            RobotIK.SetTCP(TCP);
        
        if (SetRobotIK())
        {
            RobotIK.FollowTarget = false;
            drivesatposition.Clear();
            NumTarget = 0;
            PathIsActive = true;
            if (signalisstartednotnull)
                SignalIsStarted.Value = true;
            if (signalendednotnull)
                SignalEnded.Value = false;
            CheckNextTarget();
        }
    }

    void StartDrivePTP(IKTarget target)
    {
        // Get All Times
        // Check max Times 
        float maxtime = 0;
        var i = 0;
        var maxdrive = 0;
        if (target.InterpolationToTarget == IKTarget.Interploation.PointToPoint)
        {
            foreach (var drive in RobotIK.Axis)
            {
                drive.SpeedOverride = SpeedOverride * target.SpeedToTarget;
                var driveteime = drive.GetTimeTo(target.AxisPos[i]);
                if (driveteime > maxtime)
                {
                    maxtime = driveteime;
                    maxdrive = i;
                }

                i++;
            }

            // Start Drives
            i = 0;
            foreach (var drive in RobotIK.Axis)
            {
                drive.OnAtPosition += DriveOnOnAtPosition;
                if (i != maxdrive)
                    drive.DriveTo(target.AxisPos[i], maxtime);
                else
                    drive.DriveTo(target.AxisPos[i]);
                i++;
            }
        }

        if (target.InterpolationToTarget == IKTarget.Interploation.PointToPointUnsynced)
        {
            // Start Drives
            i = 0;
            foreach (var drive in RobotIK.Axis)
            {
                drive.OnAtPosition += DriveOnOnAtPosition;
                drive.DriveTo(target.AxisPos[i]);
                i++;
            }
        }
    }

    public void OnTargetDelete(IKTarget target)
    {
        Path.Remove(target);
    }
    
    void StartDriveLinear(IKTarget target)
    {
        LinearPathStartTime = Time.fixedTime;
        LinearPathStartPos = RobotIK.GetTCPPosGlobal();
        LinearPathStartRot = RobotIK.GetTCPRotGlobal();
        PositionOnPath = LinearPathStartPos;
        linearpathspeed = 0;
        linearpathacceleration = 0;
        linearacceleration = false;
        lineardeceleration = false;
        LinearPathActive = true;
        LinearPathPos = 0;
    }
    
   public void DriveToTarget(IKTarget target)
    {
        if (LastTarget != null)
            LastTarget.OnLeaveTarget();
        LastTarget = null;
        RobotIK.SolveIK(target);
        CurrentTarget = target;
        if (target.Reachable)
        {
            if (target.InterpolationToTarget == IKTarget.Interploation.PointToPoint)
                StartDrivePTP(target);
            if (target.InterpolationToTarget == IKTarget.Interploation.PointToPointUnsynced)
                StartDrivePTP(target);
            if (target.InterpolationToTarget == IKTarget.Interploation.Linear)
                StartDriveLinear(target);
        }
        else
        {
            Error("Target " + target + "is not reachable");
        }
    }

    private void CheckNextTarget()
    {
        if (!PathIsActive)
            return;
        if (NumTarget < Path.Count)
        {
            DriveToTarget( Path[NumTarget]);
        }
        else
        {
            // Path Ended
            CurrentTarget = null;
            NumTarget = 0;
            PathIsActive = false;
            if (signalendednotnull)
                SignalEnded.Value = true;
            if (signalisstartednotnull)
                SignalIsStarted.Value = false;
            if (StartNextPath != null)
                StartNextPath.startPath();
            else
                if (LoopPath)
                    startPath();
        }
        
    }

    private void AtTarget()
    {
        NumTarget++;
        // Reset all Values for all motion types
        drivesatposition.Clear();
        linearpathspeed = 0;
        linearpathacceleration = 0;
        linearacceleration = false;
        lineardeceleration = false;
        LinearPathActive = false;
        LinearPathPos = 0;
        
        CurrentTarget.OnAtTarget();

        if (CurrentTarget.WaitForSignal != null)
        {
            waitforsignal = CurrentTarget.WaitForSignal;
            WaitForSignal = true;
        }
        else
        {
            ReadyForCheckNextTarget();
        }
    }

    private void ReadyForCheckNextTarget()
    {
        WaitForSignal = false;
        Invoke("CheckNextTarget",CurrentTarget.WaitForSeconds);
        LastTarget = CurrentTarget;
        CurrentTarget = null;
    }
    private void DriveOnOnAtPosition(Drive drive)
    {
        drive.OnAtPosition -= DriveOnOnAtPosition;
        drive.SpeedOverride = 1;
        drivesatposition.Add(drive);
        if (drivesatposition.Count == 6)
        {
           AtTarget();
        }
    }

    void Reset()
    {
        CurrentTarget = null;
        NumTarget = 0;
    }
  
    // Update is called once per frame
    new void Awake()
    {
        NumTarget = 0;
        PathIsActive = false;
        CurrentTarget = null;
        signalendednotnull = SignalEnded != null;
        signalisstartednotnull = SignalIsStarted != null;
        signalstartnotnull = SignalStart != null;
        base.Awake();
        waitforstart = true;
    }

    void Start()
    {
        Invoke("EndWaitForStart",0.1f);
    }

    void EndWaitForStart()
    {
        waitforstart = false;
    }
    
    private void PositionOnLinearPath()
    {
        // Calculate Path Position
        // Slow down needed
        bool slowdownneeded = false;
        var vectortoend = PositionOnPath-CurrentTarget.transform.position;
        var distancetoend = vectortoend.magnitude * Game4AutomationController.Scale;
        var availslowdowntime = Mathf.Sqrt(2 * distancetoend / CurrentTarget.LinearAcceleration);
        var needslowdowntime =linearpathspeed*SpeedOverride/ CurrentTarget.LinearAcceleration; 
        if (needslowdowntime >= availslowdowntime)
            slowdownneeded = true;
        
        // Accelereation needed
        if (!linearacceleration && !lineardeceleration && !slowdownneeded && linearpathspeed <= CurrentTarget.LinearSpeedToTarget*SpeedOverride)
        {
            linearpathacceleration = CurrentTarget.LinearAcceleration;
            linearacceleration = true;
        }
        
        // Deceleration needed
        if (slowdownneeded && !lineardeceleration)
        {
            linearpathacceleration = -CurrentTarget.LinearAcceleration;
            lineardeceleration = true;
        }
        
        // Limit Speed
        if (!lineardeceleration && linearpathspeed > CurrentTarget.LinearSpeedToTarget*SpeedOverride)
        {
            linearpathacceleration = 0;
            linearpathspeed = CurrentTarget.LinearSpeedToTarget*SpeedOverride;
        }
         
            
        // Calculate Speed
        linearpathspeed = linearpathspeed + linearpathacceleration * Time.fixedDeltaTime;
        
        // Callculate new position
        LinearPathPos = LinearPathPos + Time.fixedDeltaTime * linearpathspeed*SpeedOverride;
        
        
        
        var path = CurrentTarget.transform.position-LinearPathStartPos;
        var pathpercent = (LinearPathPos/Game4AutomationController.Scale)/path.magnitude;

        var endpath = false;
        
        // At destination
        if (lineardeceleration && pathpercent >= 1 || lineardeceleration && linearpathspeed < 0)
        {
            pathpercent = 1;
            endpath = true;
        }
        
        PositionOnPath = LinearPathStartPos + path * pathpercent;
        var rotation = Quaternion.Lerp(LinearPathStartRot, CurrentTarget.transform.rotation, pathpercent);

        var ispossible = RobotIK.PositionRobotGlobal(PositionOnPath, rotation, CurrentTarget.Solution);

        if (!ispossible)
        {
            Error("Solution on the linear path [" + this.name + "] to Target [" + CurrentTarget.name + "]is not possible");
        }
        if (endpath)
        {
            RobotIK.PositionRobotGlobal(CurrentTarget.transform.position, rotation, CurrentTarget.Solution);
            AtTarget();
        }
           
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
    
    [Button("Add new Target to Path")]
    public void AddTargetToPath()
    {
        var newgo = new GameObject();
        newgo.transform.parent = this.transform;
        // Same Position and Rotation as last Target
        if (Path != null && Path.Count > 0)
        {
            var last = Path[Path.Count - 1];
            newgo.transform.position = last.transform.position;
            newgo.transform.rotation = last.transform.rotation;
        }
        else
        {
            if (SetRobotIK())
            {
                newgo.transform.position = RobotIK.GetTCPPosGlobal();
                newgo.transform.rotation = RobotIK.GetTCPRotGlobal();
            }
        }
        newgo.name = "Target" + Path.Count;
        var target = newgo.AddComponent<IKTarget>();
        Path.Add(target);
#if UNITY_EDITOR
        Selection.activeGameObject = newgo;
#endif
    }
    
    private void FixedUpdate()
    {
        if (WaitForSignal)
            if (waitforsignal.Value == true)
                ReadyForCheckNextTarget();
        
        if (signalstartnotnull)
            StartPath = SignalStart.Value;
        
        if (!startbefore && StartPath && !PathIsActive && !waitforstart)
            startPath();
        
        if (LinearPathActive)
            PositionOnLinearPath();
        if (!waitforstart)
           startbefore = StartPath;
    }
}
