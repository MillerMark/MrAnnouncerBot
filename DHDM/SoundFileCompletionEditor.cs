//#define profiling
using DndCore;
using Serilog;
using System;
using System.Linq;

namespace DHDM
{
	[ParameterCompletion(typeof(AddSound), 1)]
	public class SoundFileCompletionEditor //: ICompletionEditor
	{

		public SoundFileCompletionEditor()
		{

		}
	}
}
