using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Fragments from demolition
// Scale support for bound + Unyielding component

namespace RayFire
{
    [AddComponentMenu ("RayFire/Rayfire Connectivity")]
    [HelpURL ("http://rayfirestudios.com/unity-online-help/unity-connectivity-component/")]
    public class RayfireConnectivity : MonoBehaviour
    {
        [Header ("  Connectivity")]
        [Space (3)]
        
        [Tooltip ("Define the the way connections among Shards will be calculated.")]
        public ConnectivityType type = ConnectivityType.ByBoundingBox;
        
        [Header ("  Filters")]
        [Space (3)]
        
        [Tooltip ("Two shards will have connection if their shared area is bigger than this value.")]
        [Range (0, 1f)] public float minimumArea;
        [Space (2)]
        
        [Tooltip ("Two shards will have connection if their size is bigger than this value.")]
        [Range (0, 10f)] public float minimumSize;
        [Space (2)]
        
        [Tooltip ("Random percentage of connections will be discarded.")]
        [Range (0, 100)] public int percentage;
        [Space (2)]
        
        [Tooltip ("Seed for random percentage filter and for Random Collapse.")]
        [Range (0, 100)] public int seed;

        // [Space (2)]
        // [Header ("Check")]
        // [HideInInspector] public bool onActivation = true;
        // [Space (1)]
        // [HideInInspector] public bool onDemolition = true;
        
        [Header ("  Cluster Properties")]
        [Space (3)]
        
        [Tooltip ("Create Connected Cluster for group of Shards connected with each other but not connected with any Unyielding Shard.")]
        public bool clusterize = true;
        [Space (2)]
        
        [Tooltip ("Set Demolition type to Runtime for Connected Clusters created during activation.")]
        public bool demolishable;

        [Header ("  Collapse")]
        [Space (3)]
        
        [Tooltip ("Collapse allows you start break connections among shards and activate single Shards or " +
                  "Group of Shards if they are not connected with any of Unyielding Shard. ")]
        public RFCollapse collapse;
        
        // Hidden
        [HideInInspector] public bool               showConnections = true;
        [HideInInspector] public bool               showNodes       = true;
        [HideInInspector] public bool               checkConnectivity;
        [HideInInspector] public bool               connectivityCheckNeed;
        [HideInInspector] public bool               showGizmo = true;
        [HideInInspector] public List<RayfireRigid> rigidList;
        [HideInInspector] public RFCluster          cluster;
        [HideInInspector] public int                initShardAmount;
        
        
        [NonSerialized] RayfireRigidRoot rigidRoot;
        [NonSerialized] bool childrenCorState;
        [NonSerialized] bool connectivityCorState;
        [NonSerialized] bool childrenChanged;
        
        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////
        
        // Awake
        void Awake()
        {
            // Set by children.
            SetConnectivity();
        }

        // Start is called before the first frame update
        void Start()
        {
            // Check
            if (Check() == false)
                return;

            // Start cors
            StartCoroutine(ChildrenCor());
            StartCoroutine(ConnectivityCor());
        }

        // Check
        bool Check()
        {
            // Check for Rigid Root with shards
            if (rigidRoot != null)
            {
                if (rigidRoot.cluster.shards.Count > 0)
                {
                    Debug.Log ("RayFire RigidRoot: " + name + " has some shards.", gameObject);
                    return true;
                }
            }
            
            // Rigid check
            if (rigidList.Count == 0)
            {
                Debug.Log ("RayFire Connectivity: " + name + " has no objects to check for connectivity. Connectivity disabled.", gameObject);
                return false;
            }
            
            // Check for not mesh root rigid
            RayfireRigid rigid = GetComponent<RayfireRigid>();
            if (rigid != null)
            {
                if (rigid.objectType != ObjectType.MeshRoot)
                {
                    Debug.Log ("RayFire Connectivity: " + name + " object has Rigid component but object type is not Mesh Root. Connectivity disabled.", gameObject);
                    return false;
                }
            }

            return true;
        }

        /// /////////////////////////////////////////////////////////
        /// Enable/Disable
        /// /////////////////////////////////////////////////////////
        
        /*
        
        // Disable
        void OnDisable()
        {
            childrenCorState     = false;
            connectivityCorState = false;
        }

        // Enable
        void OnEnable()
        {
            // Start cors
            if (gameObject.activeSelf == true && cluster != null && cluster.shards != null && cluster.shards.Count > 0)
            {
                StartCoroutine(ChildrenCor());
                StartCoroutine(ConnectivityCor());
            }
        }
        
        */
        
        /// /////////////////////////////////////////////////////////
        /// Children change
        /// /////////////////////////////////////////////////////////    
        
        // Child removed
        void OnTransformChildrenChanged()
        {
            childrenChanged = true;
        }

        void OnTransformParentChanged()
        {
            
        }

        // Connectivity check cor
        IEnumerator ChildrenCor()
        {
            // Stop if running 
            if (childrenCorState == true)
                yield break;
            
            // Set running state
            childrenCorState = true;
            
            bool checkChildren = true;
            while (checkChildren == true)
            {
                // Get not connected groups
                if (childrenChanged == true)
                    connectivityCheckNeed = true;

                yield return null;
            }
            
            // Set state
            childrenCorState = false;
        }

        // Check for children
        void ChildrenCheck()
        {
            for (int s = cluster.shards.Count - 1; s >= 0; s--)
            {
                if (cluster.shards[s].tm == null)
                {
                    if (cluster.shards[s].neibShards.Count > 0)
                    {
                        // Remove itself in neibs
                        for (int n = 0; n < cluster.shards[s].neibShards.Count; n++)
                        {
                            // Check every neib in neib
                            for (int i = 0; i < cluster.shards[s].neibShards[n].neibShards.Count; i++)
                            {
                                if (cluster.shards[s].neibShards[n].neibShards[i] == cluster.shards[s])
                                {
                                    cluster.shards[s].neibShards[n].neibShards.RemoveAt (i);
                                    cluster.shards[s].neibShards[n].nArea.RemoveAt (i);
                                    cluster.shards[s].neibShards[n].nIds.RemoveAt (i);
                                    break;
                                }
                            }
                        }
                        
                    }
                    cluster.shards.RemoveAt (s);
                }
            }
            childrenChanged = false;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Setup
        /// ///////////////////////////////////////////////////////// 
        
        // Children tms
        public void SetConnectivity()
        {
            rigidRoot = GetComponent<RayfireRigidRoot>();
            if (rigidRoot != null)
                SetClusterRigidRoot();
            else
                SetClusterRigidList();
        }

        // Get all children
        List<Transform> SetChildren()
        {
            List<Transform> tmList = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
                tmList.Add (transform.GetChild (i));
            return tmList;
        }
        
        // Set cluster
        void SetRigids(List<Transform> tmList)
        {
            // No targets
            if (tmList.Count == 0)
                return;

            // Get rigid with byConnectivity
            rigidList = new List<RayfireRigid>();
            for (int i = 0; i < tmList.Count; i++)
            {
                RayfireRigid rigid = tmList[i].GetComponent<RayfireRigid>();
                if (rigid != null)
                    if (rigid.simulationType == SimType.Inactive || rigid.simulationType == SimType.Kinematic)
                        if (rigid.activation.byConnectivity == true)
                            rigidList.Add (rigid);
            }
            
            // No targets
            if (rigidList.Count == 0)
                return;
            
            // Set this connectivity as main connectivity node
            for (int i = 0; i < rigidList.Count; i++)
                rigidList[i].activation.connect = this;
        }

        /// /////////////////////////////////////////////////////////
        /// Cluster Common
        /// /////////////////////////////////////////////////////////  
        
        // Prepare cluster
        void PrepareCluster()
        {
            // In case of runtime add
            if (cluster == null)
                cluster = new RFCluster();

            // Missing shards check in case of cached shards
            if (RFCluster.IntegrityCheck (cluster) == false)
            {
                Debug.Log ("IntegrityCheck fail");
                cluster = new RFCluster();
            }
        }

        // Create default cluster
        void CreateCluster()
        {
            cluster              = new RFCluster();
            cluster.id           = RFCluster.GetUniqClusterId (cluster);
            cluster.tm           = transform;
            cluster.depth        = 0;
            cluster.pos          = transform.position;
            cluster.rot          = transform.rotation;
            cluster.demolishable = demolishable;
            cluster.initialized  = true;
        }
        
        // Final custer ops: set shards neibs, set range data
        void FinishCLuster()
        {
            // Set shard neibs
            RFShard.SetShardNeibs (cluster.shards, type, minimumArea, minimumSize, percentage, seed);

            // Set range for area and size
            RFCollapse.SetRangeData (cluster, percentage, seed);
                
            // Set initial shards amount
            initShardAmount = cluster.shards.Count;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Cluster by Rigids
        /// /////////////////////////////////////////////////////////  
        
        // Set cluster
        void SetClusterRigidList ()
        {
            // Get all children to set connectivity
            List<Transform> tmList = SetChildren();

            // Prepare cluster. Check for integrity of cached shards.
            PrepareCluster();
            
            // Play mode ops. Not for Editor
            if (Application.isPlaying == true)
            {
                // Set rigids list and connect with Connectivity component
                SetRigids (tmList);
                
                // Shards were cached, reinit non serialized vars, clear list otherwise
                if (InitCachedShardsByRigidList (rigidList, cluster) == true)
                    cluster.shards.Clear();
            }
            
            // Create main cluster
            if (cluster.shards.Count == 0)
            {
                // Create default cluster
                CreateCluster();
                
                // Set shards for main cluster
                if (Application.isPlaying == true)
                    RFShard.SetShardsByRigidList (cluster, rigidList, type);
                else
                    RFShard.SetShardsByTransformList (cluster, tmList, type);
                
                // Final custer ops
                FinishCLuster();
            }
        }
        
        // Reinit shard's non serialized fields in case of prefab use
        public static bool InitCachedShardsByRigidList (List<RayfireRigid> rigids, RFCluster cluster)
        {
            // Not initialized
            if (cluster.initialized == true)
                return false;
            
            // No shards
            if (cluster.shards.Count == 0)
                return false;
            
            // Rigid list doesn't match shards. TODO compare per shard
            if (cluster.shards.Count != rigids.Count)
                return true;
            
            // Reinit
            for (int s = 0; s < cluster.shards.Count; s++)
            {
                if (rigids[s] != null)
                {
                    cluster.shards[s].rigid = rigids[s];
                    cluster.shards[s].uny = rigids[s].activation.unyielding;
                }
                
                cluster.shards[s].cluster = cluster;
                cluster.shards[s].neibShards = new List<RFShard>();
                for (int n = 0; n < cluster.shards[s].nIds.Count; n++)
                    cluster.shards[s].neibShards.Add (cluster.shards[cluster.shards[s].nIds[n]]);
            }
            cluster.initialized = true;
            return false;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Cluster By Rigid Root
        /// ///////////////////////////////////////////////////////// 
                
        // Set cluster
        void SetClusterRigidRoot ()
        {
            // Prepare cluster
            PrepareCluster();
            
            // Main cluster cached, reinit non serialized vars
            if (cluster.shards.Count > 0)
            {
                // TODO reinit for rigid root
            }
            
            // Create main cluster
            if (cluster.shards.Count == 0)
            {
                // Create default cluster
                CreateCluster();

                // Set connectivity controller
                rigidRoot.activation.connect = this;
                
                // TODo collect rigidroot shards if manual editor setup

                // Set shards for main cluster
                for (int i = 0; i < rigidRoot.cluster.shards.Count; i++)
                {
                    if (rigidRoot.cluster.shards[i].mf != null)
                    {
                        // Set shards fields 
                        rigidRoot.cluster.shards[i].cluster = cluster;
                        
                        // Set faces data for connectivity
                        if (type == ConnectivityType.ByMesh)
                            RFTriangle.SetTriangles(rigidRoot.cluster.shards[i], rigidRoot.cluster.shards[i].mf); 
                        
                        // Collect shard
                        cluster.shards.Add(rigidRoot.cluster.shards[i]);
                    }
                }
                
                // Final custer ops
                FinishCLuster();;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Connectivity
        /// /////////////////////////////////////////////////////////   
        
        // Connectivity check cor
        IEnumerator ConnectivityCor()
        {
            // Stop if running 
            if (connectivityCorState == true)
                yield break;
            
            // Set running state
            connectivityCorState = true;
            
            checkConnectivity = true;
            while (checkConnectivity == true)
            {
                // Child deleted
                if (childrenChanged == true)
                    ChildrenCheck();
                
                // Get not connected groups
                if (connectivityCheckNeed == true)
                    CheckConnectivity();
                
                yield return null;
            }
            
            // Set state
            connectivityCorState = false;
        }
        
        // Check for connectivity TODO temp, combine CheckConnectivityRigidRoot and CheckConnectivityRigidList
        public void CheckConnectivity()
        {
            // Rigid Root connectivity check.
            if (rigidRoot != null)
            {
                CheckConnectivityRigidRoot();
                return;
            }

            // Check for connectivity
            CheckConnectivityRigidList();
        }
        
        // Check for connectivity
        void CheckConnectivityRigidList()
        {
            // Do once
            connectivityCheckNeed = false;

            // Clear all activated/demolished shards
            CleanUpActivatedShards (cluster);
            
            // No shards to check
            if (cluster.shards.Count == 0)
                return;
            
            // Reinit neibs after cleaning
            RFShard.ReinitNeibs (cluster.shards);
            
            // List of shards to be activated
            List<RFShard> soloShards = new List<RFShard>();

            // TODO do not collect solo uny shards
             
            // Check for solo shards and collect
            RFCluster.GetSoloShards (cluster, soloShards);
            
            // Reinit neibs before connectivity check
            RFShard.ReinitNeibs (cluster.shards);
            
            // Connectivity check
            RFCluster.ConnectivityCheck (cluster);
            
            // Get not connected and not unyielding child cluster
            CheckUnyielding (cluster);

            // TODO ONE NEIB DETACH FOR CHILD CLUSTERS
            
            // Activate shards or clusterize not connected groups
            ActivateShards (soloShards);

            // Stop checking. Everything activated
            if (cluster.shards.Count == 0)
                checkConnectivity = false;
        }
        
        // Check for connectivity TODO combine with original
        void CheckConnectivityRigidRoot()
        {
            // Do once
            connectivityCheckNeed = false;

            // Clear all activated/demolished shards
            CleanUpActivatedShardsRigidRoot (cluster);
            
            // No shards to check
            if (cluster.shards.Count == 0)
                return;
            
            // Reinit neibs after cleaning
            RFShard.ReinitNeibs (cluster.shards);
            
            // List of shards to be activated
            List<RFShard> soloShards = new List<RFShard>();

            // TODO do not collect solo uny shards
             
            // Check for solo shards and collect
            RFCluster.GetSoloShards (cluster, soloShards);
            
            // Reinit neibs before connectivity check
            RFShard.ReinitNeibs (cluster.shards);
            
            // Connectivity check
            RFCluster.ConnectivityCheck (cluster);
            
            // Get not connected and not unyielding child cluster
            CheckUnyieldingRigidRoot (cluster);

            // TODO ONE NEIB DETACH FOR CHILD CLUSTERS
            
            // Activate shards or clusterize not connected groups
            ActivateShards (soloShards);

            // Stop checking. Everything activated
            if (cluster.shards.Count == 0)
                checkConnectivity = false;
        }
        
        // Activate shards or clusterize not connected groups
        void ActivateShards(List<RFShard> soloShards)
        {
            // Activate not connected shards. 
            if (soloShards.Count > 0)
                for (int i = 0; i < soloShards.Count; i++)
                    RFActivation.ActivateShard (soloShards[i], rigidRoot);
            
            // Clusterize childClusters or activate their shards
            if (cluster.HasChildClusters == true)
            {
                if (clusterize == true)
                    Clusterize();
                else
                    for (int c = 0; c < cluster.childClusters.Count; c++)
                        for (int s = 0; s < cluster.childClusters[c].shards.Count; s++)
                            RFActivation.ActivateShard (cluster.childClusters[c].shards[s], rigidRoot);
            }
        }
        
        // Clusterize not connected groups
        void Clusterize()
        {
            for (int i = 0; i < cluster.childClusters.Count; i++)
            {
                // set demolishable state for child cluster TODO ???????? why cluster and not cluster.childClusters[i]?
                cluster.demolishable = demolishable;
                
                // Set bound 
                cluster.childClusters[i].bound = RFCluster.GetShardsBound (cluster.childClusters[i].shards);
                
                // TODO get source for rigid props: any shard's rigid or Rigid Root
                
                // TODO TEMP SOLUTION
                if (rigidRoot != null)
                {
                    
                    // Create root for left children TODO set name
                    if (cluster.childClusters[i].tm == null)
                        RFCluster.CreateClusterRoot (cluster.childClusters[i], cluster.childClusters[i].shards[0].tm.position, 
                            Quaternion.identity, gameObject.layer, gameObject.tag, gameObject.name + cluster.id);
                    
                    // Create rigid cor connected cluster
                    cluster.childClusters[i].rigid = cluster.childClusters[i].tm.gameObject.AddComponent<RayfireRigid>();
                    rigidRoot.CopyPropertiesTo (cluster.childClusters[i].rigid);
                    cluster.childClusters[i].rigid.objectType = ObjectType.ConnectedCluster;

                    // Destroy components and set dynamic state
                    for (int s = 0; s < cluster.childClusters[i].shards.Count; s++)
                    {
                        cluster.childClusters[i].shards[s].sm = 1;
                        Destroy (cluster.childClusters[i].shards[s].rb);
                    }

                    RFDemolitionCluster.CreateClusterRuntime (cluster.childClusters[i].rigid, cluster.childClusters[i]);
                    
                        // TODO copy child cluster shards to connected cluster shards
                    
                        // TODO Clean Up from shards
                }

                else
                {
                    // Create cluster
                    cluster.childClusters[i].shards[0].rigid.simulationType = SimType.Dynamic; // TODO IN BETTER WAY
                    cluster.childClusters[i].shards[0].rigid.objectType     = ObjectType.ConnectedCluster;
                    RFDemolitionCluster.CreateClusterRuntime (cluster.childClusters[i].shards[0].rigid, cluster.childClusters[i]);
                    cluster.childClusters[i].shards[0].rigid.objectType = ObjectType.Mesh;
                    
                    // Destroy components
                    for (int s = 0; s < cluster.childClusters[i].shards.Count; s++)
                    {
                        Destroy (cluster.childClusters[i].shards[s].rigid.physics.rigidBody);
                        Destroy (cluster.childClusters[i].shards[s].rigid);
                    }
                }
                
                // Copy preview
                cluster.childClusters[i].rigid.clusterDemolition.cn = showConnections;
                cluster.childClusters[i].rigid.clusterDemolition.nd = showNodes;
            }
        }

        // Clear all activated/demolished shards TODO change to CleanUpActivatedShardsRigidRoot method:sm check
        static void CleanUpActivatedShards(RFCluster cluster)
        {
            for (int i = cluster.shards.Count - 1; i >= 0; i--)
            {
                if (cluster.shards[i].rigid == null ||
                    cluster.shards[i].rigid.activation.connect == null ||
                    cluster.shards[i].rigid.limitations.demolished == true)
                {
                    cluster.shards[i].cluster = null;
                    cluster.shards.RemoveAt (i);
                }
            }
        }
        
        // Clear all activated/demolished shards TODO combine with original
        static void CleanUpActivatedShardsRigidRoot(RFCluster cluster)
        {
            for (int i = cluster.shards.Count - 1; i >= 0; i--)
            {
                if (cluster.shards[i].sm == 1)
                {
                    cluster.shards[i].cluster = null;
                    cluster.shards.RemoveAt (i);
                }
            }
        }
 
        // Collect solo shards, remove from cluster, reinit cluster
        static void CheckUnyielding(RFCluster cluster)
        {
            // Get not connected and not unyielding child cluster
            if (cluster.HasChildClusters == true)
            {
                // Remove all unyielding child clusters
                for (int c = cluster.childClusters.Count - 1; c >= 0; c--)
                {
                    if (cluster.childClusters[c].UnyieldingByRigid == true)
                    {
                        cluster.shards.AddRange (cluster.childClusters[c].shards);
                        cluster.childClusters.RemoveAt (c);
                    }
                }
                
                // Set unyielding cluster shards back to original cluster
                for (int s = 0; s < cluster.shards.Count; s++)
                    cluster.shards[s].cluster = cluster;
            }
        }
        
        // Collect solo shards, remove from cluster, reinit cluster
        static void CheckUnyieldingRigidRoot(RFCluster cluster)
        {
            // Get not connected and not unyielding child cluster
            if (cluster.HasChildClusters == true)
            {
                // Remove all unyielding child clusters
                for (int c = cluster.childClusters.Count - 1; c >= 0; c--)
                {
                    if (cluster.childClusters[c].UnyieldingByShard == true)
                    {
                        cluster.shards.AddRange (cluster.childClusters[c].shards);
                        cluster.childClusters.RemoveAt (c);
                    }
                }
                
                // Set unyielding cluster shards back to original cluster
                for (int s = 0; s < cluster.shards.Count; s++)
                    cluster.shards[s].cluster = cluster;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Reset
        /// ///////////////////////////////////////////////////////// 

        // Reset shards back to initial state
        public void ResetShards()
        {
            // Reset solo rigids 
            foreach (var rigid in rigidList)
            {
                if (rigid != null)
                {
                    rigid.ResetRigid();
                }
            }
            
            // TODO Reset clustered rigids

        }

        /// /////////////////////////////////////////////////////////
        /// Get
        /// /////////////////////////////////////////////////////////  

        // CLuster Integrity
        public float AmountIntegrity
        {
            get
            {
                return  (float)cluster.shards.Count / initShardAmount * 100f;
            }
        }
    }
}