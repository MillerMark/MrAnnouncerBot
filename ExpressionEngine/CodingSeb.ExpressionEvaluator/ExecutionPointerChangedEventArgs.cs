/******************************************************************************************************
    Title : ExpressionEvaluator (https://github.com/codingseb/ExpressionEvaluator)
    Version : 1.4.4.0 
    (if last digit (the forth) is not a zero, the version is an intermediate version and can be unstable)

    Author : Mark Miller
    License : MIT (https://github.com/codingseb/ExpressionEvaluator/blob/master/LICENSE.md)
*******************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace CodingSeb.ExpressionEvaluator
{
	public delegate void ExecutionPointerChangedHandler(object sender, ExecutionPointerChangedEventArgs ea);
	public class ExecutionPointerChangedEventArgs : EventArgs
	{
		public ExecutionPointerChangedEventArgs()
		{
		}

		public void Set(string originalScript, int instructionPointer, string script, Stack<StackFrame> stackFrames)
		{
			StackFrames = stackFrames;
			Code = script;
			OriginalScript = originalScript;
			GiveUpOnThisInstructionPointer = instructionPointer;
		}

		public int GiveUpOnThisInstructionPointer { get; set; }
		public string OriginalScript { get; set; }
		public string Code { get; set; }
		public Stack<StackFrame> StackFrames { get; set; }
	}
}
