//#define profiling
using DndCore;
using Serilog;
using System;
using System.Linq;

namespace DHDM
{
	[ParameterCompletion(typeof(AddSound), ParameterNames.AddSound_FileName)]
	public class SoundFileCompletionEditor //: ICompletionEditor
	{

		public SoundFileCompletionEditor()
		{

		}
	}
}
