using System;
using System.Collections.Generic;
using UnityEngine;
using Yusuf.AI.Pathfinding;
namespace Yusuf.AI.Behaviour
{
    public class MoveToObjective : ArtificialBehaviour
    {
        CharacterController charactercontroller;
        movement_stat mov_stat;
        Animator animator;

        Transformnode transformnode;
        node Goalnode;

        public int Run;
        public int Stop;
        public int Search;

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
            get { if (transformnode) return transformnode.Currentnode(); return null; }
        }
        Dictionary<node, int> fscore
        {
            get { return Goalnode.Fscore(); }
        }
        bool issearchcomplete
        {
            get { return Goalnode.IsSearchcomplete(); }
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
                    else
                    {
                        move();
                        animator.SetInteger("Behaviour", Run);//move when it is not goal
                    }
                }
                else
                {
                    blindsearch();
                    animator.SetInteger("Behaviour", Search);//stop when search is not complete
                }
            }else
            {
                animator.SetInteger("Behaviour", Stop);//stop if currentnode or goal node is null
            }
        }

        public override void EndBehaviour()
        {
            throw new NotImplementedException();
        }

    }
}
