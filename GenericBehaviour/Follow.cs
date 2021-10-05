using System;
using System.Collections.Generic;
using UnityEngine;
using Yusuf.AI.Pathfinding;
using Yusuf.DataStructure;
namespace Yusuf.AI.Behaviour
{
    public class Follow : ArtificialBehaviour
    {
        CharacterController charactercontroller;
        movement_stat mov_stat;
        Animator animator;

        public GameObject TargetToFollow;

        Transformnode transformnode;
        Transformnode target_transformnode;

        public int Run;
        public int Stop;
        void Start()
        {
            charactercontroller = GetComponent<CharacterController>();
            mov_stat = GetComponent<movement_stat>();
            animator = GetComponent<Animator>();
            transformnode = GetComponent<Transformnode>();
        }
        void Update()
        {
            if (Targetnode)
            {
                if (!issearchcomplete)
                {
                    blindsearch();
                }
            }
        }
        void move()
        {
            Vector3 lookpos = next().transform.position - transform.position;
            lookpos.y = 0;
            Quaternion desirerotation = Quaternion.LookRotation(lookpos);
            transform.rotation = Quaternion.Slerp(transform.rotation, desirerotation, mov_stat.rotation_speed * Time.deltaTime);
            //movetoward
            Vector3 movement = transform.TransformDirection(new Vector3(0, -1, 1 * mov_stat.speed * Time.deltaTime));
            charactercontroller.Move(movement);
        }
        void movetotarget()
        {
            Vector3 lookpos = TargetToFollow.transform.position - transform.position;
            lookpos.y = 0;
            Quaternion desirerotation = Quaternion.LookRotation(lookpos);
            transform.rotation = Quaternion.Slerp(transform.rotation, desirerotation, mov_stat.rotation_speed * Time.deltaTime);
            //movetoward
            Vector3 movement = transform.TransformDirection(new Vector3(0, -1, 1 * mov_stat.speed * Time.deltaTime));
            charactercontroller.Move(movement);
        }
        node next()
        {
            node leastfscore = Currentnode;
            foreach (node neightbor in leastfscore.Neightbor)
            {
                if (fscore[neightbor] < fscore[leastfscore])
                {
                    leastfscore = neightbor;
                }
            }
            return leastfscore;
        }
        Dictionary<node, int> fscore
        {
            get { return Targetnode.Fscore(); }
        }
        void blindsearch()
        {
            Targetnode.blindsearch();
        }
        node Currentnode
        {
            get
            {
                if (transformnode) return transformnode.Currentnode();
                return null;
            }
        }

        node Targetnode
        {
            get
            {
                if(TargetToFollow)
                {
                    return TargetToFollow.GetComponent<Transformnode>().Currentnode();
                }
                return null;
            }
        }
        bool issearchcomplete
        {
            get { return Targetnode.IsSearchcomplete(); }
        }
        public override void BeginBehaviour()
        {
            animator.SetInteger("Behaviour", Stop);
        }
        public override void UpdateBehaviour()
        {
            if (Currentnode && Targetnode)
            {
                if (issearchcomplete)
                {
                    if (Currentnode==Targetnode)
                    {
                        movetotarget();
                    }
                    else
                    {
                        move();
                    }
                    animator.SetInteger("Behaviour", Run);
                }
                else
                {
                    animator.SetInteger("Behaviour", Stop);
                }
            }
            else
            {
                animator.SetInteger("Behaviour", Stop);
            }
        }
        public override void EndBehaviour()
        {
            
        }
        public void SetTargettoFollow(GameObject target)
        {
            TargetToFollow = target;
        }
    }
}
