using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Management.Instrumentation;

namespace TaleSpireCore
{
	public class CompositeEffect
	{
		/// <summary>
		/// <see cref="refreshableProperties"></see> is a list of properties that, if changed, would require a refresh of the parent GameObject.
		/// For example, "&lt;ParticleSystem&gt;.main.prewarm".
		/// </summary>
		static List<string> refreshableProperties = new List<string>();
		static List<GameObject> objectsToRefresh = new List<GameObject>();
		static List<GameObject> objectsToActivate = new List<GameObject>();

		static CompositeEffect()
		{
			refreshableProperties.Add("<ParticleSystem>.main.prewarm");
		}
		public static IKnownEffectsBuilder KnownEffectsBuilder { get; set; }

		/// <summary>
		/// The seconds to delay awakening this effect, in seconds.
		/// </summary>
		[DefaultValue(0)]
		public float DelayStart { get; set; }

		[DefaultValue(false)]
		public bool SuppressPosition { get; set; }

		[DefaultValue(false)]
		public bool SuppressRotation { get; set; }

		[DefaultValue(false)]
		public bool SuppressScale { get; set; }

		/// <summary>
		/// The name of the prefab effect to create at this location in the hierarchy.
		/// Leave empty to create an empty game object.
		/// </summary>
		[DefaultValue(null)]
		public string PrefabToCreate { get; set; }

		[DefaultValue(null)]
		public string ExistingChildName { get; set; }

		[DefaultValue(null)]
		public string ItemToClone { get; set; }

		/// <summary>
		/// If true, then the effect will be hidden when the base is hidden.
		/// </summary>
		[DefaultValue(false)]
		public bool VisibilityMatchesBase { get; set; }

		/// <summary>
		/// The list of all the properties needing changing in this composite effect.
		/// </summary>
		[DefaultValue(null)]
		public List<PropertyChangerDto> Props { get; set; }


		/// <summary>
		/// Any child effects. The structure of this effect composite needs to match the structure of the prefab.
		/// Empty game objects and other prefabs can parent this prefab/game object or be children to this effect.
		/// </summary>
		[DefaultValue(null)]
		public List<CompositeEffect> Children { get; set; }

		/// <summary>
		/// The list of SmartProperty elements created for this effect.
		/// </summary>
		[DefaultValue(null)]
		public List<SmartProperty> SmartProperties { get; set; }


		// TODO: Change Scripts to a more sophisticate class, to include property settings.

		/// <summary>
		/// Scripts to add to this composite effect (added before setting properties).
		/// </summary>
		[DefaultValue(null)]
		public List<string> Scripts { get; set; }


		/// <summary>
		/// Private list of properties to change. This is where the real work (changing properties) is done.
		/// The public "Props" property is just for serialization/deserialization.
		/// </summary>
		List<BasePropertyChanger> properties { get; set; }

		[JsonIgnore]
		public CreatureBoardAsset Mini { get; set; }

		[DefaultValue(null)]
		public string ItemToBorrow { get; set; }

		[DefaultValue(null)]
		public string EffectNameToCreate { get; set; }


		CharacterPosition sourcePosition;
		CharacterPosition targetPosition;
		bool needRefresh;

		public CompositeEffect()
		{

		}

		public bool NeedsRefreshing()
		{
			if (needRefresh)
				return true;
			
			if (Children != null)
				foreach (CompositeEffect compositeEffect in Children)
					if (compositeEffect.NeedsRefreshing())
						return true;

			return false;
		}

		/// <summary>
		/// Call after deserialization...
		/// </summary>
		public void RebuildProperties()
		{
			//Talespire.Log.Indent();
			//LogThis();

			if (Props != null)
			{
				properties = new List<BasePropertyChanger>();

				foreach (PropertyChangerDto propertyChangerDto in Props)
				{
					if (string.IsNullOrWhiteSpace(propertyChangerDto.FullPropertyPath))
					{
						Talespire.Log.Error($"propertyChangerDto.FullPropertyPath for type {propertyChangerDto.Type} with value {propertyChangerDto.Value} is null or empty!");
						continue;
					}
					Talespire.Log.Debug($"{propertyChangerDto.FullPropertyPath}: {propertyChangerDto.Type};");
					BasePropertyChanger changer = PropertyChangerFactory.CreateFrom(propertyChangerDto);
					if (changer != null)
					{
						Talespire.Log.Debug($"Found property changer ({propertyChangerDto.Type})!");
						changer.FullPropertyPath = propertyChangerDto.FullPropertyPath;
						changer.Value = propertyChangerDto.Value;
						properties.Add(changer);
					}
				}
			}

			if (Children != null)
				foreach (CompositeEffect compositeEffect in Children)
					compositeEffect.RebuildProperties();

			//Talespire.Log.Unindent();
		}

		private void LogThis()
		{
			if (PrefabToCreate != null)
				Talespire.Log.Warning($"PrefabToCreate: {PrefabToCreate}");
			else if (EffectNameToCreate != null)
				Talespire.Log.Warning($"EffectNameToCreate: {EffectNameToCreate}");
			else if (ItemToBorrow != null)
				Talespire.Log.Warning($"ItemToBorrow: {ItemToBorrow}");
			else if (ItemToClone != null)
				Talespire.Log.Warning($"ItemToClone: {ItemToClone}");
			else if (ExistingChildName != null)
				Talespire.Log.Warning($"ExistingChildName: {ExistingChildName}");
		}

		void RemoveExistingProperty(string name)
		{
			BasePropertyChanger propertyChanger = properties.FirstOrDefault(x => x.FullPropertyPath == name);
			if (propertyChanger != null)
				properties.Remove(propertyChanger);

			PropertyChangerDto propDto = Props.FirstOrDefault(y => y.FullPropertyPath == name);
			if (propDto != null)
				Props.Remove(propDto);
		}

		public void AddProperty(BasePropertyChanger propertyChanger)
		{
			InitializePropertyVars();
			RemoveExistingProperty(propertyChanger.FullPropertyPath);
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
				if (string.IsNullOrWhiteSpace(basePropertyDto.FullPropertyPath))
				{
					Talespire.Log.Error($"basePropertyDto.FullPropertyPath is null or empty.");
					continue;
				}
#pragma warning disable CS0253  // Intentional reference comparison.
				if (basePropertyDto.ValueOverride != null && basePropertyDto.Value != basePropertyDto.ValueOverride)
				{
					Talespire.Log.Error($"ValueOverride == \"{basePropertyDto.ValueOverride}\", but Value == \"{basePropertyDto.Value}\"");
					Talespire.Log.Warning($">> Setting {basePropertyDto.FullPropertyPath} to \"{basePropertyDto.ValueOverride}\"");
				}
				else
					Talespire.Log.Warning($">> Setting {basePropertyDto.FullPropertyPath} to \"{basePropertyDto.Value}\"");

				if (refreshableProperties.Contains(basePropertyDto.FullPropertyPath))
					needRefresh = true;

				basePropertyDto.ModifyProperty(effect);
			}
		}

		public GameObject CreateOrFindSafe(string instanceId = null, CharacterPosition sourcePosition = null, CharacterPosition targetPosition = null, GameObject parentInstance = null)
		{
			GameObject gameObject = null;
			UnityMainThreadDispatcher.ExecuteOnMainThread(() =>
			{
				gameObject = CreateOrFindUnsafe(instanceId, sourcePosition, targetPosition, parentInstance);
				RefreshIfNecessary(gameObject);
			});

			return gameObject;
		}

		static Dictionary<GameObject, CompositeEffect> compositeEffectMap = new Dictionary<GameObject, CompositeEffect>();

		[Browsable(false)]
		public GameObject CreateOrFindUnsafe(string instanceId = null, CharacterPosition sourcePosition = null, CharacterPosition targetPosition = null, GameObject parentInstance = null)
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
				Talespire.Log.Debug($"instance = Talespire.Prefabs.Clone(\"{PrefabToCreate}\", instanceId);");
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
					Talespire.Log.Debug($"Transform childTransform = parentInstance.transform.Find(\"{ExistingChildName}\");");
					Transform childTransform = parentInstance.transform.Find(ExistingChildName);
					instance = childTransform?.gameObject;
				}
				else
					Talespire.Log.Debug($"parentInstance == null!");
			else if (ItemToClone != null)
			{
				Talespire.Log.Debug($"instance = Talespire.GameObjects.Clone(\"{ItemToClone}\", instanceId);");
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
				Talespire.Log.Debug($"instance = Talespire.GameObjects.Get(\"{ItemToBorrow}\");");
				instance = Talespire.GameObjects.Get(ItemToBorrow);
			}
			else if (EffectNameToCreate != null)
			{
				if (KnownEffectsBuilder != null)
				{
					Talespire.Log.Debug($"instance = KnownEffectsBuilder.Create(\"{EffectNameToCreate}\", instanceId);");
					instance = KnownEffectsBuilder.Create(EffectNameToCreate, instanceId);
				}
			}

			//Talespire.Log.Debug($"Creation complete!");

			if (instance != null)
			{
				//Talespire.Log.Debug($"ModifyProperties(instance)....");
				ModifyProperties(instance);
			}

			//Talespire.Log.Debug($"Repeat with children...");
			if (Children != null)
				foreach (CompositeEffect nestedEffectDto in Children)
					if (nestedEffectDto != null)
						nestedEffectDto.CreateOrFindUnsafe(instanceId, sourcePosition, targetPosition, instance);
					else
						Talespire.Log.Error($"nestedEffectDto == null!");

			if (instance != null)
			{
				//Talespire.Log.Warning($"compositeEffectMap[{instance}] = this ({this});");
				compositeEffectMap[instance] = this;
			}
			else
				Talespire.Log.Error($"Unable to map newly-created composite effect to instance {instance}.");

			// TODO: Worried about a slow memory leak here. If we can hook a unity event for when GOs are destroyed, we could plug the leak by removing those elements from compositeEffectMap.

			return instance;
		}

		public static CompositeEffect GetFromGameObject(GameObject gameObject)
		{
			return compositeEffectMap.ContainsKey(gameObject) ? compositeEffectMap[gameObject] : null;
		}

		public static GameObject CreateKnownEffect(string effectName, string instanceId = null)
		{
			if (KnownEffectsBuilder != null)
			{
				EffectParameters.GetEffectNameAndParameters(ref effectName, out string parameters);
				GameObject result = KnownEffectsBuilder.Create(effectName, instanceId);
				EffectParameters.ApplyAfterCreation(result, parameters);
				return result;
			}
			else
				Talespire.Log.Error($"KnownEffectsBuilder == null!!!");
			return null;
		}
		
		public static CompositeEffect CreateFrom(string json, string instanceId = null)
		{
			//Talespire.Log.Indent("CompositeEffect.CreateFrom");
			try
			{
				CompositeEffect compositeEffect = JsonConvert.DeserializeObject<CompositeEffect>(json);
				compositeEffect.RebuildProperties();

				return compositeEffect;
			}
			finally
			{
				//Talespire.Log.Unindent();
			}
		}

		public void RefreshIfNecessary(GameObject gameObject)
		{
			if (gameObject == null)
				return;

			if (NeedsRefreshing() && gameObject.activeSelf)
				objectsToRefresh.Add(gameObject);
		}

		public static void Update()
		{
			foreach (GameObject gameObject in objectsToActivate)
				gameObject.SetActive(true);

			objectsToActivate.Clear();

			foreach (GameObject gameObject in objectsToRefresh)
			{
				gameObject.SetActive(false);
				objectsToActivate.Add(gameObject);
			}
			
			objectsToRefresh.Clear();
		}

		public bool HasSmartProperty(string fullPropertyName)
		{
			if (SmartProperties != null)
				foreach (SmartProperty smartProperty in SmartProperties)
					if (smartProperty.PropertyPaths.Contains(fullPropertyName))
						return true;

			return false;
		}

		public SmartProperty GetSmartProperty(string fullPropertyPath)
		{
			if (fullPropertyPath == null || SmartProperties == null)
				return null;

			foreach (SmartProperty smartProperty in SmartProperties)
				if (smartProperty.PropertyPaths.Contains(fullPropertyPath))
					return smartProperty;

			return null;
		}

		public bool SmartPropertyNameExists(string newName)
		{
			if (SmartProperties == null)
				return false;

			return SmartProperties.Any(smartProperty => smartProperty.Name == newName);
		}

		public void DisconnectProperty(string fullPropertyPath)
		{
			if (fullPropertyPath == null)
				return;

			SmartProperty smartProperty = GetSmartProperty(fullPropertyPath);
			if (smartProperty == null)
				return;

			smartProperty.PropertyPaths.Remove(fullPropertyPath);
		}
		
		public SmartProperty GetSmartPropertyByName(string smartPropertyName)
		{
			if (SmartProperties == null)
				return null;

			return SmartProperties.FirstOrDefault(x => x.Name == smartPropertyName);
		}

		public void AddScript(string scriptName)
		{
			if (Scripts == null)
				Scripts = new List<string>();

			Scripts.Add(scriptName);
		}
	}
}
