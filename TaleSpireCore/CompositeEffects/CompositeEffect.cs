using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using System.Threading;

namespace TaleSpireCore
{
	public class CompositeEffect
	{
		public static IKnownEffectsBuilder EffectsBuilder { get; set; }

		/// <summary>
		/// The seconds to delay awakening this effect, in seconds.
		/// </summary>
		public float DelayStart { get; set; }


		/// <summary>
		/// The name of the prefab effect to create at this location in the hierarchy.
		/// Leave empty to create an empty game object.
		/// </summary>
		public string PrefabToCreate { get; set; }
		public string ExistingChildName { get; set; }
		public string ItemToClone { get; set; }



		/// <summary>
		/// The list of all the properties needing changing in this composite effect.
		/// </summary>
		public List<PropertyChangerDto> Props { get; set; }


		/// <summary>
		/// Any child effects. The structure of this effect composite needs to match the structure of the prefab.
		/// Empty game objects and other prefabs can parent this prefab/game object or be children to this effect.
		/// </summary>
		public List<CompositeEffect> Children { get; set; } = new List<CompositeEffect>();


		// TODO: Change Scripts to a more sophisticate class, to include property settings.

		/// <summary>
		/// Scripts to add to this composite effect (added before setting properties).
		/// </summary>
		public List<string> Scripts { get; set; }


		/// <summary>
		/// Private list of properties to change. This is where the real work (changing properties) is done.
		/// The public "Props" property is just for serialization/deserialization.
		/// </summary>
		List<BasePropertyChanger> properties { get; set; }

		[JsonIgnore]
		public CreatureBoardAsset Mini { get; set; }
		public string ItemToBorrow { get; set; }
		public string EffectNameToCreate { get; set; }


		CharacterPosition sourcePosition;
		CharacterPosition targetPosition;

		public CompositeEffect()
		{

		}

		/// <summary>
		/// Call after deserialization...
		/// </summary>
		public void RebuildPropertiesAfterLoad()
		{
			Talespire.Log.Debug("RebuildPropertiesAfterLoad");

			if (Props != null)
			{
				properties = new List<BasePropertyChanger>();

				foreach (PropertyChangerDto propertyChangerDto in Props)
				{
					Talespire.Log.Debug($"{propertyChangerDto.Name}: {propertyChangerDto.Type};");
					BasePropertyChanger changer = PropertyChangerFactory.CreateFrom(propertyChangerDto);
					if (changer != null)
					{
						changer.Name = propertyChangerDto.Name;
						changer.Value = propertyChangerDto.Value;
						properties.Add(changer);
					}
				}
			}

			foreach (CompositeEffect compositeEffect in Children)
				compositeEffect.RebuildPropertiesAfterLoad();
		}

		void RemoveExistingProperty(string name)
		{
			BasePropertyChanger propertyChanger = properties.FirstOrDefault(x => x.Name == name);
			if (propertyChanger != null)
				properties.Remove(propertyChanger);

			PropertyChangerDto propDto = Props.FirstOrDefault(y => y.Name == name);
			if (propDto != null)
				Props.Remove(propDto);
		}

		public void AddProperty(BasePropertyChanger propertyChanger)
		{
			InitializePropertyVars();
			RemoveExistingProperty(propertyChanger.Name);
			properties.Add(propertyChanger);
			Props.Add(propertyChanger.ToPropertyChangerDto());
		}

		private void InitializePropertyVars()
		{
			if (properties == null)
				properties = new List<BasePropertyChanger>();
			if (Props == null)
				Props = new List<PropertyChangerDto>();
		}

		void ModifyProperties(GameObject effect)
		{
			if (properties == null)
				return;

			foreach (var basePropertyDto in properties)
			{
				Talespire.Log.Debug($"Modifying {basePropertyDto.Name} with \"{basePropertyDto.Value}\"");
				basePropertyDto.ModifyProperty(effect);
			}
		}

		public GameObject CreateOrFindSafe(string instanceId = null, CharacterPosition sourcePosition = null, CharacterPosition targetPosition = null, GameObject parentInstance = null)
		{
			GameObject gameObject = null;
			UnityMainThreadDispatcher.ExecuteOnMainThread(() =>
			{
				gameObject = CreateOrFindUnsafe(instanceId, sourcePosition, targetPosition, parentInstance);
			});
			
			return gameObject;
		}

		private GameObject CreateOrFindUnsafe(string instanceId = null, CharacterPosition sourcePosition = null, CharacterPosition targetPosition = null, GameObject parentInstance = null)
		{
			//if (ExistingChildName != null)
			//	Talespire.Log.Debug($"Finding {ExistingChildName}...");
			//else
			//	Talespire.Log.Debug($"Creating {PrefabToCreate}...");

			this.targetPosition = targetPosition;
			this.sourcePosition = sourcePosition;
			GameObject instance = null;

			if (PrefabToCreate != null)
			{
				Talespire.Log.Debug($"instance = Talespire.Prefabs.Clone(PrefabToCreate, instanceId);");
				instance = Talespire.Prefabs.Clone(PrefabToCreate, instanceId);
				// TODO: Figure out spell effect motion/positioning...
				if (targetPosition != null && instance.transform != null)
					instance.transform.position = targetPosition.Position.GetVector3();
				else
				{
					if (instance.transform == null)
					{
						Talespire.Log.Error($"Prefab {PrefabToCreate}'s instance.transform == null!");
						return null;
					}
				}
			}
			else if (ExistingChildName != null)
				if (parentInstance != null && parentInstance.transform != null)
				{
					Talespire.Log.Debug($"Transform childTransform = parentInstance.transform.Find(ExistingChildName);");
					Transform childTransform = parentInstance.transform.Find(ExistingChildName);
					instance = childTransform?.gameObject;
				}
				else
					Talespire.Log.Debug($"parentInstance == null!");
			else if (ItemToClone != null)
			{
				Talespire.Log.Debug($"instance = Talespire.GameObjects.Clone(ItemToClone, instanceId);");
				instance = Talespire.GameObjects.Clone(ItemToClone, instanceId);
				// TODO: Figure out spell effect motion/positioning...
				if (targetPosition != null && instance.transform != null)
					instance.transform.position = targetPosition.Position.GetVector3();
				else
				{
					if (instance.transform == null)
					{
						Talespire.Log.Error($"Clone {ItemToClone}'s instance.transform == null!");
						return null;
					}
				}
			}
			else if (ItemToBorrow != null)
			{
				Talespire.Log.Debug($"instance = Talespire.GameObjects.Get(ItemToBorrow);");
				instance = Talespire.GameObjects.Get(ItemToBorrow);
			}
			else if (EffectNameToCreate != null)
			{
				if (EffectsBuilder != null)
				{
					Talespire.Log.Debug($"instance = EffectsBuilder.Create(EffectNameToCreate, instanceId);");
					instance = EffectsBuilder.Create(EffectNameToCreate, instanceId);
				}
			}

			Talespire.Log.Debug($"Creation complete!");

			if (instance != null)
			{
				Talespire.Log.Debug($"ModifyProperties(instance)....");
				ModifyProperties(instance);
			}

			Talespire.Log.Debug($"Repeat with children...");
			if (Children != null)
				foreach (CompositeEffect nestedEffectDto in Children)
					if (nestedEffectDto != null)
						nestedEffectDto.CreateOrFindUnsafe(instanceId, sourcePosition, targetPosition, instance);
					else
						Talespire.Log.Error($"nestedEffectDto == null!");

			return instance;
		}

		public static CompositeEffect CreateFrom(string json, string instanceId = null)
		{
			CompositeEffect compositeEffect = JsonConvert.DeserializeObject<CompositeEffect>(json);
			compositeEffect.RebuildPropertiesAfterLoad();
			return compositeEffect;
		}
	}
}
