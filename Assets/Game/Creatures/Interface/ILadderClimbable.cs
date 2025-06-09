using UnityEngine;

namespace Asce.Game.Entities
{
    public interface ILadderClimbable : IEntity
    {
        public bool IsClimbEnabled { get; set; }
        public bool IsClimbingLadder { get; set; }

        public bool IsEnteringLadder { get; }
        public bool IsExitingLadder { get; }

        public float LadderEnterHeight { get; }
        public float LadderExitHeight { get; }

        public float ClimbMaxSpeed { get; }
        public float ClimbFastMaxSpeed { get; }


    }
}
