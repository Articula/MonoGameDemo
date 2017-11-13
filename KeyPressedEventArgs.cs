using System;
using Microsoft.Xna.Framework.Input;
namespace MonoGameDemo
{
	public class KeyPressedEventArgs : EventArgs
	{
		public KeyPressedEventArgs(Keys key)
		{
			this.key = key;
		}

		public Keys key { get; }
	}
}
