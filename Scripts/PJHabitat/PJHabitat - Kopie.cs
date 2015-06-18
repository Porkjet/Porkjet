using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Centrifuge
{
	public class PJCentrifuge : PartModule
	{
	/*	[KSPField]
		public string deployAnimationName = "deploy";
		
		[KSPField]
		public string loopAnimationName = "loop";
		
		[KSPField]
		public int deployLayer = 1;
		
		[KSPField]
		public int loopLayer = 2;
		
		[KSPField(isPersistant = true)]
		public float deploySpeed = 1f;
		
		[KSPField(isPersistant = true)]
		public float loopSpeed = 1f;
		
		[KSPField]
		public string startEventGUIName = "Deploy";

		[KSPField]
		public string endEventGUIName = "Retract";
		
		[KSPField]
		public int crewCapacityDeployed = 0;
		
		[KSPField]
		public int crewCapcityRetracted = 0;  */
		
		[KSPField]
    	public string rotorTransformName = "";
		public Transform rotorTransform;
		[KSPField]
		public string flywheelTransformName = "";
		public Transform flywheelTransform;	
		[KSPField]
		public float rotorRPM = 0f;	
		[KSPField]
		public float flywheelRotationMult = 0f;
		[KSPField]
		public float acceleration = 0.005f;
		[KSPField]
		public float habRadius = 0f;
		[KSPField(isPersistant = true)]
		private float speedMult = 0f;
		
		[KSPField]
		public string animationName = "";
		
		[KSPField(guiActive = true, guiName = "Artificial Gravity", guiUnits = "g", isPersistant = false)]
		public string gravity = "";
		public float geeforce = 0f;
		
		[KSPField(guiActive = true, guiName = "Animationtime", isPersistant = false)]
		public float test = 0f;
		
		
		private bool startrot = false;
		private bool stoprot = false;
		
		Animation anim;
		
		[KSPEvent(guiActive = true, guiName = "Spin", guiActiveEditor = true, guiActiveUnfocused = true, unfocusedRange = 5f)]
		public void activate()
		{
			if(anim[animationName].normalizedTime == 1)
			{
				startrot = true;
				stoprot = false;
			}
			else
			{
				ScreenMessages.PostScreenMessage("<color=orange>[" + part.partInfo.title + "]: Deploy first!</color>", 5, ScreenMessageStyle.UPPER_LEFT);
			}
		}
		
		[KSPEvent(guiActive = true, guiName = "Stop", guiActiveEditor = true, guiActiveUnfocused = true, unfocusedRange = 5f)]
		public void deactivate()
		{
			stoprot = true;
			startrot = false;
		}
		
		
		public override void OnStart(StartState state)
        {
			rotorTransform = part.FindModelTransform(rotorTransformName);
			flywheelTransform = part.FindModelTransform(flywheelTransformName);
        }
	
		private void Update()
		{
			rotorTransform.Rotate(new Vector3(0,6,0) * rotorRPM * speedMult * Time.deltaTime);
			flywheelTransform.Rotate(new Vector3(0,-6,0) * rotorRPM * speedMult *flywheelRotationMult * Time.deltaTime);
			geeforce = ((habRadius * Mathf.Pow((Mathf.PI * rotorRPM * speedMult / 30f), 2f)) / 9.81f);
			gravity = geeforce.ToString("F3");
			
			if(startrot == true && speedMult < 1)
			{
				speedMult += acceleration;
			}

			if(stoprot == true && speedMult > 0)
			{
				speedMult -= acceleration;
			}

			if(speedMult < 0)
			{
				speedMult = 0f;
			}
		}
		
		public override void OnUpdate()
        {
			anim = part.FindModelAnimators(animationName).FirstOrDefault();
			test = anim[animationName].normalizedTime;

		}
	}
}
   
