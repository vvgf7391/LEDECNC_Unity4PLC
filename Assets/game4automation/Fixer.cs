// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  


using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


namespace game4automation
{
    [SelectionBase]
    //! The Fixer is able to fix Mus as Subcomponents to any Gameobject where the Fixer is attached. As soon as the free moving Mus are colliding or as soon as a Gripper is releasing
    //! the MUs the Fixer will fix the MU. MUs fixed by the Fixer have no Gravity and are purely kinematic.
    [ExecuteAlways]
    public class Fixer : BehaviorInterface,IFix
    {
        public bool UseRayCast; //!< Use Raycasts instead of Box Collider for detecting parts
        public List<string> LimitFixToTags; //< Limits all Fixing functions to objects with defined Tags
        [Tooltip("Raycast direction")] [ShowIf("UseRayCast")] public Vector3 RaycastDirection = new Vector3(1, 0, 0); //!< Raycast direction
        [Tooltip("Length of Raycast in mm, Scale is considered")] [ShowIf("UseRayCast")] public float RayCastLength = 100; //!<  Length of Raycast in mm
        [Tooltip("Raycast Layers")] [ShowIf("UseRayCast")] public List<string> RayCastLayers = new List<string>(new string[] {"g4a MU","g4A SensorMU",}); //!< Raycast Layers
        [Tooltip("MU should be fixed")] public bool FixMU; //!< true if MU should be fixed
        [Tooltip("True if MU should be fixed or aligned when Distance between MU and Fixer is minimum (distance is increasing again)")] public bool AlignAndFixOnMinDistance;  //!< true if MU should be fixed or aligned when Distance between MU and Fixer is minimum (distance is increasing again)
        [Tooltip("Releases the fixer if something which is not an MU is colliding with the Fixer")] [ShowIf("FixMU")]public bool ReleaseOnCollissionNonMU = false;  //!< Releases the fixer if something which is not an MU is colliding with the Fixer
        [Tooltip("Align pivot point of MU to Fixer pivot point")] public bool AlignMU; //!< true if pivot Points of MU and Fixer should be aligned
        [Tooltip("Offset to pivot point to align to in local coordinate system")][ShowIf("AlignMU")] public Vector3 DeltaAlign;
        [Tooltip("Display status of Raycast or BoxCollider")] public bool ShowStatus; //! true if Status of Collider or Raycast should be displayed
        [Tooltip("Opacity of Mesh in case of status display")] [ShowIf("ShowStatus")] [HideIf("UseRayCast")] public float StatusOpacity = 0.2f; //! Opacity of Mesh in case of status display
        [Tooltip("Only controlled by Signal FixerFix - with one bit")]public bool OneBitFix; //! Only controlled by Signal FixerFix - with one bit
        [Tooltip("PLCSignal for fixing current MUs and turning Fixer off")]  public PLCOutputBool FixerFix; 
        [Tooltip("PLCSignal for releasing current MUs and turning Fixer off")] [HideIf("OneBitFix")] public PLCOutputBool FixerRelease; //! PLCSignal for releasing current MUs and turning Fixer off
    
        
        private bool nextmunotnull; 
        public List<MU> MUSEntered;
        public List<MU> MUSFixed;
        private MeshRenderer meshrenderer;
        private int layermask;
        private  RaycastHit[] hits;

        private bool lastfix;
        private bool lastfixmu;
        private bool lastrelease;
        private bool signalfixerreleasenotnull;
        private bool signalfixerfixnotnull;
        private bool Deactivated = false;
        
        // Trigger Enter and Exit from Sensor
        public void OnTriggerEnter(Collider other)
        {
            var mu = other.gameObject.GetComponentInParent<MU>();
            if (LimitFixToTags.Count>0)
                if (!LimitFixToTags.Contains(mu.tag))
                {
                    MUSEntered.Remove(mu); 
                    return;
                }

            if (mu != null)
            {
                if (!MUSFixed.Contains(mu) && !MUSEntered.Contains(mu))
                {
                    MUSEntered.Add(mu);
                    mu.FixerLastDistance = -1;
                }
            }
            if (ReleaseOnCollissionNonMU)
                if (mu == null)
                    Release();
        }
        
        public void OnTriggerExit(Collider other)
        {
            var mu = other.gameObject.GetComponentInParent<MU>();
            if (MUSEntered.Contains(mu))
                MUSEntered.Remove(mu);
        }

        
        
         void Reset()
         {
             if (GetComponent<BoxCollider>())
                 UseRayCast = false;
             else
                 UseRayCast = true;
         }
        
        public void Release()
        {
            var mus = MUSFixed.ToArray();
            for (int i = 0; i < mus.Length; i++)
            {
                Unfix(mus[i]);
            } 
        }
        
        public void Unfix(MU mu)
        {
            if (Deactivated)
                return;
            
            var fix = false;
            if (signalfixerfixnotnull && FixerFix.Value == false)
                fix = true;
            
            if (FixMU || fix)
            {
                mu.Unfix();
            }
            MUSFixed.Remove(mu);
            if (ShowStatus && !UseRayCast && meshrenderer != null)
                 meshrenderer.material.color = new Color(1,0,0,StatusOpacity);
        }
        
        public void Fix()
        {
            var mus = MUSEntered.ToArray();
            if (mus.Length == 0)
                return;
            
            for (int i = 0; i < mus.Length; i++)
            {
                Fix(mus[i]);
            } 
        }

        public void DeActivate(bool activate)
        {
            Deactivated = activate;
        }
        
        public void Fix(MU mu)
        {
            if (Deactivated)
                return;
            MUSEntered.Remove(mu);
            if(!MUSFixed.Contains(mu))
                MUSFixed.Add(mu);
            if (AlignMU)
            {
                mu.transform.position = transform.position + transform.TransformDirection(DeltaAlign);
                mu.transform.rotation = transform.rotation;
            }

            var fix = false;
            if (signalfixerfixnotnull && FixerFix.Value == true)
                fix = true;
            
            if (FixMU || fix)
            {
                mu.Fix(this.gameObject);
            }
            else
            {
                Release();
            }
        }

        
        private void AtPosition(MU mu)
        {
            if (Deactivated)
                return;
            if (mu.LastFixedBy == this.gameObject)
            {
                return;
            }
            /// Only fix if another fixer element has fixed it - don't fix it if it is fixed by a gripper
            
            var fixedby = mu.FixedBy;
            Fixer fixedbyfixer = null;
            if (fixedby != null)
                fixedbyfixer = mu.FixedBy.GetComponent<Fixer>();
            
            // Fix only if FixMU or Signal is set
            var fix = false;
            if (signalfixerfixnotnull && FixerFix.Value)
                fix = true;
            if (FixMU)
                fix = true;
            
            if (mu.FixedBy == null || (fixedbyfixer != null && fixedbyfixer != this))
            {
                if (ShowStatus && !UseRayCast && meshrenderer != null)
                       meshrenderer.material.color = new Color(0, 1, 0, StatusOpacity);
                 if (fix)
                    Fix(mu);
            }
        }

        private new void Awake()
        {
            if (!UseRayCast)
            {
                if (GetComponent<BoxCollider>()==null)
                    Error("Fixer neeeds a Box Collider attached to if no Raycast is used!");
            }
            base.Awake(); 
            layermask = LayerMask.GetMask(RayCastLayers.ToArray());
        }
        
        private void Start()
        {
            meshrenderer = GetComponent<MeshRenderer>();
            if (meshrenderer != null && meshrenderer.sharedMaterial != null && !UseRayCast)
            {
                if (ShowStatus)
                    meshrenderer.sharedMaterial.color = new Color(1, 0, 0, StatusOpacity);
            }

            if (!Application.isPlaying)
                return;
            signalfixerreleasenotnull = FixerRelease != null;
            signalfixerfixnotnull = FixerFix != null;
            var mus = GetComponentsInChildren<MU>();
            foreach (MU mu in mus)
            {
                if (!MUSFixed.Contains(mu))
                    Fix(mu);
            }
        }
        
        private void Raycast()
        {
            float scale = 1000;
            if (!Application.isPlaying)
            {
                if (Global.g4acontroller != null) scale = Global.g4acontroller.Scale;
            }
            else
            {
                scale = Game4AutomationController.Scale;
            }

            var globaldir = transform.TransformDirection(RaycastDirection);
            var display = Vector3.Normalize(globaldir) * RayCastLength / scale;
            hits = Physics.RaycastAll(transform.position, globaldir, RayCastLength/scale, layermask,
                QueryTriggerInteraction.UseGlobal);
            if (hits.Length>0)
            {
             
                if (ShowStatus) Debug.DrawRay(transform.position, display ,Color.red,0,true);
            }
            else
            {
                if (ShowStatus) Debug.DrawRay(transform.position, display, Color.yellow,0,true);
            
            }
    
        }

        private float GetDistance(MU mu)
        {
            return Vector3.Distance(mu.gameObject.transform.position, this.transform.position);
        }


        void CheckEntered()
        {
            var entered = MUSEntered.ToArray();
            for (int i = 0; i < entered.Length; i++)
            {
                AtPosition(entered[i]);
            }
        }
        
        void Update()
        {
            if (Deactivated)
                return;
            if (!Application.isPlaying && ShowStatus && UseRayCast)
            {
                Raycast();
            }
        }
        
        void FixedUpdate()
        {
       
           if (Deactivated)
                return;
            if (UseRayCast)
            {
                Raycast();
                if (hits.Length > 0)
                {
                    MUSEntered.Clear();
                    foreach (var hit in hits)
                    {
                        var mu = hit.collider.GetComponentInParent<MU>();
                        if (mu != null)
                        {
                            if (!MUSFixed.Contains(mu))
                            {
                                MUSEntered.Add(mu);
                            }
                        }
                    }
                }
                else
                {
                    MUSEntered.Clear();
                }
            }
            
            if (FixMU && !lastfixmu)
                Fix();
            if (!FixMU && lastfixmu)
                Release();
            lastfixmu = FixMU;
            
            if (OneBitFix)
            {
                // One Bit fixer - Fix = true fixes and false = releases - only on signal change
                if (signalfixerfixnotnull)
                {
                    if (FixerFix.Value && !lastfix)
                        Fix();
                    if (!FixerFix.Value && lastfix)
                        Release();
                    lastfix = FixerFix.Value;
                }
            }
            else
            {
                // Two Bit fixer
                if (signalfixerreleasenotnull)
                {
                    if (FixerRelease.Value && lastrelease == false)
                        Release();
                    lastrelease = FixerRelease.Value;
                }
                
                if (signalfixerfixnotnull)
                {
                    if (FixerFix.Value  && lastfix == false)
                        Fix();
                    lastfix = FixerFix.Value;
                }
            }

            if (AlignAndFixOnMinDistance)
            {
                foreach (var mu in MUSEntered)
                {
                    var distance = GetDistance(mu);
                    if (distance > mu.FixerLastDistance && mu.FixerLastDistance != -1)
                    {
                        AtPosition(mu);
                    }
                    mu.FixerLastDistance = distance;
                }
            }
            else
            {
                   CheckEntered();
            }
        }
    }
}