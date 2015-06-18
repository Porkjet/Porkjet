using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Habitat
{
	public class Centrifuge : PartModule
	{
		#region CFG parameters
		
		[KSPField]
    	public string rotorTransformName = "center";
		public Transform rotorTransform;
		[KSPField]
		public string flywheelTransformName = "flywheel";
		public Transform flywheelTransform;	
		[KSPField]
		public string internalTransformName = "internal";
		public Transform internalTransform;	
		[KSPField]
		public float rotorRPM = 0f;	
		[KSPField]
		public float flywheelRotationMult = 0f;
		[KSPField]
		public float acceleration = 0.002f;
		[KSPField]
		public float habRadius = 0f;
		[KSPField]
		public string animationName = "";		// animation used in ModuleAnimateGeneric
		
		#endregion
		
		[KSPField(isPersistant = true)]
		private float speedMult = 0f;
		
		[KSPField(guiActive = true, guiActiveEditor = true, guiFormat = "F2", guiName = "Artificial Gravity", guiUnits = "g", isPersistant = false)]
		public float currentGeeforce = 0f;
		private float geeforce = 0f;	//info for editor action menu
		
		private bool startrot = false;
		
		Animation anim;	
		
		//display anim normalized time in pupup for testing
		//[KSPField(guiActive = true, guiActiveEditor = true, guiName = "Animationtime", isPersistant = false)]
		//public float test = 0f;
		
		public override void OnStart(StartState state)
        {
			rotorTransform = part.FindModelTransform(rotorTransformName);
			flywheelTransform = part.FindModelTransform(flywheelTransformName);
			//internalTransform = part.InternalModel.FindModelTransform(internalTransformName);			//doesnt work
			geeforce = ((habRadius * Mathf.Pow((Mathf.PI * rotorRPM / 30f), 2f)) / 9.81f);
        }
	
		private void Update()
		{
			anim = part.FindModelAnimators(animationName).FirstOrDefault();
			//test = anim[animationName].normalizedTime;
			
			rotorTransform.Rotate(new Vector3(0,6,0) * rotorRPM * speedMult * Time.deltaTime);
			flywheelTransform.Rotate(new Vector3(0,-6,0) * rotorRPM * speedMult *flywheelRotationMult * Time.deltaTime);
			//internalTransform.Rotate(new Vector3(0,6,0) * rotorRPM * speedMult * Time.deltaTime);
			currentGeeforce = ((habRadius * Mathf.Pow((Mathf.PI * rotorRPM * speedMult / 30f), 2f)) / 9.81f);
			
			if(anim[animationName].normalizedTime == 1f)
			{
				startrot = true;
			}
			else
			{
				startrot = false;
			}
			
			if(startrot == true && speedMult < 1)
			{
				speedMult += acceleration;
			}

			if(startrot == false && speedMult > 0)
			{
				speedMult -= acceleration;
			}

			if(speedMult < 0)
			{
				speedMult = 0f;
			}
		}
		public override string GetInfo()
		{
			string output = "";
			output += "<b>Artificial gravity: </b> " + ((habRadius * Mathf.Pow((Mathf.PI * rotorRPM / 30f), 2f)) / 9.81f).ToString("0.00") + "g";
			output += "\n<b>Speed of rotation: </b> " + rotorRPM.ToString("0") + "rpm";
			return output;
		}
	}
	
	
	
	//Sirkut's original Habitat Module
	
	//TODO - make this work in editor
	
	public class DeployableHabitat : PartModule
    {
        [KSPField]
        public string animationName = ""; //animation used by ModuleAnimateGeneric
        [KSPField]
        public int crewCapacityDeployed = 0;
        [KSPField]
        public int crewCapacityRetracted = 0;

        Animation anim;
        public override void OnStart(StartState state)
        {
            base.OnStart(state);
        }
		public override string GetInfo()
		{
			string output = "";
			output += "<b>Crew capacity deployed: </b>" + crewCapacityDeployed.ToString("0");
			if(crewCapacityRetracted > 0)
			{
				output += "\n<b>Crew capacity retracted: </b>" + crewCapacityRetracted.ToString("0");
			}
			else
			{
				output += "\n<b><color=orange>Can't be crewed while retracted</color></b>";
			}
			return output;
		}
        public void Start()
        {
            if (!HighLogic.LoadedSceneIsFlight) return;
        }

        
        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                anim = part.FindModelAnimators(animationName).FirstOrDefault();
                if (anim[animationName].normalizedTime < 1f)
                {
                    this.part.CrewCapacity = crewCapacityRetracted;
                }
                if (anim[animationName].normalizedTime == 1f)
                {
                    this.part.CrewCapacity = crewCapacityDeployed;
                }


                ModuleAnimateGeneric animateModule = (ModuleAnimateGeneric)this.part.GetComponent("ModuleAnimateGeneric");
                if (this.part.protoModuleCrew.Count() > 0)
                {
                    foreach (BaseEvent eventname in this.part.GetComponent<ModuleAnimateGeneric>().Events)
                    {
                        if(eventname.guiName == animateModule.endEventGUIName)
                            eventname.guiActive = false;
                    }
                }
                else
                {
                    foreach (BaseEvent eventname in this.part.GetComponent<ModuleAnimateGeneric>().Events)
                    {
                        if (eventname.guiName == animateModule.endEventGUIName)
                            eventname.guiActive = true;
                    }
                }
            }
            base.OnUpdate();
        }
    }
}
   
