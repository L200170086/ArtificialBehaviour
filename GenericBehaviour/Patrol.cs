using System;
using UnityEngine;
namespace Yusuf.AI.Behaviour
{
    public class Patrol:ArtificialBehaviour
    {
        CharacterController charactercontroller;
        movement_stat mov_stat;
        Animator animator;
        int i;

        public Transform[] patrolwaypoint;
        public int Run;
        void Start()
        {
            charactercontroller = GetComponent<CharacterController>();
            mov_stat = GetComponent<movement_stat>();
            animator = GetComponent<Animator>();
        }
        void move()
        {
            Vector3 lookpos = patrolwaypoint[i].transform.position - transform.position;
            lookpos.y = 0;
            Quaternion desirerotation = Quaternion.LookRotation(lookpos);
            transform.rotation = Quaternion.Slerp(transform.rotation, desirerotation, mov_stat.rotation_speed * Time.deltaTime);
            //movetoward
            Vector3 movement = transform.TransformDirection(new Vector3(0, -1, 1 * mov_stat.speed * Time.deltaTime));
            charactercontroller.Move(movement);
        }
        public override void BeginBehaviour()
        {
            throw new NotImplementedException();
        }
        public override void UpdateBehaviour()
        {
            Vector3 diffence = patrolwaypoint[i].transform.position - transform.position;
            if (diffence.magnitude < 0.5f)
            {
                i = (i + 1) % patrolwaypoint.Length;
            }
            move();
            animator.SetInteger("Behaviour", Run);
        }
        public override void EndBehaviour()
        {
            throw new NotImplementedException();
        }
    }
}
