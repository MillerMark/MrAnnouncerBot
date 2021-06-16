﻿/*
Copyright 2015 Pim de Witte All Rights Reserved.
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
	http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace TaleSpireCore
{
	/// Author: Pim de Witte (pimdewitte.com) and contributors, https://github.com/PimDeWitte/UnityMainThreadDispatcher
	/// <summary>
	/// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for
	/// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
	/// </summary>
	public class UnityMainThreadDispatcher : MonoBehaviour
	{

		private static readonly Queue<Action> _executionQueue = new Queue<Action>();

		public void Update()
		{
			lock (_executionQueue)
			{
				while (_executionQueue.Count > 0)
				{
					_executionQueue.Dequeue().Invoke();
				}
			}
		}

		/// <summary>
		/// Locks the queue and adds the IEnumerator to the queue
		/// </summary>
		/// <param name="action">IEnumerator function that will be executed from the main thread.</param>
		public void Enqueue(IEnumerator action)
		{
			lock (_executionQueue)
			{
				_executionQueue.Enqueue(() =>
				{
					StartCoroutine(action);
				});
			}
		}

		/// <summary>
		/// Locks the queue and adds the Action to the queue
		/// </summary>
		/// <param name="action">function that will be executed from the main thread.</param>
		public void Enqueue(Action action)
		{
			Enqueue(ActionWrapper(action));
		}

		/// <summary>
		/// Locks the queue and adds the Action to the queue, returning a Task which is completed when the action completes
		/// </summary>
		/// <param name="action">function that will be executed from the main thread.</param>
		/// <returns>A Task that can be awaited until the action completes</returns>
		public Task EnqueueAsync(Action action)
		{
			var tcs = new TaskCompletionSource<bool>();

			void WrappedAction()
			{
				try
				{
					action();
					tcs.TrySetResult(true);
				}
				catch (Exception ex)
				{
					tcs.TrySetException(ex);
				}
			}

			Enqueue(ActionWrapper(WrappedAction));
			return tcs.Task;
		}


		IEnumerator ActionWrapper(Action a)
		{
			a();
			yield return null;
		}


		private static UnityMainThreadDispatcher _instance = null;

		public static bool Exists()
		{
			return _instance != null;
		}

		public static UnityMainThreadDispatcher Instance
		{
			get
			{
				if (!Exists())
				{
					throw new Exception("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
				}
				return _instance;
			}
		}

		List<UnityMainThreadDispatcher> knownInstances = new List<UnityMainThreadDispatcher>();
		void AddSelfToList()
		{
			knownInstances.Add(this);
		}

		void Awake()
		{
			AddSelfToList();
			//Talespire.Log.Debug($"UnityMainThreadDispatcher.knownInstances = {knownInstances.Count}");
			//Talespire.Log.Debug($"UnityMainThreadDispatcher.Awake!!!");
			//Talespire.Log.Debug($"_instance = this;");
			
			// Use the most recent value:
			_instance = this;
			DontDestroyOnLoad(this.gameObject);
		}

		void OnDestroy()
		{
			//Talespire.Log.Debug($"UnityMainThreadDispatcher.OnDestroy");
			knownInstances.Remove(this);
			_instance = null;
			if (knownInstances.Count > 0)
			{
				//Talespire.Log.Warning($"We still have instances!!! Let's use the last one");
				_instance = knownInstances[knownInstances.Count - 1];
			}
		}

		public static void ExecuteOnMainThread(Action action)
		{
			if (Instance == null)
			{
				Talespire.Log.Error($"No instances of UnityMainThreadDispatcher found!!! Unable to execute action on main thread!");
				return;
			}
			ManualResetEvent mre = new ManualResetEvent(false);
			Instance.Enqueue(() =>
			{
				action();
				mre.Set();
			});
			mre.WaitOne();
		}

		//public void WakeUp()
		//{
		//	Talespire.Log.Debug($"UnityMainThreadDispatcher.WakeUp!");
		//	Awake();
		//}
	}
}
