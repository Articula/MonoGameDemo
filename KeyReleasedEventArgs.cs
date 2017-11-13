using System;
using Microsoft.Xna.Framework.Input;
namespace MonoGameDemo
{
	public class KeyReleasedEventArgs : EventArgs
	{
		public KeyReleasedEventArgs(Keys key)
		{
			this.key = key;
		}

		public Keys key { get; }
	}
}
