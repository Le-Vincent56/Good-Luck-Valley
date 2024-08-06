using GoodLuckValley.Patterns.StateMachine;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Settings.Controls.States
{
    public class ControlsState : IState
    {
        protected readonly ControlsSettingController controller;
        protected List<Animator> animators;

        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int BindingHash = Animator.StringToHash("Rebinding");

        protected const float crossFadeDuration = 0f;

        protected const float fadeTime = 0.2f;

        public ControlsState(ControlsSettingController controller, List<Animator> animators)
        {
            this.controller = controller;
            this.animators = animators;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void OnExit()
        {
        }
    }
}