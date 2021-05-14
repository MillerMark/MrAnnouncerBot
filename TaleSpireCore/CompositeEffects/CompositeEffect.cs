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
		public string Prefab { get; set; }
		
		
		/// <summary>
		/// The list of all the properties needing changing in this composite effect.
		/// </summary>
		public List<PropertyChangerDto> Props { get; set; }


		/// <summary>
		/// Any child effects. The structure of this effect composite needs to match the structure of the prefab.
		/// Empty game objects and other prefabs can parent this prefab/game object or be children to this effect.
		/// </summary>
		public List<CompositeEffect> Children { get; set; }


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
		}
		public void AddProperty(BasePropertyChanger propertyChanger)
		{
			
			if (properties == null)
				properties = new List<BasePropertyChanger>();
			if (Props == null)
				Props = new List<PropertyChangerDto>();

			properties.Add(propertyChanger);
			Props.Add(propertyChanger.ToPropertyChangerDto());
		}

		void ModifyProperties(GameObject effect)
		{
			if (properties == null)
				return;

			foreach (var basePropertyDto in properties)
				basePropertyDto.ModifyProperty(effect);
		}

		public void Create(CharacterPosition sourcePosition, CharacterPosition targetPosition, string instanceId)
		{
			this.targetPosition = targetPosition;
			this.sourcePosition = sourcePosition;
			GameObject effect = Talespire.Prefabs.Clone(Prefab, instanceId);

			effect.transform.position = targetPosition.Position.GetVector3();
			ModifyProperties(effect);
			if (Children != null)
				foreach (CompositeEffect nestedEffectDto in Children)
					nestedEffectDto.Create(sourcePosition, targetPosition, instanceId);
		}
	}
}
