using System;
namespace MonoGameDemo
{
    public class InputPressedEventArgs : EventArgs
	{
		public InputPressedEventArgs(InputAction action)
		{
			this.action = action;
		}

		public InputAction action { get; }
	}
}
