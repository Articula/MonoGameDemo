using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace MonoGameDemo
{
	public class KeyboardMonitor
	{
		public event EventHandler<KeyPressedEventArgs> KeyPressedEvent;
		public event EventHandler<KeyReleasedEventArgs> KeyReleasedEvent;
		private static KeyboardMonitor instance;
		private List<Keys> lastActiveKeys = new List<Keys>();

		private KeyboardMonitor() {}

		public static KeyboardMonitor Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new KeyboardMonitor();
				}

				return instance;
			}
		}

		public List<Keys> Update()
		{
			KeyboardState ks = Keyboard.GetState();
			List<Keys> currentKeys = ks.GetPressedKeys().ToList();

			var releasedKeys = lastActiveKeys.Except(currentKeys);
			var pressedKeys = currentKeys.Except(lastActiveKeys);

			foreach (Keys key in releasedKeys)
			{
				this.KeyReleasedEvent.Invoke(this, new KeyReleasedEventArgs(key));
			}

			foreach (Keys key in pressedKeys)
			{
				this.KeyPressedEvent.Invoke(this, new KeyPressedEventArgs(key));
			}

			lastActiveKeys = currentKeys;
			return currentKeys;
		}
	}
}
