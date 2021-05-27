using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace TaleSpireCore
{
	public class CompositeEffect
	{
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

			if (Props == null)
				return;

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
				basePropertyDto.ModifyProperty(effect);
		}

		public GameObject Create(string instanceId, CharacterPosition sourcePosition = null, CharacterPosition targetPosition = null, GameObject parentInstance = null)
		{
			this.targetPosition = targetPosition;
			this.sourcePosition = sourcePosition;
			GameObject instance = null;

			if (PrefabToCreate != null)
			{
				instance = Talespire.Prefabs.Clone(PrefabToCreate, instanceId);
			}
			else if (ExistingChildName != null && parentInstance != null)
			{
				Transform childTransform = parentInstance.transform.Find(ExistingChildName);
				instance = childTransform?.gameObject;
			}

			if (instance != null)
			{
				if (targetPosition != null)
					instance.transform.position = targetPosition.Position.GetVector3();
				ModifyProperties(instance);
			}

			if (Children != null)
				foreach (CompositeEffect nestedEffectDto in Children)
					nestedEffectDto.Create(instanceId, sourcePosition, targetPosition, instance);

			return instance;
		}
	}
}
