using System;
namespace MonoGameDemo
{
	public class InputReleasedEventArgs : EventArgs
	{
		public InputReleasedEventArgs(InputAction action)
		{
			this.action = action;
		}

		public InputAction action { get; }
	}
}
