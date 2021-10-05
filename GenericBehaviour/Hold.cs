using UnityEngine;
namespace Yusuf.AI.Behaviour
{
    public class Hold:ArtificialBehaviour
    {
        Animator animator;

        public int Stop;
        void Start()
        {
            animator = GetComponent<Animator>();
        }
        public override void BeginBehaviour()
        {
            animator.SetInteger("Behaviour", Stop);
        }
        public override void UpdateBehaviour()
        {
            animator.SetInteger("Behaviour", Stop);
        }
        public override void EndBehaviour()
        {
        }
    }
}
