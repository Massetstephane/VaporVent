﻿
//using UnityEngine;
//using System.Collections;

//using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using UnityEngine;

namespace vaporvent
{
	public class makeSteam : PartModule 
	{
		List<KSPParticleEmitter> emitter;

        bool vaporVisible = false;

        [KSPEvent(guiActive = true, guiName = "Show Vapor", active = true)]
        public void ShowSteamEvent()
        {
            vaporVisible = true;
            SlowCheck();
            UpdateEvents(vaporVisible);
        }

        [KSPEvent(guiActive = true, guiName = "Hide Vapor", active = false)]
        public void HideSteamEvent()
        {
            vaporVisible = false;
            UpdateEvents(vaporVisible);
        }

        private void UpdateEvents(bool visible)
        {
            Events["ShowSteamEvent"].active = !visible;
            Events["HideSteamEvent"].active = visible;
        }

        [KSPAction("Toggle Vapor Vent")]
        public void ToggleSteamAction(KSPActionParam param)
        {
            vaporVisible = !vaporVisible;
            if (vaporVisible)
                SlowCheck();
            UpdateEvents(vaporVisible);
        }


        void PLSteam()
		{
            if (HighLogic.LoadedScene != GameScenes.FLIGHT)
                return;
            print ("vaporvent: started makesteam, vessel.situation: " + vessel.situation.ToString());
            emitter = part.FindModelComponents<KSPParticleEmitter>();
			if (emitter != null)
			{
				foreach(KSPParticleEmitter unit in emitter)
				{
					if (vaporVisible ) //|| Vessel.Situations.PRELAUNCH == vessel.situation || Vessel.Situations.LANDED == vessel.situation) 
					{
						unit.emit = true;
						print ("vaporvent: true");
					} 
					else 
					{
						unit.emit = false;
						print ("vaporvent: false");
                        vaporVisible = false;
                        UpdateEvents(vaporVisible);
                        CancelInvoke();
                    }
				}
			}
		}
		void SlowCheck()
		{
            // print("vaporvent: SlowCheck");
			InvokeRepeating("PLSteam", 0, 5F);
		}
        void onLaunch(EventReport evt)
        {
            Debug.Log("VaporVent.onLaunch");
           // if (evt.origin.vessel == this.vessel)
            {
                vaporVisible = false;
                UpdateEvents(vaporVisible);
            }
        }

        public override void OnStart(StartState state)
        {
            // print("vaporvent: OnStart");
            emitter = part.FindModelComponents<KSPParticleEmitter>();
            if (emitter != null)
                foreach (KSPParticleEmitter unit in emitter)
                    EffectBehaviour.AddParticleEmitter(unit);
            GameEvents.onLaunch.Add(onLaunch);

            base.OnStart(state);
            if (Vessel.Situations.PRELAUNCH == vessel.situation || Vessel.Situations.LANDED == vessel.situation)
            {
                vaporVisible = true;
                UpdateEvents(vaporVisible);
                SlowCheck();
            }
        }
        public void OnDestroy()
        {
            GameEvents.onLaunch.Remove(onLaunch);
            emitter = part.FindModelComponents<KSPParticleEmitter>();
            if (emitter != null)
                foreach (KSPParticleEmitter unit in emitter)
                    EffectBehaviour.RemoveParticleEmitter(unit);
        }
    }
}