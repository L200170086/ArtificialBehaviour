using System.Collections.Generic;
using UnityEngine;
using Yusuf.AI.Pathfinding;
namespace Yusuf.AI.Behaviour
{
    /// <summary>
    /// this behavior to guide player.
    /// stop if pathfinding not complete or player arent in area
    /// the will to goal when player in area  
    /// </summary>
    public class Guide : ArtificialBehaviour
    {
        CharacterController charactercontroller;
        movement_stat mov_stat;
        Animator animator;

        Transformnode transformnode;
        node Goalnode;

        public GameObject targettoguide;
        public int Run;
        public int Stop;
        public int Search;

        //public GameObject experiment;
        //public node experimentgoal;
        void Start()
        {
            charactercontroller = GetComponent<CharacterController>();
            mov_stat = GetComponent<movement_stat>();
            animator = GetComponent<Animator>();
            transformnode = GetComponent<Transformnode>();
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
        void blindsearch()
        {
            Goalnode.blindsearch();
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
        node Currentnode
        {
            get{if (transformnode) return transformnode.Currentnode();return null;}
        }
        Dictionary<node, int> fscore
        {
            get { return Goalnode.Fscore(); }
        }
        bool issearchcomplete
        {
            get{return Goalnode.IsSearchcomplete();}
        }
        public void SetObjecttoGuide(GameObject willguide)
        {
            targettoguide = willguide;
        }
        public void SetGoal(node goal)
        {
            Goalnode = goal;
        }
        public override void BeginBehaviour()
        {
            animator.SetInteger("Behaviour", Stop);
        }
        public override void UpdateBehaviour()
        {
            if (Currentnode && Goalnode)
            {
                if (issearchcomplete)
                {
                    if (fscore[Currentnode] == 0)
                    {
                        animator.SetInteger("Behaviour", Stop);//stop when it is goal
                    }
                    else if (targettoguide)
                    {
                        if (Currentnode.inarea(targettoguide.transform.position))
                        {
                            move();
                            animator.SetInteger("Behaviour", Run);//guide player
                        }
                        else
                        {
                            animator.SetInteger("Behaviour", Stop);//wait for player
                        }
                    }
                    else
                    {
                        animator.SetInteger("Behaviour", Stop);//no player are be guided
                    }
                }
                else
                {
                    blindsearch();
                    animator.SetInteger("Behaviour",Search);//pathplanning
                }
            }
            else
            {
                animator.SetInteger("Behaviour", Stop);//no goal
            }
        }
        public override void EndBehaviour()
        {
        }
    }
}
